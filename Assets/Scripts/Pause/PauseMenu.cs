using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PausePanel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))       // key input as "Esc" to exit from Pause Menu
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Pause() // pause the game when the button is triggered
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        // Freeze player rigidbody
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var anim = player.GetComponent<Animator>();
            if (anim != null) anim.speed = 0f;
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

    }

    public void Resume()    // To resume the game from pause
    {

        PausePanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var anim = player.GetComponent<Animator>();
            if (anim != null) anim.speed = 1f;
        }
    }

    public void LoadMainMenu()  // Load to Main Menu
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Edris' Scene");
    }
}

    // IF the pause menu requires a quit button function to quit the entire game directly.
    /* public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
    */