using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordFactoryBlendingManager : MonoBehaviour
{
    public static WordFactoryBlendingManager instance;

    [Header("Game Objects")]
    [SerializeField] private HorizontalLayoutGroup frameGroup;
    [SerializeField] private Transform[] frames; // six in-game frames
    [SerializeField] private float[] frameSpacing;
    [SerializeField] private Transform coinParent;
    [SerializeField] private GameObject universalCoin;

    [Header("Characters")]
    [SerializeField] private Animator redAnimator;
    [SerializeField] private Animator tigerAnimator;

    [Header("Polaroids")]
    [SerializeField] private Polaroid[] polaroids; // three in-game polaroids
    [SerializeField] private Transform polaroidParent;
    public Color normalGlow;

    [Header("Cards")]
    [SerializeField] private GameObject[] redCards;
    [SerializeField] private GameObject[] tigerCards;
    private int redCardCount = 0;
    private int tigerCardCount = 0;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform polaroid0Pos;
    [SerializeField] private Transform polaroid2Pos;
    [SerializeField] private Transform redPilePos;
    [SerializeField] private Transform tigerPilePos;
    [SerializeField] private Transform away0Pos;
    [SerializeField] private Transform away2Pos;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;

    // other variables
    private ChallengeWord currentWord;
    private List<ChallengeWord> currentWords;
    private Polaroid currentPolaroid;
    private List<UniversalCoinImage> currentCoins;
    private bool playingCoinAudio = false;
    private int numWins = 0;
    private int numMisses = 0;

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;


    [Header("Tutorial")]
    public bool playTutorial;
    private bool playIntro = false;
    public int[] correctTutorialIndex;
    private int tutorialEvent = 0;
    public List<ChallengeWord> polaroids1;
    public List<ChallengeWord> polaroids2;
    public List<ChallengeWord> polaroids3;
    [Header("Scripted")]
    private int scriptedEvent = 0;
    public List<ChallengeWord> polaroidsScripted1;
    public List<ChallengeWord> polaroidsScripted2;
    public List<ChallengeWord> polaroidsScripted3;
    public List<ChallengeWord> polaroidsScripted4;
    public List<ChallengeWord> polaroidsScripted5;

    public bool testthis;



    [Header("Testing")]
    public bool overridePool;
    public List<ChallengeWord> testChallengeWords;
    public int correctIndex;

    

    private void Start() 
    {
        if (instance == null)
            instance = this;
        
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

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
                // save tutorial done to SIS
                StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
                // times missed set to 0
                numMisses = 0;
                // update AI data
                AIData(StudentInfoSystem.GetCurrentProfile());
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(CalculateStars());
                // remove all raycast blockers
                RaycastBlockerController.instance.ClearAllRaycastBlockers();
            }
        }
    }

    private void PregameSetup()
    {   
        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial;

        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // create word lists based on unlocked action words
        globalWordList = ChallengeWordDatabase.GetChallengeWords(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

        // disable card objects
        foreach (var card in redCards)
        {
            card.SetActive(false);
        }
        foreach (var card in tigerCards)
        {
            card.SetActive(false);
        }

        // begin first round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = false;

        // place polaroids in start pos + reset alphas
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
            pol.SetPolaroidAlpha(1f, 0f);
        }

        // set frames to be invisible
        frameGroup.spacing = frameSpacing[0];
        foreach (var frame in frames)
        {
            frame.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        // initialize list
        currentWords = new List<ChallengeWord>();
        currentWords.Clear();

        // tutorial stuff
        if (playTutorial)
        {
            // short pause before start
            yield return new WaitForSeconds(1f);

            if (tutorialEvent == 0)
            {
                // play tutorial intro 1
                AudioClip clip = GameIntroDatabase.instance.blendingIntro1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play tutorial intro 2
                clip = GameIntroDatabase.instance.blendingIntro2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
            else
            {
                // play tutorial intro 3
                AudioClip clip = GameIntroDatabase.instance.blendingIntro3;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // // play tutorial intro 4
                // clip = GameIntroDatabase.instance.blendingIntro4;
                // TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                // yield return new WaitForSeconds(clip.length + 1f);                
            }
        }
        else
        {
            if (!playIntro)
            {
                // short pause before start
                yield return new WaitForSeconds(1f);

                // play start 1
                AudioClip clip = GameIntroDatabase.instance.blendingStart1;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);

                // play start 2
                clip = GameIntroDatabase.instance.blendingStart2;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
        }

        // add menu button
        if (!playIntro)
        {
            playIntro = true;
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        // get challenge words
        if (playTutorial)
        {
            // get correct tutorial polaroids
            List<ChallengeWord> tutorialList = new List<ChallengeWord>();
            switch (tutorialEvent)
            {
                case 0:
                    tutorialList.AddRange(polaroids1);
                    currentWords.AddRange(polaroids1);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 1:
                    tutorialList.AddRange(polaroids2);
                    currentWords.AddRange(polaroids2);
                    correctIndex = correctTutorialIndex[1];
                    break;
                case 2:
                    tutorialList.AddRange(polaroids3);
                    currentWords.AddRange(polaroids3);
                    correctIndex = correctTutorialIndex[2];
                    break;
            }
            tutorialEvent++;

            // set tutorial polaroids
            for (int i = 0; i < 3; i++)
            {
                polaroids[i].SetPolaroid(tutorialList[i]);
            }

            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }
        else if (overridePool)
        {
            currentWord = testChallengeWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];

            // use override words
            int count = 0;
            foreach (var pol in polaroids)
            {
                pol.SetPolaroid(testChallengeWords[count]);
                count++;
            }
        }
        else if(StudentInfoSystem.GetCurrentProfile().blendPlayed == 0 || testthis)
        {
            // get correct tutorial polaroids
            List<ChallengeWord> scriptedList = new List<ChallengeWord>();
            switch (scriptedEvent)
            {
                case 0:
                    scriptedList.AddRange(polaroidsScripted1);
                    currentWords.AddRange(polaroidsScripted1);
                    correctIndex = correctTutorialIndex[1];
                    break;
                case 1:
                    scriptedList.AddRange(polaroidsScripted2);
                    currentWords.AddRange(polaroidsScripted2);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 2:
                    scriptedList.AddRange(polaroidsScripted3);
                    currentWords.AddRange(polaroidsScripted3);
                    correctIndex = correctTutorialIndex[0];
                    break;
                case 3:
                    scriptedList.AddRange(polaroidsScripted4);
                    currentWords.AddRange(polaroidsScripted4);
                    correctIndex = correctTutorialIndex[2];
                    break;
                case 4:
                    scriptedList.AddRange(polaroidsScripted5);
                    currentWords.AddRange(polaroidsScripted5);
                    correctIndex = correctTutorialIndex[1];
                    break;
            }
            scriptedEvent++;

            // set tutorial polaroids
            for (int i = 0; i < 3; i++)
            {
                polaroids[i].SetPolaroid(scriptedList[i]);
            }

            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }
        else
        {   
            // use random words
            foreach (var polaroid in polaroids)
            {
                List<ChallengeWord> tempChallengeWordList = new List<ChallengeWord>();
                tempChallengeWordList = AISystem.ChallengeWordSelectionBlending(StudentInfoSystem.GetCurrentProfile());
                ChallengeWord correctWord = tempChallengeWordList[0];
                ChallengeWord incorrectWord1 = tempChallengeWordList[1];
                ChallengeWord incorrectWord2 = tempChallengeWordList[2];
                int randIndex = Random.Range(0, tempChallengeWordList.Count);
                //ChallengeWord word = GetUnusedWord();
                if(randIndex == 0)
                {
                    correctIndex = randIndex;
                }
                polaroid.SetPolaroid(tempChallengeWordList[randIndex]);
                currentWords.Add(tempChallengeWordList[randIndex]);
                tempChallengeWordList.RemoveAt(randIndex);
            }

            
            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }

        // show polaroids
        yield return new WaitForSeconds(1f);
        StartCoroutine(ShowPolaroids());
        yield return new WaitForSeconds(3f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 5
            AudioClip clip = GameIntroDatabase.instance.blendingIntro5;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 6
            clip = GameIntroDatabase.instance.blendingIntro6;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 7
            clip = GameIntroDatabase.instance.blendingIntro7;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            frames[5 - i].gameObject.SetActive(false);
        }
        
        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[currentWord.elkoninCount - 1], 0f));

        // show frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(frames[i].GetComponent<Image>(), 1f, 0.25f));
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);

        currentCoins = new List<UniversalCoinImage>();
        yield return new WaitForSeconds(1f);

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, frames[i].position, Quaternion.identity, coinParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.transform.localScale = new Vector3(0f, 0f, 1f);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            currentCoins.Add(coin);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play audio in frame order
            for (int i = 0; i < currentWord.elkoninCount; i++)
            {
                StartCoroutine(PlayAudioCoinRoutine(currentCoins[i]));
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(SelectedPolaroid(currentPolaroid));

            yield return new WaitForSeconds(0.5f);

            // play tutorial intro 8
            AudioClip clip = GameIntroDatabase.instance.blendingIntro8;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            TogglePolaroidsWiggle(true);

            // remove raycast blocker
            RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
            WordFactoryRaycaster.instance.isOn = true;
        }
        else
        {
            StartCoroutine(StartRound());
        }   
    }

    public void TogglePolaroidsWiggle(bool opt)
    {
        // wiggle polaroids
        foreach(Polaroid polaroid in polaroids)
        {
            polaroid.ToggleWiggle(opt);
        }
    }

    private IEnumerator StartRound()
    {
        // play audio in frame order
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(PlayAudioCoinRoutine(currentCoins[i]));
            yield return new WaitForSeconds(1f);
        }

        TogglePolaroidsWiggle(true);

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = true;
    }

    private ChallengeWord GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];

        // make sure word is not being used
        if (usedWordList.Contains(word))
        {
            unusedWordList.Remove(word);
            return GetUnusedWord();
        }

        unusedWordList.Remove(word);
        usedWordList.Add(word);
        return word;
    }

    public bool EvaluatePolaroid(Polaroid polaroid)
    {
        WordFactoryRaycaster.instance.isOn = false;

        if (!playTutorial)
            StartCoroutine(SelectedPolaroid(polaroid));

        if (polaroid.challengeWord == currentWord)
        {
            numWins++;
            currentPolaroid = polaroid;
            StartCoroutine(CorrectPolaroidRoutine());
            return true;
        }
        // wrong answer
        numMisses++;
        StartCoroutine(FailPolaroidRoutine());
        return false;
    }

    private IEnumerator SelectedPolaroid(Polaroid polaroid)
    {
        yield return new WaitForSeconds(0.5f);

        // select polaroid
        foreach (Polaroid pol in polaroids)
        {
            if (pol.challengeWord == polaroid.challengeWord)
            {
                pol.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.3f);
            }
            else
            {
                pol.GetComponent<LerpableObject>().LerpScale(new Vector2(0.6f, 0.6f), 0.3f);
            }
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator CorrectPolaroidRoutine()
    {
        //yield return new WaitForSeconds(2f);
        
        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine(true));
        yield return new WaitForSeconds(2f);

        // ####################

        // hide coins + frames
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideCoinsAndFrames());

        // animate cards away
        int count = 0;
        foreach(var polaroid in polaroids)
        {
            if (count == 0)
            {
                polaroid.MovePolaroid(away0Pos.position, 0.25f);
            }
            else if (count == 1)
            {
                polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
            }
            else if (count == 2)
            {
                polaroid.MovePolaroid(away2Pos.position, 0.25f);
            }
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.1f);
            count++;
        }
    
        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award red new card
        redCards[redCardCount].SetActive(true);
        // animate card init
        switch (redCardCount)
        {
            case 0:
                redCards[redCardCount].GetComponent<Animator>().Play("Card1");
                break;
            case 1:
                redCards[redCardCount].GetComponent<Animator>().Play("Card2");
                break;
            case 2:
                redCards[redCardCount].GetComponent<Animator>().Play("Card3");
                break;
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 0.5f);
        yield return new WaitForSeconds(0.1f);
        redCardCount++;
        
        // animate characters
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 9
            AudioClip clip = GameIntroDatabase.instance.blendingIntro9;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 10
            clip = GameIntroDatabase.instance.blendingIntro10;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 11
            clip = GameIntroDatabase.instance.blendingIntro11;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else
        {
            // play encouragement popup
            // julius
            int index = Random.Range(0, 2);
            AudioClip clip = null;
            if (index == 0)
            {
                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_ugh");
            }
            else if (index == 1)
            {
                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("julius_grr");
            }
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // red
            index = Random.Range(0, 3);
            clip = null;
            if (index == 0)
            {
                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_woohoo");
            }
            else if (index == 1)
            {
                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_hurrah");
            }
            else if (index == 2)
            {
                clip = TalkieDatabase.instance.GetTalkieReactionDuplicate("red_uhhuh");
            }
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }

        // win game iff 3 or more rounds have been won
        if (numWins >= 3)
            StartCoroutine(WinGameRoutine());
        // else continue to next round
        else 
            StartCoroutine(NewRound());
    }

    private IEnumerator FailPolaroidRoutine()
    {
        if (playTutorial)
        {
            WordFactoryRaycaster.instance.isOn = true;
            yield break;
        }

        yield return new WaitForSeconds(2f);

        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine(false));
        yield return new WaitForSeconds(2f);

        // ####################

        // hide coins + frames
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideCoinsAndFrames());

        // animate cards away
        int count = 0;
        foreach(var polaroid in polaroids)
        {
            if (count == 0)
            {
                polaroid.MovePolaroid(away0Pos.position, 0.25f);
            }
            else if (count == 1)
            {
                polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
            }
            else if (count == 2)
            {
                polaroid.MovePolaroid(away2Pos.position, 0.25f);
            }
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award tiger new card
        tigerCards[tigerCardCount].SetActive(true);
        // animate card init
        switch (tigerCardCount)
        {
            case 0:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card1");
                break;
            case 1:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card2");
                break;
            case 2:
                tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card3");
                break;
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
        yield return new WaitForSeconds(0.1f);
        tigerCardCount++;
        
        // animate characters
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");

        yield return new WaitForSeconds(1f);

        // play reminder popup
        List<AudioClip> clips = GameIntroDatabase.instance.blendingReminderClips;
        AudioClip clip = clips[Random.Range(0, clips.Count)];
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
        yield return new WaitForSeconds(clip.length + 1f);

        if (numMisses >= 3)
            StartCoroutine(LoseGameRoutine());
        else
            StartCoroutine(NewRound());
    }

    private IEnumerator HideCoinsAndFrames()
    {
        // remove coin glows
        foreach (var coin in currentCoins)
        {
            coin.ToggleGlowOutline(false);
            yield return new WaitForSeconds(0.05f);
        }

        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[0], 1f));

        // hide frames
        foreach (var frame in frames)
        {
            StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 0f, 0.25f));
        }

        // make coins invisible
        foreach (var coin in currentCoins)
        {
            coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);

        // deactivate all frames
        foreach (var frame in frames)
        {
            frame.gameObject.SetActive(true);
        }
    }

    private IEnumerator LoseGameRoutine()
    {
        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinGameRoutine()
    {
        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);


        if (playTutorial)
        {
            // short pause before start
            yield return new WaitForSeconds(1f);

            // play end 1
            AudioClip clip = GameIntroDatabase.instance.blendingEnd1;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play end 2
            clip = GameIntroDatabase.instance.blendingEnd2;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("WordFactoryBlending", true, 3f);
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
        playerData.blendPlayed = playerData.blendPlayed + 1;
        playerData.starsBlend = CalculateStars() + playerData.starsBlend;
        
        // save to SIS
        StudentInfoSystem.SaveStudentPlayerData();
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

    private IEnumerator PolaroidRevealRoutine(bool isCorrect)
    {
        // read word aloud to player
        if (currentWord.audio != null)
            AudioManager.instance.PlayTalk(currentWord.audio);

        // glow coins fast
        foreach (var coin in currentCoins)
        {
            yield return new WaitForSeconds(0.05f);
        }

        // wait an appropriate amount of time
        if (currentWord.audio != null)
            yield return new WaitForSeconds(currentWord.audio.length + 0.25f);
        else 
            yield return new WaitForSeconds(2f);

        // reveal correct polaroid 
        currentPolaroid.ToggleGlowOutline(true);
        if (isCorrect)
        {
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
        }
        else
        {
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);
        }


        // make other polaroids transparent
        foreach (Polaroid polaroid in polaroids)
        {
            if (polaroid != currentPolaroid)
                polaroid.SetPolaroidAlpha(0.2f, 0.5f);

            // re-do scales to show correct polaroid
            if (polaroid.challengeWord == currentWord)
            {
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.25f);
            }
            else
            {
                polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(0.6f, 0.6f), 0.25f);
            }
        }
    }

    public void ResetPolaroids(bool instant = false, bool startPos = false)
    {
        float timeLerp = 0.1f;
        if (instant)
            timeLerp = 0f;

        // reset polaroid scale
        polaroids[0].LerpScale(1f, timeLerp);
        polaroids[1].LerpScale(1f, timeLerp);
        polaroids[2].LerpScale(1f, timeLerp);

        // return to start position
        if (startPos)
        {
            polaroids[0].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroidStartPos.position, timeLerp);
        }
        // return to original position
        else 
        {
            polaroids[0].MovePolaroid(polaroid0Pos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidLandPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroid2Pos.position, timeLerp);
        }
        

        // set appropriate parent
        polaroids[0].transform.SetParent(polaroidParent);
        polaroids[1].transform.SetParent(polaroidParent);
        polaroids[2].transform.SetParent(polaroidParent);

        // remove glow
        polaroids[0].ToggleGlowOutline(false);
        polaroids[1].ToggleGlowOutline(false);
        polaroids[2].ToggleGlowOutline(false);
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1f);
        coin.LerpSize(normalCoinSize, 0.25f);

        playingCoinAudio = false;
    }

    private IEnumerator LerpImageAlpha(Image image, float targetAlpha, float totalTime)
    {
        float start = image.color.a;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                image.color = new Color(1f, 1f, 1f, targetAlpha);
                break;
            }
            
            float tempAlpha = Mathf.Lerp(start, targetAlpha, timer / totalTime);
            image.color = new Color(1f, 1f, 1f, tempAlpha);
            yield return null;
        }
    }

    private IEnumerator LerpFrameSpacing(float target, float totalTime)
    {
        float start = frameGroup.spacing;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                frameGroup.spacing = target;
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            frameGroup.spacing = temp;
            yield return null;
        }
    }

    private IEnumerator ShowPolaroids()
    {   
        // hide polaroid images
        foreach (var polaroid in polaroids)
        {
            polaroid.HideImage(0f);
        }

        // short 0.5s delay
        yield return new WaitForSeconds(0.5f);
        
        Vector3 bouncePos = polaroidLandPos.position;
        bouncePos.y -= 0.5f;
        // move polaroids down  
        foreach (var polaroid in polaroids)
        {
            polaroid.MovePolaroid(bouncePos, 0.3f);
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidFall, 0.5f);
        yield return new WaitForSeconds(0.3f);
        // move polaroids down  
        foreach (var polaroid in polaroids)
        {
            polaroid.MovePolaroid(polaroidLandPos.position, 0.2f);
        }

        yield return new WaitForSeconds(0.5f);

        // move each polaroid to their respective spot
        polaroids[0].MovePolaroid(polaroid0Pos.position, 0.5f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.15f);
        polaroids[2].MovePolaroid(polaroid2Pos.position, 0.5f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.8f);

        // reveal polaroid images
        foreach (var polaroid in polaroids)
        {
            polaroid.RevealImage(0.25f);
        }
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CameraClick, 1f);
        // scale polaroids when revealing images
        foreach (var polaroid in polaroids)
        {
            polaroid.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        }
    }
}