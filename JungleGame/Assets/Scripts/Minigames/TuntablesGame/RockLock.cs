using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockLock : MonoBehaviour
{
    // glow variables
    public LerpableObject glowLerpObject;
    public Image glowImage;

     void Awake()
    {
        // set glow to be off on awake
        glowLerpObject.SetImageAlpha(glowImage, 0f);
    }

    // toggle glow
    public void ToggleGlow(bool opt)
    {
        if (opt)
        {
            glowLerpObject.LerpImageAlpha(glowImage, 1f, 0.25f);
        }
        else
        {
            glowLerpObject.LerpImageAlpha(glowImage, 0f, 0.25f);
        }
    }
}
