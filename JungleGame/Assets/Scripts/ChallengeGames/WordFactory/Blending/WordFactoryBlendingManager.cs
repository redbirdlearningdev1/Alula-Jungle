using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;

public class WordFactoryBlendingManager : MonoBehaviour
{
    public static WordFactoryBlendingManager instance;

    public bool showChallengeWordLetters;

    [Header("Game Objects")]
    [SerializeField] private HorizontalLayoutGroup frameGroup;
    [SerializeField] private Transform[] frames; // six in-game frames
    [SerializeField] private float[] frameSpacing;
    [SerializeField] private Transform coinParent;
    [SerializeField] private GameObject universalCoin;

    [Header("Characters")]
    [SerializeField] private Animator redAnimator;
    [SerializeField] private Animator tigerAnimator;

    [Header("Polaroids")]
    [SerializeField] private Polaroid[] polaroids; // three in-game polaroids
    [SerializeField] private Transform polaroidParent;
    public Color normalGlow;

    [Header("Cards")]
    [SerializeField] private GameObject[] redCards;
    [SerializeField] private GameObject[] tigerCards;
    private int redCardCount = 0;
    private int tigerCardCount = 0;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform polaroid0Pos;
    [SerializeField] private Transform polaroid2Pos;
    [SerializeField] private Transform redPilePos;
    [SerializeField] private Transform tigerPilePos;
    [SerializeField] private Transform away0Pos;
    [SerializeField] private Transform away2Pos;
    [SerializeField] private Transform centerPos;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;

    // other variables
    private ChallengeWord currentWord;
    private List<ChallengeWord> currentWords;
    private List<ChallengeWord> prevWords;
    private Polaroid currentPolaroid;
    private List<UniversalCoinImage> currentCoins;
    private bool playingCoinAudio = false;
    private int numWins = 0;
    private int numMisses = 0;

    [Header("Tutorial")]
    public bool playTutorial;
    private bool playIntro = false;
    public int[] correctTutorialIndex;
    private int tutorialEvent = 0;
    public List<ChallengeWord> polaroids1;
    public List<ChallengeWord> polaroids2;
    public List<ChallengeWord> polaroids3;
    [Header("Scripted")]
    private int scriptedEvent = 0;
    public List<ChallengeWord> polaroidsScripted1;
    public List<ChallengeWord> polaroidsScripted2;
    public List<ChallengeWord> polaroidsScripted3;
    public List<ChallengeWord> polaroidsScripted4;
    public List<ChallengeWord> polaroidsScripted5;



    [Header("Testing")]
    public bool overridePool;
    public List<ChallengeWord> testChallengeWords;
    public int correctIndex;



    private void Start()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
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

    private void PregameSetup()
    {
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial;

        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // init empty list
        prevWords = new List<ChallengeWord>();

        // disable card objects
        foreach (var card in redCards)
        {
            card.SetActive(false);
        }
        foreach (var card in tigerCards)
        {
            card.SetActive(false);
        }

        // begin first round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = false;

        // place polaroids in start pos + reset alphas
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
            pol.SetPolaroidAlpha(1f, 0f);
        }

        // set frames to be invisible
        frameGroup.spacing = frameSpacing[0];
        foreach (var frame in frames)
        {
            frame.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        // initialize list
        currentWords = new List<ChallengeWord>();
        currentWords.Clear();

        // tutorial stuff
        if (playTutorial)
        {
            // short pause before start
            yield return new WaitForSeconds(1f);

            if (tutorialEvent == 0)
            {
                // play tutorial intro 1
                AssetReference clip = GameIntroDatabase.instance.blendingIntro1;

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);

                // play tutorial intro 2
                clip = GameIntroDatabase.instance.blendingIntro2;

                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd2.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd2.GetResult() + 1f);
            }
            else
            {
                // play tutorial intro 3
                AssetReference clip = GameIntroDatabase.instance.blendingIntro3;

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
        }
        else if (!GameManager.instance.practiceModeON)
        {
            if (!playIntro)
            {
                // short pause before start
                yield return new WaitForSeconds(1f);

                // play start 1
                AssetReference clip = GameIntroDatabase.instance.blendingStart1;

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult() + 0.5f);

                // play start 2
                clip = GameIntroDatabase.instance.blendingStart2;

                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd2.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd2.GetResult());
            }
        }

        // add menu button
        if (!playIntro)
        {
            playIntro = true;
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        // get challenge words
        if (playTutorial)
        {
            // get correct tutorial polaroids
            List<ChallengeWord> tutorialList = new List<ChallengeWord>();
            switch (tutorialEvent)
            {
                case 0:
                    tutorialList.AddRange(polaroids1);
                    currentWords.AddRange(polaroids1);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 1:
                    tutorialList.AddRange(polaroids2);
                    currentWords.AddRange(polaroids2);
                    correctIndex = correctTutorialIndex[1];
                    break;
                case 2:
                    tutorialList.AddRange(polaroids3);
                    currentWords.AddRange(polaroids3);
                    correctIndex = correctTutorialIndex[2];
                    break;
            }
            tutorialEvent++;

            // set tutorial polaroids
            for (int i = 0; i < 3; i++)
            {
                polaroids[i].SetPolaroid(tutorialList[i]);
            }

            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }
        else if (overridePool)
        {
            currentWord = testChallengeWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];

            // use override words
            int count = 0;
            foreach (var pol in polaroids)
            {
                pol.SetPolaroid(testChallengeWords[count]);
                count++;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().blendPlayed == 0)
        {
            // get correct tutorial polaroids
            List<ChallengeWord> scriptedList = new List<ChallengeWord>();
            switch (scriptedEvent)
            {
                case 0:
                    scriptedList.AddRange(polaroidsScripted1);
                    currentWords.AddRange(polaroidsScripted1);
                    correctIndex = correctTutorialIndex[1];
                    break;
                case 1:
                    scriptedList.AddRange(polaroidsScripted2);
                    currentWords.AddRange(polaroidsScripted2);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 2:
                    scriptedList.AddRange(polaroidsScripted3);
                    currentWords.AddRange(polaroidsScripted3);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 3:
                    scriptedList.AddRange(polaroidsScripted4);
                    currentWords.AddRange(polaroidsScripted4);
                    correctIndex = correctTutorialIndex[2];
                    break;
                case 4:
                    scriptedList.AddRange(polaroidsScripted5);
                    currentWords.AddRange(polaroidsScripted5);
                    correctIndex = correctTutorialIndex[1];
                    break;
            }
            scriptedEvent++;

            // set tutorial polaroids
            for (int i = 0; i < 3; i++)
            {
                polaroids[i].SetPolaroid(scriptedList[i]);
            }

            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }
        else if (GameManager.instance.practiceModeON)
        {
            // use AI word selection
            List<ChallengeWord> tempChallengeWordList = new List<ChallengeWord>();
            tempChallengeWordList = AISystem.ChallengeWordSelectionBlending(prevWords, GameManager.instance.practiceDifficulty, GameManager.instance.practicePhonemes);
            ChallengeWord correctWord = tempChallengeWordList[0];
            ChallengeWord incorrectWord1 = tempChallengeWordList[1];
            ChallengeWord incorrectWord2 = tempChallengeWordList[2];

            // set prev words
            prevWords.Add(correctWord);
            prevWords.Add(incorrectWord1);
            prevWords.Add(incorrectWord2);

            foreach (var polaroid in polaroids)
            {
                int randIndex = Random.Range(0, tempChallengeWordList.Count);
                polaroid.SetPolaroid(tempChallengeWordList[randIndex]);
                currentWords.Add(tempChallengeWordList[randIndex]);
                tempChallengeWordList.RemoveAt(randIndex);
            }
            currentWord = correctWord;
            currentPolaroid = polaroids[correctIndex];
        }
        else
        {
            // use AI word selection
            List<ChallengeWord> tempChallengeWordList = new List<ChallengeWord>();
            tempChallengeWordList = AISystem.ChallengeWordSelectionBlending(prevWords);
            ChallengeWord correctWord = tempChallengeWordList[0];
            ChallengeWord incorrectWord1 = tempChallengeWordList[1];
            ChallengeWord incorrectWord2 = tempChallengeWordList[2];

            // set prev words
            prevWords.Add(correctWord);
            prevWords.Add(incorrectWord1);
            prevWords.Add(incorrectWord2);

            foreach (var polaroid in polaroids)
            {
                int randIndex = Random.Range(0, tempChallengeWordList.Count);
                polaroid.SetPolaroid(tempChallengeWordList[randIndex]);
                currentWords.Add(tempChallengeWordList[randIndex]);
                tempChallengeWordList.RemoveAt(randIndex);
            }
            currentWord = correctWord;
            currentPolaroid = polaroids[correctIndex];
        }

        // show polaroids
        StartCoroutine(ShowPolaroids());
        yield return new WaitForSeconds(3f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 5
            AssetReference clip = GameIntroDatabase.instance.blendingIntro5;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            // play tutorial intro 6
            clip = GameIntroDatabase.instance.blendingIntro6;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd2.GetResult() + 1f);

            // play tutorial intro 7
            clip = GameIntroDatabase.instance.blendingIntro7;

            CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd3.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd3.GetResult() + 1f);
        }

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            frames[5 - i].gameObject.SetActive(false);
        }

        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[currentWord.elkoninCount - 1], 0f));

        // show frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(frames[i].GetComponent<Image>(), 1f, 0.25f));
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);

        currentCoins = new List<UniversalCoinImage>();
        yield return new WaitForSeconds(0.5f);

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, frames[i].position, Quaternion.identity, coinParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.transform.localScale = new Vector3(0f, 0f, 1f);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            currentCoins.Add(coin);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play audio in frame order
            for (int i = 0; i < currentWord.elkoninCount; i++)
            {
                StartCoroutine(PlayAudioCoinRoutine(currentCoins[i]));
                yield return new WaitForSeconds(0.8f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(SelectedPolaroid(currentPolaroid));

            yield return new WaitForSeconds(0.5f);

            // play tutorial intro 8
            AssetReference clip = GameIntroDatabase.instance.blendingIntro8;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            TogglePolaroidsWiggle(true);

            // remove raycast blocker
            RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
            WordFactoryRaycaster.instance.isOn = true;
        }
        else
        {
            StartCoroutine(StartRound());
        }
    }

    public void TogglePolaroidsWiggle(bool opt)
    {
        // wiggle polaroids
        foreach (Polaroid polaroid in polaroids)
        {
            polaroid.ToggleWiggle(opt);
        }
    }

    private IEnumerator StartRound()
    {
        // play audio in frame order
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(PlayAudioCoinRoutine(currentCoins[i]));
            yield return new WaitForSeconds(0.8f);
        }

        TogglePolaroidsWiggle(true);

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = true;
    }

    public bool EvaluatePolaroid(Polaroid polaroid)
    {
        WordFactoryRaycaster.instance.isOn = false;

        if (!playTutorial)
            StartCoroutine(SelectedPolaroid(polaroid));

        bool success = (polaroid.challengeWord == currentWord);
        // only track challenge round attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            int difficultyLevel = 1 + Mathf.FloorToInt(StudentInfoSystem.GetCurrentProfile().starsBlend / 3);
            StudentInfoSystem.SavePlayerChallengeRoundAttempt(GameType.WordFactoryBlending, success, currentWord, difficultyLevel);
        }

        if (success)
        {
            numWins++;
            currentPolaroid = polaroid;
            StartCoroutine(CorrectPolaroidRoutine());
            return true;
        }
        // wrong answer
        numMisses++;
        StartCoroutine(FailPolaroidRoutine());
        return false;
    }

    private IEnumerator SelectedPolaroid(Polaroid polaroid)
    {
        yield return new WaitForSeconds(0.5f);

        // select polaroid
        foreach (Polaroid pol in polaroids)
        {
            if (pol.challengeWord == polaroid.challengeWord)
            {
                pol.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.3f);
            }
            else
            {
                pol.GetComponent<LerpableObject>().LerpScale(new Vector2(0.6f, 0.6f), 0.3f);
            }
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator CorrectPolaroidRoutine()
    {
        yield return new WaitForSeconds(2f);

        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine(true));

        // show challenge word letters
        if (showChallengeWordLetters)
        {
            // hide coins + frames
            yield return new WaitForSeconds(2f);

            // animate cards away - except for current polaroid
            int count = 0;
            foreach (var pol in polaroids)
            {
                // skip current polaroid
                if (pol == currentPolaroid)
                {
                    count++;
                    continue;
                }
                    
                
                if (count == 0)
                {
                    pol.MovePolaroid(away0Pos.position, 0.25f);
                }
                else if (count == 1)
                {
                    pol.MovePolaroid(polaroidStartPos.position, 0.25f);
                }
                else if (count == 2)
                {
                    pol.MovePolaroid(away2Pos.position, 0.25f);
                }
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
                yield return new WaitForSeconds(0.1f);
                count++;
            }

            yield return new WaitForSeconds(0.5f);

            // center current polaroid on screen
            LerpableObject polaroid = currentPolaroid.GetComponent<LerpableObject>();
            polaroid.LerpPosToTransform(centerPos, 0.5f, false);
            polaroid.LerpScale(new Vector2(1.8f, 1.8f), 0.5f);
            polaroid.LerpRotation(360f, 0.5f);
            // play audio fx 
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidFall, 0.5f);

            yield return new WaitForSeconds(1f);

            // squish on x scale
            polaroid.LerpScale(new Vector2(0f, 1.8f), 0.2f);
            yield return new WaitForSeconds(0.2f);
            // play audio fx 
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 1f);
            // remove image
            currentPolaroid.ShowPolaroidWord(60f);
            // un-squish on x scale
            polaroid.LerpScale(new Vector2(1.8f, 1.8f), 0.2f);
            yield return new WaitForSeconds(1f);

            // say letter groups using coins
            for (int i = 0; i < currentPolaroid.challengeWord.elkoninCount; i++)
            {
                GameObject letterElement = currentPolaroid.GetLetterGroupElement(i);
                letterElement.GetComponent<TextMeshProUGUI>().color = Color.black;
                letterElement.GetComponent<LerpableObject>().LerpTextSize(70f - (Polaroid.FONT_SCALE_DECREASE * currentPolaroid.challengeWord.elkoninCount), 0.2f);
                StartCoroutine(PlayAudioCoinRoutine(currentCoins[i]));
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.2f);

            // read word aloud to player
            if (currentWord.audio != null)
                AudioManager.instance.PlayTalk(currentWord.audio);
            // start wiggle
            currentPolaroid.ToggleWiggle(true);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentWord.audio));
            yield return cd.coroutine;

            // wait an appropriate amount of time
            if (currentWord.audio != null)
                yield return new WaitForSeconds(cd.GetResult() + 0.25f);
            else
                yield return new WaitForSeconds(2f);

            // end wiggle
            currentPolaroid.ToggleWiggle(false);

            yield return new WaitForSeconds(0.2f);

            polaroid.LerpPosToTransform(polaroidStartPos, 0.25f, false);
            StartCoroutine(HideCoinsAndFrames());
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        }
        // don't show challenge word and just animate cards away
        else
        {
            // hide coins + frames
            yield return new WaitForSeconds(1f);
            StartCoroutine(HideCoinsAndFrames());

            // animate cards away
            int count = 0;
            foreach (var polaroid in polaroids)
            {
                if (count == 0)
                {
                    polaroid.MovePolaroid(away0Pos.position, 0.25f);
                }
                else if (count == 1)
                {
                    polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
                }
                else if (count == 2)
                {
                    polaroid.MovePolaroid(away2Pos.position, 0.25f);
                }
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
                yield return new WaitForSeconds(0.1f);
                count++;
            }
        }

        // ####################

        

        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award red new card
        redCards[redCardCount].SetActive(true);
        // animate card init
        switch (redCardCount)
        {
            case 0:
                redCards[redCardCount].GetComponent<Animator>().Play("Card1");
                break;
            case 1:
                redCards[redCardCount].GetComponent<Animator>().Play("Card2");
                break;
            case 2:
                redCards[redCardCount].GetComponent<Animator>().Play("Card3");
                break;
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 0.5f);
        yield return new WaitForSeconds(0.1f);
        redCardCount++;

        // animate characters
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

        yield return new WaitForSeconds(1f);

        // reset current polaroid
        currentPolaroid.HidePolaroidWord();

        // win game iff 3 or more rounds have been won
        if (numWins >= 3)
        {
            StartCoroutine(WinGameRoutine());
            yield break;
        }

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 9
            AssetReference clip = GameIntroDatabase.instance.blendingIntro9;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            // play tutorial intro 10
            clip = GameIntroDatabase.instance.blendingIntro10;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd2.GetResult() + 1f);

            // play tutorial intro 11
            clip = GameIntroDatabase.instance.blendingIntro11;

            CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd3.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd3.GetResult() + 1f);

            // end tutorial
            StartCoroutine(WinGameRoutine());
            yield break;
        }
        else
        {
            if (GameManager.DeterminePlayPopup())
            {
                // play encouragement popup
                // julius
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

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult() + 0.5f);

                // red
                index = Random.Range(0, 3);
                clip = null;
                if (index == 0)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_woohoo");
                }
                else if (index == 1)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_hurrah");
                }
                else if (index == 2)
                {
                    clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_uhhuh");
                }

                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd2.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd2.GetResult());
            }
        }

        // go to next round
        StartCoroutine(NewRound());
    }

    private IEnumerator FailPolaroidRoutine()
    {
        if (playTutorial)
        {
            WordFactoryRaycaster.instance.isOn = true;
            yield break;
        }

        yield return new WaitForSeconds(2f);

        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine(false));

        // ####################

        // hide coins + frames
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideCoinsAndFrames());

        // animate cards away
        int count = 0;
        foreach (var polaroid in polaroids)
        {
            if (count == 0)
            {
                polaroid.MovePolaroid(away0Pos.position, 0.25f);
            }
            else if (count == 1)
            {
                polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
            }
            else if (count == 2)
            {
                polaroid.MovePolaroid(away2Pos.position, 0.25f);
            }
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award tiger new card
        tigerCards[tigerCardCount].SetActive(true);
        // animate card init
        switch (tigerCardCount)
        {
            case 0:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card1");
                break;
            case 1:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card2");
                break;
            case 2:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card3");
                break;
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
        yield return new WaitForSeconds(0.1f);
        tigerCardCount++;

        // animate characters
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");

        if (numMisses >= 3)
        {
            StartCoroutine(LoseGameRoutine());
            yield break;
        }

        yield return new WaitForSeconds(1f);

        // play reminder popup
        List<AssetReference> clips = GameIntroDatabase.instance.blendingReminderClips;
        AssetReference clip = clips[Random.Range(0, clips.Count)];

        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd.coroutine;

        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
        yield return new WaitForSeconds(cd.GetResult() + 1f);


        StartCoroutine(NewRound());
    }

    private IEnumerator HideCoinsAndFrames()
    {
        // remove coin glows
        foreach (var coin in currentCoins)
        {
            yield return new WaitForSeconds(0.05f);
        }

        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[0], 1f));

        // hide frames
        foreach (var frame in frames)
        {
            StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 0f, 0.25f));
        }

        // make coins invisible
        foreach (var coin in currentCoins)
        {
            coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);

        // deactivate all frames
        foreach (var frame in frames)
        {
            frame.gameObject.SetActive(true);
        }
    }

    private IEnumerator LoseGameRoutine()
    {
        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinGameRoutine()
    {
        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);


        if (playTutorial)
        {
            // short pause before start
            yield return new WaitForSeconds(1f);

            // play end 1
            AssetReference clip = GameIntroDatabase.instance.blendingEnd1;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            // play end 2
            clip = GameIntroDatabase.instance.blendingEnd2;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd2.GetResult() + 1f);

            StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("WordFactoryBlending", true, 3f);
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
        playerData.blendPlayed = playerData.blendPlayed + 1;
        playerData.starsBlend = CalculateStars() + playerData.starsBlend;

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

    private IEnumerator PolaroidRevealRoutine(bool isCorrect)
    {
        // make other polaroids transparent
        foreach (Polaroid polaroid in polaroids)
        {
            // re-do scales to show correct polaroid
            if (polaroid.challengeWord == currentWord)
            {
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
                currentPolaroid = polaroid;
            }
            else
            {
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(0.6f, 0.6f), 0.25f);
                polaroid.SetPolaroidAlpha(0.2f, 0.2f);
            }
        }

        if (!showChallengeWordLetters)
        {
            // read word aloud to player
            if (currentWord.audio != null)
                AudioManager.instance.PlayTalk(currentWord.audio);

            // glow coins fast
            foreach (var coin in currentCoins)
            {
                yield return new WaitForSeconds(0.05f);
            }

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentWord.audio));
            yield return cd.coroutine;

            // wait an appropriate amount of time
            if (currentWord.audio != null)
                yield return new WaitForSeconds(cd.GetResult() + 0.25f);
            else
                yield return new WaitForSeconds(2f);
        }

        // reveal correct polaroid 
        if (isCorrect)
        {
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
        }

        // shake polaroid
        currentPolaroid.ToggleWiggle(true);

        yield return new WaitForSeconds(1.6f);

        // shake polaroid
        currentPolaroid.ToggleWiggle(false);
    }

    public void ResetPolaroids(bool instant = false, bool startPos = false)
    {
        float timeLerp = 0.1f;
        if (instant)
            timeLerp = 0f;

        // reset polaroid scale
        polaroids[0].LerpScale(1f, timeLerp);
        polaroids[1].LerpScale(1f, timeLerp);
        polaroids[2].LerpScale(1f, timeLerp);

        // return to start position
        if (startPos)
        {
            polaroids[0].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroidStartPos.position, timeLerp);
        }
        // return to original position
        else
        {
            polaroids[0].MovePolaroid(polaroid0Pos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidLandPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroid2Pos.position, timeLerp);
        }


        // set appropriate parent
        polaroids[0].transform.SetParent(polaroidParent);
        polaroids[1].transform.SetParent(polaroidParent);
        polaroids[2].transform.SetParent(polaroidParent);

        // set appropriate rotation
        polaroids[0].GetComponent<LerpableObject>().LerpRotation(10f, 0f);
        polaroids[1].GetComponent<LerpableObject>().LerpRotation(0f, 0f);
        polaroids[2].GetComponent<LerpableObject>().LerpRotation(-10f, 0f);
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
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(0.8f);
        coin.LerpSize(normalCoinSize, 0.25f);

        playingCoinAudio = false;
    }

    private IEnumerator LerpImageAlpha(Image image, float targetAlpha, float totalTime)
    {
        float start = image.color.a;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                image.color = new Color(1f, 1f, 1f, targetAlpha);
                break;
            }

            float tempAlpha = Mathf.Lerp(start, targetAlpha, timer / totalTime);
            image.color = new Color(1f, 1f, 1f, tempAlpha);
            yield return null;
        }
    }

    private IEnumerator LerpFrameSpacing(float target, float totalTime)
    {
        float start = frameGroup.spacing;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                frameGroup.spacing = target;
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            frameGroup.spacing = temp;
            yield return null;
        }
    }

    private IEnumerator ShowPolaroids()
    {
        // hide polaroid images
        foreach (var polaroid in polaroids)
        {
            polaroid.HideImage(0f);
        }

        // short 0.5s delay
        yield return new WaitForSeconds(0.5f);

        Vector3 bouncePos = polaroidLandPos.position;
        bouncePos.y -= 0.5f;
        // move polaroids down  
        foreach (var polaroid in polaroids)
        {
            polaroid.MovePolaroid(bouncePos, 0.3f);
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidFall, 0.5f);
        yield return new WaitForSeconds(0.3f);
        // move polaroids down  
        foreach (var polaroid in polaroids)
        {
            polaroid.MovePolaroid(polaroidLandPos.position, 0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        // move each polaroid to their respective spot
        polaroids[0].MovePolaroid(polaroid0Pos.position, 0.5f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.15f);
        polaroids[2].MovePolaroid(polaroid2Pos.position, 0.5f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.8f);

        // reveal polaroid images
        foreach (var polaroid in polaroids)
        {
            polaroid.RevealImage(0.25f);
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CameraClick, 1f);
        // scale polaroids when revealing images
        foreach (var polaroid in polaroids)
        {
            polaroid.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        }
    }
}