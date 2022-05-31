using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TigerCoinGameManager : MonoBehaviour
{
    public static TigerCoinGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private Polaroid polaroidC;
    [SerializeField] private TigerCoinRayCaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;


    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform CoinPos1;
    [SerializeField] private Transform CoinPos2;
    [SerializeField] private Transform CoinPos3;
    [SerializeField] private Transform CoinPos4;
    [SerializeField] private Transform CoinPos5;
    [SerializeField] private Transform CoinStartPos;
    [SerializeField] private Transform CoinEndPos;

    [SerializeField] private List<UniversalCoinImage> waterCoins;


    // other variables
    [SerializeField] private ChallengeWord currentWord;
    [SerializeField] private ElkoninValue currentTargetValue;
    [SerializeField] private ChallengeWord currentTargetWord;
    private List<ChallengeWord> prevWords;

    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;


    [Header("Tutorial")]
    public bool playTutorial;
    private int tutorialEvent = 0;
    private bool playIntro = false;
    public ChallengeWord tutorialPhoto1;
    public ChallengeWord tutorialPhoto2;
    public ChallengeWord tutorialPhoto3;

    [Header("Scripted")]
    private int scriptedEvent = 0;
    public ChallengeWord coinsScripted1;
    public ChallengeWord coinsScripted2;
    public ChallengeWord coinsScripted3;
    public ChallengeWord coinsScripted4;
    public ChallengeWord coinsScripted5;




    public bool testthis;


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
        // add ambiance
        // AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        // AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
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
        prevWords = new List<ChallengeWord>();
        pattern.baseState();
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // reset coin positions
        foreach (var coin in waterCoins)
            coin.transform.position = CoinStartPos.position;

        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentWord = tutorialPhoto1;
                    break;

                case 1:
                    currentWord = tutorialPhoto2;
                    break;

                case 2:
                    currentWord = tutorialPhoto3;
                    break;
            }

            tutorialEvent++;
            polaroidC.SetPolaroid(currentWord);
            currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentWord.set);
            int rand = Random.Range(0, 5);
            waterCoins[rand].SetValue(currentTargetValue);
        }
        //Scripted Tiger Paw Photo
        else if (StudentInfoSystem.GetCurrentProfile().tPawCoinPlayed == 0 || testthis)
        {
            // get correct tutorial polaroids
            switch (scriptedEvent)
            {
                case 0:
                    polaroidC.SetPolaroid(coinsScripted1);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted1.set);
                    waterCoins[0].SetValue(ElkoninValue.explorer);
                    waterCoins[1].SetValue(ElkoninValue.hello);
                    waterCoins[2].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.poop);
                    waterCoins[4].SetValue(ElkoninValue.think);
                    break;
                case 1:
                    polaroidC.SetPolaroid(coinsScripted2);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted2.set);
                    waterCoins[0].SetValue(ElkoninValue.mudslide);
                    waterCoins[1].SetValue(ElkoninValue.listen);
                    waterCoins[3].SetValue(currentTargetValue);
                    waterCoins[2].SetValue(ElkoninValue.explorer);
                    waterCoins[4].SetValue(ElkoninValue.poop);
                    break;
                case 2:
                    polaroidC.SetPolaroid(coinsScripted3);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted3.set);
                    waterCoins[2].SetValue(ElkoninValue.poop);
                    waterCoins[1].SetValue(ElkoninValue.hello);
                    waterCoins[0].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.think);
                    waterCoins[4].SetValue(ElkoninValue.listen);
                    break;
                case 3:
                    polaroidC.SetPolaroid(coinsScripted4);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted4.set);
                    waterCoins[0].SetValue(ElkoninValue.poop);
                    waterCoins[1].SetValue(ElkoninValue.think);
                    waterCoins[4].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.orcs);
                    waterCoins[2].SetValue(ElkoninValue.hello);
                    break;
                case 4:
                    polaroidC.SetPolaroid(coinsScripted5);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted5.set);
                    waterCoins[0].SetValue(ElkoninValue.orcs);
                    waterCoins[1].SetValue(ElkoninValue.listen);
                    waterCoins[2].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.explorer);
                    waterCoins[4].SetValue(ElkoninValue.mudslide);
                    break;
            }
            scriptedEvent++;
        }
        else
        {
            // use AI word selection
            List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();
            CurrentChallengeList = AISystem.ChallengeWordSelectionTigerPawCoin(prevWords);
            currentWord = CurrentChallengeList[0];
            polaroidC.SetPolaroid(currentWord);
            prevWords.Add(currentWord);

            // set coin options
            List<ActionWordEnum> coinOptions = new List<ActionWordEnum>();
            coinOptions = AISystem.TigerPawCoinsCoinSelection(StudentInfoSystem.GetCurrentProfile(), CurrentChallengeList);
            currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentWord.set);
            for (int i = 0; i < 5; i++)
            {
                int rand = Random.Range(0, coinOptions.Count);
                waterCoins[i].SetActionWordValue(coinOptions[rand]);
                coinOptions.RemoveAt(rand);
            }
        }


        yield return new WaitForSeconds(0.5f);

        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        // return pattern to base state
        pattern.baseState();

        // yield return new WaitForSeconds(0.5f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 1-2
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro1);
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro2);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd2.GetResult() + 1f);

            // play tutorial intro 3
            AssetReference clip = GameIntroDatabase.instance.tigerPawCoinIntro3;

            CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd3.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd3.GetResult() + 1f);

            // play tutorial intro 4
            clip = GameIntroDatabase.instance.tigerPawCoinIntro4;

            CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd4.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd4.GetResult() + 1f);
        }
        else if (!playIntro)
        {
            // short pause before start
            //yield return new WaitForSeconds(1f);

            // play start 1
            AssetReference clip = GameIntroDatabase.instance.tigerPawCoinStart;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd.GetResult());
        }

        if (!playIntro)
        {
            playIntro = true;
            // turn on settings button
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        Tiger.TigerDeal();
        polaroidC.gameObject.transform.position = polaroidLandPos.position;
        yield return new WaitForSeconds(.6f);

        polaroidC.LerpScale(1f, 0f);
        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos1.position, .2f));
        }
        yield return new WaitForSeconds(.15f);
        for (int i = 1; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos2.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 2; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos3.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 3; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos4.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 4; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos5.position, .2f));
        }

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            yield return new WaitForSeconds(1f);

            // play tutorial intro 5-6
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro5);
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro6);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[0]));
            yield return cd.coroutine;

            CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clips[1]));
            yield return cd2.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(cd.GetResult() + cd2.GetResult() + 1f);
        }

        // turn on raycaster
        TigerCoinRayCaster.instance.isOn = true;
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

    public void PlayCoinAudio(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (waterCoins.Contains(coin))
        {
            StartCoroutine(PlayCoinAudioRoutine(coin, true));
        }
    }

    private IEnumerator PlayCoinAudioRoutine(UniversalCoinImage coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);
        AudioManager.instance.PlayPhoneme(coin.value);
        yield return new WaitForSeconds(0.5f);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);

        playingCoinAudio = false;
    }

    public void EvaluateWaterCoin(UniversalCoinImage coin)
    {
        // turn off raycaster
        TigerCoinRayCaster.instance.isOn = false;

        if (coin.value == currentTargetValue)
        {
            StartCoroutine(PostRound(true));
        }
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    public void ReturnToPos(GameObject currCoin)
    {
        currCoin.gameObject.transform.SetParent(selectedObjectParentCoin);

        if (currCoin.name == "Coin1")
        {
            StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPos1.position, .2f));
        }
        else if (currCoin.name == "Coin2")
        {
            StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPos2.position, .2f));
        }
        else if (currCoin.name == "Coin3")
        {
            StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPos3.position, .2f));
        }
        else if (currCoin.name == "Coin4")
        {
            StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPos4.position, .2f));
        }
        else if (currCoin.name == "Coin5")
        {
            StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPos5.position, .2f));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        if (win)
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            pattern.correct();
            correctCoins[numWins].SetActive(true);
            numWins++;
        }
        else
        {
            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            if (playTutorial)
            {
                // turn on raycaster
                TigerCoinRayCaster.instance.isOn = true;
                yield break;
            }

            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
        }

        // play popup
        StartCoroutine(PlayPopup(win));

        Tiger.TigerDeal();
        yield return new WaitForSeconds(.4f);
        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 1; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos2.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos3.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos4.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos5.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinEndPos.position, .2f));
        }

        yield return new WaitForSeconds(1f);

        // reset coin positions
        foreach (var coin in waterCoins)
            coin.transform.position = CoinStartPos.position;
            
        // win or lose ?
        if (numWins == 3)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }
        else if (numMisses == 3)
        {
            StartCoroutine(LoseRoutine());
            yield break;
        }

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
                    //yield return new WaitForSeconds(cd.GetResult() + 1f); 
                }
                
            }
            else
            {
                if (GameManager.DeterminePlayPopup())
                {
                    // play julius final lose popup
                    AssetReference clip = null;

                    if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                    }
                    else
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
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
            StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("TigerPawCoins", true, 3f);
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
        playerData.tPawCoinPlayed = playerData.tPawCoinPlayed + 1;
        playerData.starsTPawCoin = CalculateStars() + playerData.starsTPawCoin;

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
