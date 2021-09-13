using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDelayWaveAnimation : MonoBehaviour
{
    public Animator animator;
    public float maxDelay;

    void Awake()
    {
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        float randomDelay = Random.Range(0, maxDelay);

        yield return new WaitForSeconds(randomDelay);

        animator.Play("water_wavea"); // i accidenally named this file this and im too lazy to fix it and also it doesnt really matter
    }
}
