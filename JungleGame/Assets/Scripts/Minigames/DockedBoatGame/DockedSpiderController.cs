using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockedSpiderController : MonoBehaviour
{
    private Coroutine playSpiderCoroutine;
    private bool isSpiderGone;

    public void Start()
    {
        isSpiderGone = true;
    }

    public void PlaySpider()
    {
        if (playSpiderCoroutine == null)
        {
            playSpiderCoroutine = StartCoroutine(PlaySpiderCoroutine());
        }
    }

    IEnumerator PlaySpiderCoroutine()
    {
        GetComponent<Animator>().Play("Spider_Intro");
        yield return new WaitForSeconds(1.0f);
        while (!isSpiderGone)
        {
            yield return null;
        }
        playSpiderCoroutine = null;
    }

    // Can't be a bool parameter for Animation Events :(
    public void SetIsSpiderGone(int isGone)
    {
        if (isGone == 0)
        {
            isSpiderGone = false;
        }
        else
        {
            isSpiderGone = true;
        }
    }
}
