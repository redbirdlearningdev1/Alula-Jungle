using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PracticeModeGame
{
    blending,
    phoneme_in_word,
    elkonin_boxes,
    phoneme_changing,
    phoneme_id,
    phoneme_practice
}

public class PracticeSceneManager : MonoBehaviour
{
    public static PracticeSceneManager instance;

    public LerpableObject backButton;
    public LerpableObject windowBG;
    public Image windowBGImage;

    public DefaultPracticeWindow defaultPracticeWindow;
    public PhonemeChangingWindow phonemeChangingWindow;
    public PhonemePracticeWindow phonemePracticeWindow;

    public List<LerpableObject> practiceButtons;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // hide back button
        backButton.transform.localScale = Vector3.zero;

        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.SetImageAlpha(windowBGImage, 0f);

        // hide all buttons
        foreach (var b in practiceButtons)
        {
            b.transform.localScale = Vector3.zero;
        }
    }

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        // play song
        AudioManager.instance.PlaySong(AudioDatabase.instance.SplashScreenSong);

        // short delay before showing UI
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);

        // show all buttons
        foreach (var b in practiceButtons)
        {
            b.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.25f);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

        // show UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    public void RemoveWindowBG()
    {
        // hide window BG
        windowBGImage.raycastTarget = false;
        windowBG.LerpImageAlpha(windowBGImage, 0f, 0.5f);
    }

    public void OnBackButtonPressed()
    {
        // show all buttons
        foreach (var b in practiceButtons)
        {
            b.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }

        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        GameManager.instance.RestartGame();
    }

    public void OnBlendingPressed()
    {
        practiceButtons[0].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.blending);
    }

    public void OnPhonemeInWordPressed()
    {
        practiceButtons[1].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.phoneme_in_word);
    }

    public void OnElkoninBoxesPressed()
    {
        practiceButtons[2].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.elkonin_boxes);
    }

    public void OnPhonemeIDPressed()
    {
        practiceButtons[4].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        defaultPracticeWindow.OpenWindow(PracticeModeGame.phoneme_id);
    }

    public void OnPhonemeChangingPressed()
    {
        practiceButtons[3].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        phonemeChangingWindow.OpenWindow();
    }

    public void OnPhonemePracticePressed()
    {
        practiceButtons[5].SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        phonemePracticeWindow.OpenWindow();
    }
}
