using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPasswordGameManager : MonoBehaviour
{
    public static NewPasswordGameManager instance;

    [Header("Game Parts")]
    public List<UniversalCoinImage> coins;
    public Transform coinParent;
    public Transform polaroidParent;
    public Polaroid polaroid;
    public Transform tigerCharacter;
    public Transform marcusCharacter;
    public Transform brutusCharacter;

    [Header("Coin Positions")]
    public Transform coinOffScreenPos;
    public List<Transform> coinOnScreenPositions;
    public List<Transform> coinDownPositions;
    public List<Transform> coinTubePositions;

    [Header("Polaroid Positions")]
    public Transform polaroidOffScreenTigerPos;
    public Transform polaroidOffScreenPlayerPos;
    public Transform polaroidOnScreenPos;

    [Header("Character Positions")]
    public Transform tigerOnScreenPos;
    public Transform tigerOffScreenPos;
    public Transform marcusOnScreenPos;
    public Transform marcusOffScreenPos;
    public Transform brutusOnScreenPos;
    public Transform brutusOffScreenPos;

    private ChallengeWord currentWord;
    private int numMisses = 0;

    // challenge word pool
    private List<ChallengeWord> wordPool;


    [Header("Tutorial")]
    public bool playTutorial;
    private bool playIntro = false;
    private int tutorialEvent = 0;
    public ChallengeWord tutorialPolaroid1;
    public ChallengeWord tutorialPolaroid2;
    public ChallengeWord tutorialPolaroid3;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().passwordTutorial;

        // add settings button if not playing tutorial
        if (!playTutorial)
        {
            // turn on settings button
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        PregameSetup();
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
                // save to sis
                StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;
                StudentInfoSystem.SaveStudentPlayerData();
                // calculate and show stars
                AIData(StudentInfoSystem.GetCurrentProfile());
                StarAwardController.instance.AwardStarsAndExit(3);
            }
        }
    }

    private void PregameSetup()
    {
        // reset win cards
        WinCardsController.instance.ResetCards();

        // hide lock
        PasswordLock.instance.HideLock();

        // fill challenge word pool
        wordPool = new List<ChallengeWord>();
        wordPool.AddRange(ChallengeWordDatabase.GetChallengeWords(StudentInfoSystem.GetCurrentProfile().actionWordPool));

        // place charcters off screen
        tigerCharacter.position = tigerOffScreenPos.position;
        marcusCharacter.position = marcusOffScreenPos.position;
        brutusCharacter.position = brutusOffScreenPos.position;

        // make coins empty gold
        foreach(var coin in coins)
            coin.SetValue(ElkoninValue.empty_gold);

        StartCoroutine(NewRound(true));
    }

    private IEnumerator NewRound(bool moveBG)
    {
        // place coins off-screen
        foreach(var coin in coins)
            coin.transform.position = coinOffScreenPos.position;
            
        // place polaroid off-screen
        polaroid.transform.position = polaroidOffScreenTigerPos.position;

        // select challenge word + reset polaroid
        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentWord = tutorialPolaroid1;
                    break;

                case 1:
                    currentWord = tutorialPolaroid2;
                    break;

                case 2:
                    currentWord = tutorialPolaroid3;
                    break;
            }
            tutorialEvent++;
        }
        else
        {
            currentWord = wordPool[Random.Range(0, wordPool.Count)];
            wordPool.Remove(currentWord);
        }

        
        polaroid.SetPolaroid(currentWord);
        polaroid.SetPolaroidAlpha(0f, 0f);
        yield return new WaitForSeconds(1f);

        // move to new section
        if (moveBG)
        {
            PasswordLock.instance.ResetLock();
            BGManager.instance.MoveToNextSection();
            PasswordTube.instance.TurnTube();
            yield return new WaitForSeconds(2f);
            PasswordTube.instance.StopTube();
            yield return new WaitForSeconds(0.5f);

            // play walking animations
            float moveTime = 2f;
            tigerCharacter.GetComponent<Animator>().Play("tigerWalk");
            marcusCharacter.GetComponent<Animator>().Play("marcusWalkIn");
            brutusCharacter.GetComponent<Animator>().Play("brutusWalkIn");

            // move characters on screen
            tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOnScreenPos, moveTime, false);
            marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOnScreenPos, moveTime, false);
            brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOnScreenPos, moveTime, false);
            yield return new WaitForSeconds(moveTime);
        }

        // play idle animations + show lock
        tigerCharacter.GetComponent<Animator>().Play("aTigerIdle");
        marcusCharacter.GetComponent<Animator>().Play("marcusBroken");
        brutusCharacter.GetComponent<Animator>().Play("brutusBroken");
        PasswordLock.instance.ShowLock();
        yield return new WaitForSeconds(1f);

        if (playTutorial)
        {
            if (tutorialEvent == 1)
            {
                // play tutorial intro 1
                AudioClip clip = GameIntroDatabase.instance.passwordIntro1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 2
                clip = GameIntroDatabase.instance.passwordIntro2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 3
                clip = GameIntroDatabase.instance.passwordIntro3;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 4
                clip = GameIntroDatabase.instance.passwordIntro4;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 5 + 6
                List<AudioClip> clips = new List<AudioClip>();
                clips.Add(GameIntroDatabase.instance.passwordIntro5);
                clips.Add(GameIntroDatabase.instance.passwordIntro6);
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clips);
                yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);
            }
        }
        else
        {
            if (!playIntro)
            {
                playIntro = true;

                // play start 1
                AudioClip clip = GameIntroDatabase.instance.passwordStart1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play start 2
                clip = GameIntroDatabase.instance.passwordStart2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play start 3
                clip = GameIntroDatabase.instance.passwordStart3;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
        }

        // show polaroid
        polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOnScreenPos, 0.5f, false);
        yield return new WaitForSeconds(0.2f);
        polaroid.SetPolaroidAlpha(1f, 0.2f);
        yield return new WaitForSeconds(0.5f);

        // say polaroid word
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        AudioManager.instance.PlayTalk(polaroid.challengeWord.audio);
        yield return new WaitForSeconds(polaroid.challengeWord.audio.length + 0.1f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // show coins
        int i = 0;
        foreach(var coin in coins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 0.8f + (0.1f * i));
            yield return new WaitForSeconds(0.1f);
            i++;
        }

        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 7
            AudioClip clip = GameIntroDatabase.instance.passwordIntro7;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 8 + 9
            List<AudioClip> clips = new List<AudioClip>();
            clips.Add(GameIntroDatabase.instance.passwordIntro8);
            clips.Add(GameIntroDatabase.instance.passwordIntro9);
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clips);
            yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);

            // play tutorial intro 10
            clip = GameIntroDatabase.instance.passwordIntro10;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else
        {
            // play new photo popup
            int index = Random.Range(0, GameIntroDatabase.instance.passwordNewPhoto.Count);
            AudioClip clip = GameIntroDatabase.instance.passwordNewPhoto[index];

            switch (index)
            {
                case 0:
                case 1:
                case 2:
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                    break;

                case 3:
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                    break;
            }
        }

        // turn on raycaster
        NewPasswordRaycaster.instance.isOn = true;
    }   

    public void ResetCoins()
    {
        int i = 0;
        foreach(var coin in coins)
        {
            if (!PasswordTube.instance.tubeCoins.Contains(coin))
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
                i++;
            }
        }
    }

    public void EvaluateCoins()
    {
        // turn off raycaster
        NewPasswordRaycaster.instance.isOn = false;
        StartCoroutine(EvaluateCoinsRoutine());
    }

    private IEnumerator EvaluateCoinsRoutine()
    {
        // small delay
        yield return new WaitForSeconds(1f);

        bool winGame = false;

        // determine if correct num of coins
        if (currentWord.elkoninCount == PasswordTube.instance.tubeCoins.Count)
        {
            winGame = true;

            // play right audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            yield return new WaitForSeconds(1f);

            // turn empty coins into polaroid coins
            PasswordTube.instance.ShowPolaroidCoins(currentWord, coins, true);
            while (PasswordTube.instance.playingAnimation)
                yield return null;

            yield return new WaitForSeconds(1f);

            // move polaroid to player off-screen pos
            polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOffScreenPlayerPos, 0.2f, false);
            yield return new WaitForSeconds(0.5f);

            // unlock lock
            PasswordLock.instance.Unlock();

            // add card to win cards
            WinCardsController.instance.AddPolaroid();

            // coin animations
            PasswordTube.instance.CorrectCoinsAnimation();

            yield return new WaitForSeconds(1f);
            
            // remove extra coins
            // create list of non tube coins
            List<UniversalCoinImage> extraCoins = new List<UniversalCoinImage>();
            extraCoins.AddRange(coins);

            // remove tube coins
            foreach (var tubeCoin in PasswordTube.instance.tubeCoins)
            {
                extraCoins.Remove(tubeCoin);
            }
            RemoveExtraCoins(extraCoins);

            // determine if win
            if (WinCardsController.instance.WinGame())
            {
                StartCoroutine(WinRoutine());
                yield break;
            }
            else
            {
                // play appropriate popup
                if (playTutorial && tutorialEvent == 1)
                {
                    // play tutorial intro 11
                    AudioClip clip = GameIntroDatabase.instance.passwordIntro11;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                    yield return new WaitForSeconds(clip.length + 1f);

                    // play tutorial intro 12
                    clip = GameIntroDatabase.instance.passwordIntro12;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                    yield return new WaitForSeconds(clip.length + 1f);
                }
                else
                {
                    AudioClip clip = null;
                    int index = Random.Range(0, 5);
                    switch (index)
                    {
                        case 0:
                            clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ugh");
                            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                            yield return new WaitForSeconds(clip.length + 1f);
                            break;

                        case 1:
                            clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_grr");
                            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                            yield return new WaitForSeconds(clip.length + 1f);
                            break;

                        case 2:
                            clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_argh");
                            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                            yield return new WaitForSeconds(clip.length + 1f);
                            break;

                        case 3:
                            clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("brutus_heh");
                            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                            yield return new WaitForSeconds(clip.length + 1f);
                            break;

                        case 4:
                            clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_grr");
                            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                            yield return new WaitForSeconds(clip.length + 1f);
                            break;
                    }
                }

                // tiger run away
                tigerCharacter.GetComponent<Animator>().Play("aTigerTurn");
                yield return new WaitForSeconds(0.25f);
                tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOffScreenPos, 1f, false);
                yield return new WaitForSeconds(1f);

                // monkies walk way
                marcusCharacter.GetComponent<Animator>().Play("marcusTurn");
                brutusCharacter.GetComponent<Animator>().Play("brutusTurn");
                yield return new WaitForSeconds(0.25f);
                marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOffScreenPos, 2f, false);
                brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOffScreenPos, 2f, false);
                yield return new WaitForSeconds(2f);

                // remove lock
                PasswordLock.instance.HideLock();
            }
        }
        else
        {
            // only count misses if not playing tutorial
            if (!playTutorial)
                numMisses++;

            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);
            yield return new WaitForSeconds(1f);

            if (playTutorial)
            {
                // show polaroid
                polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOnScreenPos, 0.5f, false);
                yield return new WaitForSeconds(0.2f);
                polaroid.SetPolaroidAlpha(1f, 0.2f);
                yield return new WaitForSeconds(0.5f);

                // turn on raycaster
                NewPasswordRaycaster.instance.isOn = true;
                yield break;
            }

            // turn empty coins into polaroid coins
            PasswordTube.instance.ShowPolaroidCoins(currentWord, coins, false);
            while (PasswordTube.instance.playingAnimation)
                yield return null;

            yield return new WaitForSeconds(1f);

            // move polaroid to tiger off-screen pos
            polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOffScreenTigerPos, 0.2f, false);
            yield return new WaitForSeconds(0.5f);

            PasswordLock.instance.UpgradeLock();

            // coin animation reset
            PasswordTube.instance.RemoveAllCoins();

            yield return new WaitForSeconds(0.5f);

            // remove extra coins
            // create list of non tube coins
            List<UniversalCoinImage> extraCoins = new List<UniversalCoinImage>();
            extraCoins.AddRange(coins);

            // remove tube coins
            foreach (var tubeCoin in PasswordTube.instance.tubeCoins)
            {
                extraCoins.Remove(tubeCoin);
            }
            RemoveExtraCoins(extraCoins);

            // determine if lose
            if (PasswordLock.instance.LoseGame())
            {
                StartCoroutine(LoseRoutine());
                yield break;
            }
            else
            {
                // play appropriate popup
                AudioClip clip = null;
                int index = Random.Range(0, 4);
                switch (index)
                {
                    case 0:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_haha");
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                        yield return new WaitForSeconds(clip.length + 1f);
                        break;

                    case 1:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ahhah");
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Julius, clip);
                        yield return new WaitForSeconds(clip.length + 1f);
                        break;

                    case 2:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("marcus_laugh");
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Marcus, clip);
                        yield return new WaitForSeconds(clip.length + 1f);
                        break;

                    case 3:
                        clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("brutus_laugh");
                        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Brutus, clip);
                        yield return new WaitForSeconds(clip.length + 1f);
                        break;
                }
            }
        }
        
        yield return new WaitForSeconds(0.5f);

        // begin next round
        StartCoroutine(NewRound(winGame));
    }

    private void RemoveExtraCoins(List<UniversalCoinImage> extraCoins)
    {
        int i = 0;
        foreach (var coin in extraCoins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinDownPositions[i], 0.2f, false);
            i++;
        }
    }

    public void SayPolaroidWord()
    {
        // return if there is no word
        if (currentWord == null)
            return;

        StartCoroutine(SayPolaroidWordRoutine());
    }

    private IEnumerator SayPolaroidWordRoutine()
    {
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
        AudioManager.instance.PlayTalk(currentWord.audio);
        yield return new WaitForSeconds(currentWord.audio.length + 1f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
    }

    private IEnumerator WinRoutine()
    {
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("NewPasswordGame", true, 3f);
        }
        else
        {
            // show stars
            AIData(StudentInfoSystem.GetCurrentProfile());
            StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        }
    }

    public void AIData(StudentPlayerData playerData)
    {
        playerData.passPlayed = playerData.passPlayed + 1;
        playerData.starsPass = CalculateStars() + playerData.starsPass;
        
    }

    private int CalculateStars()
    {
        if (numMisses <= 0)
            return 3;
        else if (numMisses > 0 && numMisses <= 2)
            return 2;
        else
            return 1;
    }

    private IEnumerator LoseRoutine()
    {        
        yield return new WaitForSeconds(2f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}
