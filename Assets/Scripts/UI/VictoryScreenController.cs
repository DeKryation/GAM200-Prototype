using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreenController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button defaultSelectedButton;

    private void Awake()
    {
        if (victoryPanel) victoryPanel.SetActive(false);
    }

    // Hook this to the Door's onVictoryTriggered
    public void ShowVictoryScreen()
    {
        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        if (defaultSelectedButton) defaultSelectedButton.Select();
    }

    // Buttons:
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // fallback
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
