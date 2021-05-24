using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatButtonID
{
    Green, Blue, Mic, Sound, Escape
}

public class BoatButton : MonoBehaviour
{
    public BoatButtonID id;

    [Header("Sprite Stuff")]
    public bool swapSprites;
    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Sprite pressedSprite;

    void Awake()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        SetPressedSprite(false);
    }

    // sets sprite between defualt and pressed
    public void SetPressedSprite(bool opt)
    {
        if (!swapSprites)
            return;

        if (opt)
            spriteRenderer.sprite = pressedSprite;
        else
            spriteRenderer.sprite = defaultSprite;
    }
}
