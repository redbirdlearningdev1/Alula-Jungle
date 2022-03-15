using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public enum BackButtonType
{
    Wagon,
    StickerBoard
}

public class BackButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable;
    public BackButtonType buttonType;
    private bool isPressed = false;

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
            GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
    }

    void OnMouseExit()
    {
        // skip if not interactable 
        if (!interactable)
            return;
            
        if (isOver)
        {
            isOver = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
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
            GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && isOver)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            switch (buttonType)
            {
                case BackButtonType.Wagon:
                    StickerSystem.instance.OnBackButtonPressed();
                    break;
                
                case BackButtonType.StickerBoard:
                    StickerSystem.instance.CloseStickerBoards();
                    break;
            }
            
            isPressed = false;
        }
    }
}