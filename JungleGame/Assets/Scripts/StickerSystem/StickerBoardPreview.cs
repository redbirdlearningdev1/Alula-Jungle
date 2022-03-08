using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class StickerBoardPreview : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int boardPreviewIndex;

    public Image boardImage;
    public Image soldOutImage;

    public bool interactable;
    private bool isPressed = false;

    void Awake()
    {
        // set board to be unselected
        boardImage.color = new Color(1f, 1f, 1f, 0.25f);
        // set sold out image to be 0f alpha
        soldOutImage.color = new Color(1f, 1f, 1f, 0f);
    }

    public void SetBoardSelected(bool opt)
    {
        if (opt)
        {
            // set board to be selected
            boardImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // set board to be selected
            boardImage.color = new Color(1f, 1f, 1f, 0.1f);
        }
    }

    public void SetBoardSoldOut(bool opt)
    {
        if (opt)
        {
            // set board to be selected
            soldOutImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // set board to be selected
            soldOutImage.color = new Color(1f, 1f, 1f, 0f);
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
            //GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
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
            //GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
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
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
            
            isPressed = false;
            interactable = false;

            GetComponent<LerpableObject>().LerpScale(Vector2.one, 0.1f);
            BoardBookWindow.instance.CenterOnBoardPreview(boardPreviewIndex);
        }
    }
}
