using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaShellGameManager : MonoBehaviour
{
    public static SeaShellGameManager instance;

    [SerializeField] private BluMermaidController bluMermaid;
    [SerializeField] private PinkMermaidController pinkMermaid;
    [SerializeField] private BluMermaidController bluMermaidPlay;
    [SerializeField] private PinkMermaidController pinkMermaidPlay;
    [SerializeField] private CoinHolder coinHolder;
    [SerializeField] private Chest chest;
    [SerializeField] private OctoController octo;
    [SerializeField] private TideController tide;
    [SerializeField] private TideController wash;
    [SerializeField] private List<SeaShell> shells;
    [SerializeField] private Coin coin;
    private int selectedIndex;
    private SeaShell selectedShell;
    private bool gameSetup = false;

    public List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private int Posit;
    private int winCount;
    private int mermaidChosen = 0;



    [Header("Dev Stuff")]
    [SerializeField] private GameObject devObject;


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
        
        StartCoroutine(StartGame());
    }

    void Update()
    {
   
       
    }

    private IEnumerator ShellRoutine()
    {
        yield return new WaitForSeconds(1.5f);
    }

    public bool EvaluateSelectedShell(SeaShell shell)
    {

        if (coin.type == shell.type)
        {
            // success! go on to the next row or win game if on last row
            if (winCount < 4)
                StartCoroutine(ShellSuccessRoutine());
            else
                StartCoroutine(WinRoutine());
            return true;
        }
        // fail go back to previous row
        StartCoroutine(ShellFailRoutine());
        return false;
    }

    private IEnumerator ShellFailRoutine()
    {
        Debug.Log("FAIL");
        coinHolder.IncorrectCoinHolder();
        octo.tenticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        octo.incorrectAnimation();
        coin.GoToOffStage();
        yield return new WaitForSeconds(1f);
        coin.ToggleVisibilityCoin(false, false);
        coin.GoToOffStage();
        octo.tenticle.gameObject.SetActive(false);

        StartCoroutine(StartGame());

    }

    private IEnumerator ShellSuccessRoutine()
    {
        Debug.Log("Win");
        coinHolder.CorrectCoinHolder();
        if (mermaidChosen == 0)
        {
            if (selectedIndex == 0)
            {
                shells[0].ToggleVisibility(false, false);
                shells[0].shadow.gameObject.SetActive(false);
                shells[0].ReturnToLog();

             
                
                bluMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                bluMermaid.gameObject.SetActive(false);
                bluMermaidPlay.pinkShellAnimation();
            }
            else if (selectedIndex == 1)
            {
                shells[1].ToggleVisibility(false, false);
                shells[1].shadow.gameObject.SetActive(false);
                shells[1].ReturnToLog();
                
                bluMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                bluMermaid.gameObject.SetActive(false);
                bluMermaidPlay.blueShellAnimation();
            }
            else
            {
                shells[2].ToggleVisibility(false, false);
                shells[2].shadow.gameObject.SetActive(false);
                shells[2].ReturnToLog();

                bluMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                bluMermaid.gameObject.SetActive(false);
                bluMermaidPlay.redShellAnimation();
            }
            yield return new WaitForSeconds(2f);
            

            bluMermaid.gameObject.SetActive(true);
            mermaidChosen++;
        }
        else
        {
            if (selectedIndex == 0)
            {
                shells[0].ToggleVisibility(false, false);
                shells[0].shadow.gameObject.SetActive(false);
                shells[0].ReturnToLog();

                pinkMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                pinkMermaid.gameObject.SetActive(false);
                pinkMermaidPlay.pinkShellAnimation();
            }
            else if (selectedIndex == 1)
            {
                shells[1].ToggleVisibility(false, false);
                shells[1].shadow.gameObject.SetActive(false);
                shells[1].ReturnToLog();

                pinkMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                pinkMermaid.gameObject.SetActive(false);
                pinkMermaidPlay.blueShellAnimation();
            }
            else
            {
                shells[2].ToggleVisibility(false, false);
                shells[2].shadow.gameObject.SetActive(false);
                shells[2].ReturnToLog();

                pinkMermaid.diveAnimation();
                yield return new WaitForSeconds(1f);
                pinkMermaid.gameObject.SetActive(false);
                pinkMermaidPlay.redShellAnimation();
            }
            yield return new WaitForSeconds(2f);
            
            pinkMermaid.gameObject.SetActive(true);
            mermaidChosen--;
        }
        coin.coinAnimation(selectedShell.type.ToString());
        winCount++;
        octo.correctAnimation();
        yield return new WaitForSeconds(.25f);
        octo.tenticle.gameObject.SetActive(true);
        yield return new WaitForSeconds(.5f);
        coin.GoToCorrectPosition();
        yield return new WaitForSeconds(1.75f);
        octo.tenticle.gameObject.SetActive(false);
        octo.correctAnimationTwo();
        coin.GoToChestPosition();
        coin.shrink();
        yield return new WaitForSeconds(.25f);
        coin.GoToChestTwoPosition();
        coin.ToggleVisibilityTwoCoin(false, true);
        yield return new WaitForSeconds(.25f);
        coin.grow();
        chest.UpgradeChest();
        yield return new WaitForSeconds(.2f);
        coin.GetComponent<Animator>().enabled = false;
        coin.GoToStartingPosition();
        yield return new WaitForSeconds(.5f);
        StartCoroutine(StartGame());
    }

    private IEnumerator WinRoutine()
    {
       
        yield return new WaitForSeconds(1.2f);
    }

    private void PregameSetup()
    {
        // Create Global Coin List
        
        globalCoinPool = new List<ActionWordEnum>();
        unusedCoinPool = new List<ActionWordEnum>();
        string[] coins = System.Enum.GetNames(typeof(ActionWordEnum));
        for (int i = 0; i < coins.Length; i++)
        {
            ActionWordEnum coin = (ActionWordEnum)System.Enum.Parse(typeof(ActionWordEnum), coins[i]);
            globalCoinPool.Add(coin);
        }
        globalCoinPool.Remove(ActionWordEnum.SIZE);
        unusedCoinPool.AddRange(globalCoinPool);
        coin.ToggleVisibilityCoin(false, false);

        shells[0].ToggleVisibility(false, false);
        shells[1].ToggleVisibility(false, false);
        shells[2].ToggleVisibility(false, false);
        
        //shells[0].gameObject.SetActive(false);
        //shells[1].gameObject.SetActive(false);
        //shells[2].gameObject.SetActive(false);
        shells[0].shadow.gameObject.SetActive(false);
        shells[1].shadow.gameObject.SetActive(false);
        shells[2].shadow.gameObject.SetActive(false);

    }


    private IEnumerator StartGame()
    {
        octo.octoIdle();
        //while (!gameSetup)
        //yield return null;
        coinHolder.BaseCoinHolder();
        yield return new WaitForSeconds(.5f);
        coin.GoToStartingPosition();
        
        yield return new WaitForSeconds(.25f);
        StartCoroutine(createShells());
        SelectRandomShell();
        ActionWordEnum Answer = selectedShell.type;
        StartCoroutine(ShowCoin(Answer));
        tide.gameObject.SetActive(false);
        wash.gameObject.SetActive(true);
        yield return new WaitForSeconds(.6f);
        shells[0].ToggleVisibility(true, true);
        shells[1].ToggleVisibility(true, true);
        shells[2].ToggleVisibility(true, true);
        shells[0].shadow.gameObject.SetActive(true);
        shells[1].shadow.gameObject.SetActive(true);
        shells[2].shadow.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        wash.gameObject.SetActive(false);
        tide.gameObject.SetActive(true);
        
    }

    private IEnumerator ShowCoin(ActionWordEnum type)
    {

        // set random type
        Debug.Log("here");
            Posit = globalCoinPool.IndexOf(type);
            Debug.Log(type);
            coin.SetCoinType(type,Posit);
        
            coin.ToggleVisibilityCoin(true, true);
        octo.tenticle.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.15f);
        octo.tenticle.gameObject.SetActive(false);


        //SelectRandomCoin(currRow);

    }


    private IEnumerator createShells()
    {
        List<SeaShell> shells = GetShells();
        foreach (var shell in shells)
        {
            // set random type
            if (unusedCoinPool.Count == 0)
                unusedCoinPool.AddRange(globalCoinPool);
            ActionWordEnum type = unusedCoinPool[Random.Range(0, unusedCoinPool.Count)];
            unusedCoinPool.Remove(type);

            shell.SetShellType(type);
            Debug.Log(shell.type);
            

            
        }
        yield return new WaitForSeconds(0.1f);



    }

    private IEnumerator HideCoins(int index, LogCoin exceptCoin = null)
    {
        
        foreach (var coin in shells)
        {
            if (coin != exceptCoin)
                coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomShell()
    {
        
        selectedIndex = Random.Range(0, shells.Count);
        print("selected index: " + selectedIndex);
        selectedShell = shells[selectedIndex];
        Debug.Log(selectedShell.type);
    }

    private List<SeaShell> GetShells()
    {

                return shells;

        
    }


}

