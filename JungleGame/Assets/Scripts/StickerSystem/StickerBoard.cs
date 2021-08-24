using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoard : MonoBehaviour
{
    public static StickerBoard instance;

    public LerpableObject stickerInventoryWindow;

    public Transform selectedStickerParent;
    public Transform placedStickerParent;
    public bool stickerInventoryActive = false;
    private bool canPlaceSticker = false;

    private Transform currentSticker = null;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // close inventory
        stickerInventoryWindow.LerpScale(new Vector2(0f, 1f), 0.0001f);

        // deactivate self
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentSticker != null)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 0f;
            currentSticker.transform.position = mousePos;
        }

        // open and close inventory with sticker
        if (currentSticker != null)
        {
            if (HideInventoryArea.instance.CheckIfMouseOverObject() && !canPlaceSticker)
            {
                canPlaceSticker = true;
                ToggleStickerInventory(false);
            }

            if (OpenInventoryArea.instance.CheckIfMouseOverObject() && canPlaceSticker)
            {
                canPlaceSticker = false;
                ToggleStickerInventory(true);
            }
        }
    }

    public void SetCurrentSticker(Transform newSticker)
    {
        canPlaceSticker = false;
        currentSticker = newSticker;
        currentSticker.SetParent(selectedStickerParent);
        currentSticker.GetComponent<Image>().raycastTarget = false;
    }

    public void RemoveCurrentSticker()
    {
        // either return sticker to inventory or place on sticker board

        if (canPlaceSticker && !stickerInventoryActive)
        {
            // make sure sticker is over sticker board (NOT OFF SCREEN)

            canPlaceSticker = false;
            // ask if this is an okay place to put sticker

            // FOR NOW JUST LET GO OF THE STICKER LOL
            currentSticker.GetComponent<StickerImage>().UseOneSticker();
            currentSticker.SetParent(placedStickerParent);
            currentSticker = null;
            return;
        }

        currentSticker.GetComponent<StickerImage>().ReturnStickerImageToParent();
        currentSticker.GetComponent<Image>().raycastTarget = true;
        currentSticker = null;
    }

    public void OnStickerInventoryPressed()
    {
        stickerInventoryActive = !stickerInventoryActive;

        // open window
        if (stickerInventoryActive)
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);
        }
        // close window
        else
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);
        }
    }

    public void ToggleStickerInventory(bool opt)
    {
        if (opt == stickerInventoryActive)
            return;
        stickerInventoryActive = opt;

        // open window
        if (stickerInventoryActive)
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);
        }
        // close window
        else
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);
        }
    }
}
