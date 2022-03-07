using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySticker : MonoBehaviour //, IPointerUpHandler, IPointerDownHandler
{
    public Image stickerImage;
    public TextMeshProUGUI stickerCountText;

    private bool isPressed;
    public float pressedScaleChange;

    public Sticker myStickerObject;
    private StickerBoard stickerBoard;

    public void SetStickerType(Sticker sticker, bool currentSticker = false)
    {
        myStickerObject = new Sticker(sticker);

        if (!currentSticker)
        {
            stickerImage.sprite = myStickerObject.sprite;
            stickerCountText.text = "x" + StudentInfoSystem.GetStickerCount(myStickerObject);
        }
        else 
        {
            int count = StudentInfoSystem.GetStickerCount(myStickerObject) - 1;
            stickerCountText.text = "x" + count.ToString();
        }
        
    }

    public void SetStickerBoard(StickerBoard board)
    {
        this.stickerBoard = board;
        stickerImage.GetComponent<StickerImage>().SetStickerBoard(board);
    }


    public void UpdateStickerCount(int newVal)
    {
        print ("new val count: " + newVal);
        stickerCountText.text = "x" + newVal;
    }

    public void UseOneSticker()
    {
        // remove sticker from inventory
        StudentInfoSystem.RemoveStickerFromInventory(myStickerObject);
        // update SIS
        stickerBoard.UpdateStickerInventory();
    }
}
