using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown navDropdown;
    [SerializeField] private TMP_Dropdown storyGameDropdown;
    [SerializeField] private TMP_Dropdown profileDropdown;

    [SerializeField] private TMP_InputField profileInput;

    private string[] sceneNames = new string[] {  "SplashScreen",
                                                    "DevMenu",
                                                    "JungleWelcomeScene",
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
                                                    "MinigameDemoScene"};

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
        if (StudentInfoSystem.currentStudentPlayer == null)
            StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
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
        AssetDatabase.Refresh();

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
        //print ("student index: " + StudentInfoSystem.currentStudentPlayer.studentIndex);
        switch (StudentInfoSystem.currentStudentPlayer.studentIndex)
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
    }

    public void OnProfileResetPressed()
    {
        StudentInfoSystem.ResetProfile(StudentInfoSystem.currentStudentPlayer.studentIndex);
        SetupProfileDropdown();
    }

    public void OnAllProfilesResetPressed()
    {
        StudentInfoSystem.ResetProfile(StudentIndex.student_1);
        StudentInfoSystem.ResetProfile(StudentIndex.student_2);
        StudentInfoSystem.ResetProfile(StudentIndex.student_3);
        SetupProfileDropdown();
    }

    public void OnRenameProfilePressed()
    {
        StudentInfoSystem.currentStudentPlayer.name = profileInput.text;
        StudentInfoSystem.SaveStudentPlayerData();
        SetupProfileDropdown();
    }   

    /* 
    ################################################
    #   NAVIGATION SECTION
    ################################################
    */

    public void OnGoToSceneButtonPressed()
    {
        GameManager.instance.LoadScene(sceneNames[navDropdown.value], true);
    }

    public void OnPlayStoryGamePressed()
    {   
        GameManager.instance.SetData(GameManager.instance.storyGameDatas[storyGameDropdown.value]);
        GameManager.instance.LoadScene("StoryGame", true);
    }

    public void OnRestartGamePressed()
    {
        GameManager.instance.RestartGame();
    }

    public void OnExitAppPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
