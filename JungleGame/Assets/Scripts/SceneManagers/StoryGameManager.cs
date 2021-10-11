﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGameManager : MonoBehaviour
{   
    [Header("Dev Mode Stuff")]
    public bool overrideGame;
    public StoryGameEnum storyGameIndex;

    [Header("Game Object Variables")]
    [SerializeField] private LogCoin coin;
    [SerializeField] private DancingManController dancingMan;
    [SerializeField] private SpriteRenderer microphone;
    public float timeBetweenRepeat;

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
    // text sizes used 
    public float defaultTextSize;
    public float actionTextSize;


    [Header("Audio Variables")]
    public float audioInputThreshold;

    private List<Transform> actionWords;
    private int currWord;
    private bool waitingForAudioInput = false;
    private bool playingDancingManAnimation = false;

    // current stuff
    private ActionWordEnum currentEnum;
    private AudioClip currentClip;

    private StoryGameData storyGameData;

    void Awake()
    {
        // every scene must call this in awake
        GameManager.instance.SceneInit();

        // make microphone invisible
        microphone.color = new Color(1f, 1f, 1f, 0f);

        if (overrideGame)
        {
            // use dev data if in dev mode
            if (GameManager.instance.devModeActivated)
            {
                storyGameData = GameManager.instance.storyGameDatas[(int)storyGameIndex];
            }
            else // send error
                GameManager.instance.SendError(this, "invalid game data");
        }
        else 
        {
            // load in game data from game manager
            storyGameData = GameManager.instance.storyGameData;
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
                StartCoroutine(StopMicrophoneIndicator());
                waitingForAudioInput = false;
            }
        }


        // repeat word and dancing man animation
        if (dancingMan.isClicked)
        {
            StartCoroutine(DancingManRoutine());
        }

        // skip story game w SPACE if in dev mode
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.instance.SendLog(this, "skipping story game!");
                StopAllCoroutines();
                EndGame();
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
            if (seg.writeText)
            {
                // add space
                var space = Instantiate(textWrapperObject, textLayoutGroup);
                space.GetComponent<TextWrapper>().SetSpace();

                var textObj = Instantiate(textWrapperObject, textLayoutGroup);
                textObj.GetComponent<TextWrapper>().SetText(seg.text);
                textObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
                //print ("adding text: " + seg.text);
            }

            // add empty action word if segment says so
            if (seg.actAsActionWord)
            {
                var emptyWord = Instantiate(textWrapperObject, textLayoutGroup);
                emptyWord.GetComponent<TextWrapper>().SetText("");
                emptyWord.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
                actionWords.Add(emptyWord.transform);
            }

            if (seg.containsActionWord)
            {
                // add space
                var space = Instantiate(textWrapperObject, textLayoutGroup);
                space.GetComponent<TextWrapper>().SetSpace();

                var wordObj = Instantiate(textWrapperObject, textLayoutGroup);
                wordObj.GetComponent<TextWrapper>().SetText(seg.actionWordText);
                wordObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
                actionWords.Add(wordObj.transform);
                //print ("adding word: " + seg.actionWordText);
            }

            if (seg.containsPostText)
            {
                var postWordObj = Instantiate(textWrapperObject, textLayoutGroup);
                postWordObj.GetComponent<TextWrapper>().SetText(seg.postText);
                postWordObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
                //print ("adding word: " + seg.actionWordText);

                // add space
                var space = Instantiate(textWrapperObject, textLayoutGroup);
                space.GetComponent<TextWrapper>().SetSpace();
            }
        }
    }

    private IEnumerator GameRoutine()
    {
        int segCount = 1;
        int segMax = actionWords.Count;

        // set coin init before pause
        coin.SetCoinType(ActionWordEnum._blank);
        coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0.25f, 0.5f);

        // small pause before game begins
        yield return new WaitForSeconds(2f);
        
        foreach (StoryGameSegment seg in storyGameData.segments)
        {

            // set the coin in action word in segment
            if (seg.containsActionWord)
                coin.SetCoinType(seg.actionWord);
            else
                coin.SetCoinType(ActionWordEnum._blank);
            // make coin transparent
            coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0.25f, 0.5f);

            // read text if available
            if (seg.readText)
            {
                AudioManager.instance.PlayTalk(seg.textAudio);

                if (seg.containsActionWord || seg.actAsActionWord)
                {
                    // move text until action word is in place
                    StartCoroutine(MoveTextToNextActionWord(seg.textAudio.length));

                    // move gorilla to new pos
                    ScrollingBackground.instance.LerpScrollPosTo((float)segCount / (float)segMax, seg.textAudio.length);

                    // increment currActionWord
                    if (seg.actAsActionWord)
                        currWord++;
                }

                yield return new WaitForSeconds(seg.textAudio.length);
            }
            // if no text - just scroll to next action word
            else if (!seg.readText && seg.containsActionWord)
            {
                // move text until action word is in place
                StartCoroutine(MoveTextToNextActionWord(seg.wordAudio.length));
            }

            // read action word if available
            if (seg.containsActionWord)
            {
                // remove coin transparency
                coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 1f, 0.5f);

                // set current variables
                currentEnum = seg.actionWord;
                currentClip = seg.wordAudio;

                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                AudioManager.instance.PlayTalk(seg.wordAudio);
                // highlight action word
                actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                actionWords[currWord].GetComponent<TextWrapper>().SetTextSize(actionTextSize, true);
                currWord++;

                if (seg.requireMicInput)
                {
                    // wait for play input
                    waitingForAudioInput = true;
                    // activate microphone indicator
                    StartCoroutine(StartMicrophoneIndicator());
                    StartCoroutine(RepeatWhileWating());
                    while (waitingForAudioInput)
                        yield return null;

                    // play gorilla "yeah" animation
                    ScrollingBackground.instance.GorillaCorrectAnim();

                    yield return new WaitForSeconds(1f);

                    // play correct audio cue
                    AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
                }

                yield return new WaitForSeconds(seg.wordAudio.length);

                // successful input stuff
                ShakeCoin();
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(seg.actionWord).audio);
            }

            segCount++;
            yield return new WaitForSeconds(2f);
        }
        
        // make coin transparent
        coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0.25f, 0.5f);

        EndGame();
    } 

    private void EndGame()
    {
        // make sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1.0f);
        
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PrologueStoryGame)
        {
            // add action words to player's pool
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.listen);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.poop);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.orcs);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.think);

            // advance story beat
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        // return to scrollmap
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

    private IEnumerator StartMicrophoneIndicator()
    {
        microphone.GetComponent<WiggleController>().StartWiggle();
        microphone.GetComponent<GlowOutlineController>().ToggleGlowOutline(true);
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.25f)
            {
                break;
            }

            float tempAlpha = Mathf.Lerp(0f, 1f, timer / 0.25f);
            microphone.color = new Color(1f, 1f, 1f, tempAlpha);
            yield return null;
        }
    }

    private IEnumerator StopMicrophoneIndicator()
    {
        microphone.GetComponent<WiggleController>().StopWiggle();
        microphone.GetComponent<GlowOutlineController>().ToggleGlowOutline(false);
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.25f)
            {
                break;
            }

            float tempAlpha = Mathf.Lerp(1f, 0f, timer / 0.25f);
            microphone.color = new Color(1f, 1f, 1f, tempAlpha);
            yield return null;
        }
    }

    // used to control dancing man's animations
    private IEnumerator DancingManRoutine()
    {
        if (!waitingForAudioInput)
            yield break;
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
        // print ("dancing man animation -> " + selectedCoin.type);
        dancingMan.PlayUsingPhonemeEnum(currentEnum);
        AudioManager.instance.PlayTalk(currentClip);

        yield return new WaitForSeconds(1.5f);
        playingDancingManAnimation = false;
    }

    private IEnumerator RepeatWhileWating()
    {
        float timer = 0f;

        while (true)
        {
            while (playingDancingManAnimation)
                yield return null;

            if (!waitingForAudioInput)
                yield break;  

            timer += Time.deltaTime;
            if (timer > timeBetweenRepeat)
            {
                timer = 0f;
                StartCoroutine(DancingManRoutine());
            }

            yield return null;
        }
    }
}
