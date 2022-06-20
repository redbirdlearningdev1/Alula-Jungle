using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSplashSceneManager : MonoBehaviour
{
    void Awake()
    {
        SceneManager.LoadSceneAsync("SplashScene");
    }
}
