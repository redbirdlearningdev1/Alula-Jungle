using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : DontDestroy<GameManager>
{
    public bool devModeActivated;
    [HideInInspector] public bool acceptPlayerInput;
    public const float transitionTime = 1f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)


    new void Awake() 
    {
        acceptPlayerInput = false;
    }

    private void Update() 
    {
        if (devModeActivated)
        {
            // press 'D' to go to the dev menu
            if (Input.GetKeyDown(KeyCode.D))
                LoadScene("DevMenu", true);
        }
    }

    public void SceneInit()
    {
        StartCoroutine(SceneInitCoroutine());
    }

    private IEnumerator SceneInitCoroutine()
    {
        FadeHelper.FadeIn();
        yield return new WaitForSeconds(transitionTime);
        acceptPlayerInput = true;
    }

    public void RestartGame()
    {
        LoadScene(0, true);
    }

    public void LoadScene(string sceneName, bool fadeOut, float time = transitionTime)
    {
        acceptPlayerInput = false;
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeOut, time));
    }

    public void LoadScene(int sceneNum, bool fadeOut, float time = transitionTime)
    {
        acceptPlayerInput = false;
        StartCoroutine(LoadSceneCoroutine(sceneNum, fadeOut, time));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeHelper.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneCoroutine(int sceneNum, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeHelper.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(sceneNum);
    }
}