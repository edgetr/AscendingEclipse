using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public void NewGameButton()
    {

        SceneManager.LoadSceneAsync("Level1");
    }

    public void QuitGame()
    {

        Application.Quit();
    }



}
