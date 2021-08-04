using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkieManager : MonoBehaviour
{
    public static TalkieManager instance;

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
    public Transform inactivePos;
    public Transform activePos;
    public Transform leftSideHiddenPos;
    public Transform rightSideHiddenPos;

    [Header("Talkie Variables")]
    public float inactiveScale;
    public float inactiveAlpha;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlayTalkie(TalkieObject talkie)
    {
        talkiePlaying = true;
        currentTalkie = talkie;
        StartCoroutine(PlayTalkieRoutine());
    }

    private IEnumerator PlayTalkieRoutine()
    {
        ResetTalkies();

        // activate letterbox and background
        LetterboxController.instance.ToggleLetterbox(true);
        DefaultBackground.instance.Activate();

        yield return new WaitForSeconds(1f);

        // play segments in order
        foreach (var talkieSeg in currentTalkie.segmnets)
        {
            float waitTime = 0f;
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
                    waitTime += talkieMoveSpeed;
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
                    StartCoroutine(MoveObjectRouitne(leftTalkie, inactivePos.position, talkieMoveSpeed));
                    yield return new WaitForSeconds(talkieMoveSpeed);
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
                    waitTime += talkieMoveSpeed;
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
                    StartCoroutine(MoveObjectRouitne(rightTalkie, inactivePos.position, talkieMoveSpeed));
                    yield return new WaitForSeconds(talkieMoveSpeed);
                    ResetRight();
                }
            }
            
            // set current right talkie values
            currRightCharacter = talkieSeg.rightCharacter;
            currRightEmotionNum = talkieSeg.rightEmotionNum;
            currRightMouthEnum = talkieSeg.rightMouthEnum;
            currRightEyesEnum = talkieSeg.rightEyesEnum;
            
            // wait time
            yield return new WaitForSeconds(waitTime);

            // scale and alpha talkies
            if (talkieSeg.activeCharacter == ActiveCharacter.Left)
            {
                StartCoroutine(LerpScaleAndAlpha(leftImage, 1f, 1f, true));
                if (!rightHidden) 
                {
                    StartCoroutine(LerpScaleAndAlpha(rightImage, inactiveScale, inactiveAlpha, true));
                }
                //yield return new WaitForSeconds(talkieDeactivateSpeed);
            }
            else if (talkieSeg.activeCharacter == ActiveCharacter.Right)
            {
                StartCoroutine(LerpScaleAndAlpha(rightImage, 1f, 1f, false));
                if (!leftHidden) 
                {
                    StartCoroutine(LerpScaleAndAlpha(leftImage, inactiveScale, inactiveAlpha, false));
                }
                //yield return new WaitForSeconds(talkieDeactivateSpeed);
            }

            // play audio
            AudioManager.instance.PlayTalk(talkieSeg.audioClip);
            yield return new WaitForSeconds(talkieSeg.audioClip.length + 0.5f);
        }

        /* 
        ################################################
        #   END TALKIE
        ################################################
        */

        yield return new WaitForSeconds(1f);

        // bring talkies down
        StartCoroutine(MoveObjectRouitne(leftTalkie, inactivePos.position, talkieMoveSpeed));
        StartCoroutine(MoveObjectRouitne(rightTalkie, inactivePos.position, talkieMoveSpeed));

        yield return new WaitForSeconds(talkieMoveSpeed);

        // deactivate letterbox and background
        LetterboxController.instance.ToggleLetterbox(false);
        DefaultBackground.instance.Deactivate();

        // stop playing talkie
        talkiePlaying = false;
        currentTalkie = null;
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
            StartCoroutine(MoveObjectRouitne(tform, inactivePos.position, talkieMoveSpeed));
            yield return new WaitForSeconds(talkieMoveSpeed);
        }

        // swap sprite
        image.sprite = TalkieDatabase.instance.GetTalkieSprite(character, emotionNum, mouth, eyes);

        // bring down talkie
        StartCoroutine(MoveObjectRouitne(tform, activePos.position, talkieMoveSpeed));
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
    }

    private void ResetLeft()
    {
        leftHidden = true;
        leftTalkie.position = inactivePos.position;
        currLeftCharacter = TalkieCharacter.None;
        leftImage.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        leftImage.color = new Color(1f, 1f, 1f, 1f);
        leftImage.sprite = null;
    }

    private void ResetRight()
    {
        rightHidden = true;
        rightTalkie.position = inactivePos.position;
        currRightCharacter = TalkieCharacter.None;
        rightImage.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        rightImage.color = new Color(1f, 1f, 1f, 1f);
        rightImage.sprite = null;
    }
}
