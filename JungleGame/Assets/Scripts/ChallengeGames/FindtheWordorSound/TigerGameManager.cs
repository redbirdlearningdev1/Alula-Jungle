using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerGameManager : MonoBehaviour
{
    public static TigerGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private CoinChoice coinC;
    [SerializeField] private PatternRightWrong pattern;
    //[SerializeField] private Poloroid poloroidC;
    [SerializeField] private TigerGameRaycaster caster;


    private bool gameSetup = false;

    private List<ActionWordEnum> globalPoloroidPool;
    private List<ActionWordEnum> unusedPoloroidPool;




    [SerializeField] private List<Poloroid> poloroids;
    [SerializeField] private List<GameObject> pics;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;


    private List<Poloroid> allPoloroid = new List<Poloroid>();
    private int selectedIndex;
    public Poloroid selectedPoloroid;

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



    public bool EvaluateSelectedPoloroid(ActionWordEnum polor, Poloroid correctCoin)
    {
        //Debug.Log(selectedSpiderCoin.type);
        if (polor == selectedPoloroid.type)
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
        coinC.ToggleVisibility(false, false);
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
        coinC.ToggleVisibility(false, false);
        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.65f);
        List<Poloroid> poloroidZ = GetPoloroids(0);
        poloroidZ[0].PoloroindMovePos1Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[1].PoloroindMovePos2Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[2].PoloroindMovePos3Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[3].PoloroindMovePos4Last();

        yield return new WaitForSeconds(1.25f);
        foreach (var polor in poloroidZ)
        {
            polor.PoloroidMoveOut();
        }
        yield return new WaitForSeconds(1.5f);
        pics[0].SetActive(false);
        pics[1].SetActive(false);
        pics[2].SetActive(false);
        pics[3].SetActive(false);
        pics[4].SetActive(false);
        foreach (var polor in poloroidZ)
        {

            polor.ToggleVisibility(false, false);

            polor.PoloroidMoveBase();


        }


        yield return new WaitForSeconds(.5f);

        StartCoroutine(StartGame(0));
    }

    private IEnumerator CoinSuccessRoutine(Poloroid coin)
    {
        if(winCount-1 == 0)
        {
            correctCoins[0].SetActive(true);
        }
        else if (winCount-1 == 1)
        {
            correctCoins[1].SetActive(true);
        }
        else if (winCount-1 == 2)
        {
            correctCoins[2].SetActive(true);
        }
        coinC.ToggleVisibility(false, false);
        Tiger.TigerSwipe();
        yield return new WaitForSeconds(.65f);
        List<Poloroid> poloroidZ = GetPoloroids(0);
        poloroidZ[0].PoloroindMovePos1Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[1].PoloroindMovePos2Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[2].PoloroindMovePos3Last();
        yield return new WaitForSeconds(.02f);
        poloroidZ[3].PoloroindMovePos4Last();
 
        yield return new WaitForSeconds(1.25f);
        foreach (var polor in poloroidZ)
        {
            polor.PoloroidMoveOut();
        }
        yield return new WaitForSeconds(1.5f);
        pics[0].SetActive(false);
        pics[1].SetActive(false);
        pics[2].SetActive(false);
        pics[3].SetActive(false);
        pics[4].SetActive(false);
        foreach (var polor in poloroidZ)
        {

            polor.ToggleVisibility(false, false);
            
            polor.PoloroidMoveBase();


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

        foreach (var polor in poloroids)
        {
            //Debug.Log(coin);
            allPoloroid.Add(polor);
        }


        // Create Global Coin List
        globalPoloroidPool = GameManager.instance.GetGlobalActionWordList();
        unusedPoloroidPool = new List<ActionWordEnum>();
        unusedPoloroidPool.AddRange(globalPoloroidPool);

        // disable all coins
        coinC.ToggleVisibility(false, false);




        foreach (var polor in poloroids)
        {
            //coin.gameObject.SetActive(false);
        }



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
        coinC.ToggleVisibility(true, true);
        yield return new WaitForSeconds(.45f);
        List<Poloroid> poloroidZ = GetPoloroids(index);
        foreach (var polor in poloroidZ)
        {

            polor.PoloroidMoveIn();



        }
        yield return new WaitForSeconds(.6f);
        poloroidZ[0].PoloroindMovePos1();
        poloroidZ[1].PoloroindMovePos2();
        poloroidZ[2].PoloroindMovePos3();
        poloroidZ[3].PoloroindMovePos4();
        poloroidZ[4].PoloroindMovePos5();










        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowCoins(int index)
    {
        Debug.Log("ShowCoins???????????????");
        List<Poloroid> currentPoloroids = GetPoloroids(index);
        foreach (var polor in currentPoloroids)
        {
            // set random type
            if (unusedPoloroidPool.Count == 0)
            {
                unusedPoloroidPool.AddRange(globalPoloroidPool);
            }
            ActionWordEnum type = unusedPoloroidPool[Random.Range(0, unusedPoloroidPool.Count)];
            unusedPoloroidPool.Remove(type);

            polor.SetCoinType(type);
            pics[0].SetActive(true);
            pics[1].SetActive(true);
            pics[2].SetActive(true);
            pics[3].SetActive(true);
            pics[4].SetActive(true);
            polor.ToggleVisibility(true, true);
            //Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }

        SelectRandomPoloroid(index);
    }

    private IEnumerator CoinsUp(int index)
    {
        //Debug.Log("ShowCoins");
        List<Poloroid> coins = GetPoloroids(index);
        foreach (var coin in coins)
        {


            yield return new WaitForSeconds(0f);

        }
       // SelectRandomCoin(index);
    }
    private IEnumerator CoinsDown(int index)
    {
        //Debug.Log("ShowCoins");
        List<Poloroid> coins = GetPoloroids(index);
        foreach (var coin in coins)
        {

            yield return new WaitForSeconds(0f);

        }
        //SelectRandomCoin(index);
    }


    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<Poloroid> row = GetPoloroids(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomPoloroid(int index)
    {
        List<Poloroid> pile = GetPoloroids(index);
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
        selectedPoloroid = pile[selectedIndex];
        coinC.SetCoinType(selectedPoloroid.type);


    }

    private List<Poloroid> GetPoloroids(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return poloroids;


        }
    }
}
