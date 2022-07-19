using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSplashSceneManager : MonoBehaviour
{
    public bool resetPlayerPrefs;
    public LerpableObject consentToAnalyticsWindow;

    void Awake()
    {
        if (resetPlayerPrefs)
            PlayerPrefs.DeleteAll();

        if (!PlayerPrefs.HasKey("USE_ANALYTICS"))
        {
            Debug.LogError("FIRST TIME!");
            consentToAnalyticsWindow.transform.localScale = Vector3.zero;
            StartCoroutine(OpenAskForConsentRoutine());
        }
        else
        {
            Debug.LogError("NOT FIRST TIME!");
            SceneManager.LoadSceneAsync("SplashScene");
        }   
    }

    private IEnumerator OpenAskForConsentRoutine()
    {
        yield return new WaitForSeconds(1f);
        consentToAnalyticsWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
    }

    private IEnumerator CloseAskForConsentRoutine()
    {
        consentToAnalyticsWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("SplashScene");
    }

    public void OnYesPressed()
    {
        PlayerPrefs.SetInt("USE_ANALYTICS", 1);
        StartCoroutine(CloseAskForConsentRoutine());
    }

    public void OnNoPressed()
    {
        PlayerPrefs.SetInt("USE_ANALYTICS", 0);
        StartCoroutine(CloseAskForConsentRoutine());
    }
}
