using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TigerGameManager : MonoBehaviour
{
    public static TigerGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private List<Polaroid> polaroidC;
    [SerializeField] private TigerGameRaycaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;
    private bool gameSetup = false;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;

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

    [SerializeField] private UniversalCoinImage waterCoins;
    private UniversalCoinImage currWaterCoin;

    // other variables
    private ChallengeWord currentWord1;
    private ChallengeWord currentWord2;
    private ChallengeWord currentWord3;
    private ChallengeWord currentWord4;
    private ChallengeWord currentWord5;
    private ElkoninValue currentTargetValue;
    private ChallengeWord currentTargetWord;
    private int currentSwipeIndex;

    private List<UniversalCoinImage> currentCoins;
    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    [Header("Testing")] // ache -> bake
    public bool overrideWord;
    //public ChallengeWord testChallengeWord;
    public ChallengeWord targetChallengeWord1;
    public ChallengeWord targetChallengeWord2;
    public ChallengeWord targetChallengeWord3;
    public ChallengeWord targetChallengeWord4;
    public ChallengeWord targetChallengeWord5;
    public List<ElkoninValue> coinOptions;
    public int swipeIndex;

    [SerializeField] private Vector2 noCoin;



    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        PregameSetup();
        StartCoroutine(StartGame());
    }


    private void PregameSetup()
    {
        ChallengeWordDatabase.InitCreateGlobalList(true);
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

        // set base pattern
        pattern.baseState();
    }

    private IEnumerator StartGame()
    {
        waterCoins.gameObject.transform.position = coinStartPos.position;
        waterCoins.SetSize(noCoin);
        
        if (overrideWord)
        {
            currentWord1 = targetChallengeWord1;
            currentWord2 = targetChallengeWord2;
            currentWord3 = targetChallengeWord3;
            currentWord4 = targetChallengeWord4;
            currentWord5 = targetChallengeWord5;
            polaroidC[0].SetPolaroid(currentWord1);
            polaroidC[1].SetPolaroid(currentWord2);
            polaroidC[2].SetPolaroid(currentWord3);
            polaroidC[3].SetPolaroid(currentWord4);
            polaroidC[4].SetPolaroid(currentWord5);
        }
        else
        {
            // devise a query algorithm to determine polaroids / coin
        }

        yield return new WaitForSeconds(1f);

        pattern.baseState();
        yield return new WaitForSeconds(0.5f);

        Tiger.TigerDeal();
        waterCoins.gameObject.transform.position = coinLandPos.position;
        yield return new WaitForSeconds(.6f);

        waterCoins.SetValue(coinOptions[0]);
        waterCoins.SetSize(normalCoinSize);
        yield return new WaitForSeconds(.45f);

        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoStartPos.position, 0f));
        }
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos1.position, .2f));
        }
        yield return new WaitForSeconds(.35f);

        for (int i = 1; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos2.position, .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 2; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos3.position, .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 3; i < 5; i++)
        {

            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos4.position, .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 4; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos5.position, .2f));
        }
        yield return new WaitForSeconds(.6f);

        currentTargetWord = targetChallengeWord2;
        currentTargetValue = currentTargetWord.elkoninList[swipeIndex];

        yield return new WaitForSeconds(1f);
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

    public void GlowAndPlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);

        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1.5f);

        coin.LerpSize(normalCoinSize, 0.25f);
        coin.ToggleGlowOutline(false);

        playingCoinAudio = false;
    }


    private ChallengeWord GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];

        // make sure word is not being used
        if (usedWordList.Contains(word))
        {
            unusedWordList.Remove(word);
            return GetUnusedWord();
        }

        unusedWordList.Remove(word);
        usedWordList.Add(word);
        return word;
    }

    public void EvaluateWaterCoin(Polaroid Photo)
    {
        if (Photo.challengeWord.elkoninList[swipeIndex] == waterCoins.value)
        {
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

        if (currPhoto.name == "PolaroidObject1")
        {
            StartCoroutine(LerpMoveObject(polaroidC[0].transform, PhotoPos1.position, .2f));
        }
        else if (currPhoto.name == "PolaroidObject2")
        {
            StartCoroutine(LerpMoveObject(polaroidC[1].transform, PhotoPos2.position, .2f));
        }
        else if (currPhoto.name == "PolaroidObject3")
        {
            StartCoroutine(LerpMoveObject(polaroidC[2].transform, PhotoPos3.position, .2f));
        }
        else if (currPhoto.name == "PolaroidObject4")
        {
            StartCoroutine(LerpMoveObject(polaroidC[3].transform, PhotoPos4.position, .2f));
        }
        else
        {
            StartCoroutine(LerpMoveObject(polaroidC[4].transform, PhotoPos5.position, .2f));
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
            Tiger.TigerAway();
            yield return new WaitForSeconds(.55f);
            waterCoins.gameObject.transform.position = coinStartPos.position;
        }
        else
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);
            
            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
            Tiger.TigerAway();
            yield return new WaitForSeconds(.55f);
            waterCoins.gameObject.transform.position = coinStartPos.position;
        }
        yield return new WaitForSeconds(.5f);

        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.45f);
        for (int i = 0; i < 1; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos2.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos3.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos4.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 4; i++)
        {

            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoPos5.position, .2f));
        }
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoEndPos.position, .2f));
        }


        if (numWins == 3)
        {
            StartCoroutine(WinRoutine());
        }
        else if (numMisses == 3)
        {
           StartCoroutine(LoseRoutine());
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);

        // // update SIS
        // if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_3)
        // {
        //     // first time losing
        //     if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame)
        //         StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = true;
        //     else
        //     {
        //         // every other time losing
        //         if (!StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        //         {
        //             StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = true;
        //         }
        //     }
        //     StudentInfoSystem.SaveStudentPlayerData();
        // }

        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        // update SIS
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_3)
        {
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = false;
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = false;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }

        // show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private int CalculateStars()
    {
        if (numMisses <= 0)
            return 3;
        else if (numMisses > 0 && numMisses <= 2)
            return 2;
        else
            return 1;
    }
}
