using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{
    public static TurntablesGameManager instance;

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

    private bool gameStart;
    private const float animateKeysDownDelay = 0.3f;

    public Color finishedDoorColor;
    public Color winDoorColor;

    [Header("Tutorial")]
    public bool playTutorial;
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

        if (!instance)
        {
            instance = this;
        }
    }

    void Start()
    {   
        PregameSetup();

        // play real game or tutorial
        if (playTutorial)
        {
            GameManager.instance.SendLog(this, "starting turntables game tutorial");
            StartCoroutine(StartTutorial());
        }
        else
        {
            StartCoroutine(StartGame());
        }
    }

    void Update()
    {
        // if (GameManager.instance.devModeActivated)
        // {
        //     if (Input.GetKeyDown(KeyCode.Alpha1))
        //     {
        //         bool direction = (Random.Range(0, 2) == 0);
        //         int angle = Random.Range(0, 360);

        //         doors[0].RotateToAngle(angle, direction);
        //     }
        //     else if (Input.GetKeyDown(KeyCode.Alpha2))
        //     {
        //         bool direction = (Random.Range(0, 2) == 0);
        //         int angle = Random.Range(0, 360);

        //         doors[1].RotateToAngle(angle, direction);
        //     }
        //     else if (Input.GetKeyDown(KeyCode.Alpha3))
        //     {
        //         bool direction = (Random.Range(0, 2) == 0);
        //         int angle = Random.Range(0, 360);

        //         doors[2].RotateToAngle(angle, direction);
        //     }
        //     else if (Input.GetKeyDown(KeyCode.Alpha4))
        //     {
        //         bool direction = (Random.Range(0, 2) == 0);
        //         int angle = Random.Range(0, 360);

        //         doors[3].RotateToAngle(angle, direction);
        //     }
        //     else if (Input.GetKeyDown(KeyCode.Alpha5))
        //     {
        //         bool direction = (Random.Range(0, 2) == 0);
        //         doors[0].RotateToAngle(0, direction);
        //         direction = (Random.Range(0, 2) == 0);
        //         doors[1].RotateToAngle(0, direction);
        //         direction = (Random.Range(0, 2) == 0);
        //         doors[2].RotateToAngle(0, direction);
        //         direction = (Random.Range(0, 2) == 0);
        //         doors[3].RotateToAngle(0, direction);
        //     }
        // }
    }

    /* 
    ################################################
    #   PREGAME SETUP
    ################################################
    */

    private void PregameSetup()
    {
        StartCoroutine(StartMusicDelay(musicStartDelay));

        // start ambiance noise
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.BreezeLoop, 0.01f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.QuarryLoop, 0.01f);

        // remove glow from door icons
        foreach (Door d in doors)
            d.glowController.ToggleGlowOutline(false);
        // remove glow from rock lock
        RockLock.instance.glowController.ToggleGlowOutline(false);

        doorWords = new ActionWordEnum[4];

        // Create Global Coin List
        globalWordPool = GameManager.instance.GetGlobalActionWordList();
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

        // set up the keys
        KeySetup();
        // set first frame icon
        frameIcon.SetFrameIcon(doorWords[currentDoorIndex]);
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
        // print ("success!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(2f, 1, finishedDoorColor, true);
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
    }

    private IEnumerator DoorFailRoutine()
    {
        // print ("fail!");
        // return key
        keys[correctKeyIndex].ReturnToRope();
        
        yield return new WaitForSeconds(0.5f);
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        
        // change door to have a new icon
        ActionWordEnum newWord = GetUnusedWord();
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
    }

    private IEnumerator WinRoutine()
    {
        // print ("you win!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(1f, 1, finishedDoorColor, true);
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

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
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
        
        // toggle outline on
        GlowOutline(currentDoorIndex);
        RopeController.instance.MoveFromInitToNormal();
        yield return new WaitForSeconds(RopeController.instance.moveTime * 0.75f);
        RopeController.instance.AnimateKeysDown();
        // glow the correct key
        RopeController.instance.SetKeyGlow(keys[correctKeyIndex], true);
        gameStart = true;
    }  

    // evaluate selected key
    public bool EvaluateTutorialKey(Key key)
    {
        if (key.keyActionWord == doorWords[currentDoorIndex])
        {
            // play success audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyUnlock, 1f);
            AudioManager.instance.PlayKeyJingle();

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
        print ("success!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound((RopeController.instance.moveTime * 2) - 0.4f, moveStonePitch[currentDoorIndex]);

        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(2f, 1, finishedDoorColor, true);
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
    }

    private IEnumerator TutorialIncorrectRoutine()
    {
        // print ("fail!");
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
    }

    private IEnumerator TutorialCompleteRoutine()
    {
        // print ("you win!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(2f, 1, finishedDoorColor, true);
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

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    /* 
    ################################################
    #   UTILITY FUNCTIONS
    ################################################
    */

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

        // set other keys to be random word (not current or other door words)
        for (int j = 0; j < 4; j++)
        {
            if (j != correctKeyIndex)
            {
                ActionWordEnum word = GetUnusedWord();
                keys[j].SetKeyActionWord(word);
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
        }

        int index = Random.Range(0, unusedWordPool.Count);
        ActionWordEnum word = unusedWordPool[index];
        unusedWordPool.RemoveAt(index);
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
            d.glowController.ToggleGlowOutline(false);
        // turn on correct door glow
        doors[index].glowController.ToggleGlowOutline(true);
    }

    private IEnumerator WinGlowAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Door d in doors)
        {
            d.glowController.SetGlowSettings(3f, 1, winDoorColor, true);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private IEnumerator StartMusicDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlaySong(AudioDatabase.instance.TurntablesGameSong);
    }
}
