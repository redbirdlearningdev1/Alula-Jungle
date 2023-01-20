using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class PrintingGameManager : MonoBehaviour
{
    public static PrintingGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;
    public bool playTutorial;

    public bool glowCorrectCoin = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;

    [HideInInspector] public ActionWordEnum correctValue;
    private int timesMissed = 0;
    private int timesCorrect = 0;

    [Header("Tutorial Variables")]
    public int[] t_correctIndexes;
    public List<ActionWordEnum> t_firstRound;
    public List<ActionWordEnum> t_secondRound;
    public List<ActionWordEnum> t_thirdRound;
    public List<ActionWordEnum> t_fourthRound;

    private int t_currRound = 0;
    [HideInInspector] public bool t_waitingForPlayer = false;

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

        // get mapID
        mapID = GameManager.instance.mapID;
    }

    void Start()
    {
        // set start time
        startTime = Time.time;

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().pirateTutorial;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = true;
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
    }

    private void PregameSetup()
    {
        // reset game parts
        PrintingRayCaster.instance.isOn = false;
        BallsController.instance.ResetBalls();
        PirateRopeController.instance.ResetRope();
        ParrotController.instance.interactable = false;
        RopeCoin.instance.interactable = false;

        // add ambiance sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.SeaAmbiance, 0.1f, "sea_ambiance");

        globalCoinPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (GameManager.instance.practiceModeON)
        {
            globalCoinPool.AddRange(GameManager.instance.practicePhonemes);
        }
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


        // play tutorial if first time
        if (playTutorial)
        {
            StartCoroutine(StartTutorial());
        }
        else
        {
            // start song
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.PirateSongSplit);

            // place menu button
            SettingsManager.instance.ToggleMenuButtonActive(true);

            // start game
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartTutorial()
    {
        // small delay before starting
        yield return new WaitForSeconds(2f);

        // play tutorial audio
        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.pirateIntro1);
        clips.Add(GameIntroDatabase.instance.pirateIntro2);
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
        yield return cd.coroutine;
        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
        yield return cd0.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Ollie, clips);
        yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + 1f);


        // reset rope
        PirateRopeController.instance.ResetRope();
        PirateRopeController.instance.DropRope();
        yield return new WaitForSeconds(1f);

        correctValue = t_firstRound[t_correctIndexes[0]];
        yield return new WaitForSeconds(1f);
        RopeCoin.instance.GetComponent<WiggleController>().StartWiggle();
        t_waitingForPlayer = true;
        RopeCoin.instance.interactable = true;

        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // wait for player input
        while (t_waitingForPlayer)
            yield return null;

        // disable rope coin
        RopeCoin.instance.GetComponent<WiggleController>().StopWiggle();

        yield return new WaitForSeconds(1.25f);
        RopeCoin.instance.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        RopeCoin.instance.interactable = false;
        yield return new WaitForSeconds(1f);

        // play tutorial audio
        AssetReference clip = GameIntroDatabase.instance.pirateIntro3;
        CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd1.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Ollie, clip);
        yield return new WaitForSeconds(cd1.GetResult() + 1f);

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // get correct value
        int correctIndex = 0;
        if (playTutorial)
        {
            correctIndex = t_correctIndexes[t_currRound];
        }
        else
        {
            correctIndex = Random.Range(0, BallsController.instance.balls.Count);
        }


        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();

        // set ball values
        int i = 0;
        foreach (Ball ball in BallsController.instance.balls)
        {
            // set ball value
            ActionWordEnum value = ActionWordEnum._blank;
            if (playTutorial)
            {
                switch (t_currRound)
                {
                    case 0: value = t_firstRound[i]; break;
                    case 1: value = t_secondRound[i]; break;
                    case 2: value = t_thirdRound[i]; break;
                    case 3: value = t_fourthRound[i]; break;
                }
            }
            else
            {
                value = GetUnusedWord();
            }
            ball.SetValue(value);

            // find correct value
            if (i == correctIndex)
            {
                correctValue = value;
                // for testing purposes
                if (glowCorrectCoin)
                    ImageGlowController.instance.SetImageGlow(ball.GetComponent<Image>(), true, GlowValue.glow_1_00);
            }
            else
            {
                // for testing purposes
                if (glowCorrectCoin)
                    ImageGlowController.instance.SetImageGlow(ball.GetComponent<Image>(), false);
            }

            i++;
        }

        yield return new WaitForSeconds(1f);
        BallsController.instance.ShowBalls();
        yield return new WaitForSeconds(1f);

        if (playTutorial && t_currRound == 0)
        {
            // do nothing :)
        }
        else
        {
            PirateRopeController.instance.DropRope();
            yield return new WaitForSeconds(0.5f);
        }

        ParrotController.instance.SayAudio(correctValue, true);
        yield return new WaitForSeconds(1f);

        // turn on raycaster + parrot
        PrintingRayCaster.instance.isOn = true;
        RopeCoin.instance.interactable = true;
    }

    public bool EvaluateSelectedBall(ActionWordEnum ball)
    {
        // turn off raycaster + parrot
        PrintingRayCaster.instance.isOn = false;
        ParrotController.instance.StopAllCoroutines();
        ParrotController.instance.interactable = false;
        RopeCoin.instance.StopAllCoroutines();
        RopeCoin.instance.interactable = false;

        bool success = (ball == correctValue);
        // only track phoneme attempt if not in tutorial AND not in practice mode
        if (!playTutorial  /*&& !GameManager.instance.practiceModeON */)
        {
            StudentInfoSystem.SavePlayerMinigameRoundAttempt(GameType.PirateGame, success);
            StudentInfoSystem.SavePlayerPhonemeAttempt(correctValue, success);
        }

        // correct!
        if (success)
        {
            StartCoroutine(CorrectBallRoutine());
            return true;
        }
        // incorrcet!
        else
        {
            if (playTutorial)
            {
                // turn on raycaster + parrot
                PrintingRayCaster.instance.isOn = true;
                ParrotController.instance.interactable = true;
                RopeCoin.instance.interactable = true;
            }
            else
            {
                StartCoroutine(IncorrectBallRoutine());
            }

            return false;
        }
    }

    private IEnumerator CorrectBallRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        timesCorrect++;

        // parrot animation
        ParrotController.instance.CelebrateAnimation(3f);

        // load cannon
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Load");

        // shoot cannon
        yield return new WaitForSeconds(0.25f);
        CannonController.instance.cannonAnimator.Play("Shoot");
        CannonController.instance.explosionAnimator.Play("hit");
        yield return new WaitForSeconds(0.15f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonShoot, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonHitCoin, 0.5f);

        // set coin value
        yield return new WaitForSeconds(0.25f);
        PirateRopeController.instance.printingCoin.SetActionWordValue(correctValue);

        // play sound coin audio
        PirateRopeController.instance.printingCoin.LerpScale(new Vector2(1.2f, 1.2f), 0.1f);
        AudioManager.instance.PlayPhoneme(ChallengeWordDatabase.ActionWordEnumToElkoninValue(correctValue));
        yield return new WaitForSeconds(1f);

        // drop coin and upgrade chest
        PirateRopeController.instance.DropCoinAnimation();
        yield return new WaitForSeconds(0.4f);
        PirateChest.instance.UpgradeChest();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
        AudioManager.instance.PlayCoinDrop();
        yield return new WaitForSeconds(1f);

        // end tutorial after 3 turns
        if (timesCorrect >= 4 || t_currRound == 2)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // reset balls and start new round
        BallsController.instance.ResetBalls();

        // play tutorial audio
        if (playTutorial && t_currRound == 0)
        {
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.pirateIntro4);
            clips.Add(GameIntroDatabase.instance.pirateIntro5);
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;
            CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd0.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Ollie, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd0.GetResult() + 1f);
        }
        else
        {
            if (GameManager.DeterminePlayPopup())
            {
                // play random encouragement popup
                int index = Random.Range(0, 3);

                // skip #3 on jeff's request
                if (index == 2)
                    index = 3;

                AssetReference clip = GameIntroDatabase.instance.pirateEncouragementClips[index];
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Ollie, clip);
                //yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
        }

        // increase tutorial round
        if (playTutorial)
            t_currRound++;

        StartCoroutine(StartGame());
    }

    private IEnumerator IncorrectBallRoutine()
    {
        timesMissed++;

        // parrot animation
        ParrotController.instance.SadAnimation(3f);

        // load cannon
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Load");

        // shoot cannon
        yield return new WaitForSeconds(0.25f);
        CannonController.instance.cannonAnimator.Play("Shoot");
        CannonController.instance.explosionAnimator.Play("miss");
        yield return new WaitForSeconds(0.15f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonShoot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonFall, 0.25f);


        if (!playTutorial)
        {
            // play reminder popup
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.pirateReminder1);
            clips.Add(GameIntroDatabase.instance.pirateReminder2);
            clips.Add(GameIntroDatabase.instance.pirateEncouragementClips[2]);

            AssetReference clip = clips[Random.Range(0, clips.Count)];
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Ollie, clip);
            //yield return new WaitForSeconds(cd.GetResult() + 1f);
        }

        // raise rope
        PirateRopeController.instance.RaiseRopeAnimation();
        yield return new WaitForSeconds(2f);

        // reset balls and start new round
        BallsController.instance.ResetBalls();
        StartCoroutine(StartGame());
    }

    private IEnumerator WinRoutine()
    {
        // parrot fly!!!
        ParrotController.instance.WinAnimation();

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

        if (playTutorial)
        {
            // save to SIS
            StudentInfoSystem.GetCurrentProfile().pirateTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            float elapsedTime = Time.time - startTime;

            //// ANALYTICS : send minigame_completed event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "minigame_name", GameType.PirateGame.ToString() },
                { "stars_awarded", 0 },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", true },
                { "prev_times_played", data.piratePlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            //AnalyticsManager.SendCustomEvent("minigame_completed", parameters);

            GameManager.instance.LoadScene("NewPirateGame", true, 3f);
        }
        else
        {
            // AI stuff
            AIData(StudentInfoSystem.GetCurrentProfile());

            int starsAwarded = CalculateStars();
            float elapsedTime = Time.time - startTime;

            //// ANALYTICS : send minigame_completed event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "minigame_name", GameType.PirateGame.ToString() },
                { "stars_awarded", starsAwarded },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", false },
                { "prev_times_played", data.piratePlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            //AnalyticsManager.SendCustomEvent("minigame_completed", parameters);

            // calculate and show stars
            StarAwardController.instance.AwardStarsAndExit(starsAwarded);
        }
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.starsGameBeforeLastPlayed = playerData.starsLastGamePlayed;
        playerData.starsLastGamePlayed = CalculateStars();
        playerData.gameBeforeLastPlayed = playerData.lastGamePlayed;
        playerData.lastGamePlayed = GameType.PirateGame;
        playerData.starsPirate = CalculateStars() + playerData.starsPirate;
        playerData.piratePlayed++;

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

        // make sure word is not being used or already successfully completed
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }
}
