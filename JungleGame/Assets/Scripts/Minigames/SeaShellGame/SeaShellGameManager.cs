using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

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
            AudioManager.instance.InitSplitSong(AudioDatabase.instance.SeashellsSongSplit);

            StartCoroutine(PregameSetupRoutine());
        }
    }

    void Update()
    {
        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SkipGame();
                }
            }
        }
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save tutorial done to SIS
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = true;
        // times missed set to 0
        timesMissed = 0;
        // update AI data
        AIData(StudentInfoSystem.GetCurrentProfile());
        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();
    }

    private IEnumerator StartTutorial()
    {
        // play ambiance sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.SeaAmbiance, 0.1f, "sea_ambiance");

        // small delay before starting
        yield return new WaitForSeconds(1f);

        // show mermaids
        MermaidController.instance.ShowMermaids();
        yield return new WaitForSeconds(1f);

        // turn off raycaster
        ShellRayCaster.instance.isOn = false;

        // play tutorial audio
        AssetReference clip = GameIntroDatabase.instance.seashellIntro1;
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
        yield return new WaitForSeconds(cd.GetResult() + 1f);

        // play tutorial audio
        clip = GameIntroDatabase.instance.seashellIntro2;
        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd0.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
        yield return new WaitForSeconds(cd0.GetResult() + 1f);

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
        if (GameManager.instance.practiceModeON)
        {
            globalCoinPool.AddRange(GameManager.instance.practicePhonemes);
        }
        else if (mapID != MapIconIdentfier.None)
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
        // place menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

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
        // yield return new WaitForSeconds(1f);

        // reveal shells
        ShellController.instance.RevealShells();

        if (playTutorial && t_currRound == 0)
        {
            yield return new WaitForSeconds(3f);

            // play tutorial audio
            AssetReference clip = GameIntroDatabase.instance.seashellIntro3;
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);
        }

        // show correct shell if playing tutorial
        if (playTutorial)
        {
            // only wait if not first round
            if (t_currRound != 0)
                yield return new WaitForSeconds(3f);

            for (int i = 0; i < 3; i++)
            {
                SeaShell shell = ShellController.instance.shells[i];
                if (i == t_correctIndexes[t_currRound])
                {
                    shell.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.5f);
                }
                else
                {
                    shell.GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.5f);
                    shell.GetComponent<LerpableObject>().LerpImageAlpha(shell.GetComponent<Image>(), 0.5f, 0.5f);

                    AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);
                }
            }

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

        // coin holder color
        CoinHolder.instance.CorrectCoinHolder();

        // correct coin animation
        OctoController.instance.CoinCorrect();
        yield return new WaitForSeconds(4f);

        // coin holder color
        CoinHolder.instance.BaseCoinHolder();

        // play tutorial audio
        if (playTutorial && t_currRound == 0)
        {
            // play tutorial intro
            AssetReference clip = GameIntroDatabase.instance.seashellIntro4;
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Bubbles, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);

            // play tutorial intro
            clip = GameIntroDatabase.instance.seashellIntro5;
            CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd0.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
            yield return new WaitForSeconds(cd0.GetResult() + 1f);
        }
        else if (t_currRound <= 2 && !(playTutorial && t_currRound > 1))
        {                
            if (GameManager.DeterminePlayPopup())
            {
                // play random encouragement popup
                int index = Random.Range(0, 3);
                AssetReference clip = null;
                switch (index)
                {
                    case 0:
                        clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd.coroutine;
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Bubbles, clip);
                        //yield return new WaitForSeconds(cd.GetResult() + 1f);
                        break;
                    case 1:
                        clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd0.coroutine;
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
                        //yield return new WaitForSeconds(cd0.GetResult() + 1f);
                        break;
                    case 2:
                        clip = GameIntroDatabase.instance.seashellEncouragementClips[index];
                        CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                        yield return cd1.coroutine;
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
                        //yield return new WaitForSeconds(cd1.GetResult() + 1f);
                        break;
                }
            }
        }
        yield return new WaitForSeconds(1f);

        t_currRound++;

        // finish tutorial after 3 rounds
        if (playTutorial && t_currRound == 3)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // check if win game
        if (timesCorrect >= 4)
        {
            StartCoroutine(WinRoutine());
            yield break;
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
            // AI stuff
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

        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
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
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

        // coin holder color
        CoinHolder.instance.IncorrectCoinHolder();

        // incorrect coin animation
        OctoController.instance.CoinIncorrect();
        yield return new WaitForSeconds(2f);

        // play random reminder popup
        int index = Random.Range(0, 2);
        AssetReference clip = null;
        switch (index)
        {
            case 0:
                clip = GameIntroDatabase.instance.seashellReminder1;
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Sylvie, clip);
                //yield return new WaitForSeconds(cd.GetResult() + 1f);
                break;
            case 1:
                clip = GameIntroDatabase.instance.seashellReminder2;
                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd0.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Celeste, clip);
                //yield return new WaitForSeconds(cd0.GetResult() + 1f);
                break;
        }
        yield return new WaitForSeconds(3f);

        // coin holder color
        CoinHolder.instance.BaseCoinHolder();

        StartCoroutine(StartGame());
    }
}

