using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatGameManager : MonoBehaviour
{
    public static BoatGameManager instance;

    public GameObject sky;
    public GameObject backIslands;
    public GameObject ocean;
    public GameObject frontIslands;

    public float skyMoveMult;
    public float backIslandsMoveMult;
    public float oceanMoveMult;
    public float frontIslandsMoveMult;

    [Range(0,1)] private float paralaxPos = 0.5f;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }
    }

    
}
