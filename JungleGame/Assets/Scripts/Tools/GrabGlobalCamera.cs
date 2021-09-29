using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabGlobalCamera : MonoBehaviour
{
    public bool grabCameraOnStart = true;
    public Canvas canvas;

    private void Start()
    {   
        // grabs the global camera from the game manager to use on the canvas
        if (grabCameraOnStart)
            canvas.worldCamera = GameManager.instance.globalCamera;
    }
}
