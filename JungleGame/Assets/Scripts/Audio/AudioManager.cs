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

    // plays a sound once
    public void PlayFX_oneShot(AudioClip clip, float volume, string id = "fx_oneShot")
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clip, volume, clip.length + 1f);
    }

    // plays a sound continuously until scene changes or stopped manually
    public void PlayFX_loop(AudioClip clip, float volume, string id = "fx_loop")
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clip, volume, 0f);
    }

    // stops fx from plaing based on id
    public void StopFX(string id)
    {
        // iterate though each FX audio object
        int num = fxObjectHolder.childCount;
        for (int idx = 0; idx < num; idx++)
        {
            // if the id matches - destroy object
            var obj = fxObjectHolder.GetChild(idx).GetComponent<FxAudioObject>();
            if (obj.id == id)
            {
                obj.InstaDestroy();
                return;
            }
        }
    }

    // clear all fx
    public void ClearAllFX()
    {
        // iterate though each FX audio object and destory each one
        int num = fxObjectHolder.childCount;
        for (int idx = num - 1; idx >= 0; idx--)
        {
            var obj = fxObjectHolder.GetChild(idx).GetComponent<FxAudioObject>();
            obj.InstaDestroy();
        }
    }

    public void PlayCoinDrop(int num = 0)
    {
        // play random coin drop sound iff 0
        if (num == 0)
        {
            int idx = Random.Range(0, 8);
            PlayFX_oneShot(AudioDatabase.instance.CoinDropArray[idx], 1f);
        }
        else
        {
            if (num > 0 && num < 8)
            PlayFX_oneShot(AudioDatabase.instance.CoinDropArray[num], 1f);
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
