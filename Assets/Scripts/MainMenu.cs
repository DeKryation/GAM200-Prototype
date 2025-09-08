using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {

    }

    public void GoToSceneLevelOne()              //This is attached to the button in unity
    {
        SceneManager.LoadScene("Lucas' Scene");  //RMB TO UPDATE (NAME) IF SCENE NAME CHANGES
    }

    public void QuitGame()
    {
        Application.Quit();
    } 
}
