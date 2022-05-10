using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestSplashSceneManager : MonoBehaviour
{
    public CanvasGroup fadeInCanvasGroup;
    public float loadFadeInTime;
    public LerpableObject titleLogo;

    void Awake()
    {
        StartCoroutine(FadeIntoScene());
    }

    private IEnumerator FadeIntoScene()
    {
        fadeInCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        titleLogo.LerpScale(new Vector2(1.2f, 1.2f), 2f);

        float timer = 0f;
        while (timer < loadFadeInTime)
        {
            timer += Time.deltaTime;
            fadeInCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / loadFadeInTime);
            yield return null;
        }
        SceneManager.LoadSceneAsync("SplashScene");
    }
}
