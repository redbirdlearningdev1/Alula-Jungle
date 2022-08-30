using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using TMPro;

public class TigerGameManager : MonoBehaviour
{
    public static TigerGameManager instance;

    public bool showChallengeWordLetters;
    private Polaroid currentPolaroid;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private List<Polaroid> polaroidC;
    [SerializeField] private TigerGameRaycaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;

    [Header("Positions")]
    [SerializeField] private Transform coinStartPos;
    [SerializeField] private Transform coinLandPos;
    [SerializeField] private Transform PhotoPos1;
    [SerializeField] private Transform PhotoPos2;
    [SerializeField] private Transform PhotoPos3;
    [SerializeField] private Transform PhotoPos4;
    [SerializeField] private Transform PhotoPos5;
    [SerializeField] private Transform PhotoStartPos;
    [SerializeField] private Transform PhotoEndPos;

    public UniversalCoinImage currCoin;
    private ActionWordEnum currSet;
    private ChallengeWord currWord;
    private List<ChallengeWord> prevWords;

    // other variables
    private ChallengeWord currentWord1;
    private ChallengeWord currentWord2;
    private ChallengeWord currentWord3;
    private ChallengeWord currentWord4;
    private ChallengeWord currentWord5;
    private int currentSwipeIndex;

    private List<UniversalCoinImage> currentCoins;
    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;

    [Header("Tutorial")]
    public bool playTutorial;
    private int tutorialEvent = 0;
    private bool playIntro = false;
    public ActionWordEnum tutorialSet1;
    public ActionWordEnum tutorialSet2;
    public ActionWordEnum tutorialSet3;
    public List<ChallengeWord> tutorialWords1;
    public List<ChallengeWord> tutorialWords2;
    public List<ChallengeWord> tutorialWords3;

    [Header("Scripted")]
    private int scriptedEvent = 0;
    public List<ChallengeWord> polaroidsScripted1;
    public List<ChallengeWord> polaroidsScripted2;
    public List<ChallengeWord> polaroidsScripted3;
    public List<ChallengeWord> polaroidsScripted4;
    public List<ChallengeWord> polaroidsScripted5;

    public bool testthis;
    List<ChallengeWord> word_pool = new List<ChallengeWord>();

    private float startTime;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // set start time
        startTime = Time.time;

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
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
        // set coin off-screen
        currCoin.transform.position = coinStartPos.position;

        // set base pattern
        pattern.baseState();

        // turn off raycaster
        TigerGameRaycaster.instance.isOn = false;

        // init empty lists
        prevWords = new List<ChallengeWord>();

        // start split song

        if (!playTutorial)
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.challengeGameSongSplit1);

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        currCoin.transform.position = coinStartPos.position;

        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currSet = tutorialSet1;
                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords1[i - 1]);
                    }
                    break;

                case 1:
                    currSet = tutorialSet2;
                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords2[i - 1]);
                    }
                    break;

                case 2:
                    currSet = tutorialSet3;

                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords3[i - 1]);
                    }
                    break;
            }
            tutorialEvent++;
        }
        else if ((StudentInfoSystem.GetCurrentProfile().tPawPolPlayed == 0 || testthis))
        {
            // get correct tutorial polaroids
            List<ChallengeWord> scriptedList = new List<ChallengeWord>();
            switch (scriptedEvent)
            {
                case 0:
                    scriptedList.AddRange(polaroidsScripted1);
                    currWord = polaroidsScripted1[4];
                    currSet = ActionWordEnum.mudslide;

                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted1[i]);
                    }
                    break;

                case 1:
                    scriptedList.AddRange(polaroidsScripted2);
                    currWord = polaroidsScripted2[2];
                    currSet = ActionWordEnum.orcs;

                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted2[i]);
                    }
                    break;

                case 2:
                    scriptedList.AddRange(polaroidsScripted3);
                    currWord = polaroidsScripted3[1];
                    currSet = ActionWordEnum.poop;

                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted3[i]);
                    }
                    break;

                case 3:
                    scriptedList.AddRange(polaroidsScripted4);
                    currWord = polaroidsScripted4[4];
                    currSet = ActionWordEnum.listen;

                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted4[i]);
                    }
                    break;

                case 4:
                    scriptedList.AddRange(polaroidsScripted5);
                    currWord = polaroidsScripted5[3];
                    currSet = ActionWordEnum.explorer;

                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted5[i]);
                    }
                    break;
            }
            scriptedEvent++;
        }
        else if (GameManager.instance.practiceModeON)
        {
            currSet = AISystem.TigerPawPhotosCoinSelection(GameManager.instance.practicePhonemes);
            word_pool = AISystem.ChallengeWordSelectionTigerPawPol(currSet, prevWords, GameManager.instance.practiceDifficulty, true);
            prevWords.AddRange(word_pool);

            for (int i = 0; i < polaroidC.Count; i++)
            {
                int randomIndex = Random.Range(0, word_pool.Count);
                polaroidC[i].SetPolaroid(word_pool[randomIndex]);
                word_pool.RemoveAt(randomIndex);
            }
        }
        else
        {
            currSet = AISystem.TigerPawPhotosCoinSelection();
            word_pool = AISystem.ChallengeWordSelectionTigerPawPol(currSet, prevWords);
            prevWords.AddRange(word_pool);

            for (int i = 0; i < polaroidC.Count; i++)
            {
                int randomIndex = Random.Range(0, word_pool.Count);
                polaroidC[i].SetPolaroid(word_pool[randomIndex]);
                word_pool.RemoveAt(randomIndex);
            }
        }

        pattern.baseState();
        AudioManager.instance.StopFX("tiger_paw_glitter");

        yield return new WaitForSeconds(0.5f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 1-2
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro1);
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro2);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd2.GetResult() + 1f);

            // play tutorial intro 3
            AssetReference clip = GameIntroDatabase.instance.tigerPawPhotosIntro3;

            CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd3.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd3.GetResult() + 1f);

            // play tutorial intro 4
            clip = GameIntroDatabase.instance.tigerPawPhotosIntro4;

            CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd4.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd4.GetResult() + 1f);
        }
        else if (!GameManager.instance.practiceModeON)
        {
            if (!playIntro)
            {
                // play start 1
                AssetReference clip = GameIntroDatabase.instance.tigerPawPhotosStart;

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult());
            }
        }

        if (!playIntro)
        {
            playIntro = true;
            // turn on settings button
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }


        Tiger.TigerDeal();
        // play audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PhotosSlideIn, 0.5f);
        yield return new WaitForSeconds(.6f);
        currCoin.gameObject.transform.position = coinLandPos.position;
        currCoin.SetActionWordValue(currSet);
        yield return new WaitForSeconds(.5f);

        int startPolaroid = 0;
        int endPolaroid = 5;
        if (playTutorial)
        {
            startPolaroid = 1;
            endPolaroid = 4;
        }

        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoStartPos.position, 0f));
        }
        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid), .2f));
        }
        yield return new WaitForSeconds(.35f);

        for (int i = 1 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 1), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 2 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 2), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 3 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 3), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 4 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 4), .2f));
        }

        PlayAudioCoin(currCoin);

        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 5-6
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro5);
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro6);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd2.GetResult() + 1f);
        }

        // disable raycaster
        TigerGameRaycaster.instance.isOn = true;
    }

    private Vector3 GetPolaroidPosition(int index)
    {
        switch (index)
        {
            default: return PhotoStartPos.position;
            case 0: return PhotoPos1.position;
            case 1: return PhotoPos2.position;
            case 2: return PhotoPos3.position;
            case 3: return PhotoPos4.position;
            case 4: return PhotoPos5.position;
        }
    }

    private IEnumerator LerpMoveObject(Transform obj, Vector3 target, float lerpTime)
    {
        Vector3 start = obj.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                obj.position = target;
                break;
            }

            Vector3 tempPos = Vector3.Lerp(start, target, timer / lerpTime);
            obj.position = tempPos;
            yield return null;
        }
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        StartCoroutine(PlayAudioCoinRoutine(coin));
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        // glow coin
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);

        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);
        yield return new WaitForSeconds(0.5f);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);

        playingCoinAudio = false;
    }

    public void EvaluateWaterCoin(Polaroid Photo)
    {
        // disable raycaster + stop coroutines
        TigerGameRaycaster.instance.isOn = false;
        TigerGameRaycaster.instance.EndAudioRoutine();

        bool success = (Photo.challengeWord.set == currSet);
        // only track challenge round attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            int difficultyLevel = 1 + Mathf.FloorToInt(StudentInfoSystem.GetCurrentProfile().starsTPawPol / 3);
            StudentInfoSystem.SavePlayerChallengeRoundAttempt(GameType.TigerPawPhotos, success, Photo.challengeWord, difficultyLevel);
        }

        if (success)
        {
            currentPolaroid = Photo;
            StartCoroutine(PostRound(true));
        }
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    public void returnToPos(GameObject currPhoto)
    {

        currPhoto.gameObject.transform.SetParent(selectedObjectParentCoin);

        if (currPhoto.name == "Polaroid1")
        {
            StartCoroutine(LerpMoveObject(polaroidC[0].transform, PhotoPos1.position, .2f));
        }
        else if (currPhoto.name == "Polaroid2")
        {
            StartCoroutine(LerpMoveObject(polaroidC[1].transform, PhotoPos2.position, .2f));
        }
        else if (currPhoto.name == "Polaroid3")
        {
            StartCoroutine(LerpMoveObject(polaroidC[2].transform, PhotoPos3.position, .2f));
        }
        else if (currPhoto.name == "Polaroid4")
        {
            StartCoroutine(LerpMoveObject(polaroidC[3].transform, PhotoPos4.position, .2f));
        }
        else if (currPhoto.name == "Polaroid5")
        {
            StartCoroutine(LerpMoveObject(polaroidC[4].transform, PhotoPos5.position, .2f));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        // play audio
        AudioManager.instance.PlayCoinDrop();

        if (win)
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            pattern.correct();
            AudioManager.instance.PlayFX_loop(AudioDatabase.instance.GlitterLoop, 0.1f, "tiger_paw_glitter");
            correctCoins[numWins].SetActive(true);
            numWins++;

            // show challenge word letters
            if (showChallengeWordLetters)
            {
                yield return new WaitForSeconds(1f);

                // squish on x scale
                currentPolaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 1f), 0.2f);
                yield return new WaitForSeconds(0.2f);
                // play audio fx 
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 1f);
                // remove image
                currentPolaroid.ShowPolaroidWord(60f);
                // un-squish on x scale
                currentPolaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
                yield return new WaitForSeconds(1f);

                // say letter groups using coins
                for (int i = 0; i < currentPolaroid.challengeWord.elkoninCount; i++)
                {
                    GameObject letterElement = currentPolaroid.GetLetterGroupElement(i);
                    letterElement.GetComponent<TextMeshProUGUI>().color = Color.black;
                    letterElement.GetComponent<LerpableObject>().LerpTextSize(70f - (Polaroid.FONT_SCALE_DECREASE * currentPolaroid.challengeWord.elkoninCount), 0.2f);
                    AudioManager.instance.PlayPhoneme(currentPolaroid.challengeWord.elkoninList[i]);

                    // move coin if equal to set
                    if (currentPolaroid.challengeWord.elkoninList[i] == ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentPolaroid.challengeWord.set))
                    {
                        currCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
                        yield return new WaitForSeconds(1f);

                        currCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
                        continue;
                    }
                    
                    yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(0.2f);

                // read word aloud to player
                if (currentPolaroid.challengeWord.audio != null)
                    AudioManager.instance.PlayTalk(currentPolaroid.challengeWord.audio);
                // start wiggle
                currentPolaroid.ToggleWiggle(true);

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentPolaroid.challengeWord.audio));
                yield return cd.coroutine;

                // wait an appropriate amount of time
                if (currentPolaroid.challengeWord.audio != null)
                    yield return new WaitForSeconds(cd.GetResult() + 0.25f);
                else
                    yield return new WaitForSeconds(2f);

                // end wiggle
                currentPolaroid.ToggleWiggle(false);

                yield return new WaitForSeconds(0.2f);

            }

            // increase split song
            if (!playTutorial)
                AudioManager.instance.IncreaseSplitSong();
        }
        else
        {
            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.2f);

            if (playTutorial)
            {
                // turn on raycaster
                TigerGameRaycaster.instance.isOn = true;
                yield break;
            }

            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
        }

        // play popup
        StartCoroutine(PlayPopup(win));

        //yield return new WaitForSeconds(1f);
        // play audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PhotosSlideOut, 0.5f);
        Tiger.TigerDeal();
        yield return new WaitForSeconds(.5f);
        currCoin.gameObject.transform.position = coinStartPos.position;
        yield return new WaitForSeconds(1f);

        int startPolaroid = 0;
        int endPolaroid = 5;
        if (playTutorial)
        {
            startPolaroid = 1;
            endPolaroid = 4;
        }

        for (int i = startPolaroid; i < endPolaroid - 4; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 3; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 1), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 2; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 2), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 1; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 3), .2f));
        }
        yield return new WaitForSeconds(.2f);
        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoEndPos.position, .2f));
        }

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

        // special wait for seconds for tutorial
        if (playTutorial && tutorialEvent == 1)
        {
            yield return new WaitForSeconds(2f);
        }

        // reset current polaroid
        currentPolaroid.HidePolaroidWord();

        StartCoroutine(StartGame());
    }

    private IEnumerator PlayPopup(bool win)
    {
        yield return new WaitForSeconds(1f);

        // play appropriate popup
        if (win)
        {
            if (playTutorial && tutorialEvent == 1)
            {
                // play start 1
                AssetReference clip = GameIntroDatabase.instance.tigerPawCoinIntro7;

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
            else if (numWins < 3)
            {
                if (GameManager.DeterminePlayPopup())
                {
                    // play julius lose popup
                    AssetReference clip = null;

                    if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapters1_4.Count)];
                    }
                    else
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapter5[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapter5.Count)];
                    }

                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    // yield return new WaitForSeconds(cd.GetResult() + 1f);
                }
            }
            else
            {
                if (GameManager.DeterminePlayPopup())
                {
                    // play julius final lose popup
                    AssetReference clip = null;

                    if (playTutorial)
                    {
                        clip = GameIntroDatabase.instance.tigerPawPhotosIntro8;
                    }
                    else
                    {
                        if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                        {
                            clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                        }
                        else
                        {
                            clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                        }
                    }

                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                    //yield return new WaitForSeconds(cd.GetResult() + 1f);
                }
            }
        }
        else
        {
            if (numMisses < 3)
            {
                // play julius win popup
                AssetReference clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusWinChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusWinChapters1_4.Count)];
                }
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusWinChapter5[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusWinChapter5.Count)];
                }

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult());
            }
            else
            {
                // play julius final win popup
                AssetReference clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusWinChapters1_4;
                }
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusWinChapters1_4;
                }

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd.GetResult());
            }
        }
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            float elapsedTime = Time.time - startTime;

            //// ANALYTICS : send challengegame_completed event
            StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "challengegame_name", GameType.TigerPawPhotos.ToString() },
                { "stars_awarded", 0 },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", true },
                { "prev_times_played", data.tPawPolPlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            AnalyticsManager.SendCustomEvent("challengegame_completed", parameters);

            GameManager.instance.LoadScene("TigerPawPhotos", true, 3f);
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
                { "challengegame_name", GameType.TigerPawPhotos.ToString() },
                { "stars_awarded", starsAwarded },
                { "elapsed_time", elapsedTime },
                { "tutorial_played", false },
                { "prev_times_played", data.tPawPolPlayed },
                { "curr_storybeat", data.currStoryBeat.ToString() }
            };            
            AnalyticsManager.SendCustomEvent("challengegame_completed", parameters);

            // calculate and show stars
            StarAwardController.instance.AwardStarsAndExit(starsAwarded);
        }
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.tPawPolPlayed = playerData.tPawPolPlayed + 1;
        playerData.starsTPawPol = CalculateStars() + playerData.starsTPawPol;

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
}
