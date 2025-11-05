using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneEndLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer; // assign, or it will auto-GetComponent

    [Header("Flow")]
    [SerializeField] private string nextSceneName = "Edris' DS Scene";
    [SerializeField] private float extraDelayAfter = 0f; // seconds after video ends

    [Header("Skip (optional)")]
    [SerializeField] private bool allowSkipWithAnyKey = true;
    [SerializeField] private float minSecondsBeforeSkip = 0.5f; // small grace so you don’t insta-skip

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; // full-screen black image with CanvasGroup
    [SerializeField] private float fadeDuration = 0.6f;   // seconds
    [SerializeField] private bool createFadeIfMissing = true; // auto-make a simple fader if none assigned

    private bool _loading;
    private double _startTime;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (fadeCanvasGroup == null && createFadeIfMissing)
            fadeCanvasGroup = EnsureRuntimeFader();
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void Start()
    {
        _startTime = Time.unscaledTimeAsDouble;

        // If your VideoPlayer isn’t set to Play On Awake, uncomment this:
        // if (videoPlayer != null && !videoPlayer.isPlaying) videoPlayer.Play();

        // make sure we start clear
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (_loading || !allowSkipWithAnyKey) return;

        if (Time.unscaledTimeAsDouble - _startTime >= minSecondsBeforeSkip && Input.anyKeyDown)
        {
            EndNow();
        }
    }

    private void OnVideoFinished(VideoPlayer _)
    {
        if (_loading) return;
        StartCoroutine(LoadNextAfterDelay());
    }

    private void EndNow()
    {
        if (_loading) return;
        if (videoPlayer != null && videoPlayer.isPlaying) videoPlayer.Stop();
        StartCoroutine(LoadNextAfterDelay());
    }

    private IEnumerator LoadNextAfterDelay()
    {
        _loading = true;

        if (extraDelayAfter > 0f)
            yield return new WaitForSecondsRealtime(extraDelayAfter);

        // Fade to black, then load
        if (fadeCanvasGroup != null && fadeDuration > 0f)
            yield return StartCoroutine(FadeTo(1f, fadeDuration));

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = fadeCanvasGroup.alpha;
        float t = 0f;

        // ensure it sits on top & blocks clicks during fade
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = false;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, target, Mathf.Clamp01(t / duration));
            fadeCanvasGroup.alpha = a;
            yield return null;
        }

        fadeCanvasGroup.alpha = target;
    }

    // Creates a basic full-screen black fader at runtime if none is assigned.
    private CanvasGroup EnsureRuntimeFader()
    {
        // Canvas root
        var canvasGO = new GameObject("RuntimeFadeCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767; // very top

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Child image with CanvasGroup
        var imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(canvasGO.transform, false);

        var rt = imgGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var img = imgGO.AddComponent<Image>();
        img.color = Color.black;

        var cg = imgGO.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;


        return cg;
    }
}
