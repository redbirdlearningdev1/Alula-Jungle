using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerConfirmWindow : MonoBehaviour
{
    public static StickerConfirmWindow instance;

    public LerpableObject BG;
    public LerpableObject window;
    public Button noButton;

    public bool windowActive;

    void Awake()
    {
        if (instance == null)
            instance = this;

        BG.GetComponent<Image>().raycastTarget = false;

        window.transform.localScale = new Vector3(0f, 0f, 1f);
        windowActive = false;
    }

    public void OpenWindow()
    {
        if (!windowActive)
        {
            StartCoroutine(OpenWindowRoutine());
        }
    }

    private IEnumerator OpenWindowRoutine()
    {
        // hide wagon lester
        StickerSystem.instance.lesterAnimator.Play("geckoLeave");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = true;

        // set buttons to be not interactable
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = false;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = false;

        // tutorial stuff
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // disable no button
            noButton.interactable = false;
        }
        else
        {
            // enable no button
            noButton.interactable = true;
            // hide back button
            StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = false;
            StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // show BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0.95f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = true;

        // play lester buy talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterBuy_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        if (TalkieManager.instance.yesNoChoices.Count == 1)
        {
            // if player chooses yes
            if (TalkieManager.instance.yesNoChoices[0])
            {
                TalkieManager.instance.yesNoChoices.Clear();

                // show window
                window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
                yield return new WaitForSeconds(0.5f);
                windowActive = true;
                
                yield break;
                
            }
            else // if the player chooses no
            {
                TalkieManager.instance.yesNoChoices.Clear();

                // play lester buy talkie 2
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterBuy_1_p3"));
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // remove BG
                BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
                BG.GetComponent<Image>().raycastTarget = false;

                // set buttons interactable
                if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
                {
                    StickerSystem.instance.lesterAnimator.Play("geckoIntro");
                    StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
                    StickerSystem.instance.lesterButton.GetComponent<WiggleController>().StartWiggle();
                    StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

                    yield return new WaitForSeconds(0.5f);

                    StickerSystem.instance.lesterButton.GetComponent<LesterButton>().interactable = true;
                }
                else
                {
                    // show wagon lester
                    StickerSystem.instance.lesterAnimator.Play("geckoIntro");
                    StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
                    StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

                    yield return new WaitForSeconds(0.5f);

                    // set buttons to be interactable 
                    StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
                    StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
                    StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;

                    // show back button
                    StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = true;
                    StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
                }
                
                yield break;
            }
        }
        else
        {
            TalkieManager.instance.yesNoChoices.Clear();
            Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
        }
    }

    public void OnNoPressed()
    {
        StartCoroutine(OnNoPressedRoutine());
    }

    private IEnumerator OnNoPressedRoutine()
    {   
        // hide window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        windowActive = false;

        // play lester buy talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterBuy_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = false;

        // show wagon lester
        StickerSystem.instance.lesterAnimator.Play("geckoIntro");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(0.5f);

        // set buttons to be interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;

        // show back button
        StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = true;
        StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    public void CloseWindow()
    {
        if (windowActive)
        {
            StartCoroutine(CloseWindowRoutine());
        }
    }

    private IEnumerator CloseWindowRoutine()
    {
        // hide window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        windowActive = false;

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = false;

        // show wagon lester
        StickerSystem.instance.lesterAnimator.Play("geckoIntro");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(0.5f);

        // set buttons to be interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;

        // show back button
        StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = true;
        StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    public void CloseWindowForStickerRoll()
    {
        StartCoroutine(CloseWindowForStickerRollRoutine());
    }

    private IEnumerator CloseWindowForStickerRollRoutine()
    {
        // close window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        windowActive = false;
    }
}
