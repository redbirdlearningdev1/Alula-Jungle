using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JungleWelcomeManager : MonoBehaviour
{
    void Awake() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();
    }

    public void OnSkipButtonPressed()
    {
        GameManager.instance.LoadScene("ScrollMap", true);
    }
}
