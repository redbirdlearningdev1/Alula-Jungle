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
        animator.Play("Deal");
    }
    
    public void TigerAway()
    {
        animator.Play("CoinAway");
    }
}
