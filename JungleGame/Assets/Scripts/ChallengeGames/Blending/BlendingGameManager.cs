using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendingGameManager : MonoBehaviour
{
    public static BlendingGameManager instance;


    [SerializeField] private BlendTigerController tiger;
    [SerializeField] private BlendingRedController Red;
    [SerializeField] private BlendingRayCaster caster;
    [SerializeField] private FrameController frames;
    //[SerializeField] private BPoloroid PoloroidChoice;
    [SerializeField] private BlendCoin coinButtons;


    private bool gameSetup = false;

    private List<ActionWordEnum> globalPoloroidPool;
    private List<ActionWordEnum> unusedPoloroidPool;




    [SerializeField] private List<BPoloroid> poloroids;
    [SerializeField] private List<GameObject> pics;
    [SerializeField] private List<GameObject> correctCards;
    [SerializeField] private List<GameObject> incorrectCards;


    private List<BPoloroid> allPoloroid = new List<BPoloroid>();
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

        if (loseCount == 3)
        {
            StartCoroutine(WinRoutine());
        }
        StartCoroutine(CoinFailRoutine());

        return false;
    }


    private IEnumerator CoinFailRoutine()
    {



        yield return new WaitForSeconds(.5f);

        StartCoroutine(StartGame(0));
    }

    private IEnumerator CoinSuccessRoutine(Poloroid coin)
    {


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
        StartCoroutine(ShowCoins(0));
        yield return new WaitForSeconds(.3f);

        yield return new WaitForSeconds(.45f);
        List<BPoloroid> poloroidZ = GetPoloroids(index);
        foreach (var polor in poloroidZ)
        {

        }
        yield return new WaitForSeconds(.6f);











        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ShowCoins(int index)
    {
        Debug.Log("ShowCoins???????????????");
        List<BPoloroid> currentPoloroids = GetPoloroids(index);
        foreach (var polor in currentPoloroids)
        {
            // set random type
            if (unusedPoloroidPool.Count == 0)
            {
                unusedPoloroidPool.AddRange(globalPoloroidPool);
            }
            ActionWordEnum type = unusedPoloroidPool[Random.Range(0, unusedPoloroidPool.Count)];
            unusedPoloroidPool.Remove(type);

            //polor.SetCoinType(type);
            pics[0].SetActive(true);
            pics[1].SetActive(true);
            pics[2].SetActive(true);
            pics[3].SetActive(true);
            pics[4].SetActive(true);
            //polor.ToggleVisibility(true, true);
            //Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }

        SelectRandomPoloroid(index);
    }

    private IEnumerator CoinsUp(int index)
    {
        //Debug.Log("ShowCoins");
        List<BPoloroid> coins = GetPoloroids(index);
        foreach (var coin in coins)
        {


            yield return new WaitForSeconds(0f);

        }
        // SelectRandomCoin(index);
    }
    private IEnumerator CoinsDown(int index)
    {
        //Debug.Log("ShowCoins");
        List<BPoloroid> coins = GetPoloroids(index);
        foreach (var coin in coins)
        {

            yield return new WaitForSeconds(0f);

        }
        //SelectRandomCoin(index);
    }


    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<BPoloroid> row = GetPoloroids(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                //coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomPoloroid(int index)
    {
        List<BPoloroid> pile = GetPoloroids(index);
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
        //selectedPoloroid = pile[selectedIndex];
        //coinC.SetCoinType(selectedPoloroid.type);


    }

    private List<BPoloroid> GetPoloroids(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return poloroids;


        }
    }
}
