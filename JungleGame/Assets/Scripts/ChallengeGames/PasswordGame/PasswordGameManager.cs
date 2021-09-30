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

    private bool gameSetup = false;




    [Header("Animators")]
    [SerializeField] private Animator tiger;
    [SerializeField] private Animator monkey1;
    [SerializeField] private Animator monkey2;
    [SerializeField] private Animator wheel;
    [SerializeField] private Animator unlock;
    [SerializeField] private Animator win1;
    [SerializeField] private Animator win2;
    [SerializeField] private Animator win3;


    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;
    // water coins
    [SerializeField] private Vector2 waterNormalCoinSize;
    [SerializeField] private Vector2 waterExpandedCoinSize;

    [Header("Positions")]
    [SerializeField] private GameObject BG1;
    [SerializeField] private GameObject BG2;
    [SerializeField] private GameObject Tiger;

    [SerializeField] private GameObject monkey1Obj;
    [SerializeField] private GameObject monkey2Obj;

    [SerializeField] private Transform BG1Pos;
    [SerializeField] private Transform BG2Pos;
    [SerializeField] private Transform Ahead;
    [SerializeField] private Transform TigerPos1;
    [SerializeField] private Transform TigerPos2;
    [SerializeField] private Transform monkey1Pos1;
    [SerializeField] private Transform monkey1Pos2;
    [SerializeField] private Transform monkey1Pos3;
    [SerializeField] private Transform monkey2Pos1;
    [SerializeField] private Transform monkey2Pos2;
    [SerializeField] private Transform monkey2Pos3;
    [SerializeField] private Transform LockPos1;
    [SerializeField] private Transform LockPos2;
    [SerializeField] private Transform LockPos3;
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform CoinPos1;
    [SerializeField] private Transform CoinPos2;
    [SerializeField] private Transform CoinPos3;
    [SerializeField] private Transform CoinPos4;
    [SerializeField] private Transform CoinPos5;
    [SerializeField] private Transform CoinPos6;
    [SerializeField] private Transform CoinPosIn1;
    [SerializeField] private Transform CoinPosIn2;
    [SerializeField] private Transform CoinPosIn3;
    [SerializeField] private Transform CoinPosIn4;
    [SerializeField] private Transform CoinPosIn5;
    [SerializeField] private Transform CoinPosIn6;
    [SerializeField] private Transform CoinPosEnd1;
    [SerializeField] private Transform CoinPosEnd2;
    [SerializeField] private Transform CoinPosEnd3;
    [SerializeField] private Transform CoinPosEnd4;
    [SerializeField] private Transform CoinPosEnd5;
    [SerializeField] private Transform CoinPosEnd6;
    [SerializeField] private Transform CoinStartPos;
    [SerializeField] private Transform CoinEndPos;

    // coins in position or out
    private bool In1 = false;
    private bool In2 = false;
    private bool In3 = false;
    private bool In4 = false;
    private bool In5 = false;
    private bool In6 = false;

    // tab position
    [SerializeField] private Transform TabIn1;
    [SerializeField] private Transform TabIn2;
    [SerializeField] private Transform TabIn3;
    [SerializeField] private Transform TabIn4;
    [SerializeField] private Transform TabIn5;
    [SerializeField] private Transform TabIn6;
    //coin animators
    [SerializeField] private GameObject CoinAnim1;
    [SerializeField] private GameObject CoinAnim2;
    [SerializeField] private GameObject CoinAnim3;
    [SerializeField] private GameObject CoinAnim4;
    [SerializeField] private GameObject CoinAnim5;
    [SerializeField] private GameObject CoinAnim6;
    [SerializeField] private Sprite Blank;

    [SerializeField] private GameObject LockObject;
    [SerializeField] private Sprite Lock1;
    [SerializeField] private Sprite Lock2;
    [SerializeField] private Sprite Lock3;
    [SerializeField] private Sprite Lock4;




    [SerializeField] private List<UniversalCoinImage> waterCoins;
    [SerializeField] private List<GameObject> tabs;
    private UniversalCoinImage currWaterCoin;

    // other variables
    private ChallengeWord currentWord;
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
    public ChallengeWord targetChallengeWord;
    public List<ElkoninValue> coinOptions;
    public int swipeIndex;

    public int numberLockedIn = 0;


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
        //LockObject.SetActive(true);
        In1 = false;
        In2 = false;
        In3 = false;
        In4 = false;
        In5 = false;
        In6 = false;
        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        if (overrideWord)
        {
            currentWord = targetChallengeWord;
            polaroidC.SetPolaroid(currentWord);
        }
        polaroidC.LerpScale(1f, 1f);
        
        yield return new WaitForSeconds(.2f);

        StartCoroutine(LerpMoveObject(polaroidC.gameObject.transform, polaroidLandPos.position, 1f));
        yield return new WaitForSeconds(.45f);
        for (int i = 0; i < 6; i++)
        {

            waterCoins[i].SetValue(coinOptions[i]);
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinStartPos.position, 0f));
            waterCoins[i].ToggleGlowOutline(false);
        }
        CoinAnim1.GetComponent<Animator>().enabled = true;
        CoinAnim2.GetComponent<Animator>().enabled = true;
        CoinAnim3.GetComponent<Animator>().enabled = true;
        CoinAnim4.GetComponent<Animator>().enabled = true;
        CoinAnim5.GetComponent<Animator>().enabled = true;

        StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPos1.position, .5f));

        yield return new WaitForSeconds(.11f);

            StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPos2.position, .5f));

        yield return new WaitForSeconds(.11f);

            StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPos3.position, .5f));

        yield return new WaitForSeconds(.11f);


            StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPos4.position, .5f));

        yield return new WaitForSeconds(.11f);


            StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPos5.position, .5f));

        yield return new WaitForSeconds(.11f);


            StartCoroutine(LerpMoveObject(waterCoins[5].transform, CoinPos6.position, .5f));



        yield return new WaitForSeconds(.6f);


        currentTargetWord = targetChallengeWord;
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

        // check lists
        if (waterCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
        }
        // water coins
        else if (waterCoins.Contains(coin))
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
        // water coin differential
        //if (!waterCoin) coin.LerpSize(expandedCoinSize, 0.25f);
        //else coin.LerpSize(waterExpandedCoinSize, 0.25f);
        yield return new WaitForSeconds(0f);
        // return if current water coin


        // water coin differential
        //if (!waterCoin) coin.LerpSize(normalCoinSize, 0.25f);
        //else coin.LerpSize(waterNormalCoinSize, 0.25f);



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

    public void EvaluateCoinLockIn(Polaroid polar)
    {
        if(In1 && !In2 && !In3 && !In4 && !In5 && !In6)
        {
            numberLockedIn = 1;
        }
        else if (In1 && In2 && !In3 && !In4 && !In5 && !In6)
            {
            numberLockedIn = 2;
        }
        else if (In1 && In2 && In3 && !In4 && !In5 && !In6)
                {
            numberLockedIn = 3;
        }
        else if (In1 && In2 && In3 && In4 && !In5 && !In6)
                    {
            numberLockedIn = 4;
        }
        else if (In1 && In2 && In3 && In4 && In5 && !In6)
        {
            numberLockedIn = 5;
        }
        else if (In1 && In2 && In3 && In4 && In5 && In6)
        {
            numberLockedIn = 6;
        }
        if (numberLockedIn == polar.challengeWord.elkoninCount)
        {

            Debug.Log("You WIN");

            StartCoroutine(PostRound(true));
        }
        else
        {
            Debug.Log("You Lose");
            StartCoroutine(PostRound(false));
        }

        // return water coins
        //WordFactorySubstitutingManager.instance.ResetWaterCoins();
    }

    public void returnToPos(GameObject currCoin)
    {

        currCoin.gameObject.transform.SetParent(selectedObjectParentCoin);
        if (currCoin.name == "PolaroidObject")
        {
            StartCoroutine(LerpMoveObject(polaroidC.gameObject.transform, polaroidLandPos.position, .2f));
        }




    }
    public void SlotIn(GameObject currCoin)
    {

        currCoin.gameObject.transform.SetParent(selectedObjectParentCoin);

        if (currCoin.name == "Coin1")
        {
            if(!In1)
            {

                CoinAnim1.GetComponent<Animator>().enabled = false;
                CoinAnim1.GetComponent<SpriteRenderer>().sprite = Blank;

                In1 = true;
                StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPosIn1.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[0].transform, CoinPosIn1.position, .2f));
            }
            else
            {

                CoinAnim1.GetComponent<Animator>().enabled = true;
                In1 = false;
                StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPos1.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[0].transform, TabIn1.position, .2f));
            }
        }
        else if (currCoin.name == "Coin2")
        {
            if (!In2)
            {

                CoinAnim2.GetComponent<Animator>().enabled = false;
                CoinAnim2.GetComponent<SpriteRenderer>().sprite = Blank;
                In2 = true;
                StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPosIn2.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[1].transform, CoinPosIn2.position, .2f));
            }
            else
            {

                CoinAnim2.GetComponent<Animator>().enabled = true;
                In2 = false;
                StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPos2.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[1].transform, TabIn2.position, .2f));
            }
        }
        else if (currCoin.name == "Coin3")
        {
            if (!In3)
            {

                CoinAnim3.GetComponent<Animator>().enabled = false;
                CoinAnim3.GetComponent<SpriteRenderer>().sprite = Blank;
                In3 = true;
                StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPosIn3.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[2].transform, CoinPosIn3.position, .2f));
            }
            else
            {

                CoinAnim3.GetComponent<Animator>().enabled = true;
                In3 = false;
                StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPos3.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[2].transform, TabIn3.position, .2f));
            }
        }
        else if (currCoin.name == "Coin4")
        {
            if (!In4)
            {

                CoinAnim4.GetComponent<Animator>().enabled = false;
                CoinAnim4.GetComponent<SpriteRenderer>().sprite = Blank;
                In4 = true;
                StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPosIn4.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[3].transform, CoinPosIn4.position, .2f));
            }
            else
            {

                CoinAnim4.GetComponent<Animator>().enabled = true;
                In4 = false;
                StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPos4.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[3].transform, TabIn4.position, .2f));
            }
        }
        else if (currCoin.name == "Coin5")
        {
            if (!In5)
            {

                CoinAnim5.GetComponent<Animator>().enabled = false;
                CoinAnim5.GetComponent<SpriteRenderer>().sprite = Blank;
                In5 = true;
                StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPosIn5.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[4].transform, CoinPosIn5.position, .2f));
            }
            else
            {

                CoinAnim5.GetComponent<Animator>().enabled = true;
                In5 = false;
                StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPos5.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[4].transform, TabIn5.position, .2f));
            }
        }
        else
        {
            if (!In6)
            {

                CoinAnim6.GetComponent<Animator>().enabled = false;
                CoinAnim6.GetComponent<SpriteRenderer>().sprite = Blank;
                In6 = true;
                StartCoroutine(LerpMoveObject(waterCoins[5].transform, CoinPosIn6.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[5].transform, CoinPosIn6.position, .2f));
            }
            else
            {

                CoinAnim6.GetComponent<Animator>().enabled = true;
                In6 = false;
                StartCoroutine(LerpMoveObject(waterCoins[5].transform, CoinPos6.position, .2f));
                StartCoroutine(LerpMoveObject(tabs[5].transform, TabIn6.position, .2f));
            }
        }
    }

    private IEnumerator PostRound(bool win)
    {
        Debug.Log(numberLockedIn);
        polaroidC.LerpScale(0f, 0f);
        if (win)
        {
            
            Debug.Log("goodjob");

            numMisses = 0;
            numWins++;
            if(numWins == 1)
            {
                win1.Play("Winning1");
            }
            else if (numWins == 2)
            {
                win2.Play("Winning2");
            }
            else if (numWins == 3)
            {
                win3.Play("Winning3");
            }
            polaroidC.LerpScale(0f, 0f);
            yield return new WaitForSeconds(.65f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;

        }
        else
        {
            Debug.Log("job");
            LockObject.GetComponent<Animator>().enabled = false;
            numMisses++;
            if(numMisses == 1)
            {
                LockObject.GetComponent<SpriteRenderer>().sprite = Lock2;
                Debug.Log("HERE");
            }
            else if (numMisses == 2)
            {
                LockObject.GetComponent<SpriteRenderer>().sprite = Lock3;
            }
            else if (numMisses == 3)
            {
                LockObject.GetComponent<SpriteRenderer>().sprite = Lock4;
            }


            yield return new WaitForSeconds(.65f);
            polaroidC.gameObject.transform.position = polaroidStartPos.position;
        }


        yield return new WaitForSeconds(.15f);
        if(!In1)
        {
            StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPosEnd1.position, .5f));
        }
        if (!In2)
        {
            StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPosEnd2.position, .5f));
        }
        if (!In3)
        {
            StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPosEnd3.position, .5f));
        }
        if (!In4)
        {
            StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPosEnd4.position, .5f));
        }
        if (!In5)
        {
            StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPosEnd5.position, .5f));
        }
        if (!In6)
        {
            StartCoroutine(LerpMoveObject(waterCoins[5].transform, CoinPosEnd6.position, .5f));
        }
        //Coins that are in
        if (win)
        {
            if (In1)
            {
                waterCoins[0].ToggleGlowOutline(true);

            }
            if (In2)
            {
                waterCoins[1].ToggleGlowOutline(true);

            }
            if (In3)
            {
                waterCoins[2].ToggleGlowOutline(true);
            }
            if (In4)
            {
                waterCoins[3].ToggleGlowOutline(true);
            }
            if (In5)
            {
                waterCoins[4].ToggleGlowOutline(true);

            }
            if (In6)
            {
                waterCoins[5].ToggleGlowOutline(true);


            }
            yield return new WaitForSeconds(.5f);
            LockObject.GetComponent<Animator>().enabled = true;
            unlock.Play("UnlockAnim");
            tiger.Play("tigerLose");
            monkey1.Play("MarcusLose");
            monkey2.Play("BrutusLose");
            yield return new WaitForSeconds(.75f);

            //LockObject.SetActive(false);
            tiger.Play("TigerAngry");
            yield return new WaitForSeconds(.35f);
            StartCoroutine(LerpMoveObject(Tiger.transform, TigerPos2.position, 2f));
            yield return new WaitForSeconds(1f);
            StartCoroutine(LerpMoveObject(Tiger.transform, TigerPos2.position, 0f));
            tiger.Play("aTigerIdle");
        }
        if(!win)
        {
            tiger.Play("tigerWin");
            monkey1.Play("MarcusWin");
            monkey2.Play("BrutusWin");
            yield return new WaitForSeconds(1f);
     
        }
        
        StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPosEnd1.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[0].transform, TabIn1.position, .2f));
        StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPosEnd2.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[1].transform, TabIn2.position, .2f));
        StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPosEnd3.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[2].transform, TabIn3.position, .2f));
        StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPosEnd4.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[3].transform, TabIn4.position, .2f));
        StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPosEnd5.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[4].transform, TabIn5.position, .2f));
        StartCoroutine(LerpMoveObject(waterCoins[5].transform, CoinPosEnd6.position, .5f));
        StartCoroutine(LerpMoveObject(tabs[5].transform, TabIn6.position, .2f));
        if(win)
        {

            StartCoroutine(LerpMoveObject(BG1.transform, BG1Pos.position, 1f));
            StartCoroutine(LerpMoveObject(BG2.transform, BG2Pos.position, 1f));
            StartCoroutine(LerpMoveObject(Tiger.transform, TigerPos1.position, 1f));
            StartCoroutine(LerpMoveObject(monkey1Obj.transform, monkey1Pos3.position, 1f));
            StartCoroutine(LerpMoveObject(monkey2Obj.transform, monkey2Pos3.position, 1f));
            StartCoroutine(LerpMoveObject(LockObject.transform, LockPos2.position, 1f));
            wheel.Play("PasswordTurn");
            yield return new WaitForSeconds(1f);
            LockObject.GetComponent<Animator>().enabled = false;
            LockObject.GetComponent<SpriteRenderer>().sprite = Lock1;
            StartCoroutine(LerpMoveObject(monkey1Obj.transform, monkey1Pos2.position, 0f));
            StartCoroutine(LerpMoveObject(monkey2Obj.transform, monkey2Pos2.position, 0f));
            StartCoroutine(LerpMoveObject(BG1.transform, BG2Pos.position, 0f));
            StartCoroutine(LerpMoveObject(BG2.transform, Ahead.position, 0f));
            //StartCoroutine(LerpMoveObject(LockObject.transform, LockPos3.position, 0f));
            StartCoroutine(LerpMoveObject(LockObject.transform, LockPos1.position, 0f));
            monkey1.Play("MarcusWalkIn");
            monkey2.Play("BrutusWalkIn");
            StartCoroutine(LerpMoveObject(monkey1Obj.transform, monkey1Pos1.position, 2f));
            StartCoroutine(LerpMoveObject(monkey2Obj.transform, monkey2Pos1.position, 2f));
            yield return new WaitForSeconds(2f);

            monkey1.Play("MarcusBroken");
            monkey2.Play("BrutusBroken");
        }

        numberLockedIn = 0;
        if (numMisses == 3 || numWins == 3)
        {
            WinRoutine();
        }
        StartCoroutine(StartGame(0));
        yield return null;
    }










}
