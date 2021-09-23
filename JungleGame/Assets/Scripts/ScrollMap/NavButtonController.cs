using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavButtonController : MonoBehaviour
{
    public bool isOn = false;
    public float lerpDuration;
    private bool mouseOver = false;

    void OnMouseOver()
    {
        if (!isOn)
            return;

        if (!mouseOver)
        {
            mouseOver = true;
        }
    }

    void OnMouseExit()
    {
        if (!isOn)
            return;

        if (mouseOver)
        {
            mouseOver = false;
        }
    }

    public void TurnOffButton()
    {
        isOn = false;
    }
}
