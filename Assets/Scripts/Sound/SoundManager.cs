using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public SoundLibrary[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public static float musicVolume = 1f, sfxVolume = 1f;
    [SerializeField] private int sfxSourceCount = 5;
    private List<AudioSource> sfxSourcePool;
    private int currentSFXIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        sfxSourcePool = new List<AudioSource>();
        for (int i = 0; i < sfxSourceCount; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxSourcePool.Add(src);
        }
    }

    private void Start()
    {
        PlayMusic("BGMLevel1");
    }

    public void PlayMusic(string name)
    {
        SoundLibrary s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.volume = musicVolume * s.volume;
            musicSource.pitch = s.pitch;
            musicSource.loop = s.loop;
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        SoundLibrary s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log($"SFX '{name}' not found!");
            return;
        }

        AudioSource src = sfxSourcePool[currentSFXIndex];
        src.volume = sfxVolume * s.volume;
        src.pitch = s.pitch;
        src.loop = false;
        src.PlayOneShot(s.clip);

        currentSFXIndex = (currentSFXIndex + 1) % sfxSourcePool.Count;
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
        musicVolume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
        sfxVolume = volume;
    }
    public IEnumerator FadeOutTrack(float timeToFade)
    {

        float timeElapsed = 0;
        float b = musicSource.volume;

        while (timeElapsed < timeToFade)
        {
            Debug.Log(musicSource.volume);
            musicSource.volume = b * Mathf.Lerp(1, 0, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = b;
    }

    public void FadeOut(float timeToFade)
    {
        StartCoroutine(FadeOutTrack(timeToFade));
    }
}
