using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class FxAudioObject : MonoBehaviour
{
    public string id;
    public AudioMixerGroup mixerGroup;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        audioSource.outputAudioMixerGroup = mixerGroup;
    }

    public void PlayClip(string id, AudioClip clip, float volume, float duration, float pitch)
    {
        // set id
        this.id = id;

        // set clip and volume
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        // play clip and destroy object once complete OR loop forever
        audioSource.Play();

        if (duration == 0f)
        {
            audioSource.loop = true;
        }
        else
            StartCoroutine(DelayDestruction(duration));
    }

    public void InstaDestroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private IEnumerator DelayDestruction(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
