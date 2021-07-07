using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;


    public void SetStar(bool full)
    {
        // get sprite renderer if null
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (full)
            spriteRenderer.sprite = fullStar;
        else
            spriteRenderer.sprite = emptyStar;
    }
}
