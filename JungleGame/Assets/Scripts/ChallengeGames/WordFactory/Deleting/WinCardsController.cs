using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCardsController : MonoBehaviour
{
    public static WinCardsController instance;

    public Animator card1Anim;
    public Animator card2Anim;
    public Animator card3Anim;

    private int currPolaroidCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ResetCards()
    {
        card1Anim.Play("Card1Off");
        card2Anim.Play("Card2Off");
        card3Anim.Play("Card3Off");
    }

    public void AddPolaroid()
    {
        currPolaroidCount++;
        switch (currPolaroidCount)
        {
            case 1:
                card1Anim.Play("Card1Enter");
                break;
            case 2:
                card2Anim.Play("Card2Enter");
                break;
            case 3:
                card3Anim.Play("Card3Enter");
                break;
            default:
                break;
        }
    }
}
