using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StickerInventoryButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static StickerInventoryButton instance;

    private bool isPressed = false;
    public float pressedScaleChange;

    public TextMeshProUGUI buttonText;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void UpdateButtonText()
    {
        buttonText.text = StudentInfoSystem.GetCurrentProfile().stickerInventory.Count.ToString() + "/" + StickerDatabase.instance.GetTotalStickerAmount();
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
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);

            // stop wiggle inventory button
            GetComponent<WiggleController>().StopWiggle();
            ImageGlowController.instance.SetImageGlow(GetComponent<Image>(), false, GlowValue.none);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
