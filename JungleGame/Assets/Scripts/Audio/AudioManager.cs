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
