using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummageGameManager : MonoBehaviour
{
    public static RummageGameManager instance;

    public bool playingInEditor;
    public bool playTutorial;
    public float timeBetweenRepeat;

    private int timesMissed = 0;

    [SerializeField] private OrcController orc;
    [SerializeField] private chest chest;
    [SerializeField] private RummageChest stretch;
    [SerializeField] private DancingManController dancingMan;
    [SerializeField] private List<pileRummage> pile;
    [SerializeField] private RummageCoinRaycaster caster;
    [SerializeField] private List<GameObject> Repairs;

    private bool dancingManClickable = false;
    private bool waitingForCoinSelection = false;
    private bool playingDancingManAnimation = false;
    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;
    private List<ActionWordEnum> prevMainCoins;

    [Header("Piles")]
    [SerializeField] private List<RummageCoin> pile1;
    [SerializeField] private List<RummageCoin> pile2;
    [SerializeField] private List<RummageCoin> pile3;
    [SerializeField] private List<RummageCoin> pile4;
    [SerializeField] private List<RummageCoin> pile5;

    private List<RummageCoin> allCoins = new List<RummageCoin>();
    private int selectedIndex;
    public RummageCoin selectedRummageCoin;
    private pileRummage selectedPile;
    private bool pileLock1 = false;
    private bool pileLock2 = false;
    private bool pileLock3 = false;
    private bool pileLock4 = false;
    private bool pileLock5 = false;
    private bool pileComplete1 = false;
    private bool pileComplete2 = false;
    private bool pileComplete3 = false;
    private bool pileComplete4 = false;
    private bool pileComplete5 = false;
    private bool atPile = false;
    private int winCount = 0;
    private int lastLocation;

    private RummageGameData gameData;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        // get game data
        gameData = (RummageGameData)GameManager.instance.GetData();
        //playTutorial = !StudentInfoSystem.currentStudentPlayer.rummageTutorial;

        if (!playingInEditor)
            playTutorial = !StudentInfoSystem.currentStudentPlayer.rummageTutorial;


        PregameSetup();
    }

    private void PregameSetup()
    {
        // TODO: add music and ambiance
        AudioManager.instance.StopMusic();

        // create coin list
        foreach (var coin in pile1)
            allCoins.Add(coin);
        foreach (var coin in pile2)
            allCoins.Add(coin);
        foreach (var coin in pile3)
            allCoins.Add(coin);
        foreach (var coin in pile4)
            allCoins.Add(coin);
        foreach (var coin in pile5)
            allCoins.Add(coin);

        // get Global Coin List
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
            coin.ToggleVisibility(false, false);
            coin.setOrigin();
        }
           
        foreach(var coin in allCoins)
        {
            coin.gameObject.SetActive(false);
        }

        prevMainCoins = new List<ActionWordEnum>();

        StartCoroutine(SetPileGlow(true));
        StartCoroutine(SetPileWiggles(true));
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            if (!dancingManClickable)
                return;
            StartCoroutine(DancingManRoutine());
        }
        if (playTutorial && pileComplete2 == false)
        {
            walkThrough();
        }
        if (pile[0].chosen == true && pileLock1 == false && atPile == false && pileComplete1 == false)
        {
            pile[0].pileComplete();
            pileLock1 = true;
            pileLock2 = false;
            pileLock3 = false;
            pileLock4 = false;
            pileLock5 = false;

            selectedPile = pile[0];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile1();
        }
        if (pile[1].chosen == true && pileLock2 == false && atPile == false && pileComplete2 == false)
        {
            pile[1].pileComplete();
            pileLock1 = false;
            pileLock2 = true;
            pileLock3 = false;
            pileLock4 = false;
            pileLock5 = false;

            selectedPile = pile[1];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile2();
        }
        if (pile[2].chosen == true && pileLock3 == false && atPile == false && pileComplete3 == false)
        {

            pile[2].pileComplete();
            pileLock1 = false;
            pileLock2 = false;
            pileLock3 = true;
            pileLock4 = false;
            pileLock5 = false;

            selectedPile = pile[2];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile3();
        }
        if (pile[3].chosen == true && pileLock4 == false && atPile == false && pileComplete4 == false)
        {
            pile[3].pileComplete();
            pileLock1 = false;
            pileLock2 = false;
            pileLock3 = false;
            pileLock4 = true;
            pileLock5 = false;

            selectedPile = pile[3];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile4();
        }
        if (pile[4].chosen == true && pileLock5 == false && atPile == false && pileComplete5 == false)
        {
            pile[4].pileComplete();
            pileLock1 = false;
            pileLock2 = false;
            pileLock3 = false;
            pileLock4 = false;
            pileLock5 = true;

            selectedPile = pile[4];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile5();
        }
        if (orc.AtLocation() == 1 && pileLock1 == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLock1 = false;
            atPile = true;

            if (playTutorial)
            {
                StartCoroutine(StartGame(5));
            }
            else
            {
                StartCoroutine(StartGame(0));
            }
        }
        if (orc.AtLocation() == 2 && pileLock2 == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLock2 = false;
            atPile = true;
            StartCoroutine(StartGame(1));
        }
        if (orc.AtLocation() == 3 && pileLock3 == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLock3 = false;
            atPile = true;
            StartCoroutine(StartGame(2));
        }
        if (orc.AtLocation() == 4 && pileLock4 == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLock4 = false;
            atPile = true;
            StartCoroutine(StartGame(3));
        }
        if (orc.AtLocation() == 5 && pileLock5 == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLock5 = false;
            atPile = true;
            StartCoroutine(StartGame(4));
        }
    }

    private IEnumerator DancingManRoutine()
    {
        if (playingDancingManAnimation)
            yield break;
        
        dancingManClickable = true;
        playingDancingManAnimation = true;
        dancingMan.PlayUsingPhonemeEnum(selectedRummageCoin.type);
        yield return new WaitForSeconds(1.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedRummageCoin(ActionWordEnum coin)
    {
        waitingForCoinSelection = false;
        dancingManClickable = false;
        if (coin == selectedRummageCoin.type)
        {
            // success! go on to the next row or win game if on last row
            if (winCount < 4)
            {
                StartCoroutine(CoinSuccessRoutine());
            }               
            else
            {
                StartCoroutine(WinRoutine());
            }
                
            return true;
        }
        StartCoroutine(CoinFailRoutine());
        return false;
    }


    private IEnumerator CoinFailRoutine()
    {
        timesMissed++;

        orc.failOrc();
        yield return new WaitForSeconds(.9f);

        StartCoroutine(pileBounceInCoins(lastLocation-1));
        yield return new WaitForSeconds(.5f);
        stretch.stretchIn();
        yield return new WaitForSeconds(1f);
        orc.GoToOrigin();
        yield return new WaitForSeconds(2.0f);
        orc.stopOrc();
        atPile = false;
        //chest.chestGlowNo();
        StartCoroutine(SetPileGlow(true));
    }

    private IEnumerator CoinSuccessRoutine()
    {
        winCount++;

        yield return new WaitForSeconds(.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation()-1);
        if (playTutorial)
        {
            pileComplete2 = false;
            pileComplete3 = false;
            pileComplete4 = false;
            pileComplete5 = false;
            playTutorial = false;
            pileSet = GetCoinPile(0);
        }

        Repairs[orc.AtLocation() - 1].SetActive(true);
        foreach (var coin in pileSet)
        {
            coin.gameObject.SetActive(false);
        }

        chest.UpgradeBag();
        pile[orc.AtLocation()-1].pileDone();
        stretch.stretchIn();

        orc.GoToOrigin();
        
        yield return new WaitForSeconds(1.0f);
        Repairs[lastLocation-1].SetActive(false);
        yield return new WaitForSeconds(1.0f);
        orc.successOrc();
        yield return new WaitForSeconds(1f);
        orc.stopOrc();
        atPile = false;
        if(lastLocation == 1)
        {
            Debug.Log("Pile 1 locked");
            pileComplete1 = true;
            pile[0].pileLock();
           
        }
        else if (lastLocation == 2)
        {
            pileComplete2 = true;
            pile[1].pileLock();
        }
        else if (lastLocation == 3)
        {
            pileComplete3 = true;
            pile[2].pileLock();
        }
        else if (lastLocation == 4)
        {
            pileComplete4 = true;
            pile[3].pileLock();
        }
        else
        {
            pileComplete5 = true;
            pile[4].pileLock();
        }
        yield return new WaitForSeconds(.25f);
        StartCoroutine(SetPileGlow(true));

        StartCoroutine(SetPileWiggles(true));
        yield return new WaitForSeconds(.25f);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation()-1);
        if (playTutorial)
        {
            pileComplete2 = false;
            pileComplete3 = false;
            pileComplete4 = false;
            pileComplete5 = false;
            playTutorial = false;
            pileSet = GetCoinPile(0);
        }

        Repairs[orc.AtLocation() - 1].SetActive(true);
        foreach (var coin in pileSet)
        {
            coin.gameObject.SetActive(false);
        }

        chest.UpgradeBag();
        pile[orc.AtLocation()-1].pileDone();
        stretch.stretchIn();

        orc.GoToOrigin();
        
        yield return new WaitForSeconds(1.0f);
        Repairs[lastLocation-1].SetActive(false);
        yield return new WaitForSeconds(1.0f);
        orc.successOrc();
        yield return new WaitForSeconds(1f);
        orc.stopOrc();
        atPile = false;

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(1f);

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

    private void walkThrough()
    {

        //StartCoroutine(SetPileGlow(false));
        pile[0].pileGlowOn();

        pile[0].SetWiggleOn();

        pileComplete2 = true;
        pileComplete3 = true;
        pileComplete4 = true;
        pileComplete5 = true;
    }



    private IEnumerator StartGame(int piles)
    {
        // wait a moment for the setup to finish
        Debug.Log("STARTING GAME");
        StartCoroutine(SetPileGlow(false));
        orc.channelOrc();

        List<RummageCoin> pileSet = GetCoinPile(piles);
        foreach (var coin in pileSet)
        {
            coin.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(ShowCoins(piles));
        StartCoroutine(pileBounceCoins(piles));
        stretch.stretchOut();
        yield return new WaitForSeconds(1f);
        foreach (var coin in pileSet)
        {
            coin.grow();
            coin.BounceToCloth();
        }
        //chest.chestGlow();
        SelectRandomCoin(piles);
    }

    private IEnumerator ShowCoins(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        foreach (var coin in pile)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
            {
                unusedCoinPool.Clear();
                unusedCoinPool.AddRange(globalCoinPool);
            }
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

    private IEnumerator pileBounceCoins(int index)
    {
        Debug.Log("ShowCoins");
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            
            coin.BounceOut1();
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator pileBounceInCoins(int index)
    {
        Debug.Log("ShowCoins");
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            coin.shrink();
            
            coin.BounceIn1();
            
            yield return new WaitForSeconds(0.5f);
            coin.gameObject.SetActive(false);
        }

        //SelectRandomCoin(currRow);
    }

    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<RummageCoin> row = GetCoinPile(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);
        selectedIndex = Random.Range(0, pile.Count);
        selectedRummageCoin = pile[selectedIndex];

        // make sure new coin is selected to be main coin each time
        if (prevMainCoins.Contains(selectedRummageCoin.type))
        {
            for (int i = 1; i < pile.Count; i++)
            {
                selectedIndex += i;
                if (selectedIndex > pile.Count - 1)
                {
                    selectedIndex -= pile.Count - 1;
                }

                selectedRummageCoin = pile[selectedIndex];
                if (!prevMainCoins.Contains(selectedRummageCoin.type))
                    break;
                
                if (i == 3)
                {
                    prevMainCoins.Clear();
                    selectedIndex = Random.Range(0, pile.Count);
                    selectedRummageCoin = pile[selectedIndex];
                }
            }
        }

        prevMainCoins.Add(selectedRummageCoin.type);

        StartCoroutine(DancingManRoutine());
        StartCoroutine(RepeatWhileWating());
        waitingForCoinSelection = true;
    }

    private IEnumerator RepeatWhileWating()
    {
        float timer = 0f;

        while (true)
        {
            while (playingDancingManAnimation)
                yield return null;

            if (!waitingForCoinSelection)
                yield break;  

            timer += Time.deltaTime;
            if (timer > timeBetweenRepeat)
            {
                timer = 0f;
                StartCoroutine(DancingManRoutine());
            }

            yield return null;
        }
    }

    private List<RummageCoin> GetCoinPile(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return pile1;
            case 1:
                return pile2;
            case 2:
                return pile3;
            case 3:
                return pile4;
            case 4:
                return pile5;
        }
    }

    private IEnumerator SetPileWiggles(bool opt)
    {
        if (opt)
        {
            foreach(var p in pile)
            {
                p.SetWiggleOn();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            foreach(var p in pile)
            {
                p.SetWiggleOff();
            }
        }
    }

    private IEnumerator SetPileGlow(bool opt, bool excludeSelectedPile = false)
    {
        yield return new WaitForSeconds(0.1f);
        if (opt)
        {
            print ("glow on");
            foreach(var p in pile)
            {
                if (excludeSelectedPile)
                {
                    if (p == selectedPile)
                        continue;
                }

                p.pileGlowOn();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            print ("glow off");
            foreach(var p in pile)
            {
                if (excludeSelectedPile)
                {
                    if (p == selectedPile)
                        continue;
                }

                p.pileGlowOff();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
