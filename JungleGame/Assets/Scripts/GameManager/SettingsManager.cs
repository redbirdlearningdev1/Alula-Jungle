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

    [Header("Audio Settings")]
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider fxVol;
    [SerializeField] private Slider talkVol;

    [Header("Microphone Settings")]
    [SerializeField] private TMP_Dropdown microphoneDropdown;

    // settings window
    [SerializeField] private GameObject settingsWindow;
    private bool settingsWindowOpen;


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

        // close window
        settingsWindow.SetActive(false);
    }

    public void SaveSettingsToProfile()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        var data = StudentInfoSystem.GetCurrentProfile();

        // volumes
        data.masterVol = Mathf.Round(masterVol.value * 1000.0f) / 1000.0f;
        data.musicVol = Mathf.Round(musicVol.value * 1000.0f) / 1000.0f;
        data.fxVol = Mathf.Round(fxVol.value * 1000.0f) / 1000.0f;
        data.talkVol = Mathf.Round(talkVol.value * 1000.0f) / 1000.0f;
        // mic
        data.micDevice = MicInput.instance.micDeviceIndex;
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

    public void ToggleStickerButtonWiggle(bool opt)
    {
        if (opt) wagonButton.GetComponent<WiggleController>().StartWiggle();
        else wagonButton.GetComponent<WiggleController>().StopWiggle();
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
            print ("here");
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

        print ("toggling wagon button - " + opt);

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

        print ("toggling menu button - " + opt);

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
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        settingsWindowOpen = !settingsWindowOpen;
        settingsWindow.SetActive(settingsWindowOpen);
    }

    public void OnWagonButtonPressed()
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // remove wiggle if need be
        ToggleStickerButtonWiggle(false);

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
