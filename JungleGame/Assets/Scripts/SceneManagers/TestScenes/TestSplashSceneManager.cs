using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSplashSceneManager : MonoBehaviour
{
    public Button goToSplashSceneButton;
    public CanvasGroup fadeInCanvasGroup;
    public CanvasGroup loadingScreenCanvasGroup;
    public float loadFadeInTime;
    public WiggleController textWiggleController;
    public Animator gorillaWalkAnimator;

    void Awake()
    {
        textWiggleController.StartWiggle();
        StartCoroutine(FadeIntoScene());
    }

    private IEnumerator FadeIntoScene()
    {
        fadeInCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        float timer = 0f;
        while (timer < loadFadeInTime)
        {
            timer += Time.deltaTime;
            fadeInCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / loadFadeInTime);
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 0f;
    }

    public void OnGoToSplashSceneButtonPressed()
    {
        textWiggleController.StopWiggle();
        goToSplashSceneButton.interactable = false;
        StartCoroutine(ShowLoadingScreen());
    }

    private IEnumerator ShowLoadingScreen()
    {
        float timer = 0f;
        while (timer < loadFadeInTime)
        {
            timer += Time.deltaTime;
            loadingScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / loadFadeInTime);
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 1f;

        // start gorilla walk animation
        gorillaWalkAnimator.Play("GorillaWalkAcrossScreen");

        // load splash scene after short delay
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("SplashScene");
    }
}
