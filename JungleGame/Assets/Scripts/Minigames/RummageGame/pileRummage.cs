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
    [SerializeField] private SpriteRenderer Pile;

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

        Pile.sprite = pileSprites[currPile];

    }

    public void pileChose()
    {
        if (currPile == 0)
        {
            chosen = true;
            //Debug.Log(Pile.ToString());
        }
    }

    public void pileComplete()
    {
        chosen = false;
    }

    public void pileDone()
    {
        chosen = false;
        Pile.sprite = pileSprites[1];
    }

    public void pileGlowOff()
    {
        //print ("turning off glow! " + gameObject.name);
        Pile.GetComponent<GlowOutlineController>().ToggleGlowOutline(false);
    }

    public void pileGlowOn()
    {
        if (currPileLock)
        {
            //print ("turning on glow! " + gameObject.name);
            Pile.GetComponent<GlowOutlineController>().ToggleGlowOutline(true);
        }
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
