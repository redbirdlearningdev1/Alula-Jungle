using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool playInEditor;
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
        if (!playInEditor)
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
            AudioManager.instance.InitSplitSong(SplitSong.Frogger);
            AudioManager.instance.IncreaseSplitSong();

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                // play win tune
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(3);
            }
        }
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
        if (mapID != MapIconIdentfier.None)
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
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllLogs();
        yield return new WaitForSeconds(1f);
        
        // determine correct landing sound
        AudioClip landingSound = null;
        //print("curr row: "  + currRow);
        if (currRow < 2)
            landingSound = AudioDatabase.instance.GrassThump;
        else
            landingSound = AudioDatabase.instance.WoodThump;
        gorilla.JumpBack(landingSound);

        yield return new WaitForSeconds(1f);

        if (currRow > 0)
            currRow--;
        rows[currRow].RiseAllLogs();
        rows[currRow].ResetLogRow();
        yield return new WaitForSeconds(1f);

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
        yield return new WaitForSeconds(1f);
        selectedCoin.ReturnToLog();

        // add coin to successful coins
        prevCorrectCoin = selectedCoin.type;
        selectedCoin = null;

        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward(AudioDatabase.instance.WoodThump);
        yield return new WaitForSeconds(1.25f);
        gorilla.CelebrateAnimation();

        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(1f);

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

        yield return new WaitForSeconds(2f);

        // hide dancing man
        StartCoroutine(HideDancingManRoutine());

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
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

        // show menu button
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

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // play tutorial audio
        AudioClip clip = AudioDatabase.instance.FroggerTutorial_1;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 0.5f);

        // play tutorial audio
        clip = AudioDatabase.instance.FroggerTutorial_2;
        StartCoroutine(RepeatTutorialAudioRoutine(clip));
        yield return new WaitForSeconds(clip.length + 0.5f);

        // reveal dancing man
        StartCoroutine(ShowDancingManRoutine());
        yield return new WaitForSeconds(1f);

        currRow = 0;
        ShowTutorialCoins();
    }

    private IEnumerator RepeatTutorialAudioRoutine(AudioClip clip)
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
        yield return new WaitForSeconds(1.25f);
        gorilla.CelebrateAnimation();
        
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

        print ("you win!");
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
        // play audio iff currRow == 1
        if (currRow == 1)
        {
            AudioClip clip = AudioDatabase.instance.FroggerTutorial_3;
            AudioManager.instance.PlayTalk(clip);
            yield return new WaitForSeconds(clip.length + 0.5f);
        }

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

        if (coin == selectedCoin)
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
        yield return new WaitForSeconds(2f);
        // game is done setting up
        gameSetup = true;
    }
}
