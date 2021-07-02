using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerPawController : MonoBehaviour
{
    private Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TigerDeal()
    {
        StartCoroutine(TigerDealRoutine());
    }
    private IEnumerator TigerDealRoutine()
    {
        animator.Play("Deal");
        yield return new WaitForSeconds(0f);
    }
    public void TigerSwipe()
    {
        StartCoroutine(TigerSwipeRoutine());
    }
    private IEnumerator TigerSwipeRoutine()
    {
        animator.Play("Swipe");
        yield return new WaitForSeconds(0f);
    }
    public void TigerAway()
    {
        StartCoroutine(TigerAwayRoutine());
    }
    private IEnumerator TigerAwayRoutine()
    {
        animator.Play("CoinAway");
        yield return new WaitForSeconds(0f);
    }


}
