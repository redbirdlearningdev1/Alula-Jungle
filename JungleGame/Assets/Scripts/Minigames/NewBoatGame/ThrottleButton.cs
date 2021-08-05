using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrottleButton : MonoBehaviour
{
    public float pressedScale;

    public void ToggleScalePressed(bool opt)
    {    
        if (opt)
            transform.localScale = new Vector3(pressedScale, pressedScale, 1f);
        else
            transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
