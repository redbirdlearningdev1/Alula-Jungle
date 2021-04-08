using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerGameManager : MonoBehaviour
{
    public static FroggerGameManager instance;

    [SerializeField] private GorillaController gorilla;
    [SerializeField] private Bag bag;
    [SerializeField] private TaxiController taxi;
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
        
    }

    public bool EvaluateSelectedCoin(Coin coin)
    {
        if (coin == selectedCoin)
        {
            // success! go on to the next row or win game if on last row
            if (currRow < 4)
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
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowCoins(currRow));
        SelectRandomCoin(currRow);
    }

    private IEnumerator CoinSuccessRoutine()
    {
        // TODO: animate coin into bag
        selectedCoin.SafeDisable();

        taxi.CelebrateAnimation();
        bag.UpgradeBag();
        StartCoroutine(HideCoins(currRow, selectedCoin));
        yield return new WaitForSeconds(1f);

        rows[currRow].SinkAllExcept(selectedIndex);
        rows[currRow].MoveToCenterLog(selectedIndex);
        yield return new WaitForSeconds(1f);

        gorilla.JumpForward();
        yield return new WaitForSeconds(1f);

        currRow++;
        rows[currRow].RiseAllLogs();
        yield return new WaitForSeconds(1f);

        StartCoroutine(ShowCoins(currRow));
    }

    private IEnumerator WinRoutine()
    {
        yield return null;
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
            coin.gameObject.SetActive(false);

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
            coin.gameObject.SetActive(true);

            // set random type
            if (unusedCoinPool.Count == 0)
                unusedCoinPool.AddRange(globalCoinPool);
            CoinType type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            print ("setting coinType: " + type);
            coin.SetCoinType(type);

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
                coin.SafeDisable();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<Coin> row = GetCoinRow(index);
        selectedIndex = Random.Range(0, coins1.Count);
        selectedCoin = row[selectedIndex];
        if (GameManager.instance.devModeActivated)
        {
            print ("dev coin set!");
            print ("selected coin index: " + selectedIndex);
            print ("selected coin type: " + selectedCoin.coinType);
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
