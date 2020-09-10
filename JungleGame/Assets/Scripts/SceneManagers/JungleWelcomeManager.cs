using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JungleWelcomeManager : MonoBehaviour
{
    void Awake() 
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();
    }

    public void OnSkipButtonPressed()
    {
        GameHelper.LoadScene("ScrollMap", true);
    }
}
