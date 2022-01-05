using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBlock : MonoBehaviour
{
    public Image image;

    public void SetBlock(Sprite sprite, Vector2 size)
    {
        image.sprite = sprite;
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
