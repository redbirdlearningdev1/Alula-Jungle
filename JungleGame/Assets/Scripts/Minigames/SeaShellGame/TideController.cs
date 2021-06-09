using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TideController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void washAnimation(float time = 1.5f)
    {
        StartCoroutine(resetAnimationRoutine(time));
    }

    private IEnumerator resetAnimationRoutine(float time)
    {
        gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }


}
