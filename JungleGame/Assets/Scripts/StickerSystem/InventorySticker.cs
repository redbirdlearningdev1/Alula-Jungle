using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventorySticker : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public TextMeshProUGUI countText;
    public Image stickerImage;
    public Transform stickerObject;
    public LerpableObject stickerLerpableObject;

    private bool interactable = true;
    private bool isPressed = false;

    [HideInInspector] public InventoryStickerData currentStickerData;

    public void SetSticker(InventoryStickerData stickerData)
    {
        currentStickerData = stickerData;
        countText.text = "x" + currentStickerData.count;
        Sticker sticker = StickerDatabase.instance.GetSticker(currentStickerData);
        stickerImage.sprite = sticker.sprite;
    }

    public void RemoveStickerIfValid()
    {
        if ((currentStickerData.count - 1) <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    private bool isOver = false;

    void OnMouseOver()
    {
        // skip if not interactable 
        if (!interactable)
            return;
        
        if (!isOver)
        {
            isOver = true;
            stickerLerpableObject.LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
    }

    void OnMouseExit()
    {
        if (isOver)
        {
            isOver = false;
            stickerLerpableObject.LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable || !isOver)
            return;

        if (!isPressed)
        {
            isPressed = true;
            stickerLerpableObject.LerpScale(new Vector2(0.9f, 0.9f), 0.1f);

            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            // reduce count by 1
            countText.text = "x" + (currentStickerData.count - 1);

            // select this sticker
            StickerSystem.instance.SetCurrentHeldSticker(stickerObject);

            // close sticker inventory
            StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
        }
    }
}
