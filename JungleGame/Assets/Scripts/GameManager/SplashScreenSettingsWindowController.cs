using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SplashScreenSettingsWindowController : MonoBehaviour
{
    public static SplashScreenSettingsWindowController instance;

    public LerpableObject audioWindow;
    public LerpableObject exitWindow;

    public LerpableObject audioTab;
    public LerpableObject exitTab;

    private SettingsTab currentTab;

    [HideInInspector] public bool isAnimating = false;
    private bool tabsOn = false;

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVol;
    [SerializeField] private Slider musicVol;
    [SerializeField] private Slider fxVol;
    [SerializeField] private Slider talkVol;

    [Header("Microphone Settings")]
    [SerializeField] private TMP_Dropdown microphoneDropdown;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        currentTab = SettingsTab.none;

        audioWindow.transform.localScale = new Vector3(0f, 0f, 0f);
        exitWindow.transform.localScale = new Vector3(0f, 0f, 1f);

        audioTab.transform.localScale = new Vector3(0f, 0f, 0f);
        exitTab.transform.localScale = new Vector3(0f, 0f, 0f);

        // set up microphone dropdown
        SetUpMircophone();

        // set audio volumes
        SetUpVolumes();
    }

    public void SaveSplashSettingsToProfile()
    {
        var data = StudentInfoSystem.GetCurrentProfile();

        // volumes
        data.masterVol = Mathf.Round(masterVol.value * 1000.0f) / 1000.0f;
        data.musicVol = Mathf.Round(musicVol.value * 1000.0f) / 1000.0f;
        data.fxVol = Mathf.Round(fxVol.value * 1000.0f) / 1000.0f;
        data.talkVol = Mathf.Round(talkVol.value * 1000.0f) / 1000.0f;
        // mic
        data.micDevice = MicInput.instance.micDeviceIndex;

        GameManager.instance.SendLog(this, "saving splash settings to current profile");

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

    public void CloseAllWindows()
    {   
        if (currentTab == SettingsTab.none)
            return;

        if (isAnimating)
            return;
        isAnimating = true;

        StartCoroutine(CloseAllWindowsRoutine());
    }

    private IEnumerator CloseAllWindowsRoutine()
    {
        ToggleTabs(false);

        yield return new WaitForSeconds(0.3f);

        CloseCurrentWindow();

        yield return new WaitForSeconds(0.4f);

        // remove background
        SettingsManager.instance.settingsWindowBG.LerpImageAlpha(SettingsManager.instance.settingsWindowBG.GetComponent<Image>(), 0f, 0.2f);
        SettingsManager.instance.settingsWindowBG.GetComponent<Image>().raycastTarget = false;
        
        isAnimating = false;
    }

    public void OpenWindow()
    {
        if (isAnimating)
            return;
        isAnimating = true;

        // load from profile
        LoadSettingsFromProfile();

        // add background
        SettingsManager.instance.settingsWindowBG.LerpImageAlpha(SettingsManager.instance.settingsWindowBG.GetComponent<Image>(), 0.75f, 0.2f);
        SettingsManager.instance.settingsWindowBG.GetComponent<Image>().raycastTarget = true;

        // open to audio tab
        StartCoroutine(TabPressedRoutine(SettingsTab.audio));
    }

    public void AudioTabPressed()
    {
        if (currentTab == SettingsTab.audio)
            return;

        if (isAnimating)
            return;
        isAnimating = true;

        StartCoroutine(TabPressedRoutine(SettingsTab.audio));
    }

    public void ExitTabPressed()
    {
        if (currentTab == SettingsTab.exit)
            return;

        if (isAnimating)
            return;
        isAnimating = true;

        StartCoroutine(TabPressedRoutine(SettingsTab.exit));
    }

    private IEnumerator TabPressedRoutine(SettingsTab tab)
    {
        // close tabs
        if (tabsOn)
        {
            ToggleTabs(false);
            yield return new WaitForSeconds(0.3f);
        }
            
        // close current window
        if (currentTab != SettingsTab.none)
        {
            CloseCurrentWindow();
            yield return new WaitForSeconds(0.5f);
        }
        
        switch (tab)
        {
            case SettingsTab.audio:
                currentTab = SettingsTab.audio;
                audioWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;

            case SettingsTab.exit:
                currentTab = SettingsTab.exit;
                exitWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;
        }

        yield return new WaitForSeconds(0.4f);

        ToggleTabs(true);

        yield return new WaitForSeconds(0.3f);

        isAnimating = false;
    }

    private void CloseCurrentWindow()
    {
        switch (currentTab)
        {
            case SettingsTab.audio:
                audioWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;

            case SettingsTab.exit:
                exitWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;
        }

        currentTab = SettingsTab.none;
    }


    public void ToggleTabs(bool opt)
    {
        tabsOn = opt;        

        if (opt)
        {
            StartCoroutine(ShowTabs());
        }
        else
        {
            StartCoroutine(HideTabs());
        }
    }

    private IEnumerator HideTabs()
    {
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
    }

    private IEnumerator ShowTabs()
    {
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
    }
}
