using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

[System.Serializable]
public struct RummageTutorialList
{
    public List<ActionWordEnum> list;
}

public class RummageGameManager : MonoBehaviour
{
    public static RummageGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;

    private int timesMissed = 0;
    private int currentPile = 0;

    [SerializeField] private OrcController orc;
    [SerializeField] private chest chest;
    [SerializeField] private RummageChest stretch;
    [SerializeField] private DancingManController dancingMan;
    public Transform dancingManOffScreen;
    public Transform dancingManOnScreen;

    [SerializeField] private List<pileRummage> piles;
    [SerializeField] private RummageCoinRaycaster caster;
    [SerializeField] private List<GameObject> Repairs;

    private bool playingDancingManAnimation = false;

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

    [Header("Tutorial Stuff")]
    public bool playTutorial;
    public List<RummageTutorialList> tutorialPiles;
    public int[] correctIndexes;
    private int tutorialEvent = 0;

    private float startTime;


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

        // get game data
        mapID = GameManager.instance.mapID;
    }

    void Start()
    {
        // set start time
        startTime = Time.time;

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().rummageTutorial;

        PregameSetup();

        if (playTutorial)
        {
            StartCoroutine(StartTutorial());
        }
        else
        {
            // start song
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.RummageSongSplit);

            StartCoroutine(StartGame());
        }
    }

    private void PregameSetup()
    {
        // start ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 1f, "forest_ambiance");
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.1f, "river_flowing");

        // turn off raycaster
        RummageCoinRaycaster.instance.isOn = false;

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

        globalCoinPool = new List<ActionWordEnum>();

        // get Global Coin List
        if (GameManager.instance.practiceModeON)
        {
            globalCoinPool.AddRange(GameManager.instance.practicePhonemes);
        }
        else if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // place dancing man off-screen
        dancingMan.gameObject.transform.position = dancingManOffScreen.position;

        // disable all coins
        foreach (var coin in allCoins)
        {
            coin.ToggleVisibility(false);
            coin.setOrigin();
        }

        foreach (var coin in allCoins)
        {
            coin.gameObject.SetActive(false);
        }

        prevMainCoins = new List<ActionWordEnum>();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // reveal dancing man
        StartCoroutine(ShowDancingManRoutine());
        yield return new WaitForSeconds(1f);

        StartCoroutine(SetPilesActive(true));

        // make coins interactable
        SetCoinsInteractable(true);
        // turn on raycaster
        RummageCoinRaycaster.instance.isOn = true;
    }

    private IEnumerator StartTutorial()
    {
        LockAllPiles();
        yield return new WaitForSeconds(1f);

        // play tutorial audio
        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.rummageIntro1);
        clips.Add(GameIntroDatabase.instance.rummageIntro2);
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
        yield return cd.coroutine;
        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
        yield return cd0.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Clogg, clips);
        yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + 1f);

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // reveal dancing man
        StartCoroutine(ShowDancingManRoutine());
        yield return new WaitForSeconds(1f);

        // turn on raycaster
        RummageCoinRaycaster.instance.isOn = true;

        NextTutorialEvent();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = true;
        // times missed set to 0
        timesMissed = 0;
        // update AI data
        AIData(StudentInfoSystem.GetCurrentProfile());
        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            if (playingDancingManAnimation)
                return;
            StartCoroutine(DancingManRoutine());
        }

        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SkipGame();
                }
            }
        }

        // CLICKING ON PILES
        if (piles[0].chosen == true && pileLockArray[0] == false && atPile == false && pileCompleteArray[0] == false)
        {
            piles[1].colliderOff();
            piles[2].colliderOff();
            piles[3].colliderOff();
            piles[4].colliderOff();
            piles[0].pileComplete();
            LockAllPiles();

            selectedPile = piles[0];
            StartCoroutine(SetPilesActive(false));

            orc.GoToPile1();
        }
        if (piles[1].chosen == true && pileLockArray[1] == false && atPile == false && pileCompleteArray[1] == false)
        {
            piles[0].colliderOff();
            piles[2].colliderOff();
            piles[3].colliderOff();
            piles[4].colliderOff();
            piles[1].pileComplete();
            LockAllPiles();

            selectedPile = piles[1];
            StartCoroutine(SetPilesActive(false));

            orc.GoToPile2();
        }
        if (piles[2].chosen == true && pileLockArray[2] == false && atPile == false && pileCompleteArray[2] == false)
        {
            piles[0].colliderOff();
            piles[1].colliderOff();
            piles[3].colliderOff();
            piles[4].colliderOff();
            piles[2].pileComplete();
            LockAllPiles();

            selectedPile = piles[2];
            StartCoroutine(SetPilesActive(false));

            orc.GoToPile3();
        }
        if (piles[3].chosen == true && pileLockArray[3] == false && atPile == false && pileCompleteArray[3] == false)
        {
            piles[0].colliderOff();
            piles[1].colliderOff();
            piles[2].colliderOff();
            piles[4].colliderOff();
            piles[3].pileComplete();
            LockAllPiles();

            selectedPile = piles[3];
            StartCoroutine(SetPilesActive(false));

            orc.GoToPile4();
        }
        if (piles[4].chosen == true && pileLockArray[4] == false && atPile == false && pileCompleteArray[4] == false)
        {
            piles[0].colliderOff();
            piles[1].colliderOff();
            piles[2].colliderOff();
            piles[3].colliderOff();
            piles[4].pileComplete();
            LockAllPiles();

            selectedPile = piles[4];
            StartCoroutine(SetPilesActive(false));

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
        // return if coin is null
        if (selectedRummageCoin == null)
            yield break;
        // return if already animating
        if (playingDancingManAnimation)
            yield break;

        playingDancingManAnimation = true;
        dancingMan.PlayUsingPhonemeEnum(selectedRummageCoin.type);
        yield return new WaitForSeconds(2.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedRummageCoin(RummageCoin coin)
    {
        // turn off raycaster
        RummageCoinRaycaster.instance.isOn = false;

        // make pile normal sized
        piles[currentPile].GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        piles[currentPile].SetWiggleOff();

        // make coins not interactable
        SetCoinsInteractable(false);

        // stop rummage sound
        AudioManager.instance.StopFX("wood_rummage");

        bool success = (coin.type == selectedRummageCoin.type);
        // only track phoneme attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            StudentInfoSystem.SavePlayerMinigameRoundAttempt(GameType.RummageGame, success);
            StudentInfoSystem.SavePlayerPhonemeAttempt(coin.type, success);
        }

        if (success)
        {
            selectedRummageCoin = null;

            // finish tutorial after 3 piles
            if (playTutorial && tutorialEvent == 3)
            {
                StartCoroutine(WinRoutine(coin.gameObject));
                return true;
            }

            // success! go on to the next row or win game if on last row
            if (winCount < 4)
            {
                StartCoroutine(CoinSuccessRoutine(coin.gameObject));
            }
            else
            {
                StartCoroutine(WinRoutine(coin.gameObject));
            }

            return true;
        }

        if (!playTutorial)
            selectedRummageCoin = null;

        StartCoroutine(CoinFailRoutine());
        return false;
    }

    private IEnumerator CoinFailRoutine()
    {
        if (playTutorial)
        {
            orc.failOrc();
            yield return new WaitForSeconds(1f);
            orc.channelOrc();

            // turn on raycaster
            RummageCoinRaycaster.instance.isOn = true;
            // make coins interactable
            SetCoinsInteractable(true);

            yield break;
        }

        // play incorrect sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

        timesMissed++;
        orc.failOrc();
        yield return new WaitForSeconds(1f);
        StartCoroutine(pileBounceInCoins(lastLocation - 1));
        yield return new WaitForSeconds(1f);
        stretch.stretchIn();
        orc.GoToOrigin();
        yield return new WaitForSeconds(2.0f);
        orc.stopOrc();

        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.rummageReminder1);
        clips.Add(GameIntroDatabase.instance.rummageReminder2);
        clips.Add(GameIntroDatabase.instance.rummageReminder3);

        AssetReference clip = clips[Random.Range(0, clips.Count)];
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Clogg, clip);
        // yield return new WaitForSeconds(cd.GetResult() + 1f);
        
    
        piles[0].colliderOn();
        piles[1].colliderOn();
        piles[2].colliderOn();
        piles[3].colliderOn();
        piles[4].colliderOn();

        // make coins interactable
        SetCoinsInteractable(true);

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(SetPilesActive(true));
        yield return new WaitForSeconds(0.25f);

        // unlock all piles
        UnlockAllPiles();
        atPile = false;
        // turn on raycaster
        RummageCoinRaycaster.instance.isOn = true;
    }

    private IEnumerator CoinSuccessRoutine(GameObject currCoin)
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();
        winCount++;

        yield return new WaitForSeconds(.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation() - 1);

        yield return new WaitForSeconds(0.5f);

        // repair pile
        Repairs[orc.AtLocation() - 1].GetComponent<Animator>().Play("repairAnimation");
        piles[orc.AtLocation() - 1].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        piles[orc.AtLocation() - 1].pileDone();
        // play heal sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HealFixItem, 0.5f);

        foreach (var coin in pileSet)
        {
            if (coin != currCoin)
                coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(2.25f, 2.25f), new Vector2(0f, 0f), 0.2f, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        foreach (var coin in pileSet)
            coin.gameObject.SetActive(false);


        stretch.stretchIn();
        orc.GoToOrigin();


        yield return new WaitForSeconds(2f);
        orc.successOrc();
        yield return new WaitForSeconds(1f);
        orc.stopOrc();

        piles[0].colliderOn();
        piles[1].colliderOn();
        piles[2].colliderOn();
        piles[3].colliderOn();
        piles[4].colliderOn();
        // complete and lock pile
        if (lastLocation == 1)
        {
            pileCompleteArray[0] = true;
            piles[0].pileLock();
        }
        else if (lastLocation == 2)
        {
            pileCompleteArray[1] = true;
            piles[1].pileLock();
        }
        else if (lastLocation == 3)
        {
            pileCompleteArray[2] = true;
            piles[2].pileLock();
        }
        else if (lastLocation == 4)
        {
            pileCompleteArray[3] = true;
            piles[3].pileLock();
        }
        else
        {
            pileCompleteArray[4] = true;
            piles[4].pileLock();
        }

        // make coins interactable
        SetCoinsInteractable(true);

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial audio
                AssetReference clip = GameIntroDatabase.instance.rummageIntro5;
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Clogg, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
            else
            {
                // play encouragement popup
                List<AssetReference> clips = GameIntroDatabase.instance.rummageEncouragementClips;
                AssetReference clip = clips[Random.Range(0, clips.Count)];
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Clogg, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }

            // turn on raycaster
            RummageCoinRaycaster.instance.isOn = true;
            atPile = false;
            NextTutorialEvent();
            yield break;
        }
        else
        {
            if (GameManager.DeterminePlayPopup())
            {
                // play encouragement popup
                List<AssetReference> clips = GameIntroDatabase.instance.rummageEncouragementClips;
                AssetReference clip = clips[Random.Range(0, clips.Count)];
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Clogg, clip);
                // yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
        }

        yield return new WaitForSeconds(.25f);
        StartCoroutine(SetPilesActive(true));
        yield return new WaitForSeconds(.25f);

        // unlock all piles
        UnlockAllPiles();
        atPile = false;
        // turn on raycaster
        RummageCoinRaycaster.instance.isOn = true;
    }

    private IEnumerator WinRoutine(GameObject currCoin)
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        winCount++;

        yield return new WaitForSeconds(.01f);
        List<RummageCoin> pileSet = GetCoinPile(orc.AtLocation() - 1);

        Repairs[orc.AtLocation() - 1].GetComponent<Animator>().Play("repairAnimation");

        foreach (var coin in pileSet)
        {
            if (coin != currCoin)
                coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(2.25f, 2.25f), new Vector2(0f, 0f), 0.2f, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);

        foreach (var coin in pileSet)
            coin.gameObject.SetActive(false);

        piles[orc.AtLocation() - 1].pileDone();
        stretch.stretchIn();

        orc.GoToOrigin();

        yield return new WaitForSeconds(2f);
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
            StudentInfoSystem.GetCurrentProfile().rummageTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            float elapsedTime1 = Time.time - startTime;

            //// ANALYTICS : send minigame_completed event
            StudentPlayerData data1 = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters1 = new Dictionary<string, object>()
            {
                { "minigame_name", GameType.RummageGame.ToString() },
                { "stars_awarded", 0 },
                { "elapsed_time", elapsedTime1 },
                { "tutorial_played", true },
                { "prev_times_played", data1.rummagePlayed },
                { "curr_storybeat", data1.currStoryBeat.ToString() }
            };            
           // AnalyticsManager.SendCustomEvent("minigame_completed", parameters1);

            GameManager.instance.LoadScene("RummageGame", true, 3f);

            yield break;
        }

        yield return new WaitForSeconds(1f);

        // hide dancing man
        StartCoroutine(HideDancingManRoutine());

        // AI stuff
        AIData(StudentInfoSystem.GetCurrentProfile());

        int starsAwarded = CalculateStars();
        float elapsedTime = Time.time - startTime;

        //// ANALYTICS : send minigame_completed event
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "minigame_name", GameType.RummageGame.ToString() },
            { "stars_awarded", starsAwarded },
            { "elapsed_time", elapsedTime },
            { "tutorial_played", false },
            { "prev_times_played", data.rummagePlayed },
            { "curr_storybeat", data.currStoryBeat.ToString() }
        };            
        //AnalyticsManager.SendCustomEvent("minigame_completed", parameters);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(starsAwarded);
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.starsGameBeforeLastPlayed = playerData.starsLastGamePlayed;
        playerData.starsLastGamePlayed = CalculateStars();
        playerData.gameBeforeLastPlayed = playerData.lastGamePlayed;
        playerData.lastGamePlayed = GameType.RummageGame;
        playerData.starsRummage = CalculateStars() + playerData.starsRummage;
        playerData.rummagePlayed++;

        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
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

    private IEnumerator StartGame(int index)
    {
        StartCoroutine(SetPilesActive(false, true));
        orc.channelOrc();

        // make pile larger
        currentPile = index;
        piles[currentPile].GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.1f);
        piles[currentPile].SetWiggleOn();

        List<RummageCoin> pileSet = GetCoinPile(index);
        foreach (var coin in pileSet)
        {
            coin.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(.5f);
        StartCoroutine(ShowCoins(index));
        StartCoroutine(pileBounceCoins(index));
        stretch.stretchOut();
        yield return new WaitForSeconds(1f);
        foreach (var coin in pileSet)
        {
            coin.grow();
            coin.BounceToCloth();
        }

        SelectRandomCoin(index);
    }

    private void NextTutorialEvent()
    {
        int index = tutorialEvent;
        this.piles[index].SetWiggleOn();
        this.piles[index].GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);

        LockAllPilesExcept(index);
        tutorialEvent++;
    }

    private void LockAllPilesExcept(int index)
    {
        print("locking all piles except: " + index);
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
        StartCoroutine(SetPilesActive(false));
        orc.channelOrc();

        // make pile larger
        currentPile = index;
        piles[currentPile].GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.1f);
        piles[currentPile].SetWiggleOn();

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

        if (index == 0)
        {
            // play tutorial audio
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.rummageIntro3);
            clips.Add(GameIntroDatabase.instance.rummageIntro4);
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;
            CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd0.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Clogg, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + 1f);
        }

        // turn on raycaster
        RummageCoinRaycaster.instance.isOn = true;
        // make coins not interactable
        SetCoinsInteractable(true);
    }

    private void SelectTutorialCoin(List<RummageCoin> pile, int index)
    {
        // select current coin
        selectedIndex = correctIndexes[index];
        selectedRummageCoin = pile[selectedIndex];
        print("selectedRummageCoin: " + selectedRummageCoin.type);

        // dancing man animation
        StartCoroutine(DancingManRoutine());
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

            coin.ToggleVisibility(true);
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
        int i = 0;
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            coin.BounceOut1();
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f + (i * 0.1f));
            yield return new WaitForSeconds(0.2f);
            i++;
        }
    }

    private IEnumerator pileBounceInCoins(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);
        int i = 0;
        foreach (var coin in pile)
        {
            coin.shrink();
            coin.BounceIn1();
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f - (i * 0.1f));
            yield return new WaitForSeconds(0.2f);
            i++;
        }
    }

    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<RummageCoin> row = GetCoinPile(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false);
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

    private IEnumerator SetPilesActive(bool opt, bool excludeSelectedPile = false)
    {
        if (opt)
        {
            foreach (var p in piles)
            {
                if (excludeSelectedPile)
                {
                    if (p == selectedPile)
                        continue;
                }

                // set active if pile not locked
                if (p.currPileLock)
                {
                    p.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                    p.SetWiggleOn();
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            foreach (var p in piles)
            {
                if (excludeSelectedPile)
                {
                    if (p == selectedPile)
                        continue;
                }

                p.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
                p.SetWiggleOff();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void ToggleWally(bool opt)
    {
        if (opt)
        {
            StartCoroutine(ShowDancingManRoutine());
        }
        else
        {
            StartCoroutine(HideDancingManRoutine());
        }
    }

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

    public void SetCoinsInteractable(bool opt)
    {
        if (allCoins != null)
        {
            foreach (var coin in allCoins)
            {
                coin.interactable = opt;
            }
        }
    }
}
