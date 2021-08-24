using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoardPreview : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int price;

    public LerpableObject boardObject;
    public LerpableObject priceObject;

    public float unselectedAlpha;
    public float selectedAlpha;

    private bool boardSelected = false;
    private bool isPressed = false;

    public void SetSelectBoard(bool opt)
    {
        boardSelected = opt;

        if (boardSelected)
        {
            boardObject.LerpImageAlpha(boardObject.GetComponent<Image>(), selectedAlpha, 0.25f);
            //priceObject.LerpImageAlpha(priceObject.GetComponent<Image>(), selectedAlpha, 0.25f);

            boardObject.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.025f);
        }
        else
        {
            boardObject.LerpImageAlpha(boardObject.GetComponent<Image>(), unselectedAlpha, 0.25f);
            //priceObject.LerpImageAlpha(priceObject.GetComponent<Image>(), unselectedAlpha, 0.25f);
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
            BuyBoardWindow.instance.UnselectAllBoards();
            BuyBoardWindow.instance.SetAsChosenBoard(this);
            SetSelectBoard(true);
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
