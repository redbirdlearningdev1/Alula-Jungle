using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class TextWrapper : MonoBehaviour
{
    public bool isOn;
    public float constHeight;
    public ActionWordEnum wordEnum;

    private const float colorTransTime = 0.5f;
    private RectTransform rectTransform;
    private TextMeshProUGUI textMesh;

    void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(textMesh.preferredWidth, constHeight);
    }
    
    public void SetText(string text)
    {
        textMesh.text = text;
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
}
