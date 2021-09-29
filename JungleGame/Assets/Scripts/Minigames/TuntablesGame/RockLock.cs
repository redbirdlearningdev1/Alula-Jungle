using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockLock : MonoBehaviour
{
    public static RockLock instance;
    public Image image;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
}
