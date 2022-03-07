using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerRevealCanvas : MonoBehaviour
{
    public static StickerRevealCanvas instance;
    public Image image;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        // hide sticker
        HideSticker();
    }

    public void RevealSticker(Sticker sticker)
    {
        image.sprite = sticker.sprite;
        image.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.25f, 1.25f), new Vector2(1f, 1f), 0.2f, 0.2f);
    }

    public void HideSticker()
    {
        image.transform.localScale = new Vector3(0f, 0f, 1f);
    }
}
