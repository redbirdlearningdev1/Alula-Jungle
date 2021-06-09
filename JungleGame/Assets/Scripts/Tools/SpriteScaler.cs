using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteScaler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private RectTransform rectTransform;

    public Vector2 spriteResolution;
    [Range(0,2)] public float scale;
    private float prevScale;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Sliced;
        spriteRenderer.size = spriteResolution * scale;
    }

    void Update()
    {
        if (scale != prevScale)
        {
            prevScale = scale;
            Vector2 scaledVector = spriteResolution * scale;
            spriteRenderer.size = scaledVector;
            rectTransform.sizeDelta = scaledVector;
        }
    }
}
