using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip DungeonOneMusic;
    public AudioSource source;
    private AudioClip nextSong;
    private AudioClip currentSong;
    public AudioClip FactoryForemanMusic;
    public bool backToMain;
    private bool fading;
    private float fadeSpeed;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (fading)
        {
            source.volume = Mathf.Lerp(source.volume, -1, Time.deltaTime);
            if(source.volume < 0.05f)
            {
                source.Stop();
                source.clip = nextSong;
                source.volume = .58f;
                source.Play();
                fading = false;
            }
        }
    }

    public void ChangeSong( AudioClip song, bool fadeOut = true, float fadeTime = 1)
    {
        if (fadeOut)
        {
            fadeSpeed = fadeTime;
            nextSong = song;
            fading = true;
        }
        else
        {
            source.Stop();
            source.clip = song;
            source.volume = .58f;
            source.Play();
            fading = false;
        }
    }
}
