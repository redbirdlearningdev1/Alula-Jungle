using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteScaler : MonoBehaviour
{
    [Header("Sprite Autosize")]
    public bool autoSizeToRect;
    public RectTransform rect;
    public float extraPaddingMultiplier;

    private SpriteRenderer spriteRenderer;

    [Header("Sprite manual size")]
    public Vector2 spriteResolution;
    [Range(0,3)] public float scale;
    private float prevScale;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        spriteRenderer.size = spriteResolution * scale;
    }

    void Update()
    {
        // auto size sprite to be size of image
        if (autoSizeToRect)
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }

            Vector2 rectSize = rect.sizeDelta;
            rectSize *= extraPaddingMultiplier;
            spriteRenderer.size = rectSize;
            return;
        }

        if (scale != prevScale)
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }

            prevScale = scale;
            Vector2 scaledVector = spriteResolution * scale;
            spriteRenderer.size = scaledVector;
            rect.sizeDelta = scaledVector;
        }
    }
}
