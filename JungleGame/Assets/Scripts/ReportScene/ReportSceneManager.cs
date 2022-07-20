using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ReportSceneManager : MonoBehaviour
{
    // current profile parts
    private int currentProfile;
    private List<StudentPlayerData> profiles;
    public Image profileImage;
    public TextMeshProUGUI profileText;

    // phoneme success window
    [Header("Phoneme Success Window")]
    public List<UniversalCoinImage> coins;
    public List<LerpableObject> fillBars;
    public List<TextMeshProUGUI> percents;
    public Button successAllTimeButton;
    public Button successPrev10Button;
    public Color selectedColor;
    public Color nonselectedColor;
    private bool phonemesAllTime = true;

    void Awake()
    {
        GameManager.instance.SceneInit();

        // get all profiles
        profiles = new List<StudentPlayerData>();
        List<StudentPlayerData> allProfiles = StudentInfoSystem.GetAllStudentDatas();
        // determine active profiles
        foreach (var profile in allProfiles)
        {
            if (profile.active)
            {
                profiles.Add(profile);
            }
        }

        // only allow practice iff more than 0 active profiles
        currentProfile = 0;
        if (profiles.Count > 0)
        {
            PracticeSceneManager.instance.LoadAvatar(profileImage, profiles[currentProfile].profileAvatar);
            profileText.text = profiles[currentProfile].name;

            // update phoneme window
            UpdatePhonemeSuccessWindow();
        }
        else
        {
            // no active profiles
        }

        successAllTimeButton.image.color = selectedColor;
        successPrev10Button.image.color = nonselectedColor;
    }

    public void OnBackButtonPressed()
    {
        GameManager.instance.LoadScene("SplashScene", true);
    }

    public void OnProfileButtonPressed()
    {
        if (profiles.Count > 1)
        {
            PracticeSceneManager.instance.UnloadAvatar(currentProfile);
        }
        currentProfile++;
        if (currentProfile > profiles.Count - 1)
        {
            currentProfile = 0;
        }

        // swap current profile
        profileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        PracticeSceneManager.instance.LoadAvatar(profileImage, profiles[currentProfile].profileAvatar);
        profileText.text = profiles[currentProfile].name;

        // update phoneme window
        UpdatePhonemeSuccessWindow();
    }

    public void OnPhonemesAllTimePressed()
    {
        if (!phonemesAllTime)
        {
            phonemesAllTime = true;
            successAllTimeButton.image.color = selectedColor;
            successPrev10Button.image.color = nonselectedColor;
            UpdatePhonemeSuccessWindow();
        }
    }

    public void OnPhonemesPrev10Pressed()
    {
        if (phonemesAllTime)
        {
            phonemesAllTime = false;
            successAllTimeButton.image.color = nonselectedColor;
            successPrev10Button.image.color = selectedColor;
            UpdatePhonemeSuccessWindow();
        }
    }

    public void UpdatePhonemeSuccessWindow()
    {
        List<PhonemeData> data = new List<PhonemeData>();
        data.AddRange(profiles[currentProfile].phonemeData);

        for (int i = 0; i < data.Count; i++)
        {
            float percent = 0f;
            if (phonemesAllTime)
            {
                percent = data[i].GetSuccessAllTime() * 100f;
            }
            else
            {
                percent = data[i].GetSuccessPrev10() * 100f;
            }
            // round to one decimal place
            percent = Mathf.Round(percent * 10.0f) * 0.1f;

            coins[i].SetValue(data[i].elkoninValue);
            percents[i].text = percent.ToString() + "%";
            fillBars[i].transform.localScale = new Vector3(0f, 1f, 1f);
            fillBars[i].LerpScale(new Vector2(percent / 100f, 1f), 0.5f);
        }
    }
}
