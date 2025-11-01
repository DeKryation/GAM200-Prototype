using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;        // A full-screen black Image (on a Canvas)
    [SerializeField] private float fadeDuration = 1f; // Duration of fade-in/out

    private void Awake()
    {
        // Ensure the fadeImage starts transparent at the beginning of a scene
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        // Optional: Start with a fade-in from black
        if (fadeImage != null)
            StartCoroutine(FadeInFromBlack());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadeImage != null)
        {
            // Fade to black
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // Load the next scene
        SceneManager.LoadScene(sceneName);      // LOAD THE SPECIFIED SCENE SO CHANGE THE NAME OF THE SCENE WHICHEVER U WANT TO LOAD.
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        Color c = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;
            yield return null;
        }
    }

    // Optional: Call this at the start of a scene to fade from black to clear
    public IEnumerator FadeInFromBlack()
    {
        if (fadeImage != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
        }
    }

     // Method to be called when player makes a choice in the cutscene

    // Called when player selects “Yes”
    public void OnYesSelected(string nextSceneName)
    {
        LoadScene(nextSceneName);   // Load the next scene based on the player's choice
    }

    // Called when player selects “No” (e.g. failed the level returns back to the main menu)
    public void OnNoSelected()
    {
        LoadScene("Edris' Scene"); // Make sure this matches your main menu scene name
    }
}