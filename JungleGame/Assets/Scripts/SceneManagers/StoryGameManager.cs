﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGameManager : MonoBehaviour
{   
    [Header("Dev Mode Stuff")]
    public StoryGameData devData;
    public bool skipToPartTwo;

    [Header("Game Object Variables")]
    [SerializeField] private LogCoin coin;
    [SerializeField] private DancingManController dancingMan;

    [Header("Shake Function Variables")]
    public float shakeDuration;
    public float shakeSpeed;
    public float shakeAmount;

    [Header("Text Variables")]
    [SerializeField] private Transform textLayoutGroup;
    [SerializeField] private GameObject textWrapperObject;
    [SerializeField] private Transform textStartPos;
    [SerializeField] private Transform actionWordStopPos;
    public float textHeight;
    // text colors used 
    public Color defaultTextColor;
    public Color actionTextColor;

    [Header("Audio Variables")]
    public float audioInputThreshold;

    private List<Transform> actionWords;
    private int currWord;
    private bool partOneDone = false;
    private bool partTwoDone = false;
    private bool waitingForAudioInput = false;



    private StoryGameData storyGameData;

    void Awake()
    {
        GameManager.instance.SceneInit();

        // load in game data from game manager
        GameData data = GameManager.instance.GetData();
        // make sure it is usable
        if (data == null || data.gameType != GameType.StoryGame)
        {
            // use dev data if in dev mode
            if (GameManager.instance.devModeActivated)
                storyGameData = devData;
            else // send error
                GameManager.instance.SendError(this, "invalid game data");
        } 
        else
        {
            // use imported game data
            storyGameData = (StoryGameData)data;
        }  

        // send log
        GameManager.instance.SendLog(this, "starting story game: " + storyGameData.name);
    }

    void Start()
    {
        PregameSetup();
        StartCoroutine(GameRoutine());
    }

    void Update()
    {
        if (waitingForAudioInput)
        {
            // get mic input and determine if input is loud enough
            float volumeLevel = MicInput.MicLoudness * 200;
            //print ("volume level: " + volumeLevel);
            if (volumeLevel >= audioInputThreshold)
            {
                waitingForAudioInput = false;
            }
        }
    }

    private void PregameSetup()
    {
        // TODO: add music and ambiance
        AudioManager.instance.StopMusic();

        // set scrolling background
        ScrollingBackground.instance.SetBackgroundType(storyGameData.background);

        // make action word list
        actionWords = new List<Transform>();

        // add the text objects to the layout group
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            if (seg.containsText)
            {
                var textObj = Instantiate(textWrapperObject, textLayoutGroup);
                textObj.GetComponent<TextWrapper>().SetText(seg.text);
                textObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
            }

            if (seg.containsActionWord)
            {
                var wordObj = Instantiate(textWrapperObject, textLayoutGroup);
                wordObj.GetComponent<TextWrapper>().SetText(seg.actionWordText);
                wordObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
                actionWords.Add(wordObj.transform);
            }
            
            // add small space inbetween segments
            // var spaceObj = Instantiate(textWrapperObject, textLayoutGroup);
            // spaceObj.GetComponent<TextWrapper>().SetText("  ");
        }
    }

    private IEnumerator GameRoutine()
    {
        // only play part two if dev wants to
        if (GameManager.instance.devModeActivated && skipToPartTwo)
        {
            // start part two (repeating)
            StartCoroutine(PartTwoRoutine());

            // wait until part two is complete
            while (!partTwoDone)
                yield return null;

            print ("part two complete!");

            yield break;
        }

        // start part one (listening)
        StartCoroutine(PartOneRoutine());

        // wait until part one is complete
        while (!partOneDone)
            yield return null;

        print ("part one complete!");

        yield return new WaitForSeconds(1f);

        // TODO: audio queue -> "lets try that again!"

        // return text to start position
        StartCoroutine(ResetTextRoutine(2f));
        yield return new WaitForSeconds(2f);
        currWord = 0; // reset curr word index

        // change action words to be default color
        foreach (Transform t in actionWords)
            t.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);

        // start part two (repeating)
        StartCoroutine(PartTwoRoutine());

        // wait until part two is complete
        while (!partTwoDone)
            yield return null;

        print ("part two complete!");
    } 

    private IEnumerator PartOneRoutine()
    {
        // set coin init before pause
        coin.SetCoinType(ActionWordEnum._blank);
        coin.SetTransparency(0.25f, true);

        // small pause before game begins
        yield return new WaitForSeconds(1f);
        
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            // set the coin in action word in segment
            if (seg.containsActionWord)
                coin.SetCoinType(seg.actionWord);
            else
                coin.SetCoinType(ActionWordEnum._blank);
            // make coin transparent
            coin.SetTransparency(0.25f, true);

            AudioManager.instance.PlayTalk(seg.textAudio);

            if (seg.containsActionWord)
            {
                // move text until action word is in place
                StartCoroutine(MoveTextToNextActionWord(seg.textAudio.length));
            }

            yield return new WaitForSeconds(seg.textAudio.length);
            yield return new WaitForSeconds(0.5f);

            // read action word if available
            if (seg.containsActionWord)
            {
                // remove coin transparency
                coin.SetTransparency(1f, true);
                yield return new WaitForSeconds(0.25f);

                // say action word phoneme
                AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(seg.actionWord).audio);
                actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                currWord++;
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                ShakeCoin();

                yield return new WaitForSeconds(1f);

                // say action word literal
                AudioManager.instance.PlayTalk(seg.wordAudio);
            }
            
            yield return new WaitForSeconds(2f);
        }
        partOneDone = true;
    }

    private IEnumerator PartTwoRoutine()
    {
        int segCount = 1;
        int segMax = storyGameData.segments.Count;
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            // set the coin in action word in segment
            if (seg.containsActionWord)
                coin.SetCoinType(seg.actionWord);
            AudioManager.instance.PlayTalk(seg.textAudio);

            // move gorilla to new pos
            ScrollingBackground.instance.LerpScrollPosTo((float)segCount / (float)segMax, seg.textAudio.length);
            segCount++;

            if (seg.containsActionWord)
            {
                // move text until action word is in place
                StartCoroutine(MoveTextToNextActionWord(seg.textAudio.length));
            }

            yield return new WaitForSeconds(seg.textAudio.length);

            if (seg.containsActionWord)
            {
                // highlight action word and show dancing man animation
                actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                currWord++;
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);

                // TODO: show UI to indicate that mic input is expected

                // wait for play input
                waitingForAudioInput = true;
                while (waitingForAudioInput)
                    yield return null;

                // TODO: play correct audio cue

                // play gorilla "yeah" animation
                ScrollingBackground.instance.GorillaCorrectAnim();

                yield return new WaitForSeconds(1f);

                // TODO: remove UI to indicate that mic input is no longer expected

                // successful input stuff
                ShakeCoin();
                AudioManager.instance.PlayTalk(seg.wordAudio);
            }
            
            yield return new WaitForSeconds(2f);
        }
        partTwoDone = true;

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private IEnumerator ResetTextRoutine(float duration)
    {
        float start = textLayoutGroup.position.x;
        float end = start - 50f;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > duration)
            {
                // reset text position
                textLayoutGroup.position = textStartPos.position;
                break;
            }

            float tempX = Mathf.Lerp(start, end, timer / duration);
            textLayoutGroup.position = new Vector3(tempX, textHeight, 0f);
            yield return null;
        }
    }

    private IEnumerator MoveTextToNextActionWord(float duration)
    {
        float start = textLayoutGroup.position.x;
        float end = start - Mathf.Abs(actionWordStopPos.position.x - actionWords[currWord].transform.position.x);
        float timer = 0f;

        //print ("end: " + end);

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > duration)
            {
                break;
            }

            float tempX = Mathf.Lerp(start, end, timer / duration);
            textLayoutGroup.position = new Vector3(tempX, textHeight, 0f);
            yield return null;
        }
    }

    private void ShakeCoin()
    {
        StartCoroutine(ShakeCoinRoutine(shakeDuration));
    }

    private IEnumerator ShakeCoinRoutine(float duration)
    {
        float timer = 0f;
        Vector3 originalPos = coin.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                coin.transform.position = originalPos;
                break;
            }

            Vector3 pos = originalPos;
            pos.x = originalPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            coin.transform.position = pos;
            yield return null;
        }
    }
}
