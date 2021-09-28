using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebController : MonoBehaviour
{
    public static WebController instance;

    public Animator animator;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void webSmall()
    {
        animator.Play("WebSmall");
    }

    public void webLarge()
    {
        animator.Play("WebLarge");
    }
}
