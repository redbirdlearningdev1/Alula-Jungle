using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDelayWaveAnimation : MonoBehaviour
{
    public Animator animator;
    public float maxDelay;
    public string animationName;

    void Awake()
    {
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        float randomDelay = Random.Range(0, maxDelay);

        yield return new WaitForSeconds(randomDelay);

        animator.Play(animationName);
    }
}
