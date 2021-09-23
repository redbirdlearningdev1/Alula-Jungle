using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GlowValue
{
    none,
    glow_1_025,
    glow_5_04
}

public class ImageGlowController : MonoBehaviour
{
    public static ImageGlowController instance;

    public Material glowOnMaterial_1_025;
    public Material glowOnMaterial_5_04;
    public Material glowOffMaterial;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetImageGlow(Image img, bool opt, GlowValue value = GlowValue.none)
    {
        if (opt)
        {
            switch (value)
            {
                default:
                case GlowValue.none:
                    img.material = glowOffMaterial;
                    break;
                case GlowValue.glow_1_025:
                    img.material = glowOnMaterial_1_025;
                    break;
                case GlowValue.glow_5_04:
                    img.material = glowOnMaterial_5_04;
                    break;
            }
        }
        else
            img.material = glowOffMaterial;
    }
}
