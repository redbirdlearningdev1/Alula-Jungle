using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmeraldTigerHolder : MonoBehaviour
{
    public static EmeraldTigerHolder instance;

    public Animator animator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OpenMouth()
    {
        animator.Play("Open");
    }

    public void CloseMouth()
    {
        animator.Play("Close");
    }

    public void Thinking()
    {
        // start wiggle
        GetComponent<WiggleController>().StartWiggle();

        animator.Play("Thinking");
    }

    public void SetCorrect(bool isCorrect)
    {
        StartCoroutine(SetCorrectRoutine(isCorrect));
    }

    private IEnumerator SetCorrectRoutine(bool isCorrect)
    {
        // start wiggle
        GetComponent<WiggleController>().StopWiggle();

        if (isCorrect)
        {
            animator.Play("ThinkCorrect");
        }
        else
        {
            animator.Play("ThinkWrong");
        }

        yield return new WaitForSeconds(1f);

        if (isCorrect)
        {
            animator.Play("CorrectClose");
        }
        else
        {
            animator.Play("WrongClose");
        }
    }

}
