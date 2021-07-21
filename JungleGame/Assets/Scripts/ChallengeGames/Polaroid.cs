using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Polaroid : MonoBehaviour
{
    public ChallengeWord challengeWord;
    [SerializeField] private SpriteRenderer picture;
    [SerializeField] private SpriteRenderer background;

    public void SetPolaroid(ChallengeWord word)
    {
        challengeWord = word;

        // set picture
        picture.sprite = word.sprite;
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

    public void SetLayer(int layer)
    {
        background.sortingOrder = layer;
        picture.sortingOrder = layer + 1;
    }

    public void ToggleGlowOutline(bool opt)
    {
        background.GetComponent<GlowOutlineController>().ToggleGlowOutline(opt);
    }

    public void SetGlowColor(Color color)
    {
        background.GetComponent<GlowOutlineController>().SetColor(color);
    }
}
