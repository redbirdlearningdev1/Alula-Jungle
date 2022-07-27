using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;

public class TalkieManager : MonoBehaviour
{
    public static TalkieManager instance;

    // public vars for other scripts to access
    //[HideInInspector] public bool doNotContinueToGame = false;
    [HideInInspector] public List<bool> yesNoChoices;

    [HideInInspector] public bool talkiePlaying = false; // used to pause routines while talkies are playing
    [HideInInspector] public TalkieObject currentTalkie;

    private AsyncOperationHandle leftHandle;
    private AsyncOperationHandle rightHandle;
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
    public TextMeshProUGUI currentTalkieText;

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

    [Header("Fast Forward Button")]
    public LerpableObject fastForwardButton;
    public float showButtonDelay;
    private Coroutine showFastForwardButtonRoutine;


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

        leftImage.sprite = null;
        rightImage.sprite = null;
        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 0f);
        currentTalkieText.text = "";

        // clear yes no buttons
        yesButton.transform.localScale = new Vector3(0f, 0f, 1f);
        noButton.transform.localScale = new Vector3(0f, 0f, 1f);

        yesButton.interactable = false;
        noButton.interactable = false;

        // activate left and right talkies
        leftImage.gameObject.SetActive(true);
        rightImage.gameObject.SetActive(true);

        // hide button
        fastForwardButton.transform.localScale = new Vector3(0f, 0f, 1f);
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            // press 'Shift + T' to skip talkie
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    SkipTalkie();
                }
            }
        }
    }

    public void SkipTalkie()
    {
        if (!talkiePlaying)
        {
            return;
        }
            

        if (!endingTalkie)
        {
            GameManager.instance.SendLog(this, "skipping talkie");
            StopAllCoroutines();
            StartCoroutine(EndTalkie());
        }
    }

    public void PlayTalkie(TalkieObject talkie)
    {
        talkiePlaying = true;
        currentTalkie = talkie;

        // close settings menu if open
        SettingsManager.instance.CloseAllSettingsWindows();

        // set fast talkies
        SetFastTalkies(StudentInfoSystem.GetCurrentProfile().talkieFast);

        // start talkie
        StartCoroutine(PlayTalkieRoutine());
    }

    // stops current talkie from continuing
    public void StopTalkieSystem()
    {
        StopAllCoroutines();

        // clear subtitles
        subtitleText.text = "";
        subtitleBox.color = new Color(0f, 0f, 0f, 0f);
        currentTalkieText.text = "";

        // bring talkies down
        StartCoroutine(MoveObjectRouitne(leftTalkie, leftInactivePos.position, talkieMoveSpeed));
        StartCoroutine(MoveObjectRouitne(rightTalkie, rightInactivePos.position, talkieMoveSpeed));

        // deactivate letterbox and background
        LetterboxController.instance.ToggleLetterbox(false);
        DefaultBackground.instance.Deactivate();

        // stop playing talkie
        talkiePlaying = false;
        currentTalkie = null;
        ResetTalkies();
    }

    public void OnFastForwardButtonPressed()
    {
        fastForwardButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.2f, 0.2f);
        fastForwardButton.GetComponent<WiggleController>().StopWiggle();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.FastForwardSound, 0.5f);
        SkipTalkie();
    }

    private IEnumerator DelayShowFastForwardButton()
    {
        yield return new WaitForSeconds(showButtonDelay);
        fastForwardButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.2f, 0.2f);
        fastForwardButton.GetComponent<WiggleController>().StartWiggle();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
    }

    private IEnumerator PlayTalkieRoutine()
    {
        ResetTalkies();

        // set audio to be well balanced
        AudioManager.instance.ToggleMusicSmooth(false);

        // set talk vol
        AudioManager.instance.SetTalkVolume(StudentInfoSystem.GetCurrentProfile().talkVol);

        // check if talkie object has a yes/no branch
        bool addFastForwardButton = true;
        foreach (var seg in currentTalkie.segmnets)
        {
            if (seg.requireYN)
            {
                addFastForwardButton = false;
                break;
            }
        }
        // check if object is quip collection
        if (currentTalkie.quipsCollection)
        {
            addFastForwardButton = false;
        }
        
        if (addFastForwardButton)
        {
            // show fast forward button after delay
            showFastForwardButtonRoutine = StartCoroutine(DelayShowFastForwardButton());
        }
       

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

        // set current talkie name
        if (GameManager.instance.devModeActivated)
            currentTalkieText.text = currentTalkie.name;

        // segment to start talkie on
        int startIndex = 0;

        // only for quips collection
        if (currentTalkie.quipsCollection)
        {
            // start talkie at a random VALID segment
            startIndex = currentTalkie.validQuipIndexes[Random.Range(0, currentTalkie.validQuipIndexes.Count)];
        }

        // play segments in order
        for (currSegmentIndex = startIndex; currSegmentIndex < currentTalkie.segmnets.Count; currSegmentIndex++)
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

        /* 
        ################################################
        #   END TALKIE
        ################################################
        */

        StartCoroutine(EndTalkie());
    }

    private IEnumerator EndTalkie()
    {
        endingTalkie = true;

        if (showFastForwardButtonRoutine != null)
        {
            // stop fast forward routine
            StopCoroutine(showFastForwardButtonRoutine);
        }

        // hide button iff shown
        if (fastForwardButton.transform.localScale.x > 0f)
        {
            fastForwardButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.2f, 0.2f);
            fastForwardButton.GetComponent<WiggleController>().StopWiggle();
        }
        
        // hide talkie sprites
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
        currentTalkieText.text = "";

        // turn off particles
        ParticleController.instance.isOn = false;
        ParticleController.instance.SetActiveParticles(TalkieCharacter.None);

        // enable nav buttons on scroll map
        if (SceneManager.GetActiveScene().name == "ScrollMap")
        {
            ScrollMapManager.instance.ToggleNavButtons(true);
        }

        // set audio back to what it was before
        AudioManager.instance.ToggleMusicSmooth(true);
        AudioManager.instance.StopTalk();

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

                StartCoroutine(ChangeParticles(talkieSeg.leftCharacter));
            }
            // if they are the same, check if emotion is the same
            else if (currLeftEmotionNum != talkieSeg.leftEmotionNum ||
                    currLeftMouthEnum != talkieSeg.leftMouthEnum ||
                    currLeftEyesEnum != talkieSeg.leftEyesEnum)
            {
                // swap emotion sprites
                SwapTalkieEmotion(leftImage, talkieSeg.leftCharacter, talkieSeg.leftEmotionNum, talkieSeg.leftMouthEnum, talkieSeg.leftEyesEnum, true);
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
                    currRightMouthEnum != talkieSeg.rightMouthEnum ||
                    currRightEyesEnum != talkieSeg.rightEyesEnum)
            {
                // swap emotion sprites
                SwapTalkieEmotion(rightImage, talkieSeg.rightCharacter, talkieSeg.rightEmotionNum, talkieSeg.rightMouthEnum, talkieSeg.rightEyesEnum, false);
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
            StartCoroutine(ChangeParticles(talkieSeg.leftCharacter));
            StartCoroutine(LerpScaleAndAlpha(leftImage, 1f, 1f, true));
            if (!rightHidden)
            {
                StartCoroutine(LerpScaleAndAlpha(rightImage, inactiveScale, inactiveAlpha, false));
            }
        }
        else if (talkieSeg.activeCharacter == ActiveCharacter.Right)
        {
            StartCoroutine(ChangeParticles(talkieSeg.rightCharacter));
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
            //Debug.Log("Talkie Seg clip: " + talkieSeg.audioClip);
            if (talkieSeg.audioClip.RuntimeKeyIsValid())
            {
                //Debug.Log("IsValid");
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(talkieSeg.audioClip));
                yield return cd.coroutine;
                AudioManager.instance.PlayTalk(talkieSeg.audioClip);
                yield return new WaitForSeconds(cd.GetResult() + 0.2f);
            }
            else
            {
                //Debug.Log("IsNotValid");

                // attempt to match audio clip name to reaction duplicate
                AssetReference clip = TalkieDatabase.instance.GetTalkieReactionDuplicate(talkieSeg.audioClipName);
                //Debug.Log(clip);

                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
                yield return cd.coroutine;

                if (clip != null)
                {
                    AudioManager.instance.PlayTalk(clip);
                    yield return new WaitForSeconds(cd.GetResult() + 0.2f);
                }
                else
                {
                    Debug.LogError("no audio clip found: \'" + talkieSeg.audioClipName + "\' in: \'" + currentTalkie.name + "\'");
                    yield return new WaitForSeconds(0.2f);
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
        }

        playingSegment = false;
    }

    public void OnYesPressed()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
        DoYesNoAction(yesGoto, true);
    }

    public void OnNoPressed()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
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

        // on yes
        if (isYes)
        {
            yesNoChoices.Add(true);
        }
        // on no
        else
        {
            yesNoChoices.Add(false);
            //doNotContinueToGame = true;
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

        // Unload previous sprite
        if (isLeft)
        {
            if (leftHandle.IsValid())
            {
                Addressables.Release(leftHandle);
            }
        }
        else
        {
            if (rightHandle.IsValid())
            {
                Addressables.Release(rightHandle);
            }
        }

        // swap sprite
        AssetReferenceAtlasedSprite spriteRef = TalkieDatabase.instance.GetTalkieSprite(character, emotionNum, mouth, eyes, currSegmentIndex);
        if (spriteRef.OperationHandle.IsValid())
        {
            image.sprite = (Sprite)spriteRef.OperationHandle.Result;
        }
        else
        {
            AsyncOperationHandle handle = spriteRef.LoadAssetAsync<Sprite>();
            //Debug.LogError("Loading sprite: " + character);
            yield return handle;
            //Debug.LogError("Finished Loading sprite: " + character);
            image.sprite = (Sprite)handle.Result;
        }

        // bring up talkie
        if (isLeft)
            StartCoroutine(MoveObjectRouitne(tform, leftActivePos.position, talkieMoveSpeed));
        else
            StartCoroutine(MoveObjectRouitne(tform, rightActivePos.position, talkieMoveSpeed));
        yield return new WaitForSeconds(talkieMoveSpeed);
    }

    public void SwapTalkieEmotion(Image image, TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes, bool isLeft)
    {
        StartCoroutine(LoadAndSwapTalkieEmotion(image, character, emotionNum, mouth, eyes, isLeft));
    }

    private IEnumerator LoadAndSwapTalkieEmotion(Image image, TalkieCharacter character, int emotionNum, TalkieMouth mouth, TalkieEyes eyes, bool isLeft)
    {
        // Unload previous sprite
        if (isLeft)
        {
            if (leftHandle.IsValid())
            {
                Addressables.Release(leftHandle);
            }
        }
        else
        {
            if (rightHandle.IsValid())
            {
                Addressables.Release(rightHandle);
            }
        }

        // swap sprite
        AssetReferenceAtlasedSprite spriteRef = TalkieDatabase.instance.GetTalkieSprite(character, emotionNum, mouth, eyes, currSegmentIndex);
        if (spriteRef.OperationHandle.IsValid())
        {
            image.sprite = (Sprite)spriteRef.OperationHandle.Result;
        }
        else
        {
            AsyncOperationHandle handle = spriteRef.LoadAssetAsync<Sprite>();
            //Debug.LogError("Loading sprite: " + character);
            yield return handle;
            //Debug.LogError("Finished Loading sprite: " + character);
            image.sprite = (Sprite)handle.Result;
        }
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
    private IEnumerator ChangeParticles(TalkieCharacter character)
    {
        ParticleController.instance.SetActiveParticles(character);
        yield return new WaitForSeconds(0f);
    }

    private void ResetTalkies()
    {
        ResetLeft();
        ResetRight();

        // turn off particles
        ParticleController.instance.isOn = false;
        ParticleController.instance.SetActiveParticles(TalkieCharacter.None);
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
