using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WordFactorySubstitutingManager : MonoBehaviour
{
    public static WordFactorySubstitutingManager instance;

    [Header("Frames")]
    [SerializeField] private HorizontalLayoutGroup frameTargetGroup;
    [SerializeField] private Transform[] framesReal; // six in-game frames
    [SerializeField] private Transform[] framesTarget;
    [SerializeField] private float[] frameSpacings;

    [Header("Coins")]
    [SerializeField] private Transform boxCoinParent;
    [SerializeField] private Transform swipeParent;
    public Transform waterCoinParent;
    [SerializeField] private List<UniversalCoinImage> waterCoins;
    private UniversalCoinImage currWaterCoin;
    [SerializeField] private List<Transform> waterCoinActivePos;
    [SerializeField] private List<Transform> waterCoinInactivePos;
    [SerializeField] private GameObject universalCoin;
    [SerializeField] private GameObject tigerSwipe;

    [Header("MoveableCanvas")]
    [SerializeField] private Transform moveableCanvas;
    [SerializeField] private Transform normalPos;
    [SerializeField] private Transform bottomPos;
    public GameObject leftArrow;

    [Header("Characters")]
    [SerializeField] private Animator redAnimator;
    [SerializeField] private Animator tigerAnimator;

    [Header("Polaroids")]
    [SerializeField] private Polaroid tigerPolaroid;
    [SerializeField] private Polaroid redPolaroid;

    [Header("Cards")]
    [SerializeField] private GameObject[] redCards;
    [SerializeField] private GameObject[] tigerCards;
    private int redCardCount = 0;
    private int tigerCardCount = 0;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos_left;
    [SerializeField] private Transform polaroidMiddlePos_left;
    [SerializeField] private Transform polaroidBottomPos_left;

    [SerializeField] private Transform polaroidStartPos_right;
    [SerializeField] private Transform polaroidMiddlePos_right;
    [SerializeField] private Transform polaroidBottomPos_right;

    [SerializeField] private Transform polaroid_middlePos;

    // other variables
    private WordPair currentPair;
    private List<WordPair> prevPairs;
    private ChallengeWord currentWord;
    private ElkoninValue currentTargetValue;
    private ChallengeWord currentTargetWord;
    private int currentSwipeIndex;

    private List<UniversalCoinImage> currentCoins;
    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;
    private bool evaluatingCoin = false;

    private List<ElkoninValue> elkoninPool;
    private List<ElkoninValue> lockedPool;

    [Header("Tutorial")]
    public bool playTutorial;
    private int tutorialEvent = 0;
    private bool playIntro = false;
    public WordPair tutorialPair1;
    public WordPair tutorialPair2;
    public WordPair tutorialPair3;


    [Header("Testing")] // ache -> bake
    public bool overrideWord;
    public WordPair testObject;

    private bool firstEntry;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    private void Start()
    {
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = true;
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
        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // init empty lists
        prevPairs = new List<WordPair>();
        lockedPool = new List<ElkoninValue>();

        // add all action words excpet unlocked ones
        foreach (var coin in GameManager.instance.actionWords)
        {
            if (!StudentInfoSystem.GetCurrentProfile().actionWordPool.Contains(ChallengeWordDatabase.ElkoninValueToActionWord(coin.elkoninValue)))
                lockedPool.Add(coin.elkoninValue);
        }

        // begin first round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        firstEntry = true;
        WordFactorySubstituteRaycaster.instance.isOn = false;

        // place polaroids in start pos
        tigerPolaroid.LerpScale(0f, 0.001f);
        tigerPolaroid.transform.position = polaroidStartPos_right.position;
        tigerPolaroid.LerpRotation(0, 0001f);

        redPolaroid.LerpScale(0f, 0.001f);
        redPolaroid.transform.position = polaroidStartPos_left.position;
        redPolaroid.LerpRotation(0, 0001f);

        // hide left arrow
        leftArrow.transform.localScale = new Vector3(0f, 0f, 1f);


        // tutorial stuff
        if (playTutorial)
        {
            // short pause before start
            yield return new WaitForSeconds(1f);

            if (tutorialEvent == 0)
            {
                // play tutorial intro 1
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro1;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);

                // play tutorial intro 2
                clip = GameIntroDatabase.instance.substitutingIntro2;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 1f);
            }
            else
            {
                // play tutorial intro 3
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro3;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);
            }
        }
        else
        {
            if (!playIntro)
            {
                // short pause before start
                yield return new WaitForSeconds(1f);

                // play start 1
                AssetReference clip = GameIntroDatabase.instance.substitutingStart1;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 0.5f);

                // play start 2
                clip = GameIntroDatabase.instance.substitutingStart2;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 0.5f);
            }
        }

        if (!playIntro)
        {
            playIntro = true;
            // turn on settings button
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        // get challenge words
        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentPair = tutorialPair1;
                    currentWord = currentPair.word1;
                    break;

                case 1:
                    currentPair = tutorialPair2;
                    currentWord = currentPair.word1;
                    break;

                case 2:
                    currentPair = tutorialPair3;
                    currentWord = currentPair.word1;
                    break;
            }
            tutorialEvent++;
        }
        else
        {
            // set polaroid challenge word
            if (overrideWord)
            {
                currentPair = testObject;
                currentWord = currentPair.word1;
            }
            else
            {
                // use AI word selection
                currentPair = AISystem.ChallengeWordSub(prevPairs);
                currentWord = currentPair.word1;

                // set prev words
                prevPairs.Add(currentPair);
            }
        }

        tigerPolaroid.SetPolaroid(currentWord);

        // move tiger polaroid to middle pos
        tigerAnimator.Play("TigerSwipe");
        tigerPolaroid.LerpScale(1f, 0.5f);
        tigerPolaroid.MovePolaroid(polaroid_middlePos.position, 0.5f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.5f);
        yield return new WaitForSeconds(1f);

        // shake tiger polaroid
        tigerPolaroid.GetComponent<SpriteShakeController>().ShakeObject(1f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidRattle, 0.5f);

        // place all real frames behind tiger polaroid and make scale 0
        foreach (var frame in framesReal)
        {
            frame.transform.position = polaroid_middlePos.position;
            StartCoroutine(LerpObjectScale(frame.transform, 0f, 0f));
        }

        // activate all frames
        foreach (var frame in framesTarget)
        {
            frame.gameObject.SetActive(true);
        }
        foreach (var frame in framesReal)
        {
            frame.gameObject.SetActive(true);
        }

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            framesReal[5 - i].gameObject.SetActive(false);
            framesTarget[5 - i].gameObject.SetActive(false);
        }

        // move target frames
        StartCoroutine(LerpFrameSpacing(frameSpacings[currentWord.elkoninCount - 1], 0f));

        yield return new WaitForSeconds(0.5f);

        // show + move real frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(framesReal[i].GetComponent<Image>(), 1f, 0f));
            StartCoroutine(LerpMoveObject(framesReal[i], framesTarget[i].position, 0.5f));
            StartCoroutine(LerpObjectScale(framesReal[i].transform, 1f, 0.5f));
        }

        yield return new WaitForSeconds(1f);

        // place coins
        // show coins + add to list
        currentCoins = new List<UniversalCoinImage>();
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, framesReal[i].position, Quaternion.identity, boxCoinParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.SetValue(currentWord.elkoninList[i]);
            coin.LerpScale(new Vector2(1.2f, 1.2f), 0.1f);
            currentCoins.Add(coin);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.5f);

        // say polaroid elkonin audios
        foreach (var coin in currentCoins)
        {
            PlayAudioCoin(coin);
            yield return new WaitForSeconds(0.8f);
        }
        yield return new WaitForSeconds(0.5f);

        // say polaroid word
        foreach (var coin in currentCoins)
        {
            coin.LerpScale(new Vector2(1.35f, 1.35f), 0.25f);
            yield return new WaitForSeconds(0.01f);
        }

        tigerPolaroid.LerpScale(1.1f, 0.1f);


        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(tigerPolaroid.challengeWord.audio));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(tigerPolaroid.challengeWord.audio);
        yield return new WaitForSeconds(cd.GetResult());
        tigerPolaroid.LerpScale(1f, 0.1f);

        foreach (var coin in currentCoins)
        {
            coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
            yield return new WaitForSeconds(0.1f);
        }

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 5
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro5;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);

                // play tutorial intro 6
                clip = GameIntroDatabase.instance.substitutingIntro6;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 1f);
            }
        }

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinRattle, 0.1f);
        yield return new WaitForSeconds(0.2f);
        // shake coin
        currentCoins[currentPair.index].ShakeCoin(1f);
        yield return new WaitForSeconds(0.5f);

        // tiger swipe away coin
        Vector3 swipePos = framesReal[currentPair.index].position;
        swipePos.y += 1f;
        var swipe = Instantiate(tigerSwipe, swipePos, Quaternion.identity, swipeParent);
        swipe.GetComponent<TigerSwipe>().PlayTigerSwipe();
        tigerAnimator.Play("TigerSwipe");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.TigerSwipe, 0.5f);
        yield return new WaitForSeconds(0.25f);

        // remove coin
        currentCoins[currentPair.index].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.3f, 1.3f), new Vector2(0f, 0f), 0.1f, 0.1f);
        redAnimator.Play("Lose");

        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 7
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro7;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);

                // play tutorial intro 8
                clip = GameIntroDatabase.instance.substitutingIntro8;

                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd1.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd1.GetResult() + 1f);
            }
        }

        redAnimator.Play("Win");
        yield return new WaitForSeconds(0.5f);

        // remove coin from current coins list
        currentCoins[currentPair.index].gameObject.SetActive(false);
        currentCoins.RemoveAt(currentPair.index);

        // move tiger polaroid out of the way
        StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidMiddlePos_right.position, 0.6f));
        tigerPolaroid.LerpRotation(-8f, 0.6f);

        // bring red polaroid into game
        currentTargetWord = currentPair.word2;
        redPolaroid.SetPolaroid(currentTargetWord);
        redPolaroid.LerpScale(1f, 0.5f);
        redPolaroid.LerpRotation(8f, 0.5f);
        redPolaroid.MovePolaroid(polaroidMiddlePos_left.position, 0.5f);
        currentTargetValue = currentTargetWord.elkoninList[currentPair.index];
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f, "red_reveal", 0.8f);
        yield return new WaitForSeconds(1f);

        // say red's word
        redPolaroid.LerpScale(1.1f, 0.1f);

        cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(redPolaroid.challengeWord.audio));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(redPolaroid.challengeWord.audio);
        yield return new WaitForSeconds(cd.GetResult());
        redPolaroid.LerpScale(1f, 0.1f);

        // chose which index to be the correct 
        int correctIndex = Random.Range(0, 4);

        // create elkonin pool to choose water coins from
        elkoninPool = new List<ElkoninValue>();
        // add ALL values
        string[] allElkoninValues = System.Enum.GetNames(typeof(ElkoninValue));
        for (int i = 0; i < allElkoninValues.Length; i++)
        {
            elkoninPool.Add((ElkoninValue)System.Enum.Parse(typeof(ElkoninValue), allElkoninValues[i]));
        }
        // remove extra values
        elkoninPool.Remove(ElkoninValue.empty_gold);
        elkoninPool.Remove(ElkoninValue.empty_silver);
        elkoninPool.Remove(ElkoninValue.COUNT);
        // remove specific swipe values
        elkoninPool.Remove(currentPair.word2.elkoninList[currentPair.index]);
        elkoninPool.Remove(currentPair.word1.elkoninList[currentPair.index]);
        // remove any locked action word coins
        foreach (var value in lockedPool)
        {
            elkoninPool.Remove(value);
        }

        // place water coins
        ResetWaterCoins(false);
        for (int i = 0; i < 4; i++)
        {
            if (i == correctIndex)
            {
                waterCoins[i].SetValue(currentPair.word2.elkoninList[currentPair.index]);
            }
            else
            {
                waterCoins[i].SetValue(GetElkoninValue());
            }
        }

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 9
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro9;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);
            }
        }

        // pan camera down to bottom position + move polaroid down
        StartCoroutine(LerpMoveObject(moveableCanvas, bottomPos.position, 1f));
        StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidBottomPos_right.position, 1f));
        StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidBottomPos_left.position, 1f));
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PanDown, 0.1f);
        yield return new WaitForSeconds(1f);

        // move water coins up
        int count = 0;
        foreach (var coin in waterCoins)
        {
            Vector3 bouncePos = waterCoinActivePos[count].position;
            bouncePos.y += 0.5f;
            waterCoins[count].GetComponent<LerpableObject>().LerpPosition(bouncePos, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            waterCoins[count].GetComponent<LerpableObject>().LerpPosition(waterCoinActivePos[count].position, 0.2f, false);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.1f, "water_splash", (1f + 0.25f * count));
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "water_splash", (1f + 0.25f * count));
            yield return new WaitForSeconds(0.1f);
            count++;
        }
        yield return new WaitForSeconds(0.5f);

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // grow and wiggle correct coin
                foreach (var coin in waterCoins)
                {
                    if (coin.value == currentTargetValue)
                    {
                        coin.GetComponent<WiggleController>().StartWiggle();
                        coin.LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
                    }
                    else
                    {
                        coin.LerpScale(new Vector2(0.8f, 0.8f), 0.1f);
                        coin.SetTransparency(0.5f, true);
                    }
                }

                // play tutorial intro 10
                AssetReference clip = GameIntroDatabase.instance.substitutingIntro10;

                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd0.GetResult() + 1f);
            }
        }

        // reveal left arrow
        leftArrow.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 0.25f);
        // make current frame wiggle
        framesReal[currentPair.index].GetComponent<WiggleController>().StartWiggle();

        WordFactorySubstituteRaycaster.instance.isOn = true;
    }

    private ElkoninValue GetElkoninValue()
    {
        int index = Random.Range(0, elkoninPool.Count);
        ElkoninValue value = elkoninPool[index];
        elkoninPool.RemoveAt(index);
        return value;
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
        float start = frameTargetGroup.spacing;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                frameTargetGroup.spacing = target;
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            frameTargetGroup.spacing = temp;
            yield return null;
        }
    }

    private IEnumerator LerpObjectScale(Transform obj, float target, float totalTime)
    {
        float start = obj.localScale.x;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                obj.localScale = new Vector3(target, target, 1f);
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            obj.localScale = new Vector3(temp, temp, 1f);
            yield return null;
        }
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        // check lists
        if (currentCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
        // water coins
        else if (waterCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin, true));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        // water coin differential
        if (!waterCoin) coin.LerpScale(new Vector2(1.35f, 1.35f), 0.25f);
        else coin.LerpScale(new Vector2(1.1f, 1.1f), 0.25f);

        yield return new WaitForSeconds(0.8f);
        // return if current water coin
        if (coin == currWaterCoin && waterCoin)
        {
            playingCoinAudio = false;
            yield break;
        }

        // water coin differential
        if (!waterCoin) coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
        else coin.LerpScale(Vector2.one, 0.25f);

        playingCoinAudio = false;
    }

    public void ReturnWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            if (waterCoins[i] != currWaterCoin)
            {
                StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinActivePos[i].position, 0.25f));
                waterCoins[i].LerpScale(Vector2.one, 0.1f);
            }
        }
    }

    public void ResetWaterCoins(bool skipCurrentCoin)
    {
        for (int i = 0; i < 4; i++)
        {
            if (skipCurrentCoin && waterCoins[i] == currWaterCoin)
            {
                // skip coin
            }
            else
            {
                StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinInactivePos[i].position, 0.25f));
                waterCoins[i].LerpScale(Vector2.one, 0.1f);
                waterCoins[i].SetTransparency(1f, true);
            }
        }
    }

    public void EvaluateWaterCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        if (currentPair.index == 0)
        {
            currentCoins.Insert(0, coin);
        }
        else
        {
            currentCoins.Add(coin);
        }

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SelectBoop, 0.5f);

        // remove left arrow
        leftArrow.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        // make current frame stop wiggle
        framesReal[currentPair.index].GetComponent<WiggleController>().StopWiggle();

        currWaterCoin = coin;
        // scale water coin correctly
        coin.LerpScale(new Vector2(1.2f, 1.2f), 0.25f);

        // lerp coin to frame position
        StartCoroutine(LerpMoveObject(coin.transform, framesReal[currentPair.index].position, 0.25f));

        bool success = (coin.value == currentTargetValue);
        // only track challenge round attempt if not in tutorial AND not in practice mode
        if (!playTutorial && !GameManager.instance.practiceModeON)
        {
            StudentInfoSystem.SavePlayerChallengeRoundAttempt(GameType.WordFactorySubstituting, success, currentTargetWord, 0); //// TODO: add player difficulty once it is available
        }

        if (success)
        {
            StartCoroutine(PostRound(true));
        }
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        WordFactorySubstituteRaycaster.instance.isOn = false;

        yield return new WaitForSeconds(1f);

        if (win)
        {
            numWins++;

            // reset coin wiggle
            if (playTutorial)
            {
                currWaterCoin.GetComponent<WiggleController>().StopWiggle();
            }

            // say each letter
            foreach (var coin in currentCoins)
            {
                PlayAudioCoin(coin);
                yield return new WaitForSeconds(0.8f);
            }

            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // say new challenge word
            redPolaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(currentTargetWord.audio));
            yield return cd.coroutine;

            AudioManager.instance.PlayTalk(currentTargetWord.audio);
            yield return new WaitForSeconds(cd.GetResult() + 0.5f);
            redPolaroid.GetComponent<LerpableObject>().LerpScale(Vector2.one, 0.1f);

            // slide tiger polaroid under red polaroid
            tigerPolaroid.transform.SetAsFirstSibling();
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, redPolaroid.transform.position, 0.2f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_left.position, 0.5f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_left.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
            yield return new WaitForSeconds(1.5f);

            // play correct animations
            redAnimator.Play("Win");
            tigerAnimator.Play("TigerLose");

            // give red one polaroid
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
            redCardCount++;
        }
        else
        {
            foreach (var coin in currentCoins)
            {
                PlayAudioCoin(coin);
                yield return new WaitForSeconds(0.8f);
            }

            // play wrong sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            // skip if playing tutorial
            if (playTutorial || firstEntry)
            {
                firstEntry = false;
                WordFactorySubstituteRaycaster.instance.isOn = true;

                // remove coin from current coins
                currentCoins.Remove(currWaterCoin);
                currWaterCoin = null;
                ReturnWaterCoins();

                // add left arrow
                leftArrow.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
                // make current frame wiggle
                framesReal[currentPair.index].GetComponent<WiggleController>().StartWiggle();

                evaluatingCoin = false;

                yield break;
            }

            // only increase misses if not playing tutorial
            if (!playTutorial)
                numMisses++;

            // slide tiger polaroid under red polaroid
            redPolaroid.transform.SetAsFirstSibling();
            StartCoroutine(LerpMoveObject(redPolaroid.transform, tigerPolaroid.transform.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_right.position, 0.5f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_right.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
            yield return new WaitForSeconds(1.5f);

            // play correct animations
            redAnimator.Play("Lose");
            tigerAnimator.Play("TigerWin");

            // give red one polaroid
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
            tigerCardCount++;
        }

        redAnimator.Play("Idle");
        tigerAnimator.Play("TigerIdle");

        // pan camera up
        StartCoroutine(LerpMoveObject(moveableCanvas, normalPos.position, 1f));
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PanUp, 0.5f);

        // place polaroids in start pos
        tigerPolaroid.LerpScale(0f, 0.001f);
        tigerPolaroid.transform.position = polaroidStartPos_right.position;
        tigerPolaroid.LerpRotation(0, 0001f);

        redPolaroid.LerpScale(0f, 0.001f);
        redPolaroid.transform.position = polaroidStartPos_left.position;
        redPolaroid.LerpRotation(0, 0001f);

        // get rid of coins
        foreach (var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        }
        currWaterCoin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);

        // make frames scale 0
        foreach (var frame in framesReal)
        {
            if (frame.gameObject.activeSelf)
            {
                frame.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
                StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 0f, 0.5f));
            }
        }
        yield return new WaitForSeconds(0.5f);

        // place all real frames behind tiger polaroid
        foreach (var frame in framesReal)
            frame.transform.position = polaroid_middlePos.position;

        // reset water coins
        currWaterCoin.transform.position = waterCoinInactivePos[0].position;
        currWaterCoin.transform.localScale = new Vector3(1f, 1f, 1f);
        currWaterCoin = null;
        ResetWaterCoins(false);
        yield return new WaitForSeconds(0.5f);

        // check for win
        if (numWins >= 3)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // check for lose
        if (numMisses >= 3)
        {
            StartCoroutine(LoseRoutine());
            yield break;
        }

        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 11
            AssetReference clip = GameIntroDatabase.instance.substitutingIntro11;

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            // play tutorial intro 12
            clip = GameIntroDatabase.instance.substitutingIntro12;

            CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd0.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(cd0.GetResult() + 1f);

            // play tutorial intro 13
            clip = GameIntroDatabase.instance.substitutingIntro13;

            CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd1.coroutine;

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd1.GetResult() + 1f);

            // finish tutorial
            StartCoroutine(WinRoutine());
            yield break;
        }
        else
        {
            if (win)
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

                    CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd0.coroutine;

                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                    yield return new WaitForSeconds(cd0.GetResult());
                }
            }
            else
            {
                // play reminder popup
                List<AssetReference> clips = GameIntroDatabase.instance.substitutingReminderClips;
                AssetReference clip = clips[Random.Range(0, clips.Count)];

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1);
            }
        }

        // stop evaluating coin
        evaluatingCoin = false;

        // start next round
        StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("WordFactorySubstituting", true, 3f);
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
        playerData.subPlayed = playerData.subPlayed + 1;
        playerData.starsSub = CalculateStars() + playerData.starsSub;

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
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");

        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}