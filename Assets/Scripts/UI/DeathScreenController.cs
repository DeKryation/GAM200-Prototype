using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject deathScreenPanel;   // Panel with your message + buttons
    [SerializeField] private Button defaultSelectedButton;  // (optional) first selected for gamepad

    private Damageable playerDamageable;

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
            playerDamageable.damageableDeath.AddListener(ShowDeathScreen);
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
            playerDamageable.damageableDeath.RemoveListener(ShowDeathScreen);
    }

    private void ShowDeathScreen()
    {
        if (deathScreenPanel) deathScreenPanel.SetActive(true);
        Time.timeScale = 0f; // pause game

        // (optional) set selected for keyboard/controller navigation
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
