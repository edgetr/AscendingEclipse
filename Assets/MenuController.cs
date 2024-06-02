using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static bool gameIsPaused;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public AudioSource theme;

    void Update()
    {

    }

    public void SetQuality(int qual)
    {
        QualitySettings.SetQualityLevel(qual);
    }

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;

    }

    public void SetMusic(bool isMusic)
    {
        theme.mute = !isMusic;

    }
}
