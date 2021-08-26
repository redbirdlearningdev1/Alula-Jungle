using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown navDropdown;
    [SerializeField] private TMP_Dropdown storyGameDropdown;
    [SerializeField] private TMP_Dropdown profileDropdown;

    [SerializeField] private TMP_InputField profileInput;

    private string[] sceneNames = new string[] {  "SplashScene",
                                                    "DevMenu",
                                                    "ScrollMap",
                                                    "TrophyRoomScene",
                                                    "LoadSaveTestScene",
                                                    "StoryGame",
                                                    "FroggerGame",
                                                    "BoatGame",
                                                    "SeaShellGame",
                                                    "TurntablesGame",
                                                    "RummageGame",
                                                    "PirateGame",
                                                    "SpiderwebGame",
                                                    "MinigameDemoScene",
                                                    "WordFactoryBlending",
                                                    "WordFactorySubstituting"};

    private string[] storyGames = new string[] {
        "0 - Welcome",
        "1 - The Prologue"
    };

    private List<StudentPlayerData> datas;                        

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // add all build scenes to navDropdown
        List<string> sceneNamesList = new List<string>(sceneNames);
        navDropdown.AddOptions(sceneNamesList);

        // add story game options to dropdown
        List<string> storyGamesList = new List<string>(storyGames);
        storyGameDropdown.AddOptions(storyGamesList);

        // setup profiles
        profileDropdown.onValueChanged.AddListener(delegate { OnProfileDropdownChanged(); });
        SetupProfileDropdown();
    }

    /* 
    ################################################
    #   PROFILE SECTION
    ################################################
    */

    private void SetupProfileDropdown()
    {
        // refresh database
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        // set profile dropdown
        datas = StudentInfoSystem.GetAllStudentDatas();
        List<string> profileList = new List<string>();
        for (int i = 1; i < 4; i++)
        {
            string str = i + " - ";
            if (datas[i - 1].active)
                str += datas[i - 1].name;
            else
                str += "empty";
            profileList.Add(str);
        }
        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(profileList);
        
        // set current profile

        switch (StudentInfoSystem.GetCurrentProfile().studentIndex)
        {
            case StudentIndex.student_1:
                profileDropdown.value = 0;
                break;
            case StudentIndex.student_2:
                profileDropdown.value = 1;
                break;
            case StudentIndex.student_3:
                profileDropdown.value = 2;
                break;
        }        
    }

    public void OnProfileDropdownChanged()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        int value = profileDropdown.value;
        switch (value)
        {
            case 0:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
                break;
            case 1:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_2);
                break;
            case 2:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_3);
                break;
        }
        // update settings
        SettingsManager.instance.LoadSettingsFromProfile();
    }

    public void OnProfileResetPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(StudentInfoSystem.GetCurrentProfile().studentIndex);
        SettingsManager.instance.LoadSettingsFromProfile();
        SetupProfileDropdown();
    }

    public void OnAllProfilesResetPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(StudentIndex.student_1);
        StudentInfoSystem.ResetProfile(StudentIndex.student_2);
        StudentInfoSystem.ResetProfile(StudentIndex.student_3);
        SetupProfileDropdown();
        StudentInfoSystem.RemoveCurrentStudentPlayer();
    }

    public void OnRenameProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        StudentInfoSystem.GetCurrentProfile().name = profileInput.text;
        StudentInfoSystem.SaveStudentPlayerData();
        SetupProfileDropdown();
    }

    public void OnGiveGoldCoinsPressed()
    {
        // play audio blip
        AudioManager.instance.PlayKeyJingle();
        DropdownToolbar.instance.AwardGoldCoins(999);
    }

    /* 
    ################################################
    #   NAVIGATION SECTION
    ################################################
    */

    public void OnGoToSceneButtonPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        GameManager.instance.LoadScene(sceneNames[navDropdown.value], true);
    }

    public void OnPlayStoryGamePressed()
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        GameManager.instance.SetData(GameManager.instance.storyGameDatas[storyGameDropdown.value]);
        GameManager.instance.LoadScene("StoryGame", true);
    }

    public void OnRestartGamePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);

        GameManager.instance.RestartGame();
    }

    public void OnExitAppPressed()
    {
        StartCoroutine(ExitDelayRoutine());
    }

    private IEnumerator ExitDelayRoutine()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        // fade out to black
        FadeObject.instance.FadeOut(3f);
        yield return new WaitForSeconds(3f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
