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

        // reveal shells
        ShellController.instance.RevealShells();


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
}

