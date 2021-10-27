using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoatButtonID
{
    Green, Blue, Mic, Sound, Escape, Throttle
}

public class BoatButton : MonoBehaviour
{
    public BoatButtonID id;

    public Image image;
    public WiggleController wiggleController;
    public Sprite defaultSprite;
    public Sprite pressedSprite;

    void Awake()
    {
        SetPressedSprite(false);
    }

    // sets sprite between defualt and pressed
    public void SetPressedSprite(bool opt)
    {
        if (opt)
            image.sprite = pressedSprite;
        else
            image.sprite = defaultSprite;
    }
}
