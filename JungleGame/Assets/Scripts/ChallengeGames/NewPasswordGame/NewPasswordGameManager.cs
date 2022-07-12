using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using TMPro;

public class NewPasswordGameManager : MonoBehaviour
{
    public static NewPasswordGameManager instance;

    public bool showChallengeWordLetters;

    [Header("Game Parts")]
    public List<UniversalCoinImage> coins;
    public Transform coinParent;
    public Transform polaroidParent;
    public Polaroid polaroid;
    public Transform tigerCharacter;
    public Transform marcusCharacter;
    public Transform brutusCharacter;

    [Header("Coin Positions")]
    public Transform coinOffScreenPos;
    public List<Transform> coinOnScreenPositions;
    public List<Transform> coinDownPositions;
    public List<Transform> coinTubePositions;

    [Header("Polaroid Positions")]
    public Transform polaroidOffScreenTigerPos;
    public Transform polaroidOffScreenPlayerPos;
    public Transform polaroidOnScreenPos;

    [Header("Character Positions")]
    public Transform tigerOnScreenPos;
    public Transform tigerOffScreenPos;
    public Transform marcusOnScreenPos;
    public Transform marcusOffScreenPos;
    public Transform brutusOnScreenPos;
    public Transform brutusOffScreenPos;

    private ChallengeWord currentWord;
    private List<ChallengeWord> prevWords;
    private int numMisses = 0;


    [Header("Tutorial")]
    public bool playTutorial;
    private bool playIntro = false;
    private int tutorialEvent = 0;
    public ChallengeWord tutorialPolaroid1;
    public ChallengeWord tutorialPolaroid2;
    public ChallengeWord tutorialPolaroid3;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().passwordTutorial;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;
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
        // reset win cards
        WinCardsController.instance.ResetCards();

        // hide lock
        PasswordLock.instance.HideLock();

        // init empty list
        prevWords = new List<ChallengeWord>();

        // place charcters off screen
        tigerCharacter.position = tigerOffScreenPos.position;
        marcusCharacter.position = marcusOffScreenPos.position;
        brutusCharacter.position = brutusOffScreenPos.position;

        // make coins empty gold
        foreach (var coin in coins)
            coin.SetValue(ElkoninValue.empty_gold);

        // start split song
        if (!playTutorial)
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.challengeGameSongSplit2);

        // play init audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PasswordInitRound, 0.5f);

        StartCoroutine(NewRound(true));
    }

    private IEnumerator NewRound(bool moveBG)
    {
        // place coins off-screen
        foreach (var coin in coins)
            coin.transform.position = coinOffScreenPos.position;

        // place polaroid off-screen
        polaroid.transform.position = polaroidOffScreenTigerPos.position;

        // select challenge word + reset polaroid
        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentWord = tutorialPolaroid1;
                    break;

                case 1:
                    currentWord = tutorialPolaroid2;
                    break;

                case 2:
                    currentWord = tutorialPolaroid3;
                    break;
            }
            tutorialEvent++;
        }
        else if (GameManager.instance.practiceModeON)
        {
            List<ChallengeWord> ChallengeWordList = new List<ChallengeWord>();
            ChallengeWordList = AISystem.ChallengeWordPassword(prevWords, GameManager.instance.practicePhonemes, GameManager.instance.practiceDifficulty);
            currentWord = ChallengeWordList[0];
            prevWords.Add(currentWord);
        }
        else
        {
            List<ChallengeWord> ChallengeWordList = new List<ChallengeWord>();
            ChallengeWordList = AISystem.ChallengeWordPassword(prevWords);
            currentWord = ChallengeWordList[0];
            prevWords.Add(currentWord);
        }

        PasswordTube.instance.TurnTube();
        polaroid.SetPolaroid(currentWord);
        polaroid.SetPolaroidAlpha(0f, 0f);
        yield return new WaitForSeconds(.5f);

        // move to new section
        if (moveBG)
        {
            PasswordLock.instance.ResetLock();
            BGManager.instance.MoveToNextSection();
            yield return new WaitForSeconds(1.5f);
            PasswordTube.instance.StopTube();

            // play walking animations
            float moveTime = 2f;
            tigerCharacter.GetComponent<Animator>().Play("tigerWalk");
            marcusCharacter.GetComponent<Animator>().Play("marcusWalkIn");
            brutusCharacter.GetComponent<Animator>().Play("brutusWalkIn");

            // move characters on screen
            tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOnScreenPos, moveTime, false);
            marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOnScreenPos, moveTime, false);
            brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOnScreenPos, moveTime, false);
            yield return new WaitForSeconds(moveTime);
        }
        else
        {
            yield return new WaitForSeconds(0.7f);
            PasswordTube.instance.StopTube();
        }

        // play idle animations + show lock
        tigerCharacter.GetComponent<Animator>().Play("aTigerIdle");
        marcusCharacter.GetComponent<Animator>().Play("marcusBroken");
        brutusCharacter.GetComponent<Animator>().Play("brutusBroken");
        PasswordLock.instance.ShowLock();
        // yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 1
                AssetReference clip = GameIntroDatabase.instance.passwordIntro1;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 0.5f);

                // play tutorial intro 2
                clip = GameIntroDatabase.instance.passwordIntro2;

                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd2.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                yield return new WaitForSeconds(cd2.GetResult() + 0.5f);

                // play tutorial intro 3
                clip = GameIntroDatabase.instance.passwordIntro3;

                CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd3.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                yield return new WaitForSeconds(cd3.GetResult() + 0.5f);

                // play tutorial intro 4
                clip = GameIntroDatabase.instance.passwordIntro4;

                CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd4.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                yield return new WaitForSeconds(cd4.GetResult() + 0.5f);

                // play tutorial intro 5 + 6
                List<AssetReference> clips = new List<AssetReference>();
                clips.Add(GameIntroDatabase.instance.passwordIntro5);
                clips.Add(GameIntroDatabase.instance.passwordIntro6);

                CoroutineWithData<float> cd5 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
                yield return cd5.coroutine;

                CoroutineWithData<float> cd6 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
                yield return cd6.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clips);
                yield return new WaitForSeconds(cd5.GetResult() + cd6.GetResult());
            }
        }
        else if (!GameManager.instance.practiceModeON)
        {
            if (!playIntro)
            {
                // play start 1
                AssetReference clip = GameIntroDatabase.instance.passwordStart1;
                if (GameManager.DeterminePlayPopup())
                {
                    CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd1.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                    yield return new WaitForSeconds(cd1.GetResult() + 0.5f);

                    // play start 2
                    clip = GameIntroDatabase.instance.passwordStart2;

                    CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd2.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                    yield return new WaitForSeconds(cd2.GetResult() + 0.5f);
                }
                
                // play start 3
                clip = GameIntroDatabase.instance.passwordStart3;

                CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd3.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd3.GetResult());
            }
        }

        // add settings button if not playing tutorial
        if (!playIntro)
        {
            playIntro = true;
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        // show polaroid
        polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOnScreenPos, 0.5f, false);
        // play audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
        yield return new WaitForSeconds(0.2f);
        polaroid.SetPolaroidAlpha(1f, 0.2f);
        yield return new WaitForSeconds(0.5f);

        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);

        // Get audio length
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(polaroid.challengeWord.audio));
        yield return cd.coroutine;

        // say polaroid word
        AudioManager.instance.PlayTalk(polaroid.challengeWord.audio);
        yield return new WaitForSeconds(cd.GetResult() + 0.1f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // show coins
        int i = 0;
        foreach (var coin in coins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 0.8f + (0.1f * i));
            yield return new WaitForSeconds(0.1f);
            i++;
        }

        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 7
            AssetReference clip = GameIntroDatabase.instance.passwordIntro7;

            CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd1.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd1.GetResult() + 0.5f);

            // play tutorial intro 8 + 9
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.passwordIntro8);
            clips.Add(GameIntroDatabase.instance.passwordIntro9);

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd2.coroutine;

            CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd3.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clips);
            yield return new WaitForSeconds(cd2.GetResult() + cd3.GetResult() + 0.5f);

            // play tutorial intro 10
            clip = GameIntroDatabase.instance.passwordIntro10;

            CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd4.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd4.GetResult() + 0.5f);
        }
        else
        {
            // // play new photo popup
            // int index = Random.Range(0, GameIntroDatabase.instance.passwordNewPhoto.Count);
            // AssetReference clip = GameIntroDatabase.instance.passwordNewPhoto[index];

            // CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            // yield return cd1.coroutine;


            // switch (index)
            // {
            //     case 0:
            //     case 1:
            //     case 2:
            //         TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
            //         yield return new WaitForSeconds(cd1.GetResult() + 0.5f);
            //         break;
            //     case 3:
            //         TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
            //         yield return new WaitForSeconds(cd1.GetResult() + 0.5f);
            //         break;
            // }
        }

        // turn on raycaster
        NewPasswordRaycaster.instance.isOn = true;
    }

    public void ResetCoins()
    {
        int i = 0;
        foreach (var coin in coins)
        {
            if (!PasswordTube.instance.tubeCoins.Contains(coin))
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
                i++;
            }
        }
    }

    public void EvaluateCoins()
    {
        // turn off raycaster
        NewPasswordRaycaster.instance.isOn = false;
        StartCoroutine(EvaluateCoinsRoutine());
    }

    private IEnumerator EvaluateCoinsRoutine()
    {
        // play right audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);

        // small delay
        // yield return new WaitForSeconds(1f);

        bool success = (currentWord.elkoninCount == PasswordTube.instance.tubeCoins.Count);
        // only track challenge round attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            int difficultyLevel = 1 + Mathf.FloorToInt(StudentInfoSystem.GetCurrentProfile().starsPass / 3);
            StudentInfoSystem.SavePlayerChallengeRoundAttempt(GameType.Password, success, currentWord, difficultyLevel);
        }

        if (success)
        {
            // play right audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            //yield return new WaitForSeconds(1f);

            // turn empty coins into polaroid coins
            PasswordTube.instance.ShowPolaroidCoins(currentWord, coins, true);
            while (PasswordTube.instance.playingAnimation)
                yield return null;

            if (showChallengeWordLetters)
            {
                LerpableObject pol = polaroid.GetComponent<LerpableObject>();

                // squish on x scale
                pol.LerpScale(new Vector2(0f, 1f), 0.2f);
                yield return new WaitForSeconds(0.2f);
                // play audio fx 
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 1f);
                // remove image
                polaroid.ShowPolaroidWord(60f);
                // un-squish on x scale
                pol.LerpScale(new Vector2(1f, 1f), 0.2f);
                yield return new WaitForSeconds(1f);

                // say letter groups using coins
                for (int j = 0; j < polaroid.challengeWord.elkoninCount; j++)
                {
                    GameObject letterElement = polaroid.GetLetterGroupElement(j);
                    letterElement.GetComponent<TextMeshProUGUI>().color = Color.black;
                    letterElement.GetComponent<LerpableObject>().LerpTextSize(70f - (Polaroid.FONT_SCALE_DECREASE * polaroid.challengeWord.elkoninCount), 0.2f);
                    PasswordTube.instance.tubeCoins[j].LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
                    // audio fx
                    AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(PasswordTube.instance.tubeCoins[j].value).audio);
                    AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * j));
                    yield return new WaitForSeconds(1f);
                    PasswordTube.instance.tubeCoins[j].LerpScale(new Vector2(1f, 1f), 0.25f);
                }
                yield return new WaitForSeconds(0.2f);

                // read word aloud to player
                if (currentWord.audio != null)
                    AudioManager.instance.PlayTalk(currentWord.audio);
                // start wiggle
                polaroid.ToggleWiggle(true);

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentWord.audio));
                yield return cd.coroutine;

                // wait an appropriate amount of time
                if (currentWord.audio != null)
                    yield return new WaitForSeconds(cd.GetResult() + 0.25f);
                else
                    yield return new WaitForSeconds(2f);

                // end wiggle
                polaroid.ToggleWiggle(false);
            }

            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PasswordNewRound, 0.5f);
            AudioManager.instance.IncreaseSplitSong();

            // move polaroid to player off-screen pos
            polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOffScreenPlayerPos, 0.2f, false);
            yield return new WaitForSeconds(0.5f);

            // unlock lock
            PasswordLock.instance.Unlock();

            // add card to win cards
            WinCardsController.instance.AddPolaroid();

            // coin animations
            PasswordTube.instance.CorrectCoinsAnimation();

            // yield return new WaitForSeconds(1f);

            // remove extra coins
            // create list of non tube coins
            List<UniversalCoinImage> extraCoins = new List<UniversalCoinImage>();
            extraCoins.AddRange(coins);

            // remove tube coins
            foreach (var tubeCoin in PasswordTube.instance.tubeCoins)
            {
                extraCoins.Remove(tubeCoin);
            }
            RemoveExtraCoins(extraCoins);

            // determine if win
            if (WinCardsController.instance.WinGame())
            {
                StartCoroutine(WinRoutine());
                yield break;
            }
            else
            {
                // play appropriate popup
                if (playTutorial && tutorialEvent == 1)
                {
                    // play tutorial intro 11
                    AssetReference clip = GameIntroDatabase.instance.passwordIntro11;

                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(cd.GetResult() + 1f);

                    // play tutorial intro 12
                    clip = GameIntroDatabase.instance.passwordIntro12;

                    CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd2.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                    yield return new WaitForSeconds(cd2.GetResult() + 1f);

                    StartCoroutine(WinRoutine());
                    yield break;
                }
                else
                {
                    if (GameManager.DeterminePlayPopup())
                    {
                        AssetReference clip = null;
                        int index = Random.Range(0, 5);
                        switch (index)
                        {
                            case 0:
                                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ugh");

                                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                                yield return cd.coroutine;

                                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                                //yield return new WaitForSeconds(cd.GetResult() + 1f);
                                break;

                            case 1:
                                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_grr");

                                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                                yield return cd2.coroutine;

                                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                                //yield return new WaitForSeconds(cd2.GetResult() + 1f);
                                break;

                            case 2:
                                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_argh");

                                CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                                yield return cd3.coroutine;

                                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                                //yield return new WaitForSeconds(cd3.GetResult() + 1f);
                                break;

                            case 3:
                                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("brutus_heh");

                                CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                                yield return cd4.coroutine;

                                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                                //yield return new WaitForSeconds(cd4.GetResult() + 1f);
                                break;

                            case 4:
                                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_grr");

                                CoroutineWithData<float> cd5 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                                yield return cd5.coroutine;

                                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                                ///yield return new WaitForSeconds(cd5.GetResult() + 1f);
                                break;
                        }
                    }
                }

                // tiger run away
                tigerCharacter.GetComponent<Animator>().Play("aTigerTurn");
                yield return new WaitForSeconds(0.25f);
                tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOffScreenPos, 1f, false);
                yield return new WaitForSeconds(1f);

                // monkies walk way
                marcusCharacter.GetComponent<Animator>().Play("marcusTurn");
                brutusCharacter.GetComponent<Animator>().Play("brutusTurn");
                yield return new WaitForSeconds(0.25f);
                marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOffScreenPos, 2f, false);
                brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOffScreenPos, 2f, false);
                yield return new WaitForSeconds(1f);

                // reset current polaroid
                polaroid.HidePolaroidWord();

                // remove lock
                PasswordLock.instance.HideLock();
            }
        }
        else
        {
            // only count misses if not playing tutorial
            if (!playTutorial)
                numMisses++;

            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            if (playTutorial)
            {
                // show polaroid
                polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOnScreenPos, 0.5f, false);
                yield return new WaitForSeconds(0.2f);
                polaroid.SetPolaroidAlpha(1f, 0.2f);
                yield return new WaitForSeconds(0.5f);

                // turn on raycaster
                NewPasswordRaycaster.instance.isOn = true;
                yield break;
            }

            yield return new WaitForSeconds(0.5f);

            // turn empty coins into polaroid coins
            PasswordTube.instance.ShowPolaroidCoins(currentWord, coins, false);
            while (PasswordTube.instance.playingAnimation)
                yield return null;

            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PasswordWrongRound, 0.5f);

            yield return new WaitForSeconds(0.5f);

            // move polaroid to tiger off-screen pos
            polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOffScreenTigerPos, 0.2f, false);
            //yield return new WaitForSeconds(0.5f);

            PasswordLock.instance.UpgradeLock();

            // coin animation reset
            PasswordTube.instance.RemoveAllCoins();

            //yield return new WaitForSeconds(0.5f);

            // remove extra coins
            // create list of non tube coins
            List<UniversalCoinImage> extraCoins = new List<UniversalCoinImage>();
            extraCoins.AddRange(coins);

            // remove tube coins
            foreach (var tubeCoin in PasswordTube.instance.tubeCoins)
            {
                extraCoins.Remove(tubeCoin);
            }
            RemoveExtraCoins(extraCoins);

            // determine if lose
            if (PasswordLock.instance.LoseGame())
            {
                StartCoroutine(LoseRoutine());
                yield break;
            }
            else
            {
                // play appropriate popup
                AssetReference clip = null;
                int index = Random.Range(0, 4);
                switch (index)
                {
                    case 0:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_haha");

                        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd.coroutine;

                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                        //yield return new WaitForSeconds(cd.GetResult() + 1f);
                        break;

                    case 1:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ahhah");

                        CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd2.coroutine;

                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                        //yield return new WaitForSeconds(cd2.GetResult() + 1f);
                        break;

                    case 2:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_laugh");

                        CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd3.coroutine;

                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                        //yield return new WaitForSeconds(cd3.GetResult() + 1f);
                        break;

                    case 3:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("brutus_laugh");

                        CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd4.coroutine;

                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                        //yield return new WaitForSeconds(cd4.GetResult() + 1f);
                        break;
                }
            }
        }

        // reset current polaroid
        polaroid.HidePolaroidWord();

        // begin next round
        StartCoroutine(NewRound(success));
    }

    private void RemoveExtraCoins(List<UniversalCoinImage> extraCoins)
    {
        int i = 0;
        foreach (var coin in extraCoins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinDownPositions[i], 0.2f, false);
            i++;
        }
    }

    public void SayPolaroidWord()
    {
        // return if there is no word
        if (currentWord == null)
            return;

        StartCoroutine(SayPolaroidWordRoutine());
    }

    private IEnumerator SayPolaroidWordRoutine()
    {
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);

        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentWord.audio));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(currentWord.audio);
        yield return new WaitForSeconds(0.5f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
    }

    private IEnumerator WinRoutine()
    {
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("NewPasswordGame", true, 3f);
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
        playerData.passPlayed = playerData.passPlayed + 1;
        playerData.starsPass = CalculateStars() + playerData.starsPass;

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
        yield return new WaitForSeconds(2f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}
