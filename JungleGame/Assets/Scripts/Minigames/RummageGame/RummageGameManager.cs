using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RummageTutorialList
{
    public List<ActionWordEnum> list;
}

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
    private bool[] pileLockArray = new bool[5];
    private bool[] pileCompleteArray = new bool[5];
    private bool atPile = false;
    private int winCount = 0;
    private int lastLocation;

    private RummageGameData gameData;

    [Header("Tutorial Stuff")]
    public List<RummageTutorialList> tutorialPiles;
    public int[] correctIndexes;
    private int tutorialEvent = 0;


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

        if (playTutorial)
        {
            print ("playing tutorial");
            StartCoroutine(StartTutorial());
        }   
        else
        {
            StartCoroutine(SetPileGlow(true));
            StartCoroutine(SetPileWiggles(true));
        }
    }

    private IEnumerator StartTutorial()
    {
        LockAllPiles();
        yield return new WaitForSeconds(1f);

        // play tutorial audio 1
        AudioClip clip = AudioDatabase.instance.RummageTutorial_1;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 2
        clip = AudioDatabase.instance.RummageTutorial_2;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 3
        clip = AudioDatabase.instance.RummageTutorial_3;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        NextTutorialEvent();
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            if (playingDancingManAnimation)
                return;
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

        // CLICKING ON PILES
        if (pile[0].chosen == true && pileLockArray[0] == false && atPile == false && pileCompleteArray[0] == false)
        {

            pile[1].colliderOff();
            pile[2].colliderOff();
            pile[3].colliderOff();
            pile[4].colliderOff();
            pile[0].pileComplete();
            LockAllPiles();

            selectedPile = pile[0];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile1();
        }
        if (pile[1].chosen == true && pileLockArray[1] == false && atPile == false && pileCompleteArray[1] == false)
        {
            pile[0].colliderOff();
            pile[2].colliderOff();
            pile[3].colliderOff();
            pile[4].colliderOff();
            pile[1].pileComplete();
            LockAllPiles();

            selectedPile = pile[1];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile2();
        }
        if (pile[2].chosen == true && pileLockArray[2] == false && atPile == false && pileCompleteArray[2] == false)
        {
            pile[0].colliderOff();
            pile[1].colliderOff();
            pile[3].colliderOff();
            pile[4].colliderOff();
            pile[2].pileComplete();
            LockAllPiles();

            selectedPile = pile[2];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile3();
        }
        if (pile[3].chosen == true && pileLockArray[3] == false && atPile == false && pileCompleteArray[3] == false)
        {
            pile[0].colliderOff();
            pile[1].colliderOff();
            pile[2].colliderOff();
            pile[4].colliderOff();
            pile[3].pileComplete();
            LockAllPiles();

            selectedPile = pile[3];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile4();
        }
        if (pile[4].chosen == true && pileLockArray[4] == false && atPile == false && pileCompleteArray[4] == false)
        {
            pile[0].colliderOff();
            pile[1].colliderOff();
            pile[2].colliderOff();
            pile[3].colliderOff();
            pile[4].pileComplete();
            LockAllPiles();

            selectedPile = pile[4];
            StartCoroutine(SetPileWiggles(false));
            StartCoroutine(SetPileGlow(false, true));

            orc.GoToPile5();
        }


        // COIN SELECT
        if (orc.AtLocation() == 1 && pileLockArray[0] == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLockArray[0] = false;
            atPile = true;

            if (playTutorial)
                StartCoroutine(TutorialGame(0));
            else
                StartCoroutine(StartGame(0));
        }
        if (orc.AtLocation() == 2 && pileLockArray[1] == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLockArray[1] = false;
            atPile = true;

            if (playTutorial)
                StartCoroutine(TutorialGame(1));
            else
                StartCoroutine(StartGame(1));
        }
        if (orc.AtLocation() == 3 && pileLockArray[2] == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLockArray[2] = false;
            atPile = true;

            if (playTutorial)
                StartCoroutine(TutorialGame(2));
            else
                StartCoroutine(StartGame(2));
        }
        if (orc.AtLocation() == 4 && pileLockArray[3] == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLockArray[3] = false;
            atPile = true;

            if (playTutorial)
                StartCoroutine(TutorialGame(3));
            else
                StartCoroutine(StartGame(3));
        }
        if (orc.AtLocation() == 5 && pileLockArray[4] == true && atPile == false)
        {
            lastLocation = orc.AtLocation();
            pileLockArray[4] = false;
            atPile = true;

            if (playTutorial)
                StartCoroutine(TutorialGame(4));
            else
                StartCoroutine(StartGame(4));
        }
    }

    private IEnumerator DancingManRoutine()
    {
        if (playingDancingManAnimation || selectedRummageCoin == null)
            yield break;
        
        playingDancingManAnimation = true;
        dancingMan.PlayUsingPhonemeEnum(selectedRummageCoin.type);
        yield return new WaitForSeconds(2.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedRummageCoin(ActionWordEnum coin)
    {
        // play audio iff winCount == 0
        if (winCount == 0)
        {
            AudioClip clip = AudioDatabase.instance.FroggerTutorial_3;
            AudioManager.instance.PlayTalk(clip);
        }

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

    private IEnumerator SkipToWinRoutine()
    {        
        stretch.stretchIn();
        orc.GoToOrigin();
        
        yield return new WaitForSeconds(1f);
        orc.successOrc();
        yield return new WaitForSeconds(1f);
        orc.stopOrc();
        atPile = false;
        pile[0].colliderOn();
        pile[1].colliderOn();
        pile[2].colliderOn();
        pile[3].colliderOn();
        pile[4].colliderOn();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private IEnumerator CoinFailRoutine()
    {
        if (playTutorial)
        {
            orc.failOrc();
            yield return new WaitForSeconds(1f);
            orc.channelOrc();
            yield break;
        }

        timesMissed++;
        orc.failOrc();
        yield return new WaitForSeconds(.75f);

        StartCoroutine(pileBounceInCoins(lastLocation-1));
        yield return new WaitForSeconds(1.5f);
        stretch.stretchIn();
        yield return new WaitForSeconds(1f);
        orc.GoToOrigin();
        yield return new WaitForSeconds(2.0f);
        orc.stopOrc();
        atPile = false;
        UnlockAllPiles();
        pile[0].colliderOn();
        pile[1].colliderOn();
        pile[2].colliderOn();
        pile[3].colliderOn();
        pile[4].colliderOn();
        StartCoroutine(SetPileGlow(true));
    }

    private IEnumerator CoinSuccessRoutine()
    {
        winCount++;

        yield return new WaitForSeconds(.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation() - 1);

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
        Repairs[lastLocation - 1].SetActive(false);
        yield return new WaitForSeconds(1.0f);
        orc.successOrc();
        yield return new WaitForSeconds(1f);
        orc.stopOrc();
        atPile = false;
        pile[0].colliderOn();
        pile[1].colliderOn();
        pile[2].colliderOn();
        pile[3].colliderOn();
        pile[4].colliderOn();
        // complete and lock pile
        if (lastLocation == 1)
        {
            pileCompleteArray[0] = true;
            pile[0].pileLock();
        }
        else if (lastLocation == 2)
        {
            pileCompleteArray[1] = true;
            pile[1].pileLock();
        }
        else if (lastLocation == 3)
        {
            pileCompleteArray[2] = true;
            pile[2].pileLock();
        }
        else if (lastLocation == 4)
        {
            pileCompleteArray[3] = true;
            pile[3].pileLock();
        }
        else
        {
            pileCompleteArray[4] = true;
            pile[4].pileLock();
        }
        // unlock all piles
        UnlockAllPiles();

        if (playTutorial)
        {
            NextTutorialEvent();
            yield break;
        }

        yield return new WaitForSeconds(.25f);
        StartCoroutine(SetPileGlow(true));

        StartCoroutine(SetPileWiggles(true));
        yield return new WaitForSeconds(.25f);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(0.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation()-1);

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

        if (playTutorial)
        {
            yield return new WaitForSeconds(3f);

            // save to SIS
            StudentInfoSystem.currentStudentPlayer.rummageTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("RummageGame", true, 3f);
        }

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

    private IEnumerator StartGame(int piles)
    {
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

        SelectRandomCoin(piles);
    }

    private void NextTutorialEvent()
    {
        int index = tutorialEvent;
        this.pile[index].pileGlowOn();
        this.pile[index].SetWiggleOn();

        LockAllPilesExcept(index);
        tutorialEvent++;
    }

    private void LockAllPilesExcept(int index)
    {
        print ("locking all piles except: " + index);
        for (int i = 0; i < 5; i++)
        {
            if (i != index)
                pileLockArray[i] = true;
            else
                pileLockArray[i] = false;
        }
    }

    private void LockAllPiles()
    {
        for (int i = 0; i < 5; i++)
            pileLockArray[i] = true;   
    }

    private void UnlockAllPiles()
    {
        for (int i = 0; i < 5; i++)
            pileLockArray[i] = false;   
    }

    private IEnumerator TutorialGame(int index)
    {
        print ("index: " + index);
        StartCoroutine(SetPileGlow(false));
        orc.channelOrc();

        List<RummageCoin> pileSet = GetCoinPile(index);
        int i = 0;
        foreach (var coin in pileSet)
        {
            coin.gameObject.SetActive(true);
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(.5f);

        StartCoroutine(ShowCoins(index, true));
        StartCoroutine(pileBounceCoins(index));
        stretch.stretchOut();
        yield return new WaitForSeconds(1f);

        foreach (var coin in pileSet)
        {
            coin.grow();
            coin.BounceToCloth();
        }

        SelectTutorialCoin(pileSet, index);
    }

    private void SelectTutorialCoin(List<RummageCoin> pile, int index)
    {
        // select current coin
        selectedIndex = correctIndexes[index];
        selectedRummageCoin = pile[selectedIndex];
        print ("selectedRummageCoin: " + selectedRummageCoin.type);

        // dancing man animation
        StartCoroutine(DancingManRoutine());
        StartCoroutine(RepeatWhileWating());
        waitingForCoinSelection = true;
    }

    private IEnumerator ShowCoins(int index, bool tutorial = false)
    {
        List<RummageCoin> pile = GetCoinPile(index);

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        int i = 0;
        foreach (var coin in pile)
        {
            if (!tutorial)
            {
                // set random type
                if (unusedCoinPool.Count == 0)
                {
                    unusedCoinPool.Clear();
                    unusedCoinPool.AddRange(globalCoinPool);
                }
                ActionWordEnum type = GetUnusedWord();

                coin.SetCoinType(type);
            }
            else
            {
                //print ("coin type: " + tutorialPiles[index].list[i]);
                coin.SetCoinType(tutorialPiles[index].list[i]);
            }

            coin.ToggleVisibility(true, true);
            i++;
            yield return new WaitForSeconds(0.1f);
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
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            
            coin.BounceOut1();
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator pileBounceInCoins(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            coin.shrink();
            
            coin.BounceIn1();
            
            yield return new WaitForSeconds(0.5f);
            coin.gameObject.SetActive(false);
        }
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
        //StartCoroutine(RepeatWhileWating());
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
        if (opt)
        {
            //print ("glow on");
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
            //print ("glow off");
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
