using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerBoardController : MonoBehaviour
{
    public static StickerBoardController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public LerpableObject stickerBoardWindow;

    public bool stickerBoardActive = false;
    private bool stickerBoardReady = true;
    
    [Header("Bounce Positions + Timings")]
    public Vector3 topOffScreenPos;
    public Vector3 bottomOffScreenPos;
    public Vector3 onScreenPos;
    public Vector3 inBounce1Pos;
    public Vector3 inBounce2Pos;
    public Vector3 outBouncePos;

    public float time1, time2, time3;

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
        stickerBoardWindow.gameObject.SetActive(true);
        ResetStickerBoard();

        stickerBoardWindow.LerpPosition(inBounce1Pos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoardWindow.LerpPosition(inBounce2Pos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoardWindow.LerpPosition(onScreenPos, time3, true);
        yield return new WaitForSeconds(time3);

        // place stickers on board
        var boardData = StudentInfoSystem.GetStickerBoardData(StickerBoardType.Classic);
        StickerBoard classicBoard = stickerBoardWindow.GetComponent<StickerBoard>();
        foreach (var sticker in boardData.stickers)
        {
            classicBoard.AddStickerOntoBoard(sticker);
        }

        stickerBoardReady = true;
    }

    private IEnumerator CloseStickerBoardWindowRoutine()
    {
        // return any active stickers to inventory
        StickerBoard.instance.ReturnCurrentStickerToInventory();
        yield return new WaitForSeconds(0.25f);

        stickerBoardWindow.LerpPosition(outBouncePos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoardWindow.LerpPosition(bottomOffScreenPos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoardReady = true;

        ResetStickerBoard();
        stickerBoardWindow.gameObject.SetActive(false);
    }

    private void ResetStickerBoard()
    {
        // place in start pos
        stickerBoardWindow.transform.localPosition = topOffScreenPos;

        // update inventory text
        StickerInventoryButton.instance.UpdateButtonText();

        // reset bools
        StickerBoard.instance.ResetBools();
    }
}
