using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chest : MonoBehaviour
{
    private int currBag = 0;
    private const int maxBag = 5;

    [Header("Objects")]
    [SerializeField] private Image image;

    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;


    void Awake()
    {
        image.sprite = bagSprites[currBag];
    }

    public void UpgradeBag()
    {
        if (currBag < maxBag)
        {
            currBag++;
        }

        image.sprite = bagSprites[currBag];
    }

    public void DowngradeBag()
    {
        if (currBag > 0)
        {
            currBag--;
        }

        image.sprite = bagSprites[currBag];
    }

    public void chestGlow()
    {
        ImageGlowController.instance.SetImageGlow(image, true, GlowValue.glow_1_025);
    }

    public void chestGlowNo()
    {
        ImageGlowController.instance.SetImageGlow(image, false);
    }
}
