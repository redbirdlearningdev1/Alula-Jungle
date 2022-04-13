using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(AudioSource))]
public class FxAudioObject : MonoBehaviour
{
    public string id;
    public AudioMixerGroup mixerGroup;
    private AudioSource audioSource;
    private AsyncOperationHandle handle;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public void PlayClip(string id, AssetReference clip, float volume, bool loop, float pitch)
    {
        StartCoroutine(PlayClipRoutine(id, clip, volume, loop, pitch));
    }

    private IEnumerator PlayClipRoutine(string id, AssetReference clipRef, float volume, bool loop, float pitch)
    {
        if (clipRef.OperationHandle.IsValid())
        {
            handle = clipRef.OperationHandle;
        }
        else
        {
            handle = clipRef.LoadAssetAsync<AudioClip>();
        }
        yield return handle;

        AudioClip clip = (AudioClip)handle.Result;

        // set id
        this.id = id;

        // set clip and volume
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        // play clip and destroy object once complete OR loop forever
        audioSource.Play();

        if (loop)
        {
            audioSource.loop = true;
        }
        else
            StartCoroutine(DelayDestruction(clip.length + 1f));
    }

    public void InstaDestroy()
    {
        StopAllCoroutines();
        if (gameObject != null)
            UnloadAndDestroy();
    }

    private IEnumerator DelayDestruction(float duration)
    {
        yield return new WaitForSeconds(duration);
        UnloadAndDestroy();
    }

    private void UnloadAndDestroy()
    {
        if(handle.IsValid())
        {
            Addressables.Release(handle);
        }
        Destroy(gameObject);
    }
}
