using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    public static CutscenePlayer Instance { get; private set; }

    [Header("Options")]
    [SerializeField] private bool pauseGameTime = false;       // pause gameplay while playing
    [SerializeField] private int overlaySortingOrder = 10000;  // ensure it's above all other UI
    [SerializeField] private AudioSource audioSource;          // optional: route video audio here

    private VideoPlayer vp;
    private Canvas overlayCanvas;
    private RawImage rawImage;
    private RenderTexture rt;
    private bool playing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // --- Build overlay canvas (once) ---
        var goCanvas = new GameObject("CutsceneOverlayCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        overlayCanvas = goCanvas.GetComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = overlaySortingOrder;
        overlayCanvas.enabled = false; // hidden until we play

        // Optional scaler to match reference resolution
        var scaler = goCanvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Make RawImage child filling the screen
        var goImage = new GameObject("CutsceneRawImage", typeof(RectTransform), typeof(RawImage));
        goImage.transform.SetParent(goCanvas.transform, false);
        rawImage = goImage.GetComponent<RawImage>();
        var rtImage = goImage.GetComponent<RectTransform>();
        rtImage.anchorMin = Vector2.zero;
        rtImage.anchorMax = Vector2.one;
        rtImage.offsetMin = Vector2.zero;
        rtImage.offsetMax = Vector2.zero;

        // --- VideoPlayer on this object ---
        vp = gameObject.AddComponent<VideoPlayer>();
        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.renderMode = VideoRenderMode.RenderTexture;

        if (audioSource != null)
        {
            vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
            vp.EnableAudioTrack(0, true);
            vp.SetTargetAudioSource(0, audioSource);
        }
        else
        {
            vp.audioOutputMode = VideoAudioOutputMode.Direct;
        }
    }

    public void Play(VideoClip clip, float extraDelayAfterSeconds, UnityAction onComplete)
    {
        if (playing || clip == null) { onComplete?.Invoke(); return; }
        StartCoroutine(PlayRoutine(clip, extraDelayAfterSeconds, onComplete));
    }

    private IEnumerator PlayRoutine(VideoClip clip, float extraDelayAfterSeconds, UnityAction onComplete)
    {
        playing = true;

        // Create / resize RT to current screen
        AllocateRT(Screen.width, Screen.height);
        vp.targetTexture = rt;
        rawImage.texture = rt;

        vp.clip = clip;
        vp.Prepare();
        while (!vp.isPrepared) yield return null;

        // Show overlay on top of everything
        overlayCanvas.enabled = true;

        float oldScale = Time.timeScale;
        if (pauseGameTime) Time.timeScale = 0f;

        bool finished = false;
        vp.loopPointReached += _ => finished = true;
        vp.Play();

        // Wait until video finishes
        while (!finished) yield return null;

        // Optional padding
        if (extraDelayAfterSeconds > 0f)
            yield return new WaitForSecondsRealtime(extraDelayAfterSeconds);

        if (pauseGameTime) Time.timeScale = oldScale;

        vp.Stop();
        overlayCanvas.enabled = false;

        // Clean up RT to free memory (optional keep if you replay often)
        CleanupRT();

        playing = false;
        onComplete?.Invoke();
    }

    private void AllocateRT(int w, int h)
    {
        if (rt != null && (rt.width == w && rt.height == h)) return;
        CleanupRT();
        // Use a safe format for video
        rt = new RenderTexture(Mathf.Max(2, w), Mathf.Max(2, h), 0, RenderTextureFormat.ARGB32);
        rt.Create();
    }

    private void CleanupRT()
    {
        if (rt != null)
        {
            if (vp != null && vp.targetTexture == rt) vp.targetTexture = null;
            rawImage.texture = null;
            rt.Release();
            Destroy(rt);
            rt = null;
        }
    }
}