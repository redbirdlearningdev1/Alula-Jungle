using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryGameManager : MonoBehaviour
{
    [Header("Story Map Data")]
    public float volumeInputThreshold;
    private StoryGameData data;
    private int index = 0;
    private bool waitForSegment = true;
    private bool waitForAudioInput = true;

    [Header("Story Map Navigation")]
    [SerializeField] private RectTransform StoryMap;
    [SerializeField] private GameObject[] mapLocations;
    private float mapMinX;
    private float mapMaxX;

    [Header("Birb")]
    [SerializeField] private GameObject birb;
    [SerializeField] private Transform leftBirbBounds;
    [SerializeField] private Transform rightBirbBounds;

    void Awake()
    {
        GameHelper.SceneInit();
    }

    void Start()
    {
        data = GameHelper.GetData(DataType.StoryGame) as StoryGameData;

        if (data != null)
        {
            StartCoroutine(StartStoryGame());
        }
        else
        {
            GameHelper.SendError(this, "StoryGameData is null");
        }
    }

    void Update()
    {
        if (waitForAudioInput)
        {
            float volumeLevel = AudioInput.volumeLevel;
            if (volumeLevel >= volumeInputThreshold)
            {
                waitForAudioInput = false;
            }
        }
    }

    private IEnumerator StartStoryGame()
    {
        foreach(StoryGameSegment segment in data.segments)
        {
            waitForSegment = true;
            StartCoroutine(PlaySegment(segment));

            while (waitForSegment)
                yield return null;
            
            waitForAudioInput = true;
            while(waitForAudioInput)
                yield return null;
        }
    }

    private IEnumerator PlaySegment(StoryGameSegment segment)
    {
        // move scrolling background

        // play audio

        // limit player input 

        yield return new WaitForSeconds(segment.duration);
        waitForSegment = false;
        waitForAudioInput = true;
    }
}
