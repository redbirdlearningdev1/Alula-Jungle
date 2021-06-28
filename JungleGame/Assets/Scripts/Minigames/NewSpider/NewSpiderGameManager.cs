using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpiderGameManager : MonoBehaviour
{
    public static NewSpiderGameManager instance;

    [SerializeField] private NewSpiderController spider;
    [SerializeField] private WebBall ball;
    [SerializeField] private BugController bug;
    [SerializeField] private WebberController webber;
    [SerializeField] private WebberController webber2;
    [SerializeField] private WebController web;
    [SerializeField] private SpiderRayCaster caster;


    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;




    [SerializeField] private List<SpiderCoin> Coins;


    private List<SpiderCoin> allCoins = new List<SpiderCoin>();
    private int selectedIndex;
    public SpiderCoin selectedSpiderCoin;

    private int winCount = 0;

    private bool firstTimePlaying = true;


    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;


        }

        // dev object stuff
        //devObject.SetActive(GameManager.instance.devModeActivated);

        PregameSetup();
        StartCoroutine(StartGame(0));
        //StartCoroutine(StartGame(0));
    }

    void Update()
    {
        
    }

    

    public bool EvaluateSelectedSpiderCoin(ActionWordEnum coin,SpiderCoin correctCoin)
    {
        Debug.Log(selectedSpiderCoin.type);
        if (coin == selectedSpiderCoin.type)
        {
            // success! go on to the next row or win game if on last row
            Debug.Log("YOU DID IT");
            if (winCount < 5)
            {
                
                StartCoroutine(CoinSuccessRoutine(correctCoin));
            }
            else
            {
                Debug.Log("YOU DID IT AGAIn");
                StartCoroutine(WinRoutine());
            }

            return true;
        }
        StartCoroutine(CoinFailRoutine());
        return false;
    }


    private IEnumerator CoinFailRoutine()
    {
        
        bug.die();
        yield return new WaitForSeconds(.5f);
        spider.fail();
        webber2.gameObject.SetActive(true);
        webber2.grabBug();
        yield return new WaitForSeconds(.5f);
        bug.webGetEat();
        yield return new WaitForSeconds(.5f);
        webber2.gameObject.SetActive(false);
        StartCoroutine(StartGame(0));
    }

    private IEnumerator CoinSuccessRoutine(SpiderCoin coin)
    {
        spider.success();
        yield return new WaitForSeconds(1f);
        webber.gameObject.SetActive(true);
        if(selectedIndex == 0)
        {
            webber.grab1();
        }
        else if(selectedIndex == 1)
        {
            webber.grab2();
        }
        else if (selectedIndex == 2)
        {
            webber.grab3();
        }
        else if (selectedIndex == 3)
        {
            webber.grab4();
        }
        yield return new WaitForSeconds(.5f);
        
        coin.correct();
        yield return new WaitForSeconds(.15f);

        StartCoroutine(bugLeaves());
        webber.gameObject.SetActive(false);
        yield return new WaitForSeconds(.35f);
        coin.ToggleVisibility(false, false);
        ball.UpgradeChest();
        
        yield return new WaitForSeconds(1.5f);
        
        StartCoroutine(StartGame(0));
    }
    private IEnumerator bugLeaves()
    {
        web.webLarge();
        yield return new WaitForSeconds(.4f);
        bug.takeOff();
        yield return new WaitForSeconds(.1f);
        bug.leaveWeb();
        yield return new WaitForSeconds(.75f);
        bug.grow();
        bug.leaveWeb2();
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private void PregameSetup()
    {
        // create coin list
        bug.setOrigin();
        foreach (var coin in Coins)
        {
            //Debug.Log(coin);
            allCoins.Add(coin);
        }


        // Create Global Coin List
        globalCoinPool = GameManager.instance.GetGlobalActionWordList();
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins
        foreach (var coin in allCoins)
        {
            
            coin.setOrigin();
            Debug.Log(coin.transform.position);
        }



        foreach (var coin in allCoins)
        {
            //coin.gameObject.SetActive(false);
        }



    }

    private void walkThrough()
    {
       
    }



    private IEnumerator StartGame(int coins)
    {
        StartCoroutine(CoinsDown(coins));
        bug.goToOrigin();
        yield return new WaitForSeconds(1f);
        List<SpiderCoin> coinz = GetCoins(coins);
        foreach (var coin in coinz)
        {
            
            coin.ToggleVisibility(false, false);
            coin.grow();
            yield return new WaitForSeconds(0f);

        }

        StartCoroutine(ShowCoins(coins));
        yield return new WaitForSeconds(1f);
        bug.StartToWeb();
        yield return new WaitForSeconds(1f);
        web.webSmall();
       
        StartCoroutine(CoinsUp(coins));
        Debug.Log("Working?");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowCoins(int index)
    {
        Debug.Log("ShowCoins???????????????");
        List<SpiderCoin> currentCoins = GetCoins(index);
        foreach (var coin in currentCoins)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
            {
                unusedCoinPool.AddRange(globalCoinPool);
            }
            ActionWordEnum type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);
            Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }

        //SelectRandomCoin(currRow);
    }

    private IEnumerator CoinsUp(int index)
    {
        Debug.Log("ShowCoins");
        List<SpiderCoin> coins = GetCoins(index);
        foreach (var coin in coins)
        {

            coin.MoveUp();
            yield return new WaitForSeconds(0f);

        }
        SelectRandomCoin(index);
    }
    private IEnumerator CoinsDown(int index)
    {
        Debug.Log("ShowCoins");
        List<SpiderCoin> coins = GetCoins(index);
        foreach (var coin in coins)
        {
            
            coin.MoveDown();
            yield return new WaitForSeconds(0f);

        }
        //SelectRandomCoin(index);
    }


    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<SpiderCoin> row = GetCoins(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<SpiderCoin> pile = GetCoins(index);
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
        selectedSpiderCoin = pile[selectedIndex];
        bug.SetCoinType(selectedSpiderCoin.type);


    }

    private List<SpiderCoin> GetCoins(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return Coins;


        }
    }
}
