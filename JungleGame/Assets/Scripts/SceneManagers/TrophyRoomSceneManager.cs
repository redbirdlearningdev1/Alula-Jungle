using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyRoomSceneManager : MonoBehaviour
{
    void Awake()
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();
    }

    public void OnBackButtonPressed()
    {
        GameHelper.LoadScene("ScrollMap", true);
    }
}
