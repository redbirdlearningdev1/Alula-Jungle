using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public Button menuButton;
    public Button wagonButton;

    public float hiddenButtonYvalue;
    public float shownButtonYvalue;

    private bool movingMenuButton = false;
    private bool movingWagonButton = false;

    private bool menuButtonShown = false;
    private bool wagonButtonShown = false;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider fxVol;
    [SerializeField] private Slider talkVol;

    [Header("Microphone Settings")]
    [SerializeField] private TMP_Dropdown microphoneDropdown;

    [Header("Talkie Toggles")]
    public Toggle talkieSubtitlesToggle;
    public Toggle fastTalkiesToggle;
    public Toggle talkieParticlesToggle;

    // settings windows
    public LerpableObject settingsWindowBG;
    public LerpableObject returnToScrollMapWindow;
    public LerpableObject returnToScrollMapButton;
    public LerpableObject returnToScrollMapConfirmWindow;
    public LerpableObject returnToSplashScreenConfirmWindow;
    

    private bool settingsWindowOpen;
    private bool animatingWindow = false;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        // set up microphone dropdown
        SetUpMircophone();

        // set audio volumes
        SetUpVolumes();

        // hide buttons
        menuButton.transform.localPosition = new Vector3(menuButton.transform.localPosition.x, hiddenButtonYvalue, 1f);
        wagonButton.transform.localPosition = new Vector3(wagonButton.transform.localPosition.x, hiddenButtonYvalue, 1f);

        // close settings windows + hide BG
        settingsWindowBG.SetImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f);
        returnToScrollMapWindow.transform.localScale = new Vector3(0f, 0f, 1f);
        returnToScrollMapButton.transform.localScale = new Vector3(0f, 0f, 1f);
        returnToScrollMapConfirmWindow.transform.localScale = new Vector3(0f, 0f, 1f);
    }

    public void SaveSettingsToProfile()
    {
        var data = StudentInfoSystem.GetCurrentProfile();

        // volumes
        data.masterVol = Mathf.Round(masterVol.value * 1000.0f) / 1000.0f;
        data.musicVol = Mathf.Round(musicVol.value * 1000.0f) / 1000.0f;
        data.fxVol = Mathf.Round(fxVol.value * 1000.0f) / 1000.0f;
        data.talkVol = Mathf.Round(talkVol.value * 1000.0f) / 1000.0f;
        // mic
        data.micDevice = MicInput.instance.micDeviceIndex;
        // talkie options
        data.talkieSubtitles = talkieSubtitlesToggle.isOn;
        data.talkieFast = fastTalkiesToggle.isOn;
        data.talkieParticles = talkieParticlesToggle.isOn;

        // save to profile
        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void LoadSettingsFromProfile()
    {
        var data = StudentInfoSystem.GetCurrentProfile();
        // volumes
        AudioManager.instance.SetMasterVolume(data.masterVol);
        masterVol.value = AudioManager.instance.GetMasterVolume();

        AudioManager.instance.SetMusicVolume(data.musicVol);
        musicVol.value = AudioManager.instance.GetMusicVolume();

        AudioManager.instance.SetFXVolume(data.fxVol);
        fxVol.value = AudioManager.instance.GetFxVolume();

        AudioManager.instance.SetTalkVolume(data.talkVol);
        talkVol.value = AudioManager.instance.GetTalkVolume();

        // mic
        MicInput.instance.SwitchDevice(data.micDevice);
        microphoneDropdown.value = data.micDevice;

        // toggles
        talkieSubtitlesToggle.isOn = data.talkieSubtitles;
        fastTalkiesToggle.isOn = data.talkieFast;
        talkieParticlesToggle.isOn = data.talkieParticles;
    }

    private void SetUpVolumes()
    {
        // master vol
        masterVol.onValueChanged.AddListener(delegate { OnMasterVolumeSliderChanged(); });
        masterVol.value = AudioManager.instance.GetMasterVolume();

        // music vol
        musicVol.onValueChanged.AddListener(delegate { OnMusicVolumeSliderChanged(); });
        musicVol.value = AudioManager.instance.GetMusicVolume();

        // fx vol
        fxVol.onValueChanged.AddListener(delegate { OnFxVolumeSliderChanged(); });
        fxVol.value = AudioManager.instance.GetFxVolume();

        // talk vol
        talkVol.onValueChanged.AddListener(delegate { OnTalkVolumeSliderChanged(); });
        talkVol.value = AudioManager.instance.GetTalkVolume();
    }

    /* 
    ################################################
    #   GAMEPLAY SETTINGS
    ################################################
    */

    public void ToggleTalkieSubtitles()
    {
        // play audio blip
        //AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        var data = StudentInfoSystem.GetCurrentProfile();
        StudentInfoSystem.GetCurrentProfile().talkieSubtitles = !data.talkieSubtitles;
    }

    public void ToggleFastTalkies()
    {
        // play audio blip
        //AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        var data = StudentInfoSystem.GetCurrentProfile();
        StudentInfoSystem.GetCurrentProfile().talkieFast = !data.talkieFast;
    }

    public void ToggleTalkieParticles()
    {
        // play audio blip
        //AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        var data = StudentInfoSystem.GetCurrentProfile();
        StudentInfoSystem.GetCurrentProfile().talkieParticles = !data.talkieParticles;
    }

    public void AllTutorialsOff()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // skip tutorials
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = true;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = true;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = true;
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = true;
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = true;
        // skip challenge game tutorials
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryBuildingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
        StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
        StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;

        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void AllTutorialsOn()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // add tutorials
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = false;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = false;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = false;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = false;
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = false;
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = false;
        // add challenge game tutorials
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactoryBuildingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = false;
        StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = false;
        StudentInfoSystem.GetCurrentProfile().passwordTutorial = false;

        StudentInfoSystem.SaveStudentPlayerData();
    }

    /* 
    ################################################
    #   MICROPHONE SETTINGS
    ################################################
    */

    private void SetUpMircophone()
    {
        // update microphone dropdown with available
        List<TMP_Dropdown.OptionData> devices = new List<TMP_Dropdown.OptionData>();
        foreach (string opt in Microphone.devices)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = opt;
            devices.Add(option);
        }
        microphoneDropdown.ClearOptions();
        microphoneDropdown.AddOptions(devices);
        microphoneDropdown.onValueChanged.AddListener(delegate { OnMicrophoneDropdownChange(microphoneDropdown); });
    }

    private void OnMicrophoneDropdownChange(TMP_Dropdown dropdown)
    {
        GameManager.instance.SendLog(this, "changing microphone device to: " + dropdown.value);
        MicInput.instance.SwitchDevice(dropdown.value);
    }

    /* 
    ################################################
    #   MISC FUNCTIONS
    ################################################
    */

    public void ToggleStickerButtonWiggleGlow(bool opt)
    {
        if (opt)
        {
            wagonButton.GetComponent<WiggleController>().StartWiggle();
            ImageGlowController.instance.SetImageGlow(wagonButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
        }
        else 
        {
            wagonButton.GetComponent<WiggleController>().StopWiggle();
            ImageGlowController.instance.SetImageGlow(wagonButton.GetComponent<Image>(), false, GlowValue.none);
        }
    }

    public void SetWagonButton(bool opt)
    {
        if (opt)
        {
            wagonButtonShown = true;
            movingWagonButton = false;
            wagonButton.transform.localPosition = new Vector3(wagonButton.transform.localPosition.x, shownButtonYvalue, 1f);
        }
        else 
        {
            wagonButtonShown = false;
            movingWagonButton = false;
            wagonButton.transform.localPosition = new Vector3(wagonButton.transform.localPosition.x, hiddenButtonYvalue, 1f);
        }
    }

    public void SetMenuButton(bool opt)
    {
        if (opt)
        {
            menuButtonShown = true;
            movingMenuButton = false;
            menuButton.transform.localPosition = new Vector3(menuButton.transform.localPosition.x, shownButtonYvalue, 1f);
        }
        else 
        {
            menuButtonShown = false;
            movingMenuButton = false;
            menuButton.transform.localPosition = new Vector3(menuButton.transform.localPosition.x, hiddenButtonYvalue, 1f);
        }
    }

    public void ToggleWagonButtonActive(bool opt)
    {
        StartCoroutine(ToggleWagonButtonRoutine(opt));
    }

    private IEnumerator ToggleWagonButtonRoutine(bool opt)
    {
        // check if bools are equal
        if (opt == wagonButtonShown)
            yield break;

        // wait for bool to be false
        while (movingWagonButton)
            yield return null;

        movingWagonButton = true;

        if (opt)
        {
            wagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            wagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            wagonButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            wagonButton.GetComponent<LerpableObject>().LerpYPos(hiddenButtonYvalue, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }

        wagonButtonShown = opt;
        movingWagonButton = false;
    }

    public void ToggleMenuButtonActive(bool opt)
    {
        StartCoroutine(ToggleMenuButtonRoutine(opt));
    }

    private IEnumerator ToggleMenuButtonRoutine(bool opt)
    {
        // check if bools are equal
        if (opt == menuButtonShown)
            yield break;

        // wait for bool to be false
        while (movingMenuButton)
            yield return null;

        movingMenuButton = true;

        if (opt)
        {
            menuButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            menuButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            menuButton.GetComponent<LerpableObject>().LerpYPos(shownButtonYvalue - 50, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
            menuButton.GetComponent<LerpableObject>().LerpYPos(hiddenButtonYvalue, 0.2f, true);
            yield return new WaitForSeconds(0.2f);
        }

        menuButtonShown = opt;
        movingMenuButton = false;
    }

    public void ToggleSettingsWindow()
    {
        settingsWindowOpen = !settingsWindowOpen;

        if (animatingWindow)
            return;
        animatingWindow = true;
        
        // open settings window iff at scroll map
        if (SceneManager.GetActiveScene().name == "ScrollMap" || SceneManager.GetActiveScene().name == "SplashScene")
        {
            StartCoroutine(ToggleSettingsWindow(settingsWindowOpen));
        } 
        // else open return to scroll map window
        else
        {
            StartCoroutine(ToggleReturnToScrollMapWindow(settingsWindowOpen));
        }
    }

    public IEnumerator ToggleSettingsWindow(bool opt)
    {
        if (opt)
        {
            // open window
            SettingsWindowController.instance.OpenWindow();
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0.75f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = true;
        }
        else
        {
            // save settings to profile
            SaveSettingsToProfile();

            // close window
            SettingsWindowController.instance.CloseAllWindows();
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = false;

            // close confirm windows iff open
            if (returnToScrollMapConfirmWindow.transform.localScale.x > 0f)
                CloseConfirmScrollMapWindow();
            if (returnToSplashScreenConfirmWindow.transform.localScale.x > 0f)
                CloseConfirmSplashScreenWindow();
            
        }

        yield return new WaitForSeconds(0.2f);

        animatingWindow = false;
    }

    public IEnumerator ToggleReturnToScrollMapWindow(bool opt)
    {
        if (opt)
        {
            // open window
            returnToScrollMapWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.2f);
            returnToScrollMapButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        }
        else
        {
            // close window
            returnToScrollMapButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.2f);
            returnToScrollMapWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        }
        animatingWindow = false;
    }

    public void OpenConfirmScrollMapWindow()
    {
        StartCoroutine(ToggleReturnToScrollMapsConfirmWindow(true));
    }

    public void CloseConfirmScrollMapWindow()
    {
        StartCoroutine(ToggleReturnToScrollMapsConfirmWindow(false));
    }

    public void GoToScrollMapButtonPressed()
    {
        StartCoroutine(GoToScollMapButtonRoutine());
    }

    private IEnumerator GoToScollMapButtonRoutine()
    {
        // close confirm window
        returnToScrollMapConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
        settingsWindowBG.GetComponent<Image>().raycastTarget = false;

        // close settings window
        returnToScrollMapButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.2f);
        returnToScrollMapWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);

        // go to scroll map scene
        GameManager.instance.LoadScene("ScrollMap", true, 0.5f, true);
    }

    public IEnumerator ToggleReturnToScrollMapsConfirmWindow(bool opt)
    {
        if (opt)
        {
            // open window
            returnToScrollMapConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0.75f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = true;
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            // close window
            returnToScrollMapConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = false;
            yield return new WaitForSeconds(0.2f);

            // close window too
            StartCoroutine(ToggleReturnToScrollMapWindow(false));
            settingsWindowOpen = false;
            yield return new WaitForSeconds(0.2f);
        }
        animatingWindow = false;
    }

    public void CloseSettingsWindow()
    {
        // do this thing
    }






    public IEnumerator ToggleReturnToSplashScreenConfirmWindow(bool opt)
    {
        if (opt)
        {
            // open window
            returnToSplashScreenConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0.75f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = true;
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            // close window
            returnToSplashScreenConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
            settingsWindowBG.GetComponent<Image>().raycastTarget = false;
            yield return new WaitForSeconds(0.2f);
        }
        animatingWindow = false;
    }

    public void OpenConfirmSplashScreenWindow()
    {
        StartCoroutine(ToggleReturnToSplashScreenConfirmWindow(true));
    }

    public void CloseConfirmSplashScreenWindow()
    {
        StartCoroutine(ToggleReturnToSplashScreenConfirmWindow(false));
    }

    public void GoToSplashScreenButtonPressed()
    {
        StartCoroutine(GoToSplashScreenButtonRoutine());
    }

    private IEnumerator GoToSplashScreenButtonRoutine()
    {
        // close confirm window
        returnToSplashScreenConfirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        settingsWindowBG.LerpImageAlpha(settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
        settingsWindowBG.GetComponent<Image>().raycastTarget = false;

        // close settings window
        SettingsWindowController.instance.CloseAllWindows();
        yield return new WaitForSeconds(0.2f);

        // go to scroll map scene
        GameManager.instance.LoadScene("SplashScene", true, 0.5f, true);
    }






    public void OnWagonButtonPressed()
    {   
        // remove wiggle if need be
        ToggleStickerButtonWiggleGlow(false);

        // only workable on scroll map scene (or dev menu)
        if (SceneManager.GetActiveScene().name == "ScrollMap" ||
            SceneManager.GetActiveScene().name == "DevMenu")
            WagonWindowController.instance.ToggleCart();
    }

    public void OnMasterVolumeSliderChanged()
    {
        AudioManager.instance.SetMasterVolume(masterVol.value);
        //print ("set master vol: " + AudioManager.instance.GetMasterVolume());
    }

    public void OnMusicVolumeSliderChanged()
    {
        AudioManager.instance.SetMusicVolume(musicVol.value);
        //print ("set music vol: " + AudioManager.instance.GetMusicVolume());
    }

    public void OnFxVolumeSliderChanged()
    {
        AudioManager.instance.SetFXVolume(fxVol.value);
        //print ("set fx vol: " + AudioManager.instance.GetFxVolume());
    }

    public void OnTalkVolumeSliderChanged()
    {
        AudioManager.instance.SetTalkVolume(talkVol.value);
        //print ("set talk vol: " + AudioManager.instance.GetTalkVolume());
    }
}
