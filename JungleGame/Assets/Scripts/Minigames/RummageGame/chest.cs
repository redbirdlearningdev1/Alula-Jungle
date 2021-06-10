using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chest : MonoBehaviour
{
    private int currBag = 0;
    private const int maxBag = 5;

    [Header("Objects")]
    [SerializeField] private Image bag;
    [SerializeField] private SpriteRenderer chest1;

    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;



    void Awake()
    {
        bag.sprite = bagSprites[currBag];
        chest1.enabled = false;
    }

    public void UpgradeBag()
    {
        chest1.enabled = false;
        if (currBag < maxBag)
        {
            currBag++;
        }

        bag.sprite = bagSprites[currBag];

    }

    public void DowngradeBag()
    {
        chest1.enabled = false;
        if (currBag > 0)
        {
            currBag--;
        }

        bag.sprite = bagSprites[currBag];

    }
    public void chestGlow()
    {
        //chest1.sprite = bagSprites[currBag];
        chest1.enabled = true;

    }
    public void chestGlowNo()
    {
        //chest1.sprite = bagSprites[currBag];
        chest1.enabled = false;

    }
}
