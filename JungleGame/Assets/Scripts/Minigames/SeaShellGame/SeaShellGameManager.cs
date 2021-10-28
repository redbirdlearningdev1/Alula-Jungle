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

    public bool playTutorial = false;

    private ActionWordEnum currentCoin;
    private int timesMissed = 0;
    private int timesCorrect = 0;

    // coin lists
    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;
    private ActionWordEnum prevCorrectCoin;


    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }

        // stop music 
        AudioManager.instance.StopMusic();

        // get mapID
        mapID = GameManager.instance.mapID;
        
        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);
    }

    void Start()
    {
        StartCoroutine(PregameSetupRoutine());
    }


    private IEnumerator PregameSetupRoutine()
    {
        // turn off raycaster
        ShellRayCaster.instance.isOn = false;
        
        globalCoinPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }
        
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        yield return new WaitForSeconds(1f);

        // show mermaids
        MermaidController.instance.ShowMermaids();

        // short delay before game starts
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // get shell options
        usedCoinPool = new List<ActionWordEnum>();
        List<ActionWordEnum> shellOptions = new List<ActionWordEnum>();
        for (int i = 0; i < 3; i++)
        {
            shellOptions.Add(GetUnusedWord());
        }
        // get correct option
        int correctIndex = Random.Range(0, 3);
        currentCoin = shellOptions[correctIndex];
        // set shells
        ShellController.instance.shell1.SetValue(shellOptions[0]);
        ShellController.instance.shell2.SetValue(shellOptions[1]);
        ShellController.instance.shell3.SetValue(shellOptions[2]);

        // place coin
        OctoController.instance.PlaceNewCoin(currentCoin);
        yield return new WaitForSeconds(2f);

        // reveal shells
        ShellController.instance.RevealShells();

        // turn on raycaster
        ShellRayCaster.instance.isOn = true;
        yield return null;
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

        // make sure word is not being used or already successfully completed
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }

    public bool EvaluateSelectedShell(ActionWordEnum value, int shellNum)
    {
        // turn off raycaster
        ShellRayCaster.instance.isOn = false;
        
        // hide shells
        ShellController.instance.HideShells();

        // correct!
        if (value == currentCoin)
        {
            StartCoroutine(CorrectRoutine(shellNum));
            return true;
        }
        // incorrect
        else
        {
            StartCoroutine(IncorrectRoutine());
            return false;
        }
    }

    private IEnumerator CorrectRoutine(int shellNum)
    {
        timesCorrect++;

        // play mermaid routine
        MermaidController.instance.PlayShell(shellNum);
        yield return new WaitForSeconds(3f);

        // coin holder color
        CoinHolder.instance.CorrectCoinHolder();
        yield return new WaitForSeconds(1f);

        // correct coin animation
        OctoController.instance.CoinCorrect();
        yield return new WaitForSeconds(3f);

        // coin holder color
        CoinHolder.instance.BaseCoinHolder();
        yield return new WaitForSeconds(1f);

        // check if win game
        if (timesCorrect >= 4)
        {
            StartCoroutine(WinRoutine());
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator WinRoutine()
    {
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private int CalculateStars()
    {
        if (timesMissed <= 0)
            return 3;
        else if (timesMissed > 0 && timesMissed <= 2)
            return 2;
        else
            return 1;
    }

    private IEnumerator IncorrectRoutine()
    {
        timesMissed++;

        // coin holder color
        CoinHolder.instance.IncorrectCoinHolder();
        yield return new WaitForSeconds(1f);

        // correct coin animation
        OctoController.instance.CoinIncorrect();
        yield return new WaitForSeconds(3f);

        // coin holder color
        CoinHolder.instance.BaseCoinHolder();
        yield return new WaitForSeconds(1f);

        StartCoroutine(StartGame());
    }
}

