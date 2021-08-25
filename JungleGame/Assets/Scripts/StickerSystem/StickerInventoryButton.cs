using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (StudentInfoSystem.currentStudentPlayer != null)
            buttonText.text = StudentInfoSystem.currentStudentPlayer.stickerInventory.Count.ToString() + "/" + StickerDatabase.instance.GetTotalStickerAmount();
        else
            buttonText.text = "0/" + StickerDatabase.instance.GetTotalStickerAmount();
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
