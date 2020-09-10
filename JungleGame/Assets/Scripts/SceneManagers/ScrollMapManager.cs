using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMapManager : MonoBehaviour
{
    void Awake()
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();
    }

    public void OnSettingsButtonPressed()
    {
        GameHelper.LoadScene("SettingsScene", true);
    }

    public void OnTrophyRoomButtonPressed()
    {
        GameHelper.LoadScene("TrophyRoomScene", true);
    }
}
