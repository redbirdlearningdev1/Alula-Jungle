using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerGameManager : MonoBehaviour
{
    void Start()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();
    }

    void Update()
    {
        
    }
}
