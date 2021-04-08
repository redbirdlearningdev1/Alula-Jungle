using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToolbar : MonoBehaviour
{
    /* 
    ################################################
    #   UI BUTTONS 
    ################################################
    */

    public void OnSettingsButtonPressed()
    {
        GameManager.instance.LoadScene("SettingsScene", true);
    }

    public void OnTrophyRoomButtonPressed()
    {
        GameManager.instance.LoadScene("TrophyRoomScene", true);
    }

    public void OnQuitButtonPressed()
    {
        FadeObject.instance.FadeOut(3f);
        Application.Quit();
    }
}
