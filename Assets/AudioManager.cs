using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour

{

    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---Audio Clip---")]

    public AudioClip background;
    public AudioClip death;
    public AudioClip attack;
    public AudioClip bossAttack;
    public AudioClip jump;

    private void Awake()
    {
        SFXSource = GetComponent<AudioSource>();
    }
    public void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);

        }
        else
        {
            Debug.LogWarning("Audio clip is null!");
        }
    }






}
