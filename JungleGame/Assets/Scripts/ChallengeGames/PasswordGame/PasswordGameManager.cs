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
    [SerializeField] private List<Transform> CoinPosList;
    [SerializeField] private List<Transform> CoinPosInList;
    [SerializeField] private List<Transform> TabsOut;
    [SerializeField] private Transform CoinPosEnd1;
    [SerializeField] private Transform CoinPosEnd2;
    [SerializeField] private Transform CoinPosEnd3;
    [SerializeField] private Transform CoinPosEnd4;
    [SerializeField] private Transform CoinPosEnd5;
    [SerializeField] private Transform CoinPosEnd6;
    [SerializeField] private Transform CoinStartPos;
    [SerializeField] private Transform CoinEndPos;
    [SerializeField] private List<Transform> CoinEndList;

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
    [SerializeField] private List<GameObject> CoinAnims;
    [SerializeField] private Sprite Blank;

    [SerializeField] private GameObject LockObject;
    [SerializeField] private Sprite Lock1;
    [SerializeField] private Sprite Lock2;
    [SerializeField] private Sprite Lock3;
    [SerializeField] private Sprite Lock4;



    [SerializeField] private List<bool> CoinIns;
    [SerializeField] private List<bool> CoinOuts;
    [SerializeField] private List<bool> WhichCoin;
    [SerializeField] private List<UniversalCoinImage> waterCoins;
    [SerializeField] private List<GameObject> tabs;
    [SerializeField] private List<UniversalCoinImage> LastCoin;
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
    private int numIn;
    private int removePos;

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
            //waterCoins[i].SetLayer(2);
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

            //StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));

        // water coins

            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin, true));

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
        playingCoinAudio = false;
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

        if (numIn == polar.challengeWord.elkoninCount)
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
    public void SlotIn(UniversalCoinImage currCoin, GameObject currCoinAnim)
    {

        currCoin.gameObject.transform.SetParent(selectedObjectParentCoin);

        for(int i = 5; i >= 0+numIn; i--)
        {

                CoinOuts[i] = true;


        }


        if (currCoin.name == "CoinOut")
        {
            LastCoin.Add(currCoin);
            for(int i = 0; i<6;i++)
            {
                if(CoinOuts[i] == true)
                {
                    CoinOuts[i] = false;
                    numIn++;
                    break;
                }
            }

            currCoin.name = "CoinIn";
            currCoinAnim.GetComponentInChildren<Animator>().Play("_blankPerm"); 
            //currCoinAnim.GetComponentInChildren<Animator>().enabled = false;
            //currCoinAnim.GetComponentInChildren<SpriteRenderer>().sprite = Blank;
            
            for (int i = 0; i < 6; i++)
            {
                if(CoinIns[i] == false)
                {
                    StartCoroutine(LerpMoveObject(currCoin.transform, CoinPosInList[i].position, .2f));
                    StartCoroutine(LerpMoveObject(tabs[i].transform, CoinPosInList[i].position, .2f));
                    break;
                }
            }
            for (int i = 0; i < numIn; i++)
            {
                CoinIns[i] = true;
            }
            for (int j = 0; j < waterCoins.Count; j++)
            {


                    if(waterCoins[j].name == currCoin.name)
                    {
                        break;
                    }
                    else
                    {
                        removePos++;
                    }
                
            }

            Debug.Log(removePos);
            waterCoins.Remove(currCoin);
            currCoinAnim.GetComponentInChildren<SpriteRenderer>().sprite = Blank;
        }
        else if(currCoin.name == "CoinIn")
        {
            LastCoin[numIn - 1].GetComponentInChildren<Animator>().Play("_blank");
            
            StartCoroutine(LerpMoveObject(LastCoin[numIn-1].transform, CoinPosList[numIn-1].position, .2f));
            StartCoroutine(LerpMoveObject(tabs[numIn-1].transform, TabsOut[numIn-1].position, .2f));

            CoinIns[numIn - 1] = false;
            CoinOuts[numIn - 1] = true;
            LastCoin[numIn- 1].name = "CoinOut";
            waterCoins.Insert(0, LastCoin[numIn - 1]);
            LastCoin.RemoveAt(numIn-1);
            numIn--;
            
        }
        for(int i = 0; i < removePos; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPosList[i+numIn].position, .2f));
        }
        removePos = 0;

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
        for(int i = 0; i < 6-numIn; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinEndList[i+numIn].position, .5f));
        }
        
        //Coins that are in
        if (win)
        {
            for(int i = 0; i < numIn; i++)
            {
                LastCoin[i].SetValue(polaroidC.challengeWord.elkoninList[i]);
                LastCoin[i].GetComponentInChildren<Animator>().Play(polaroidC.challengeWord.elkoninList[i].ToString()+"Coin");
                //LastCoin[i].ToggleGlowOutline(true);
                GlowAndPlayAudioCoin(LastCoin[i]);
                yield return new WaitForSeconds(1f);
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

        for(int i = 0; i < numIn; i++)
        {
            StartCoroutine(LerpMoveObject(LastCoin[i].transform, CoinEndList[i].position, .5f));
            StartCoroutine(LerpMoveObject(tabs[i].transform, TabsOut[i].position, .2f));
        }
        int tempNum = numIn;
        for (int i = 0; i < tempNum; i++)
        {
            LastCoin[numIn - 1].name = "CoinOut";
            waterCoins.Insert(0, LastCoin[numIn - 1]);

            LastCoin.RemoveAt(numIn - 1);
            CoinIns[i] = false;
            numIn--;
        }
        if (win)
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
