using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;

public class StickerSystem : MonoBehaviour
{
    public static StickerSystem instance;

    public LerpableObject wagonBackButton;
    public LerpableObject wagonBG;

    [Header("Get Sticker")]
    public StickerConfirmWindow stickerConfirmWindow;
    public Image revealSticker;
    public VideoPlayer commonVP;
    public VideoPlayer uncommonVP;
    public VideoPlayer rareVP;
    public VideoPlayer legendaryVP;

    private bool waitingOnPlayerInput = false;

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

    [Header("Stickerboards")]
    public List<StickerBoard> stickerboards;

    public LerpableObject stickerboardLayoutGroup;
    public LerpableObject stickerboardBackButton;
    public LerpableObject stickerboardLeftButton;
    public LerpableObject stickerboardRightButton;

    public Transform stickerboardHiddenPos;
    public Transform stickerboardShownPos;

    public float bumpAmount;

    private bool stickerboardButtonsDeactivated = false;
    private int currentBoardIndex = 0;
    private int numActiveBoards;

    [HideInInspector] public bool stickerboardsOpen = false;

    [Header("Board Book")]
    public BoardBookWindow boardBookWindow;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        revealSticker.transform.localScale = Vector3.zero;
        stickerWagonButton.transform.localPosition = new Vector3(stickerWagonButton.transform.localPosition.x, hiddenButtonPos, 1f);
        wagonBackButton.transform.localScale = Vector3.zero;
        stickerboardBackButton.transform.localScale = Vector3.zero;
        stickerboardLeftButton.transform.localScale = Vector3.zero;
        stickerboardRightButton.transform.localScale = Vector3.zero;

        wagonBG.GetComponent<Image>().raycastTarget = false;

        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;
        lesterButton.GetComponent<LesterButton>().interactable = false;
        wagonBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardLeftButton.GetComponent<Button>().interactable = false;
        stickerboardRightButton.GetComponent<Button>().interactable = false;
    }

    void Update() 
    {
        if (waitingOnPlayerInput)
        {
            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
            {
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
                waitingOnPlayerInput = false;
            }
        }
    }

    /* 
    ################################################
    #   BOARD BOOK FUNCTIONS
    ################################################
    */

    public void OpenBoardBook()
    {
        boardBookWindow.OpenWindow();
    }

    /* 
    ################################################
    #   STICKER BOARDS FUNCTIONS
    ################################################
    */

    public void OpenStickerBoards()
    {
        // return if already open
        if (stickerboardsOpen)
            return;
        stickerboardsOpen = true;

        StartCoroutine(OpenStickerBoardsRoutine());
    }

    private IEnumerator OpenStickerBoardsRoutine()
    {
        // set buttons to be not interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = false;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = false;

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("stickerboard_blocker");

        // set stickerboards active
        numActiveBoards = 1;
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        foreach(var board in stickerboards)
        {
            bool isActive = false;
            if (board.boardType == StickerBoardType.Mossy)
            {
                isActive = data.mossyStickerBoard.active;
                board.gameObject.SetActive(isActive);
            }
            else if (board.boardType == StickerBoardType.Emerald)
            {
                isActive = data.emeraldStickerBoard.active;
                board.gameObject.SetActive(isActive);
            }
            else if (board.boardType == StickerBoardType.Beach)
            {
                isActive = data.beachStickerBoard.active;
                board.gameObject.SetActive(isActive);
            }

            if (isActive)
            {
                numActiveBoards++;
            }
        }

        // hide dropdown toolbar
        DropdownToolbar.instance.ToggleToolbar(false);

        // bounce stickerboards into place
        Vector3 bouncePos = stickerboardShownPos.position;
        bouncePos.y += 1f;

        stickerboardLayoutGroup.LerpYPos(bouncePos.y, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        stickerboardLayoutGroup.LerpYPos(stickerboardShownPos.position.y, 0.2f, false);

        yield return new WaitForSeconds(0.5f);

        // show sticker board back button
        stickerboardBackButton.GetComponent<BackButton>().interactable = true;
        stickerboardBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

        // show left and right buttons
        stickerboardLeftButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        stickerboardRightButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        stickerboardLeftButton.GetComponent<Button>().interactable = true;
        stickerboardRightButton.GetComponent<Button>().interactable = true;

        // show inventory button
        StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("stickerboard_blocker");
    }

    public void CloseStickerBoards()
    {
        // return if already open
        if (!stickerboardsOpen)
            return;
        stickerboardsOpen = false;

        StartCoroutine(CloseStickerBoardsRoutine());
    }

    private IEnumerator CloseStickerBoardsRoutine()
    {   
        // hide sticker board back button
        stickerboardBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        // hide left and right buttons
        stickerboardLeftButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        stickerboardRightButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        stickerboardLeftButton.GetComponent<Button>().interactable = false;
        stickerboardRightButton.GetComponent<Button>().interactable = false;

        // hide inventroy button
        StickerInventory.instance.SetInventoryState(InventoryState.Hidden);

        yield return new WaitForSeconds(0.5f);

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("stickerboard_blocker");

        // bounce stickerboards out of place
        Vector3 bouncePos = stickerboardShownPos.position;
        bouncePos.y += 1f;

        stickerboardLayoutGroup.LerpYPos(bouncePos.y, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        stickerboardLayoutGroup.LerpYPos(stickerboardHiddenPos.position.y, 0.2f, false);

        // hide dropdown toolbar
        DropdownToolbar.instance.ToggleToolbar(true);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("stickerboard_blocker");

        // set buttons to be not interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;
    }

    public void OnLeftButtonPressed()
    {
        if (stickerboardButtonsDeactivated)
            return;
        stickerboardButtonsDeactivated = true;

        currentBoardIndex--;
        if (currentBoardIndex < 0)
        {
            currentBoardIndex = 0;
            // bump board left
            StartCoroutine(BumpAnimation(true));
            return;
        }

        // move board left
        StartCoroutine(MoveToLeftBoard());
    }

    public void OnRightButtonPressed()
    {
        if (stickerboardButtonsDeactivated)
            return;
        stickerboardButtonsDeactivated = true;

        currentBoardIndex++;
        if (currentBoardIndex >= numActiveBoards)
        {
            currentBoardIndex = numActiveBoards - 1;
            // bump board right
            StartCoroutine(BumpAnimation(false));
            return;
        }

        // move board right
        StartCoroutine(MoveToRightBoard());
    }

    private IEnumerator MoveToLeftBoard()
    {
        // close inventory iff open
        if (StickerInventory.instance.currentState == InventoryState.Open)
        {   
            StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);
            yield return new WaitForSeconds(0.5f);
        }

        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount + 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        stickerboardButtonsDeactivated = false;
    }

    private IEnumerator MoveToRightBoard()
    {
        // close inventory iff open
        if (StickerInventory.instance.currentState == InventoryState.Open)
        {   
            StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);
            yield return new WaitForSeconds(0.5f);
        }

        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount - 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        stickerboardButtonsDeactivated = false;
    }

    private IEnumerator BumpAnimation(bool isLeft)
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);

        if (isLeft)
        {
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
        }

        stickerboardButtonsDeactivated = false;
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

    public void OnYesButtonStickerConfirmPressed()
    {
        // check to make sure player has sufficent funds
        if (StudentInfoSystem.GetCurrentProfile().goldCoins < 3)
        {
            // play sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);
            return;
        }
        // if they do, remove coins from player profile
        else 
        {
            // play sound
            AudioManager.instance.PlayCoinDrop();
            DropdownToolbar.instance.RemoveGoldCoins(3);
        }

        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        StartCoroutine(RollForNewStickerRoutine());
    }

    public IEnumerator RollForNewStickerRoutine()
    {
        stickerConfirmWindow.CloseWindowForStickerRoll();

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // roll for a random sticker + set reveal sticker
        Sticker sticker = StickerDatabase.instance.RollForSticker();
        revealSticker.sprite = sticker.sprite;

        GameManager.instance.SendLog(this, "you got a sticker! " + sticker.rarity + " " + sticker.id);

        // save sticker to SIS
        StudentInfoSystem.AddStickerToInventory(sticker);

        // fade to black
        FadeObject.instance.FadeOut(1f);
        yield return new WaitForSeconds(1.1f);

        VideoPlayer currentVideo = null;
        // play correct video player
        switch (sticker.rarity)
        {
            default:
            case StickerRarity.Common:
                currentVideo = commonVP;
                break;

            case StickerRarity.Uncommon:
                currentVideo = uncommonVP;
                break;

            case StickerRarity.Rare:
                currentVideo = rareVP;
                break;

            case StickerRarity.Legendary:
                currentVideo = legendaryVP;
                break;
        }

        // wait for video to start
        currentVideo.Play();
        while (!currentVideo.isPlaying)
            yield return null;
        
        // Fade back in 
        FadeObject.instance.FadeIn(1f);

        yield return new WaitForSeconds((float)currentVideo.length - 1.5f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");

        // reveal sticker here after certain amount of time
        revealSticker.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        // wait for player input to continue
        waitingOnPlayerInput = true;
        while (waitingOnPlayerInput)
            yield return null;

        // hide sticker
        revealSticker.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // fade to black
        FadeObject.instance.FadeOut(1f);
        yield return new WaitForSeconds(1f);

        // stop video player
        currentVideo.Stop();

        // Fade back in 
        FadeObject.instance.FadeIn(1f);
        yield return new WaitForSeconds(1f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");

        // show lester
        StickerSystem.instance.lesterAnimator.Play("geckoIntro");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(0.5f);

        // set buttons to be interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;
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
        wagonBackButton.GetComponent<BackButton>().interactable = false;
        wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

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
        wagonBackButton.GetComponent<BackButton>().interactable = false;

        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();

        // show BG
        wagonBG.LerpImageAlpha(wagonBG.GetComponent<Image>(), 0.95f, 1f);
        wagonBG.GetComponent<Image>().raycastTarget = true;

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
        wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        DropdownToolbar.instance.ToggleToolbar(true);

        yield return new WaitForSeconds(0.5f);

        // set buttons interactable
        stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        boardBook.GetComponent<BoardBookButton>().interactable = true;
        lesterButton.GetComponent<LesterButton>().interactable = true;
        wagonBackButton.GetComponent<BackButton>().interactable = true;

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
        wagonBackButton.GetComponent<BackButton>().interactable = false;

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
        wagonBG.LerpImageAlpha(wagonBG.GetComponent<Image>(), 0f, 1f);
        wagonBG.GetComponent<Image>().raycastTarget = false;

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
