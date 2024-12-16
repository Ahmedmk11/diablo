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
    [SerializeField] private AudioClip earthquakeSFX;

    private Dictionary<string, AudioClip> audios;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            sfxSource = gameObject.AddComponent<AudioSource>();

            audios = new Dictionary<string, AudioClip>
            {
                { "mainMenuMusic", mainMenuMusic },
                { "bosslevelMusic", bosslevelMusic },
                { "baselevelMusic", baselevelMusic },
                { "healingSFX", healingSFX },
                { "abilitySFX", abilitySFX },
                { "dashSFX", dashSFX },
                { "playerDeathSFX", playerDeathSFX },
                { "bossDeathSFX", bossDeathSFX },
                { "bossAuraSFX", bossAuraSFX },
                { "explosionSFX", explosionSFX },
                { "fireballSFX", fireballSFX },
                { "itemPickupSFX", itemPickupSFX },
                { "summonSFX", summonSFX },
                { "bossHitSFX", bossHitSFX },
                { "arrowHitSFX", arrowHitSFX },
                { "enemyDeathSFX", enemyDeathSFX },
                { "playerHitSFX", playerHitSFX },
                { "bossSpikesSFX", bossSpikesSFX },
                { "earthquakeSFX", earthquakeSFX }
            };

            PlayMusic("mainMenuMusic");
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

    public void PlayMusic(string musicString)
    {
        musicSource.Stop();
        AudioClip music = audios[musicString];
        musicSource.clip = music;
        musicSource.Play();
    }

    public void PlaySFX(string sfxString)
    {
        AudioClip sfx = audios[sfxString];
        sfxSource.PlayOneShot(sfx);
    }
}
