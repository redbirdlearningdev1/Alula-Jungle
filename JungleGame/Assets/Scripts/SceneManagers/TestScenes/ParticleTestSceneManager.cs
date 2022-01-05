using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTestSceneManager : MonoBehaviour
{
    void Awake()
    {
    }

    void Start()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // turn on particle controller
        ParticleController.instance.isOn = true;
    }
}
