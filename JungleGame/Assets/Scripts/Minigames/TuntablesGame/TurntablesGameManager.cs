using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntablesGameManager : MonoBehaviour
{

    public List<Door> doors;
    public List<Key> keys;
    public FrameIcon frameIcon;

    private ActionWordEnum[] doorWords;
    private int currentIndex = 0;

    private float[] globalAngleArray = { 30, 45, 60, 90, 120, 135, 150, 180, 210, 225, 240, 270, 300, 315, 330 };
    private List<float> globalAnglePool;
    private List<float> unusedAnglePool;

    private List<ActionWordEnum> globalWordPool;
    private List<ActionWordEnum> unusedWordPool;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

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
        doorWords = new ActionWordEnum[4];

        // Create Global Coin List
        globalWordPool = GameManager.instance.GetGlobalActionWordList();
        unusedWordPool = new List<ActionWordEnum>();
        unusedWordPool.AddRange(globalWordPool);

        // set random icon to each door
        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, unusedWordPool.Count);
            doorWords[i] = unusedWordPool[index];
            // set door icon
            doors[i].SetDoorIcon(doorWords[i]);
            unusedWordPool.RemoveAt(index);
            // set key word aswell
            keys[i].SetKeyActionWord(doorWords[i]);

            // reset unused pool if empty
            if (unusedWordPool.Count <= 0)
            {
                unusedWordPool.Clear();
                unusedWordPool.AddRange(globalWordPool);
            }
        }

        // set first frame icon
        frameIcon.SetFrameIcon(doorWords[currentIndex]);
    }

    private IEnumerator StartGame()
    {
        globalAnglePool = new List<float>(globalAngleArray);
        unusedAnglePool = new List<float>(globalAnglePool);

        bool direction = false;
        float duration = 3f;
        float difference = 0.5f;

        // set door angle
        for (int i = 3; i >= 0; i--)
        {
            int index = Random.Range(0, unusedAnglePool.Count);
            float angle = unusedAnglePool[index];
            doors[i].RotateToAngle(angle, direction, duration);
            direction = !direction;
            
            unusedAnglePool.RemoveAt(index);
            if (unusedAnglePool.Count <= 0)
            {
                unusedAnglePool.Clear();
                unusedAnglePool.AddRange(globalAnglePool);
            }

            yield return new WaitForSeconds(difference);
            duration -= difference;
        }
        
        yield return null;
    }   
}
