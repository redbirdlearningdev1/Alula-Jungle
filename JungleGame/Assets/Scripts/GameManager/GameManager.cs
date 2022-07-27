﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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
    public static string currentGameVersion = "alpha1.7";

    public static int stickerInventorySize = 16;

    public static float popup_probability = 0.2f;

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
    public List<AssetReferenceAtlasedSprite> avatars;

    [HideInInspector]
    public bool neverSleep = false;

    public int sleepSeconds = 10;
    public GameObject console;
    public Text consoleText;
    private Coroutine sleepCoroutine;

    [Header("Practice Mode")]
    public bool practiceModeON = false;
    public TextMeshProUGUI practiceModeCounter;
    private List<GameType> practiceGameQueue;
    [HideInInspector] public int practiceDifficulty;
    [HideInInspector] public List<ActionWordEnum> practicePhonemes;
    private int practiceTotalGames;
    

    void Start()
    {
#if !(UNITY_IOS || UNITY_ANDROID)
        // set game resolution
        Screen.SetResolution(GameAwake.gameResolution.x, GameAwake.gameResolution.y, true);
#endif

        // init mic
        MicInput.instance.InitMic();
        MicInput.instance.StopMicrophone();

        // set default volumes
        AudioManager.instance.SetMasterVolume(AudioManager.default_masterVol);
        AudioManager.instance.SetMusicVolume(AudioManager.default_musicVol);
        AudioManager.instance.SetFXVolume(AudioManager.default_fxVol);
        AudioManager.instance.SetTalkVolume(AudioManager.default_talkVol);

        if (devModeActivated)
        {
            SendLog(this, "Dev Mode set as - ON");
        }
        else
        {
            SendLog(this, "Dev Mode set as - OFF");
        }

        Screen.sleepTimeout = sleepSeconds;
        neverSleep = false;
        
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
        /*
        string sleepTimeout = "";
        if (Screen.sleepTimeout == -1)
            sleepTimeout = "NeverSleep";
        else if (Screen.sleepTimeout == -2)
            sleepTimeout = "System Default";
        else sleepTimeout = "" + Screen.sleepTimeout;


        consoleText.text = "Battery Status: " + SystemInfo.batteryStatus;
        consoleText.text += "\nSleep Status: " + sleepTimeout;
        consoleText.text += "\nSleep bool: " + neverSleep;
        */

        if ((SystemInfo.batteryStatus == BatteryStatus.Charging || SystemInfo.batteryStatus == BatteryStatus.Full) && !neverSleep)
        {
            consoleText.color = Color.red;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (sleepCoroutine != null)
            {
                StopCoroutine(sleepCoroutine);
                sleepCoroutine = null;
            }
            neverSleep = true;
        }
        if (!(SystemInfo.batteryStatus == BatteryStatus.Charging || SystemInfo.batteryStatus == BatteryStatus.Full))
        {
            if (neverSleep)
            {
                neverSleep = false;
                if (sleepCoroutine != null)
                {
                    StopCoroutine(sleepCoroutine);
                    sleepCoroutine = null;
                }
                sleepCoroutine = StartCoroutine(SleepCoroutine());
            }
            else if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    if (sleepCoroutine != null)
                    {
                        StopCoroutine(sleepCoroutine);
                        sleepCoroutine = null;
                    }
                    sleepCoroutine = StartCoroutine(SleepCoroutine());
                }
            }
        }
    }

    IEnumerator SleepCoroutine()
    {
        consoleText.color = Color.blue;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        yield return new WaitForSeconds(sleepSeconds);
        consoleText.color = Color.green;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }


    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {

        }
        else
        {

        }
    }

    private IEnumerator LogDateTimeNow(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.LogError("it is currently " + System.DateTime.Now);
    }

    /* 
################################################
# SCENE INITIALIZATION
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

        // show practice mode counter
        if (practiceModeON && SceneManager.GetActiveScene().name != "LoadingScene")
        {
            practiceModeCounter.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
    }

    /* 
################################################
# UTILITY
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
        foreach (var consonantWord in consonantWords)
        {
            if (consonantWord.elkoninValue == value)
                return consonantWord;
        }
        SendError(this, "Could not find elkonin value: \'" + value + "\'");
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

    public void SkipCurrentGame()
    {
        // get current scene
        string currentScene = SceneManager.GetActiveScene().name;
        
        switch (currentScene)
        {
            default: return;

            // other games
            case "StoryGame": StoryGameManager.instance.SkipGame(); break;
            case "NewBoatGame": NewBoatGameManager.instance.SkipGame(); break;

            // minigames:
            case "FroggerGame": FroggerGameManager.instance.SkipGame(); break;
            case "SeaShellGame": SeaShellGameManager.instance.SkipGame(); break;
            case "RummageGame": RummageGameManager.instance.SkipGame(); break;
            case "NewPirateGame": PrintingGameManager.instance.SkipGame(); break;
            case "TurntablesGame": TurntablesGameManager.instance.SkipGame(); break;
            case "NewSpiderGame": NewSpiderGameManager.instance.SkipGame(); break;

            // challenge games
            case "WordFactoryDeleting": WordFactoryDeletingManager.instance.SkipGame(); break;
            case "WordFactorySubstituting": WordFactorySubstitutingManager.instance.SkipGame(); break;
            case "WordFactoryBuilding": WordFactoryBuildingManager.instance.SkipGame(); break;
            case "WordFactoryBlending": WordFactoryBlendingManager.instance.SkipGame(); break;

            case "TigerPawCoins": TigerCoinGameManager.instance.SkipGame(); break;
            case "TigerPawPhotos": TigerGameManager.instance.SkipGame(); break;

            case "NewPasswordGame": NewPasswordGameManager.instance.SkipGame(); break;
        }
    }

    public static bool DeterminePlayPopup()
    {
        float num = Random.Range(0f, 1f);
        print ("num: " + num);
        if (num < popup_probability)
            return true;
        return false;
    }

    /* 
################################################
# SCENE MANAGEMENT
################################################
    */

    public void GoToDevMenu()
    {
        LoadScene("DevMenu", true, 0.5f, true);
    }

    public void OpenConsole()
    {
        console.SetActive(!console.activeSelf);
    }


    public void ReturnToScrollMap()
    {
        LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void EndStarAwardScreen()
    {
        if (practiceModeON)
        {
            ContinuePracticeMode();
        }
        else
        {
            LoadScene("ScrollMap", true, 0.5f, true);
        }
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
        yield return new WaitForSeconds(1.5f);
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
# GAME DATA
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

    /* 
################################################
# PRACTICE MODE
################################################
    */

    public void SetPracticeMode(List<GameType> gameQueue, int diff, List<ActionWordEnum> phonemes)
    {
        // turn on practice mode and copy over data
        practiceModeON = true;
        practiceGameQueue = new List<GameType>();
        practiceGameQueue.AddRange(gameQueue);
        practiceDifficulty = diff;
        practicePhonemes = new List<ActionWordEnum>();
        practicePhonemes.AddRange(phonemes);
        // set counter
        practiceTotalGames = practiceGameQueue.Count;
        practiceModeCounter.text =  practiceTotalGames + "/" + practiceTotalGames;
    }

    public void ContinuePracticeMode()
    {
        if (practiceGameQueue.Count > 0)
        {
            practiceModeCounter.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

            // update text
            practiceModeCounter.text =  practiceGameQueue.Count + "/" + practiceTotalGames;

            // load next game in queue
            GameType nextGame = practiceGameQueue[practiceGameQueue.Count - 1];
            practiceGameQueue.RemoveAt(practiceGameQueue.Count - 1);
            LoadScene(GameTypeToSceneName(nextGame), true);
        }
        else
        {
            // return to practice mode scene
            practiceModeON = false;
            practiceModeCounter.text = "";
            LoadScene("PracticeScene", true);
        }
    }
}