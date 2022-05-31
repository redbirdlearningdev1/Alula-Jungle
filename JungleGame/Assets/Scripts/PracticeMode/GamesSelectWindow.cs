using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamesSelectWindow : MonoBehaviour
{
    public static GamesSelectWindow instance;

    public LerpableObject myWindow;
    public LerpableObject confirmButton;

    public LerpableObject windowBG;
    public Image windowBGImage;

    public TMP_InputField numText;
    public LerpableObject leftArrow;
    public LerpableObject rightArrow;
    public LerpableObject numBox;

    private int myNum;
    private ReturnLocation myReturnLocation;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // start window closed
        transform.localScale = Vector3.zero;

        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.SetImageAlpha(windowBGImage, 0f);
    }

    //////////////////////////////////////////

    public void OnInputFieldValueChanged()
    {
        // remove any non-numeric characters
        string numString = "";
        foreach (char c in numText.text)
        {
            if ("0123456789".Contains(c.ToString()))
            {
                numString += c;
            }
        }

        // make sure int is possible
        if (numString.Length > 0)
        {
            myNum = int.Parse(numString);
            if (myNum < 1)
            {
                myNum = 1;
            }
            if (myNum > 99)
            {
                myNum = 99;
            }
        }
    }

    public void OnLeftArrowPressed()
    {
        myNum--;
        if (myNum < 1)
        {
            myNum = 1;
        }

        leftArrow.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        numBox.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        numText.text = myNum.ToString();
    }

    public void OnRightArrowPressed()
    {
        myNum++;
        if (myNum > 99)
        {
            myNum = 99;
        }

        rightArrow.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        numBox.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        numText.text = myNum.ToString();
    }

    public void OpenWindow(int num, ReturnLocation returnLocation)
    {
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        myNum = num;
        numText.text = myNum.ToString();

        myReturnLocation = returnLocation;

        StartCoroutine(OpenWindowRoutine());
    }

    private IEnumerator OpenWindowRoutine()
    {
        myWindow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.4f);

        // show confirm button
        confirmButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    ////////////////////////////////////////////

    public void CloseWindow()
    {   
        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.LerpImageAlpha(windowBGImage, 0f, 0.5f);

        StartCoroutine(CloseWindowRoutine());
        
        // return phoneme list to correct location
        switch (myReturnLocation)
        {
            case ReturnLocation.defaultWindow:
                PracticeSceneManager.instance.defaultPracticeWindow.ReturnNumGames(myNum);
                break;

            case ReturnLocation.phonemeChangingWindow:
                PracticeSceneManager.instance.phonemeChangingWindow.ReturnNumGames(myNum);
                break;

            case ReturnLocation.phonemePracticeWindow:
                PracticeSceneManager.instance.phonemePracticeWindow.ReturnNumGames(myNum);
                break;
        }
    }

    private IEnumerator CloseWindowRoutine()
    {
        // hide confirm button
        confirmButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        myWindow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
        PracticeSceneManager.instance.RemoveWindowBG();
    }
}
