using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource talkSource;

    [Header("FX Audio Stuff")]
    [SerializeField] private GameObject fxAudioObject;
    [SerializeField] private Transform fxObjectHolder;

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

    public void PlaySong(AudioClip song)
    {
        musicSource.Stop();

        if (song == musicSource.clip)
            return;

        musicSource.clip = song;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    public void ChageMusicVolume(float amt)
    {
        if (amt >= 0f && amt <= 1)
            musicSource.volume = amt;
    }

    /* 
    ################################################
    #   FX SOURCE
    ################################################
    */

    public void PlayFX(AudioClip clip, float volume)
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(clip, volume, clip.length + 1f);
    }

    public void PlayCoinDrop(int num = 0)
    {
        // play random coin drop sound iff 0
        if (num == 0)
        {
            int idx = Random.Range(0, 8);
            PlayFX(AudioDatabase.instance.CoinDropArray[idx], 1f);
        }
        else
        {
            if (num > 0 && num < 8)
            PlayFX(AudioDatabase.instance.CoinDropArray[num], 1f);
        }
    }

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

    public void PlayPhoneme(ActionWordEnum phoneme)
    {
        talkSource.Stop();
        AudioClip clip = GameManager.instance.GetActionWord(phoneme).audio;
        talkSource.clip = clip;
        talkSource.loop = false;
        talkSource.Play();
        //print ("playing clip: " + clip.name);
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */
}
