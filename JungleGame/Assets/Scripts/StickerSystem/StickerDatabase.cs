using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerDatabase : MonoBehaviour
{
    public static StickerDatabase instance;

    [Header("Sticker Objects")]
    public List<Sticker> commonStickers;
    public List<Sticker> uncommonStickers;
    public List<Sticker> rareStickers;
    public List<Sticker> legendaryStickers;

    [Header("Rarities")]
    public int commonRarity;
    public int uncommonRarity;
    public int rareRarity;
    public int legendaryRarity;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Sticker RollForSticker()
    {
        int totalRarity = commonRarity + uncommonRarity + rareRarity + legendaryRarity;
        int index = Random.Range(0, totalRarity);

        // common
        if (index >= 0 && index < commonRarity)
        {
            return GetRandomSticker(StickerRarity.Common);
        }
        // uncommon
        else if (index >= commonRarity && index < commonRarity + uncommonRarity)
        {
            return GetRandomSticker(StickerRarity.Uncommon);
        }
        // rare
        else if (index >= commonRarity + uncommonRarity && index < commonRarity + uncommonRarity + rareRarity)
        {
            return GetRandomSticker(StickerRarity.Rare);
        }
        // legendary
        else if (index >= commonRarity + uncommonRarity + rareRarity && index < totalRarity)
        {
            return GetRandomSticker(StickerRarity.Legendary);
        }

        // should never get to this point
        GameManager.instance.SendError(this, "no sticker was returned - theres a bug in your code dude...");
        return null;
    }

    private Sticker GetRandomSticker(StickerRarity rarity)
    {
        int index = 0;
        switch (rarity)
        {
            default:
            case StickerRarity.Common:
                index = Random.Range(0, commonStickers.Count);
                return commonStickers[index];

            case StickerRarity.Uncommon:
                index = Random.Range(0, uncommonStickers.Count);
                return uncommonStickers[index];

            case StickerRarity.Rare:
                index = Random.Range(0, rareStickers.Count);
                return rareStickers[index];

            case StickerRarity.Legendary:
                index = Random.Range(0, legendaryStickers.Count);
                return legendaryStickers[index];
        }
    }
}
