using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordGameManager : MonoBehaviour
{
    public static PasswordGameManager instance;


    [SerializeField] private Polaroid polaroidC;
    [SerializeField] private PasswordRaycaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;
    private bool gameSetup = false;






    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;
    // water coins
    [SerializeField] private Vector2 waterNormalCoinSize;
    [SerializeField] private Vector2 waterExpandedCoinSize;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform CoinPos1;
    [SerializeField] private Transform CoinPos2;
    [SerializeField] private Transform CoinPos3;
    [SerializeField] private Transform CoinPos4;
    [SerializeField] private Transform CoinPos5;
    [SerializeField] private Transform CoinPos6;
    [SerializeField] private Transform CoinStartPos;
    [SerializeField] private Transform CoinEndPos;

    [SerializeField] private List<UniversalCoin> waterCoins;
    private UniversalCoin currWaterCoin;

    // other variables
    private ChallengeWord currentWord;
    private ElkoninValue currentTargetValue;
    private ChallengeWord currentTargetWord;
    private int currentSwipeIndex;

    private List<UniversalCoin> currentCoins;
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
        StartCoroutine(StartGame(0));

    }

    void Update()
    {

    }





    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }



    private void PregameSetup()
    {

        ChallengeWordDatabase.InitCreateGlobalList();
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

    }

    private void walkThrough()
    {

    }



    private IEnumerator StartGame(int index)
    {

       
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

    public void GlowAndPlayAudioCoin(UniversalCoin coin)
    {
        if (playingCoinAudio)
            return;

        // check lists
        if (currentCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
        }
        // water coins
        else if (waterCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin, true));
        }
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoin coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        // water coin differential
        if (!waterCoin) coin.LerpSize(expandedCoinSize, 0.25f);
        else coin.LerpSize(waterExpandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1.5f);
        // return if current water coin
        if (coin == currWaterCoin)
        {
            coin.ToggleGlowOutline(false);
            playingCoinAudio = false;
            yield break;
        }

        // water coin differential
        if (!waterCoin) coin.LerpSize(normalCoinSize, 0.25f);
        else coin.LerpSize(waterNormalCoinSize, 0.25f);

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

    public void EvaluateWaterCoin(UniversalCoin coin)
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

        // return water coins
        //WordFactorySubstitutingManager.instance.ResetWaterCoins();
    }

    public void returnToPos(GameObject currCoin)
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
        else
        {
            StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPos5.position, .2f));
        }

    }

    private IEnumerator PostRound(bool win)
    {

        if (win)
        {
            Debug.Log("goodjob");

            correctCoins[numWins].SetActive(true);
            numWins++;

            yield return new WaitForSeconds(.65f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;

        }
        else
        {
            Debug.Log("job");

            incorrectCoins[numMisses].SetActive(true);
            numMisses++;

            yield return new WaitForSeconds(.65f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;
        }


        yield return new WaitForSeconds(.45f);
        for (int i = 0; i < 1; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos2.position, .2f));
        }
        yield return new WaitForSeconds(.15f);
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos3.position, .2f));
        }
        yield return new WaitForSeconds(.15f);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos4.position, .2f));
        }
        yield return new WaitForSeconds(.15f);
        for (int i = 0; i < 4; i++)
        {

            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos5.position, .2f));
        }
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < 5; i++)
        {

            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinEndPos.position, .2f));
        }


        if (numMisses == 3 || numWins == 3)
        {
            WinRoutine();
        }
        StartCoroutine(StartGame(0));
        yield return null;
    }










}
