using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerGameManager : MonoBehaviour
{
    public static FroggerGameManager instance;

    [SerializeField] private GorillaController gorilla;
    [SerializeField] private Bag bag;
    [SerializeField] private TaxiController taxi;
    [SerializeField] private DancingManController dancingMan;
    private bool playingDancingManAnimation = false;
    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;

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


    [Header("Dev Stuff")]
    [SerializeField] private GameObject devObject;
    [SerializeField] private LogCoin devCoin; 

    void Awake() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        // dev object stuff
        devObject.SetActive(GameManager.instance.devModeActivated);

        PregameSetup();
        StartCoroutine(StartGame());
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            StartCoroutine(DancingManRoutine());
        }

        // dev stuff for fx audio testing
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(SkipToWinRoutine());
            }
        }
    }

    private IEnumerator DancingManRoutine()
    {
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
        //print ("dancing man animation -> " + selectedCoin.type);
        dancingMan.PlayUsingPhonemeEnum(selectedCoin.type);
        yield return new WaitForSeconds(1.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedCoin(LogCoin coin)
    {
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

    private IEnumerator CoinFailRoutine()
    {
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
        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.ToggleVisibility(false, true);
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
        gorilla.CelebrateAnimation();
        

        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator WinRoutine()
    {
        print ("you win!");
        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos(selectedCoin);
        taxi.CelebrateAnimation();
        bag.UpgradeBag();

        // remove selected coin
        selectedCoin.ToggleVisibility(false, true);
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
        yield return new WaitForSeconds(1.2f);
        gorilla.CelebrateAnimation(10f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(3f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    // This is a special fucntion that skips to the end of the game to win (DEV ONLY)
    private IEnumerator SkipToWinRoutine()
    {
        while (currRow < 4)
        {
            StartCoroutine(HideCoins(currRow));
            yield return new WaitForSeconds(0.5f);

            rows[currRow].SinkAllExcept(1);
            rows[currRow].MoveToCenterLog(1);
            yield return new WaitForSeconds(0.5f);

            gorilla.JumpForward(AudioDatabase.instance.WoodThump);
            
            currRow++;
            if (currRow < 4)
                rows[currRow].RiseAllLogs();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);

        gorilla.JumpForward(AudioDatabase.instance.GrassThump);

        yield return new WaitForSeconds(1.2f);
        gorilla.CelebrateAnimation(10f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(3f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private void PregameSetup()
    {
        // start ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.1f, "river_loop");

        // start song
        AudioManager.instance.PlaySong(AudioDatabase.instance.FroggerGameSong);
        AudioManager.instance.ChageMusicVolume(0.25f);

        // create coin list
        foreach (var coin in coins1)
            allCoins.Add(coin);
        foreach (var coin in coins2)
            allCoins.Add(coin);
        foreach (var coin in coins3)
            allCoins.Add(coin);
        foreach (var coin in coins4)
            allCoins.Add(coin);

        // Create Global Coin List
        globalCoinPool = GameManager.instance.GetGlobalActionWordList();
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins
        foreach (var coin in allCoins)
            coin.ToggleVisibility(false, false);

        // sink all the logs except the first row
        StartCoroutine(SinkLogsExceptFirstRow());
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

    private IEnumerator StartGame()
    {
        // wait a moment for the setup to finish
        while (!gameSetup)
            yield return null;

        currRow = 0;
        // show first row coins
        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator ShowCoins(int index)
    {
        List<LogCoin> row = GetCoinRow(index);
        foreach (var coin in row)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
                unusedCoinPool.AddRange(globalCoinPool);
            ActionWordEnum type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);

            yield return new WaitForSeconds(0.1f);
        }
        SelectRandomCoin(currRow);
    }

    private IEnumerator HideCoins(int index, LogCoin exceptCoin = null)
    {
        List<LogCoin> row = GetCoinRow(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<LogCoin> row = GetCoinRow(index);
        selectedIndex = Random.Range(0, row.Count);
        // print ("selected index: " + selectedIndex);
        selectedCoin = row[selectedIndex];
        //print ("selected coin -> " + selectedCoin.type);
        StartCoroutine(DancingManRoutine());

        if (GameManager.instance.devModeActivated)
        {
            devCoin.SetCoinType(selectedCoin.type);
        }
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
}
