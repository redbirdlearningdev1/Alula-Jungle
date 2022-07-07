using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class Polaroid : MonoBehaviour
{
    public ChallengeWord challengeWord;
    [SerializeField] private Image pictureImg;
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image frameImage;
    [SerializeField] private GameObject backOfPolaroid;
    [SerializeField] private Transform letterLayoutGroup;
    [SerializeField] private GameObject letterGroupElement;

    public static float FONT_SCALE_DECREASE = 4f;

    void Awake()
    {
        // don't show back of polaroid
        backOfPolaroid.SetActive(false);
    }

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

    public void SetPolaroidAlpha(float alpha, float lerpDuration)
    {
        //backgroundImg.GetComponent<LerpableObject>().LerpImageAlpha(backgroundImg, alpha, lerpDuration);
        pictureImg.GetComponent<LerpableObject>().LerpImageAlpha(pictureImg, alpha, lerpDuration);
        //frameImage.GetComponent<LerpableObject>().LerpImageAlpha(frameImage, alpha, lerpDuration);
    }

    public void ToggleWiggle(bool opt)
    {
        if (opt)
            GetComponent<WiggleController>().StartWiggle();
        else
            GetComponent<WiggleController>().StopWiggle();
    }

    public void ShowPolaroidWord(float startFontSize)
    {
        // reset letter layout group
        foreach (Transform child in letterLayoutGroup)
        {
            Destroy(child.gameObject);
        }

        // set letter group elements
        foreach (string letterGroup in challengeWord.letterGroupList)
        {
            GameObject newElement = Instantiate(letterGroupElement, letterLayoutGroup);
            newElement.GetComponent<TextMeshProUGUI>().text = letterGroup;
            newElement.GetComponent<TextMeshProUGUI>().fontSize = startFontSize - (Polaroid.FONT_SCALE_DECREASE * challengeWord.elkoninCount);
        }

        // show back of polaroid 
        backOfPolaroid.SetActive(true);
    }

    public void HidePolaroidWord()
    {
        // reset letter layout group
        foreach (Transform child in letterLayoutGroup)
        {
            Destroy(child.gameObject);
        }

        // hide back of polaroid 
        backOfPolaroid.SetActive(false);
    }

    public GameObject GetLetterGroupElement(int index)
    {
        int count = 0;
        // return the element according to the index given
        foreach (Transform child in letterLayoutGroup)
        {
            if (count == index)
            {
                return child.gameObject;
            }
            else
            {
                count++;
            }
        }

        return null;
    }
}
