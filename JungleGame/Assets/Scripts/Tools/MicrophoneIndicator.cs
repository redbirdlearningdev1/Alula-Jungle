using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneIndicator : MonoBehaviour
{
    [Header("Mic Sprites")]
    public Sprite micBlue;
    public Sprite micGreen;
    public Sprite micRed;

    [Header("Components")]
    public Image image;
    public LerpableObject lerpObj;
    public WiggleController wiggleController;

    void Awake()
    {
        image.sprite = micBlue;
        lerpObj.LerpScale(new Vector2(0f, 0f), 0f);
    }

    public void ShowIndicator()
    {
        image.sprite = micBlue;
        wiggleController.StartWiggle();
        lerpObj.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
    }

    public void AudioInputDetected()
    {
        image.sprite = micGreen;
        wiggleController.StopWiggle();
        lerpObj.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
    }

    public void NoInputDetected()
    {
        image.sprite = micRed;
        wiggleController.StopWiggle();
        lerpObj.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
    }

    public void HideIndicator()
    {
        lerpObj.SquishyScaleLerp(new Vector2(1.35f, 1.35f), new Vector2(0f, 0f), 0.2f, 0.2f);
    }
}
