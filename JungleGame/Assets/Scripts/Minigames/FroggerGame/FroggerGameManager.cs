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

    private List<CoinType> globalCoinPool;
    private List<CoinType> unusedCoinPool;

    [Header("Log Rows")]
    [SerializeField] private List<LogRow> rows; 
    private int currRow = 0;
    

    [Header("Coin Rows")]
    [SerializeField] private List<Coin> coins1;
    [SerializeField] private List<Coin> coins2;
    [SerializeField] private List<Coin> coins3;
    [SerializeField] private List<Coin> coins4;
    private List<Coin> allCoins = new List<Coin>();
    private int selectedIndex;
    private Coin selectedCoin;


    [Header("Dev Stuff")]
    [SerializeField] private GameObject devObject;
    [SerializeField] private Coin devCoin; 

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
    }

    private IEnumerator DancingManRoutine()
    {
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
        dancingMan.PlayUsingPhonemeEnum((Phoneme)selectedCoin.coinType);
        yield return new WaitForSeconds(1.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedCoin(Coin coin)
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
        rows[currRow].ResetCoinPos();
        taxi.TwitchAnimation();
        bag.DowngradeBag();
        StartCoroutine(HideCoins(currRow));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllLogs();
        yield return new WaitForSeconds(1f);

        gorilla.JumpBack();
        yield return new WaitForSeconds(1f);

        if (currRow > 0)
            currRow--;
        rows[currRow].RiseAllLogs();
        rows[currRow].ResetLogRow();
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowCoins(currRow));
        SelectRandomCoin(currRow);
    }

    private IEnumerator CoinSuccessRoutine()
    {
        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos();
        taxi.CelebrateAnimation();
        bag.UpgradeBag();
        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward();
        yield return new WaitForSeconds(0.5f);
        selectedCoin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(0.7f);
        gorilla.CelebrateAnimation();
        selectedCoin = null;

        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator WinRoutine()
    {
        print ("you win!");
        // TODO: animate coin into bag
        rows[currRow].ResetCoinPos();
        taxi.CelebrateAnimation();
        bag.UpgradeBag();
        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward();
        yield return new WaitForSeconds(0.5f);
        selectedCoin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(0.5f);
        selectedCoin = null;

        gorilla.JumpForward();
        yield return new WaitForSeconds(1.2f);
        gorilla.CelebrateAnimation(5f);
    }

    private void PregameSetup()
    {
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
        globalCoinPool = new List<CoinType>();
        unusedCoinPool = new List<CoinType>();
        string[] coins = System.Enum.GetNames(typeof(CoinType));
        for (int i = 0; i < coins.Length; i++) 
        {
            CoinType coin = (CoinType)System.Enum.Parse(typeof(CoinType), coins[i]);
            globalCoinPool.Add(coin);
        }
        globalCoinPool.Remove(CoinType.COUNT);
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
        List<Coin> row = GetCoinRow(index);
        foreach (var coin in row)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
                unusedCoinPool.AddRange(globalCoinPool);
            CoinType type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);

            yield return new WaitForSeconds(0.1f);
        }
        SelectRandomCoin(currRow);
    }

    private IEnumerator HideCoins(int index, Coin exceptCoin = null)
    {
        List<Coin> row = GetCoinRow(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<Coin> row = GetCoinRow(index);
        selectedIndex = Random.Range(0, row.Count);
        print ("selected index: " + selectedIndex);
        selectedCoin = row[selectedIndex];
        StartCoroutine(DancingManRoutine());

        if (GameManager.instance.devModeActivated)
        {
            devCoin.SetCoinType(selectedCoin.coinType);
        }
    }

    private List<Coin> GetCoinRow(int index)
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
