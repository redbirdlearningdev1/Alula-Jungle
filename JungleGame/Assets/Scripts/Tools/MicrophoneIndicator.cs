using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MicrophoneIndicator : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = false;
    private bool isPressed = false;
    public bool hasBeenPressed = false;
    private static float pressedScaleChange = 0.95f;

    [Header("Mic Sprites")]
    public Sprite micBlue;
    public Sprite micGreen;
    public Sprite micRed;

    [Header("Components")]
    public Image image;
    public LerpableObject lerpObj;
    public WiggleController wiggleController;

    void Awake()
    {
        image.sprite = micBlue;
        lerpObj.LerpScale(new Vector2(0f, 0f), 0f);
    }

    public void ShowIndicator()
    {
        image.sprite = micBlue;
        wiggleController.StartWiggle();
        lerpObj.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
    }

    public void AudioInputDetected()
    {
        image.sprite = micGreen;
        wiggleController.StopWiggle();
        lerpObj.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
    }

    public void NoInputDetected()
    {
        image.sprite = micRed;
        wiggleController.StopWiggle();
        lerpObj.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
    }

    public void HideIndicator()
    {
        lerpObj.SquishyScaleLerp(new Vector2(1.35f, 1.35f), new Vector2(0f, 0f), 0.2f, 0.2f);
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

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

            hasBeenPressed = true;
        }
    }
}
