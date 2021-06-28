using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotController : MonoBehaviour
{
    // Start is called before the first frame update\
    private Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void idle()
    {
        StartCoroutine(idleRoutine());
    }
    private IEnumerator idleRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("Idle");
    }
    public void success()
    {
        StartCoroutine(successRoutine());
    }
    private IEnumerator successRoutine()
    {

        animator.Play("PreFly");
        yield return new WaitForSeconds(1.3f);
        animator.Play("Celebrate");

    }
    public void fail()
    {
        StartCoroutine(failRoutine());
    }
    private IEnumerator failRoutine()
    {

        animator.Play("PreFly");
        yield return new WaitForSeconds(1.3f);
        animator.Play("No");

    }
    public void miss()
    {
        StartCoroutine(missRoutine());
    }
    private IEnumerator missRoutine()
    {

        animator.Play("miss");
        yield return new WaitForSeconds(0f);


    }
    public void hit()
    {
        StartCoroutine(hitRoutine());
    }
    private IEnumerator hitRoutine()
    {

        animator.Play("hit");
        yield return new WaitForSeconds(0f);

    }
}
