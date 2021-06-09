using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeObject : MonoBehaviour
{
    public static FadeObject instance;

    [SerializeField] private Image vignette;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Plus))
                FadeIn(1.2f);
            if (Input.GetKeyDown(KeyCode.Minus))
                FadeOut(1.2f);
        }
    }
    
    public void FadeIn(float time)
    {
        StartCoroutine(FadeEnumerator(time, true));
    }

    public void FadeOut(float time)
    {
        StartCoroutine(FadeEnumerator(time, false));
    }

    private IEnumerator FadeEnumerator(float time, bool fadeIn)
    {
        float to, from;
        if (fadeIn)
        {
            to = 0f;
            from = 1f;
        }
        else
        {
            to = 1f;
            from = 0f;
        }

        vignette.color = new Color(0f, 0f, 0f, from);

        float timer = 0f;
        while(true)
        {
            if (timer > time)
                break;

            timer += Time.deltaTime;
            vignette.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, (timer / time)));

            yield return null;
        }

        vignette.color = new Color(0f, 0f, 0f, to);
    }
}
