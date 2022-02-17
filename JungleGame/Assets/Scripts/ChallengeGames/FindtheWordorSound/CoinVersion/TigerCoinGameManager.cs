using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerCoinGameManager : MonoBehaviour
{
    public static TigerCoinGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private Polaroid polaroidC;
    [SerializeField] private TigerCoinRayCaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;
    private bool gameSetup = false;


    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform CoinPos1;
    [SerializeField] private Transform CoinPos2;
    [SerializeField] private Transform CoinPos3;
    [SerializeField] private Transform CoinPos4;
    [SerializeField] private Transform CoinPos5;
    [SerializeField] private Transform CoinStartPos;
    [SerializeField] private Transform CoinEndPos;

    [SerializeField] private List<UniversalCoinImage> waterCoins;
    

    // other variables
    [SerializeField]  private ChallengeWord currentWord;
    [SerializeField]  private ElkoninValue currentTargetValue;
    [SerializeField]  private ChallengeWord currentTargetWord;

    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;


    [Header("Tutorial")]
    public bool playTutorial;
    private int tutorialEvent = 0;
    private bool playIntro = false;
    public ChallengeWord tutorialPhoto1;
    public ChallengeWord tutorialPhoto2;
    public ChallengeWord tutorialPhoto3;

    [Header("Scripted")]
    private int scriptedEvent = 0;
    public ChallengeWord coinsScripted1;
    public ChallengeWord coinsScripted2;
    public ChallengeWord coinsScripted3;
    public ChallengeWord coinsScripted4;
    public ChallengeWord coinsScripted5;

    
    

    public bool testthis;
    

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // add ambiance
        // AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        // AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // only turn off tutorial if false
        if (!playTutorial)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial;

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
                // save tutorial done to SIS
                StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
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
        globalWordList = new List<ChallengeWord>();
        globalWordList.AddRange(ChallengeWordDatabase.GetChallengeWords(StudentInfoSystem.GetCurrentProfile().actionWordPool));
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();
        pattern.baseState();

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // reset coin positions
        foreach (var coin in waterCoins)
            coin.transform.position = CoinStartPos.position;

        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currentWord = tutorialPhoto1;
                    break;

                case 1:
                    currentWord = tutorialPhoto2;
                    break;

                case 2:
                    currentWord = tutorialPhoto3;
                    break;
            }

            tutorialEvent++;
            polaroidC.SetPolaroid(currentWord);
            currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentWord.set);
            int rand = Random.Range(0,5);
            waterCoins[rand].SetValue(currentTargetValue);
        }

        //Scripted Tiger Paw Photo
        else if (StudentInfoSystem.GetCurrentProfile().tPawCoinPlayed == 0 || testthis)
        {
            // get correct tutorial polaroids
            switch (scriptedEvent)
            {
                case 0:
                    polaroidC.SetPolaroid(coinsScripted1);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted1.set);
                    waterCoins[0].SetValue(ElkoninValue.explorer);
                    waterCoins[1].SetValue(ElkoninValue.hello);
                    waterCoins[2].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.poop);
                    waterCoins[4].SetValue(ElkoninValue.think); 
                    break;
                case 1:
                    polaroidC.SetPolaroid(coinsScripted2);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted2.set);
                    waterCoins[0].SetValue(ElkoninValue.mudslide);
                    waterCoins[1].SetValue(ElkoninValue.listen);
                    waterCoins[3].SetValue(currentTargetValue);
                    waterCoins[2].SetValue(ElkoninValue.explorer);
                    waterCoins[4].SetValue(ElkoninValue.poop); 
                    break;
                case 2:
                    polaroidC.SetPolaroid(coinsScripted3);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted3.set);
                    waterCoins[2].SetValue(ElkoninValue.poop);
                    waterCoins[1].SetValue(ElkoninValue.hello);
                    waterCoins[0].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.think);
                    waterCoins[4].SetValue(ElkoninValue.listen); 
                    break;
                case 3:
                    polaroidC.SetPolaroid(coinsScripted4);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted4.set);
                    waterCoins[0].SetValue(ElkoninValue.poop);
                    waterCoins[1].SetValue(ElkoninValue.think);
                    waterCoins[4].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.orcs);
                    waterCoins[2].SetValue(ElkoninValue.hello); 
                    break;
                case 4:
                    polaroidC.SetPolaroid(coinsScripted5);
                    currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(coinsScripted5.set);
                    waterCoins[0].SetValue(ElkoninValue.orcs);
                    waterCoins[1].SetValue(ElkoninValue.listen);
                    waterCoins[2].SetValue(currentTargetValue);
                    waterCoins[3].SetValue(ElkoninValue.explorer);
                    waterCoins[4].SetValue(ElkoninValue.mudslide); 
                    break;
            }
            scriptedEvent++;
        }
        else
        {
            // get random word
            List<ChallengeWord> CurrentChallengeList = new List<ChallengeWord>();
            CurrentChallengeList = AISystem.ChallengeWordSelectionTigerPawCoin(StudentInfoSystem.GetCurrentProfile());
            currentWord = CurrentChallengeList[0];
            polaroidC.SetPolaroid(currentWord);
            

            //currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentWord.set);

            //print ("current value: " + currentTargetValue);

            // set coin options
            List<ActionWordEnum> coinOptions = new List<ActionWordEnum>();
            //coinOptions.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
            //coinOptions.Remove(currentWord.set);
            coinOptions = AISystem.TigerPawCoinsCoinSelection(StudentInfoSystem.GetCurrentProfile(), CurrentChallengeList);

            //print ("coin options: " + coinOptions.Count);

            //int correctIndex = Random.Range(0, waterCoins.Count);
            currentTargetValue = ChallengeWordDatabase.ActionWordEnumToElkoninValue(currentWord.set);
            for (int i = 0; i < 5; i++)
            {
                int rand = Random.Range(0, coinOptions.Count);
                waterCoins[i].SetActionWordValue(coinOptions[rand]);
                coinOptions.RemoveAt(rand);
            }
        }


        


        print ("current value: " + currentTargetValue);

        yield return new WaitForSeconds(0.5f);

        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        polaroidC.LerpScale(0f, 0f);

        // return pattern to base state
        pattern.baseState();
        
        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 1-2
            List<AudioClip> clips = new List<AudioClip>();
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro1);
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro2);
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);

            // play tutorial intro 3
            AudioClip clip = GameIntroDatabase.instance.tigerPawCoinIntro3;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 4
            clip = GameIntroDatabase.instance.tigerPawCoinIntro4;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else if (!playIntro)
        {
            playIntro = true;

            // short pause before start
            yield return new WaitForSeconds(1f);

            // play start 1
            AudioClip clip = GameIntroDatabase.instance.tigerPawCoinStart;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else
        {
            AudioClip clip = null;

            if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
            {
                clip = GameIntroDatabase.instance.tigerPawCoinNewPhotosChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinNewPhotosChapters1_4.Count)];
            }   
            else
            {
                clip = GameIntroDatabase.instance.tigerPawCoinNewPhotosChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinNewPhotosChapter5.Count)];
            }       

            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }

        Tiger.TigerDeal();
        polaroidC.gameObject.transform.position = polaroidLandPos.position;
        yield return new WaitForSeconds(.6f);

        polaroidC.LerpScale(1f, 0f);
        yield return new WaitForSeconds(0.85f);


        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos1.position, .2f));
        }
        yield return new WaitForSeconds(.15f);
        for (int i = 1; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos2.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 2; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos3.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 3; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos4.position, .2f));
        }
        yield return new WaitForSeconds(.08f);
        for (int i = 4; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos5.position, .2f));
        }

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            yield return new WaitForSeconds(1f);

            // play tutorial intro 5-6
            List<AudioClip> clips = new List<AudioClip>();
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro5);
            clips.Add(GameIntroDatabase.instance.tigerPawCoinIntro6);
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);
        }

        // turn on raycaster
        TigerCoinRayCaster.instance.isOn = true;
    }

    private IEnumerator LerpMoveObject(Transform obj, Vector3 target, float lerpTime)
    {
        Vector3 start = obj.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                obj.position = target;
                break;
            }

            Vector3 tempPos = Vector3.Lerp(start, target, timer / lerpTime);
            obj.position = tempPos;
            yield return null;
        }
    }

    public void PlayCoinAudio(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (waterCoins.Contains(coin))
        {
            StartCoroutine(PlayCoinAudioRoutine(coin, true));
        }
    }

    private IEnumerator PlayCoinAudioRoutine(UniversalCoinImage coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);
        AudioManager.instance.PlayPhoneme(ChallengeWordDatabase.ElkoninValueToActionWord(coin.value));
        yield return new WaitForSeconds(0.5f);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);

        playingCoinAudio = false;
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

    public void EvaluateWaterCoin(UniversalCoinImage coin)
    {
        // turn off raycaster
        TigerCoinRayCaster.instance.isOn = false;

        if (coin.value == currentTargetValue)
        {
            StartCoroutine(PostRound(true));
        }
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    public void ReturnToPos(GameObject currCoin)
    {
        currCoin.gameObject.transform.SetParent(selectedObjectParentCoin);

        if (currCoin.name == "Coin1")
        {
            StartCoroutine(LerpMoveObject(waterCoins[0].transform, CoinPos1.position, .2f));
        }
        else if(currCoin.name == "Coin2")
        {
            StartCoroutine(LerpMoveObject(waterCoins[1].transform, CoinPos2.position, .2f));
        }
        else if (currCoin.name == "Coin3")
        {
            StartCoroutine(LerpMoveObject(waterCoins[2].transform, CoinPos3.position, .2f));
        }
        else if (currCoin.name == "Coin4")
        {
            StartCoroutine(LerpMoveObject(waterCoins[3].transform, CoinPos4.position, .2f));
        }
        else if (currCoin.name == "Coin5")
        {
            StartCoroutine(LerpMoveObject(waterCoins[4].transform, CoinPos5.position, .2f));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        if (win)
        {
            // play correct audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            pattern.correct();
            correctCoins[numWins].SetActive(true);
            numWins++; 
        }
        else
        {
            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            if (playTutorial)
            {
                // turn on raycaster
                TigerCoinRayCaster.instance.isOn = true;
                yield break;
            }

            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
        }

        Tiger.TigerDeal();
        yield return new WaitForSeconds(.4f);
        polaroidC.gameObject.transform.position = polaroidStartPos.position;
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 1; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos2.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos3.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos4.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinPos5.position, .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < 5; i++)
        {
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, CoinEndPos.position, .2f));
        }
        yield return new WaitForSeconds(0.5f);
        // reset coin positions
        foreach (var coin in waterCoins)
            coin.transform.position = CoinStartPos.position;

        // play appropriate popup
        if (win)
        {
            if (playTutorial && tutorialEvent == 1)
            {
                // play start 1
                AudioClip clip = GameIntroDatabase.instance.tigerPawCoinIntro7;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f); 
            }
            else if (numWins < 3)
            {
                // play julius lose popup
                AudioClip clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapters1_4.Count)];
                }   
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapter5[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusLoseChapter5.Count)];
                }       

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
            else
            {
                // play julius final lose popup
                AudioClip clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                }   
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                }       

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
        }
        else
        {
            if (numMisses < 3)
            {
                // play julius win popup
                AudioClip clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusWinChapters1_4[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusWinChapters1_4.Count)];
                }   
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinJuliusWinChapter5[Random.Range(0, GameIntroDatabase.instance.tigerPawCoinJuliusWinChapter5.Count)];
                }   

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
            else
            {
                // play julius final win popup
                AudioClip clip = null;

                if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusWinChapters1_4;
                }   
                else
                {
                    clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusWinChapters1_4;
                }       

                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clip);
                yield return new WaitForSeconds(clip.length + 1f);
            }
        }

        if (numWins == 3)
        {
            StartCoroutine(WinRoutine());
        }
        else if (numMisses == 3)
        {
           StartCoroutine(LoseRoutine());
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(1f);

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);
        

        if (playTutorial)
        {
            StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("TigerPawCoins", true, 3f);
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
        playerData.tPawCoinPlayed = playerData.tPawCoinPlayed + 1;
        playerData.starsTPawCoin = CalculateStars() + playerData.starsTPawCoin;
        
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
}
