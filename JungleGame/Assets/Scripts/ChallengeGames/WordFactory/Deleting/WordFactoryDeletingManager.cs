using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFactoryDeletingManager : MonoBehaviour
{
    public static WordFactoryDeletingManager instance;

    public Polaroid polaroid; // main polarid used in this game
    public GameObject universalCoinImage; // universal coin prefab
    public Transform coinsParent;
    public Vector2 normalCoinSize;

    private List<WordPair> pairPool;
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
    public WordPair tutorialPair2;
    public WordPair tutorialPair3;

    private bool firstTry;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
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
            }
        }
    }

    void Start()
    {
        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial;

        PregameSetup();
    }

    private void PregameSetup()
    {
        // get pair pool from game manager
        pairPool = new List<WordPair>();
        pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(StudentInfoSystem.GetCurrentProfile().actionWordPool));

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
                case 1:
                    currentPair = tutorialPair2;
                    break;
                case 2:
                    currentPair = tutorialPair3;
                    break;
            }
            tutorialEvent++;
        }
        else
        {
            // new pair
            //currentPair = pairPool[Random.Range(0, pairPool.Count)];
            currentPair = AISystem.ChallengeWordBuildingDeleting(StudentInfoSystem.GetCurrentProfile());
        }
        

        // init game delay
        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 1
                AudioClip clip = GameIntroDatabase.instance.deletingIntro1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
        }
        else
        {
            if (!playIntro)
            {
                // play start 1
                AudioClip clip = GameIntroDatabase.instance.deletingStart1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_4)
                {
                    // play start 2
                    clip = GameIntroDatabase.instance.deletingStart2Chapters1_3;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                }
                else
                {
                    // play start 2
                    clip = GameIntroDatabase.instance.deletingStart2Chapters4_5;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
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
        yield return new WaitForSeconds(1f);

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
        yield return new WaitForSeconds(3f);

        // throw out real frames
        VisibleFramesController.instance.PlaceActiveFrames(polaroid.transform.localPosition);
        VisibleFramesController.instance.MoveFramesToInvisibleFrames();
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);
        yield return new WaitForSeconds(1f);

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
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.5f);

        // say challenge word
        AudioManager.instance.PlayTalk(currentWord.audio);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        foreach (var coin in currentCoins)
        {
            coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
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
                AudioClip clip = GameIntroDatabase.instance.deletingIntro2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 3
                clip = GameIntroDatabase.instance.deletingIntro3;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
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
        yield return new WaitForSeconds(1.5f);
        // say new challenge word
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        AudioManager.instance.PlayTalk(currentPair.word1.audio);
        yield return new WaitForSeconds(currentPair.word1.audio.length);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // tutorial stuff
        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                yield return new WaitForSeconds(1f);

                // play tutorial intro 4
                AudioClip clip = GameIntroDatabase.instance.deletingIntro4;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
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

        // win
        if (coin.value == currentPair.word2.elkoninList[currentPair.index])
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
        yield return new WaitForSeconds(1.5f);

        // win round
        if (win)
        {
            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            // emerald tiger correct
            EmeraldTigerHolder.instance.SetCorrect(true);

            // remove coin from list
            currentCoins.Remove(currentCoin);

            yield return new WaitForSeconds(1f);

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
            yield return new WaitForSeconds(1f);

            x = 0;
            foreach (var coin in currentCoins)
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(VisibleFramesController.instance.frames[x].transform, 0f, false);
                x++;
            }
            yield return new WaitForSeconds(0.5f);

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
            yield return new WaitForSeconds(1f);

            // say new challenge word
            AudioManager.instance.PlayTalk(currentPair.word1.audio);
            polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
            foreach (var coin in currentCoins)
            {
                coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f);
            foreach (var coin in currentCoins)
            {
                coin.LerpScale(Vector2.one, 0.25f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.25f);
            polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

            // tutorial stuff
            if (playTutorial)
            {
                if (tutorialEvent == 1)
                {
                    yield return new WaitForSeconds(1f);

                    // play tutorial intro 5
                    AudioClip clip = GameIntroDatabase.instance.deletingIntro5;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
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
                yield return new WaitForSeconds(1f);

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
        yield return new WaitForSeconds(1.5f);

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
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
        }
        yield return new WaitForSeconds(1f);

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
                int index = Random.Range(0, 2);
                AudioClip clip = null;
                if (index == 0)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ugh");
                }
                else if (index == 1)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_grr");
                }
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
            else
            {
                int index = Random.Range(0, 3);
                AudioClip clip = null;
                if (index == 0)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_haha");
                }
                else if (index == 1)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ahhah");
                }
                else if (index == 2)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_hrm");
                }
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play reminder popup
                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    List<AudioClip> clips = GameIntroDatabase.instance.deletingReminderClipsChapters1_4;
                    clip = clips[Random.Range(0, clips.Count)];
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                }
                else
                {
                    List<AudioClip> clips = GameIntroDatabase.instance.deletingReminderClipsChapter5;
                    clip = clips[Random.Range(0, clips.Count)];
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                }
            }
        }

        StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        print ("you win!");

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("WordFactoryDeleting", true, 3f);
        }
        else
        {
            // AI stuff
            AIData(StudentInfoSystem.GetCurrentProfile());

            // calculate and show stars
            StarAwardController.instance.AwardStarsAndExit(CalculateStars());
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
        print ("you lose!");
        
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

        yield return new WaitForSeconds(0.9f);
        coin.LerpScale(Vector2.one, 0.25f);

        playingCoinAudio = false;
    }
}
