using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpiderController : MonoBehaviour
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
        animator.Play("Success");
        yield return new WaitForSeconds(1f);
        animator.Play("WebShoot");
    }
    public void fail()
    {
        animator.Play("Hungry");
    }
}
