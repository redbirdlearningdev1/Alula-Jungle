using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsSceneManager : MonoBehaviour
{
    [Header("Microphone Settings")]
    [SerializeField] private TMP_Dropdown microphoneDropdown;
    
    [SerializeField] private Image volumeBar;
    [SerializeField] private Toggle testMicToggle;
    public float timeBetwnVolumeUpdates;
    private float timer = 0f;
    private bool testMic = false;

    [Header("Audio Settings")]
    [SerializeField] private Toggle muteMusicToggle;


    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        SetUpMircophone();
    }

    void Update()
    {
        // shows volume input
        if (testMic)
        {
            if (timer < timeBetwnVolumeUpdates)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
                float volumeLevel = MicInput.MicLoudness * 200;
                //GameManager.instance.SendLog(this, "Current mic volume: " + volumeLevel.ToString());

                if (volumeLevel <= 0.3f)
                {
                    volumeBar.color = Color.green;
                }
                else if (volumeLevel <= 0.7f)
                {
                    volumeBar.color = Color.yellow;
                }
                else 
                {
                    volumeBar.color = Color.red;
                }
                volumeBar.transform.localScale = new Vector3(volumeLevel + 0.01f, 0.5f, 1f);
            }
        }
        
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

        // toggle 
        testMicToggle.onValueChanged.AddListener(delegate { ToggleMicrophoneTest(testMicToggle); });
    }

    private void ToggleMicrophoneTest(Toggle toggle)
    {
        GameManager.instance.SendLog(this, "toggle mic set to: " + toggle.isOn);
        testMic = toggle.isOn;
    }

    private void OnMicrophoneDropdownChange(TMP_Dropdown dropdown)
    {
        GameManager.instance.SendLog(this, "changing microphone device to: " + dropdown.value);
        MicInput.instance.SwitchDevice(dropdown.value);
    }

    /* 
    ################################################
    #   OTHER
    ################################################
    */
    
    public void OnBackButtonPressed()
    {
        GameManager.instance.LoadScene("ScrollMap", true);
    }
}
