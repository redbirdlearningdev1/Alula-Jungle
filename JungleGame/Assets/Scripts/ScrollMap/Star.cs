using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    private Image image;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    public float lerpSpeed;
    public BobController bobController;

    public void SetStar(bool full)
    {
        // get sprite renderer if null
        if (image == null)
            image = GetComponent<Image>();

        if (full)
        {
            image.sprite = fullStar;
        }
        else
        {
            image.sprite = emptyStar;
        }    
    }

    public void LerpStarAlphaScale(float targetAlpha, float targetScale)
    {
        // get sprite renderer if null
        if (image == null)
            image = GetComponent<Image>();

        StopAllCoroutines();
        StartCoroutine(LerpStarAlphaRoutine(targetAlpha));
        StartCoroutine(LerpStarScaleRoutine(targetScale));
    }

    private IEnumerator LerpStarAlphaRoutine(float targetAlpha)
    {
        float startAlpha = image.color.a;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpSpeed)
            {
                image.color = new Color(1f, 1f, 1f, targetAlpha);
                break;
            }

            float tempAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / lerpSpeed);
            image.color = new Color(1f, 1f, 1f, tempAlpha);

            yield return null;
        }
    }

    private IEnumerator LerpStarScaleRoutine(float targetScale)
    {
        float startScale = transform.localScale.x;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpSpeed)
            {
                transform.localScale = new Vector3(targetScale, targetScale, 1f);
                break;
            }

            float tempScale = Mathf.Lerp(startScale, targetScale, timer / lerpSpeed);
            transform.localScale = new Vector3(tempScale, tempScale, 1f);

            yield return null;
        }
    }
}
