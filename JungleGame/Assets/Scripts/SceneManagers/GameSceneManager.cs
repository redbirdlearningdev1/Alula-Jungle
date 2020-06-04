using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    void Start()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Quiting Application");
        Application.Quit();
    }

    public void SettingsButton()
    {
        Debug.Log("Settings Page");
        settingsPanel.SetActive(true);
    }

    public void BackToMenuButton()
    {
        Debug.Log("Back to menu button");
        settingsPanel.SetActive(false);
    }


}
