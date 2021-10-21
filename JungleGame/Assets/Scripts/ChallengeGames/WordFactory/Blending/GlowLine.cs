using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowLine : MonoBehaviour
{
    private RectTransform rect;
    public float constWidth;
    public float maxHeight;
    public float timeTolerp;

    void Awake()
    {
        // get rect transform
        if (rect == null)
            rect = GetComponent<RectTransform>();
        // set height to 0
        rect.sizeDelta = new Vector2(constWidth, 0f);
    }

    public void ToggleGlow(bool opt)
    {
        StopAllCoroutines();
        if (opt)
            StartCoroutine(LerpLineHeight(maxHeight));
        else
            StartCoroutine(LerpLineHeight(0f));
    }

    private IEnumerator LerpLineHeight(float targetHeight)
    {
        // get rect transform
        if (rect == null)
            rect = GetComponent<RectTransform>();

        float timer = 0f;
        float startHeight = rect.sizeDelta.y;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > timeTolerp)
            {
                rect.sizeDelta = new Vector2(constWidth, targetHeight);
                break;
            }

            float tempHeight = Mathf.Lerp(startHeight, targetHeight, timer / timeTolerp);
            rect.sizeDelta = new Vector2(constWidth, tempHeight);
            yield return null;
        }
    }
}
