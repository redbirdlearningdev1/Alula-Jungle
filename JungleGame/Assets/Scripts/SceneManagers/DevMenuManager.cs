using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown navDropdown;

    void Awake()
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();

        // add all build scenes to navDropdown
        List<string> sceneNames = new List<string>();
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes)
        {
            string name = Path.GetFileNameWithoutExtension(scene.path);
            sceneNames.Add(name);
        }  
        navDropdown.AddOptions(sceneNames);
    }

    public void OnGoToSceneButtonPressed()
    {
        GameHelper.LoadScene(navDropdown.value, true);
    }

    public void OnRestartGamePressed()
    {
        GameHelper.RestartGame();
    }

    public void OnExitAppPressed()
    {
        Application.Quit();
    }
}
