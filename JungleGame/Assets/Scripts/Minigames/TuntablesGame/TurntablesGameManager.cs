using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{
    public static TurntablesGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;
    private List<ActionWordEnum> globalPool;

    [Header("Tutorial Stuff")]
    public bool playTutorial;
    public bool glowCorrectKey;

    [Header("Game Stuff")]
    // doors
    public LerpableObject bigDoor;
    public List<LerpableObject> doors;
    public List<LerpableObject> doorTiles;
    // keys
    public LerpableObject keyRope;
    public List<Transform> keyRopePositions;
    public List<Key> keys;
    // frame
    public Image frame;
    // rock lock
    public LerpableObject rockLock;


    // private game varibales
    private List<ActionWordEnum> doorValues;
    private int currentDoor = 0;
    

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
            playTutorial = !StudentInfoSystem.GetCurrentProfile().turntablesTutorial;

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
                StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
                StudentInfoSystem.SaveStudentPlayerData();
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(3);
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
        // start ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.1f, "river_loop");

        // turn off raycaster
        KeyRaycaster.instance.isOn = false;

        // create global pool
        globalPool = new List<ActionWordEnum>();
        if (mapID != MapIconIdentfier.None)
        {
            globalPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }

        // set door values
        doorValues = new List<ActionWordEnum>();
        for (int i = 0; i < 4; i++)
        {
            doorValues.Add(GetNewValue());
            doors[i].GetComponentInChildren<DoorTile>().SetTile(doorValues[i], true);
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
        yield return new WaitForSeconds(0.25f);
        doors[1].LerpRotation(45, 2.75f);
        yield return new WaitForSeconds(0.5f);
        doors[2].LerpRotation(-45, 2.25f);
        yield return new WaitForSeconds(0.5f);
        doors[3].LerpRotation(45, 1.75f);
        yield return new WaitForSeconds(1.75f);
        // grow / shrink door tiles
        foreach (var d in doorTiles)
        {
            d.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        }

        // stop big door wiggle
        bigDoor.GetComponent<WiggleController>().StopWiggle();

        // start new round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // shoet delay before new round
        yield return new WaitForSeconds(1f);

        // set key values
        List<ActionWordEnum> values = new List<ActionWordEnum>();
        values.AddRange(doorValues);
        foreach (Key k in keys)
        {
            // set each door value to a random key
            int randomIndex = Random.Range(0, values.Count);
            var randomvalue = values[randomIndex];
            k.SetKeyType(randomvalue);

            // glow correct key
            if (glowCorrectKey)
            {
                if (randomvalue == doorValues[currentDoor])
                {
                    ImageGlowController.instance.SetImageGlow(k.GetComponent<Image>(), true, GlowValue.glow_1_00);
                }
            }
            
            // remove value from list
            values.Remove(randomvalue);
        }

        // show keys
        StartCoroutine(ShowKeys());

        // turn on key raycaster
        KeyRaycaster.instance.isOn = true;
    }

    public bool EvaluateKey(Key selectedKey)
    {
        // turn off raycaster
        KeyRaycaster.instance.isOn = false;

        bool isCorrect = false;

        if (selectedKey.GetKeyType() == doorValues[currentDoor])
        {
            isCorrect = true;
            StartCoroutine(PostEvaluationRoutine(true));
        }
        else
        {
            StartCoroutine(PostEvaluationRoutine(false));
        }

        return isCorrect;
    }

    private IEnumerator PostEvaluationRoutine(bool isCorrect)
    {
        if (isCorrect)
        {
            
        }
        else
        {

        }

        yield return null;
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
            k.KeyUpAnim();
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

    private ActionWordEnum GetNewValue()
    {
        // make a list of unused values
        List<ActionWordEnum> unusedValues = new List<ActionWordEnum>();
        unusedValues.AddRange(globalPool);
        foreach(var usedValue in doorValues)
        {
            unusedValues.Remove(usedValue);
        }

        // return a random unused value
        return unusedValues[Random.Range(0, unusedValues.Count)];
    }
}
