using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FxAudioObject : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlayClip(AudioClip clip, float volume, float duration)
    {
        // set clip and volume
        audioSource.clip = clip;
        audioSource.volume = volume;

        // play clip and destroy object once complete
        audioSource.Play();
        StartCoroutine(DelayDestruction(duration));
    }

    private IEnumerator DelayDestruction(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
