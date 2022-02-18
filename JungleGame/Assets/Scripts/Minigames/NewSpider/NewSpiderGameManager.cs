using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct SpiderwebTutorialList
{
    public List<ActionWordEnum> list;
}

public class NewSpiderGameManager : MonoBehaviour
{
    public static NewSpiderGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;

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


    public List<UniversalCoinImage> coins;
    public List<Transform> coinPosOffScreen;
    public List<Transform> coinPosOnScreen;

    private int selectedIndex;
    [HideInInspector] public UniversalCoinImage selectedCoin;

    private int winCount = 0;
    private int timesMissed = 0;

    [Header("Tutorial Stuff")]
    public bool playTutorial;
    public List<SpiderwebTutorialList> tutorialLists;
    public int[] correctIndexes;
    [HideInInspector] public int tutorialEvent = 0;

    /* 
    ################################################
    #   MONOBEHAVIOR METHODS
    ################################################
    */

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

        // get mapID
        mapID = GameManager.instance.mapID;
    }

    void Start()
    {
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().spiderwebTutorial;

        PregameSetup();

        // play tutorial if first time
        if (playTutorial)
        {
            StartCoroutine(StartTutorialGame());
        }
        else
        {
            // start song
            AudioManager.instance.InitSplitSong(SplitSong.Spiderweb);
            AudioManager.instance.IncreaseSplitSong();

            StartCoroutine(StartGame());
        }
    }

    void Update()
    {
        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                // play win tune
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
                // save tutorial done to SIS
                StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = true;
                // times missed set to 0
                timesMissed = 0;
                // update AI data
                AIData(StudentInfoSystem.GetCurrentProfile());
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(CalculateStars());
                // remove all raycast blockers
                RaycastBlockerController.instance.ClearAllRaycastBlockers();
            }
        }
    }

    private void PregameSetup()
    {
        // play forest ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 1f, "forest_ambiance");

        // turn off raycaster
        SpiderRayCaster.instance.isOn = false;

        globalCoinPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }

        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);
    }

    /* 
    ################################################
    #   NORMAL GAME METHODS
    ################################################
    */

    public bool EvaluateSelectedSpiderCoin(ActionWordEnum coin, UniversalCoinImage correctCoin)
    {
        // turn off raycaster
        SpiderRayCaster.instance.isOn = false;

        if (coin ==  ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value))
        {
            winCount++;


            // finish tutorial after 3 rounds
            if (playTutorial && winCount == 3)
            {
                StartCoroutine(TutorialWinRoutine(correctCoin));
                return true;
            }

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
        webber2.gameObject.SetActive(true);
        webber2.grabBug();
        yield return new WaitForSeconds(.4f);
        bug.webGetEat();
        yield return new WaitForSeconds(.5f);
        webber2.gameObject.SetActive(false);
        StartCoroutine(CoinsDown());

        // play om nom nom sound
        spider.fail();
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Spindle, GameIntroDatabase.instance.spiderwebsOmNomNom);
        yield return new WaitForSeconds(GameIntroDatabase.instance.spiderwebsOmNomNom.length + 1f);
        spider.idle();

        // play reminder popup
        List<AudioClip> clips = new List<AudioClip>();
        clips.Add(GameIntroDatabase.instance.spiderwebsReminder1);
        clips.Add(GameIntroDatabase.instance.spiderwebsReminder2);

        AudioClip clip = clips[Random.Range(0, clips.Count)];
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clip);
        yield return new WaitForSeconds(clip.length + 1f);


        StartCoroutine(StartGame());
    }

    private IEnumerator CoinSuccessRoutine(UniversalCoinImage coin)
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // play right choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);

        spider.success();
        yield return new WaitForSeconds(1f);
        webber.gameObject.SetActive(true);

        // play web grab sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WebWhip, 0.5f);

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
        
        coin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(.15f);
        ball.UpgradeChest();

        StartCoroutine(bugLeaves());
        webber.gameObject.SetActive(false);

        StartCoroutine(CoinsDown());
        yield return new WaitForSeconds(2f);


        // play encouragement popup
        List<AudioClip> clips = GameIntroDatabase.instance.spiderwebsEncouragementClips;
        AudioClip clip = clips[Random.Range(0, clips.Count)];
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clip);
        yield return new WaitForSeconds(clip.length + 1f);
        
        StartCoroutine(StartGame());
    }

    private IEnumerator WinRoutine(UniversalCoinImage coin)
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

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
        
        coin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(.15f);
        ball.UpgradeChest();

        StartCoroutine(bugLeaves());
        webber.gameObject.SetActive(false);

        StartCoroutine(CoinsDown());
        yield return new WaitForSeconds(2f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(2f);

        // AI stuff
        AIData(StudentInfoSystem.GetCurrentProfile());

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.starsGameBeforeLastPlayed = playerData.starsLastGamePlayed;
        playerData.starsLastGamePlayed = CalculateStars();
        playerData.gameBeforeLastPlayed = playerData.lastGamePlayed;
        playerData.lastGamePlayed = GameType.SpiderwebGame;
        playerData.starsSpiderweb = CalculateStars() + playerData.starsSpiderweb;
        playerData.totalStarsSpiderweb = 3 + playerData.totalStarsSpiderweb;

        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
    }

    private IEnumerator StartGame()
    {
        ResetCoins();
        bug.goToOrigin();
        yield return new WaitForSeconds(1f);

        SetCoins();
        bug.StartToWeb();
        yield return new WaitForSeconds(1.5f);

        web.webSmall();
        bug.BugBounce();
        yield return new WaitForSeconds(1.5f);

        // play audio
        bug.PlayPhonemeAudio();
        yield return new WaitForSeconds(1f);

        // bring coins up
        StartCoroutine(CoinsUp());
        yield return new WaitForSeconds(1f);

        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // turn on raycaster
        SpiderRayCaster.instance.isOn = true;
    }

    /* 
    ################################################
    #   TUTORIAL GAME METHODS
    ################################################
    */

    private IEnumerator StartTutorialGame()
    {
        yield return new WaitForSeconds(1f);

        // play tutorial audio
        List<AudioClip> clips = new List<AudioClip>();
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro1);
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro2);
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clips);
        yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);

        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        StartCoroutine(PlayTutorialGame());
    }

    private IEnumerator PlayTutorialGame()
    {
        ResetCoins();
        bug.goToOrigin();
        yield return new WaitForSeconds(1f);

        SetCoins();
        bug.StartToWeb();
        yield return new WaitForSeconds(1.5f);

        web.webSmall();
        bug.BugBounce();
        yield return new WaitForSeconds(1.5f);

        if (tutorialEvent == 1)
        {
            // turn on raycaster
            SpiderRayCaster.instance.isOn = true;
            // make bug glow
            ImageGlowController.instance.SetImageGlow(bug.image, true, GlowValue.glow_1_00);
        }
        else
        {
            // play audio
            bug.PlayPhonemeAudio();
            yield return new WaitForSeconds(1f);

            // bring coins up
            StartCoroutine(CoinsUp());
            yield return new WaitForSeconds(1f);

            // turn on raycaster
            SpiderRayCaster.instance.isOn = true;
        }
    }

    public void ContinueTutorialPart()
    {
        // bring coins up
        StartCoroutine(CoinsUp());

        // turn on raycaster
        SpiderRayCaster.instance.isOn = true;
    }

    private IEnumerator TutorialFailRoutine()
    {
        // play wrong choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);
        yield return new WaitForSeconds(1f);
        // turn on raycaster
        SpiderRayCaster.instance.isOn = true;
    }

    private IEnumerator TutorialSuccessRoutine(UniversalCoinImage coin)
    {
        // play right choice audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);

        spider.success();
        yield return new WaitForSeconds(1f);
        webber.gameObject.SetActive(true);

        // play web grab sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WebWhip, 0.5f);
        
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
        
        coin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(.15f);
        ball.UpgradeChest();

        StartCoroutine(bugLeaves());
        webber.gameObject.SetActive(false);

        StartCoroutine(CoinsDown());
        yield return new WaitForSeconds(2f);

        if (tutorialEvent == 1)
        {
            // play tutorial audio
            AudioClip clip = GameIntroDatabase.instance.spiderwebsIntro5;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else
        {
            // play encouragement popup
            List<AudioClip> clips = GameIntroDatabase.instance.spiderwebsEncouragementClips;
            AudioClip clip = clips[Random.Range(0, clips.Count)];
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        
        StartCoroutine(PlayTutorialGame());
    }

    private IEnumerator TutorialWinRoutine(UniversalCoinImage coin)
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
        
        coin.ToggleVisibility(false, true);
        yield return new WaitForSeconds(.15f);
        ball.UpgradeChest();

        StartCoroutine(bugLeaves());
        webber.gameObject.SetActive(false);

        StartCoroutine(CoinsDown());
        yield return new WaitForSeconds(2f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(2f);

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = true;
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
        bug.takeOff();

        // play bug leave sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BugFlyOut, 0.5f);

        yield return new WaitForSeconds(.25f);
        bug.leaveWeb();
    }

    private void SetCoins()
    {
        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        int i = 0;
        foreach (var coin in coins)
        {
            ActionWordEnum type;
            if (!playTutorial)
                type = GetUnusedWord();
            else
            {   
                type = tutorialLists[tutorialEvent].list[i];
                i++;
            }
            
            coin.SetActionWordValue(type);
            coin.ToggleVisibility(true, true);
        }

        // select random coin OR tutorial coin
        if (!playTutorial)
            SelectRandomCoin();
        else
        {
            selectedIndex = correctIndexes[tutorialEvent];
            selectedCoin = coins[selectedIndex];
            bug.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value));
            tutorialEvent++;
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

    private IEnumerator CoinsUp()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            Vector2 pos = coinPosOnScreen[i].position;
            Vector2 bouncePos = pos;
            bouncePos.y += 0.5f;

            coins[i].GetComponent<LerpableObject>().LerpPosition(bouncePos, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            coins[i].GetComponent<LerpableObject>().LerpPosition(pos, 0.2f, false);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator CoinsDown()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            Vector2 pos = coinPosOffScreen[i].position;
            Vector2 bouncePos = pos;
            bouncePos.y += 0.5f;

            coins[i].GetComponent<LerpableObject>().LerpPosition(bouncePos, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            coins[i].GetComponent<LerpableObject>().LerpPosition(pos, 0.2f, false);
        }
    }

    private void ResetCoins()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].transform.position = coinPosOffScreen[i].position;
        }
    }

    public void ReturnCoinsToPosition()
    {
        int i = 0;
        foreach (var coin in coins)
        {
            coin.GetComponent<LerpableObject>().LerpPosition(coinPosOnScreen[i].position, 0.25f, false);
            i++;
        }
    }

    private void SelectRandomCoin()
    {
        selectedIndex = Random.Range(0, coins.Count);
        selectedCoin = coins[selectedIndex];
        bug.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value));
    }
}
