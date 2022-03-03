using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryGameManager : MonoBehaviour
{   
    public static StoryGameManager instance;

    [Header("Dev Mode Stuff")]
    public bool overrideGame;
    public StoryGameBackground storyGameIndex;

    [Header("Game Object Variables")]
    [SerializeField] private LogCoin coin;
    [SerializeField] private DancingManController dancingMan;
    public Transform dancingManOffScreen;
    public Transform dancingManOnScreen;
    public MicrophoneIndicator microphone;
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

    private List<Transform> wordTransforms;
    private int currWord = 0;
    private bool waitingForAudioInput = false;
    private bool playingDancingManAnimation = false;

    // current stuff
    private ActionWordEnum currentEnum;
    private AudioClip currentClip;

    private StoryGameData storyGameData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // every scene must call this in awake
        GameManager.instance.SceneInit();

        // make microphone invisible
        microphone.HideIndicator();

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

        // set flag location
        ScrollingBackground.instance.SetFlagPosition(storyGameData.background);

        // send log
        GameManager.instance.SendLog(this, "starting story game: " + storyGameData.name);

        // activate settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);
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
            if (volumeLevel >= audioInputThreshold)
            {
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
        wordTransforms = new List<Transform>();

        // add the text objects to the layout group
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            var textObj = Instantiate(textWrapperObject, textLayoutGroup);
            textObj.GetComponent<TextWrapper>().SetText(seg.text.Replace("~", ","));
            textObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);

            if (seg.moveWord)
            {
                wordTransforms.Add(textObj.transform);
            }

            if (seg.actionWord != ActionWordEnum._blank)
            {
                textObj.GetComponent<TextWrapper>().SetText(seg.actionWord.ToString());
            }

            if (seg.postText != "")
            {
                var postWordObj = Instantiate(textWrapperObject, textLayoutGroup);
                postWordObj.GetComponent<TextWrapper>().SetText(seg.postText.Replace("~", ","));
                postWordObj.GetComponent<TextWrapper>().SetTextColor(defaultTextColor, false);

                // add extra space
                var extra_space = Instantiate(textWrapperObject, textLayoutGroup);
                extra_space.GetComponent<TextWrapper>().SetSpace();
            }   

            // add space
            var space = Instantiate(textWrapperObject, textLayoutGroup);
            space.GetComponent<TextWrapper>().SetSpace();
        }

        // add last moveable word
        var last_word = Instantiate(textWrapperObject, textLayoutGroup);
        last_word.GetComponent<TextWrapper>().SetSpace();
        wordTransforms.Add(last_word.transform);
    }

    private IEnumerator GameRoutine()
    {
        int segMax = wordTransforms.Count;

        // // set coin init before pause
        // coin.SetCoinType(ActionWordEnum._blank);
        // coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0.25f, 0.5f);

        // small pause before game begins
        yield return new WaitForSeconds(2f);
        
        foreach (StoryGameSegment seg in storyGameData.segments)
        {
            // advance BG if segment says so
            if (seg.advanceBG)
            {
                ScrollingBackground.instance.IncreaseLoopIndex();
            }

            if (seg.actionWord == ActionWordEnum._blank)
            {
                coin.SetCoinType(seg.actionWord);
            }
            else if (coin.type != seg.actionWord)
            {
                // scale coin and swap values
                coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.15f);
                yield return new WaitForSeconds(0.15f);
                coin.SetCoinType(seg.actionWord);
                coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.15f);
                yield return new WaitForSeconds(0.15f);
            }

            // read text if available
            if (seg.actionWord == ActionWordEnum._blank)
            {
                // start moving gorilla
                ScrollingBackground.instance.StartMoving();
                // move text until action word is in place
                StartCoroutine(MoveTextToNextWord(seg.audio.length));

                AudioManager.instance.PlayTalk(seg.audio);
                // wait for audio to be over
                yield return new WaitForSeconds(seg.audio.length);
                // stop moving gorilla
                ScrollingBackground.instance.StopMoving();
                // small delay
                yield return new WaitForSeconds(0.2f);
            }

            // read action word if available
            else
            {
                // // remove coin transparency
                // coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 1f, 0.5f);

                // set current variables
                currentEnum = seg.actionWord;

                dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                AudioManager.instance.PlayTalk(seg.audio);
                // highlight action word
                wordTransforms[currWord].GetComponent<TextWrapper>().SetTextColor(actionTextColor, true);
                wordTransforms[currWord].GetComponent<TextWrapper>().SetTextSize(actionTextSize, true);

                yield return new WaitForSeconds(seg.audio.length);

                if (seg.requireInput)
                {
                    // turn on mic button
                    if (!microphone.interactable)
                    {
                        microphone.interactable = true;
                    }

                    // wait for play input
                    waitingForAudioInput = true;
                    // activate microphone indicator
                    microphone.ShowIndicator();
                    // repeat word if microphone has not been pressed
                    if (!microphone.hasBeenPressed)
                        StartCoroutine(RepeatWhileWating());
                    else
                    {
                        StopCoroutine(RepeatWhileWating());
                    }
                    while (waitingForAudioInput)
                    {
                        // break from loop if button is mic button is pressed
                        if (microphone.hasBeenPressed)
                            break;
                        yield return null;
                    }

                    // move text until action word is in place
                    StartCoroutine(MoveTextToNextWord(seg.audio.length));
                        
                    // start skipping mic inputs
                    if (microphone.hasBeenPressed)
                    {
                        microphone.interactable = false;
                        microphone.NoInputDetected();
                    }
                    else
                    {
                        // show mic indicator
                        microphone.AudioInputDetected();

                        // play correct audio cue
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
                        dancingMan.PlayUsingPhonemeEnum(seg.actionWord);
                    }              
                }

                // successful input
                ShakeCoin();
                AudioClip clip = GameManager.instance.GetActionWord(seg.actionWord).audio;
                AudioManager.instance.PlayTalk(clip);
                yield return new WaitForSeconds(clip.length + 1f);

                if (seg.requireInput && !microphone.hasBeenPressed)
                {
                    microphone.HideIndicator();
                }
            }

            // increment curret word if needed
            if (seg.moveWord)
            {
                currWord++;
            }
        }
        
        // // make coin transparent
        // coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.image, 0.25f, 0.5f);

        EndGame();
    } 

    private void EndGame()
    {
        // make sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1.0f);
        
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PrologueStoryGame)
        {
            // add action words to player's pool
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.explorer);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.listen);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.poop);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.orcs);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.think);
        }
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.BeginningStoryGame)
        {
            // add action words to player's pool
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.hello);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.spider);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.scared);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.thatguy);
        }
        // chapter 3 action words
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.FollowRedStoryGame)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.choice);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.strongwind);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.pirate);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.gorilla);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.sounds);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.give);
        }
        // chapter 4 action words
        else if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.EmergingStoryGame)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.backpack);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.frustrating);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.bumphead);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.baby);
        }

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();
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

    private IEnumerator MoveTextToNextWord(float duration)
    {
        float start = textLayoutGroup.position.x;
        float end = start - Mathf.Abs(actionWordStopPos.position.x - wordTransforms[currWord + 1].transform.position.x); 
        float timer = 0f;

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

    // used to control dancing man's animations
    private IEnumerator DancingManRoutine()
    {
        if (!waitingForAudioInput)
            yield break;
        if (playingDancingManAnimation)
            yield break;
        playingDancingManAnimation = true;
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

    public void ToggleWally(bool opt)
    {
        if (opt)
        {
            StartCoroutine(ShowDancingManRoutine());
        }
        else
        {
            StartCoroutine(HideDancingManRoutine());
        }
    }

    private IEnumerator ShowDancingManRoutine()
    {
        float timer = 0f;
        float moveTime = 0.3f;
        Vector3 bouncePos = dancingManOnScreen.position;
        bouncePos.y += 0.5f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(dancingManOffScreen.position, bouncePos, timer / moveTime);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(bouncePos, dancingManOnScreen.position, timer / 0.1f);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
    }

    private IEnumerator HideDancingManRoutine()
    {
        float timer = 0f;
        float moveTime = 0.3f;
        Vector3 bouncePos = dancingManOnScreen.position;
        bouncePos.y += 0.5f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(dancingManOnScreen.position, bouncePos, timer / moveTime);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
        timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                break;
            }

            Vector3 tempPos = Vector3.Lerp(bouncePos, dancingManOffScreen.position, timer / 0.1f);
            dancingMan.gameObject.transform.position = tempPos;
            yield return null;
        }
    }
}
