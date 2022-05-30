using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ReturnLocation
{
    blendingWindow
}

public class PhonemeSelectWindow : MonoBehaviour
{
    public static PhonemeSelectWindow instance;

    public LerpableObject myWindow;
    public LerpableObject confirmButton;

    public LerpableObject windowBG;
    public Image windowBGImage;

    private List<ActionWordEnum> currentPhonemes;
    public List<UniversalCoinImage> myCoins;

    private ReturnLocation myReturnLocation;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // start window closed
        transform.localScale = Vector3.zero;

        // make coin list
        currentPhonemes = new List<ActionWordEnum>();

        // set my coins
        myCoins[0].SetActionWordValue(ActionWordEnum.mudslide);
        myCoins[1].SetActionWordValue(ActionWordEnum.listen);
        myCoins[2].SetActionWordValue(ActionWordEnum.poop);
        myCoins[3].SetActionWordValue(ActionWordEnum.orcs);
        myCoins[4].SetActionWordValue(ActionWordEnum.think);

        myCoins[5].SetActionWordValue(ActionWordEnum.hello);
        myCoins[6].SetActionWordValue(ActionWordEnum.spider);
        myCoins[7].SetActionWordValue(ActionWordEnum.explorer);
        myCoins[8].SetActionWordValue(ActionWordEnum.scared);
        myCoins[9].SetActionWordValue(ActionWordEnum.thatguy);

        myCoins[10].SetActionWordValue(ActionWordEnum.choice);
        myCoins[11].SetActionWordValue(ActionWordEnum.strongwind);
        myCoins[12].SetActionWordValue(ActionWordEnum.pirate);
        myCoins[13].SetActionWordValue(ActionWordEnum.gorilla);
        myCoins[14].SetActionWordValue(ActionWordEnum.sounds);

        myCoins[15].SetActionWordValue(ActionWordEnum.give);
        myCoins[16].SetActionWordValue(ActionWordEnum.backpack);
        myCoins[17].SetActionWordValue(ActionWordEnum.frustrating);
        myCoins[18].SetActionWordValue(ActionWordEnum.bumphead);
        myCoins[19].SetActionWordValue(ActionWordEnum.baby);

        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.SetImageAlpha(windowBGImage, 0f);
    }

    ////////////////////////////////////////////

    private void SetCurrentPhonemes()
    {
        // set all coins as unselected
        foreach (var coin in myCoins)
        {
            coin.GetComponent<UniversalCoinImage>().SetTransparency(0.25f, false);
        }

        // set active coins
        foreach (var coin in myCoins)
        {
            if (currentPhonemes.Contains(ChallengeWordDatabase.ElkoninValueToActionWord(coin.value)))
                coin.GetComponent<UniversalCoinImage>().SetTransparency(1f, false);
        }
    }

    public void TogglePhoneme(int actionWordEnumIntValue)
    {
        ActionWordEnum phoneme = (ActionWordEnum)actionWordEnumIntValue;

        // remove from current list
        if (currentPhonemes.Contains(phoneme))
        {
            currentPhonemes.Remove(phoneme);
            // set active coins
            foreach (var coin in myCoins)
            {
                if (ChallengeWordDatabase.ElkoninValueToActionWord(coin.value) == phoneme)
                {
                    coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector3(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
                    coin.GetComponent<UniversalCoinImage>().SetTransparency(0.25f, true);
                }
            }
        }
        // add to current list
        else
        {
            currentPhonemes.Add(phoneme);
            // set active coins
            foreach (var coin in myCoins)
            {
                if (ChallengeWordDatabase.ElkoninValueToActionWord(coin.value) == phoneme)
                {
                    coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector3(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
                    coin.GetComponent<UniversalCoinImage>().SetTransparency(1f, true);
                }
            }
        }
    }

    public void OpenWindow(List<ActionWordEnum> activatedPhonemes, ReturnLocation returnLocation)
    {
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        currentPhonemes.Clear();
        currentPhonemes.AddRange(activatedPhonemes);
        SetCurrentPhonemes();

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
            case ReturnLocation.blendingWindow:
                PracticeSceneManager.instance.blendingPracticeWindow.ReturnSelectedPhonemes(currentPhonemes);
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
