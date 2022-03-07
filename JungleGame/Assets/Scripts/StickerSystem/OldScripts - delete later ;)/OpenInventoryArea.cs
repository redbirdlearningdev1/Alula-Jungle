using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenInventoryArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static OpenInventoryArea instance;

    private bool isOver = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
    }

    public bool CheckIfMouseOverObject()
    {
        return isOver;
    }
}
