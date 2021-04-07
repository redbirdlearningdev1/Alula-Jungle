using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioSource talkSource;

    [SerializeField] private AudioDatabase audioDatabase;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /* 
    ################################################
    #   MUSIC SOURCE
    ################################################
    */

    public void PlaySong(Song song)
    {
        musicSource.Stop();
        AudioClip clip = audioDatabase.GetSongFromEnum(song);

        if (clip == musicSource.clip)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    /* 
    ################################################
    #   FX SOURCE
    ################################################
    */


    /* 
    ################################################
    #   TALK SOURCE
    ################################################
    */

    public void PlayTalk(AudioClip _clip)
    {
        talkSource.Stop();
        talkSource.clip = _clip;
        talkSource.loop = false;
        talkSource.Play();
    }

    public void PlayPhoneme(Phoneme phoneme)
    {
        talkSource.Stop();
        AudioClip clip = audioDatabase.GetPhonemeFromEnum(phoneme);
        talkSource.clip = clip;
        talkSource.loop = false;
        talkSource.Play();
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */
}
