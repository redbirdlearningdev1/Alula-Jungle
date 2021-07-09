using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroy<GameManager>
{
    public bool devModeActivated;
    public const float transitionTime = 0.5f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)
    public Vector2Int gameResolution;

    // game data
    public List<ActionWord> actionWords;
    public List<StoryGameData> storyGameDatas;
    
    [SerializeField] private GameObject devModeIndicator;
    private bool devIndicatorSet = false;

    private GameData gameData;
    private MapIconIdentfier gameID;

    // DEV STUFF:
    private bool iconsSetBroke = false;

    void Start()
    {
        // set game resolution
        Screen.SetResolution(gameResolution.x, gameResolution.y, FullScreenMode.FullScreenWindow);

        // student information system setup - set to profile 1 if not chosen
        if (StudentInfoSystem.currentStudentPlayer == null)
            StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
    }

    private void Update()
    {
        if (devModeActivated)
        {
            // set dev mode indicator on (once)
            if (!devIndicatorSet)
            {
                SendLog(this, "Dev Mode - ON");
                devIndicatorSet = true;
                devModeIndicator.SetActive(true);
            }
            // press 'D' to go to the dev menu
            if (Input.GetKeyDown(KeyCode.D))
            {
                // stop music
                AudioManager.instance.StopMusic();
                LoadScene("DevMenu", true);
            }
            // press 'F' to toggle between fixed and broken map sprites
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject smm;
                smm = GameObject.Find("ScrollMapManager");

                if (smm == null) 
                    Debug.LogError("GameManager could not find 'ScrollMapManager'");
                else
                {
                    iconsSetBroke = !iconsSetBroke;
                    smm.GetComponent<ScrollMapManager>().SetMapIconsBroke(iconsSetBroke);
                    Debug.Log("Map icons broken set to: " + iconsSetBroke);
                }
            }
        }
        else
        {
            // set dev mode indicator off (once)
            if (!devIndicatorSet)
            {
                SendLog(this, "Dev Mode - OFF");
                devIndicatorSet = true;
                devModeIndicator.SetActive(false);
            }
        }
    }

    /* 
    ################################################
    #   SCENE INITIALIZATION
    ################################################
    */

    public void SceneInit()
    {
        StartCoroutine(SceneInitCoroutine());
    }

    private IEnumerator SceneInitCoroutine()
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("SceneInit");
        FadeObject.instance.FadeIn(transitionTime);
        yield return new WaitForSeconds(transitionTime);
        RaycastBlockerController.instance.RemoveRaycastBlocker("SceneInit");
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */

    public void RestartGame()
    {
        LoadScene(0, true);
    }

    public void SendError(Object errorContext, string errorMsg)
    {
        Debug.LogError("[ERROR] " + errorMsg + " @ " + errorContext.name, errorContext);
    }

    public void SendLog(Object context, string msg)
    {
        Debug.Log("[LOG] " + msg + " @ " + context.name);
    }

    public void SendLog(string context, string msg)
    {
        Debug.Log("[LOG] " + msg + " @ " + context);
    }

    // returns action word data from enum
    public ActionWord GetActionWord(ActionWordEnum word)
    {
        //print ("word: " + word);
        foreach(ActionWord actionWord in actionWords)
        {
            if (actionWord._enum.Equals(word))
                return actionWord;
        }
        SendError(this, "Could not find action word: \'" + word + "\'");
        return null;
    }

    // returns a list with every action word enum
    public List<ActionWordEnum> GetGlobalActionWordList()
    {
        var globalCoinPool = new List<ActionWordEnum>();
        string[] coins = System.Enum.GetNames(typeof(ActionWordEnum));
        for (int i = 0; i < coins.Length; i++) 
        {
            ActionWordEnum coin = (ActionWordEnum)System.Enum.Parse(typeof(ActionWordEnum), coins[i]);
            globalCoinPool.Add(coin);
        }
        // remove two invalid coins
        globalCoinPool.Remove(ActionWordEnum.SIZE);
        globalCoinPool.Remove(ActionWordEnum._blank);
        return globalCoinPool;
    }

    /* 
    ################################################
    #   SCENE MANAGEMENT
    ################################################
    */

    public void ReturnToScrollMap()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        LoadScene("ScrollMap", true);
    }

    public void LoadScene(string sceneName, bool fadeOut, float time = transitionTime)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("LoadingScene");
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeOut, time));
    }

    public void LoadScene(int sceneNum, bool fadeOut, float time = transitionTime)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("LoadingScene");
        StartCoroutine(LoadSceneCoroutine(sceneNum, fadeOut, time));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);

        SceneCleanup();

        SendLog(this, "Loading new scene - " + sceneName);
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneCoroutine(int sceneNum, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);

        SceneCleanup();

        SceneManager.LoadSceneAsync(sceneNum);
    }

    private void SceneCleanup()
    {
        // remove all sounds
        AudioManager.instance.ClearAllAudio();

        // remove star award window + levelPreview + toolbar 
        StarAwardController.instance.ResetWindow();
        LevelPreviewWindow.instance.ResetWindow();
        DropdownToolbar.instance.ToggleToolbar(false);

        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();
    }

    /* 
    ################################################
    #   GAME DATA
    ################################################
    */

    public void SetDataAndID(GameData data, MapIconIdentfier id)
    {
        print ("id: " + id);
        this.gameData = data;
        this.gameID = id;
    }

    public void SetData(GameData data)
    {
        this.gameData = data;
    }

    public GameData GetData()
    {
        return gameData;
    }

    public MapIconIdentfier GetID()
    {
        return gameID;
    }
}