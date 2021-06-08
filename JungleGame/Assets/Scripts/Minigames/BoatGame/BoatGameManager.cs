using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatGameManager : MonoBehaviour
{
    public static BoatGameManager instance;

    public List<GlowOutlineController> glowOutlineControllers;

    private bool arrived = false;

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

    public void ArrivedAtIsland()
    {
        if (!arrived)
        {
            arrived = true;
            StartCoroutine(ArrivedAtIslandRoutine());
        }
    }

    private IEnumerator ArrivedAtIslandRoutine()
    {
        yield return new WaitForSeconds(2f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }
}
