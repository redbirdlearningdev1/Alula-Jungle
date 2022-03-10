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

        Sticker randomSticker = null;

        // common
        if (index >= 0 && index < commonRarity)
        {
            randomSticker = GetRandomSticker(StickerRarity.Common);
        }
        // uncommon
        else if (index >= commonRarity && index < commonRarity + uncommonRarity)
        {
            randomSticker = GetRandomSticker(StickerRarity.Uncommon);
        }
        // rare
        else if (index >= commonRarity + uncommonRarity && index < commonRarity + uncommonRarity + rareRarity)
        {
            randomSticker = GetRandomSticker(StickerRarity.Rare);
        }
        // legendary
        else if (index >= commonRarity + uncommonRarity + rareRarity && index < totalRarity)
        {
            randomSticker = GetRandomSticker(StickerRarity.Legendary);
        }

        if (randomSticker != null)
        {
            // unlock sticker in SIS
            switch (randomSticker.rarity)
            {
                case StickerRarity.Common:
                    StudentInfoSystem.GetCurrentProfile().commonStickerUnlocked[randomSticker.id - 1] = true;
                    break;

                case StickerRarity.Uncommon:
                    StudentInfoSystem.GetCurrentProfile().uncommonStickerUnlocked[randomSticker.id - 1] = true;
                    break;

                case StickerRarity.Rare:
                    StudentInfoSystem.GetCurrentProfile().rareStickerUnlocked[randomSticker.id - 1] = true;
                    break;

                case StickerRarity.Legendary:
                    StudentInfoSystem.GetCurrentProfile().legendaryStickerUnlocked[randomSticker.id - 1] = true;
                    break;
            }
            StudentInfoSystem.SaveStudentPlayerData();

            return randomSticker;
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

    public int GetTotalStickerAmount()
    {
        int total = 0;
        total += commonStickers.Count;
        total += uncommonStickers.Count;
        total += rareStickers.Count;
        total += legendaryStickers.Count;
        return total;
    }

    public Sticker GetSticker(InventoryStickerData data)
    {
        switch (data.rarity)
        {
            case StickerRarity.Common:
                return FindStickerByID(commonStickers, data.id);
            case StickerRarity.Uncommon:
                return FindStickerByID(uncommonStickers, data.id);
            case StickerRarity.Rare:
                return FindStickerByID(rareStickers, data.id);
            case StickerRarity.Legendary:
                return FindStickerByID(legendaryStickers, data.id);
        }
        return null;
    }

    public Sticker GetSticker(StickerData data)
    {
        switch (data.rarity)
        {
            case StickerRarity.Common:
                return FindStickerByID(commonStickers, data.id);
            case StickerRarity.Uncommon:
                return FindStickerByID(uncommonStickers, data.id);
            case StickerRarity.Rare:
                return FindStickerByID(rareStickers, data.id);
            case StickerRarity.Legendary:
                return FindStickerByID(legendaryStickers, data.id);
        }
        return null;
    }

    private Sticker FindStickerByID(List<Sticker> list, int id)
    {
        foreach (var item in list)
        {
            if (item.id == id)
                return item;
        }
        return null;
    }
}
