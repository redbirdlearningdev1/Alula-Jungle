using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterboxController : MonoBehaviour
{
    public static LetterboxController instance;

    public float boxHeight;
    public float moveTime;

    [SerializeField] private RectTransform topRect;
    [SerializeField] private RectTransform bottomRect;
    private bool isOn;


    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ToggleLetterbox(bool opt)
    {
        if (opt == isOn)
            return;
        
        isOn = opt;
        StartCoroutine(ToggleLetterboxRoutine(opt));
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
