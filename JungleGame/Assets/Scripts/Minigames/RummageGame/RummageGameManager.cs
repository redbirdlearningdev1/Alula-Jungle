using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RummageGameManager : MonoBehaviour
{
    public static RummageGameManager instance;

    [SerializeField] private OrcController orc;
    [SerializeField] private chest chest;
    [SerializeField] private DancingManController dancingMan;
    [SerializeField] private List<pileRummage> pile;
    [SerializeField] private RummageCoinRaycaster caster;
    private bool playingDancingManAnimation = false;
    private bool gameSetup = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;



    [Header("Piles")]
    [SerializeField] private List<RummageCoin> pile1;
    [SerializeField] private List<RummageCoin> pile2;
    [SerializeField] private List<RummageCoin> pile3;
    [SerializeField] private List<RummageCoin> pile4;
    [SerializeField] private List<RummageCoin> pile5;
    private List<RummageCoin> allCoins = new List<RummageCoin>();
    private int selectedIndex;
    private RummageCoin selectedRummageCoin;
    private bool pileLock1 = false;
    private bool pileLock2 = false;
    private bool pileLock3 = false;
    private bool pileLock4 = false;
    private bool pileLock5 = false;




    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            //instance = this;
           
        }

        // dev object stuff
        //devObject.SetActive(GameManager.instance.devModeActivated);

        PregameSetup();
        StartCoroutine(StartGame());
    }

    void Update()
    {
        if (dancingMan.isClicked)
        {
            StartCoroutine(DancingManRoutine());
        }
        if (pile[0].chosen == true && pileLock1 == false)
        {
            pileLock1 = true;
            Debug.Log("Does thsi work?");
        }
        if (pile[1].chosen == true && pileLock2 == false)
        {
            pileLock2 = true;
            Debug.Log("Does thsi work?");
        }
        if (pile[2].chosen == true && pileLock3 == false)
        {
            pileLock3 = true;
            Debug.Log("Does thsi work?");
        }
        if (pile[3].chosen == true && pileLock4 == false)
        {
            pileLock4 = true;
            Debug.Log("Does thsi work?");
        }
        if (pile[4].chosen == true && pileLock5 == false)
        {
            pileLock5 = true;
            Debug.Log("Does thsi work?");
        }

    }

    private IEnumerator DancingManRoutine()
    {
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
        dancingMan.PlayUsingPhonemeEnum(selectedRummageCoin.type);
        yield return new WaitForSeconds(1.5f);
        playingDancingManAnimation = false;
    }

    public bool EvaluateSelectedRummageCoin(RummageCoin coin)
    {

        return false;
    }


    private IEnumerator CoinFailRoutine()
    {
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator CoinSuccessRoutine()
    {
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1.5f);

    }

    private void PregameSetup()
    {
        // create coin list
        foreach (var coin in pile1)
            allCoins.Add(coin);
        foreach (var coin in pile2)
            allCoins.Add(coin);
        foreach (var coin in pile3)
            allCoins.Add(coin);
        foreach (var coin in pile4)
            allCoins.Add(coin);
        foreach (var coin in pile5)
            allCoins.Add(coin);

        // Create Global Coin List
        globalCoinPool = GameManager.instance.GetGlobalActionWordList();
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // disable all coins
        foreach (var coin in allCoins)
            coin.ToggleVisibility(false, false);

        // sink all the logs except the first row

    }



    private IEnumerator StartGame()
    {
        // wait a moment for the setup to finish
        while (!gameSetup)
            yield return null;


    }

    private IEnumerator ShowCoins(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);
        foreach (var coin in pile)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
                unusedCoinPool.AddRange(globalCoinPool);
            ActionWordEnum type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            coin.SetCoinType(type);
            coin.ToggleVisibility(true, true);

            yield return new WaitForSeconds(0.1f);
        }
        //SelectRandomCoin(currRow);
    }

    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<RummageCoin> row = GetCoinPile(index);
        foreach (var coin in row)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<RummageCoin> pile = GetCoinPile(index);
        selectedIndex = Random.Range(0, pile.Count);
        print("selected index: " + selectedIndex);
       
        StartCoroutine(DancingManRoutine());

    }

    private List<RummageCoin> GetCoinPile(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return pile1;
            case 1:
                return pile2;
            case 2:
                return pile3;
            case 3:
                return pile4;
            case 4:
                return pile5;
        }
    }
}
