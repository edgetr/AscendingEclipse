using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu1 : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu1;

    private void Start()
    {
        if (pauseMenu1 == null)
        {
            Debug.LogError("PauseMenu1 GameObject is not assigned in the inspector.");
        }
    }

    public void Pause()
    {
        if (pauseMenu1 != null)
        {
            pauseMenu1.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Home()
    {
        SceneManager.LoadScene("Level1");
        Time.timeScale = 1;
    }

    public void Resume()
    {
        if (pauseMenu1 != null)
        {
            pauseMenu1.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
