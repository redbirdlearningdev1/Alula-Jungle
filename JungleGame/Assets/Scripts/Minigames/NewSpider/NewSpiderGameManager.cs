using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpiderGameManager : MonoBehaviour
{
    public static NewSpiderGameManager instance;

    public bool playingInEditor;
    public bool playTutorial;

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
    private List<ActionWordEnum> usedCoinPool;


    [SerializeField] private List<SpiderCoin> Coins;


    private List<SpiderCoin> allCoins = new List<SpiderCoin>();
    private int selectedIndex;
    public SpiderCoin selectedSpiderCoin;

    private int winCount = 0;
    private int timesMissed = 0;

    private SpiderwebGameData gameData;


    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // play music
        AudioManager.instance.StopMusic();

        if (!instance)
        {
            instance = this;
        }

        // get game data
        gameData = (SpiderwebGameData)GameManager.instance.GetData();

        if (!playingInEditor)
            playTutorial = !StudentInfoSystem.currentStudentPlayer.froggerTutorial;

        PregameSetup();
        StartCoroutine(StartGame(0));
    }

    public bool EvaluateSelectedSpiderCoin(ActionWordEnum coin,SpiderCoin correctCoin)
    {
        Debug.Log(selectedSpiderCoin.type);
        if (coin == selectedSpiderCoin.type)
        {
            winCount++;

            // success! go on to the next row or win game if on last row
            Debug.Log("YOU DID IT");
            if (winCount < 4)
            {
                StartCoroutine(CoinSuccessRoutine(correctCoin));
            }
            else
            {
                Debug.Log("YOU DID IT AGAIn");
                StartCoroutine(WinRoutine(correctCoin));
            }

            return true;
        }
        StartCoroutine(CoinFailRoutine());
        return false;
    }

    private IEnumerator CoinFailRoutine()
    {
        // play wrong choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);

        timesMissed++;
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
        // play right choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);

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

    private IEnumerator WinRoutine(SpiderCoin coin)
    {
        // play right choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);

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

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(2f);

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
        if (gameData != null)
        {
            globalCoinPool = gameData.wordPool;
        }
        else
        {
            globalCoinPool = GameManager.instance.GetGlobalActionWordList();
        }

        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins
        foreach (var coin in allCoins)
        { 
            coin.setOrigin();
            Debug.Log(coin.transform.position);
        }
    }

    private IEnumerator StartGame(int coins)
    {
        StartCoroutine(CoinsDown(coins));
        bug.goToOrigin();
        yield return new WaitForSeconds(1f);
        List<SpiderCoin> coinz = GetCoins();
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
        yield return new WaitForSeconds(1f);
        bug.PlayPhonemeAudio();
    }

    private IEnumerator ShowCoins(int index)
    {
        List<SpiderCoin> currentCoins = GetCoins();

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        foreach (var coin in currentCoins)
        {
            ActionWordEnum type = GetUnusedWord();

            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);
            Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }
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

        // make sure word is not being used
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }

    private IEnumerator CoinsUp(int index)
    {
        Debug.Log("ShowCoins");
        List<SpiderCoin> coins = GetCoins();
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
        List<SpiderCoin> coins = GetCoins();
        foreach (var coin in coins)
        {
            
            coin.MoveDown();
            yield return new WaitForSeconds(0f);
        }
    }


    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<SpiderCoin> row = GetCoins();
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<SpiderCoin> pile = GetCoins();
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
        selectedSpiderCoin = pile[selectedIndex];
        bug.SetCoinType(selectedSpiderCoin.type);
    }

    private List<SpiderCoin> GetCoins()
    {
        return Coins;
    }
}
