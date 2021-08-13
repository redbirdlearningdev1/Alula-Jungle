using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatButtonID
{
    Green, Blue, Mic, Sound, Escape, Throttle
}

public class BoatButton : MonoBehaviour
{
    public BoatButtonID id;

    [Header("Sprite Stuff")]
    public SpriteRenderer spriteRenderer;
    public GlowOutlineController glowOutlineController;
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
            spriteRenderer.sprite = pressedSprite;
        else
            spriteRenderer.sprite = defaultSprite;
    }
}
