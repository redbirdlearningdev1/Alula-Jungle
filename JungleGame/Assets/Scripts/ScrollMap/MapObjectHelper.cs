using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectHelper : MonoBehaviour
{
    private static float screenHeight = 1200f;
    public Vector2 imageResolution;

    void Awake() 
    {
        RectTransform rt = GetComponent<RectTransform>();
        float ratio = screenHeight / imageResolution.y;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageResolution.x * ratio);
    }

    public void ResetResolution(Vector2 resolution)
    {
        this.imageResolution = resolution;

        RectTransform rt = GetComponent<RectTransform>();
        float ratio = screenHeight / imageResolution.y;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageResolution.x * ratio);
    }
}
