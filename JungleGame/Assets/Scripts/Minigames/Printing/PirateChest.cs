using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateChest : MonoBehaviour
{
    public static PirateChest instance;

    public LerpableObject lerpableObject;

    private int currChest = 0;
    private const int maxChest = 4;

    [Header("Objects")]
    [SerializeField] private Image chest;

    [Header("Images")]
    [SerializeField] private List<Sprite> chestSprites;

    void Awake()
    {
        if (instance == null)
            instance = this;

        chest.sprite = chestSprites[currChest];
    }

    public void UpgradeChest()
    {
        if (currChest < maxChest)
        {
            currChest++;
        }

        lerpableObject.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        chest.sprite = chestSprites[currChest];
    }

    public void DowngradeChest()
    {
        if (currChest > 0)
        {
            currChest--;
        }

        chest.sprite = chestSprites[currChest];
    }
}
