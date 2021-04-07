using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerGameManager : MonoBehaviour
{
    [SerializeField] private GorillaController gorilla;
    [SerializeField] private Bag bag;
    private int currRow = 0;
    private bool gameSetup = false;

    [Header("Log Rows")]
    [SerializeField] private LogRow row1; 
    [SerializeField] private LogRow row2; 
    [SerializeField] private LogRow row3; 
    [SerializeField] private LogRow row4; 

    [Header("Coin Rows")]
    [SerializeField] private List<Coin> coins1;
    [SerializeField] private List<Coin> coins2;
    [SerializeField] private List<Coin> coins3;
    [SerializeField] private List<Coin> coins4;
    private List<Coin> allCoins = new List<Coin>();

    void Awake() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        PregameSetup();
        StartCoroutine(StartGame());
    }

    void Update()
    {
        
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

        // Randomize all coins
        List<CoinType> coinList = new List<CoinType>();
        string[] coins = System.Enum.GetNames(typeof(CoinType));
        for (int i = 0; i < coins.Length; i++) 
        {
            CoinType coin = (CoinType)System.Enum.Parse(typeof(CoinType), coins[i]);
            coinList.Add(coin);
        }
        coinList.Remove(CoinType.COUNT);

        foreach(var coin in allCoins)
        {
            int index = Random.Range(0, coinList.Count);
            CoinType type = coinList[index];
            coinList.RemoveAt(index);
            coin.SetCoinType(type);
        }

        // disable all coins
        foreach (var coin in allCoins)
            coin.gameObject.SetActive(false);

        // sink all the logs except the first row
        StartCoroutine(SinkLogsExceptFirstRow());
    }

    private IEnumerator SinkLogsExceptFirstRow()
    {
        row4.SinkAllLogs();
        yield return new WaitForSeconds(0.2f);
        row3.SinkAllLogs();
        yield return new WaitForSeconds(0.2f);
        row2.SinkAllLogs();
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
        StartCoroutine(ShowCoins(coins1));
    }

    private IEnumerator ShowCoins(List<Coin> coins)
    {
        foreach (var coin in coins)
        {
            coin.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
