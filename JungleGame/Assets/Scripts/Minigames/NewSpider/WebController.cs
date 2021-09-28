using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public static WebController instance;

    private Animator animator;


    void Awake()
    {
        if (instance == null)
            instance = this;

        animator = GetComponent<Animator>();
    }

    public void webSmall()
    {
        print ("web small!");
        animator.Play("WebSmall");
    }

    public void webLarge()
    {
        animator.Play("WebLarge");
    }
}
