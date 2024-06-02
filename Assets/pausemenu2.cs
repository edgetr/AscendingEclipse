using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pausemenu2 : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu2;

    private void Start()
    {
        if (pauseMenu2 == null)
        {
            Debug.LogError("PauseMenu1 GameObject is not assigned in the inspector.");
        }
    }

    public void Pause()
    {
        if (pauseMenu2 != null)
        {
            pauseMenu2.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("StartScreen");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        if (pauseMenu2 != null)
        {
            pauseMenu2.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
