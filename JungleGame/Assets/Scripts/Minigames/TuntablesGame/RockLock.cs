using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLock : MonoBehaviour
{
    public static RockLock instance;
    public GlowOutlineController glowController;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
}
