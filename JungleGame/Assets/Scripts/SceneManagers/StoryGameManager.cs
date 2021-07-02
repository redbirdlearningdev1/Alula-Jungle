using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGameManager : MonoBehaviour
{   
    [Header("Dev Mode Stuff")]
    public StoryGameEnum storyGameIndex;
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
    // text sizes used 
    public float defaultTextSize;
    public float actionTextSize;


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
            {
                storyGameData = GameManager.instance.storyGameDatas[(int)storyGameIndex];
            }
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
        // only play part two if dev wants to
        if (GameManager.instance.devModeActivated && skipToPartTwo)
        {
            // start part two (repeating)
            StartCoroutine(PartTwoRoutine());
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
        {
            t.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);
            t.GetComponent<TextWrapper>().SetTextSize(defaultTextSize, false);
        }   

        // start part two (repeating)
        StartCoroutine(PartTwoRoutine());
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

            // read text if available
            if (seg.readText)
            {
                AudioManager.instance.PlayTalk(seg.textAudio);

                if (seg.containsActionWord || seg.actAsActionWord)
                {
                    // move text until action word is in place
                    StartCoroutine(MoveTextToNextActionWord(seg.textAudio.length));

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
                // say action word literal
                AudioManager.instance.PlayTalk(seg.wordAudio);

                // remove coin transparency
                coin.SetTransparency(1f, true);

                yield return new WaitForSeconds(seg.wordAudio.length);

                // say action word phoneme
                AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(seg.actionWord).audio);
                actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                actionWords[currWord].GetComponent<TextWrapper>().SetTextSize(actionTextSize, true);
                currWord++;
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                ShakeCoin();
            }
            
            yield return new WaitForSeconds(2f);
        }
        
        // make coin transparent
        coin.SetTransparency(0.25f, true);
        partOneDone = true;
    }

    private IEnumerator PartTwoRoutine()
    {
        int segCount = 1;
        int segMax = actionWords.Count;
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            // skip segment if segment says to
            if (seg.skipOnPart2)
            {
                // print ("skiping segmet");
                continue;
            }

            // set the coin in action word in segment
            if (seg.containsActionWord)
                coin.SetCoinType(seg.actionWord);
            // make coin transparent
            coin.SetTransparency(0.25f, true);

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
                

            if (seg.containsActionWord)
            {
                // remove coin transparency
                coin.SetTransparency(1f, true);
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                AudioManager.instance.PlayTalk(seg.wordAudio);
                // highlight action word
                actionWords[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                actionWords[currWord].GetComponent<TextWrapper>().SetTextSize(actionTextSize, true);
                currWord++;

                // TODO: show UI to indicate that mic input is expected

                // wait for play input
                waitingForAudioInput = true;
                while (waitingForAudioInput)
                    yield return null;

                // play gorilla "yeah" animation
                ScrollingBackground.instance.GorillaCorrectAnim();

                yield return new WaitForSeconds(1f);

                // play correct audio cue
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

                yield return new WaitForSeconds(1f);

                // TODO: remove UI to indicate that mic input is no longer expected

                // successful input stuff
                ShakeCoin();
                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(seg.actionWord).audio);

                yield return new WaitForSeconds(seg.wordAudio.length);
            }
            // inc segment count
            segCount++;
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
