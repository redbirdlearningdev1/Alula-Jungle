using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinHolder : MonoBehaviour
{
    private int currHolder = 0;
    private const int maxHolder = 2;

    [Header("Objects")]
    [SerializeField] private Image coinHolder;

    [Header("Images")]
    [SerializeField] private List<Sprite> holderSprites;

    void Awake()
    {
        coinHolder.sprite = holderSprites[currHolder];
    }

    public void CorrectCoinHolder()
    {
        currHolder = 1;
        coinHolder.sprite = holderSprites[currHolder];
    }

    public void IncorrectCoinHolder()
    {
        currHolder = 2;
        coinHolder.sprite = holderSprites[currHolder];
    }
    public void BaseCoinHolder()
    {
        currHolder = 0;
        coinHolder.sprite = holderSprites[currHolder];
    }
}
