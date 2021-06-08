using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameDemoSceneManager : MonoBehaviour
{
    public static MinigameDemoSceneManager instance;

    void Start()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }
    }
}
