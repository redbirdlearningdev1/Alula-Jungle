using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebberController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void grab1()
    {
        StartCoroutine(grab1Routine());
    }
    private IEnumerator grab1Routine()
    {
        animator.Play("Grab1");
        yield return new WaitForSeconds(0f);
    }
    public void grab2()
    {
        StartCoroutine(grab2Routine());
    }
    private IEnumerator grab2Routine()
    {
        animator.Play("Grab2");
        yield return new WaitForSeconds(0f);
    }
    public void grab3()
    {
        StartCoroutine(grab3Routine());
    }
    private IEnumerator grab3Routine()
    {
        animator.Play("Grab3");
        yield return new WaitForSeconds(0f);
    }
    public void grab4()
    {
        StartCoroutine(grab4Routine());
    }
    private IEnumerator grab4Routine()
    {
        animator.Play("Grab4");
        yield return new WaitForSeconds(0f);
    }
    public void grabBug()
    {
        StartCoroutine(grabBugRoutine());
    }
    private IEnumerator grabBugRoutine()
    {
        animator.Play("WebGrab");
        yield return new WaitForSeconds(0f);
    }
}
