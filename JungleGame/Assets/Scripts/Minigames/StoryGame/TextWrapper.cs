using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class TextWrapper : MonoBehaviour
{
    public bool isOn;
    public float constHeight;
    public float sizePadding;
    public ActionWordEnum wordEnum;

    private const float colorTransTime = 0.5f;
    private RectTransform rectTransform;
    private TextMeshProUGUI textMesh;
    private bool sizeChanging = false;

    // space stuff
    private bool isSpace;
    private const float spaceWidth = 15f;

    void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // return iff off
        if (!isOn)
            return;
        
        // add padding to width if text is growing in size over time
        if (sizeChanging)
            rectTransform.sizeDelta = new Vector2(textMesh.preferredWidth + sizePadding, constHeight);
        else 
        // no padding
            rectTransform.sizeDelta = new Vector2(textMesh.preferredWidth, constHeight);
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
    }

    public void SetSpace()
    {
        textMesh.text = "";
        isOn = false;
        rectTransform.sizeDelta = new Vector2(spaceWidth, constHeight);
    }

    public void SetTextColor(Color color, bool smoothTrans)
    {
        if (!smoothTrans)
            textMesh.color = color;
        else
            StartCoroutine(SmoothColorTransRoutine(color));
    }

    private IEnumerator SmoothColorTransRoutine(Color newColor)
    {
        float timer = 0f;
        Color startColor = textMesh.color;
        Color endColor = newColor;
        
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > colorTransTime)
            {
                textMesh.color = endColor;
                break;
            }

            Color temp = Color.Lerp(startColor, endColor, timer / colorTransTime);
            textMesh.color = temp;
            yield return null;
        }
    }

    public void SetTextSize(float size, bool smoothTrans)
    {
        if (!smoothTrans)
            textMesh.fontSize = size;
        else
            StartCoroutine(SmoothSizeRoutine(size));
    }

    private IEnumerator SmoothSizeRoutine(float newSize)
    {
        float timer = 0f;
        float startSize = textMesh.fontSize;
        float endSize = newSize;
        sizeChanging = true;

        yield return new WaitForSeconds(0.1f);
        
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > colorTransTime)
            {
                textMesh.fontSize = endSize;
                sizeChanging = false;
                break;
            }

            float temp = Mathf.Lerp(startSize, endSize, timer / colorTransTime);
            textMesh.fontSize = temp;
            yield return null;
        }
    }
}
