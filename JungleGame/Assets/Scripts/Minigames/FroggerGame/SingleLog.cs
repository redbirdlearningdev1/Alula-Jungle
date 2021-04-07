using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLog : MonoBehaviour
{
    private Animator animator;
    private bool isUp = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void LogRise()
    {
        if (!isUp)
        {
            // get animator if null
            if (!animator)
                Awake();
            animator.Play("log_rise");
            isUp = true;
        }
    }

    public void LogSink()
    {
        if (isUp)
        {
            // get animator if null
            if (!animator)
                Awake();
            animator.Play("log_sink");
            isUp = false;
        }
    }
}
