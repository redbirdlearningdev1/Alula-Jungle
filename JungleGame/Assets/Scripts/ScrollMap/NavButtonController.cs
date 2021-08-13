using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavButtonController : MonoBehaviour
{
    public bool isOn = false;
    public Transform glowLine;
    public float lerpDuration;
    private bool mouseOver = false;
    
    void Awake()
    {
        // turn off glow line + turn on glow
        glowLine.localScale = new Vector3(0f, 1f, 1f);
        glowLine.GetComponent<GlowOutlineController>().ToggleGlowOutline(true);
    }

    void OnMouseOver()
    {
        if (!isOn)
            return;

        if (!mouseOver)
        {
            mouseOver = true;
            StopAllCoroutines();
            StartCoroutine(LerpGlowLineScale(1f));
        }
    }

    void OnMouseExit()
    {
        if (!isOn)
            return;

        if (mouseOver)
        {
            mouseOver = false;
            StopAllCoroutines();
            StartCoroutine(LerpGlowLineScale(0f));
        }
    }

    public void TurnOffButton()
    {
        isOn = false;
        StopAllCoroutines();
        StartCoroutine(LerpGlowLineScale(0f));
    }

    private IEnumerator LerpGlowLineScale(float targetX)
    {
        float timer = 0f;
        float startX = glowLine.localScale.x;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpDuration)
            {
                glowLine.localScale = new Vector3(targetX, 1f, 1f);
                break;
            }

            float tempX = Mathf.Lerp(startX, targetX, timer / lerpDuration);
            glowLine.localScale = new Vector3(tempX, 1f, 1f);
            yield return null;
        }
    }
}
