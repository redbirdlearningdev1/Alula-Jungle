using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySticker : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public Image stickerImage;

    [HideInInspector] public InventoryStickerData currentStickerData;

    public void SetSticker(InventoryStickerData stickerData)
    {
        currentStickerData = stickerData;
        countText.text = "x" + currentStickerData.count;
        Sticker sticker = StickerDatabase.instance.GetSticker(currentStickerData);
        stickerImage.sprite = sticker.sprite;
    }
}
