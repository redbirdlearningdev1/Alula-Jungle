using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiController : MonoBehaviour
{
    private Animator animator;

    void Awake() 
    {
        animator = GetComponent<Animator>();    
    }

    public void CelebrateAnimation(float time = 1.5f)
    {
        StartCoroutine(CelebrateAnimationRoutine(time));
    }

    private IEnumerator CelebrateAnimationRoutine(float time)
    {
        animator.Play("taxi_celebrate");
        yield return new WaitForSeconds(time);
        animator.Play("taxi_idle");
    }

    public void TwitchAnimation()
    {
        StartCoroutine(TwitchAnimationRoutine());
    }

    private IEnumerator TwitchAnimationRoutine()
    {
        animator.Play("taxi_twitch");
        yield return new WaitForSeconds(0.5f);
        // make woosh sound effect
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Whoosh, 1f);
    }
}
