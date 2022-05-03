using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAwake : MonoBehaviour
{
    public static Vector2Int gameResolution = new Vector2Int(1350, 900);

    public Camera mainCam;

    void Awake()
    {
        // set game resolution
        //Screen.SetResolution(gameResolution.x, gameResolution.y, true);
    }
}
