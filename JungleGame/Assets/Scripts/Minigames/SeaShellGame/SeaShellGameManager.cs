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

    private MapIconIdentfier mapID = MapIconIdentfier.None;

    public bool playTutorial = false;

    [HideInInspector] public ActionWordEnum currentCoin;
    private int timesMissed = 0;
    private int timesCorrect = 0;

    // coin lists
    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;
    private ActionWordEnum prevCorrectCoin;


    [Header("Tutorial Variables")]
    public int[] t_correctIndexes;
    public List<ActionWordEnum> t_firstRound;
    public List<ActionWordEnum> t_secondRound;
    public List<ActionWordEnum> t_thirdRound;
    public List<ActionWordEnum> t_fourthRound;

    private int t_currRound = 0;


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
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().seashellTutorial;

        if (playTutorial)
        {
            StartCoroutine(StartTutorial());
        }
        else
        {
            // start song
            AudioManager.instance.InitSplitSong(SplitSong.Seashells);
            AudioManager.instance.IncreaseSplitSong();

            StartCoroutine(PregameSetupRoutine());
        }
    }

    void Update()
    {
        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                // play win tune
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
                // update AI data
                AIData(StudentInfoSystem.GetCurrentProfile());
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(3);
            }
        }
    }

    private IEnumerator StartTutorial()
    {
        // small delay before starting
        yield return new WaitForSeconds(2f);

        // show mermaids
        MermaidController.instance.ShowMermaids();

        // small delay before starting
        yield return new WaitForSeconds(1f);

        // turn off raycaster
        ShellRayCaster.instance.isOn = false;

        // play ambiance sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.SeaAmbiance, 0.1f, "sea_ambiance");

        // play tutorial audio
        AudioClip clip = GameIntroDatabase.instance.seashellIntro1;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio
        clip = GameIntroDatabase.instance.seashellIntro2;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
        yield return new WaitForSeconds(clip.length + 1f);

        StartCoroutine(StartGame());
    }


    private IEnumerator PregameSetupRoutine()
    {
        // turn off raycaster
        ShellRayCaster.instance.isOn = false;

        // play ambiance sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.SeaAmbiance, 0.25f, "sea_ambiance");
        
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

        if (playTutorial)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (t_currRound)
                {
                    case 0: shellOptions.Add(t_firstRound[i]); break;
                    case 1: shellOptions.Add(t_secondRound[i]); break;
                    case 2: shellOptions.Add(t_thirdRound[i]); break;
                    case 3: shellOptions.Add(t_fourthRound[i]); break;
                }
            } 

            currentCoin = shellOptions[t_correctIndexes[t_currRound]];
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                shellOptions.Add(GetUnusedWord());
            }
            // get correct option
            int correctIndex = Random.Range(0, 3);
            currentCoin = shellOptions[correctIndex];
        }
        
        
        // set shells
        ShellController.instance.shell1.SetValue(shellOptions[0]);
        ShellController.instance.shell2.SetValue(shellOptions[1]);
        ShellController.instance.shell3.SetValue(shellOptions[2]);

        // place coin
        OctoController.instance.PlaceNewCoin(currentCoin);
        yield return new WaitForSeconds(2f);

        // reveal shells
        ShellController.instance.RevealShells();

        if (playTutorial && t_currRound == 0)
        {
            yield return new WaitForSeconds(3f);

            // play tutorial audio
            AudioClip clip = GameIntroDatabase.instance.seashellIntro3;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }

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

        // correct!
        if (value == currentCoin)
        {
            StartCoroutine(CorrectRoutine(shellNum));
            return true;
        }
        // incorrect
        else
        {
            if (playTutorial)
            {
                // turn on raycaster
                ShellRayCaster.instance.isOn = true;
            }
            else
            {
                StartCoroutine(IncorrectRoutine());
            }
            
            return false;
        }
    }

    private IEnumerator CorrectRoutine(int shellNum)
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();
        
        timesCorrect++;

        // hide shells
        ShellController.instance.HideShells();

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


        // play tutorial audio
        if (playTutorial && t_currRound == 0)
        {
            // play tutorial intro
            AudioClip clip = GameIntroDatabase.instance.seashellIntro4;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Bubbles, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro
            clip = GameIntroDatabase.instance.seashellIntro5;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else if (t_currRound <= 2)
        {
            // play random encouragement popup
            int index = Random.Range(0, 3);
            AudioClip clip = null;
            switch (index)
            {
                case 0:
                    clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Bubbles, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                    break;
                case 1:
                    clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                    break;
                case 2:
                    clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                    break;
            }
        }


        t_currRound++;

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

        if (playTutorial)
        {
            // save to SIS
            StudentInfoSystem.GetCurrentProfile().seashellTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("SeaShellGame", true, 3f);
        }
        else
        {

            AIData(StudentInfoSystem.GetCurrentProfile());
    
            // calculate and show stars
            StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        }
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.starsGameBeforeLastPlayed = playerData.starsLastGamePlayed;
        playerData.starsLastGamePlayed = CalculateStars();
        playerData.gameBeforeLastPlayed = playerData.lastGamePlayed;
        playerData.lastGamePlayed = GameType.SeashellGame;
        playerData.starsSeashell = CalculateStars() + playerData.starsSeashell;
        playerData.totalStarsSeashell = 3 + playerData.totalStarsSeashell;
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

        // play incorrect audio
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

        // coin holder color
        CoinHolder.instance.IncorrectCoinHolder();
        yield return new WaitForSeconds(1f);

        // incorrect coin animation
        OctoController.instance.CoinIncorrect();
        yield return new WaitForSeconds(3f);

        // coin holder color
        CoinHolder.instance.BaseCoinHolder();
        yield return new WaitForSeconds(1f);

        // play random reminder popup
        int index = Random.Range(0, 2);
        AudioClip clip = null;
        switch (index)
        {
            case 0:
                clip = GameIntroDatabase.instance.seashellReminder1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
                yield return new WaitForSeconds(clip.length + 1f);
                break;
            case 1:
                clip = GameIntroDatabase.instance.seashellReminder2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
                yield return new WaitForSeconds(clip.length + 1f);
                break;
        }

        StartCoroutine(StartGame());
    }
}

