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


    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;


    void Awake()
    {
        bag.sprite = bagSprites[currBag];

    }

    public void UpgradeBag()
    {
        if (currBag < maxBag)
        {
            currBag++;
        }

        bag.sprite = bagSprites[currBag];

    }

    public void DowngradeBag()
    {
        if (currBag > 0)
        {
            currBag--;
        }

        bag.sprite = bagSprites[currBag];

    }
}
