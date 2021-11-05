using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{
    public static TurntablesGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;
    
    public List<Door> doors;
    public List<Key> keys;
    public FrameIcon frameIcon;
    public List<float> moveStonePitch;
    public float musicStartDelay;

    public bool randomizeKeyPosition;
    private ActionWordEnum[] doorWords;
    private int correctKeyIndex = 0;
    private int currentDoorIndex = 0;

    private float[] leftAngleArray = { 30, 45, 60, 90, 120, 135, 150 };
    private float[] rightAngleArray = { 210, 225, 240, 270, 300, 315, 330 };

    private List<ActionWordEnum> globalWordPool;
    private List<ActionWordEnum> unusedWordPool;

    private int timesMissed = 0;

    private bool gameStart;
    private const float animateKeysDownDelay = 0.3f;

    public Color finishedDoorColor;
    public Color winDoorColor;

    [Header("Tutorial")]
    public bool playInEditor;
    public bool playTutorial;

    private bool repeatTutorialAudio = false;
    private float timeBetweenRepeats = 8f;

    public int[] correctTutorialIcons;
    public List<ActionWordEnum> firstQuartet;
    public List<ActionWordEnum> secondQuartet;
    public List<ActionWordEnum> thirdQuartet;
    public List<ActionWordEnum> fourthQuartet;

    /* 
    ################################################
    #   MONOBEHAVIOR METHODS
    ################################################
    */

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music
        AudioManager.instance.StopMusic();

        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {   
        // get game data
        mapID = GameManager.instance.mapID;

        if (!playInEditor)
            playTutorial = !StudentInfoSystem.GetCurrentProfile().turntablesTutorial;

        PregameSetup();

        // play real game or tutorial
        if (playTutorial)
        {
            StartCoroutine(StartTutorial());
        }
        else
        {
            StartCoroutine(StartGame());
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
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(3);
            }
        }
    }

    /* 
    ################################################
    #   PREGAME SETUP
    ################################################
    */

    private void PregameSetup()
    {
        if (!playTutorial)
            StartCoroutine(StartMusicDelay(musicStartDelay));

        // start ambiance noise
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.BreezeLoop, 0.01f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.QuarryLoop, 0.01f);

        // remove glow from door icons
        foreach (Door d in doors)
            ImageGlowController.instance.SetImageGlow(d.image, false);
        // remove glow from rock lock
        ImageGlowController.instance.SetImageGlow(RockLock.instance.image, false);

        doorWords = new ActionWordEnum[4];
        globalWordPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (mapID != MapIconIdentfier.None)
        {
            print ("map ID");
            globalWordPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            print ("no map ID");
            globalWordPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }

        unusedWordPool = new List<ActionWordEnum>();
        unusedWordPool.AddRange(globalWordPool);

        // get keys
        RopeController.instance.InitNewRope();
        keys = RopeController.instance.GetKeys();

        // play tutorial if bool == true
        if (playTutorial)
        {
            // set tutorial icons
            for (int i = 0; i < 4; i++)
            {
                // get tutorial icon
                switch (i)
                {
                    case 0:
                        doorWords[i] = firstQuartet[correctTutorialIcons[i]];
                        break;
                    case 1:
                        doorWords[i] = secondQuartet[correctTutorialIcons[i]];
                        break;
                    case 2:
                        doorWords[i] = thirdQuartet[correctTutorialIcons[i]];
                        break;
                    case 3:
                        doorWords[i] = fourthQuartet[correctTutorialIcons[i]];
                        break;
                }

                // set door icon
                doors[i].SetDoorIcon(doorWords[i]);
            }
        }
        // normal minigame
        else
        {
            // set random icon to each door
            for (int i = 0; i < 4; i++)
            {
                // get random icon
                doorWords[i] = GetUnusedWord();
                // set door icon
                doors[i].SetDoorIcon(doorWords[i]);
            }
        }

        // remove door words from unused word pool
        foreach(var doorWord in doorWords)
        {
            unusedWordPool.Remove(doorWord);
        }

        // set up the keys
        KeySetup();
        // set first frame icon
        frameIcon.SetFrameIcon(doorWords[currentDoorIndex]);
        // make keys interactable
        SetKeysInteractable(true);
    }

    /* 
    ################################################
    #   MINIGAME METHODS
    ################################################
    */

    private IEnumerator StartGame()
    {
        List<float> leftAnglePool = new List<float>(leftAngleArray);
        List<float> rightAnglePool = new List<float>(rightAngleArray);

        bool direction = false;
        float duration = 3f;
        float difference = 0.5f;

        // set door angle
        for (int i = 3; i >= 0; i--)
        {
            float angle = 0f;
            if (i % 2 == 0)
            {
                int index = Random.Range(0, leftAnglePool.Count);
                angle = leftAnglePool[index];
            }
            else
            {
                int index = Random.Range(0, rightAnglePool.Count);
                angle = rightAnglePool[index];
            }

            doors[i].RotateToAngle(angle, direction, duration);
            direction = !direction;

            // play stone moving audio
            AudioManager.instance.PlayMoveStoneSound(duration - 0.4f, moveStonePitch[i]);
            
            yield return new WaitForSeconds(difference);
            duration -= difference;
        }

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);
        
        // toggle outline on
        GlowOutline(currentDoorIndex);
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();
        gameStart = true;
    }  

    // evaluate selected key
    public bool EvaluateSelectedKey(Key key)
    {
        // tutorial evaluation if in tutorial game
        if (playTutorial)
        {
            return EvaluateTutorialKey(key);
        }

        if (key.keyActionWord == doorWords[currentDoorIndex])
        {
            // play success audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyUnlock, 1f);
            AudioManager.instance.PlayKeyJingle();

            // success! go on to the next door or win game if on last door
            if (currentDoorIndex < 3)
                StartCoroutine(DoorSuccessRoutine());
            else
                StartCoroutine(WinRoutine());
            return true;
        }

        // play fail audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);
        AudioManager.instance.PlayKeyJingle();

        // fail go back to previous row
        StartCoroutine(DoorFailRoutine());
        return false;
    }

    private IEnumerator DoorSuccessRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // shake rock lock
        RockLock.instance.ShakeRock();
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // make door icon glow special
        ImageGlowController.instance.SetImageGlow(doors[currentDoorIndex].image, true, GlowValue.glow_1_025); // TODO change this back to blue glow?
        // play stone moving audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);

        // increment values
        currentDoorIndex++;
        // toggle outline on
        GlowOutline(currentDoorIndex);
        // change frame icon at the correct time
        StartCoroutine(DelayFrameIconChange(RopeController.instance.moveTime * 2, doorWords[currentDoorIndex]));
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        
        yield return new WaitForSeconds(RopeController.instance.moveTime);

        // get new keys
        RopeController.instance.InitNewRope();
        KeySetup();
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();

        // make keys interactable
        SetKeysInteractable(true);
    }

    private IEnumerator DoorFailRoutine()
    {
        // increment times missed
        timesMissed++;
        // return key
        keys[correctKeyIndex].ReturnToRope();
        
        yield return new WaitForSeconds(0.5f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        
        // change door to have a new icon
        ActionWordEnum newWord = GetUnusedWord();
        while (true)
        {
            print ("new word: " + newWord.ToString());
            bool wordOnDoor = false;
            foreach(var word in doorWords)
            {
                if (newWord == word)
                {
                    wordOnDoor = true;
                    break;
                }
            }

            // exit loop when found word not on the door
            if (wordOnDoor)
            {
                newWord = GetUnusedWord();
            }
            else
            {
                break;
            }
        }
        doorWords[currentDoorIndex] = newWord;

        // play icon switch audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LargeRockSlide, 1f);
        yield return new WaitForSeconds(0.2f);
        doors[currentDoorIndex].ShakeIconSwitch(newWord);

        // change frame icon at the correct time
        StartCoroutine(DelayFrameIconChange(doors[currentDoorIndex].shakeDuration / 2, doorWords[currentDoorIndex]));

        yield return new WaitForSeconds(RopeController.instance.moveTime);

        // get new keys
        RopeController.instance.InitNewRope();
        KeySetup();
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();

        // make keys interactable
        SetKeysInteractable(true);
    }

    private IEnumerator WinRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // shake rock lock
        RockLock.instance.ShakeRock();
        // make door icon glow special
        ImageGlowController.instance.SetImageGlow(doors[currentDoorIndex].image, true, GlowValue.glow_1_025); // TODO set blue?
        // play stone moving audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);

        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // win glow animation
        StartCoroutine(WinGlowAnimation(RopeController.instance.moveTime * 2));
        // play win audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();


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


    /* 
    ################################################
    #   TUTORIAL METHODS
    ################################################
    */

    private IEnumerator StartTutorial()
    {
        List<float> leftAnglePool = new List<float>(leftAngleArray);
        List<float> rightAnglePool = new List<float>(rightAngleArray);

        bool direction = false;
        float duration = 3f;
        float difference = 0.5f;

        // set door angle
        for (int i = 3; i >= 0; i--)
        {
            float angle = 0f;
            if (i % 2 == 0)
            {
                int index = Random.Range(0, leftAnglePool.Count);
                angle = leftAnglePool[index];
            }
            else
            {
                int index = Random.Range(0, rightAnglePool.Count);
                angle = rightAnglePool[index];
            }

            doors[i].RotateToAngle(angle, direction, duration);
            direction = !direction;

            // play stone moving audio
            AudioManager.instance.PlayMoveStoneSound(duration - 0.4f, moveStonePitch[i]);
            
            yield return new WaitForSeconds(difference);
            duration -= difference;
        }

        yield return new WaitForSeconds(2f);

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // play tutorial audio 1
        AudioClip clip = AudioDatabase.instance.TurntablesTutorial_1;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 2
        clip = AudioDatabase.instance.TurntablesTutorial_2;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // play tutorial audio 3
        clip = AudioDatabase.instance.TurntablesTutorial_3;
        AudioManager.instance.PlayTalk(clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // toggle outline on
        GlowOutline(currentDoorIndex);
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();
        // glow the correct key
        RopeController.instance.SetKeyGlow(keys[correctKeyIndex], true);
        gameStart = true;

        // make keys interactable
        SetKeysInteractable(true);
    }

    private IEnumerator RepeatTutorialAudioRoutine(AudioClip clip)
    {
        // play initially
        AudioManager.instance.PlayTalk(clip);

        // repeat until bool is false
        float timer = 0f;
        repeatTutorialAudio = true;
        while (repeatTutorialAudio)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenRepeats)
            {
                AudioManager.instance.PlayTalk(clip);
                timer = 0f;
            }
            yield return null;
        }
    }

    // evaluate selected key
    public bool EvaluateTutorialKey(Key key)
    {
        if (key.keyActionWord == doorWords[currentDoorIndex])
        {
            // play success audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyUnlock, 1f);
            AudioManager.instance.PlayKeyJingle();

            // play tutorial audio 4
            if (currentDoorIndex == 0)
            {
                AudioClip clip = AudioDatabase.instance.TurntablesTutorial_4;
                AudioManager.instance.PlayTalk(clip);
            }

            // success! go on to the next door or win game if on last door
            if (currentDoorIndex < 3)
                StartCoroutine(TutorialCorrectRoutine());
            else
                StartCoroutine(TutorialCompleteRoutine());
            return true;
        }
        // play success audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);
        AudioManager.instance.PlayKeyJingle();

        // incorrect choice - try again
        StartCoroutine(TutorialIncorrectRoutine());
        return false;
    }

    private IEnumerator TutorialCorrectRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // shake rock lock
        RockLock.instance.ShakeRock();
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // make door icon glow special
        ImageGlowController.instance.SetImageGlow(doors[currentDoorIndex].image, true, GlowValue.glow_1_025);
        // play stone moving audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);

        // increment value
        currentDoorIndex++;
        // toggle outline on
        GlowOutline(currentDoorIndex);
        // change frame icon at the correct time
        StartCoroutine(DelayFrameIconChange(RopeController.instance.moveTime * 2, doorWords[currentDoorIndex]));
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        
        yield return new WaitForSeconds(RopeController.instance.moveTime);

        // get new keys
        RopeController.instance.InitNewRope();
        TutorialKeySetup();
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();
        // glow the correct key
        RopeController.instance.SetKeyGlow(keys[correctKeyIndex], true);

        // make keys interactable
        SetKeysInteractable(true);
    }

    private IEnumerator TutorialIncorrectRoutine()
    {
        // return key
        keys[correctKeyIndex].ReturnToRope();
        
        yield return new WaitForSeconds(0.5f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        
        // play icon switch audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LargeRockSlide, 1f);
        yield return new WaitForSeconds(0.2f);
        // shake the door icon
        doors[currentDoorIndex].ShakeIconSwitch(doorWords[currentDoorIndex]);
        
        // change frame icon at the correct time
        StartCoroutine(DelayFrameIconChange(doors[currentDoorIndex].shakeDuration / 2, doorWords[currentDoorIndex]));

        yield return new WaitForSeconds(RopeController.instance.moveTime);

        // get new keys
        RopeController.instance.InitNewRope();
        TutorialKeySetup();
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();
        // glow the correct key
        RopeController.instance.SetKeyGlow(keys[correctKeyIndex], true);

        // make keys interactable
        SetKeysInteractable(true);
    }

    private IEnumerator TutorialCompleteRoutine()
    {
        // increase split song
        AudioManager.instance.IncreaseSplitSong();

        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // shake rock lock
        RockLock.instance.ShakeRock();
        // make door icon glow special
        ImageGlowController.instance.SetImageGlow(doors[currentDoorIndex].image, true, GlowValue.glow_1_025);
        // play stone moving audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);

        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // win glow animation
        StartCoroutine(WinGlowAnimation(RopeController.instance.moveTime * 2));
        // play win audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();


        yield return new WaitForSeconds(2f);

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        StudentInfoSystem.SaveStudentPlayerData();

        GameManager.instance.LoadScene("TurntablesGame", true, 3f);
    }

    /* 
    ################################################
    #   UTILITY FUNCTIONS
    ################################################
    */

    public void SetKeysInteractable(bool opt)
    {
        if (keys != null)
        {
            foreach (var key in keys)
                key.interactable = opt;
        }
    }

    private IEnumerator SkipToWinRoutine()
    {
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // make door icon glow special
        ImageGlowController.instance.SetImageGlow(doors[currentDoorIndex].image, true, GlowValue.glow_1_025);
        // play stone moving audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);

        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // win glow animation
        StartCoroutine(WinGlowAnimation(RopeController.instance.moveTime * 2));
        // play win audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();

        yield return new WaitForSeconds(2f);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private void KeySetup()
    {
        // get new keys from rope controller
        keys = RopeController.instance.GetKeys();

        // set current key
        if (randomizeKeyPosition)
            correctKeyIndex = Random.Range(0, 4);
        else 
            correctKeyIndex = 0;
        keys[correctKeyIndex].SetKeyActionWord(doorWords[currentDoorIndex]);

        if (!playTutorial)
        {
            // set other keys to be random word (not current or other door words)
            List<ActionWordEnum> exceptList = new List<ActionWordEnum>();
            exceptList.Add(doorWords[currentDoorIndex]);
            for (int j = 0; j < 4; j++)
            {
                if (j != correctKeyIndex)
                {
                    ActionWordEnum word = GetRandomWord(exceptList);
                    exceptList.Add(word);
                    keys[j].SetKeyActionWord(word);
                }
            }
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                if (j != correctKeyIndex)
                {
                    keys[j].SetKeyActionWord(ActionWordEnum._blank);
                }
            }
        }
    }

    private void TutorialKeySetup()
    {
        // get new keys from rope controller
        keys = RopeController.instance.GetKeys();

        // set correct key
        correctKeyIndex = correctTutorialIcons[currentDoorIndex];

        List<ActionWordEnum> currentList = new List<ActionWordEnum>();
        // get tutorial icon
        switch (currentDoorIndex)
        {
            default:
            case 0:
                currentList = firstQuartet;
                break;
            case 1:
                currentList = secondQuartet;
                break;
            case 2:
                currentList = thirdQuartet;
                break;
            case 3:
                currentList = fourthQuartet;
                break;
        }

        // set all keys to 
        for (int i = 0; i < 4; i++)
        {
            keys[i].SetKeyActionWord(currentList[i]);
        }
    }

    private ActionWordEnum GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedWordPool.Count <= 0)
        {
            unusedWordPool.Clear();
            unusedWordPool.AddRange(globalWordPool);
            // remove door words from unused word pool
            foreach(var doorWord in doorWords)
            {
                unusedWordPool.Remove(doorWord);
            }
        }

        int index = Random.Range(0, unusedWordPool.Count);
        ActionWordEnum word = unusedWordPool[index];

        unusedWordPool.Remove(word);
        return word;
    }

    private ActionWordEnum GetRandomWord(List<ActionWordEnum> except = null)
    {
        List<ActionWordEnum> pool = new List<ActionWordEnum>();
        pool.AddRange(globalWordPool);
        if (except != null)
        {
            foreach(var item in except)
            {
                pool.Remove(item);
            }
        }
        
        int index = Random.Range(0, pool.Count);
        ActionWordEnum word = pool[index];
        return word;
    }

    private IEnumerator DelayFrameIconChange(float delay, ActionWordEnum icon)
    {
        yield return new WaitForSeconds(delay);
        frameIcon.SetFrameIcon(icon);
    }

    private void GlowOutline(int index)
    {
        // remove all door glows
        foreach (Door d in doors)
            ImageGlowController.instance.SetImageGlow(d.image, false);
        // turn on correct door glow
        ImageGlowController.instance.SetImageGlow(doors[index].image, true, GlowValue.glow_1_025);
    }

    private IEnumerator WinGlowAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Door d in doors)
        {
            ImageGlowController.instance.SetImageGlow(d.image, true, GlowValue.glow_1_025);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator StartMusicDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.InitSplitSong(SplitSong.Turntables);
        AudioManager.instance.IncreaseSplitSong();
    }
}
