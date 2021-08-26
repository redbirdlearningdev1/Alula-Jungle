using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerBoardController : MonoBehaviour
{
    public static StickerBoardController instance;

    public LerpableObject stickerBoards;
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

        // deactivate buttons
        buttonsDeactivated = true;

        if (StudentInfoSystem.currentStudentPlayer != null)
        {
            // deactivate locked boards
            if (!StudentInfoSystem.currentStudentPlayer.mossyStickerBoard.active)
            {
                mossyBoard.gameObject.SetActive(false);
                numBoards--;
            }
            else if (!StudentInfoSystem.currentStudentPlayer.emeraldStickerBoard.active)
            {
                emeraldBoard.gameObject.SetActive(false);
                numBoards--;
            }
            else if (StudentInfoSystem.currentStudentPlayer.beachStickerBoard.active)
            {
                beachBoard.gameObject.SetActive(false);
                numBoards--;
            }
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
        stickerBoards.gameObject.SetActive(true);
        ResetStickerBoard();

        // get sticker and board data
        var boardData = StudentInfoSystem.GetStickerBoardData(StickerBoardType.Classic);

        // remove all stickers
        classicBoard.ClearBoard();
        mossyBoard.ClearBoard();
        emeraldBoard.ClearBoard();
        beachBoard.ClearBoard();
    
        // animate board intro
        stickerBoards.LerpYPos(inBounce1Pos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoards.LerpYPos(inBounce2Pos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoards.LerpYPos(onScreenPos, time3, true);
        yield return new WaitForSeconds(time3);
        
        // place stickers on board
        foreach (var sticker in boardData.stickers)
        {
            classicBoard.AddStickerOntoBoard(sticker);
            yield return new WaitForSeconds(0.01f);
        }

        // activate buttons
        buttonsDeactivated = false;

        stickerBoardReady = true;
    }

    private IEnumerator CloseStickerBoardWindowRoutine()
    {
        // deactivate buttons
        buttonsDeactivated = true;

        // return any active stickers to inventory
        StickerBoard.instance.ReturnCurrentStickerToInventory();
        yield return new WaitForSeconds(0.25f);

        stickerBoards.LerpYPos(outBouncePos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoards.LerpYPos(bottomOffScreenPos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoardReady = true;

        ResetStickerBoard();
        stickerBoards.gameObject.SetActive(false);
    }

    private void ResetStickerBoard()
    {
        // place in start pos
        stickerBoards.transform.localPosition = new Vector3(stickerBoards.transform.localPosition.x, topOffScreenPos, 1f);

        // update inventory text
        StickerInventoryButton.instance.UpdateButtonText();

        // reset bools
        StickerBoard.instance.ResetBools();
    }



    public void OnLeftButtonPressed()
    {
        if (buttonsDeactivated)
            return;

        buttonsDeactivated = true;

        currentBoardIndex--;
        if (currentBoardIndex <= 0)
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
        if (buttonsDeactivated)
            return;

        buttonsDeactivated = true;

        currentBoardIndex++;
        if (currentBoardIndex >= numBoards)
        {
            currentBoardIndex = numBoards - 1;
            // bump board right
            StartCoroutine(BumpAnimation(false));
            return;
        }
        // move board right
        StartCoroutine(MoveToRightBoard());
    }

    private IEnumerator MoveToLeftBoard()
    {
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount - 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        buttonsDeactivated = false;
    }

    private IEnumerator MoveToRightBoard()
    {
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x - bumpAmount, 0.1f, true);
        yield return new WaitForSeconds(0.1f);
        stickerBoards.LerpXPos(stickerBoards.transform.localPosition.x + bumpAmount + 1800f, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

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
}
