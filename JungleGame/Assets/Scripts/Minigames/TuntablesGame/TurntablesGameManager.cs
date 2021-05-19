using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{
    public static TurntablesGameManager instance;

    public List<Door> doors;
    public List<Key> keys;
    public FrameIcon frameIcon;

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
        StartCoroutine(StartGame());
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                doors[0].RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                doors[1].RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                doors[2].RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                bool direction = (Random.Range(0, 2) == 0);
                int angle = Random.Range(0, 360);

                doors[3].RotateToAngle(angle, direction);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                bool direction = (Random.Range(0, 2) == 0);
                doors[0].RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                doors[1].RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                doors[2].RotateToAngle(0, direction);
                direction = (Random.Range(0, 2) == 0);
                doors[3].RotateToAngle(0, direction);
            }
        }
    }

    private void PregameSetup()
    {
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

        // set random icon to each door
        for (int i = 0; i < 4; i++)
        {
            // get random icon
            doorWords[i] = GetUnusedWord();
            // set door icon
            doors[i].SetDoorIcon(doorWords[i]);
        }

        // set up the keys
        KeySetup();
        // set first frame icon
        frameIcon.SetFrameIcon(doorWords[currentDoorIndex]);
    }

    private void KeySetup()
    {
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
        if (key.keyActionWord == doorWords[currentDoorIndex])
        {
            // success! go on to the next door or win game if on last door
            if (currentDoorIndex < 3)
                StartCoroutine(DoorSuccessRoutine());
            else
                StartCoroutine(WinRoutine());
            return true;
        }
        // fail go back to previous row
        StartCoroutine(DoorFailRoutine());
        return false;
    }

    private IEnumerator DoorSuccessRoutine()
    {
        print ("success!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(1f, 1, finishedDoorColor, true);
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
        print ("fail!");
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
        print ("you win!");
        // dissipate key
        keys[correctKeyIndex].Dissipate();
        // make door icon glow special
        doors[currentDoorIndex].glowController.SetGlowSettings(1f, 1, finishedDoorColor, true);
        // move door to unlocked position
        doors[currentDoorIndex].RotateToAngle(0, true, RopeController.instance.moveTime * 2);
        // win glow animation
        StartCoroutine(WinGlowAnimation(RopeController.instance.moveTime * 2));
        // move keys down
        RopeController.instance.AnimateKeysUp();
        yield return new WaitForSeconds(animateKeysDownDelay);
        RopeController.instance.MoveFromNormalToEnd();
        yield return null;
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
}
