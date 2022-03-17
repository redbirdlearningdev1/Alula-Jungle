using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsSceneManager : MonoBehaviour
{
    void Awake()
    {
        GameManager.instance.SceneInit();
        AudioManager.instance.StopMusic();
    }

    public void OnReturnToSplashScreenPressed()
    {
        GameManager.instance.RestartGame();
    }
}
