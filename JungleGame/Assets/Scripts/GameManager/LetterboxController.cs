using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterboxController : MonoBehaviour
{
    public static LetterboxController instance;

    public float boxHeight;
    public float moveTime;
    public float textRevealTime;

    [SerializeField] private RectTransform topRect;
    [SerializeField] private RectTransform bottomRect;
    [SerializeField] private TextMeshProUGUI titleText;

    private bool isOn;


    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }

        titleText.text = "";
        titleText.color = new Color(1f, 1f, 1f, 0f);
    }

    public void ToggleLetterbox(bool opt)
    {
        if (opt == isOn)
            return;
        
        isOn = opt;
        StartCoroutine(ToggleLetterboxRoutine(opt));
    }

    public void ShowTextSmooth(string text)
    {
        titleText.color = new Color(1f, 1f, 1f, 0f);
        titleText.text = text;

        StartCoroutine(SmoothShowTextRoutine());
    }

    private IEnumerator SmoothShowTextRoutine()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > textRevealTime)
            {
                titleText.color = new Color(1f, 1f, 1f, 1f);
                break;
            }

            float temp = Mathf.Lerp(0f, 1f, timer / textRevealTime);
            titleText.color = new Color(1f, 1f, 1f, temp);

            yield return null;
        }
    }

    private IEnumerator ToggleLetterboxRoutine(bool opt)
    {
        float timer = 0f;

        float topStart, topEnd, bottomStart, bottomEnd;
        if (opt)
        {
            topStart = boxHeight;
            topEnd = 0f;

            bottomStart = boxHeight * -1f;
            bottomEnd = 0f;
        }
        else 
        {
            topStart = 0f;
            topEnd = boxHeight;

            bottomStart = 0f;
            bottomEnd = boxHeight * -1f;
        }

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > moveTime)
            {   
                // reset text
                if (!opt)
                {
                    titleText.text = "";
                    titleText.color = new Color(1f, 1f, 1f, 0f);
                }

                break;
            }

            float tempHeightTop = Mathf.Lerp(topStart, topEnd, timer / moveTime);
            float tempHeightBottom = Mathf.Lerp(bottomStart, bottomEnd, timer / moveTime);

            topRect.anchoredPosition3D = new Vector3(0f, tempHeightTop, 0f);
            bottomRect.anchoredPosition3D = new Vector3(0f, tempHeightBottom, 0f);

            yield return null;
        }
    }
}
