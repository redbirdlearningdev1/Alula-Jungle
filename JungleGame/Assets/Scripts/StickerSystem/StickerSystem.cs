using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StickerSystem : MonoBehaviour
{
    public static StickerSystem instance;

    public LerpableObject backButton;
    public LerpableObject BG;

    [Header("Get Sticker")]
    public StickerConfirmWindow stickerConfirmWindow;

    [Header("Sticker Button")]
    public Transform stickerWagonButton;
    public float hiddenButtonPos;
    public float shownButtonPos;

    [Header("Wagon Animators")]
    public Animator wagonMovementAnimator;
    public Animator wagonImageAnimator;
    public Animator lesterAnimator;

    [Header("Wagon Parts")]
    public Transform stickerBoard;
    public Transform boardBook;
    public Transform lesterButton;

    private bool wagonButtonShown = false;
    private bool movingWagonButton = false;

    [HideInInspector] public bool wagonOpen = false;
    [HideInInspector] public bool wagonAnimating = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        stickerWagonButton.transform.localPosition = new Vector3(stickerWagonButton.transform.localPosition.x, hiddenButtonPos, 1f);
        backButton.transform.localScale = Vector3.zero;

        BG.GetComponent<Image>().raycastTarget = false;

        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;
        lesterButton.GetComponent<LesterButton>().interactable = false;
        backButton.GetComponent<BackButton>().interactable = false;
    }

    /* 
    ################################################
    #   LESTER STICKER FUNCTIONS
    ################################################
    */

    public void OnLesterPressed()
    {
        stickerConfirmWindow.OpenWindow();
    }

    public void RollForNewSticker()
    {
        stickerConfirmWindow.CloseWindow();
    }

    /* 
    ################################################
    #   WAGON FUNCTIONS
    ################################################
    */

    public void ToggleStickerWagon()
    {
        // return if wagon is animating
        if (wagonAnimating)
        {
            return;
        }
        wagonAnimating = true;

        wagonOpen = !wagonOpen;

        if (wagonOpen)
        {
            StartCoroutine(ShowWagon());
        }
        else
        {
            StartCoroutine(HideWagon());
        }
    }

    public void OnBackButtonPressed()
    {
        // return if wagon is animating
        if (wagonAnimating)
        {
            return;
        }

        // hide back button
        backButton.GetComponent<BackButton>().interactable = false;
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        wagonAnimating = true;
        wagonOpen = false;
        StartCoroutine(HideWagon());
    }

    private IEnumerator ShowWagon()
    {
        // add raycast
        RaycastBlockerController.instance.CreateRaycastBlocker("show_wagon_raycast");

        // set buttons not interactable
        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;
        lesterButton.GetComponent<LesterButton>().interactable = false;
        backButton.GetComponent<BackButton>().interactable = false;

        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();

        // show BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0.95f, 1f);
        BG.GetComponent<Image>().raycastTarget = true;

        // hide settings and wagon buttons
        SettingsManager.instance.ToggleMenuButtonActive(false);
        this.ToggleWagonButtonActive(false);

        // wagon rolls in
        wagonMovementAnimator.Play("WagonShow");
        yield return new WaitForSeconds(0.8f);
        wagonImageAnimator.Play("WagonMoving");

        yield return new WaitForSeconds(3f);

        // stop wagon
        lesterAnimator.Play("geckoIntro");

        // show back button + dropdown toolbar
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        DropdownToolbar.instance.ToggleToolbar(true);

        yield return new WaitForSeconds(0.5f);

        // set buttons interactable
        stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        boardBook.GetComponent<BoardBookButton>().interactable = true;
        lesterButton.GetComponent<LesterButton>().interactable = true;
        backButton.GetComponent<BackButton>().interactable = true;

        // animation done - remove raycast
        RaycastBlockerController.instance.RemoveRaycastBlocker("show_wagon_raycast");
        wagonAnimating = false;
    }

    private IEnumerator HideWagon()
    {
        // add raycast
        RaycastBlockerController.instance.CreateRaycastBlocker("hide_wagon_raycast");

        // set buttons not interactable
        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;
        lesterButton.GetComponent<LesterButton>().interactable = false;
        backButton.GetComponent<BackButton>().interactable = false;

        // hide back button + dropdown toolbar
        DropdownToolbar.instance.ToggleToolbar(false);

        // hide lester
        lesterAnimator.Play("geckoLeave");
        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(1f);

        // wagon rolls in
        wagonMovementAnimator.Play("WagonExit");
        wagonImageAnimator.Play("WagonMoving");

        yield return new WaitForSeconds(3f);

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 1f);
        BG.GetComponent<Image>().raycastTarget = false;

        // show settings and wagon buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        this.ToggleWagonButtonActive(true);

        yield return new WaitForSeconds(0.5f);

        // animation done - remove raycast
        RaycastBlockerController.instance.RemoveRaycastBlocker("hide_wagon_raycast");
        wagonAnimating = false;
    }

    /* 
    ################################################
    #   BUTTON FUNCTIONS
    ################################################
    */

    public void ToggleStickerButtonWiggleGlow(bool opt)
    {
        if (opt)
        {
            stickerWagonButton.GetComponent<WiggleController>().StartWiggle();
            ImageGlowController.instance.SetImageGlow(stickerWagonButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
        }
        else
        {
            stickerWagonButton.GetComponent<WiggleController>().StopWiggle();
            ImageGlowController.instance.SetImageGlow(stickerWagonButton.GetComponent<Image>(), false, GlowValue.none);
        }
    }

    public void SetWagonButton(bool opt)
    {
        if (opt)
        {
            wagonButtonShown = true;
            movingWagonButton = false;
            stickerWagonButton.transform.localPosition = new Vector3(stickerWagonButton.transform.localPosition.x, shownButtonPos, 1f);
        }
        else 
        {
            wagonButtonShown = false;
            movingWagonButton = false;
            stickerWagonButton.transform.localPosition = new Vector3(stickerWagonButton.transform.localPosition.x, shownButtonPos, 1f);
        }
    }

    public void ToggleWagonButtonActive(bool opt)
    {
        StartCoroutine(ToggleWagonButtonRoutine(opt));
    }

    private IEnumerator ToggleWagonButtonRoutine(bool opt)
    {
        // check if bools are equal
        if (opt == wagonButtonShown)
            yield break;

        // wait for bool to be false
        while (movingWagonButton)
            yield return null;

        movingWagonButton = true;

        if (opt)
        {
            stickerWagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonPos - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            stickerWagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonPos, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            stickerWagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonPos - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            stickerWagonButton.GetComponent<LerpableObject>().LerpYPos(hiddenButtonPos, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }

        wagonButtonShown = opt;
        movingWagonButton = false;
    }
}
