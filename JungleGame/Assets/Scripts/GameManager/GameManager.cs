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
    public bool devModeActivated;
    public const float transitionTime = 0.5f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)
    public Vector2Int gameResolution;
    public Camera globalCamera;

    // game data
    [Header("Coin Objects")]
    public List<ActionWord> actionWords;
    public List<ConsonantWord> consonantWords;

    [Header("Game Datas")]
    public List<StoryGameData> storyGameDatas;
    private List<WordPair> subWordPairs;
    private List<WordPair> addDelWordPairs;
    
    [SerializeField] private GameObject devModeIndicator;
    private bool devIndicatorSet = false;

    public StoryGameData storyGameData;
    public MapIconIdentfier mapID;
    public GameType currChallengeGame;
    public List<ActionWordEnum> actionWordPool;
    public List<ChallengeWord> challengeWordPool;
    
    [HideInInspector] public bool repairMapIconID; // when the scroll map appears -> repair this icon
    [HideInInspector] public GameType prevGameTypePlayed = GameType.None;

    // challenge game identification
    [HideInInspector] public bool playingChallengeGame = false; // is player in a challenge game?

    void Start()
    {
        // set game resolution
        Screen.SetResolution(gameResolution.x, gameResolution.y, FullScreenMode.FullScreenWindow);
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
            // press 'Shift + D' to go to the dev menu
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    LoadScene("DevMenu", true);
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
        // clean up enviroment
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
        LoadScene(0, true);
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
        LoadScene("ScrollMap", true);
    }

    public void LoadScene(string sceneName, bool fadeOut, float time = transitionTime, bool useLoadScene = false)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("LoadingScene");
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeOut, time, useLoadScene));
    }

    public void LoadScene(int sceneNum, bool fadeOut, float time = transitionTime)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("LoadingScene");
        StartCoroutine(LoadSceneCoroutine(sceneNum, fadeOut, time));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool fadeOut, float time, bool useLoadScene)
    {
        if (fadeOut)
        {
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);

        if (useLoadScene)
        {
            SceneManager.LoadSceneAsync("LoadingScene");
            StartCoroutine(DelayLoadScene(sceneName));
        }
        else
        {
            SendLog(this, "Loading new scene - " + sceneName);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    private IEnumerator DelayLoadScene(string sceneName)
    {
        yield return new WaitForSeconds(2f);
        LoadingSceneManager.instance.LoadNextScene(sceneName);
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

        // remove default background
        DefaultBackground.instance.Deactivate();

        // remove talkies
        TalkieManager.instance.StopTalkieSystem();

        // remove ui buttons
        SettingsManager.instance.SetWagonButton(false);
        SettingsManager.instance.SetMenuButton(false);

        // remove wagon controller stuff
        WagonWindowController.instance.ResetWagonController();

        // close settings menu if open
        SettingsManager.instance.CloseSettingsWindow();
    }

    /*
    ################################################
    #   GAME DATA
    ################################################
    */

    public List<WordPair> GetSubstitutionWordPairs()
    {
        // if list already made - return it
        if (subWordPairs != null)
        {
            return subWordPairs;
        } 
        // else make list  
        else
        {
            // add all pairs to list and remove all non-sub pairs
            subWordPairs = new List<WordPair>();
            ChallengeWordDatabase.InitCreateGlobalPairList();
            subWordPairs.AddRange(ChallengeWordDatabase.globalWordPairs);
            foreach (var pair in subWordPairs)
            {
                if (pair.pairType != PairType.sub)
                {
                    subWordPairs.Remove(pair);
                }
            }
            return subWordPairs;
        }
    }

    public List<WordPair> GetAddDeleteWordPairs()
    {
        // if list already made - return it
        if (addDelWordPairs != null)
        {
            return addDelWordPairs;
        } 
        // else make list
        else
        {
            // add all pairs to list and remove all non-sub pairs
            addDelWordPairs = new List<WordPair>();
            ChallengeWordDatabase.InitCreateGlobalPairList();
            addDelWordPairs.AddRange(ChallengeWordDatabase.globalWordPairs);
            foreach (var pair in addDelWordPairs)
            {
                if (pair.pairType != PairType.del_add)
                {
                    addDelWordPairs.Remove(pair);
                }
            }
            return addDelWordPairs;
        }
    }

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
        }
    }
}