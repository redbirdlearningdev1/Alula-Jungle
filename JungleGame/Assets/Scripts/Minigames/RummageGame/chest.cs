using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chest : MonoBehaviour
{
    private int currBag = 0;
    private const int maxBag = 5;

    [Header("Objects")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GlowOutlineController glowOutlineController;

    [Header("Images")]
    [SerializeField] private List<Sprite> bagSprites;


    void Awake()
    {
        spriteRenderer.sprite = bagSprites[currBag];
    }

    public void UpgradeBag()
    {
        if (currBag < maxBag)
        {
            currBag++;
        }

        spriteRenderer.sprite = bagSprites[currBag];
    }

    public void DowngradeBag()
    {
        if (currBag > 0)
        {
            currBag--;
        }

        spriteRenderer.sprite = bagSprites[currBag];
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
