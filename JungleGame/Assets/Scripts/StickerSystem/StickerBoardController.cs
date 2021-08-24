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
            StartCoroutine(OpenBuyBoardWindowRoutine());
            Gecko.instance.isOn = false;
            StickerBoardBook.instance.isOn = false;
        }
        // close window
        else
        {
            StartCoroutine(CloseBuyBoardWindowRoutine());
            Gecko.instance.isOn = true;
            StickerBoardBook.instance.isOn = true;
        }
    }

    private IEnumerator OpenBuyBoardWindowRoutine()
    {
        stickerBoardWindow.gameObject.SetActive(true);
        ResetStickerBoard();

        stickerBoardWindow.LerpPosition(inBounce1Pos, time1, true);
        yield return new WaitForSeconds(time1);

        stickerBoardWindow.LerpPosition(inBounce2Pos, time2, true);
        yield return new WaitForSeconds(time2);

        stickerBoardWindow.LerpPosition(onScreenPos, time3, true);
        yield return new WaitForSeconds(time3);

        stickerBoardReady = true;
    }

    private IEnumerator CloseBuyBoardWindowRoutine()
    {
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
        stickerBoardWindow.transform.localPosition = topOffScreenPos;
    }
}
