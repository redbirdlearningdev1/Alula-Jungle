using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;

        GameManager.instance.SceneInit();
    }

    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    void Update()
    {
        // move background

    }
}
