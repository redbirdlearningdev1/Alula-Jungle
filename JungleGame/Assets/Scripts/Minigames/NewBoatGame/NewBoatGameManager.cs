using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBoatGameManager : MonoBehaviour
{
    public static NewBoatGameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ArrivedAtIsland()
    {

    }
}
