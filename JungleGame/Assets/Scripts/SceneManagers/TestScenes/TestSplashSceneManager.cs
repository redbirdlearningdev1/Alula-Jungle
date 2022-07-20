using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSplashSceneManager : MonoBehaviour
{
    public LerpableObject consentToAnalyticsWindow;

    void Awake()
    {
        // determine if first time running new build
        ResourceRequest request = Resources.LoadAsync("Build", typeof(BuildScriptableObject));
        request.completed += RequestCompleted;  
    }

    private void RequestCompleted(AsyncOperation obj)
    {
        BuildScriptableObject buildScriptableObject = ((ResourceRequest)obj).asset as BuildScriptableObject;
        
        if (buildScriptableObject == null)
        {
            Debug.LogError("build scriptable object not found in resources folder.");
        }
        else
        {
            // check to see if player has a build number stored in player prefs
            if (PlayerPrefs.HasKey("BUILD_NUMBER"))
            {
                // if yes - check if player pref build number is equal to application's build number
                if (PlayerPrefs.GetString("BUILD_NUMBER") == buildScriptableObject.buildNumber)
                {
                    //Debug.LogError("BUILD NUMBER EQUAL - LOADING GAME");
                    SceneManager.LoadSceneAsync("SplashScene");
                }
                // if not equal - open consent window and set build number
                else
                {
                    //Debug.LogError("NEW BUILD DETECTED - ASKING FOR CONSENT");
                    PlayerPrefs.SetString("BUILD_NUMBER", buildScriptableObject.buildNumber);
                    consentToAnalyticsWindow.transform.localScale = Vector3.zero;
                    StartCoroutine(OpenAskForConsentRoutine());
                }
            }
            // no build number stored in player prefs
            else
            {
                //Debug.LogError("FIRST TIME NEW BUILD DETECTED - ASKING FOR CONSENT");
                PlayerPrefs.SetString("BUILD_NUMBER", buildScriptableObject.buildNumber);
                consentToAnalyticsWindow.transform.localScale = Vector3.zero;
                StartCoroutine(OpenAskForConsentRoutine());
            }
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
