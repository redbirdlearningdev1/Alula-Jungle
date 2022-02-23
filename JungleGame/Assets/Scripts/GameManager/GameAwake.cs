using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAwake : MonoBehaviour
{
    public Vector2Int gameResolution;

    void Awake()
    {
        // set game resolution
        Screen.SetResolution(gameResolution.x, gameResolution.y, FullScreenMode.ExclusiveFullScreen);
    }
}
