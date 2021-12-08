using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Polaroid : MonoBehaviour
{
    public ChallengeWord challengeWord;
    [SerializeField] private Image pictureImg;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image filerImage;
    [SerializeField] private Image frameImage;

    public void SetPolaroid(ChallengeWord word)
    {
        challengeWord = word;

        // set picture
        pictureImg.sprite = word.sprite;
    }

    public void MovePolaroid(Vector3 position, float lerpTime)
    {
        StartCoroutine(MovePolaroidRoutine(position, lerpTime));
    }

    private IEnumerator MovePolaroidRoutine(Vector3 endPosition, float lerpTime)
    {
        Vector3 startPosition = transform.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                transform.position = endPosition;
                break;
            }

            var tempPos = Vector3.Lerp(startPosition, endPosition, timer / lerpTime);
            transform.position = tempPos;
            yield return null;
        }
    }

    public void LerpScale(float targetScale, float lerpTime)
    {
        StartCoroutine(LerpScaleRoutine(targetScale, lerpTime));
    }

    private IEnumerator LerpScaleRoutine(float targetScale, float lerpTime)
    {
        float startscale = transform.localScale.x;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                transform.localScale = new Vector3(targetScale, targetScale, 1f);
                break;
            }

            var tempScale = Mathf.Lerp(startscale, targetScale, timer / lerpTime);
            transform.localScale = new Vector3(tempScale, tempScale, 1f);
            yield return null;
        }
    }

    public void LerpRotation(float targetAngle, float lerpTime)
    {
        StartCoroutine(LerpRotationRoutine(targetAngle, lerpTime));
    }

    private IEnumerator LerpRotationRoutine(float targetAngle, float lerpTime)
    {
        float startAngle = transform.rotation.eulerAngles.z;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                break;
            }

            var tempAngle = Mathf.LerpAngle(startAngle, targetAngle, timer / lerpTime);
            transform.rotation = Quaternion.Euler(0f, 0f, tempAngle);
            yield return null;
        }
    }

    public void HideImage(float lerpAlphaDuration)
    {
        pictureImg.GetComponent<LerpableObject>().LerpImageAlpha(pictureImg, 0f, lerpAlphaDuration);
    }

    public void RevealImage(float lerpAlphaDuration)
    {
        pictureImg.GetComponent<LerpableObject>().LerpImageAlpha(pictureImg, 1f, lerpAlphaDuration);
    }

    public void ToggleGlowOutline(bool opt)
    {
        if (opt)
            ImageGlowController.instance.SetImageGlow(backgroundImg, true, GlowValue.glow_1_025);
        else
            ImageGlowController.instance.SetImageGlow(backgroundImg, false);
    }

    public void SetPolaroidAlpha(float alpha, float lerpDuration)
    {
        backgroundImg.GetComponent<LerpableObject>().LerpImageAlpha(backgroundImg, alpha, lerpDuration);
        pictureImg.GetComponent<LerpableObject>().LerpImageAlpha(pictureImg, alpha, lerpDuration);
        filerImage.GetComponent<LerpableObject>().LerpImageAlpha(filerImage, alpha, lerpDuration);
        frameImage.GetComponent<LerpableObject>().LerpImageAlpha(frameImage, alpha, lerpDuration);
    }

    public void ToggleWiggle(bool opt)
    {
        if (opt)
            GetComponent<WiggleController>().StartWiggle();
        else
            GetComponent<WiggleController>().StopWiggle();
    }
}
