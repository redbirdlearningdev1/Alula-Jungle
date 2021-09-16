using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCoinsController : MonoBehaviour
{
    public static WaterCoinsController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    
}
