using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public string keyName;

    public void StartMovingAnimation()
    {
        string animation_name = keyName + "_first_move";
        animator.Play(animation_name);
    }

    public void StopMovingAnimation()
    {
        string animation_name = keyName + "_last_move";
        animator.Play(animation_name);
    }

    public void PlaySoundAnimation(float duration)
    {
        StartCoroutine(PlaySoundRoutine(duration));
    }

    private IEnumerator PlaySoundRoutine(float duration)
    {
        string animation_name = keyName + "_audio_play";
        animator.Play(animation_name);

        yield return new WaitForSeconds(duration);
        
        animation_name = keyName + "_still";
        animator.Play(animation_name);
    }
}
