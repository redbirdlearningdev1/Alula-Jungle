using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer masterMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource talkSource;

    [Header("FX Audio Stuff")]
    [SerializeField] private GameObject fxAudioObject;
    [SerializeField] private Transform fxObjectHolder;

    // default volumes (on start)
    public static float default_masterVol =     1f;
    public static float default_musicVol =      0.25f;
    public static float default_fxVol =         1f;
    public static float default_talkVol =       1f;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /* 
    ################################################
    #   VOLUME METHODS
    ################################################
    */
    // make sure value is within range
    // decibel is a logarithmic unit so you can't just apply value directly from the slider

    public void SetMasterVolume(float vol)
    {
        // clamp between 0 and 1
        if (vol <= 0) vol = 0.00001f;
        else if (vol > 1) vol = 1;

        vol = 20f * Mathf.Log10(vol);
        masterMixer.SetFloat("masterVol", vol);
    }

    public void SetMusicVolume(float vol)
    {
        // clamp between 0 and 1
        if (vol <= 0) vol = 0.00001f;
        else if (vol > 1) vol = 1;

        vol = 20f * Mathf.Log10(vol);
        masterMixer.SetFloat("musicVol", vol);
    }

    public void SetFXVolume(float vol)
    {
        // clamp between 0 and 1
        if (vol <= 0) vol = 0.00001f;
        else if (vol > 1) vol = 1;

        vol = 20f * Mathf.Log10(vol); 
        masterMixer.SetFloat("fxVol", vol);
    }

    public void SetTalkVolume(float vol)
    {
        // clamp between 0 and 1
        if (vol <= 0) vol = 0.00001f;
        else if (vol > 1) vol = 1;

        vol = 20f * Mathf.Log10(vol);
        masterMixer.SetFloat("talkVol", vol);
    }

    public float GetMasterVolume()
    {
        float num;
        masterMixer.GetFloat("masterVol", out num);
        num = Mathf.Pow(10f, num / 20f);
        return num;
    }


    public float GetMusicVolume()
    {
        float num;
        masterMixer.GetFloat("musicVol", out num);
        num = Mathf.Pow(10f, num / 20f);
        return num;
    }

    public float GetFxVolume()
    {
        float num;
        masterMixer.GetFloat("fxVol", out num);
        num = Mathf.Pow(10f, num / 20f);
        return num;
    }

    public float GetTalkVolume()
    {
        float num;
        masterMixer.GetFloat("talkVol", out num);
        num = Mathf.Pow(10f, num / 20f);
        return num;
    }


    /* 
    ################################################
    #   MUSIC SOURCE
    ################################################
    */

    public void PlaySong(AudioClip song)
    {
        if (song == musicSource.clip)
            return;

        musicSource.Stop();

        musicSource.clip = song;
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

    // plays a sound once
    public void PlayFX_oneShot(AudioClip clip, float volume, string id = "fx_oneShot", float pitch = 1f)
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clip, volume, clip.length + 1f, pitch);
    }

    // plays a sound continuously until scene changes or stopped manually
    public void PlayFX_loop(AudioClip clip, float volume, string id = "fx_loop", float pitch = 1f)
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clip, volume, 0f, pitch);
    }

    // stops fx from plaing based on id
    public bool StopFX(string id)
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
                return true;
            }
        }
        return false;
    }

    // clear all fx
    public void ClearAllAudio()
    {
        // iterate though each FX audio object and destory each one
        int num = fxObjectHolder.childCount;
        for (int idx = num - 1; idx >= 0; idx--)
        {
            var obj = fxObjectHolder.GetChild(idx).GetComponent<FxAudioObject>();
            obj.InstaDestroy();
        }
        // stop talk source
        talkSource.Stop();
    }

    public void PlayCoinDrop(int num = -1)
    {
        // play random coin drop sound iff -1
        if (num == -1)
        {
            int idx = Random.Range(0, 8);
            PlayFX_oneShot(AudioDatabase.instance.CoinDropArray[idx], 1f);
        }
        else
        {
            if (num >= 0 && num < 8)
            PlayFX_oneShot(AudioDatabase.instance.CoinDropArray[num], 1f);
        }
    }

    public void PlayKeyJingle(int num = -1)
    {
        // play random key jingle sound iff -1
        if (num == -1)
        {
            int idx = Random.Range(0, 6);
            PlayFX_oneShot(AudioDatabase.instance.KeyJingleArray[idx], 1f);
        }
        else
        {
            if (num >= 0 && num < 6)
            PlayFX_oneShot(AudioDatabase.instance.KeyJingleArray[num], 1f);
        }
    }
    
    static int stoneLoopCount = 0;
    public void PlayMoveStoneSound(float duration, float pitch)
    {
        string id = "stone_" + stoneLoopCount.ToString();
        stoneLoopCount++;
        StartCoroutine(MoveStoneRoutine(id, duration, pitch));
    }

    private IEnumerator MoveStoneRoutine(string id, float duration, float pitch)
    {
        PlayFX_loop(AudioDatabase.instance.MoveStoneLoop, 1f, id, pitch);
        yield return new WaitForSeconds(duration);
        if (StopFX(id))
        {
            PlayFX_oneShot(AudioDatabase.instance.MoveStoneEnd, 0.5f, id, pitch);
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

    public void StopTalk()
    {
        talkSource.Stop();
        talkSource.clip = null;
    }

    public void PlayPhoneme(ActionWordEnum phoneme)
    {
        talkSource.Stop();
        AudioClip clip = GameManager.instance.GetActionWord(phoneme).audio;
        talkSource.clip = clip;
        talkSource.loop = false;
        talkSource.Play();
        // print ("playing clip: " + clip.name);
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */
}
