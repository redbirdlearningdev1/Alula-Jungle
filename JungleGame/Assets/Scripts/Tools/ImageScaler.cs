using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class ImageScaler : MonoBehaviour
{
    private Image img;
    private RectTransform rectTransform;

    public Vector2 spriteResolution;
    [Range(0,3)] public float scale;
    private float prevScale;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (scale != prevScale)
        {
            prevScale = scale;
            Vector2 scaledVector = spriteResolution * scale;
            rectTransform.sizeDelta = scaledVector;
        }
    }
}
