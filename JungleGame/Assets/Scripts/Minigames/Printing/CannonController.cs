using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    // Start is called before the first frame update\
    private Animator animator;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void idle()
    {
        StartCoroutine(idleRoutine());
    }
    private IEnumerator idleRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("Idle");
    }
    public void Load()
    {
        StartCoroutine(LoadRoutine());
    }
    private IEnumerator LoadRoutine()
    {

        animator.Play("Load");
        yield return new WaitForSeconds(0f);

    }
    
}
