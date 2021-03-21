using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private GameObject backgroundObject;
    private float mapMinX;
    private float mapMaxX;

    // [Header("Birb")]
    // [SerializeField] private GameObject birb;
    // [SerializeField] private Transform leftBirbBounds;
    // [SerializeField] private Transform rightBirbBounds;

    [Header("Dev")]
    [SerializeField] private TextMeshProUGUI actionWordText;
    [SerializeField] private GameObject continueButton;

    void Awake()
    {
        GameManager.instance.SceneInit();

        // stop music from playing
        AudioHelper.StopMusic();
    }

    void Start()
    {
        // disable action word
        actionWordText.gameObject.SetActive(false);
        continueButton.SetActive(false);


        // get data from game manager and assert it is of type StoryGame
        GameData temp_data = GameManager.instance.GetData();
        print (temp_data);

        if (temp_data.gameType != GameType.StoryGame)
        {
            GameManager.instance.SendError(this, "GameData is not of type: StoryGame");
        }
        else
        {
            this.data = temp_data as StoryGameData;
        }


        if (data != null)
        {
            // create and set the background images
            foreach (StoryGameImage img in data.scrollingBackgroundImages)
            {
                GameObject obj = Instantiate(backgroundObject, StoryMap);
                obj.GetComponent<Image>().sprite = img.sprite;
                obj.GetComponent<MapObjectHelper>().ResetResolution(img.resolution);
            }

            StartCoroutine(StartStoryGame());
        }
        else
        {
            GameManager.instance.SendError(this, "StoryGameData is null");
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
                actionWordText.gameObject.SetActive(false);
                continueButton.SetActive(false);
            }
        }
    }

    private IEnumerator StartStoryGame()
    {
        yield return new WaitForSeconds(1f);

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

        yield return new WaitForSeconds(1f);
        GameManager.instance.LoadScene("ScrollMap", true);
    }

    private IEnumerator PlaySegment(StoryGameSegment segment)
    {
        // move scrolling background
        //StartCoroutine(StoryMapSmoothTransition());

        // play audio
        AudioHelper.PlayTalk(segment.audio);

        // limit player input

        yield return new WaitForSeconds(segment.duration);

        // play action word
        ActionWord word =  segment.actionWord;
        AudioHelper.PlayTalk(word.audio);
        actionWordText.text = "action word: " + word.word;

        yield return new WaitForSeconds(0.5f);
        continueButton.SetActive(true);
        actionWordText.gameObject.SetActive(true);

        waitForSegment = false;
    }

    private IEnumerator StoryMapSmoothTransition(float start, float end, float transitionTime)
    {
        GameManager.instance.SetRaycastBlocker(true);
        float timer = 0f;

        StoryMap.position = new Vector3(start, 0f, 0f);
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float pos = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer / transitionTime));
            StoryMap.position = new Vector3(pos, 0f, 0f);
            yield return null;
        }
        StoryMap.position = new Vector3(end, 0f, 0f);

        GameManager.instance.SetRaycastBlocker(false);
    }

    /* 
    ################################################
    #   DEV STUFF
    ################################################
    */

    public void OnContinueButtonPressed()
    {
        waitForAudioInput = false;
        actionWordText.gameObject.SetActive(false);
        continueButton.SetActive(false);
    }
}
