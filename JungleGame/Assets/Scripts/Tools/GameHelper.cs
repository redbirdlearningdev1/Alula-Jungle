using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameHelper
{
    private static GameManager gm;

    // class constructor
    static GameHelper()
    {
        FindGameManager();
    }

    // every scene must call this function in Awake()
    public static void SceneInit()
    {
        FindGameManager();
        gm.SceneInit();
    }

    public static void NewLevelPopup(Level level)
    {
        FindGameManager();
        gm.NewLevelPopup(level);
    }

    public static void SetRaycastBlocker(bool opt)
    {
        FindGameManager();
        gm.SetRaycastBlocker(opt);
    }

    public static void LoadScene(int sceneNum, bool fadeOut, float time = GameManager.transitionTime)
    {   
        FindGameManager();
        gm.LoadScene(sceneNum, fadeOut, time);
    }

    public static void LoadScene(string sceneName, bool fadeOut, float time = GameManager.transitionTime)
    {   
        FindGameManager();
        gm.LoadScene(sceneName, fadeOut, time);
    }

    public static void RestartGame()
    {
        FindGameManager();
        gm.RestartGame();
    }

    private static void FindGameManager()
    {
        if (gm == null) gm = GameObject.Find("TheGameManager").GetComponent<GameManager>();

        // GameHelper could not find TheGameManager
        if (gm == null) Debug.LogError("GameHelper could not find 'TheGameManager'");
    }
}
