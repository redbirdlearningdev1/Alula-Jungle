using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphPoint : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool isOver;

    // void OnMouseOver()
    // {
    //     this.OnMouseOverEvent();
    // }

    // void OnMouseExit()
    // {
    //     this.OnMouseExitEvent();
    // }

    public void OnMouseOverEvent()
    {
        if (!isOver)
        {
            isOver = true;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1.5f, 1.5f), 0.1f);
        }
    }

    public void OnMouseExitEvent()
    {
        if (isOver)
        {
            isOver = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.OnMouseOverEvent();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.OnMouseExitEvent();
    }
}
