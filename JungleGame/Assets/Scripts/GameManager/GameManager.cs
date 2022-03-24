using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameType
{
    None,

    DevMenu,
    StoryGame,
    BoatGame,

    FroggerGame,
    TurntablesGame,
    RummageGame,
    SeashellGame,
    PirateGame,
    SpiderwebGame,

    WordFactoryBlending,
    WordFactorySubstituting,
    WordFactoryDeleting,
    WordFactoryBuilding,
    TigerPawPhotos,
    TigerPawCoins,
    Password,

    COUNT
}

public class GameManager : DontDestroy<GameManager>
{
    public static string currentGameVersion = "alpha1.4";

    public static int stickerInventorySize = 16;

    public bool devModeActivated;
    public const float transitionTime = 0.5f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)
    public Camera globalCamera;

    // game data
    [Header("Coin Objects")]
    public List<ActionWord> actionWords;
    public List<ConsonantWord> consonantWords;

    [Header("Game Datas")]
    public List<StoryGameData> storyGameDatas;

    public StoryGameData storyGameData;
    public MapIconIdentfier mapID;
    public MapLocation prevMapLocation = MapLocation.NONE;
    
    [HideInInspector] public bool repairMapIconID; // when the scroll map appears -> repair this icon
    [HideInInspector] public GameType prevGameTypePlayed = GameType.None;

    // game identification
    [HideInInspector] public bool playingChallengeGame = false; // is player in a challenge game?
    [HideInInspector] public bool playingRoyalRumbleGame = false; // is player in a RR game?
    [HideInInspector] public bool finishedRoyalRumbleGame = false; // for playing talkies after RR game
    [HideInInspector] public bool wonRoyalRumbleGame = false; // for playing talkies after RR game
    [HideInInspector] public bool finishedBoatGame = false; // used for docked boat scene talkies

    [HideInInspector] public bool playingBossBattleGame = false; // is player in a boss battle game?
    [HideInInspector] public bool newBossBattleStoryBeat = false; // did player move to a new boss battle story beat?

    [Header("Avatars")]
    public List<Sprite> avatars;

    void Start()
    {
        // set game resolution
        Screen.SetResolution(GameAwake.gameResolution.x, GameAwake.gameResolution.y, true);

        if (devModeActivated)
        {
            SendLog(this, "Dev Mode set as - ON");
        }
        else
        {
            SendLog(this, "Dev Mode set as - OFF");
        }
    }

    void Update()
    {
        if (devModeActivated)
        {            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                // press 'Shift + D' to go to the dev menu
                if (Input.GetKeyDown(KeyCode.D))
                {
                    LoadScene("DevMenu", true);
                }

                // press 'Shift + C' to open console
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.LogError("forcing open Development Console...");
                    StartCoroutine(LogDateTimeNow(2f));
                }       
            }
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            // turn on audio
            AudioManager.instance.SetMasterVolume(StudentInfoSystem.GetCurrentProfile().masterVol);
        }
        else
        {
            // turn off audio
            AudioManager.instance.SetMasterVolume(0f);
        }
    }

    private IEnumerator LogDateTimeNow(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.LogError("it is currently " + System.DateTime.Now);
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
        // clean up environment
        SceneCleanup();

        FadeObject.instance.SetFadeImmediate(true); // turn on black fade
        RaycastBlockerController.instance.CreateRaycastBlocker("SceneInit");
        yield return new WaitForSeconds(0.5f); // wait a short moment before fading in
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
        LoadScene("SplashScene", true);
    }

    public void SendError(Object errorContext, string errorMsg)
    {
        Debug.LogError("[ERROR] " + errorMsg + " @ " + errorContext.name, errorContext);
    }

    public void SendError(string errorContext, string errorMsg)
    {
        Debug.LogError("[ERROR] " + errorMsg + " @ " + errorContext);
    }

    public void SendLog(Object context, string msg)
    {
        Debug.Log("[LOG] " + msg + " @ " + context.name);
    }

    public void SendLog(string context, string msg)
    {
        Debug.Log("[LOG] " + msg + " @ " + context);
    }

    // return elkonin value equivalent coin (action word or consonant)
    public GameWord GetGameWord(ElkoninValue value)
    {
        // check action words
        foreach (var actionWord in actionWords)
        {
            if (actionWord.elkoninValue == value)
                return actionWord;
        }

        // check consonant words
        foreach(var consonantWord in consonantWords)
        {
            if (consonantWord.elkoninValue == value)
                return consonantWord;
        }
        SendError (this, "Could not find elkonin value: \'" + value + "\'");
        return null;
    }

    // returns action word data from enum
    public ActionWord GetActionWord(ActionWordEnum word)
    {
        //print ("word: " + word);
        foreach (ActionWord actionWord in actionWords)
        {
            if (actionWord._enum.Equals(word))
                return actionWord;
        }
        SendError (this, "Could not find action word: \'" + word + "\'");
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
        LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void LoadScene(string sceneName, bool fadeOut, float time = transitionTime, bool useLoadScene = true)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("LoadingScene");
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeOut, time, useLoadScene));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool fadeOut, float time, bool useLoadScene)
    {
        if (fadeOut)
        {
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);

        // remove any popups
        TutorialPopupController.instance.StopAllPopups();
        // clean up scene
        SceneCleanup();

        SendLog(this, "loading new scene - \"" + sceneName + "\"");
        if (useLoadScene)
        {
            SceneManager.LoadSceneAsync("LoadingScene");
            StartCoroutine(DelayLoadScene(sceneName));
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    private IEnumerator DelayLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        LoadingSceneManager.instance.LoadNextScene(sceneName);
    }

    private void SceneCleanup()
    {
        // remove all sounds
        AudioManager.instance.ClearAllAudio();

        // remove star award window + levelPreview + toolbar 
        StarAwardController.instance.ResetWindow();
        DropdownToolbar.instance.ToggleToolbar(false);

        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();

        // remove default background
        DefaultBackground.instance.Deactivate();

        // remove talkies
        TalkieManager.instance.StopTalkieSystem();

        // remove ui buttons
        SettingsManager.instance.SetMenuButton(false);
        SettingsManager.instance.ToggleWagonButtonActive(false);

        // close settings windows if open
        SettingsManager.instance.CloseAllSettingsWindows();
        SettingsManager.instance.CloseAllConfirmWindows();
    }

    /*
    ################################################
    #   GAME DATA
    ################################################
    */

    public string GameTypeToSceneName(GameType gameType)
    {
        switch (gameType)
        {
            default:
                return "DevMenu";
            case GameType.FroggerGame:
                return "FroggerGame";
            case GameType.TurntablesGame:
                return "TurntablesGame";
            case GameType.RummageGame:
                return "RummageGame";
            case GameType.SeashellGame:
                return "SeaShellGame";
            case GameType.PirateGame:
                return "NewPirateGame";
            case GameType.SpiderwebGame:
                return "NewSpiderGame";


            case GameType.DevMenu:
                return "DevMenu";
            case GameType.StoryGame:
                return "StoryGame";
            case GameType.BoatGame:
                return "NewBoatGame";

            case GameType.WordFactoryBlending:
                return "WordFactoryBlending";
            case GameType.WordFactorySubstituting:
                return "WordFactorySubstituting";
            case GameType.WordFactoryBuilding:
                return "WordFactoryBuilding";
            case GameType.WordFactoryDeleting:
                return "WordFactoryDeleting";
            case GameType.TigerPawCoins:
                return "TigerPawCoins";
            case GameType.TigerPawPhotos:
                return "TigerPawPhotos";
            case GameType.Password:
                return "NewPasswordGame";
        }
    }
}