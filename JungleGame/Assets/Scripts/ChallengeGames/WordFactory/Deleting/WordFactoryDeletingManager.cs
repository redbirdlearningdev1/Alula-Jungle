using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro;

public class WordFactoryDeletingManager : MonoBehaviour
{
    public static WordFactoryDeletingManager instance;

    public bool showChallengeWordLetters;

    public Polaroid polaroid; // main polarid used in this game
    public GameObject universalCoinImage; // universal coin prefab
    public Transform coinsParent;
    public Vector2 normalCoinSize;

    private List<WordPair> prevPairs;
    private WordPair currentPair;
    private ChallengeWord currentWord;
    private UniversalCoinImage currentCoin;

    private List<UniversalCoinImage> currentCoins;
    private bool playingCoinAudio;
    private bool evaluatingCoin = false;
    private bool playIntro = false;

    private int numWins = 0;
    private int numMisses = 0;

    [Header("Tutorial")]
    public bool playTutorial;
    private int tutorialEvent = 0;
    public WordPair tutorialPair1;

    private bool firstTry;

    private float startTime;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
        // times missed set to 0
        numMisses = 0;
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

    void Start()
    {
        // set start time
        startTime = Time.time;

        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial;
        // start split song if not tutorial
        if (!playTutorial)
        {
            // start song
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.jadeSplitSong);
        }

        PregameSetup();
    }

    private void PregameSetup()
    {
        // init empty list
        prevPairs = new List<WordPair>();

        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive
        TigerController.instance.ResetCards();

        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        firstTry = true;
        // choose correct pair
        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentPair = tutorialPair1;
                    break;
            }
            tutorialEvent++;
        }
        else if (GameManager.instance.practiceModeON)
        {
            // new pair
            currentPair = AISystem.ChallengeWordBuildingDeleting(false, prevPairs, GameManager.instance.practicePhonemes, GameManager.instance.practiceDifficulty);
            prevPairs.Add(currentPair);
        }
        else
        {
            // new pair
            currentPair = AISystem.ChallengeWordBuildingDeleting(false, prevPairs);
            prevPairs.Add(currentPair);
        }


        // init game delay
        yield return new WaitForSeconds(0.5f);

        // tutorial stuff
        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 1
                AssetReference clip = GameIntroDatabase.instance.deletingIntro1;

                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd2.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd2.GetResult() + 1f);
            }
        }
        else if (!GameManager.instance.practiceModeON)
        {
            if (!playIntro)
            {
                // play start 1
                AssetReference clip = GameIntroDatabase.instance.deletingStart1;

                // CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                // yield return cd0.coroutine;

                // TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                // yield return new WaitForSeconds(cd0.GetResult() + 1f);

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_4)
                {
                    // play start 2
                    clip = GameIntroDatabase.instance.deletingStart2Chapters1_3;

                    CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd1.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    //yield return new WaitForSeconds(cd1.GetResult() + 1f);
                }
                else
                {
                    // play start 2
                    clip = GameIntroDatabase.instance.deletingStart2Chapters4_5;

                    CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd1.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    //yield return new WaitForSeconds(cd1.GetResult() + 1f);
                }
            }
        }

        // add settings button
        if (!playIntro)
        {
            playIntro = true;
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlide, 0.25f);
        yield return new WaitForSeconds(0.5f);

        // open emerald head
        EmeraldHead.instance.animator.Play("OpenMouth");
        yield return new WaitForSeconds(1.5f);

        // choose challenge word + play enter animation
        currentWord = currentPair.word2;
        polaroid.SetPolaroid(currentWord);
        //yield return new WaitForSeconds(1f);

        // play start animations
        TigerController.instance.tigerAnim.Play("TigerSwipe");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.25f);
        EmeraldHead.instance.animator.Play("EnterPolaroid");
        yield return new WaitForSeconds(0.25f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink1, 0.5f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink2, 0.5f, "glass_dink", 1.5f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink1, 0.5f, "glass_dink", 1.2f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink2, 0.5f, "glass_dink", 0.8f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);

        // set invisible frames
        InvisibleFrameLayout.instance.SetNumberOfFrames(currentWord.elkoninCount);
        VisibleFramesController.instance.SetNumberOfFrames(currentWord.elkoninCount);
        yield return new WaitForSeconds(0.5f);

        // throw out real frames
        VisibleFramesController.instance.PlaceActiveFrames(polaroid.transform.localPosition);
        VisibleFramesController.instance.MoveFramesToInvisibleFrames();
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);
        //yield return new WaitForSeconds(1f);

        // show challenge word coins
        currentCoins = new List<UniversalCoinImage>();
        yield return new WaitForSeconds(0.5f);

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoinImage, VisibleFramesController.instance.frames[i].transform.position, Quaternion.identity, coinsParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            currentCoins.Add(coin);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // say each letter + grow coin
        foreach (var coin in currentCoins)
        {
            PlayAudioCoin(coin);
            yield return new WaitForSeconds(0.8f);
        }
        yield return new WaitForSeconds(0.1f);

        // say challenge word
        AudioManager.instance.PlayTalk(currentWord.audio);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        foreach (var coin in currentCoins)
        {
            coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (var coin in currentCoins)
        {
            coin.LerpScale(Vector2.one, 0.25f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.25f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);


        // tutorial stuff
        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                yield return new WaitForSeconds(1f);

                // play tutorial intro 2
                AssetReference clip = GameIntroDatabase.instance.deletingIntro2;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);

                // play tutorial intro 3
                clip = GameIntroDatabase.instance.deletingIntro3;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 1f);
            }
        }

        // squish polaroid
        EmeraldHead.instance.animator.Play("SquishPolaroid");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidCrunch, 0.5f);
        yield return new WaitForSeconds(1.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidUnravel, 0.5f);
        yield return new WaitForSeconds(0.5f);

        polaroid.SetPolaroid(currentPair.word1);

        // unsquish polaroid 
        EmeraldHead.instance.animator.Play("UnsquishPolaroid");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        //yield return new WaitForSeconds(1.5f);
        // say new challenge word
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);


        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentPair.word1.audio));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(currentPair.word1.audio);
        yield return new WaitForSeconds(cd.GetResult());
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // tutorial stuff
        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                yield return new WaitForSeconds(1f);

                // play tutorial intro 4
                AssetReference clip = GameIntroDatabase.instance.deletingIntro4;


                cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
        }

        // turn on raycaster
        WordFactoryDeletingRaycaster.instance.isOn = true;
        evaluatingCoin = false;
    }

    public void EvaluateCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        // turn off raycaster
        WordFactoryDeletingRaycaster.instance.isOn = false;

        // return coins to position (except current coin)
        currentCoin = coin;
        WordFactoryDeletingManager.instance.ReturnCoinsToFrame();

        // eat current coin
        currentCoin.GetComponent<LerpableObject>().LerpPosition(EmeraldTigerHolder.instance.transform.position, 0.25f, false);
        currentCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);

        bool success = (coin.value == currentPair.word2.elkoninList[currentPair.index]);
        // only track challenge round attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            int difficultyLevel = 1 + Mathf.FloorToInt(StudentInfoSystem.GetCurrentProfile().starsDel / 3);
            StudentInfoSystem.SavePlayerChallengeRoundAttempt(GameType.WordFactoryDeleting, success, currentPair.word2, difficultyLevel);
        }

        // win
        if (success)
        {
            // audio fx
            AudioManager.instance.PlayCoinDrop();

            numWins++;
            StartCoroutine(PostRound(true));
        }
        // lose 
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        // emerald tiger thinking
        EmeraldTigerHolder.instance.Thinking();
        yield return new WaitForSeconds(1f);

        // win round
        if (win)
        {
            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            // increase split song
            AudioManager.instance.IncreaseSplitSong();

            // emerald tiger correct
            EmeraldTigerHolder.instance.SetCorrect(true);

            // remove coin from list
            currentCoins.Remove(currentCoin);

            // hide coins
            int x = 0;
            foreach (var coin in currentCoins)
            {
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * x));
                coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
                yield return new WaitForSeconds(0.1f);
                x++;
            }
            yield return new WaitForSeconds(0.5f);

            // remove extra frame
            VisibleFramesController.instance.RemoveFrameSmooth(currentPair.index);
            yield return new WaitForSeconds(0.5f);

            x = 0;
            foreach (var coin in currentCoins)
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(VisibleFramesController.instance.frames[x].transform, 0f, false);
                x++;
            }
            // yield return new WaitForSeconds(0.5f);

            // show coins
            x = 0;
            foreach (var coin in currentCoins)
            {
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1.5f - 0.25f * x));
                coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
                yield return new WaitForSeconds(0.1f);
                x++;
            }
            // yield return new WaitForSeconds(1f);

            if (showChallengeWordLetters)
            {
                LerpableObject pol = polaroid.GetComponent<LerpableObject>();

                // squish on x scale
                pol.LerpScale(new Vector2(0f, 1f), 0.2f);
                yield return new WaitForSeconds(0.2f);
                // play audio fx 
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 1f);
                // remove image
                polaroid.ShowPolaroidWord(120f);
                // un-squish on x scale
                pol.LerpScale(new Vector2(1f, 1f), 0.2f);
                yield return new WaitForSeconds(1f);

                // say letter groups using coins
                for (int j = 0; j < polaroid.challengeWord.elkoninCount; j++)
                {
                    GameObject letterElement = polaroid.GetLetterGroupElement(j);
                    letterElement.GetComponent<TextMeshProUGUI>().color = Color.black;
                    letterElement.GetComponent<LerpableObject>().LerpTextSize(150f - (Polaroid.FONT_SCALE_DECREASE * polaroid.challengeWord.elkoninCount), 0.2f);
                    currentCoins[j].LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
                    // audio fx
                    AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(currentCoins[j].value).audio);
                    AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * j));
                    yield return new WaitForSeconds(1f);
                    currentCoins[j].LerpScale(new Vector2(1f, 1f), 0.25f);
                }
                yield return new WaitForSeconds(0.2f);

                // read word aloud to player
                if (currentPair.word1.audio != null)
                    AudioManager.instance.PlayTalk(currentPair.word1.audio);
                // start wiggle
                polaroid.ToggleWiggle(true);

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentPair.word1.audio));
                yield return cd.coroutine;

                // wait an appropriate amount of time
                if (currentPair.word1.audio != null)
                    yield return new WaitForSeconds(cd.GetResult() + 0.25f);
                else
                    yield return new WaitForSeconds(2f);

                // end wiggle
                polaroid.ToggleWiggle(false);
            }
            else
            {
                // say new challenge word
                AudioManager.instance.PlayTalk(currentPair.word1.audio);
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
                foreach (var coin in currentCoins)
                {
                    coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.5f);
                foreach (var coin in currentCoins)
                {
                    coin.LerpScale(Vector2.one, 0.25f);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.25f);
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
            }

            // tutorial stuff
            if (playTutorial)
            {
                if (tutorialEvent == 1)
                {
                    yield return new WaitForSeconds(1f);

                    // play tutorial intro 5
                    AssetReference clip = GameIntroDatabase.instance.deletingIntro5;

                    CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd0.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(cd0.GetResult() + 1f);
                }
            }
        }
        // lose round
        else
        {
            // play wrong sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            // emerald tiger wrong
            EmeraldTigerHolder.instance.SetCorrect(false);

            if (playTutorial || firstTry)
            {
                // return coin to frame
                currentCoin.GetComponent<LerpableObject>().LerpScale(Vector2.one, 0.5f);
                currentCoin = null;
                WordFactoryDeletingManager.instance.ReturnCoinsToFrame();
                yield return new WaitForSeconds(0.5f);

                firstTry = false;

                // turn on raycaster
                WordFactoryDeletingRaycaster.instance.isOn = true;

                evaluatingCoin = false;

                yield break;
            }

            // only increase misses if not playing tutorial
            if (!playTutorial)
                numMisses++;
        }

        // eat the polaroid
        EmeraldHead.instance.animator.Play("EatPolaroid");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidCrunch, 0.5f);
        // yield return new WaitForSeconds(1.5f);


        // award card to correct person
        if (win)
        {
            WinCardsController.instance.AddPolaroid();
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 0.5f);
        }
        else
        {
            TigerController.instance.AddTigerPolaroid();
            // audio fx
            // AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
        }
        // yield return new WaitForSeconds(1f);

        // remove current coin
        currentCoins.Remove(currentCoin);
        Destroy(currentCoin.gameObject);

        // remove coins and frames
        VisibleFramesController.instance.RemoveFrames();
        int i = 0;
        foreach (var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.1f, "water_splash", (1f - 0.25f * i));
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "water_splash", (1f - 0.2f * i));
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        yield return new WaitForSeconds(0.5f);

        // remove coins
        foreach (var coin in currentCoins)
        {
            Destroy(coin.gameObject);
        }
        currentCoins.Clear();

        // end tutorial
        if (playTutorial && tutorialEvent == 1)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // win or lose game ?
        if (numWins >= 3)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }
        else if (numMisses >= 3)
        {
            StartCoroutine(LoseRoutine());
            yield break;
        }

        // play appropriate reminder / encouragement popup
        if (playTutorial && tutorialEvent > 1 || !playTutorial)
        {
            if (win)
            {
                if (GameManager.DeterminePlayPopup())
                {
                    int index = Random.Range(0, 2);
                    AssetReference clip = null;
                    if (index == 0)
                    {
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ugh");
                    }
                    else if (index == 1)
                    {
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_grr");
                    }

                    CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd0.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    // yield return new WaitForSeconds(cd0.GetResult() + 1f);
                }
            }
            else
            {
                //int index = Random.Range(0, 3);
                AssetReference clip = null;
                // if (index == 0)
                // {
                //     clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_haha");
                // }
                // else if (index == 1)
                // {
                //     clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ahhah");
                // }
                // else if (index == 2)
                // {
                //     clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_hrm");
                // }

                // CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                // yield return cd0.coroutine;

                // TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                // yield return new WaitForSeconds(cd0.GetResult() + 1f);

                // play reminder popup
                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    List<AssetReference> clips = GameIntroDatabase.instance.deletingReminderClipsChapters1_4;
                    clip = clips[Random.Range(0, clips.Count)];

                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    //yield return new WaitForSeconds(cd.GetResult() + 1f);
                }
                else
                {
                    List<AssetReference> clips = GameIntroDatabase.instance.deletingReminderClipsChapter5;
                    clip = clips[Random.Range(0, clips.Count)];

                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    //yield return new WaitForSeconds(cd.GetResult() + 1f);
                }
            }
        }

        // reset current polaroid
        polaroid.HidePolaroidWord();

        StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        print("you win!");

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            float elapsedTime = Time.time - startTime;

            //// ANALYTICS : send challengegame_completed event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "challengegame_name", GameType.WordFactoryDeleting.ToString() },
                { "stars_awarded", 0 },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", true },
                { "prev_times_played", data.delPlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            //AnalyticsManager.SendCustomEvent("challengegame_completed", parameters);

            GameManager.instance.LoadScene("WordFactoryDeleting", true, 3f);
        }
        else
        {
            // AI stuff
            AIData(StudentInfoSystem.GetCurrentProfile());

            int starsAwarded = CalculateStars();
            float elapsedTime = Time.time - startTime;

            //// ANALYTICS : send challengegame_completed event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "challengegame_name", GameType.WordFactoryDeleting.ToString() },
                { "stars_awarded", starsAwarded },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", false },
                { "prev_times_played", data.delPlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            //AnalyticsManager.SendCustomEvent("challengegame_completed", parameters);

            // calculate and show stars
            StarAwardController.instance.AwardStarsAndExit(starsAwarded);
        }
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.delPlayed = playerData.delPlayed + 1;
        playerData.starsDel = CalculateStars() + playerData.starsDel;

        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
    }

    private int CalculateStars()
    {
        if (numMisses <= 0)
            return 3;
        else if (numMisses == 1)
            return 2;
        else if (numMisses == 2)
            return 1;
        else
            return 0;
    }

    private IEnumerator LoseRoutine()
    {
        print("you lose!");

        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    public void ReturnCoinsToFrame()
    {
        int count = 0;
        foreach (var coin in currentCoins)
        {
            if (coin != currentCoin)
            {
                coin.GetComponent<LerpableObject>().LerpPosition(InvisibleFrameLayout.instance.frames[count].transform.position, 0.25f, false);
            }
            count++;
        }
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);

        yield return new WaitForSeconds(0.8f);
        coin.LerpScale(Vector2.one, 0.25f);

        playingCoinAudio = false;
    }
}
