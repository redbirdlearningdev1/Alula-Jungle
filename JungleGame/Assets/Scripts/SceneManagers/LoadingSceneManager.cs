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
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;
        FadeObject.instance.FadeOut(0f);
    }

    void Update()
    {
        // move background

    }
}
