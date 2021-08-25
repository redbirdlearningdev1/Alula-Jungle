using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StickerRarity
{
    Common, Uncommon, Rare, Legendary
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sticker", order = 2)]
public class Sticker : ScriptableObject
{
    public Sprite sprite;
    public StickerRarity rarity;
    public int id;
    public int count;

    public Sticker(Sticker sticker)
    {
        this.sprite = sticker.sprite;
        this.rarity = sticker.rarity;
        this.id = sticker.id;
        this.count = sticker.count;
    }
}
