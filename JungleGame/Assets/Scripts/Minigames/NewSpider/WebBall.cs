using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebBall : MonoBehaviour
{
    private int currBall = 0;
    private const int maxBall = 4;

    [Header("Objects")]
    [SerializeField] private Image ball;

    [Header("Images")]
    [SerializeField] private List<Sprite> chestBall;

    void Awake()
    {
        ball.sprite = chestBall[currBall];
    }

    public void UpgradeChest()
    {
        if (currBall < maxBall)
        {
            currBall++;
        }

        ball.sprite = chestBall[currBall];
    }

    public void DowngradeChest()
    {
        if (currBall > 0)
        {
            currBall--;
        }

        ball.sprite = chestBall[currBall];
    }
}
