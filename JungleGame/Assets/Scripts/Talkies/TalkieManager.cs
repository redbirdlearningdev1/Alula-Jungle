using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TalkieManager : MonoBehaviour
{
    public static TalkieManager instance;

    // public vars for other scripts to access
    [HideInInspector] public bool doNotContinueToGame = false;

    [HideInInspector] public bool talkiePlaying = false; // used to pause routines while talkies are playing
    private TalkieObject currentTalkie;

    private TalkieCharacter currLeftCharacter;
    private TalkieCharacter currRightCharacter;

    private int currLeftEmotionNum;
    private int currRightEmotionNum;

    private TalkieMouth currLeftMouthEnum;
    private TalkieMouth currRightMouthEnum;

    private TalkieEyes currLeftEyesEnum;
    private TalkieEyes currRightEyesEnum;

    private bool leftHidden = true;
    private bool rightHidden = true;

    public float talkieMoveSpeed;
    public float talkieDeactivateSpeed;

    [Header("Talkie Transforms")]
    public Transform leftTalkie;
    public Transform rightTalkie;

    [Header("Talkie Images")]
    public Image leftImage;
    public Image rightImage;

    [Header("Talkie Positions")]
    public Transform leftInactivePos;
    public Transform rightInactivePos;
    public Transform leftActivePos;
    public Transform rightActivePos;
    public Transform leftSideHiddenPos;
    public Transform rightSideHiddenPos;

    [Header("Talkie Variables")]
    public float inactiveScale;
    public float inactiveAlpha;

    [Header("Subtitles")]
    public Image subtitleBox;
    public TextMeshProUGUI subtitleText;

    [Header("Audio Stuff")]
    public float minMusicVolWhenTalkiePlaying = 0f;
    private float prevMusicVolume;

    [Header("Yes No Stuff")]
    public Button yesButton;
    public Button noButton;
    private int yesGoto;
    private int noGoto;

    [Header("Fast Talkies")]
    public float normalTalkieMoveSpeed;
    public float normalTalkieDeactivateSpeed;

    public float fastTalkieMoveSpeed;
    public float fastTalkieDeactivateSpeed;
    public float fastTalkieDuration;
    private bool fastTalkies = false;

    // private vars
    private int currSegmentIndex = 0;
    private bool playingSegment = false;
    private bool waitingForYesNoInput = false;

    private bool overrideSegmentIndex = false;
    private int newSegmentIndex;
    private bool endingTalkie;

    public void SetFastTalkies(bool opt)
    {
        if (opt)
        {
            fastTalkies = true;
            talkieMoveSpeed = fastTalkieMoveSpeed;
            talkieDeactivateSpeed = fastTalkieDeactivateSpeed;
        }
        else
        {
            fastTalkies = false;
            talkieMoveSpeed = normalTalkieMoveSpeed;
            talkieDeactivateSpeed = normalTalkieDeactivateSpeed;
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 0f);

        // clear yes no buttons
        yesButton.transform.localScale = new Vector3(0f, 0f, 1f);
        noButton.transform.localScale = new Vector3(0f, 0f, 1f);

        yesButton.interactable = false;
        noButton.interactable = false;

        // activate left and right talkies
        leftImage.gameObject.SetActive(true);
        rightImage.gameObject.SetActive(true);
    }

    void Update()
    {
        if (talkiePlaying)
        {
            if (GameManager.instance.devModeActivated)
            {
                // press 'Shift + T' to skip talkie
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        if (!endingTalkie)
                        {
                            GameManager.instance.SendLog(this, "manual end talkie");
                            StopAllCoroutines();
                            StartCoroutine(EndTalkie());
                        }
                    }                
                }
            }
        }
    }

    public void PlayTalkie(TalkieObject talkie)
    {
        talkiePlaying = true;
        currentTalkie = talkie;

        // close settings menu if open
        SettingsManager.instance.CloseSettingsWindow();
        SettingsManager.instance.ToggleMenuButtonActive(false);

        // set fast talkies
        SetFastTalkies(StudentInfoSystem.GetCurrentProfile().talkieFast);

        if (!currentTalkie.quipsCollection)
            StartCoroutine(PlayTalkieRoutine());
        else
            StartCoroutine(PlayTalkieRoutine(true));
    }

    // stops current talkie from continuing
    public void StopTalkieSystem()
    {
        StopAllCoroutines();

        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 0f);
        
        // bring talkies down
        StartCoroutine(MoveObjectRouitne(leftTalkie, leftInactivePos.position, talkieMoveSpeed));
        StartCoroutine(MoveObjectRouitne(rightTalkie, rightInactivePos.position, talkieMoveSpeed));

        // deactivate letterbox and background
        LetterboxController.instance.ToggleLetterbox(false);
        DefaultBackground.instance.Deactivate();

        if (talkiePlaying)
            SettingsManager.instance.ToggleMenuButtonActive(true);

        // stop playing talkie
        talkiePlaying = false;
        currentTalkie = null;
        ResetTalkies();
    }

    private IEnumerator PlayTalkieRoutine(bool playRandomSegment = false)
    {
        ResetTalkies();

        // set audio to be well balanced
        AudioManager.instance.ToggleMusicSmooth(false);

        // set talk vol
        AudioManager.instance.SetTalkVolume(StudentInfoSystem.GetCurrentProfile().talkVol);

        // disable nav buttons on scroll map
        if (SceneManager.GetActiveScene().name == "ScrollMap")
        {
            ScrollMapManager.instance.ToggleNavButtons(false);
        }

        // deterime where to place init talkies
        switch (currentTalkie.start)
        {
            default:
            case TalkieStart.EnterUp:
                leftTalkie.position = leftInactivePos.position;
                rightTalkie.position = rightInactivePos.position;
                break;
            case TalkieStart.EnterSides:
                leftTalkie.position = leftSideHiddenPos.position;
                rightTalkie.position = rightSideHiddenPos.position;
                break;
            case TalkieStart.EnterLeft:
                leftTalkie.position = rightSideHiddenPos.position;
                rightTalkie.position = rightSideHiddenPos.position;
                break;
            case TalkieStart.EnterRight:
                leftTalkie.position = leftSideHiddenPos.position;
                rightTalkie.position = leftSideHiddenPos.position;
                break;
        }

        // make talkies visible
        leftImage.color = new Color(1f, 1f, 1f, 1f);
        rightImage.color = new Color(1f, 1f, 1f, 1f);

        // activate letterbox and background if need be
        if (currentTalkie.addLetterboxBeforeTalkie)
            LetterboxController.instance.ToggleLetterbox(true);
        if (currentTalkie.addBackgroundBeforeTalkie)
            DefaultBackground.instance.Activate();

        yield return new WaitForSeconds(1f);

        // turn on particles
        if (StudentInfoSystem.GetCurrentProfile().talkieParticles)
            ParticleController.instance.isOn = true;

        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 100f / 255f);

        if (playRandomSegment)
        {
            // play one segment randomly
            int index = Random.Range(0, currentTalkie.segmnets.Count);
            StartCoroutine(PlaySegment(currentTalkie.segmnets[index]));
            while (playingSegment)
                yield return null;
        }
        else 
        {
            // play segments in order
            for (currSegmentIndex = 0; currSegmentIndex < currentTalkie.segmnets.Count; currSegmentIndex++)
            {
                StartCoroutine(PlaySegment(currentTalkie.segmnets[currSegmentIndex]));
                while (playingSegment)
                    yield return null;

                // end talkie if segment says so
                if (currentTalkie.segmnets[currSegmentIndex].endTalkieAfterThisSegment)
                    break;

                // override talkie segment
                if (overrideSegmentIndex)
                {
                    overrideSegmentIndex = false;
                    currSegmentIndex = newSegmentIndex;
                }
            }
        }

        /* 
        ################################################
        #   END TALKIE
        ################################################
        */

        yield return new WaitForSeconds(1f);

        StartCoroutine(EndTalkie());
    }

    private IEnumerator EndTalkie()
    {
        endingTalkie = true;

        switch (currentTalkie.ending)
        {
            default:
            case TalkieEnding.ExitDown:
                // bring talkies down
                if (!leftHidden)
                    StartCoroutine(MoveObjectRouitne(leftTalkie, leftInactivePos.position, talkieMoveSpeed));
                if (!rightHidden)
                    StartCoroutine(MoveObjectRouitne(rightTalkie, rightInactivePos.position, talkieMoveSpeed));
                break;
            case TalkieEnding.ExitSides:
                // bring talkies to sides
                if (!leftHidden)
                    StartCoroutine(MoveObjectRouitne(leftTalkie, leftSideHiddenPos.position, talkieMoveSpeed));
                if (!rightHidden)
                    StartCoroutine(MoveObjectRouitne(rightTalkie, rightSideHiddenPos.position, talkieMoveSpeed));
                break;
            case TalkieEnding.ExitLeft:
                // bring talkies left
                if (!leftHidden)
                    StartCoroutine(MoveObjectRouitne(leftTalkie, leftSideHiddenPos.position, talkieMoveSpeed));
                if (!rightHidden)
                    StartCoroutine(MoveObjectRouitne(rightTalkie, leftSideHiddenPos.position, talkieMoveSpeed));
                break;
            case TalkieEnding.ExitRight:
                // bring talkies right
                if (!leftHidden)
                    StartCoroutine(MoveObjectRouitne(leftTalkie, rightSideHiddenPos.position, talkieMoveSpeed));
                if (!rightHidden)
                    StartCoroutine(MoveObjectRouitne(rightTalkie, rightSideHiddenPos.position, talkieMoveSpeed));
                break;
        }

        yield return new WaitForSeconds(talkieMoveSpeed);

        // deactivate letterbox and background
        if (currentTalkie.removeBackgroundAfterTalkie)
            DefaultBackground.instance.Deactivate();
        if (currentTalkie.removeLetterboxAfterTalkie)
            LetterboxController.instance.ToggleLetterbox(false);

        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 0f);

        // turn off particles
        ParticleController.instance.isOn = false;

        // enable nav buttons on scroll map
        if (SceneManager.GetActiveScene().name == "ScrollMap")
        {
            ScrollMapManager.instance.ToggleNavButtons(true);
        }

        // set audio back to what it was before
        AudioManager.instance.ToggleMusicSmooth(true);

        // readd menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // stop playing talkie
        talkiePlaying = false;
        currentTalkie = null;

        // delay end talkie bool
        yield return new WaitForSeconds(1f);
        endingTalkie = false;
    }

    private IEnumerator PlaySegment(TalkieSegment talkieSeg)
    {
        playingSegment = true;

        /* 
        ################################################
        #   LEFT TALKIE
        ################################################
        */

        // in character not NONE
        if (talkieSeg.leftCharacter != TalkieCharacter.None)
        {
            // check if left talkie is the same
            if (currLeftCharacter != talkieSeg.leftCharacter)
            {
                // swap left character sprites
                StartCoroutine(SwapTalkieCharacter(
                    leftTalkie, 
                    leftImage, 
                    talkieSeg.leftCharacter, 
                    talkieSeg.leftEmotionNum, 
                    talkieSeg.leftMouthEnum, 
                    talkieSeg.leftEyesEnum, 
                    true));
                leftHidden = false;
            }
            // if they are the same, check if emotion is the same
            else if (currLeftEmotionNum != talkieSeg.leftEmotionNum ||
                    currLeftMouthEnum  != talkieSeg.leftMouthEnum ||
                    currLeftEyesEnum   != talkieSeg.leftEyesEnum)
            {
                // swap emotion sprites
                SwapTalkieEmotion(leftImage, talkieSeg.leftCharacter, talkieSeg.leftEmotionNum, talkieSeg.leftMouthEnum, talkieSeg.leftEyesEnum);
                leftHidden = false;
            }
        }
        // else remove talkie from scene
        else
        {
            if (!leftHidden)
            {
                leftHidden = true;
                StartCoroutine(MoveObjectRouitne(leftTalkie, leftInactivePos.position, talkieMoveSpeed));
                ResetLeft();
            }    
        }
        
        // set current left talkie values
        currLeftCharacter = talkieSeg.leftCharacter;
        currLeftEmotionNum = talkieSeg.leftEmotionNum;
        currLeftMouthEnum = talkieSeg.leftMouthEnum;
        currLeftEyesEnum = talkieSeg.leftEyesEnum;


        /* 
        ################################################
        #   RIGHT TALKIE
        ################################################
        */

        // in character not NONE
        if (talkieSeg.rightCharacter != TalkieCharacter.None)
        {
            // check if right talkie is the same
            if (currRightCharacter != talkieSeg.rightCharacter)
            {
                // swap right character sprites
                StartCoroutine(SwapTalkieCharacter(
                    rightTalkie, 
                    rightImage, 
                    talkieSeg.rightCharacter, 
                    talkieSeg.rightEmotionNum, 
                    talkieSeg.rightMouthEnum, 
                    talkieSeg.rightEyesEnum, 
                    false));
                rightHidden = false;
            }
            // if they are the same, check if emotion is the same
            else if (currRightEmotionNum != talkieSeg.rightEmotionNum ||
                    currRightMouthEnum  != talkieSeg.rightMouthEnum ||
                    currRightEyesEnum   != talkieSeg.rightEyesEnum)
            {
                // swap emotion sprites
                SwapTalkieEmotion(rightImage, talkieSeg.rightCharacter, talkieSeg.rightEmotionNum, talkieSeg.rightMouthEnum, talkieSeg.rightEyesEnum);
                rightHidden = false;
            }
        }
        // else remove talkie from scene
        else
        {
            if (!rightHidden)
            {
                rightHidden = true;
                StartCoroutine(MoveObjectRouitne(rightTalkie, rightInactivePos.position, talkieMoveSpeed));
                ResetRight();
            }
        }
        
        // set current right talkie values
        currRightCharacter = talkieSeg.rightCharacter;
        currRightEmotionNum = talkieSeg.rightEmotionNum;
        currRightMouthEnum = talkieSeg.rightMouthEnum;
        currRightEyesEnum = talkieSeg.rightEyesEnum;

        // scale and alpha talkies
        if (talkieSeg.activeCharacter == ActiveCharacter.Left)
        {
            StartCoroutine(LerpScaleAndAlpha(leftImage, 1f, 1f, true));
            if (!rightHidden) 
            {
                StartCoroutine(LerpScaleAndAlpha(rightImage, inactiveScale, inactiveAlpha, false));
            }
        }
        else if (talkieSeg.activeCharacter == ActiveCharacter.Right)
        {
            StartCoroutine(LerpScaleAndAlpha(rightImage, 1f, 1f, false));
            if (!leftHidden) 
            {
                StartCoroutine(LerpScaleAndAlpha(leftImage, inactiveScale, inactiveAlpha, true));
            }
        }
        else if (talkieSeg.activeCharacter == ActiveCharacter.Both)
        {
            StartCoroutine(LerpScaleAndAlpha(leftImage, 1f, 1f, true));
            StartCoroutine(LerpScaleAndAlpha(rightImage, 1f, 1f, false));
        }

        // add subtitles
        if (StudentInfoSystem.GetCurrentProfile().talkieSubtitles)
            subtitleText.text = talkieSeg.audioString;
        else
            subtitleText.text = "";

        if (!fastTalkies)
        {
            // play audio
            if (talkieSeg.audioClip != null)
            {
                AudioManager.instance.PlayTalk(talkieSeg.audioClip);
                yield return new WaitForSeconds(talkieSeg.audioClip.length + 0.2f);
            }
            else
            {
                // attempt to match audio clip name to reaction duplicate
                AudioClip clip = TalkieDatabase.instance.GetTalkieReactionDuplicate(talkieSeg.audioClipName);

                if (clip != null)
                {
                    AudioManager.instance.PlayTalk(clip);
                    yield return new WaitForSeconds(clip.length + 0.2f);
                }
                else
                {   
                    print ("no audio clip found: \'" + talkieSeg.audioClipName + "\'");
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

    
        /* 
        ################################################
        #   YES / NO ACTION
        ################################################
        */ 
        if (talkieSeg.requireYN)
        {
            waitingForYesNoInput = true;

            yesGoto = talkieSeg.onYesGoto;
            noGoto = talkieSeg.onNoGoto;

            
            // subtract one from each goto (if less than 0 of course)
            if (yesGoto > 0)
                yesGoto--;
            if (noGoto > 0)
                noGoto--;

            // reveal yes and no buttons
            yesButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            noButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);

            yesButton.interactable = true;
            noButton.interactable = true;

            while (waitingForYesNoInput)
                yield return null;

            yield return new WaitForSeconds(1f);
        }

        playingSegment = false;
    }

    public void OnYesPressed()
    {
        DoYesNoAction(yesGoto, true);
    }

    public void OnNoPressed()
    {
        DoYesNoAction(noGoto, false);
    }

    private void DoYesNoAction(int gotoIndex, bool isYes)
    {   
        // disable buttons
        yesButton.interactable = true;
        noButton.interactable = true;

        // hide yes and no buttons
        yesButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        noButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);

        overrideSegmentIndex = true;
        newSegmentIndex = gotoIndex;

        print ("goto: " + newSegmentIndex);

        // on yes
        if (isYes)
        {
            
        }
        // on no
        else
        {
            doNotContinueToGame = true;
        }

        waitingForYesNoInput = false;
    }

    private IEnumerator LerpTransformScale(Transform tform, float targetScale, float duration)
    {
        float startScale = tform.localScale.y;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                tform.localScale = new Vector3(targetScale, targetScale, 1f);
                break;
            }

            float tempScale = Mathf.Lerp(startScale, targetScale, timer / duration);
            tform.localScale = new Vector3(tempScale, tempScale, 1f);
            yield return null;
        }
    }

    private IEnumerator LerpScaleAndAlpha(Image image, float targetScale, float targetAlpha, bool isLeft)
    {
        float startScale = image.gameObject.transform.localScale.y;
        float startAlpha = image.color.a;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > talkieDeactivateSpeed)
            {
                // flip image around if left
                if (isLeft) image.gameObject.transform.localScale = new Vector3(targetScale, targetScale, 1f);
                else image.gameObject.transform.localScale = new Vector3(targetScale * -1, targetScale, 1f);
                image.color = new Color(1f, 1f, 1f, targetAlpha);
                break;
            }

            float tempScale = Mathf.Lerp(startScale, targetScale, timer / talkieDeactivateSpeed);
            float tempAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / talkieDeactivateSpeed);

            // flip image around if left
            if (isLeft) image.gameObject.transform.localScale = new Vector3(tempScale, tempScale, 1f);
            else image.gameObject.transform.localScale = new Vector3(tempScale * -1, tempScale, 1f);
            image.color = new Color(1f, 1f, 1f, tempAlpha);

            yield return null;
        }
    }

    private IEnumerator SwapTalkieCharacter(Transform tform, Image image, TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes, bool isLeft)
    {   
        // bring down talkie iff not hidden
        if (isLeft && !leftHidden || !isLeft && !rightHidden)
        {
            if (isLeft)
                StartCoroutine(MoveObjectRouitne(tform, leftInactivePos.position, talkieMoveSpeed));
            else
                StartCoroutine(MoveObjectRouitne(tform, rightInactivePos.position, talkieMoveSpeed));
            yield return new WaitForSeconds(talkieMoveSpeed);
        }

        // swap sprite
        image.sprite = TalkieDatabase.instance.GetTalkieSprite(character, emotionNum, mouth, eyes);

        // bring up talkie
        if (isLeft)
            StartCoroutine(MoveObjectRouitne(tform, leftActivePos.position, talkieMoveSpeed));
        else
            StartCoroutine(MoveObjectRouitne(tform, rightActivePos.position, talkieMoveSpeed));
        yield return new WaitForSeconds(talkieMoveSpeed);
    }

    public void SwapTalkieEmotion(Image image, TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes)
    {
        // swap sprite
        image.sprite = TalkieDatabase.instance.GetTalkieSprite(character, emotionNum, mouth, eyes);
    }

    private IEnumerator MoveObjectRouitne(Transform obj, Vector3 targetPos, float duration)
    {
        Vector3 startPos = obj.position;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                obj.position = targetPos;
                break;
            }

            var tempPos = Vector3.Lerp(startPos, targetPos, timer / duration);
            obj.position = tempPos;
            yield return null;
        }
    }

    private void ResetTalkies()
    {
        ResetLeft();
        ResetRight();
        
        // turn off particles
        ParticleController.instance.isOn = false;
    }

    private void ResetLeft()
    {
        leftHidden = true;
        leftTalkie.position = leftInactivePos.position;
        currLeftCharacter = TalkieCharacter.None;
        leftImage.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        leftImage.color = new Color(1f, 1f, 1f, 0f);
        leftImage.sprite = null;
    }

    private void ResetRight()
    {
        rightHidden = true;
        rightTalkie.position = rightInactivePos.position;
        currRightCharacter = TalkieCharacter.None;
        rightImage.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        rightImage.color = new Color(1f, 1f, 1f, 0f);
        rightImage.sprite = null;
    }
}
