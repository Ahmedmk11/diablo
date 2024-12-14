using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiomanager : MonoBehaviour
{
    private static audiomanager instance;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    [SerializeField] private AudioClip mainMenuMusic; //
    [SerializeField] private AudioClip bosslevelMusic; //
    [SerializeField] private AudioClip baselevelMusic; //
    [SerializeField] private AudioClip healingSFX; //
    [SerializeField] private AudioClip abilitySFX; //
    [SerializeField] private AudioClip bossSpikesSFX; //
    [SerializeField] private AudioClip dashSFX; //
    [SerializeField] private AudioClip enemyDeathSFX; //
    [SerializeField] private AudioClip playerHitSFX; //
    [SerializeField] private AudioClip playerDeathSFX; //
    [SerializeField] private AudioClip bossDeathSFX; //
    [SerializeField] private AudioClip bossAuraSFX; //
    [SerializeField] private AudioClip explosionSFX; //
    [SerializeField] private AudioClip fireballSFX; //
    [SerializeField] private AudioClip itemPickupSFX; //
    [SerializeField] private AudioClip summonSFX; //
    [SerializeField] private AudioClip bossHitSFX; //
    [SerializeField] private AudioClip arrowHitSFX; //

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();

            PlayMusic(mainMenuMusic);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioSource GetMusicSource()
    {
        return musicSource;
    }

    public AudioSource GetSFXSource()
    {
        return sfxSource;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip sfx)
    {
        sfxSource.PlayOneShot(sfx);
    }
}
