using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluMermaidController : MonoBehaviour
{
    private Animator animator;
   

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void riseIdleAnimation(float time = 1.5f)
    {
        StartCoroutine(riseIdleAnimationRoutine(time));
    }

    private IEnumerator riseIdleAnimationRoutine(float time)
    {
        animator.Play("bluRiseIdle");
        yield return new WaitForSeconds(time);
        animator.Play("bluIdle");
    }

    public void diveAnimation(float time = 1.5f)
    {
        StartCoroutine(diveAnimationRoutine(time));
    }

    private IEnumerator diveAnimationRoutine(float time)
    {
        animator.Play("bluDiveIdle");
        yield return new WaitForSeconds(time);
    }

    public void pinkShellAnimation(float time = 1.5f)
    {
        StartCoroutine(pinkShellAnimationRoutine(time));
    }

    private IEnumerator pinkShellAnimationRoutine(float time)
    {
        animator.Play("bluPlayNew");
        yield return new WaitForSeconds(time);

    }


    public void blueShellAnimation(float time = 1.5f)
    {
        StartCoroutine(blueShellAnimationRoutine(time));
    }

    private IEnumerator blueShellAnimationRoutine(float time)
    {
        animator.Play("bluPlayNew1");
        yield return new WaitForSeconds(time);

    }

    public void redShellAnimation(float time = 1.5f)
    {
        StartCoroutine(redShellAnimationRoutine(time));
    }

    private IEnumerator redShellAnimationRoutine(float time)
    {

        animator.Play("bluPlayNew2");
        yield return new WaitForSeconds(time);

    }


}
