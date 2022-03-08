using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardBookWindow : MonoBehaviour
{
    public static BoardBookWindow instance;

    public LerpableObject BG;
    public LerpableObject window;
    public LerpableObject lester;
    public Transform lesterHiddenPos;
    public Transform lesterShownPos;

    public bool windowActive;

    void Awake()
    {
        if (instance == null)
            instance = this;

        BG.GetComponent<Image>().raycastTarget = false;

        window.transform.localScale = new Vector3(0f, 0f, 1f);
        lester.transform.position = lesterHiddenPos.position;
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

        // hide back button
        StickerSystem.instance.backButton.GetComponent<BackButton>().interactable = false;
        StickerSystem.instance.backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        // set buttons to be not interactable
        StickerSystem.instance.lesterAnimator.GetComponent<LesterButton>().interactable = false;
        StickerSystem.instance.stickerBoard.GetComponent<StickerBoardButton>().interactable = false;
        StickerSystem.instance.boardBook.GetComponent<BoardBookButton>().interactable = false;

        yield return new WaitForSeconds(0.5f);

        // show BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0.95f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = true;

        // show lester
        Vector3 bouncePos = lesterShownPos.position;
        bouncePos.y += 0.1f;
        lester.LerpPosition(bouncePos, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        lester.LerpPosition(lesterShownPos.position, 0.2f, false);
        // show window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
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
        StickerSystem.instance.backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        StickerSystem.instance.backButton.GetComponent<BackButton>().interactable = true;
    }

    public void OnYesPressed()
    {
        CloseWindow();
    }
}
