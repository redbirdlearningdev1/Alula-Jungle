using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerCoinGameManager : MonoBehaviour
{
    public static TigerCoinGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    //[SerializeField] private CoinChoice coinC;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private Poloroid poloroidC;
    [SerializeField] private GameObject pics;
    [SerializeField] private TigerCoinRayCaster caster;


    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;




    [SerializeField] private List<CoinChoices> coins;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;


    private List<CoinChoices> allCoins = new List<CoinChoices>();
    private int selectedIndex;
    public CoinChoices selectedCoin;

    private int winCount = 0;
    private int loseCount = 0;

    private bool firstTimePlaying = true;


    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;


        }

        // dev object stuff
        //devObject.SetActive(GameManager.instance.devModeActivated);

        PregameSetup();
        StartCoroutine(StartGame(0));
        //StartCoroutine(StartGame(0));
    }

    void Update()
    {

    }



    public bool EvaluateSelectedCoins(ActionWordEnum coinz, CoinChoices correctCoin)
    {
        //Debug.Log(selectedSpiderCoin.type);
        if (coinz == selectedCoin.type)
        {
            // success! go on to the next row or win game if on last row
            Debug.Log("YOU DID IT");
            if (winCount < 3)
            {
                winCount++;
                pattern.correct();
                StartCoroutine(CoinSuccessRoutine(correctCoin));
            }
            else
            {
                Debug.Log("YOU DID IT AGAIn");
                StartCoroutine(WinRoutine());
            }

            return true;
        }
        loseCount++;
        pattern.incorrect();
        if (loseCount == 3)
        {
            StartCoroutine(WinRoutine());
        }
        StartCoroutine(CoinFailRoutine());

        return false;
    }


    private IEnumerator CoinFailRoutine()
    {


        Tiger.TigerAway();
        yield return new WaitForSeconds(.5f);
        poloroidC.ToggleVisibility(false, false);
        pics.SetActive(false);
        yield return new WaitForSeconds(.15f);
        if (loseCount - 1 == 0)
        {
            incorrectCoins[0].SetActive(true);
        }
        else if (loseCount - 1 == 1)
        {
            incorrectCoins[1].SetActive(true);
        }
        else if (loseCount - 1 == 2)
        {
            incorrectCoins[2].SetActive(true);
        }
        poloroidC.ToggleVisibility(false, false);
        pics.SetActive(false);
        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.65f);
        List<CoinChoices> coinZ = GetCoins(0);
        coinZ[0].PoloroindMovePos1Last();
        yield return new WaitForSeconds(.02f);
        coinZ[1].PoloroindMovePos2Last();
        yield return new WaitForSeconds(.02f);
        coinZ[2].PoloroindMovePos3Last();
        yield return new WaitForSeconds(.02f);
        coinZ[3].PoloroindMovePos4Last();

        yield return new WaitForSeconds(1.25f);
        foreach (var coin in coinZ)
        {
            coin.PoloroidMoveOut();
        }
        yield return new WaitForSeconds(1.5f);
        //pics[0].SetActive(false);
        //pics[1].SetActive(false);
        //pics[2].SetActive(false);
        //pics[3].SetActive(false);
        //pics[4].SetActive(false);
        foreach (var coin in coinZ)
        {

            coin.ToggleVisibility(false, false);

            coin.PoloroidMoveBase();


        }


        yield return new WaitForSeconds(.5f);

        StartCoroutine(StartGame(0));
    }

    private IEnumerator CoinSuccessRoutine(CoinChoices coin)
    {
        if (winCount - 1 == 0)
        {
            correctCoins[0].SetActive(true);
        }
        else if (winCount - 1 == 1)
        {
            correctCoins[1].SetActive(true);
        }
        else if (winCount - 1 == 2)
        {
            correctCoins[2].SetActive(true);
        }
        poloroidC.ToggleVisibility(false, false);
        pics.SetActive(false);
        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.65f);
        List<CoinChoices> coinZ = GetCoins(0);
        coinZ[0].PoloroindMovePos1Last();
        yield return new WaitForSeconds(.02f);
        coinZ[1].PoloroindMovePos2Last();
        yield return new WaitForSeconds(.02f);
        coinZ[2].PoloroindMovePos3Last();
        yield return new WaitForSeconds(.02f);
        coinZ[3].PoloroindMovePos4Last();

        yield return new WaitForSeconds(1.25f);
        foreach (var coinN in coinZ)
        {
            coinN.PoloroidMoveOut();
        }
        yield return new WaitForSeconds(1.5f);
        //pics[0].SetActive(false);
        //pics[1].SetActive(false);
        //pics[2].SetActive(false);
        //pics[3].SetActive(false);
        //pics[4].SetActive(false);
        foreach (var coinN in coinZ)
        {

            coinN.ToggleVisibility(false, false);

            coinN.PoloroidMoveBase();


        }


        yield return new WaitForSeconds(.5f);

        StartCoroutine(StartGame(0));
    }


    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private void PregameSetup()
    {
        // create coin list

        foreach (var coin in coins)
        {
            //Debug.Log(coin);
            allCoins.Add(coin);
        }


        // Create Global Coin List
        globalCoinPool = GameManager.instance.GetGlobalActionWordList();
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins
        poloroidC.ToggleVisibility(false, false);
        pics.SetActive(false);






    }

    private void walkThrough()
    {

    }



    private IEnumerator StartGame(int index)
    {
        pattern.baseState();
        StartCoroutine(ShowCoins(0));
        Tiger.TigerDeal();
        yield return new WaitForSeconds(.3f);
        pics.SetActive(true);
        poloroidC.ToggleVisibility(true, true);
        yield return new WaitForSeconds(.45f);

        List<CoinChoices> coinZ = GetCoins(index);
        foreach (var coin in coinZ)
        {

            coin.PoloroidMoveIn();



        }
        yield return new WaitForSeconds(.6f);
        coinZ[0].PoloroindMovePos1();
        coinZ[1].PoloroindMovePos2();
        coinZ[2].PoloroindMovePos3();
        coinZ[3].PoloroindMovePos4();
        coinZ[4].PoloroindMovePos5();










        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowCoins(int index)
    {
        Debug.Log("ShowCoins???????????????");
        List<CoinChoices> currentCoins = GetCoins(index);
        foreach (var coin in currentCoins)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
            {
                unusedCoinPool.AddRange(globalCoinPool);
            }
            ActionWordEnum type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            coin.SetCoinType(type);
            //pics[0].SetActive(true);
            //pics[1].SetActive(true);
            //pics[2].SetActive(true);
            //pics[3].SetActive(true);
            //pics[4].SetActive(true);
            coin.ToggleVisibility(true, true);
            //Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }

        SelectRandomPoloroid(index);
    }



    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<CoinChoices> row = GetCoins(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomPoloroid(int index)
    {
        List<CoinChoices> pile = GetCoins(index);
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
        selectedCoin = pile[selectedIndex];
        poloroidC.SetCoinType(selectedCoin.type);


    }

    private List<CoinChoices> GetCoins(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return coins;


        }
    }
}
