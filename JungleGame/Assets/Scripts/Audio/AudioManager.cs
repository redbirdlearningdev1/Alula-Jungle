using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer masterMixer;

    public float smoothSplitDuration;
    private int currSplitIndex = 0;
    private bool startedSplitSong = false;

    [Header("Addressable Operation Handles")]
    [SerializeField] private List<AsyncOperationHandle> songHandles;
    private AsyncOperationHandle talkHandle;
    [HideInInspector] public List<AssetReference> currentlyLoadedAudioAssets;

    [Header("Audio Sources")]
    [SerializeField] private List<AudioSource> musicSources;
    [SerializeField] private AudioSource talkSource;

    [Header("FX Audio Stuff")]
    [SerializeField] private GameObject fxAudioObject;
    [SerializeField] private Transform fxObjectHolder;

    // default volumes (on start)
    public static float default_masterVol = 1f;
    public static float default_musicVol = 0.25f;
    public static float default_fxVol = 1f;
    public static float default_talkVol = 3f;

    private Coroutine smoothSetMusicRoutine = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        songHandles = new List<AsyncOperationHandle>();
        currentlyLoadedAudioAssets = new List<AssetReference>();
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

    public void ToggleMusicSmooth(bool opt)
    {
        if (opt)
        {
            if (smoothSetMusicRoutine != null)
                StopCoroutine(smoothSetMusicRoutine);
            smoothSetMusicRoutine = StartCoroutine(SmoothStartMusicSource(musicSources[0], 1f));
        }
        else
        {
            if (smoothSetMusicRoutine != null)
                StopCoroutine(smoothSetMusicRoutine);
            smoothSetMusicRoutine = StartCoroutine(SmoothEndMusicSource(musicSources[0], 1f));
        }
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
        // clamp between 0 and 3
        if (vol <= 0) vol = 0.00001f;
        else if (vol > 3) vol = 3;

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

    public void PlaySong(AssetReference songReference)
    {
        StopMusic();

        StartCoroutine(LoadAndPlaySong(songReference));
    }

    private IEnumerator LoadAndPlaySong(AssetReference songReference)
    {
        AsyncOperationHandle songHandle;

        if (songReference.OperationHandle.IsValid())
        {
            songHandle = songReference.OperationHandle;
        }
        else
        {
            songHandle = songReference.LoadAssetAsync<AudioClip>();
        }
        yield return songHandle;

        AudioClip song = (AudioClip)songHandle.Result;
        songHandles.Add(songHandle);

        if (song == musicSources[0].clip)
        {
            yield break;
        }

        musicSources[0].clip = song;
        musicSources[0].loop = true;
        musicSources[0].Play();
    }

    public void StopMusic(bool loop = false, bool setVolumeZero = false)
    {
        foreach (var source in musicSources)
        {
            source.Stop();
            source.clip = null;
            if (loop)
            {
                source.loop = true;
            }
            if (setVolumeZero)
            {
                source.volume = 0f;
            }
        }

        foreach (AsyncOperationHandle handle in songHandles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        songHandles.Clear();

    }

    /* 
    ################################################
    #   SPLIT MUSIC CONTROLLER
    ################################################
    */



    public void InitSplitSong(List<AssetReference> songReferences)
    {
        currSplitIndex = 0;
        startedSplitSong = false;

        // set each source to be empty and ready
        StopMusic(true, true);

        // Start coroutine to load and play each track in the split song
        StartCoroutine(LoadAndStartSplitSong(songReferences));
    }

    private IEnumerator LoadAndStartSplitSong(List<AssetReference> songReferences)
    {
        int count = 0;
        foreach (AssetReference reference in songReferences)
        {
            AsyncOperationHandle splitHandle;

            if (reference.OperationHandle.IsValid())
            {
                splitHandle = reference.OperationHandle;
            }
            else
            {
                splitHandle = reference.LoadAssetAsync<AudioClip>();
            }
            yield return splitHandle;

            songHandles.Add(splitHandle);

            musicSources[count].clip = (AudioClip)splitHandle.Result;
            count++;
        }

        IncreaseSplitSong();
    }

    public void EndSplitSong()
    {
        startedSplitSong = false;
    }

    public void IncreaseSplitSong()
    {
        if (!startedSplitSong)
        {
            musicSources[0].volume = 0.5f;
            currSplitIndex++;

            // begin all music sources at once
            foreach (var source in musicSources)
                source.Play();

            startedSplitSong = true;
            return;
        }
        else
        {
            if (currSplitIndex >= musicSources.Count)
                return;

            StartCoroutine(SmoothStartMusicSource(musicSources[currSplitIndex], smoothSplitDuration));
            currSplitIndex++;
        }
    }

    public void DecreaseSplitSong()
    {
        if (!startedSplitSong)
            return;

        if (currSplitIndex <= 1)
            return;

        currSplitIndex--;
        StartCoroutine(SmoothEndMusicSource(musicSources[currSplitIndex], smoothSplitDuration));
    }

    private IEnumerator SmoothStartMusicSource(AudioSource source, float duration)
    {
        float timer = 0f;
        float endVol = StudentInfoSystem.GetCurrentProfile().musicVol;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                source.volume = endVol;
                break;
            }

            float tempVol = Mathf.Lerp(0f, endVol, timer / duration);
            source.volume = tempVol;
            yield return null;
        }
    }

    private IEnumerator SmoothEndMusicSource(AudioSource source, float duration)
    {
        float timer = 0f;
        float startVol = source.volume;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                source.volume = 0f;
                break;
            }

            float tempVol = Mathf.Lerp(startVol, 0f, timer / duration);
            source.volume = tempVol;
            yield return null;
        }
    }

    /* 
    ################################################
    #   FX SOURCE
    ################################################
    */

    // plays a sound once
    public void PlayFX_oneShot(AssetReference clipRef, float volume, string id = "fx_oneShot", float pitch = 1f)
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clipRef, volume, false, pitch);
    }

    // plays a sound continuously until scene changes or stopped manually
    public void PlayFX_loop(AssetReference clipRef, float volume, string id = "fx_loop", float pitch = 1f)
    {
        var audioObj = Instantiate(fxAudioObject, fxObjectHolder);
        audioObj.GetComponent<FxAudioObject>().PlayClip(id, clipRef, volume, true, pitch);
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

    public void PlayTalk(AssetReference _clip)
    {
        talkSource.Stop();

        if (talkHandle.IsValid())
        {
            Addressables.Release(talkHandle);
        }

        StartCoroutine(LoadAndPlayTalk(_clip));
    }

    private IEnumerator LoadAndPlayTalk(AssetReference clipRef)
    {
        if (clipRef.OperationHandle.IsValid())
        {
            talkHandle = clipRef.OperationHandle;
        }
        else
        {
            talkHandle = clipRef.LoadAssetAsync<AudioClip>();
        }
        yield return talkHandle;


        AudioClip _clip = (AudioClip)talkHandle.Result;

        talkSource.clip = _clip;
        talkSource.loop = false;
        talkSource.Play();

        yield return new WaitForSeconds(_clip.length);

        if (talkHandle.IsValid())
        {
            Addressables.Release(talkHandle);
        }
    }

    public void StopTalk()
    {
        talkSource.Stop();

        if (talkHandle.IsValid())
        {
            Addressables.Release(talkHandle);
        }

        talkSource.clip = null;
    }

    public void PlayPhoneme(ElkoninValue elkoninValue)
    {
        talkSource.Stop();

        if (talkHandle.IsValid())
        {
            Addressables.Release(talkHandle);
        }

        StartCoroutine(LoadAndPlayPhoneme(elkoninValue));
    }

    private IEnumerator LoadAndPlayPhoneme(ElkoninValue elkoninValue)
    {
        AssetReference audioRef = GameManager.instance.GetGameWord(elkoninValue).audio;
        if (audioRef.OperationHandle.IsValid())
        {
            talkHandle = audioRef.OperationHandle;
        }
        else
        {
            talkHandle = audioRef.LoadAssetAsync<AudioClip>();
        }
        yield return talkHandle;

        AudioClip clip = (AudioClip)talkHandle.Result;

        talkSource.clip = clip;
        talkSource.loop = false;
        talkSource.Play();

        yield return new WaitForSeconds(clip.length);

        if (talkHandle.IsValid())
        {
            Addressables.Release(talkHandle);
        }
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */

    public IEnumerator GetClipLength(AssetReference clipRef)
    {
        AsyncOperationHandle handle;

        if (clipRef.OperationHandle.IsValid())
        {
            handle = clipRef.OperationHandle;
        }
        else
        {
            handle = clipRef.LoadAssetAsync<AudioClip>();
        }
        yield return handle;
        AudioClip audio = (AudioClip)handle.Result;
        float clipLength = audio.length;
        if(handle.IsValid())
        {
            Addressables.Release(handle);
        }
        audio = null;

        yield return clipLength;
    }
}
