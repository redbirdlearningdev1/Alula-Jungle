using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    void Awake() 
    {
        FadeHelper.FadeIn(1.2f);
    }

    void Update() 
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            StartCoroutine(LeaveScene());
        }
    }

    private IEnumerator LeaveScene()
    {
        FadeHelper.FadeOut(1.2f);
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("ScrollMap");
    }
}
