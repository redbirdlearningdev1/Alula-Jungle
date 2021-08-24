using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySticker : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Image stickerImage;
    public TextMeshProUGUI stickerCountText;
    private int stickerCount;

    private bool isPressed;
    public float pressedScaleChange;

    private Sticker myStickerObject;

    void Awake()
    {
        //stickerCount.text = "";
    }

    public void SetStickerType(Sticker sticker)
    {
        myStickerObject = sticker;
        stickerImage.sprite = sticker.sprite;
        // TODO update count using SIS
        stickerCount = 1;
    }

    public void UseOneSticker()
    {
        stickerCount--;

        // TODO update SIS
        
        // remove sticker from window
        if (stickerCount <= 0)
        {
            Destroy(gameObject);
        }
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isPressed)
        {
            isPressed = true;
            stickerImage.transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);

            StickerBoard.instance.SetCurrentSticker(stickerImage.transform);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            stickerImage.transform.localScale = new Vector3(1f, 1f, 1f);

            StickerBoard.instance.RemoveCurrentSticker();
        }
    }
}
