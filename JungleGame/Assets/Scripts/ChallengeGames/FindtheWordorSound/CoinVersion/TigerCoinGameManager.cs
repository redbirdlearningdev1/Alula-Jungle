using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool gameSetup = false;

    //maybe old??

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;

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
    private UniversalCoinImage currWaterCoin;

    // other variables
    [SerializeField]  private ChallengeWord currentWord;
    [SerializeField]  private ElkoninValue currentTargetValue;
    [SerializeField]  private ChallengeWord currentTargetWord;
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
    public ChallengeWord targetChallengeWord;
    public List<ElkoninValue> coinOptions;
    public List<ElkoninValue> BadCoinOptions;

    public int swipeIndex;

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
        pattern.baseState();
    }

    private IEnumerator StartGame()
    {
        swipeIndex = 0;
        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        if (overrideWord)
        {
            currentWord = targetChallengeWord;
            polaroidC.SetPolaroid(currentWord);
        }
        else
        {
            // use random words
            ChallengeWord word = GetUnusedWord();
            polaroidC.SetPolaroid(word);
            currentWord = word;
        }
        
        currentTargetValue = currentWord.elkoninList[swipeIndex];
        for (int d = 0; d < 6; d++)
        {
            for (int i = 0; i < BadCoinOptions.Count; i++)
            {
                if (currentTargetValue == BadCoinOptions[i])
                {
                    swipeIndex++;
                    currentTargetValue = currentWord.elkoninList[swipeIndex];
                }
            }
        }

        // return pattern to base state
        yield return new WaitForSeconds(1f);
        pattern.baseState();
        yield return new WaitForSeconds(.5f);

        Tiger.TigerDeal();
        polaroidC.gameObject.transform.position = polaroidLandPos.position;
        yield return new WaitForSeconds(.6f);

        polaroidC.LerpScale(1f, 0f);
        yield return new WaitForSeconds(0.85f);

        for (int i = 0; i < 5; i++)
        {
            int rand = Random.Range(0, coinOptions.Count);
            waterCoins[i].SetValue(coinOptions[rand]);
            coinOptions.RemoveAt(rand); 
            waterCoins[i].transform.position = CoinPos1.position;            
        }

        for (int i = 0; i < 5; i++)
        {
            coinOptions.Add(waterCoins[i].value);
        }

        if (waterCoins[0].value == currentTargetValue)
        {


        }
        else if (waterCoins[1].value == currentTargetValue)
        {


        }
        else if (waterCoins[2].value == currentTargetValue)
        {


        }
        else if (waterCoins[3].value == currentTargetValue)
        {


        }
        else if (waterCoins[4].value == currentTargetValue)
        {


        }
        else
        {
            int rand2 = Random.Range(0, 5);
            waterCoins[rand2].SetValue(currentTargetValue);
            Debug.Log(waterCoins[rand2]);
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

        if (waterCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin, true));
        }
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoinImage coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(0.5f);
        // return if current water coin
        if (coin == currWaterCoin)
        {
            coin.ToggleGlowOutline(false);
            playingCoinAudio = false;
            yield break;
        }

        coin.LerpSize(normalCoinSize, 0.25f);
        yield return new WaitForSeconds(0.25f);
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

    public void EvaluateWaterCoin(UniversalCoinImage coin)
    {
        if (coin.value == currentTargetValue)
        {
            currWaterCoin = coin;
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
        else if(currCoin.name == "Coin2")
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
        else
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
            Tiger.TigerAway();
            yield return new WaitForSeconds(.4f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;
        }
        else
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
            Tiger.TigerAway();
            yield return new WaitForSeconds(.4f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;
        }

        yield return new WaitForSeconds(.5f);

        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.5f);
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

        // update SIS
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_3)
        {
            // first time losing
            if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame)
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = true;
            else
            {
                // every other time losing
                if (!StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
                {
                    StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = true;
                }
            }
            StudentInfoSystem.SaveStudentPlayerData();
        }

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
