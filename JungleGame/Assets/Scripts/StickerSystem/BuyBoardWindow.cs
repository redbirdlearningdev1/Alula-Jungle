using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyBoardWindow : MonoBehaviour
{
    public static BuyBoardWindow instance;

    public List<BoardPreview> boardPreviews;

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

        

        buyButton.interactable = false;
        coinImage.gameObject.SetActive(false);
        buyText.text = "";
        priceText.text = "";

        UnselectAllBoards();
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
        buyButton.interactable = true;
        coinImage.gameObject.SetActive(true);
        buyText.text = "buy for";
        priceText.text = "x" + board.price;    
    }
}
