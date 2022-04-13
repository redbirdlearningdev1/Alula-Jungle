using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class StickerSystem : MonoBehaviour
{
    public static StickerSystem instance;

    public LerpableObject wagonBackButton;
    public LerpableObject wagonBG;
    public LerpableObject talkieBG;

    [Header("Get Sticker")]
    public StickerConfirmWindow stickerConfirmWindow;
    public Image revealSticker;
    public Transform stickerToolbarPos;
    public LerpableObject lesterSpeechBubble;
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
    
    private List<StickerBoard> activeStickerboards;
    private bool stickerboardButtonsDeactivated = false;
    private int currentBoardIndex = 0;
    private int numActiveBoards;

    [HideInInspector] public bool stickerboardsOpen = false;

    [Header("Board Book")]
    public BoardBookWindow boardBookWindow;

    [Header("Stickers")]
    public GameObject gluedStickerObject;
    public float stickerMoveSpeed;
    public Transform selectedStickerParent;
    public Transform gluedStickerParent;
    public Image hideInventoryArea;
    public Image openInventoryArea;
    public Image validInventoryArea;
    public LerpableObject deleteStickerButton;


    private Transform currentHeldSticker;
    private bool holdingSticker;
    private bool readyToPlaceSticker;
    private bool deleteStickerMode;
    [HideInInspector] public bool readyToGlueSticker;




    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        activeStickerboards = new List<StickerBoard>();
        activeStickerboards.Add(stickerboards[0]);

        revealSticker.transform.localScale = Vector3.zero;
        lesterSpeechBubble.transform.localScale = Vector3.zero;

        stickerWagonButton.transform.localPosition = new Vector3(stickerWagonButton.transform.localPosition.x, hiddenButtonPos, 1f);
        wagonBackButton.transform.localScale = Vector3.zero;
        stickerboardBackButton.transform.localScale = Vector3.zero;
        stickerboardLeftButton.transform.localScale = Vector3.zero;
        stickerboardRightButton.transform.localScale = Vector3.zero;
        deleteStickerButton.transform.localScale = Vector3.zero;

        wagonBG.GetComponent<Image>().raycastTarget = false;

        hideInventoryArea.raycastTarget = false;
        openInventoryArea.raycastTarget = false;

        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;
        lesterButton.GetComponent<LesterButton>().interactable = false;
        wagonBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardLeftButton.GetComponent<Button>().interactable = false;
        stickerboardRightButton.GetComponent<Button>().interactable = false;
        deleteStickerButton.GetComponent<Button>().interactable = false;

        talkieBG.GetComponent<Image>().raycastTarget = false;
        talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0f, 0f);

        // set video player cameras
        commonVP.targetCamera = GameManager.instance.globalCamera;
        uncommonVP.targetCamera = GameManager.instance.globalCamera;
        rareVP.targetCamera = GameManager.instance.globalCamera;
        legendaryVP.targetCamera = GameManager.instance.globalCamera;
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

        if (currentHeldSticker == null)
        {
            holdingSticker = false;
        }

        if (holdingSticker)
        {
            // let go of sticker after clicking again
            if (Input.GetMouseButtonDown(0) && readyToPlaceSticker)
            {
                holdingSticker = false; 

                hideInventoryArea.raycastTarget = false;
                openInventoryArea.raycastTarget = false;

                // stop sticker wiggle
                currentHeldSticker.GetComponent<StickerImage>().wiggleController.StopWiggle();

                // return sticker to inventory if inventory is open
                if (StickerInventory.instance.currentState == InventoryState.Open)
                {
                    ReturnStickerToInventory();
                }
                // place sticker on current stickerboard (not glue)
                else
                {
                    StartCoroutine(PlaceStickerOnStickerBoard());
                }
            }

            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(currentHeldSticker.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - stickerMoveSpeed, Time.deltaTime * 60));
            currentHeldSticker.transform.position = pos;

            // send raycast to check for open inventory
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("OpenInventoryArea") && StickerInventory.instance.currentState != InventoryState.Open)
                    {
                        StickerInventory.instance.SetInventoryState(InventoryState.Open);
                    }
                    else if (result.gameObject.transform.CompareTag("HideInventoryArea") && StickerInventory.instance.currentState != InventoryState.ShowTab)
                    {
                        StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);
                    }
                }
            }
        }   
    }

    /* 
    ################################################
    #   STICKER FUNCTIONS
    ################################################
    */

    public void ToggleDeleteStickerMode()
    {
        deleteStickerMode = !deleteStickerMode;

        if (deleteStickerMode)
        {
            SetDeleteStickerModeON();
        }
        else
        {
            SetDeleteStickerModeOFF();
        }
    }

    public void SetDeleteStickerModeON()
    {
        deleteStickerMode = true;

        deleteStickerButton.SquishyScaleLerp(new Vector2(0.9f, 0.9f), new Vector2(1.1f, 1.1f), 0.1f, 0.1f);

        foreach (Transform child in gluedStickerParent)
        {
            if (child.tag == "GluedSticker")
            {
                child.GetComponent<WiggleController>().StartWiggle();
                child.GetComponent<GluedSticker>().SetDeleteButton(true);
            }
        }
    }

    public void SetDeleteStickerModeOFF()
    {
        deleteStickerMode = false;

        deleteStickerButton.SquishyScaleLerp(new Vector2(0.9f, 0.9f), new Vector2(1.1f, 1.1f), 0.1f, 0.1f);

        foreach (Transform child in gluedStickerParent)
        {
            if (child.tag == "GluedSticker")
            {
                child.GetComponent<WiggleController>().StopWiggle();
                child.GetComponent<GluedSticker>().SetDeleteButton(false);
            }
        }
    }

    public StickerBoardType GetCurrentBoard()
    {
        return activeStickerboards[currentBoardIndex].boardType;
    }

    public void SetCurrentHeldSticker(Transform sticker)
    {
        // set delete sticker mode off
        SetDeleteStickerModeOFF();
        StickerSystem.instance.deleteStickerButton.GetComponent<Button>().interactable = false;

        currentHeldSticker = sticker;
        currentHeldSticker.SetParent(selectedStickerParent);
        currentHeldSticker.GetComponent<StickerImage>().wiggleController.StartWiggle();

        readyToPlaceSticker = false;
        holdingSticker = true;
        StartCoroutine(ReadyToPlaceStickerDelay(0.5f));

        hideInventoryArea.raycastTarget = true;
        openInventoryArea.raycastTarget = true;
    }

    private IEnumerator ReadyToPlaceStickerDelay(float time)
    {
        yield return new WaitForSeconds(time);
        readyToPlaceSticker = true;
    }

    public void ReturnStickerToInventory()
    {
        StartCoroutine(ReturnStickerToInventoryRoutine());
    }

    private IEnumerator ReturnStickerToInventoryRoutine()
    {
        // return if held sticker is null
        if (currentHeldSticker == null)
            yield break;

        // set delete sticker mode off
        StickerSystem.instance.deleteStickerButton.GetComponent<Button>().interactable = true;

        readyToGlueSticker = false;
        currentHeldSticker.GetComponent<StickerImage>().HideStickerButtons();
        currentHeldSticker.SetParent(selectedStickerParent);
        currentHeldSticker.GetComponent<LerpableObject>().LerpPosToTransform(currentHeldSticker.GetComponent<StickerImage>().inventorySticker.transform, 0.25f, false);
        yield return new WaitForSeconds(0.25f);
        currentHeldSticker.SetParent(currentHeldSticker.GetComponent<StickerImage>().inventorySticker.transform);
        StickerInventory.instance.UpdateStickerInventory();
        StickerInventory.instance.ToggleStickerColliders(true);
        currentHeldSticker = null;
    }

    private IEnumerator PlaceStickerOnStickerBoard()
    {
        // make area raycastable
        validInventoryArea.raycastTarget = true;

        // make sure sticker is in valid bounds
        bool validArea = false;
        // send raycast to check for open inventory
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach(var result in raycastResults)
            {
                if (result.gameObject.transform.CompareTag("ValidStickerArea"))
                {
                    validArea = true;
                }
            }
        }

        // make area no longer raycastable
        validInventoryArea.raycastTarget = false;

        if (validArea)
        {
            // set image parent as current stickerboard
            currentHeldSticker.SetParent(gluedStickerParent);
            currentHeldSticker.GetComponent<StickerImage>().ShowStickerButtons();
            // ready to glue sticker
            readyToGlueSticker = true;
        }
        else
        {
            // return sticker to inventory
            ReturnStickerToInventory();
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);
        }
        
    
        yield return null;
    }

    /* 
    ################################################
    #   BOARD BOOK FUNCTIONS
    ################################################
    */

    public void OpenBoardBook()
    {
        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();
        
        boardBook.GetComponent<WiggleController>().StopWiggle();
        boardBookWindow.OpenWindow();
    }

    /* 
    ################################################
    #   STICKER BOARDS FUNCTIONS
    ################################################
    */

    public void GlueSelectedStickerToBoard(Sticker sticker, Vector3 pos, Vector3 scale, float zAngle)
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SelectBoop, 0.5f);

        GameObject gluedSticker = Instantiate(gluedStickerObject, gluedStickerParent);
        gluedSticker.GetComponent<GluedSticker>().SetStickerData(sticker, pos, scale, zAngle);

        // set delete sticker mode off
        SetDeleteStickerModeOFF();
        StickerSystem.instance.deleteStickerButton.GetComponent<Button>().interactable = true;

        // play lester tutorial part 3
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            StartCoroutine(AfterGlueStickerTutorial());
        }
    }

    private IEnumerator AfterGlueStickerTutorial()
    {
        // add talkie bg
        talkieBG.GetComponent<Image>().raycastTarget = true;
        talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0.9f, 0.5f);

        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // remove talkie bg
        talkieBG.GetComponent<Image>().raycastTarget = false;
        talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0f, 0.5f);

        // show sticker board back button + wiggle
        stickerboardBackButton.GetComponent<BackButton>().interactable = true;
        stickerboardBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        stickerboardBackButton.GetComponent<WiggleController>().StartWiggle();
    }

    public void AddAllGluedStickersToCurrentBoard()
    {
        // remove all stickers
        RemoveAllStickers();
       
        // get sticker board data
        StickerBoardData data = StudentInfoSystem.GetStickerBoardData(GetCurrentBoard());

        foreach (var sticker in data.stickers)
        {
            GameObject gluedSticker = Instantiate(gluedStickerObject, gluedStickerParent);
            gluedSticker.transform.localScale = Vector3.zero;
            gluedSticker.GetComponent<GluedSticker>().SetStickerData(sticker);
        }      
    }

    public void RemoveAllStickers()
    {
        foreach (Transform child in gluedStickerParent)
        {
            if (child.tag == "GluedSticker")
            {   
                child.GetComponent<GluedSticker>().DeleteSticker();
            }   
        }
    }

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
        stickerBoard.GetComponent<WiggleController>().StopWiggle();

        // set delete sticker mode to OFF
        SetDeleteStickerModeOFF();

        // set buttons to be not interactable 
        lesterAnimator.GetComponent<LesterButton>().interactable = false;
        stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        boardBook.GetComponent<BoardBookButton>().interactable = false;

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("stickerboard_blocker");

        // set stickerboards active
        numActiveBoards = 1;
        activeStickerboards.Clear();
        activeStickerboards.Add(stickerboards[0]);
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        foreach(var board in stickerboards)
        {
            bool isActive = false;
            if (board.boardType == StickerBoardType.Mossy)
            {
                isActive = data.mossyStickerBoard.active;
                board.gameObject.SetActive(isActive);
                activeStickerboards.Add(stickerboards[1]);
            }
            else if (board.boardType == StickerBoardType.Emerald)
            {
                isActive = data.emeraldStickerBoard.active;
                board.gameObject.SetActive(isActive);
                activeStickerboards.Add(stickerboards[2]);
            }
            else if (board.boardType == StickerBoardType.Beach)
            {
                isActive = data.beachStickerBoard.active;
                board.gameObject.SetActive(isActive);
                activeStickerboards.Add(stickerboards[3]);
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

        // play lester tutorial part 2
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // add talkie bg
            talkieBG.GetComponent<Image>().raycastTarget = true;
            talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0.9f, 0.5f);

            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterIntro_1_p2"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // remove talkie bg
            talkieBG.GetComponent<Image>().raycastTarget = false;
            talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0f, 0.5f);
        }
        else
        {
            // show sticker board back button
            stickerboardBackButton.GetComponent<BackButton>().interactable = true;
            stickerboardBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

            // show left and right buttons
            stickerboardLeftButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            stickerboardRightButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            stickerboardLeftButton.GetComponent<Button>().interactable = true;
            stickerboardRightButton.GetComponent<Button>().interactable = true;

            // show delete sticker button
            deleteStickerButton.GetComponent<Button>().interactable = true;
            deleteStickerButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
        }

        // show inventory button
        StickerInventory.instance.SetInventoryState(InventoryState.ShowTab);
        // update inventory
        StickerInventory.instance.UpdateStickerInventory();
        // open inventory if tutorial
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            StickerInventory.instance.SetInventoryState(InventoryState.Open);
        }

        // add stickers to board
        AddAllGluedStickersToCurrentBoard();

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
        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();

        // hide sticker board back button
        stickerboardBackButton.GetComponent<BackButton>().interactable = false;
        stickerboardBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        stickerboardBackButton.GetComponent<WiggleController>().StopWiggle();

        // hide delete sticker button
        deleteStickerButton.GetComponent<Button>().interactable = false;
        deleteStickerButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        
        // hide left and right buttons
        stickerboardLeftButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        stickerboardRightButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        stickerboardLeftButton.GetComponent<Button>().interactable = false;
        stickerboardRightButton.GetComponent<Button>().interactable = false;
        
        // hide inventroy button
        StickerInventory.instance.SetInventoryState(InventoryState.Hidden);

        // return selected sticker to inventory
        if (readyToGlueSticker)
        {
            ReturnStickerToInventory();
            yield return new WaitForSeconds(0.5f);
        }

        // remove stickers on board
        RemoveAllStickers();
        yield return new WaitForSeconds(0.2f);

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

        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // only board book interactable
            boardBook.GetComponent<BoardBookButton>().interactable = true;
            boardBook.GetComponent<WiggleController>().StartWiggle();
        }
        else
        {
            // set buttons to be not interactable 
            lesterAnimator.GetComponent<LesterButton>().interactable = true;
            stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
            boardBook.GetComponent<BoardBookButton>().interactable = true;
        }
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

        // return unglued sticker to inventory
        if (readyToGlueSticker)
        {
            ReturnStickerToInventory();
            yield return new WaitForSeconds(0.1f);
        }

        // remove all stickers from prev board
        RemoveAllStickers();
        yield return new WaitForSeconds(0.1f);

        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount + 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        // add stickers to board
        AddAllGluedStickersToCurrentBoard();

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

        // return unglued sticker to inventory
        if (readyToGlueSticker)
        {
            ReturnStickerToInventory();
            yield return new WaitForSeconds(0.1f);
        }

        // remove all stickers from prev board
        RemoveAllStickers();
        yield return new WaitForSeconds(0.1f);

        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount - 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        // add stickers to board
        AddAllGluedStickersToCurrentBoard();

        stickerboardButtonsDeactivated = false;
    }

    private IEnumerator BumpAnimation(bool isLeft)
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);

        if (isLeft)
        {
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
            gluedStickerParent.GetComponent<LerpableObject>().LerpXPos(gluedStickerParent.transform.localPosition.x + bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
            gluedStickerParent.GetComponent<LerpableObject>().LerpXPos(gluedStickerParent.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x - bumpAmount, 0.1f, true);
            gluedStickerParent.GetComponent<LerpableObject>().LerpXPos(gluedStickerParent.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerboardLayoutGroup.LerpXPos(stickerboardLayoutGroup.transform.localPosition.x + bumpAmount, 0.1f, true);
            gluedStickerParent.GetComponent<LerpableObject>().LerpXPos(gluedStickerParent.transform.localPosition.x + bumpAmount, 0.1f, true);
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
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // check if there is room in inventory
        if (StudentInfoSystem.GetTotalStickerCount() >= GameManager.stickerInventorySize)
        {
            StartCoroutine(InventoryFullRoutine());
            return;
        }

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

        lesterButton.GetComponent<WiggleController>().StopWiggle();

        StartCoroutine(RollForNewStickerRoutine());
    }

    private IEnumerator InventoryFullRoutine()
    {
        // play inventory full talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterFull_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // close window
        stickerConfirmWindow.CloseWindow();
    }

    public IEnumerator RollForNewStickerRoutine()
    {
        stickerConfirmWindow.CloseWindowForStickerRoll();
    
        // play lester buy talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterBuy_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // remove BG
        StickerConfirmWindow.instance.BG.LerpImageAlpha(StickerConfirmWindow.instance.BG.GetComponent<Image>(), 0f, 0.5f);
        StickerConfirmWindow.instance.BG.GetComponent<Image>().raycastTarget = false;

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // roll for a random sticker + set reveal sticker
        Sticker sticker = StickerDatabase.instance.RollForSticker();
        revealSticker.sprite = sticker.sprite;

        GameManager.instance.SendLog(this, "you got a sticker! " + sticker.rarity + " " + sticker.id);

        // save sticker to SIS
        StudentInfoSystem.AddStickerToInventory(sticker, false);

        // fade to black
        FadeObject.instance.FadeOut(1f);
        yield return new WaitForSeconds(1f);

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

        yield return new WaitForSeconds(0.2f);
        
        // Fade back in 
        FadeObject.instance.FadeIn(1f);

        yield return new WaitForSeconds((float)currentVideo.length - 1.5f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");

        // reveal sticker here after certain amount of time
        revealSticker.transform.localPosition = Vector3.zero;
        revealSticker.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        // get appropriate sticker voiceover
        AssetReference stickerVoiceover = null;
        switch (sticker.rarity)
        {
            default:
            case StickerRarity.Common:
                stickerVoiceover = AudioDatabase.instance.commonStickerVoiceovers[Random.Range(0, 3)];
                break;

            case StickerRarity.Uncommon:
                stickerVoiceover = AudioDatabase.instance.uncommonStickerVoiceovers[Random.Range(0, 3)];
                break;

            case StickerRarity.Rare:
                stickerVoiceover = AudioDatabase.instance.rareStickerVoiceovers[Random.Range(0, 3)];
                break;

            case StickerRarity.Legendary:
                stickerVoiceover = AudioDatabase.instance.legendaryStickerVoiceovers[Random.Range(0, 3)];
                break;
        }
        // play voiceover
        lesterSpeechBubble.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-1f, 1f), 0.1f, 0.1f);
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(stickerVoiceover));
        yield return cd.coroutine;
        AudioManager.instance.PlayTalk(stickerVoiceover);
        yield return new WaitForSeconds(cd.GetResult() + 0.2f);
        lesterSpeechBubble.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        // wiggle reveal sticker
        revealSticker.GetComponent<WiggleController>().StartWiggle();

        // wait for player input to continue
        waitingOnPlayerInput = true;
        while (waitingOnPlayerInput)
            yield return null;

        // stop wiggle reveal sticker
        revealSticker.GetComponent<WiggleController>().StopWiggle();
        revealSticker.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // fade to black
        FadeObject.instance.FadeOut(0.5f);
        yield return new WaitForSeconds(0.5f);

        // stop video player
        currentVideo.Stop();

        // Fade back in 
        FadeObject.instance.FadeIn(0.5f);
        yield return new WaitForSeconds(0.25f);

        // move sticker into dropdown toolbar
        StartCoroutine(AwardStickerAnimation());
        yield return new WaitForSeconds(0.5f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");

        // show lester
        lesterAnimator.Play("geckoIntro");
        lesterAnimator.GetComponent<LesterButton>().isHidden = false;
        lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(0.5f);

        // only make stickerboard interactable if tutorial
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            stickerBoard.GetComponent<WiggleController>().StartWiggle();
            stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        }
        else
        {
            // set buttons to be interactable 
            lesterAnimator.GetComponent<LesterButton>().interactable = true;
            stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
            boardBook.GetComponent<BoardBookButton>().interactable = true;

            // show wagon back button
            wagonBackButton.GetComponent<BackButton>().interactable = true;
            wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
    }

    private IEnumerator AwardStickerAnimation()
    {
        revealSticker.GetComponent<LerpableObject>().LerpPosToTransform(stickerToolbarPos, 0.5f, false);
        revealSticker.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.5f);
        yield return new WaitForSeconds(0.5f);

        // play audio
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.ScrollRoll, 0.5f);
        DropdownToolbar.instance.silverCoinTransform.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        DropdownToolbar.instance.UpdateSilverCoins();
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

        // check for scroll map story beat if finishing tutorial
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // done with sticker tutorial
            StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();
            StickerSystem.instance.ToggleStickerButtonWiggleGlow(false);

            ScrollMapManager.instance.CheckForScrollMapGameEvent(StudentInfoSystem.GetCurrentProfile().currStoryBeat);
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
        // disable map icon colliders
        ScrollMapManager.instance.ToggleCurrentMapIconColliders(false);

        // add raycast
        RaycastBlockerController.instance.CreateRaycastBlocker("show_wagon_raycast");

        // stop music
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlaySong(AudioDatabase.instance.WagonWindowSong);

        // reset lester timers
        lesterButton.GetComponent<LesterButton>().ResetLesterTimers();

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

        // play lester tutorial 1
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // add talkie bg
            talkieBG.GetComponent<Image>().raycastTarget = true;
            talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0.9f, 0.5f);

            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterIntro_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // remove talkie bg
            talkieBG.GetComponent<Image>().raycastTarget = false;
            talkieBG.LerpImageAlpha(talkieBG.GetComponent<Image>(), 0f, 0.5f);
        }
        
        lesterAnimator.Play("geckoIntro");

        // show back button + dropdown toolbar (dont show back button iff tutorial)
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // do nothing :)
        }
        else
        {   
            wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
        DropdownToolbar.instance.ToggleToolbar(true);

        yield return new WaitForSeconds(0.5f);

        // set buttons interactable
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            lesterButton.GetComponent<LesterButton>().interactable = true;
            lesterButton.GetComponent<WiggleController>().StartWiggle();
        }
        else
        {
            stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
            boardBook.GetComponent<BoardBookButton>().interactable = true;
            lesterButton.GetComponent<LesterButton>().interactable = true;
            wagonBackButton.GetComponent<BackButton>().interactable = true;
        }

        // animation done - remove raycast
        RaycastBlockerController.instance.RemoveRaycastBlocker("show_wagon_raycast");
        wagonAnimating = false;
    }

    private IEnumerator HideWagon()
    {
        // enable map icon colliders
        ScrollMapManager.instance.ToggleCurrentMapIconColliders(true);

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

        // play scroll map song
        AudioManager.instance.StopMusic();
        AudioManager.instance.PlaySong(AudioDatabase.instance.ScrollMapSong);

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
