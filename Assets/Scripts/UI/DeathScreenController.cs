using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject deathScreenPanel;   // Panel with your message + buttons
    [SerializeField] private Button defaultSelectedButton;  // (optional) first selected for gamepad

    [Header("Timing")]
    [SerializeField] private float delaySeconds = 1.0f;     // Delay after death before showing screen

    private Damageable playerDamageable;
    private bool showQueued = false;
    private Coroutine showRoutine;

    private void Awake()
    {
        if (playerDamageable == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerDamageable = player.GetComponent<Damageable>();
        }

        if (deathScreenPanel) deathScreenPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerDamageable != null)
            playerDamageable.damageableDeath.AddListener(QueueDeathScreen);
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
            playerDamageable.damageableDeath.RemoveListener(QueueDeathScreen);

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showQueued = false;
    }

    private void QueueDeathScreen()
    {
        if (showQueued) return; // prevent double-queue
        showQueued = true;
        showRoutine = StartCoroutine(ShowAfterDelay());
    }

    private IEnumerator ShowAfterDelay()
    {
        // Use unscaled time so the delay works even if timeScale changes elsewhere.
        float t = 0f;
        while (t < delaySeconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (deathScreenPanel) deathScreenPanel.SetActive(true);
        Time.timeScale = 0f; // pause game AFTER the delay

        if (defaultSelectedButton != null)
            defaultSelectedButton.Select();
    }

    // Hook this to your Restart button OnClick
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // (optional) Hook to a "Main Menu" button
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // (optional) Quit desktop build
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
