using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;

[RequireComponent(typeof(SpriteGlowEffect))]
public class GlowOutlineController : MonoBehaviour
{
    public bool deactivated;
    
    private SpriteGlowEffect spriteGlow;
    public float glowTime = 0.2f;
    public float brightnessOn = 3f;
    public float brightnessOff = 0f;
    public int outlineOn = 1;
    public int outlineOff = 0;

    private Coroutine currRoutine;
    private bool animating = false;
    private bool isGlowing = false;
    private bool customGlow = false;

    private const float lerpGlowSettingsTime = 0.5f;

    void Awake()
    {
        spriteGlow = GetComponent<SpriteGlowEffect>();
    }

    public void SetGlowSettings(float brightness, int outline, Color color, bool lerpAnim)
    {
        customGlow = true;

        if (lerpAnim)
        {
            StopCoroutine(currRoutine);
            currRoutine = StartCoroutine(LerpGlowSettingsRoutine(brightness, outline, color, lerpGlowSettingsTime));
            return;
        }
        spriteGlow.GlowBrightness = brightness;
        spriteGlow.OutlineWidth = outline;
        spriteGlow.GlowColor = color;
    }

    public void SetColor(Color newColor)
    {
        StartCoroutine(LerpGlowColor(newColor, 0.25f));
    }

    private IEnumerator LerpGlowColor(Color color, float time)
    {
        float timer = 0;
        Color startColor = spriteGlow.GlowColor;

         while (true) 
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                spriteGlow.GlowColor = color;
                break;
            }

            var tempColor = Color.Lerp(startColor, color, timer / time);
            spriteGlow.GlowColor = tempColor;

            yield return null;
        }
    }

    private IEnumerator LerpGlowSettingsRoutine(float brightness, int outline, Color color, float time)
    {
        float timer = 0;
        float startBrightness = spriteGlow.GlowBrightness;
        Color startColor = spriteGlow.GlowColor;
        spriteGlow.OutlineWidth = outline;

        while (true) 
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                spriteGlow.GlowBrightness = brightness;
                spriteGlow.GlowColor = color;
                break;
            }

            var tempGlow = Mathf.Lerp(startBrightness, brightness, timer / time);
            var tempColor = Color.Lerp(startColor, color, timer / time);

            spriteGlow.GlowBrightness = tempGlow;
            spriteGlow.GlowColor = tempColor;

            yield return null;
        }
    }

    public void ToggleGlowOutline(bool opt)
    {
        if (customGlow) return;
        if (currRoutine != null)
            StopCoroutine(currRoutine);
        animating = false;
        isGlowing = opt;
    }

    void Update()
    {
        // do nothing if deactivated
        if (deactivated) return;

        // only start routine if not animating
        if (!animating)
        {
            if (isGlowing)
                currRoutine = StartCoroutine(ToggleGlowOutlineRoutine(true));
            else 
                currRoutine = StartCoroutine(ToggleGlowOutlineRoutine(false));
            animating = true;
        }   
    }

    private IEnumerator ToggleGlowOutlineRoutine(bool opt)
    {
        float startBrightness, endBrightness;

        // no glow -> glow
        if (opt)
        {
            startBrightness = brightnessOff;
            endBrightness = brightnessOn;
            spriteGlow.OutlineWidth = outlineOn;
        }
        // glow -> no glow
        else
        {
            startBrightness = brightnessOn;
            endBrightness = brightnessOff;
        }
        
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > glowTime)
            {
                spriteGlow.GlowBrightness = endBrightness;
                isGlowing = opt;
                // remove outline when off
                if (!opt)
                    spriteGlow.OutlineWidth = outlineOff;
                break;
            }

            float brightness = Mathf.Lerp(startBrightness, endBrightness, timer / glowTime);

            spriteGlow.GlowBrightness = brightness;
            yield return null;
        }
    }   
}
