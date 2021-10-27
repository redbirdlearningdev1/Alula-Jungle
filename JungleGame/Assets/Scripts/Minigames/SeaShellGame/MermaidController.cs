using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MermaidController : MonoBehaviour
{
    public static MermaidController instance;

    public Animator blueAnimator;
    public Animator pinkAnimator;
    public Animator playAnimator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ShowMermaids()
    {
        StartCoroutine(ShowMermaidsRoutine());
    }

    private IEnumerator ShowMermaidsRoutine()
    {
        blueAnimator.Play("BlueRise");
        yield return new WaitForSeconds(0.25f);
        pinkAnimator.Play("PinkRise");
    }
}
