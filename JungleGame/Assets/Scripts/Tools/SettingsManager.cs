using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
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
    

    void Start()
    {
        // set up microphone dropdown
        SetUpMircophone();

        // set audio volumes
        SetUpVolumes();

        // close window
        settingsWindow.SetActive(false);
    }

    private void SetUpVolumes()
    {
        // master vol
        masterVol.onValueChanged.AddListener(delegate { OnMasterVolumeSliderChanged(); });
        print("master vol: " + AudioManager.instance.GetMasterVolume());
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

    public void ToggleSettingsWindow()
    {
        settingsWindowOpen = !settingsWindowOpen;
        settingsWindow.SetActive(settingsWindowOpen);
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
