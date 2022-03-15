using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardBookWindow : MonoBehaviour
{
    public static BoardBookWindow instance;

    public LerpableObject BG;
    public LerpableObject window;    
    public LerpableObject lester;
    public Transform lesterHiddenPos;
    public Transform lesterShownPos;
    public bool windowActive;

    public Button yesBuyButton;
    private StickerBoardType selectedBoard;

    public TextMeshProUGUI priceText;
    public LerpableObject coinImage;

    [Header("Scroll Items")]
    public ScrollRect boardPreviewScrollRect;
    public Scrollbar boardPreviewScrollbar;
    public List<StickerBoardPreview> boardPreviewElements;

    void Awake()
    {
        if (instance == null)
            instance = this;

        BG.GetComponent<Image>().raycastTarget = false;

        window.transform.localScale = new Vector3(0f, 0f, 1f);
        lester.transform.position = lesterHiddenPos.position;
        yesBuyButton.interactable = false;
        windowActive = false;

        priceText.text = "";
        coinImage.transform.localScale = Vector3.zero;
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
        // update boards
        UpdateStickerBoardPreviews();
        yesBuyButton.interactable = false;
        foreach (var board in boardPreviewElements)
        {
            board.interactable = true;
            board.SetBoardSelected(false);
        }

        // reset price UI
        priceText.text = "";
        coinImage.transform.localScale = Vector3.zero;

        // hide wagon lester
        StickerSystem.instance.lesterAnimator.Play("geckoLeave");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = true;

        // hide back button
        StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = false;
        StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        // set buttons to be not interactable
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = false;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = false;

        yield return new WaitForSeconds(0.5f);

        // show BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0.95f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = true;

        // show window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);

        // center on first element
        CenterOnBoardPreview(0, true);
        yield return new WaitForSeconds(0.1f);

        // show lester
        Vector3 bouncePos = lesterShownPos.position;
        bouncePos.y += 0.1f;
        lester.LerpPosition(bouncePos, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        lester.LerpPosition(lesterShownPos.position, 0.2f, false);

        windowActive = true;
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
        // hide lester
        Vector3 bouncePos = lesterShownPos.position;
        bouncePos.y += 0.1f;
        lester.LerpPosition(bouncePos, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        lester.LerpPosition(lesterHiddenPos.position, 0.2f, false);
        // show window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        windowActive = false;

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = false;

        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // add talkie bg
            StickerSystem.instance.talkieBG.GetComponent<Image>().raycastTarget = true;
             StickerSystem.instance.talkieBG.LerpImageAlpha(StickerSystem.instance.talkieBG.GetComponent<Image>(), 0.9f, 0.5f);

            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("LesterIntro_1_p4"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // remove talkie bg
            StickerSystem.instance.talkieBG.GetComponent<Image>().raycastTarget = false;
            StickerSystem.instance.talkieBG.LerpImageAlpha( StickerSystem.instance.talkieBG.GetComponent<Image>(), 0f, 0.5f);

            // done with sticker tutorial
            StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();
            StickerSystem.instance.ToggleStickerButtonWiggleGlow(false);
        }

        // show wagon lester
        StickerSystem.instance.lesterAnimator.Play("geckoIntro");
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().isHidden = false;
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().ResetLesterTimers();

        yield return new WaitForSeconds(0.5f);

        // set buttons to be interactable 
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = true;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = true;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = true;

        // show back button + dropdown toolbar
        StickerSystem.instance.wagonBackButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        StickerSystem.instance.wagonBackButton.GetComponent<BackButton>().interactable = true;
    }

    public void CenterOnBoardPreview(int boardIndex, bool fast = false)
    {
        StartCoroutine(CenterOnBoardPreviewRoutine(boardIndex, fast));
    }

    private IEnumerator CenterOnBoardPreviewRoutine(int boardIndex, bool fast)
    {
        // set board and elements to be not interactable
        boardPreviewScrollbar.interactable = false;
        foreach (var board in boardPreviewElements)
        {
            board.interactable = false;
            // select this board
            if (boardIndex == board.boardPreviewIndex)
            {
                board.SetBoardSelected(true);
                // update UI and stuff
                SelectedBoard(board.boardType);
            }
            else
            {
                board.SetBoardSelected(false);
            }
        }

        // lerp scroll pos to be centered on selected board elements
        float timer = 0f;
        float time = 0.5f;
        float startScrollPos = boardPreviewScrollbar.value;
        float endScrollPos = (float)boardIndex / ((float)boardPreviewElements.Count - 1f);

        // change time to be faster
        if (fast)
        {
            time = 0.1f;
        }

        while (timer < time)
        {
            timer += Time.deltaTime;
            boardPreviewScrollbar.value = Mathf.Lerp(startScrollPos, endScrollPos, timer / 0.5f);
            yield return null;
        }
        boardPreviewScrollbar.value = endScrollPos;

        // set board and elements to be interactable
        boardPreviewScrollbar.interactable = true;
        foreach (var board in boardPreviewElements)
        {
            board.interactable = true;
        }
    }

    private int GetBoardPrice(StickerBoardType boardType)
    {
        switch (boardType)
        {
            default:
            case StickerBoardType.Classic:
                return 0;

            case StickerBoardType.Mossy:
                return 10;
            
            case StickerBoardType.Beach:
                return 20;

            case StickerBoardType.Emerald:
                return 30;
        }
    }

    public void OnYesPressed()
    {
        int price = GetBoardPrice(selectedBoard);

        // check to make sure player has sufficent funds
        if (StudentInfoSystem.GetCurrentProfile().goldCoins < price)
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
            DropdownToolbar.instance.RemoveGoldCoins(price);
        }

        // unlock specific board
        switch (selectedBoard)
        {
            case StickerBoardType.Classic:
                StudentInfoSystem.GetCurrentProfile().classicStickerBoard.active = true;
                StudentInfoSystem.SaveStudentPlayerData();
                break;
            case StickerBoardType.Mossy:
                StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active = true;
                StudentInfoSystem.SaveStudentPlayerData();
                break;
            case StickerBoardType.Beach:
                StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active = true;
                StudentInfoSystem.SaveStudentPlayerData();
                break;
            case StickerBoardType.Emerald:
                StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active = true;
                StudentInfoSystem.SaveStudentPlayerData();
                break;
        }

        // update UI
        yesBuyButton.interactable = false;
        // reset price UI
        priceText.text = "";
        if (coinImage.transform.localScale.x != 0)
        {
            coinImage.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
        UpdateStickerBoardPreviews();
    }

    private void SelectedBoard(StickerBoardType boardType)
    {
        selectedBoard = boardType;

        // check if board is already bought
        if (BoardBoughtAlready(selectedBoard))
        {
            yesBuyButton.interactable = false;

            // reset price UI
            priceText.text = "";
            if (coinImage.transform.localScale.x != 0)
            {
                coinImage.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
            }
        }
        else
        {
            yesBuyButton.interactable = true;

            // set price UI
            priceText.text = "x" + GetBoardPrice(selectedBoard);
            coinImage.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
    }

    private bool BoardBoughtAlready(StickerBoardType boardType)
    {
        switch (boardType)
        {
            default:
            case StickerBoardType.Classic:
                return StudentInfoSystem.GetCurrentProfile().classicStickerBoard.active;

            case StickerBoardType.Mossy:
                return StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active;
            
            case StickerBoardType.Beach:
                return StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active;

            case StickerBoardType.Emerald:
                return StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active;
        }
    }

    public void UpdateStickerBoardPreviews()
    {
        foreach (var board in boardPreviewElements)
        {
            switch (board.boardType)
            {
                case StickerBoardType.Classic:
                    board.SetBoardSoldOut(StudentInfoSystem.GetCurrentProfile().classicStickerBoard.active);
                    break;

                case StickerBoardType.Mossy:
                    board.SetBoardSoldOut(StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active);
                    break;
                
                case StickerBoardType.Beach:
                    board.SetBoardSoldOut(StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active);
                    break;

                case StickerBoardType.Emerald:
                    board.SetBoardSoldOut(StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active);
                    break;
            }
        }
    }
}
