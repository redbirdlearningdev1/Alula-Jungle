using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyBoardWindow : MonoBehaviour
{
    public static BuyBoardWindow instance;

    public List<BoardPreview> boardPreviews;
    [HideInInspector] public BoardPreview currentBoard;

    public Button buyButton;
    public Image coinImage;
    public TextMeshProUGUI buyText;
    public TextMeshProUGUI priceText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        StartCoroutine(DelayedStart(0.001f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        buyButton.interactable = false;
        coinImage.gameObject.SetActive(false);
        buyText.text = "";
        priceText.text = "";

        UnselectAllBoards();
        UpdateBuyBoardPreviews();
    }

    public void UpdateBuyBoardPreviews()
    {
        foreach (var board in boardPreviews)
        {
            switch (board.boardType)
            {
                case StickerBoardType.Classic:
                    if (StudentInfoSystem.GetCurrentProfile().classicStickerBoard.active)
                        board.SetOutOfStock(true);
                    else
                        board.SetOutOfStock(false);
                    break;

                case StickerBoardType.Mossy:
                    if (StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active)
                        board.SetOutOfStock(true);
                    else
                        board.SetOutOfStock(false);
                    break;
                
                case StickerBoardType.Beach:
                    if (StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active)
                        board.SetOutOfStock(true);
                    else
                        board.SetOutOfStock(false);
                    break;

                case StickerBoardType.Emerald:
                    if (StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active)
                        board.SetOutOfStock(true);
                    else
                        board.SetOutOfStock(false);
                    break;
            }
        }
    }

    public void OnBuyButtonPressed()
    {
        int price = currentBoard.price;

        // check to make sure player has sufficent funds
        if (StudentInfoSystem.GetCurrentProfile().goldCoins < price)
        {
            // play sound
            AudioManager.instance.PlayCoinDrop();
            return;
        }
        // if they do, remove coins from player profile
        else 
        {
            DropdownToolbar.instance.RemoveGoldCoins(price);
        }

        // unlock specific board
        switch (currentBoard.boardType)
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
        UpdateBuyBoardPreviews();
        SetUI(true, currentBoard);
    }

    public void UnselectAllBoards()
    {
        foreach (var board in boardPreviews)
        {
            board.SetSelectBoard(false);
        }
    }

    public void SetAsChosenBoard(BoardPreview board)
    {
        switch (board.boardType)
        {
            case StickerBoardType.Classic:
                if (StudentInfoSystem.GetCurrentProfile().classicStickerBoard.active)
                    SetUI(true, board);
                else
                    SetUI(false, board);
                break;

            case StickerBoardType.Mossy:
                if (StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.active)
                    SetUI(true, board);
                else
                    SetUI(false, board);
                break;
            
            case StickerBoardType.Beach:
                if (StudentInfoSystem.GetCurrentProfile().beachStickerBoard.active)
                    SetUI(true, board);
                else
                    SetUI(false, board);
                break;

            case StickerBoardType.Emerald:
                if (StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.active)
                    SetUI(true, board);
                else
                    SetUI(false, board);
                break;
        }
    }

    private void SetUI(bool opt, BoardPreview board)
    {
        if (!opt)
        {
            buyButton.interactable = true;
            coinImage.gameObject.SetActive(true);
            buyText.text = "buy for";
            priceText.text = "x" + board.price;
        }
        else
        {   
            buyButton.interactable = false;
            coinImage.gameObject.SetActive(false);
            buyText.text = "";
            priceText.text = "";
        }
    }
}
