using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehindUIBackground : MonoBehaviour
{
    public static BehindUIBackground instance;

    [SerializeField] private Image image;

    public float transitionTime;
    public float maxAlpha;

    private bool isUp;

    void Awake()
    {
        if (instance == null)
            instance = this;

        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;
        isUp = false;
    }

    public void Activate()
    {   
        if (isUp)
            return;

        StopAllCoroutines();
        StartCoroutine(SmoothSetImage(maxAlpha));
        isUp = true;
        image.raycastTarget = true;
    }

    public void Deactivate()
    {
        if (!isUp)
            return;
        
        StopAllCoroutines();
        StartCoroutine(SmoothSetImage(0f));
        isUp = false;
        image.raycastTarget = false;
    }


    private IEnumerator SmoothSetImage(float target)
    {
        float start = image.color.a;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > transitionTime)
            {
                image.color = new Color(0f, 0f, 0f, target);
                break;
            }

            float tempAlpha = Mathf.Lerp(start, target, timer / transitionTime);
            image.color = new Color(0f, 0f, 0f, tempAlpha);
            yield return null;
        }
    }
}
