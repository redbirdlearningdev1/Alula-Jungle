using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoController : MonoBehaviour
{
    private Animator animator;
    public GameObject tenticle;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void octoIdle()
    {
        StartCoroutine(octoIdleRoutine());
    }
    private IEnumerator octoIdleRoutine()
    {
        animator.Play("octoIdle");
        yield return new WaitForSeconds(0f);
    }

    public void incorrectAnimation(float time = 1.5f)
    {
        StartCoroutine(incorrectAnimationRoutine(time));
    }

    private IEnumerator incorrectAnimationRoutine(float time)
    {
        animator.Play("octoNo");
        yield return new WaitForSeconds(time);
        animator.Play("octoIdle");
    }
    public void correctAnimation(float time = 1.5f)
    {
        StartCoroutine(correctAnimationRoutine(time));
    }

    private IEnumerator correctAnimationRoutine(float time)
    {
        animator.Play("octoGrabShow");
        yield return new WaitForSeconds(time);
        //tenticle.SetActive(true);
        //tenticle.GetComponent<Animator>().Play("armGrab");
        //yield return new WaitForSeconds(time);
        //tenticle.SetActive(true);
        //animator.Play("octoAway");
        //yield return new WaitForSeconds(time);
        //animator.Play("octoIdle");
    }
    public void correctAnimationTwo(float time = 1.5f)
    {
        StartCoroutine(correctTwoAnimationRoutine(time));
    }

    private IEnumerator correctTwoAnimationRoutine(float time)
    {
        //animator.Play("octoGrabShow");
        //yield return new WaitForSeconds(time);
        //tenticle.SetActive(true);
        //tenticle.GetComponent<Animator>().Play("armGrab");
        //yield return new WaitForSeconds(time);
        //tenticle.SetActive(true);
        animator.Play("octoAway");
        yield return new WaitForSeconds(time);
        //animator.Play("octoIdle");
    }

}
