using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatGameManager : MonoBehaviour
{
    public static BoatGameManager instance;

    public List<GlowOutlineController> glowOutlineControllers;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        PregameSetup();
    }

    private void PregameSetup()
    {
        // remove glow from icons
        foreach (GlowOutlineController item in glowOutlineControllers)
            item.ToggleGlowOutline(false);
    }
}
