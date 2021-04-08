using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    private int currBag = 0;
    private const int maxBag = 3;

    [Header("Objects")]
    [SerializeField] private Image bag;
    [SerializeField] private Image shadow;

    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;
    [SerializeField] private List<Sprite> shadowSprites;

    void Awake()
    {   
        bag.sprite = bagSprites[currBag];
        shadow.sprite = shadowSprites[currBag];
    }

    public void UpgradeBag()
    {
        if (currBag < maxBag)
        {
            currBag++;
        }

        bag.sprite = bagSprites[currBag];
        shadow.sprite = shadowSprites[currBag];
    }

    public void DowngradeBag()
    {
        if (currBag > 0)
        {
            currBag--;
        }

        bag.sprite = bagSprites[currBag];
        shadow.sprite = shadowSprites[currBag];
    }
}
