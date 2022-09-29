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

    public TextMeshProUGUI playerMasteryLevel;

    public GraphObject graph;

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
            PracticeSceneManager.instance.LoadAvatar(profileImage, profiles[currentProfile].profileAvatar);
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

        // update graph
        graph.CreateGraph(profiles[currentProfile]);

        // update minigame success rates
        UpdateMinigameWindow();

        // update challenge game success rates
        UpdateChallengeGameWindow();

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

    private class GameEntry
    {
        public GameType gameType;
        public float rate;

        public GameEntry(GameType _gameType, float _rate)
        {
            this.gameType = _gameType;
            this.rate = _rate;
        }
    }

    public void UpdateChallengeGameWindow()
    {
        List<GameEntry> challengeGameEntries = new List<GameEntry>();

        // blend stats
        int blendingSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].blendData)
        {
            if (dataPoint.success)
                blendingSuccessRounds++;
        }
        float blendPercent = (float)blendingSuccessRounds / (float)profiles[currentProfile].blendData.Count;
        if (float.IsNaN(blendPercent))
        {
            blendPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.WordFactoryBlending, blendPercent));

        // sub stats
        int subSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].subData)
        {
            if (dataPoint.success)
                subSuccessRounds++;
        }
        float subPercent = (float)subSuccessRounds / (float)profiles[currentProfile].subData.Count;
        if (float.IsNaN(subPercent))
        {
            subPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.WordFactorySubstituting, subPercent));

        // build stats
        int buildSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].buildData)
        {
            if (dataPoint.success)
                buildSuccessRounds++;
        }
        float buildPercent = (float)buildSuccessRounds / (float)profiles[currentProfile].buildData.Count;
        if (float.IsNaN(buildPercent))
        {
            buildPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.WordFactoryBuilding, buildPercent));

        // delete stats
        int deleteSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].deleteData)
        {
            if (dataPoint.success)
                deleteSuccessRounds++;
        }
        float deletePercent = (float)deleteSuccessRounds / (float)profiles[currentProfile].deleteData.Count;
        if (float.IsNaN(deletePercent))
        {
            deletePercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.WordFactoryDeleting, deletePercent));

        // TP coins stats
        int tpCoinsSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].TPCoinsData)
        {
            if (dataPoint.success)
                tpCoinsSuccessRounds++;
        }
        float tpCoinsPercent = (float)tpCoinsSuccessRounds / (float)profiles[currentProfile].TPCoinsData.Count;
        if (float.IsNaN(tpCoinsPercent))
        {
            tpCoinsPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.TigerPawCoins, tpCoinsPercent));

        // TP photos stats
        int tpPhotosSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].TPPhotosData)
        {
            if (dataPoint.success)
                tpPhotosSuccessRounds++;
        }
        float tpPhotosPercent = (float)tpPhotosSuccessRounds / (float)profiles[currentProfile].TPPhotosData.Count;
        if (float.IsNaN(tpPhotosPercent))
        {
            tpPhotosPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.TigerPawPhotos, tpPhotosPercent));

        // password stats
        int passSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].passwordData)
        {
            if (dataPoint.success)
                passSuccessRounds++;
        }
        float passPercent = (float)passSuccessRounds / (float)profiles[currentProfile].passwordData.Count;
        if (float.IsNaN(passPercent))
        {
            passPercent = 0f;
        }
        challengeGameEntries.Add(new GameEntry(GameType.Password, passPercent));

        // order percents greatest to smallest
        for (int i = 0; i < 7; i++)
        {
            float max = float.MinValue;
            GameEntry currEntry = null;
            
            foreach (var entry in challengeGameEntries)
            {
                if (entry.rate > max)
                {
                    max = entry.rate;
                    currEntry = entry;
                }
            }

            max *= 100f;
            max = Mathf.Round(max * 10.0f) * 0.1f;
            string gameName = currEntry.gameType.ToString();
            gameName = gameName.Replace("WordFactory", "");
            gameName = gameName.Replace("TigerPaw", "tp ");

            switch (i)
            {
                case 0:
                    challengeGameEntry1name.text = gameName;
                    challengeGameEntry1num.text = max.ToString() + "%";
                    break;

                case 1:
                    challengeGameEntry2name.text = gameName;
                    challengeGameEntry2num.text = max.ToString() + "%";
                    break;

                case 2:
                    challengeGameEntry3name.text = gameName;
                    challengeGameEntry3num.text = max.ToString() + "%";
                    break;

                case 3:
                    challengeGameEntry4name.text = gameName;
                    challengeGameEntry4num.text = max.ToString() + "%";
                    break;

                case 4:
                    challengeGameEntry5name.text = gameName;
                    challengeGameEntry5num.text = max.ToString() + "%";
                    break;

                case 5:
                    challengeGameEntry6name.text = gameName;
                    challengeGameEntry6num.text = max.ToString() + "%";
                    break;

                case 6:
                    challengeGameEntry7name.text = gameName;
                    challengeGameEntry7num.text = max.ToString() + "%";
                    break;
            }

            challengeGameEntries.Remove(currEntry);
        }
    }

    public void UpdateMinigameWindow()
    {
        List<GameEntry> minigameEntries = new List<GameEntry>();

        // frogger stats
        int froggerSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].froggerData)
        {
            if (dataPoint.success)
                froggerSuccessRounds++;
        }
        float frogggerPercent = (float)froggerSuccessRounds / (float)profiles[currentProfile].froggerData.Count;
        if (float.IsNaN(frogggerPercent))
        {
            frogggerPercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.FroggerGame, frogggerPercent));

        // rummage stats
        int rummageSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].rummageData)
        {
            if (dataPoint.success)
                rummageSuccessRounds++;
        }
        float rummagePercent = (float)rummageSuccessRounds / (float)profiles[currentProfile].rummageData.Count;
        if (float.IsNaN(rummagePercent))
        {
            rummagePercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.RummageGame, rummagePercent));

        // seashell stats
        int seashellSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].seashellsData)
        {
            if (dataPoint.success)
                seashellSuccessRounds++;
        }
        float seashellPercent = (float)seashellSuccessRounds / (float)profiles[currentProfile].seashellsData.Count;
        if (float.IsNaN(seashellPercent))
        {
            seashellPercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.SeashellGame, seashellPercent));

        // spiderweb stats
        int spiderwebSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].spiderwebData)
        {
            if (dataPoint.success)
                spiderwebSuccessRounds++;
        }
        float spiderwebPercent = (float)spiderwebSuccessRounds / (float)profiles[currentProfile].spiderwebData.Count;
        if (float.IsNaN(spiderwebPercent))
        {
            spiderwebPercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.SpiderwebGame, spiderwebPercent));

        // turntables stats
        int turntablesSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].turntablesData)
        {
            if (dataPoint.success)
                turntablesSuccessRounds++;
        }
        float turntablesPercent = (float)turntablesSuccessRounds / (float)profiles[currentProfile].turntablesData.Count;
        if (float.IsNaN(turntablesPercent))
        {
            turntablesPercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.TurntablesGame, turntablesPercent));

        // pirate stats
        int pirateSuccessRounds = 0;
        foreach(var dataPoint in profiles[currentProfile].pirateData)
        {
            if (dataPoint.success)
                pirateSuccessRounds++;
        }
        float piratePercent = (float)pirateSuccessRounds / (float)profiles[currentProfile].pirateData.Count;
        if (float.IsNaN(piratePercent))
        {
            piratePercent = 0f;
        }
        minigameEntries.Add(new GameEntry(GameType.PirateGame, piratePercent));

        // order percents greatest to smallest
        for (int i = 0; i < 6; i++)
        {
            float max = float.MinValue;
            GameEntry currEntry = null;
            
            foreach (var entry in minigameEntries)
            {
                if (entry.rate > max)
                {
                    max = entry.rate;
                    currEntry = entry;
                }
            }

            max *= 100f;
            max = Mathf.Round(max * 10.0f) * 0.1f;
            string gameName = currEntry.gameType.ToString().Replace("Game", "");

            switch (i)
            {
                case 0:
                    minigameEntry1name.text = gameName;
                    minigameEntry1num.text = max.ToString() + "%";
                    break;

                case 1:
                    minigameEntry2name.text = gameName;
                    minigameEntry2num.text = max.ToString() + "%";
                    break;

                case 2:
                    minigameEntry3name.text = gameName;
                    minigameEntry3num.text = max.ToString() + "%";
                    break;

                case 3:
                    minigameEntry4name.text = gameName;
                    minigameEntry4num.text = max.ToString() + "%";
                    break;

                case 4:
                    minigameEntry5name.text = gameName;
                    minigameEntry5num.text = max.ToString() + "%";
                    break;

                case 5:
                    minigameEntry6name.text = gameName;
                    minigameEntry6num.text = max.ToString() + "%";
                    break;
            }

            minigameEntries.Remove(currEntry);
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
