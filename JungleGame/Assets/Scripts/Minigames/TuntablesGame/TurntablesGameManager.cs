using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TurntablesGameManager : MonoBehaviour
{
    public static TurntablesGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;
    private List<ActionWordEnum> globalPool;

    [Header("Tutorial Stuff")]
    public bool playTutorial;
    public bool showCorrectKey;
    public List<ActionWordEnum> tutorialIcons;

    [Header("Game Stuff")]
    // doors
    public LerpableObject bigDoor;
    public List<LerpableObject> doors;
    public List<DoorTile> doorTiles;
    public List<float> moveStonePitch;
    // keys
    public LerpableObject keyRope;
    public List<Transform> keyRopePositions;
    public List<Key> keys;
    // frame
    public Image frame;
    // rock lock
    public LerpableObject rockLock;
    public float musicStartDelay;


    // private game varibales
    private List<ActionWordEnum> doorValues;
    private int currentDoor = 0;
    private int numMisses = 0;


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

        // only turn off tutorial if false
        if (!playTutorial)
        {
            playTutorial = !StudentInfoSystem.GetCurrentProfile().turntablesTutorial;
        }

        // show correct key during tutorial
        if (playTutorial)
            showCorrectKey = true;

        PregameSetup();
    }

    public void SkipGame()
    {
        StopAllCoroutines();
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        // save to sis
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        // times missed set to 0
        numMisses = 0;
        // update AI data
        AIData(StudentInfoSystem.GetCurrentProfile());
        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
        // remove all raycast blockers
        RaycastBlockerController.instance.ClearAllRaycastBlockers();
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

    /* 
    ################################################
    #   GAME FUNCTIONS
    ################################################
    */

    private void PregameSetup()
    {
        if (!playTutorial)
            StartCoroutine(StartMusicDelay(musicStartDelay));

        // start ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.BreezeLoop, 0.01f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.QuarryLoop, 0.01f);

        // turn off raycaster
        KeyRaycaster.instance.isOn = false;

        // create global pool
        globalPool = new List<ActionWordEnum>();
        if (GameManager.instance.practiceModeON)
        {
            globalPool.AddRange(GameManager.instance.practicePhonemes);
        }
        else if (mapID != MapIconIdentfier.None)
        {
            globalPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }

        // set door values
        if (playTutorial)
        {
            doorValues = new List<ActionWordEnum>();
            doorValues.AddRange(tutorialIcons);
            for (int i = 0; i < 4; i++)
            {
                doors[i].GetComponentInChildren<DoorTile>().SetTile(doorValues[i], true);
            }
        }
        else
        {
            doorValues = new List<ActionWordEnum>();
            for (int i = 0; i < 4; i++)
            {
                doorValues.Add(GetNewValue());
                doorTiles[i].SetTile(doorValues[i], true);
                doorTiles[i].ToggleGlow(false);
            }
        }

        // set frame icon
        frame.sprite = GameManager.instance.GetActionWord(doorValues[0]).frameIcon;

        // start game
        StartCoroutine(StartGame());
    }


    private IEnumerator StartGame()
    {
        // short delay before game starts 
        yield return new WaitForSeconds(1f);

        // start big door wiggle
        bigDoor.GetComponent<WiggleController>().StartWiggle();

        // lerp doors to be in locked positions
        doors[0].LerpRotation(-180, 3f);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound(3f, moveStonePitch[0]);
        yield return new WaitForSeconds(0.25f);

        doors[1].LerpRotation(45, 2.75f);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound(2.75f, moveStonePitch[1]);
        yield return new WaitForSeconds(0.5f);

        doors[2].LerpRotation(-45, 2.25f);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound(2.25f, moveStonePitch[2]);
        yield return new WaitForSeconds(0.5f);

        doors[3].LerpRotation(60, 1.75f);
        // play stone moving audio
        AudioManager.instance.PlayMoveStoneSound(1.75f, moveStonePitch[3]);
        yield return new WaitForSeconds(1.75f);

        // grow / shrink door tiles
        foreach (var d in doorTiles)
        {
            d.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        }

        // stop big door wiggle
        bigDoor.GetComponent<WiggleController>().StopWiggle();

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // start new round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // short delay before new round
        yield return new WaitForSeconds(1f);

        if (playTutorial && currentDoor == 0)
        {
            // small delay
            yield return new WaitForSeconds(1f);

            // play tutorial audio 1
            AssetReference clip = GameIntroDatabase.instance.turntablesIntro1;
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);
        }

        // show keys
        StartCoroutine(ShowKeys());

        if (playTutorial && currentDoor == 0)
        {
            // small delay
            yield return new WaitForSeconds(1f);

            // play tutorial audio 2
            AssetReference clip = GameIntroDatabase.instance.turntablesIntro2;
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topRight.position, false, TalkieCharacter.Red, clip);
            yield return new WaitForSeconds(cd.GetResult() + 1f);
        }
        else
        {
            // small delay
            //yield return new WaitForSeconds(1.5f);
        }

        // scale door tile
        doorTiles[currentDoor].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.5f, 1.5f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        // add glow to current door tile
        doorTiles[currentDoor].ToggleGlow(true);
        // play sound effect
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 1f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MoveStoneEnd, 1f, "door_tile", 1.5f);

        // set frame icon
        frame.sprite = GameManager.instance.GetActionWord(doorValues[currentDoor]).frameIcon;

        if (showCorrectKey || playTutorial)
        {
            // wait a small amount of time if playing tutorial
            yield return new WaitForSeconds(1.5f);
        }

        // set key values
        List<ActionWordEnum> values = new List<ActionWordEnum>();
        values.AddRange(doorValues);
        foreach (Key k in keys)
        {
            // set each door value to a random key
            int randomIndex = Random.Range(0, values.Count);
            var randomvalue = values[randomIndex];
            k.SetKeyType(randomvalue);

            // scale correct key
            if (showCorrectKey || playTutorial)
            {
                if (randomvalue == doorValues[currentDoor])
                {
                    k.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.5f);
                    k.GetComponent<LerpableObject>().LerpImageAlpha(k.GetComponent<Image>(), 1f, 0.5f);
                }
                else
                {
                    k.GetComponent<LerpableObject>().LerpScale(new Vector2(0.8f, 0.8f), 0.5f);
                    k.GetComponent<LerpableObject>().LerpImageAlpha(k.GetComponent<Image>(), 0.5f, 0.5f);
                }
            }

            // remove value from list
            values.Remove(randomvalue);
        }

        if (showCorrectKey || playTutorial)
        {
            // play sound effect 
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.25f);
            yield return new WaitForSeconds(1f);
        }

        // turn on key raycaster
        KeyRaycaster.instance.isOn = true;
    }

    public bool EvaluateKey(Key selectedKey)
    {
        // turn off raycaster
        KeyRaycaster.instance.isOn = false;

        bool success = (selectedKey.GetKeyType() == doorValues[currentDoor]);
        // only track phoneme attempt if not in tutorial AND not in practice mode
        if (!playTutorial /*&& !GameManager.instance.practiceModeON */)
        {
            StudentInfoSystem.SavePlayerMinigameRoundAttempt(GameType.TurntablesGame, success);
            StudentInfoSystem.SavePlayerPhonemeAttempt(doorValues[currentDoor], success);
        }

        if (success)
        {
            StartCoroutine(PostEvaluationRoutine(true));
        }
        else
        {
            StartCoroutine(PostEvaluationRoutine(false));
        }

        return success;
    }

    private IEnumerator PostEvaluationRoutine(bool isCorrect)
    {
        if (isCorrect)
        {
            // play success audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyUnlock, 1f);
            AudioManager.instance.PlayKeyJingle();
        }
        else
        {
            // play audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);
            AudioManager.instance.PlayKeyJingle();
        }

        // short delay before routine
        yield return new WaitForSeconds(1f);

        if (isCorrect)
        {
            // move door to correct position
            doors[currentDoor].LerpRotation(0f, 1.5f);
            // play stone moving audio
            AudioManager.instance.PlayMoveStoneSound(1.5f, moveStonePitch[currentDoor]);
            yield return new WaitForSeconds(2f);

            // scale door tile
            doorTiles[currentDoor].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
            yield return new WaitForSeconds(0.2f);
            // play sound effect
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 1f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MoveStoneEnd, 1f, "door_tile", 1.5f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ErrieGlow, 0.2f);
            // remove glow to current door tile
            doorTiles[currentDoor].ToggleGlow(false);
            // increase split song
            AudioManager.instance.IncreaseSplitSong();
            // increment door num
            currentDoor++;

            yield return new WaitForSeconds(1f);

            // play tutorial intro 4
            if (playTutorial && currentDoor == 1)
            {
                // play tutorial audio 4
                AssetReference clip = GameIntroDatabase.instance.turntablesIntro4;
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;
                TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Red, clip);
                yield return new WaitForSeconds(cd.GetResult() + 1f);
            }
            else if (currentDoor <= 3)
            {
                if (GameManager.DeterminePlayPopup())
                {
                    // play encouragement popup
                    AssetReference clip = GameIntroDatabase.instance.turntablesEncouragementClips[Random.Range(0, GameIntroDatabase.instance.turntablesEncouragementClips.Count)];
                    CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                    yield return cd.coroutine;
                    TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
                    //yield return new WaitForSeconds(cd.GetResult() + 1f);
                }
                
            }
        }
        else
        {
            // skip if playing tutorial
            if (playTutorial)
            {
                // turn on raycaster
                KeyRaycaster.instance.isOn = true;

                yield break;
            }

            // change door tile to new tile
            doorValues[currentDoor] = GetNewValue();
            doorTiles[currentDoor].SetTile(doorValues[currentDoor]);
            AudioManager.instance.PlayMoveStoneSound(1f, 1f);
            yield return new WaitForSeconds(0.4f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 1f);
            doorTiles[currentDoor].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
            yield return new WaitForSeconds(0.2f);

            // set frame icon
            frame.sprite = GameManager.instance.GetActionWord(doorValues[currentDoor]).frameIcon;

            // increment wins
            numMisses++;

            // play reminder popup
            List<AssetReference> clips = new List<AssetReference>();
            clips.Add(GameIntroDatabase.instance.turntablesReminder1);
            clips.Add(GameIntroDatabase.instance.turntablesReminder2);
            clips.Add(GameIntroDatabase.instance.turntablesReminder3);

            AssetReference clip = clips[Random.Range(0, clips.Count)];
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
            yield return cd.coroutine;
            TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.topLeft.position, true, TalkieCharacter.Red, clip);
            //yield return new WaitForSeconds(cd.GetResult() + 1f);
        }

        // move keys down
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RemoveKeys());

        // determine if win
        if (isCorrect && currentDoor == 4)
        {
            StartCoroutine(WinRoutine());
        }
        // else next round
        else
        {
            StartCoroutine(NewRound());
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
            StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();

            GameManager.instance.LoadScene("TurntablesGame", true, 3f);
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
        playerData.lastGamePlayed = GameType.TurntablesGame;
        playerData.starsTurntables = CalculateStars() + playerData.starsTurntables;
        playerData.turntablesPlayed++;

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


    /* 
    ################################################
    #   KEY + KEY ROPE FUNCTIONS
    ################################################
    */

    private void ResetKeyRope()
    {
        keyRope.transform.position = keyRopePositions[0].position;

        // make keys be in up position
        foreach (Key k in keys)
        {
            k.ResetKey();
        }
    }

    private IEnumerator ShowKeys()
    {
        // determine bounce pos
        Vector3 bouncePos = keyRopePositions[1].position;
        bouncePos.y -= 0.5f;


        // move rope down
        keyRope.LerpPosition(bouncePos, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        keyRope.LerpPosition(keyRopePositions[1].position, 0.1f, false);

        // move keys down
        foreach (Key k in keys)
        {
            k.KeyDownAnim();
        }

        // play key jingle
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyLatch, 1f);
    }

    private IEnumerator RemoveKeys()
    {
        // determine bounce pos
        Vector3 bouncePos = keyRopePositions[1].position;
        bouncePos.y += 0.5f;

        // move keys down
        foreach (Key k in keys)
        {
            k.KeyUpAnim();
        }
        yield return new WaitForSeconds(0.25f);

        // play key jingle
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyLatch, 1f);

        // move rope down
        keyRope.LerpPosition(bouncePos, 0.1f, false);
        yield return new WaitForSeconds(0.1f);
        keyRope.LerpPosition(keyRopePositions[2].position, 0.5f, false);
        yield return new WaitForSeconds(0.5f);

        // reset rope to the top
        ResetKeyRope();
    }

    /* 
    ################################################
    #   UTILITY FUNCTIONS
    ################################################
    */

    private IEnumerator StartMusicDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.InitSplitSong(AudioDatabase.instance.TurntablesSongSplit);
    }

    private ActionWordEnum GetNewValue()
    {
        // make a list of unused values
        List<ActionWordEnum> unusedValues = new List<ActionWordEnum>();
        unusedValues.AddRange(globalPool);
        foreach (var usedValue in doorValues)
        {
            unusedValues.Remove(usedValue);
        }

        // return a random unused value
        return unusedValues[Random.Range(0, unusedValues.Count)];
    }
}
