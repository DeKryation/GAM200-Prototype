using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool _loading;
    private double _startTime;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
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
    }

    private void Update()
    {
        if (_loading || !allowSkipWithAnyKey) return;

        if (Time.unscaledTimeAsDouble - _startTime >= minSecondsBeforeSkip && Input.anyKeyDown)
        {
            // Optional skip
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

        SceneManager.LoadScene(nextSceneName);
    }
}
