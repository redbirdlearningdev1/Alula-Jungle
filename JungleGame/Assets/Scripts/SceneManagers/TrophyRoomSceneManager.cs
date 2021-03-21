using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyRoomSceneManager : MonoBehaviour
{
    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();
    }

    public void OnBackButtonPressed()
    {
        GameManager.instance.LoadScene("ScrollMap", true);
    }
}
