using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FroggerGameManager : MonoBehaviour
{
    public static FroggerGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;

    [SerializeField] private GorillaController gorilla;
    [SerializeField] private Bag bag;
    [SerializeField] private TaxiController taxi;
    [SerializeField] private DancingManController dancingMan;
    public Transform dancingManOffScreen;
    public Transform dancingManOnScreen;
    private bool playingDancingManAnimation = false;
    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;
    private ActionWordEnum prevCorrectCoin;

    private int timesMissed = 0;

    [Header("Log Rows")]
    [SerializeField] private List<LogRow> rows;
    private int currRow = 0;

    [Header("Coin Rows")]
    [SerializeField] private List<LogCoin> coins1;
    [SerializeField] private List<LogCoin> coins2;
    [SerializeField] private List<LogCoin> coins3;
    [SerializeField] private List<LogCoin> coins4;
    private List<LogCoin> allCoins = new List<LogCoin>();
    private int selectedIndex;
    private LogCoin selectedCoin;

    [Header("Tutorial Stuff")]
    public bool playTutorial;
    public int[] correctTutorialCoins;
    public List<ActionWordEnum> firstPair;
    public List<ActionWordEnum> secondTriad;
    public List<ActionWordEnum> thirdTriad;
    public List<ActionWordEnum> fourthTriad;

    private bool repeatTutorialAudio = false;
    private float timeBetweenRepeats = 8f;



    [Header("Dev Stuff")]
    [SerializeField] private GameObject devObject;
    [SerializeField] private LogCoin devCoin;

    private float startTime;


    /* 
    ################################################
    #   MONOBEHAVIOR METHODS
    ################################################
    */

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        if (!instance)
        {
            instance = this;
        }

        // get mapID
        mapID = GameManager.instance.mapID;
    }

    void Start()
    {
        // set start time
        startTime = Time.time;

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().froggerTutorial;

        PregameSetup();

        // play tutorial if first time
        if (playTutorial)
        {
            StartCoroutine(StartTutorial());
        }
        else
        {
            // start song
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.FroggerSongSplit);

            StartCoroutine(StartGame());
        }
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            StartCoroutine(DancingManRoutine());
        }

        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SkipGame();
                }
            }
        }
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        // times missed set to 0
        timesMissed = 0;
        // update AI data
        AIData(StudentInfoSystem.GetCurrentProfile());
        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();
    }

    /* 
    ################################################
    #   PREGAME SETUP
    ################################################
    */

    private void PregameSetup()
    {
        // start ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.1f, "river_loop");

        // place dancing man off-screen
        dancingMan.gameObject.transform.position = dancingManOffScreen.position;

        // turn off raycaster
        CoinRaycaster.instance.isOn = false;

        // create coin list
        foreach (var coin in coins1)
            allCoins.Add(coin);
        foreach (var coin in coins2)
            allCoins.Add(coin);
        foreach (var coin in coins3)
            allCoins.Add(coin);
        foreach (var coin in coins4)
            allCoins.Add(coin);

        globalCoinPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (GameManager.instance.practiceModeON)
        {
            globalCoinPool.AddRange(GameManager.instance.practicePhonemes);
        }
        else if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }

        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins + glow controller
        foreach (var coin in allCoins)
        {
            coin.GetComponent<LerpableObject>().SetImageAlpha(coin.image, 0f);
        }

        // sink all the logs except the first row
        StartCoroutine(SinkLogsExceptFirstRow());
    }

    /* 
    ################################################
    #   GAME FLOW METHODS
    ################################################
    */

    private IEnumerator CoinFailRoutine()
    {
        // decrease split song
        AudioManager.instance.DecreaseSplitSong();

        // inc times missed
        timesMissed++;

        rows[currRow].ResetCoinPos(null);
        taxi.TwitchAnimation();
        bag.DowngradeBag();
        StartCoroutine(HideCoins(currRow));
        yield return new WaitForSeconds(0.5f);

        // play reminder popup
        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.froggerReminder1);
        clips.Add(GameIntroDatabase.instance.froggerReminder2);
        clips.Add(GameIntroDatabase.instance.froggerEncouragementClips[1]);

        AssetReference clip = clips[Random.Range(0, clips.Count)];
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Darwin, clip);
        //yield return new WaitForSeconds(cd.GetResult());

        // sink logs
        rows[currRow].SinkAllLogs();
        yield return new WaitForSeconds(0.5f);

        // determine correct landing sound
        AssetReference landingSound = null;
        //print("curr row: "  + currRow);
        if (currRow < 2)
            landingSound = AudioDatabase.instance.GrassThump;
        else
            landingSound = AudioDatabase.instance.WoodThump;
        gorilla.JumpBack(landingSound);

        yield return new WaitForSeconds(0.5f);

        if (currRow > 0)
            currRow--;
        rows[currRow].RiseAllLogs();
        rows[currRow].ResetLogRow();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator CoinSuccessRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.GetComponent<LerpableObject>().LerpImageAlpha(selectedCoin.image, 0f, 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpPosToTransform(Bag.instance.transform, 0.25f, false);
        yield return new WaitForSeconds(0.5f);
        selectedCoin.ReturnToLog();

        // add coin to successful coins
        prevCorrectCoin = selectedCoin.type;
        selectedCoin = null;

        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(0.5f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(0.5f);

        gorilla.JumpForward(AudioDatabase.instance.WoodThump);
        yield return new WaitForSeconds(1.25f);
        gorilla.CelebrateAnimation();

        // play encouragement popup
        if (GameManager.DeterminePlayPopup())
        {
            AssetReference clip = null;
            switch (currRow)
            {
                case 0:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[0];
                    break;
                case 1:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[2];
                    break;
                case 2:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[3];
                    break;
            }
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Darwin, clip);
            //yield return new WaitForSeconds(cd.GetResult());
        }
        
        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator WinRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.GetComponent<LerpableObject>().LerpImageAlpha(selectedCoin.image, 0f, 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpPosToTransform(Bag.instance.transform, 0.25f, false);
        yield return new WaitForSeconds(0.5f);
        selectedCoin.ReturnToLog();
        selectedCoin = null;

        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(0.5f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(0.5f);

        gorilla.JumpForward(AudioDatabase.instance.WoodThump);
        yield return new WaitForSeconds(1.25f);

        gorilla.JumpForward(AudioDatabase.instance.GrassThump);
        yield return new WaitForSeconds(1.25f);
        gorilla.CelebrateAnimation(10f);
        taxi.CelebrateAnimation(10f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(1f);

        // hide dancing man
        StartCoroutine(HideDancingManRoutine());

        // AI stuff
        AIData(StudentInfoSystem.GetCurrentProfile());

        int starsAwarded = CalculateStars();
        float elapsedTime = Time.time - startTime;

        //// ANALYTICS : send minigame_completed event
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "minigame_name", GameType.FroggerGame.ToString() },
            { "stars_awarded", starsAwarded },
            { "elapsed_time", elapsedTime },
            { "tutorial_played", false },
            { "prev_times_played", data.froggerPlayed },
            { "curr_storybeat", data.currStoryBeat.ToString() }
        };            
        AnalyticsManager.SendCustomEvent("minigame_completed", parameters);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(starsAwarded);
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.starsGameBeforeLastPlayed = playerData.starsLastGamePlayed;
        playerData.starsLastGamePlayed = CalculateStars();
        playerData.gameBeforeLastPlayed = playerData.lastGamePlayed;
        playerData.lastGamePlayed = GameType.FroggerGame;
        playerData.starsFrogger = CalculateStars() + playerData.starsFrogger;
        playerData.froggerPlayed++;

        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
    }

    private int CalculateStars()
    {
        if (timesMissed <= 0)
            return 3;
        else if (timesMissed > 0 && timesMissed <= 2)
            return 2;
        else
            return 1;
    }

    private IEnumerator StartGame()
    {
        // wait a moment for the setup to finish
        while (!gameSetup)
            yield return null;

        // show settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // reveal dancing man
        StartCoroutine(ShowDancingManRoutine());
        yield return new WaitForSeconds(1f);

        currRow = 0;
        // show first row coins
        StartCoroutine(ShowCoins(currRow));
    }

    /* 
    ################################################
    #   TUTORIAL METHODS
    ################################################
    */

    private IEnumerator StartTutorial()
    {
        // wait a moment for the setup to finish
        while (!gameSetup)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // play tutorial audio
        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.froggerIntro1);
        clips.Add(GameIntroDatabase.instance.froggerIntro2);
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
        yield return cd.coroutine;
        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
        yield return cd0.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Darwin, clips);
        yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + 1f);

        // reveal dancing man
        StartCoroutine(ShowDancingManRoutine());
        yield return new WaitForSeconds(0.5f);

        // show settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        currRow = 0;
        ShowTutorialCoins();
    }

    private IEnumerator RepeatTutorialAudioRoutine(AssetReference clip)
    {
        // play initially
        AudioManager.instance.PlayTalk(clip);

        // repeat until bool is false
        float timer = 0f;
        repeatTutorialAudio = true;
        while (repeatTutorialAudio)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenRepeats)
            {
                AudioManager.instance.PlayTalk(clip);
                timer = 0f;
            }
            yield return null;
        }
    }

    private IEnumerator ContinueTutorialRoutine()
    {
        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.GetComponent<LerpableObject>().LerpImageAlpha(selectedCoin.image, 0f, 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpPosToTransform(Bag.instance.transform, 0.25f, false);
        yield return new WaitForSeconds(0.5f);
        selectedCoin.ReturnToLog();
        selectedCoin = null;

        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(0.5f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(0.5f);

        gorilla.JumpForward(AudioDatabase.instance.WoodThump);
        yield return new WaitForSeconds(1f);
        gorilla.CelebrateAnimation();

        // play tutorial audio
        if (currRow == 0)
        {
            // play tutorial audio
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.froggerIntro3);
            clips.Add(GameIntroDatabase.instance.froggerIntro4);
            clips.Add(GameIntroDatabase.instance.froggerIntro5);
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;
            CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd0.coroutine;
            CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[2]));
            yield return cd1.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Darwin, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + cd1.GetResult() + 1f);
        }
        else
        {
            // play encouragement popup
            AssetReference clip = null;
            switch (currRow)
            {
                case 0:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[0];
                    break;
                case 1:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[2];
                    break;
                case 2:
                    clip = GameIntroDatabase.instance.froggerEncouragementClips[3];
                    break;
            }
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Darwin, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);
        }

        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(1f);

        ShowTutorialCoins();
    }

    private IEnumerator TryAgainRoutine()
    {
        rows[currRow].ResetCoinPos(null);
        taxi.TwitchAnimation();
        StartCoroutine(HideCoins(currRow));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllLogs();
        yield return new WaitForSeconds(1f);

        rows[currRow].RiseAllLogs();
        rows[currRow].ResetLogRow();
        yield return new WaitForSeconds(1f);

        ShowTutorialCoins();
    }

    private IEnumerator CompleteTutorialRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        print("you win!");
        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.GetComponent<LerpableObject>().LerpImageAlpha(selectedCoin.image, 0f, 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
        selectedCoin.GetComponent<LerpableObject>().LerpPosToTransform(Bag.instance.transform, 0.25f, false);
        yield return new WaitForSeconds(1f);
        selectedCoin.ReturnToLog();
        selectedCoin = null;

        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward(AudioDatabase.instance.WoodThump);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward(AudioDatabase.instance.GrassThump);
        yield return new WaitForSeconds(1.25f);
        gorilla.CelebrateAnimation(10f);
        taxi.CelebrateAnimation(10f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(3f);

        // hide dancing man
        StartCoroutine(HideDancingManRoutine());

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = true;
        StudentInfoSystem.SaveStudentPlayerData();

        float elapsedTime = Time.time - startTime;

        //// ANALYTICS : send minigame_completed event
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "minigame_name", GameType.FroggerGame.ToString() },
            { "stars_awarded", 0 },
            { "elapsed_time", elapsedTime },
            { "tutorial_played", true },
            { "prev_times_played", data.froggerPlayed },
            { "curr_storybeat", data.currStoryBeat.ToString() }
        };            
        AnalyticsManager.SendCustomEvent("minigame_completed", parameters);

        GameManager.instance.LoadScene("FroggerGame", true, 3f);
    }

    private void ShowTutorialCoins()
    {
        // show correct row of coins
        switch (currRow)
        {
            case 0:
                StartCoroutine(ShowCoins(currRow, firstPair));
                break;
            case 1:
                StartCoroutine(ShowCoins(currRow, secondTriad));
                break;
            case 2:
                StartCoroutine(ShowCoins(currRow, thirdTriad));
                break;
            case 3:
                StartCoroutine(ShowCoins(currRow, fourthTriad));
                break;
        }

        StartCoroutine(PlayTutorialAudio());
    }

    private IEnumerator PlayTutorialAudio()
    {
        yield return null;

        // turn on raycaster
        CoinRaycaster.instance.isOn = true;
    }


    /* 
    ################################################
    #   COIN CONTROL METHODS
    ################################################
    */

    public bool EvaluateSelectedCoin(LogCoin coin)
    {
        // turn off raycaster
        CoinRaycaster.instance.isOn = false;

        // Tutorial evaluation
        if (playTutorial)
        {
            if (coin == selectedCoin)
            {
                // stop repeating audio
                repeatTutorialAudio = false;

                // success! continue or complete tutorial
                if (currRow < 3)
                    StartCoroutine(ContinueTutorialRoutine());
                else
                    StartCoroutine(CompleteTutorialRoutine());
                return true;
            }

            // fail - try again!
            StartCoroutine(TryAgainRoutine());
            return false;
        }

        bool success = (coin == selectedCoin);
        // only track phoneme attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            StudentInfoSystem.SavePlayerMinigameRoundAttempt(GameType.FroggerGame, success);
            StudentInfoSystem.SavePlayerPhonemeAttempt(coin.type, success);
        }

        if (success)
        {
            // success! go on to the next row or win game if on last row
            if (currRow < 3)
                StartCoroutine(CoinSuccessRoutine());
            else
                StartCoroutine(WinRoutine());
            return true;
        }
        // fail go back to previous row
        StartCoroutine(CoinFailRoutine());
        return false;
    }

    private IEnumerator ShowCoins(int index, List<ActionWordEnum> coinTypes = null)
    {
        List<LogCoin> row = GetCoinRow(index);

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        int idx = 0;
        foreach (var coin in row)
        {
            ActionWordEnum type = ActionWordEnum._blank;

            // use predetermined coin types
            if (coinTypes != null)
            {
                //print ("setting coin type: " +  coinTypes[idx]);
                type = coinTypes[idx];
            }
            // set random type
            else
            {
                type = GetUnusedWord();
            }

            coin.SetCoinType(type);
            coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 1f, 0.5f);

            yield return new WaitForSeconds(0.1f);
            idx++;
        }

        // select the correct coin
        if (playTutorial)
        {
            SelectCoin(currRow, correctTutorialCoins[currRow]);
        }
        else
        {
            SelectCoin(currRow);
        }

        // turn on raycaster
        CoinRaycaster.instance.isOn = true;
    }

    private ActionWordEnum GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedCoinPool.Count <= 0)
        {
            unusedCoinPool.Clear();
            unusedCoinPool.AddRange(globalCoinPool);
        }

        int index = Random.Range(0, unusedCoinPool.Count);
        ActionWordEnum word = unusedCoinPool[index];

        // make sure word is not being used or already successfully completed
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }

    private IEnumerator HideCoins(int index, LogCoin exceptCoin = null)
    {
        List<LogCoin> row = GetCoinRow(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0f, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // select a coin to be the correct coin
    private LogCoin SelectCoin(int rowIndex, int coinIndex = -1)
    {
        List<LogCoin> row = GetCoinRow(rowIndex);

        // select random coin
        selectedIndex = Random.Range(0, row.Count);

        // make sure selected coin is not the same as prev selected coin
        if (rows[currRow].coins[selectedIndex].type == prevCorrectCoin)
        {
            selectedIndex++;
            if (selectedIndex >= row.Count)
                selectedIndex = 0;
        }

        // if coin index is valid - use that indead of random choice
        //print ("coin index: " + coinIndex);
        if (coinIndex != -1)
        {
            if (coinIndex > -1 && coinIndex < row.Count)
            {
                selectedIndex = coinIndex;
                //print ("not random");
            }
        }

        // print ("selected index -> " + selectedIndex);
        selectedCoin = row[selectedIndex];
        // print ("selected coin -> " + selectedCoin.type);
        StartCoroutine(DancingManRoutine());


        if (GameManager.instance.devModeActivated)
        {
            devCoin.SetCoinType(selectedCoin.type);
        }

        return selectedCoin;
    }

    private List<LogCoin> GetCoinRow(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return coins1;
            case 1:
                return coins2;
            case 2:
                return coins3;
            case 3:
                return coins4;
        }
    }

    /* 
    ################################################
    #   MISC UTIL FUNCTIONS
    ################################################
    */

    public void ToggleWally(bool opt)
    {
        if (opt)
        {
            StartCoroutine(ShowDancingManRoutine());
        }
        else
        {
            StartCoroutine(HideDancingManRoutine());
        }
    }

    private IEnumerator ShowDancingManRoutine()
    {
        float timer = 0f;
        float moveTime = 0.3f;
        Vector3 bouncePos = dancingManOnScreen.position;
        bouncePos.y += 0.5f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(dancingManOffScreen.position, bouncePos, timer / moveTime);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(bouncePos, dancingManOnScreen.position, timer / 0.1f);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
    }

    private IEnumerator HideDancingManRoutine()
    {
        float timer = 0f;
        float moveTime = 0.3f;
        Vector3 bouncePos = dancingManOnScreen.position;
        bouncePos.y += 0.5f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(dancingManOnScreen.position, bouncePos, timer / moveTime);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(bouncePos, dancingManOffScreen.position, timer / 0.1f);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
    }

    // used to control dancing man's animations
    private IEnumerator DancingManRoutine()
    {
        // return if coin is null
        if (selectedCoin == null)
            yield break;
        // dont play if already playing animation
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
        // print ("dancing man animation -> " + selectedCoin.type);
        dancingMan.PlayUsingPhonemeEnum(selectedCoin.type);
        yield return new WaitForSeconds(2.5f);
        playingDancingManAnimation = false;
    }

    private IEnumerator SinkLogsExceptFirstRow()
    {
        rows[3].SinkAllLogs();
        yield return new WaitForSeconds(0.2f);
        rows[2].SinkAllLogs();
        yield return new WaitForSeconds(0.2f);
        rows[1].SinkAllLogs();
        yield return new WaitForSeconds(1f);
        // game is done setting up
        gameSetup = true;
    }
}
