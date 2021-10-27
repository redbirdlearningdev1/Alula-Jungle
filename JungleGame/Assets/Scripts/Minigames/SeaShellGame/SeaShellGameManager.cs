using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct SeaShellTutorialList
{
    public List<ActionWordEnum> list;
}


public class SeaShellGameManager : MonoBehaviour
{
    public static SeaShellGameManager instance;

    private MapIconIdentfier mapID;

    public bool playingInEditor;
    public bool playTutorial;

    [SerializeField] private OctoController octo;
    [SerializeField] private GameObject tenticle;
    [SerializeField] private BluMermaidController blu;
    [SerializeField] private PinkMermaidController pink;
    [SerializeField] private BluMermaidController bluPlay;
    [SerializeField] private PinkMermaidController pinkPlay;
    [SerializeField] private Chest chestAns;
    [SerializeField] private TideController tide;
    [SerializeField] private CoinHolder holder;
    [SerializeField] private ShellRayCaster caster;

    private List<ActionWordEnum> globalCoinPool;
    [SerializeField] private List<ActionWordEnum> SectionOneCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;


    public List<UniversalCoinImage> coins;
    public List<SeaShell> shells;
      public List<GameObject> shellsObj;
    public List<Transform> shellPos;
    public List<Transform> winCoinPos;
    public List<Transform> loseCoinPos;
    public List<Transform> coinPos;

    private int selectedIndex;
    [HideInInspector] public UniversalCoinImage selectedShell;
    private int winCount = 0;
    private int timesMissed = 0;

    [Header("Tutorial Stuff")]
    public List<SeaShellTutorialList> tutorialLists;
    public int[] correctIndexes;
    private int tutorialEvent = 0;

    void Awake()
    {
         // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        // get mapID
        mapID = GameManager.instance.mapID;
        
        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        if (!playingInEditor)
        {
            //playTutorial = !StudentInfoSystem.GetCurrentProfile().seashellTutorial;
        }
            

        PregameSetup();

        // start tutorial or normal game
        if (playTutorial)
            StartCoroutine(StartTutorialGame());
        else 
        {
            // start song
            //AudioManager.instance.InitSplitSong();
            //AudioManager.instance.IncreaseSplitSong();
            
            StartCoroutine(StartGame());
        }
        
        
    }

    void Update()
    {
        // dev stuff for fx audio testing
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(SkipToWinRoutine());
            }
        }
    }

    private void PregameSetup()
    {
         Debug.Log("HerePregame");
        // turn off raycaster
        //ShellRayCaster.instance.isOn = false;

        // Create Global Coin List
        if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            //This was causing an issue
            //globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
            globalCoinPool = SectionOneCoinPool;
        }

        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(SectionOneCoinPool);

        //Asset setup
        
        for(int i = 0; i < shellsObj.Count; i++)
                {
                    shellsObj[i].SetActive(false);
                }
                
    }

    public bool EvaluateSelectedShell(ActionWordEnum coin)
    {
        // turn off raycaster
        //ShellRayCaster.instance.isOn = false;
        tenticle.SetActive(false);
        if(coin ==  ChallengeWordDatabase.ElkoninValueToActionWord(coins[0].value))
        {
            shells[0].ToggleVisibility(false,false);
        }
        else if(coin ==  ChallengeWordDatabase.ElkoninValueToActionWord(coins[1].value))
        {
            shells[1].ToggleVisibility(false,false);
        }
        else
        {
            shells[2].ToggleVisibility(false,false);
        }
        
        
        
        if (coin ==  ChallengeWordDatabase.ElkoninValueToActionWord(coins[selectedIndex].value))
        {
            
            winCount++;
            Debug.Log("Win");
            // success! go on to the next row or win game if on last row
            if (winCount < 4)
            {
                if (!playTutorial)
                    StartCoroutine(CoinSuccessRoutine());
                else 
                    StartCoroutine(TutorialSuccessRoutine());
                
            }
            else
            {
                if (!playTutorial)
                    StartCoroutine(WinRoutine());
                else 
                    StartCoroutine(TutorialWinRoutine());
            }

            return true;
        }

        if (!playTutorial)
            StartCoroutine(CoinFailRoutine());
        else
            StartCoroutine(TutorialFailRoutine());
    
        return false;
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator CoinSuccessRoutine()
    {
        holder.CorrectCoinHolder();
        tide.waveAnimation();
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < shellsObj.Count; i++)
        {
            shells[i].ToggleVisibility(false,false);
        }
                 
         //yield return new WaitForSeconds(1f);
         int pickMermaid = Random.Range(0,2);
         if(pickMermaid == 0)
         {
            blu.diveAnimation();
            yield return new WaitForSeconds(1f);
            if(selectedIndex == 0)
            {
                bluPlay.pinkShellAnimation();
            }
            else if(selectedIndex == 1)
            {
                bluPlay.blueShellAnimation();
            }
            else
            {
                bluPlay.redShellAnimation();
            }
            blu.gameObject.SetActive(false);
         }
         else
         {
            pink.diveAnimation();
            yield return new WaitForSeconds(1f);
            if(selectedIndex == 0)
            {
                pinkPlay.pinkShellAnimation();
            }
            else if(selectedIndex == 1)
            {
                pinkPlay.blueShellAnimation();
            }
            else
            {
                pinkPlay.redShellAnimation();
            }
            pink.gameObject.SetActive(false);
         }
         yield return new WaitForSeconds(1f);
         shells[selectedIndex].PlayPhonemeAudio();
         yield return new WaitForSeconds(1f);
         
         
         yield return new WaitForSeconds(.5f);
         octo.correctAnimation();
         yield return new WaitForSeconds(.5f);
         tenticle.SetActive(true);
         yield return new WaitForSeconds(.5f);
         coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(winCoinPos[0].position, 0.5f, false);
         yield return new WaitForSeconds(1.25f);
         octo.correctAnimationTwo();
         coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(winCoinPos[1].position, 0.25f, false);
         yield return new WaitForSeconds(.25f);
         coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(winCoinPos[2].position, 0.25f, false);
         yield return new WaitForSeconds(.25f);
         coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(winCoinPos[3].position, 0.25f, false);
         yield return new WaitForSeconds(.25f);
         coins[selectedIndex].ToggleVisibility(false,true);
         chestAns.UpgradeChest();
         StartCoroutine(StartGame());
    }
    private IEnumerator CoinFailRoutine()
    {
        holder.IncorrectCoinHolder();
        octo.incorrectAnimation();
        tide.waveAnimation();
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < shellsObj.Count; i++)
        {
            shells[i].ToggleVisibility(false,false);
        }
        yield return new WaitForSeconds(1f);
        tenticle.SetActive(true);
        yield return new WaitForSeconds(.5f);
         coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(loseCoinPos[0].position, 0.1f, false);
         StartCoroutine(StartGame());
    }



    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        holder.BaseCoinHolder();
        blu.gameObject.SetActive(true);
        pink.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
         Debug.Log("HereStartGame");
         tenticle.SetActive(true);
          yield return new WaitForSeconds(.1f);
        SetCoins();
        StartCoroutine(ResetShells());

        yield return new WaitForSeconds(1f);



        yield return new WaitForSeconds(1.5f);


        yield return new WaitForSeconds(1.5f);

        // play audio

        yield return new WaitForSeconds(1f);

        // bring coins up

        yield return new WaitForSeconds(1f);

        // turn on raycaster
        //ShellRayCaster.instance.isOn = true;
    }

    private IEnumerator StartTutorialGame()
    {
                yield return new WaitForSeconds(1f);
    }

    private IEnumerator TutorialFailRoutine()
    {
        
        yield return new WaitForSeconds(1f);

    }
    private IEnumerator TutorialSuccessRoutine()
    {

        yield return new WaitForSeconds(2f);

    }
    private IEnumerator TutorialWinRoutine()
    {
         yield return new WaitForSeconds(2f);
    }

    private IEnumerator SkipToWinRoutine()
    { 
                yield return new WaitForSeconds(1f);
    }
    private IEnumerator ResetShells()
    { 
        Debug.Log("Here");
                for(int i = 0; i < shellsObj.Count; i++)
                {
                    shellsObj[i].SetActive(false);
                }
                yield return new WaitForSeconds(1f);
                //wash.gameObject.SetActive(true);
                tide.waveAnimation();
                yield return new WaitForSeconds(1f);
                for(int i = 0; i < shellsObj.Count; i++)
                {
                    shellsObj[i].SetActive(true);
                    shells[i].ToggleVisibility(true,true);
                }
            SetShells();
    }

    private void SetCoins()
    {
        
        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();
        usedCoinPool.Clear();

        int i = 0;
        foreach (var coin in coins)
        {
            ActionWordEnum type;
            if (!playTutorial)
                type = GetUnusedWord();
            else
            {   
                type = tutorialLists[tutorialEvent].list[i];
                i++;
            }
            
            coin.SetActionWordValue(type);
            coin.ToggleVisibility(false, false);
        }

        // select random coin OR tutorial coin
        if (!playTutorial)
            SelectRandomCoin();
        else
        {
            selectedIndex = correctIndexes[tutorialEvent];
            selectedShell = coins[selectedIndex];
            shells[selectedIndex].SetShellType(ChallengeWordDatabase.ElkoninValueToActionWord(selectedShell.value));
            tutorialEvent++;
        }
    }
    private ActionWordEnum GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedCoinPool.Count <= 0)
        {
            unusedCoinPool.Clear();
            unusedCoinPool.AddRange(globalCoinPool);
        }

        int index = Random.Range(0, unusedCoinPool.Count);
        ActionWordEnum word = unusedCoinPool[index];

        // make sure word is not being used
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }
    private void SelectRandomCoin()
    {
        
        selectedIndex = Random.Range(0, shells.Count);
        coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(coinPos[0].position, 0.0f, false);;
        coins[selectedIndex].ToggleVisibility(true, true);
        coins[selectedIndex].GetComponent<LerpableObject>().LerpPosition(coinPos[1].position, 0.15f, false);;
    }
    private void SetShells()
    {
        for(int i = 0; i < shells.Count; i++)
        {
            shells[i].SetShellType(ChallengeWordDatabase.ElkoninValueToActionWord(coins[i].value));
        }
    }

}

