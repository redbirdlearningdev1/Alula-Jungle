using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpiderwebTutorialList
{
    public List<ActionWordEnum> list;
}

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

    [Header("Tutorial Stuff")]
    public List<SpiderwebTutorialList> tutorialLists;
    public int[] correctIndexes;
    private int tutorialEvent = 0;

    /* 
    ################################################
    #   MONOBEHAVIOR METHODS
    ################################################
    */

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
            playTutorial = !StudentInfoSystem.currentStudentPlayer.spiderwebTutorial;

        PregameSetup();

        // start tutorial or normal game
        if (playTutorial)
            StartCoroutine(StartTutorialGame());
        else 
            StartCoroutine(StartGame(0));
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
        }
    }

    /* 
    ################################################
    #   NORMAL GAME METHODS
    ################################################
    */

    public bool EvaluateSelectedSpiderCoin(ActionWordEnum coin,SpiderCoin correctCoin)
    {
        Debug.Log(selectedSpiderCoin.type);
        if (coin == selectedSpiderCoin.type)
        {
            winCount++;

            // success! go on to the next row or win game if on last row
            if (winCount < 4)
            {
                if (!playTutorial)
                    StartCoroutine(CoinSuccessRoutine(correctCoin));
                else 
                    StartCoroutine(TutorialSuccessRoutine(correctCoin));
                
            }
            else
            {
                if (!playTutorial)
                    StartCoroutine(WinRoutine(correctCoin));
                else 
                    StartCoroutine(TutorialWinRoutine(correctCoin));
            }

            return true;
        }

        if (!playTutorial)
            StartCoroutine(CoinFailRoutine());
        else
            StartCoroutine(TutorialFailRoutine());
    
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

    private IEnumerator StartGame(int coins)
    {
        StartCoroutine(CoinsDown());
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

    /* 
    ################################################
    #   TUTORIAL GAME METHODS
    ################################################
    */

    private IEnumerator StartTutorialGame()
    {
        // play tutorial audio 1
        AudioClip clip = AudioDatabase.instance.SpiderwebTutorial_1;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 2
        clip = AudioDatabase.instance.SpiderwebTutorial_2;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 3
        clip = AudioDatabase.instance.SpiderwebTutorial_3;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 4
        clip = AudioDatabase.instance.SpiderwebTutorial_4;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        StartCoroutine(PlayTutorialGame(0));
    }

    private IEnumerator PlayTutorialGame(int coins)
    {
        StartCoroutine(CoinsDown());
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

    private IEnumerator TutorialFailRoutine()
    {
        // play wrong choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);
        yield return null;
    }

    private IEnumerator TutorialSuccessRoutine(SpiderCoin coin)
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
        
        StartCoroutine(PlayTutorialGame(tutorialEvent));
    }

    private IEnumerator TutorialWinRoutine(SpiderCoin coin)
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

        yield return new WaitForSeconds(3f);

        // save to SIS
        StudentInfoSystem.currentStudentPlayer.spiderwebTutorial = true;
        StudentInfoSystem.SaveStudentPlayerData();

        GameManager.instance.LoadScene("NewSpiderGame", true, 3f);
    }

    /* 
    ################################################
    #   UTIL METHODS
    ################################################
    */

    private int CalculateStars()
    {
        if (timesMissed <= 0)
            return 3;
        else if (timesMissed > 0 && timesMissed <= 2)
            return 2;
        else
            return 1;
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

    private IEnumerator ShowCoins(int index)
    {
        List<SpiderCoin> currentCoins = GetCoins();

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        int i = 0;
        foreach (var coin in currentCoins)
        {
            ActionWordEnum type;
            if (!playTutorial)
                type = GetUnusedWord();
            else
            {   
                type = tutorialLists[index].list[i];
                i++;
            }
                
            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);
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
        List<SpiderCoin> coins = GetCoins();
        foreach (var coin in coins)
        {
            coin.MoveUp();
            yield return new WaitForSeconds(0f);
        }

        // select random coin OR tutorial coin
        if (!playTutorial)
            SelectRandomCoin(index);    
        else
        {
            List<SpiderCoin> pile = GetCoins();
            selectedIndex = correctIndexes[tutorialEvent];
            selectedSpiderCoin = pile[selectedIndex];
            bug.SetCoinType(selectedSpiderCoin.type);
            tutorialEvent++;
        }
    }

    private IEnumerator CoinsDown()
    {
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
        selectedSpiderCoin = pile[selectedIndex];
        bug.SetCoinType(selectedSpiderCoin.type);
    }

    private List<SpiderCoin> GetCoins()
    {
        return Coins;
    }
}
