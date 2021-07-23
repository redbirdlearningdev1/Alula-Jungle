using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebBall : MonoBehaviour
{
    private int currBall = 0;
    private const int maxBall = 4;

    [Header("Objects")]
    [SerializeField] private SpriteRenderer ball;
    [SerializeField] private GlowOutlineController glowOutlineController;



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

    public void chestGlow()
    {
        glowOutlineController.ToggleGlowOutline(true);
    }

    public void chestGlowNo()
    {
        glowOutlineController.ToggleGlowOutline(false);
    }


}
