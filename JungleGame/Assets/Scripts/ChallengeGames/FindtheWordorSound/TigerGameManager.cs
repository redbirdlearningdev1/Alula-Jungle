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

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    [Header("Testing")] // ache -> bake
    public bool overrideWord;
    // //public ChallengeWord testChallengeWord;
    // public ChallengeWord targetChallengeWord1;
    // public ChallengeWord targetChallengeWord2;
    // public ChallengeWord targetChallengeWord3;
    // public ChallengeWord targetChallengeWord4;
    // public ChallengeWord targetChallengeWord5;
    // public List<ElkoninValue> coinOptions;
    // public int swipeIndex;

    [SerializeField] private Vector2 noCoin;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {
        PregameSetup();
    }


    private void PregameSetup()
    {
        ChallengeWordDatabase.InitCreateGlobalList(true);
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

        // set coin off-screen
        currCoin.transform.position = coinStartPos.position;

        // set base pattern
        pattern.baseState();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {   
        currCoin.transform.position = coinStartPos.position;

        if (overrideWord)
        {
            // currentWord1 = targetChallengeWord1;
            // currentWord2 = targetChallengeWord2;
            // currentWord3 = targetChallengeWord3;
            // currentWord4 = targetChallengeWord4;
            // currentWord5 = targetChallengeWord5;
            // polaroidC[0].SetPolaroid(currentWord1);
            // polaroidC[1].SetPolaroid(currentWord2);
            // polaroidC[2].SetPolaroid(currentWord3);
            // polaroidC[3].SetPolaroid(currentWord4);
            // polaroidC[4].SetPolaroid(currentWord5);
        }
        else
        {
            List<ActionWordEnum> globalList = GameManager.instance.GetGlobalActionWordList();
            currSet = globalList[Random.Range(0, globalList.Count)];
            print ("set: " + currSet);
            
            // get challenge words from a set
            List<ChallengeWord> word_pool = new List<ChallengeWord>();
            word_pool.AddRange(ChallengeWordDatabase.GetChallengeWordSet(currSet));
            print ("word pool: " + word_pool.Count);

            // get all other challenge words (from other sets)
            List<ChallengeWord> global_pool = new List<ChallengeWord>();
            global_pool.AddRange(ChallengeWordDatabase.globalChallengeWordList);
            foreach (var word in word_pool)
            {
                global_pool.Remove(word);
            }

            // determine current word
            currWord = word_pool[Random.Range(0, word_pool.Count)];
            // determine correct index
            int correctindex = Random.Range(0, polaroidC.Count);

            for (int i = 0; i < polaroidC.Count; i++)
            {
                if (i == correctindex)
                {
                    polaroidC[i].SetPolaroid(currWord);
                }
                else
                {
                    ChallengeWord otherWord = global_pool[Random.Range(0, global_pool.Count)];
                    polaroidC[i].SetPolaroid(otherWord);
                    global_pool.Remove(otherWord);
                }
            }
        }
        yield return new WaitForSeconds(1f);

        pattern.baseState();
        yield return new WaitForSeconds(0.5f);
        Tiger.TigerDeal();
        yield return new WaitForSeconds(.6f);
        currCoin.gameObject.transform.position = coinLandPos.position;
        currCoin.SetActionWordValue(currSet);
        yield return new WaitForSeconds(.5f);

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

        // disable raycaster
        TigerGameRaycaster.instance.isOn = true;
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
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);

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
        // disable raycaster
        TigerGameRaycaster.instance.isOn = false;

        if (Photo.challengeWord.set == currSet)
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
        if (win)
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            pattern.correct();
            correctCoins[numWins].SetActive(true);
            numWins++;
            Tiger.TigerAway();
            yield return new WaitForSeconds(.55f);
            currCoin.gameObject.transform.position = coinStartPos.position;
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
            currCoin.gameObject.transform.position = coinStartPos.position;
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

        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

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
