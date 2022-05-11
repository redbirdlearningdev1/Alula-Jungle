using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pileRummage : MonoBehaviour
{
    private int currPile = 0;
    private const int maxPile = 1;
    public bool chosen = false;
    public bool currPileLock = true;

    [Header("Objects")]
    [SerializeField] private Image image;

    [Header("Images")]
    [SerializeField] private List<Sprite> pileSprites;


    void Awake()
    {
        //Pile.sprite = pileSprites[currPile];    
    }

    public void UpgradeBag()
    {
        if (currPile < maxPile)
        {
            currPile++;
        }

        image.sprite = pileSprites[currPile];
    }

    public void pileChose()
    {
        if (currPile == 0)
        {
            chosen = true;
        }
    }

    public void colliderOn()
    {
        image.raycastTarget = true;
    }
    public void colliderOff()
    {
        image.raycastTarget = false;
    }

    public void pileComplete()
    {
        chosen = false;
    }

    public void pileDone()
    {
        chosen = false;
        image.sprite = pileSprites[1];
    }

    public void SetWiggleOn()
    {
        if (currPileLock)
        {
            GetComponent<WiggleController>().StartWiggle(true);
        }
    }

    public void SetWiggleOff()
    {
        GetComponent<WiggleController>().StopWiggle();
    }

    public void pileLock()
    {
        currPileLock = false;
    }
}
