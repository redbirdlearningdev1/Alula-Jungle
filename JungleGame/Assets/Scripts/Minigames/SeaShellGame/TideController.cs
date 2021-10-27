using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TideController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void washAnimation(float time = 1.5f)
    {
        StartCoroutine(resetAnimationRoutine(time));
    }

    private IEnumerator resetAnimationRoutine(float time)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    public void waveAnimation(float time = 1.5f)
    {
        StartCoroutine(waveAnimationRoutine(time));
    }
    private IEnumerator waveAnimationRoutine(float time)
    {
        animator.Play("tideWipe");
        yield return new WaitForSeconds(time);
    }


}
