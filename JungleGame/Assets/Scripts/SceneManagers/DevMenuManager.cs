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

    private string[] sceneNames = new string[] {  "SplashScreen", 
                                                    "SettingsScene", 
                                                    "DevMenu",
                                                    "JungleWelcomeScene",
                                                    "ScrollMap",
                                                    "TrophyRoomScene",
                                                    "LoadSaveTestScene",
                                                    "AudioInputTestScene",
                                                    "StoryGame",
                                                    "FroggerGame",
                                                    "BoatGame",
                                                    "RummageGame",
                                                    "SeashellGame",
                                                    "SpiderwebGame",
                                                    "TurntablesGame",
                                                    "MinigameDemoScene"};

    private string[] storyGames = new string[] {
        "0 - Welcome",
        "1 - The Prologue"
    };                                  

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
    }

    public void OnGoToSceneButtonPressed()
    {
        GameManager.instance.LoadScene(navDropdown.value, true);
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
