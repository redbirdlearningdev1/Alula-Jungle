using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsSceneManager : MonoBehaviour
{
    [Header("Variables")]
    public float scrollSpeed;
    public float creditBufferTime;
    public float particleInterval;
    public float fastForwardWaitTime;

    [Header("FastForwardButton")]
    public LerpableObject fastForwardButton;

    [Header("ScrollingBox")]
    public GameObject scrollingBox;

    [Header("Animators")]
    public Animator Lester;
    public Animator Coin;

    private RectTransform scrollRect;
    private bool isScrolling;
    private float screenHeight;

    void Awake()
    {
        GameManager.instance.SceneInit();
        AudioManager.instance.StopMusic();
        fastForwardButton.transform.localScale = Vector3.zero;
    }

    public void Start()
    {

        float targetaspect = 1800f / 1200f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        if (scaleheight < 1.0f)
        {
            screenHeight = 1200f;
        }
        else
        {
            screenHeight = Screen.height;
        }

        AudioManager.instance.PlaySong(AudioDatabase.instance.SplashScreenSong);
        isScrolling = true;
        Vector3 scrollPos = scrollingBox.transform.localPosition;
        scrollRect = scrollingBox.GetComponent<RectTransform>();

        scrollingBox.transform.localPosition = new Vector3(scrollPos.x, (scrollRect.rect.height / -2.0f) + (screenHeight / 2.0f), scrollPos.z);

        Lester.Play("geckoIdle");
        Coin.Play("hello");

        ParticleController.instance.isOn = true;
        ParticleController.instance.IncreaseCharacterParticle();

        StartCoroutine(ScrollCredits());
        StartCoroutine(FFButton());
    }

    IEnumerator FFButton()
    {
        yield return new WaitForSeconds(fastForwardWaitTime);
        fastForwardButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.2f, 0.2f);
        fastForwardButton.GetComponent<WiggleController>().StartWiggle();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
    }


    IEnumerator ScrollCredits()
    {
        yield return new WaitForSeconds(creditBufferTime);

        float timePassed = 0f;

        while (isScrolling)
        {
            timePassed += Time.deltaTime;

            if (timePassed >= particleInterval)
            {
                timePassed = 0f;
                ParticleController.instance.IncreaseCharacterParticle();
            }

            scrollingBox.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed);
            if (scrollingBox.transform.localPosition.y > scrollRect.rect.height / 2.0f)
            {
                isScrolling = false;
                GameManager.instance.RestartGame();
            }

            yield return null;
        }
    }

    public void OnFastForwardButtonPressed()
    {
        AudioManager.instance.StopTalk();
        AudioManager.instance.StopMusic();

        fastForwardButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.2f, 0.2f);
        fastForwardButton.GetComponent<WiggleController>().StopWiggle();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.FastForwardSound, 0.5f);
        OnReturnToSplashScreenPressed();
    }


    public void OnReturnToSplashScreenPressed()
    {
        GameManager.instance.RestartGame();
    }
}
