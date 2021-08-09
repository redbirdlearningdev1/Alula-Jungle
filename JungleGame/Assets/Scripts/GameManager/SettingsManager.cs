using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [SerializeField] private Button menuButton;
    [SerializeField] private Button wagonButton;

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

        // close window
        settingsWindow.SetActive(false);
    }

    public void SaveSettingsToProfile()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        // volumes
        StudentInfoSystem.currentStudentPlayer.masterVol = Mathf.Round(masterVol.value * 1000.0f) / 1000.0f;
        StudentInfoSystem.currentStudentPlayer.musicVol = Mathf.Round(musicVol.value * 1000.0f) / 1000.0f;
        StudentInfoSystem.currentStudentPlayer.fxVol = Mathf.Round(fxVol.value * 1000.0f) / 1000.0f;
        StudentInfoSystem.currentStudentPlayer.talkVol = Mathf.Round(talkVol.value * 1000.0f) / 1000.0f;
        // mic
        StudentInfoSystem.currentStudentPlayer.micDevice = MicInput.instance.micDeviceIndex;
        // save to profile
        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void LoadSettingsFromProfile()
    {
        var data = StudentInfoSystem.currentStudentPlayer;
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

    public void ToggleWagonButtonActive(bool opt)
    {
        GameManager.instance.SendLog(this, "setting wagon button to: " + opt);
        if (opt)
        {
            wagonButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            wagonButton.interactable = true;
        }
        else
        {
            wagonButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            wagonButton.interactable = false;
        }
    }

    public void ToggleMenuButtonActive(bool opt)
    {
        GameManager.instance.SendLog(this, "setting menu button to: " + opt);
        if (opt)
        {
            menuButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            menuButton.interactable = true;
        }
        else
        {
            menuButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            menuButton.interactable = false;
        }
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

        // only workable on scroll map scene
        if (SceneManager.GetActiveScene().name == "ScrollMap")
            ScrollMapManager.instance.ToggleCart();
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
