using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    // Start is called before the first frame update\
    private Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void webSmall()
    {
        StartCoroutine(WebSmallRoutine());
    }
    private IEnumerator WebSmallRoutine()
    {
        yield return new WaitForSeconds(.5f);
        animator.Play("WebSmall");
    }
    public void webLarge()
    {
        StartCoroutine(WebLargeRoutine());
    }
    private IEnumerator WebLargeRoutine()
    {
        yield return new WaitForSeconds(.5f);
        animator.Play("WebLarge");
    }
}
