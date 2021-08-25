using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StickerImage : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public Transform inventoryStickerParent;
    public Button placeButtonTop;
    public Button placeButtonBottom;

    private bool isPressed;
    public float pressedScaleChange;
    private bool isGlued = false;

    void Awake()
    {
        // deactivate buttons
        placeButtonTop.gameObject.SetActive(false);
        placeButtonBottom.gameObject.SetActive(false);
    }

    public void OnPlaceButtonPressed()
    {
        isGlued = true;
        // deactivate buttons
        placeButtonTop.gameObject.SetActive(false);
        placeButtonBottom.gameObject.SetActive(false);
        // glue sticker to board
        StickerBoard.instance.GlueCurrentSticker();
    }

    public void ReturnStickerImageToParent()
    {
        StartCoroutine(ReturnStickerRoutine());
    }
    private IEnumerator ReturnStickerRoutine()
    {
        GetComponent<LerpableObject>().LerpPosToTransform(inventoryStickerParent.transform, 0.25f, false);
        yield return new WaitForSeconds(0.25f);
        transform.SetParent(inventoryStickerParent);
        transform.localPosition = Vector3.zero;
    }

    public void UseOneSticker()
    {
        inventoryStickerParent.GetComponent<InventorySticker>().UseOneSticker();
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isGlued)
            return;

        if (!isPressed)
        {
            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);

            StickerBoard.instance.SetCurrentSticker(transform);
            StickerBoard.instance.PickUpCurrentSticker();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isGlued)
            return;

        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);

            StickerBoard.instance.PlaceCurrentStickerDown();
        }
    }
}
