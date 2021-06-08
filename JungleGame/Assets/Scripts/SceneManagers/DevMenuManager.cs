using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown navDropdown;

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

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // add all build scenes to navDropdown
        List<string> sceneNamesList = new List<string>(sceneNames);
        navDropdown.AddOptions(sceneNamesList);
    }

    public void OnGoToSceneButtonPressed()
    {
        GameManager.instance.LoadScene(navDropdown.value, true);
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
