using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGlowController : MonoBehaviour
{
    public static ImageGlowController instance;

    public Material glowOnMaterial;
    public Material glowOffMaterial;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetImageGlow(Image img, bool opt)
    {
        if (opt)
            img.material = glowOnMaterial;
        else
            img.material = glowOffMaterial;
    }
}
