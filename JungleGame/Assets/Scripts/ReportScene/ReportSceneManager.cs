using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportSceneManager : MonoBehaviour
{
    // current profile parts
    private int currentProfile;
    private List<StudentPlayerData> profiles;
    public Image profileImage;
    public TextMeshProUGUI profileText;

    public TextMeshProUGUI playerMasteryLevel;
    public List<Color> entryColors;

    [Header("Minigame Success Window")]
    public TextMeshProUGUI minigameEntry1name;
    public TextMeshProUGUI minigameEntry1num;
    public TextMeshProUGUI minigameEntry2name;
    public TextMeshProUGUI minigameEntry2num;
    public TextMeshProUGUI minigameEntry3name;
    public TextMeshProUGUI minigameEntry3num;
    public TextMeshProUGUI minigameEntry4name;
    public TextMeshProUGUI minigameEntry4num;
    public TextMeshProUGUI minigameEntry5name;
    public TextMeshProUGUI minigameEntry5num;
    public TextMeshProUGUI minigameEntry6name;
    public TextMeshProUGUI minigameEntry6num;

    [Header("Challenge Game Success Window")]
    public TextMeshProUGUI challengeGameEntry1name;
    public TextMeshProUGUI challengeGameEntry1num;
    public TextMeshProUGUI challengeGameEntry2name;
    public TextMeshProUGUI challengeGameEntry2num;
    public TextMeshProUGUI challengeGameEntry3name;
    public TextMeshProUGUI challengeGameEntry3num;
    public TextMeshProUGUI challengeGameEntry4name;
    public TextMeshProUGUI challengeGameEntry4num;
    public TextMeshProUGUI challengeGameEntry5name;
    public TextMeshProUGUI challengeGameEntry5num;
    public TextMeshProUGUI challengeGameEntry6name;
    public TextMeshProUGUI challengeGameEntry6num;
    public TextMeshProUGUI challengeGameEntry7name;
    public TextMeshProUGUI challengeGameEntry7num;

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
            profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
            profileText.text = profiles[currentProfile].name;

            // update entire scene
            UpdateEntireReportScene();
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
        currentProfile++;
        if (currentProfile > profiles.Count - 1)
        {
            currentProfile = 0;
        }

        // swap current profile
        profileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
        profileText.text = profiles[currentProfile].name;

        UpdateEntireReportScene();
    }

    public void UpdateEntireReportScene()
    {
        // update master level
        int blendNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsBlend / 3);
        int subNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsSub / 3);
        int buildNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsBuild / 3);
        int deleteNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsBlend / 3);
        int tpCoinNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsTPawCoin / 3);
        int tpPhotosNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsTPawPol / 3);
        int passwordNum = 1 + Mathf.FloorToInt(profiles[currentProfile].starsPass / 3);

        float averageNum = (float)(blendNum + subNum + buildNum + deleteNum + tpCoinNum + tpPhotosNum + passwordNum) / 7f;
        averageNum = Mathf.Round(averageNum * 10.0f) * 0.1f;
        playerMasteryLevel.text = averageNum.ToString();

        // update minigame success rates
        UpdateMinigameWindow();

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

    private class MinigameEntry
    {
        GameType gameType;
        float rate;

        public MinigameEntry(GameType _gameType, float _rate)
        {
            this.gameType = _gameType;
            this.rate = _rate;
        }
    }

    public void UpdateMinigameWindow()
    {
        List<MinigameEntry> minigameEntries = new List<MinigameEntry>();

        // frogger stats
        int froggerSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].froggerData)
        {
            if (dataPoint.success)
                froggerSuccessRounds++;
        }
        float frogggerPercent = (float)froggerSuccessRounds / (float)profiles[currentProfile].froggerData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.FroggerGame, frogggerPercent));

        // rummage stats
        int rummageSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].rummageData)
        {
            if (dataPoint.success)
                rummageSuccessRounds++;
        }
        float rummagePercent = (float)rummageSuccessRounds / (float)profiles[currentProfile].rummageData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.RummageGame, rummagePercent));

        // seashell stats
        int seashellSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].seashellsData)
        {
            if (dataPoint.success)
                seashellSuccessRounds++;
        }
        float seashellPercent = (float)seashellSuccessRounds / (float)profiles[currentProfile].seashellsData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.SeashellGame, seashellPercent));

        // spiderweb stats
        int spiderwebSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].spiderwebData)
        {
            if (dataPoint.success)
                spiderwebSuccessRounds++;
        }
        float spiderwebPercent = (float)spiderwebSuccessRounds / (float)profiles[currentProfile].spiderwebData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.SpiderwebGame, spiderwebPercent));

        // turntables stats
        int turntablesSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].turntablesData)
        {
            if (dataPoint.success)
                turntablesSuccessRounds++;
        }
        float turntablesPercent = (float)turntablesSuccessRounds / (float)profiles[currentProfile].turntablesData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.TurntablesGame, turntablesPercent));

        // pirate stats
        int pirateSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].pirateData)
        {
            if (dataPoint.success)
                pirateSuccessRounds++;
        }
        float piratePercent = (float)pirateSuccessRounds / (float)profiles[currentProfile].pirateData.Count;
        minigameEntries.Add(new MinigameEntry(GameType.PirateGame, piratePercent));

        // order percents greatest to smallest
        
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
