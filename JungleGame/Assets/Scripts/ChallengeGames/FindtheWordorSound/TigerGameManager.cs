using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TigerGameManager : MonoBehaviour
{
    public static TigerGameManager instance;

    [SerializeField] private TigerPawController Tiger;
    [SerializeField] private PatternRightWrong pattern;
    [SerializeField] private List<Polaroid> polaroidC;
    [SerializeField] private TigerGameRaycaster caster;
    [SerializeField] private Transform selectedObjectParentCoin;
    [SerializeField] private List<GameObject> correctCoins;
    [SerializeField] private List<GameObject> incorrectCoins;

    [Header("Positions")]
    [SerializeField] private Transform coinStartPos;
    [SerializeField] private Transform coinLandPos;
    [SerializeField] private Transform PhotoPos1;
    [SerializeField] private Transform PhotoPos2;
    [SerializeField] private Transform PhotoPos3;
    [SerializeField] private Transform PhotoPos4;
    [SerializeField] private Transform PhotoPos5;
    [SerializeField] private Transform PhotoStartPos;
    [SerializeField] private Transform PhotoEndPos;

    public UniversalCoinImage currCoin;
    private ActionWordEnum currSet; 
    private ChallengeWord currWord;

    // other variables
    private ChallengeWord currentWord1;
    private ChallengeWord currentWord2;
    private ChallengeWord currentWord3;
    private ChallengeWord currentWord4;
    private ChallengeWord currentWord5;
    private int currentSwipeIndex;

    private List<UniversalCoinImage> currentCoins;
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
    public ActionWordEnum tutorialSet1;
    public ActionWordEnum tutorialSet2;
    public ActionWordEnum tutorialSet3;
    public List<ChallengeWord> tutorialWords1;
    public List<ChallengeWord> tutorialWords2;
    public List<ChallengeWord> tutorialWords3;

    [Header("Scripted")]
    private int scriptedEvent = 0;
    public List<ChallengeWord> polaroidsScripted1;
    public List<ChallengeWord> polaroidsScripted2;
    public List<ChallengeWord> polaroidsScripted3;
    public List<ChallengeWord> polaroidsScripted4;
    public List<ChallengeWord> polaroidsScripted5;

    public bool testthis;
    List<ChallengeWord> word_pool = new List<ChallengeWord>();
    

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
            playTutorial = !StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial;

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
                StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
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

        // set coin off-screen
        currCoin.transform.position = coinStartPos.position;

        // set base pattern
        pattern.baseState();

        // turn off raycaster
        TigerGameRaycaster.instance.isOn = false;

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {   
        currCoin.transform.position = coinStartPos.position;

        if (playTutorial)
        {
            switch (tutorialEvent)
            {
                case 0:
                    currSet = tutorialSet1;
                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords1[i - 1]);
                    }
                    break;

                case 1:
                    currSet = tutorialSet2;
                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords2[i - 1]);
                    }
                    break;

                case 2:
                    currSet = tutorialSet3;

                    for (int i = 1; i < 4; i++)
                    {
                        polaroidC[i].SetPolaroid(tutorialWords3[i - 1]);
                    }
                    break;
            }
            tutorialEvent++;
        }   
        else if((StudentInfoSystem.GetCurrentProfile().tPawPolPlayed == 0 || testthis))
        {
            // get correct tutorial polaroids
            List<ChallengeWord> scriptedList = new List<ChallengeWord>();
            switch (scriptedEvent)
            {
                case 0:
                    scriptedList.AddRange(polaroidsScripted1);
                    currWord = polaroidsScripted1[4];
                    currSet = ActionWordEnum.mudslide;
                    
                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted1[i]);
                    }
                    break;

                case 1:
                    scriptedList.AddRange(polaroidsScripted2);
                    currWord = polaroidsScripted2[2];
                    currSet = ActionWordEnum.orcs;
                    
                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted2[i]);
                    }
                    break;

                case 2:
                    scriptedList.AddRange(polaroidsScripted3);
                    currWord = polaroidsScripted3[1];
                    currSet = ActionWordEnum.poop;
                    
                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted3[i]);
                    }
                    break;

                case 3:
                    scriptedList.AddRange(polaroidsScripted4);
                    currWord = polaroidsScripted4[4];
                    currSet = ActionWordEnum.listen;
                    
                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted4[i]);
                    }
                    break;

                case 4:
                    scriptedList.AddRange(polaroidsScripted5);
                    currWord = polaroidsScripted5[3];
                    currSet = ActionWordEnum.explorer;
                    
                    for (int i = 0; i < polaroidC.Count; i++)
                    {
                        polaroidC[i].SetPolaroid(polaroidsScripted5[i]);
                    }
                    break;
            }
            scriptedEvent++;
        }
        else
        {
            currSet = AISystem.TigerPawPhotosCoinSelection(StudentInfoSystem.GetCurrentProfile());
            word_pool = AISystem.ChallengeWordSelectionTigerPawPol(StudentInfoSystem.GetCurrentProfile(), currSet);

            for (int i = 0; i < polaroidC.Count; i++)
            {
                int randomIndex = Random.Range(0, word_pool.Count);
                polaroidC[i].SetPolaroid(word_pool[randomIndex]);
                word_pool.RemoveAt(randomIndex);
            }
        }

        pattern.baseState();

        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 1-2
            List<AudioClip> clips = new List<AudioClip>();
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro1);
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro2);
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);

            // play tutorial intro 3
            AudioClip clip = GameIntroDatabase.instance.tigerPawPhotosIntro3;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);

            // play tutorial intro 4
            clip = GameIntroDatabase.instance.tigerPawPhotosIntro4;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Julius, clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
        else if (!playIntro)
        {
            // play start 1
            AudioClip clip = GameIntroDatabase.instance.tigerPawPhotosStart;
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

        if (!playIntro)
        {
            playIntro = true;
            // turn on settings button
            SettingsManager.instance.ToggleMenuButtonActive(true);
        }

        
        Tiger.TigerDeal();
        yield return new WaitForSeconds(.6f);
        currCoin.gameObject.transform.position = coinLandPos.position;
        currCoin.SetActionWordValue(currSet);
        yield return new WaitForSeconds(.5f);

        int startPolaroid = 0;
        int endPolaroid = 5;
        if (playTutorial)
        {
            startPolaroid = 1;
            endPolaroid = 4;
        }

        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoStartPos.position, 0f));
        }
        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid), .2f));
        }
        yield return new WaitForSeconds(.35f);

        for (int i = 1 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 1), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 2 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 2), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 3 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 3), .2f));
        }
        yield return new WaitForSeconds(.11f);

        for (int i = 4 + startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 4), .2f));
        }

        PlayAudioCoin(currCoin);

        yield return new WaitForSeconds(1f);

        // tutorial stuff
        if (playTutorial && tutorialEvent == 1)
        {
            // play tutorial intro 5-6
            List<AudioClip> clips = new List<AudioClip>();
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro5);
            clips.Add(GameIntroDatabase.instance.tigerPawPhotosIntro6);
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Julius, clips);
            yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);
        }

        // disable raycaster
        TigerGameRaycaster.instance.isOn = true;
    }

    private Vector3 GetPolaroidPosition(int index)
    {
        switch (index)
        {
            default: return PhotoStartPos.position;
            case 0: return PhotoPos1.position;
            case 1: return PhotoPos2.position;
            case 2: return PhotoPos3.position;
            case 3: return PhotoPos4.position;
            case 4: return PhotoPos5.position;
        }
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

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        StartCoroutine(PlayAudioCoinRoutine(coin));
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        // glow coin
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);

        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);
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

    public void EvaluateWaterCoin(Polaroid Photo)
    {
        // disable raycaster + stop coroutines
        TigerGameRaycaster.instance.isOn = false;
        TigerGameRaycaster.instance.EndAudioRoutine();

        if (Photo.challengeWord.set == currSet)
        {
            StartCoroutine(PostRound(true));
        }
        else
        {
            StartCoroutine(PostRound(false));
        }
    }

    public void returnToPos(GameObject currPhoto)
    {

        currPhoto.gameObject.transform.SetParent(selectedObjectParentCoin);

        if (currPhoto.name == "Polaroid1")
        {
            StartCoroutine(LerpMoveObject(polaroidC[0].transform, PhotoPos1.position, .2f));
        }
        else if (currPhoto.name == "Polaroid2")
        {
            StartCoroutine(LerpMoveObject(polaroidC[1].transform, PhotoPos2.position, .2f));
        }
        else if (currPhoto.name == "Polaroid3")
        {
            StartCoroutine(LerpMoveObject(polaroidC[2].transform, PhotoPos3.position, .2f));
        }
        else if (currPhoto.name == "Polaroid4")
        {
            StartCoroutine(LerpMoveObject(polaroidC[3].transform, PhotoPos4.position, .2f));
        }
        else if (currPhoto.name == "Polaroid5")
        {
            StartCoroutine(LerpMoveObject(polaroidC[4].transform, PhotoPos5.position, .2f));
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
                TigerGameRaycaster.instance.isOn = true;
                yield break;
            }

            pattern.incorrect();
            incorrectCoins[numMisses].SetActive(true);
            numMisses++;
        }

        yield return new WaitForSeconds(1f);
        Tiger.TigerDeal();
        yield return new WaitForSeconds(.5f);
        currCoin.gameObject.transform.position = coinStartPos.position;
        yield return new WaitForSeconds(1f);

        int startPolaroid = 0;
        int endPolaroid = 5;
        if (playTutorial)
        {
            startPolaroid = 1;
            endPolaroid = 4;
        }

        for (int i = startPolaroid; i < endPolaroid - 4; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 3; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 1), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 2; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 2), .2f));
        }
        yield return new WaitForSeconds(.1f);
        for (int i = startPolaroid; i < endPolaroid - 1; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, GetPolaroidPosition(startPolaroid + 3), .2f));
        }
        yield return new WaitForSeconds(.2f);
        for (int i = startPolaroid; i < endPolaroid; i++)
        {
            StartCoroutine(LerpMoveObject(polaroidC[i].transform, PhotoEndPos.position, .2f));
        }


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

                if (playTutorial)
                {
                    clip = GameIntroDatabase.instance.tigerPawPhotosIntro8;
                }
                else
                {
                    if (StudentInfoSystem.GetCurrentProfile().currentChapter < Chapter.chapter_5)
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                    }   
                    else
                    {
                        clip = GameIntroDatabase.instance.tigerPawCoinFinalJuliusLoseChapters1_4;
                    }   
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


        if (numWins >= 3)
        {
            StartCoroutine(WinRoutine());
        }
        else if (numMisses >= 3)
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
            StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("TigerPawPhotos", true, 3f);
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
        playerData.tPawPolPlayed = playerData.tPawPolPlayed + 1;
        playerData.starsTPawPol= CalculateStars() + playerData.starsTPawPol;

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
