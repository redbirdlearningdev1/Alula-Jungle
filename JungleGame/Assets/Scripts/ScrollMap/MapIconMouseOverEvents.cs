using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapIconMouseOverEvents : MonoBehaviour
{
    public MapIcon mapIcon;

    void OnMouseOver()
    {
        mapIcon.OnMouseOverEvent();
    }

    void OnMouseExit()
    {
        mapIcon.OnMouseExitEvent();
    }
}
