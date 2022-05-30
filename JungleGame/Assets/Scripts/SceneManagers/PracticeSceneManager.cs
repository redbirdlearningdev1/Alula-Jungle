using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeSceneManager : MonoBehaviour
{
    public static PracticeSceneManager instance;

    public LerpableObject backButton;
    public LerpableObject windowBG;
    public Image windowBGImage;

    public BlendingPracticeWindow blendingPracticeWindow;

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
        yield return new WaitForSeconds(1f);

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
        backButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        GameManager.instance.RestartGame();
    }

    public void OnBlendingPressed()
    {
        // hide window BG
        windowBGImage.raycastTarget = true;
        windowBG.LerpImageAlpha(windowBGImage, 0.9f, 0.5f);

        blendingPracticeWindow.OpenWindow();
    }
}
