using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoardController : MonoBehaviour
{
    public static StickerBoardController instance;

    public LerpableObject stickerBoards;
    public LerpableObject buttons;
    public GameObject hoverAreas;

    private List<StickerBoard> activeStickerBoards;
    private int numBoards;
    private int currentBoardIndex = 0;
    private bool buttonsDeactivated = false;

    public StickerBoard classicBoard;
    public StickerBoard mossyBoard;
    public StickerBoard emeraldBoard;
    public StickerBoard beachBoard;

    public float bumpAmount;

    [HideInInspector] public bool stickerBoardActive = false;
    private bool stickerBoardReady = true;
    
    [Header("Bounce Positions + Timings")]
    public float topOffScreenPos;
    public float bottomOffScreenPos;
    public float onScreenPos;
    public float inBounce1Pos;
    public float inBounce2Pos;
    public float outBouncePos;

    public float time1, time2, time3;


    void Awake()
    {
        if (instance == null)
            instance = this;

        StartCoroutine(DelayedStart(0.001f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        hoverAreas.SetActive(false);

        // deactivate buttons
        buttons.gameObject.SetActive(true);
        buttonsDeactivated = true;
        buttons.transform.localScale = new Vector3(2f, 2f, 1f);
        buttons.gameObject.SetActive(false);

        UpdateBoards();
    }

    public void UpdateBoards()
    {
        stickerBoards.gameObject.SetActive(true);

        activeStickerBoards = new List<StickerBoard>();
        activeStickerBoards.Add(classicBoard);

        numBoards = 4; // currently 4 boards in game

        // deactivate locked boards
        if (!StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active)
        {
            mossyBoard.gameObject.SetActive(false);
            numBoards--;
        }
        else
        {
            activeStickerBoards.Add(mossyBoard);
            mossyBoard.gameObject.SetActive(true);
        }

        if (!StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active)
        {
            emeraldBoard.gameObject.SetActive(false);
            numBoards--;
        }
        else
        {
            activeStickerBoards.Add(emeraldBoard);
            emeraldBoard.gameObject.SetActive(true);
        }

        if (!StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active)
        {
            beachBoard.gameObject.SetActive(false);
            numBoards--;
        }
        else
        {
            activeStickerBoards.Add(beachBoard);
            beachBoard.gameObject.SetActive(true);
        }
        
        stickerBoards.gameObject.SetActive(false);
    }

    public void ToggleStickerBoardWindow()
    {
        if (!stickerBoardReady)
            return;

        stickerBoardActive = !stickerBoardActive;
        stickerBoardReady = false;

        // open window
        if (stickerBoardActive)
        {
            StartCoroutine(OpenStickerBoardWindowRoutine());
            Gecko.instance.isOn = false;
            StickerBoardBook.instance.isOn = false;
        }
        // close window
        else
        {
            StartCoroutine(CloseStickerBoardWindowRoutine());
            Gecko.instance.isOn = true;
            StickerBoardBook.instance.isOn = true;
        }
    }

    private IEnumerator OpenStickerBoardWindowRoutine()
    {
        ResetStickerBoards();
        UpdateBoards();
        stickerBoards.gameObject.SetActive(true);
        StickerInventoryButton.instance.UpdateButtonText();
    
        // animate board intro
        stickerBoards.LerpYPos(inBounce1Pos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoards.LerpYPos(inBounce2Pos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoards.LerpYPos(onScreenPos, time3, true);
        yield return new WaitForSeconds(time3);

        // add stickers to classic board
        classicBoard.AddAllStickersToBoard();

        // play lester tutorial
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // stop sticker board from wiggling
            Board.instance.GetComponent<WiggleController>().StopWiggle();
            Board.instance.isOn = false;

            // play lester intro 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.LesterIntro_1_p2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
            
            // wiggle inventory button
            StickerInventoryButton.instance.GetComponent<WiggleController>().StartWiggle();
            ImageGlowController.instance.SetImageGlow(StickerInventoryButton.instance.GetComponent<Image>(), true, GlowValue.glow_1_00);
        }

        // activate buttons
        buttons.gameObject.SetActive(true);
        buttons.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.05f);
        buttonsDeactivated = false;

        stickerBoardReady = true;
    }

    private IEnumerator CloseStickerBoardWindowRoutine()
    {
        // deactivate buttons
        buttonsDeactivated = true;
        buttons.LerpScale(new Vector2(2f, 2f), 0.5f);

        // return any active stickers to inventory
        foreach (var item in activeStickerBoards)
        {
            item.ReturnCurrentStickerToInventory();
        }
        yield return new WaitForSeconds(0.25f);
        // close inventory if open
        activeStickerBoards[currentBoardIndex].ToggleStickerInventory(false);

        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // disable gecko
            WagonWindowController.instance.gecko.SetActive(false);
        }

        stickerBoards.LerpYPos(outBouncePos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoards.LerpYPos(bottomOffScreenPos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoardReady = true;

        ResetStickerBoards();
        StickerInventoryButton.instance.UpdateButtonText();
        stickerBoards.gameObject.SetActive(false);

        buttons.transform.localScale = new Vector3(2f, 2f, 1f);
        buttons.gameObject.SetActive(false);

        // play lester tutorial
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // disable back button
            WagonWindowController.instance.backButton.interactable = false;
            WagonWindowController.instance.backButton.GetComponent<LerpableObject>().LerpImageAlpha(WagonWindowController.instance.backButton.GetComponent<Image>(), 0f, 0.1f);
            WagonWindowController.instance.backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.01f);
            yield return new WaitForSeconds(0.5f);
            WagonWindowController.instance.backButton.gameObject.SetActive(false);

            // enable wagon background
            WagonWindowController.instance.wagonBackground.LerpImageAlpha(WagonWindowController.instance.wagonBackground.GetComponent<Image>(), 0.75f, 0.1f);

            // play lester intro 3
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.LesterIntro_1_p3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // diable wagon background
            WagonWindowController.instance.wagonBackground.LerpImageAlpha(WagonWindowController.instance.wagonBackground.GetComponent<Image>(), 0f, 0.1f);

            // wiggle sticker board
            StickerBoardBook.instance.isOn = true;
            StickerBoardBook.instance.GetComponent<WiggleController>().StartWiggle();
        }   
    }

    private void ResetStickerBoards()
    {
        // place in start pos
        stickerBoards.transform.localPosition = new Vector3(0f, topOffScreenPos, 1f);

        // reset current index
        currentBoardIndex = 0;

        // remove all stickers
        foreach (var item in activeStickerBoards)
        {
            item.ResetBools();
            item.ClearBoard();
        }
    }

    public StickerBoard GetCurrentStickerBoard()
    {
        return activeStickerBoards[currentBoardIndex];
    }

    public void OnLeftButtonPressed()
    {
        if (buttonsDeactivated)
            return;

        buttonsDeactivated = true;

        // save prev board index
        int prevBoardIndex = currentBoardIndex;
        currentBoardIndex--;

        if (currentBoardIndex < 0)
        {
            currentBoardIndex = 0;
            // bump board left
            StartCoroutine(BumpAnimation(true));
            return;
        }

        // toggle off inventory
        activeStickerBoards[prevBoardIndex].ToggleStickerInventory(false);
        // remove stickers from prev board
        activeStickerBoards[prevBoardIndex].RemoveAllStickersFromBoard();

        // move board left
        StartCoroutine(MoveToLeftBoard());
    }

    public void OnRightButtonPressed()
    {
        if (buttonsDeactivated)
            return;

        buttonsDeactivated = true;

        // save prev board index
        int prevBoardIndex = currentBoardIndex;
        currentBoardIndex++;

        if (currentBoardIndex >= numBoards)
        {
            currentBoardIndex = numBoards - 1;
            // bump board right
            StartCoroutine(BumpAnimation(false));
            return;
        }

        // toggle off inventory
        activeStickerBoards[prevBoardIndex].ToggleStickerInventory(false);
        // remove stickers from prev board
        activeStickerBoards[prevBoardIndex].RemoveAllStickersFromBoard();

        // move board right
        StartCoroutine(MoveToRightBoard());
    }

    private IEnumerator MoveToLeftBoard()
    {
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount + 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        // add stickers to new board
        activeStickerBoards[currentBoardIndex].AddAllStickersToBoard();

        buttonsDeactivated = false;
    }

    private IEnumerator MoveToRightBoard()
    {
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount - 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        // add stickers to new board
        activeStickerBoards[currentBoardIndex].AddAllStickersToBoard();

        buttonsDeactivated = false;
    }

    private IEnumerator BumpAnimation(bool isLeft)
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);

        if (isLeft)
        {
            stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
            stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount, 0.1f, true);
            yield return new WaitForSeconds(0.1f);
        }

        buttonsDeactivated = false;
    }

    public void ToggleInventoryWindow()
    {
        activeStickerBoards[currentBoardIndex].OnStickerInventoryPressed();
    }
}
