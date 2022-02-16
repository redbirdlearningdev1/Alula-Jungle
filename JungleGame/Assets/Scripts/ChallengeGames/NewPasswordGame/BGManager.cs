using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGManager : MonoBehaviour
{
    public static BGManager instance;

    public Animator animator;
    private int currBG = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void MoveToNextSection()
    {
        switch (currBG)
        {
            case 0:
                animator.Play("BG1");
                break;
            
            case 1:
                animator.Play("BG2");
                break;

            case 2:
                animator.Play("BG3");
                break;
        }

        currBG++;
    }
}
