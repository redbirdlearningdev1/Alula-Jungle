using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpableObject : MonoBehaviour
{
    private Coroutine scaleRoutine;
    private Coroutine posRoutine;

    public void LerpScale(Vector2 targetScale, float duration)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(LerpScaleRoutine(targetScale, duration));
    }

    private IEnumerator LerpScaleRoutine(Vector2 targetScale, float duration)
    {
        Vector2 startScale = transform.localScale;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                transform.localScale = new Vector3(targetScale.x, targetScale.y, 1f);
                break;
            }

            Vector2 tempScale = Vector2.Lerp(startScale, targetScale, timer / duration);
            transform.localScale = tempScale;
            yield return null;
        }
    }

    public void LerpPosition(Vector2 targetPos, float duration, bool localPosition)
    {
        if (posRoutine != null)
            StopCoroutine(posRoutine);

        posRoutine = StartCoroutine(LerpPosRoutine(targetPos, duration, localPosition));
    }

    private IEnumerator LerpPosRoutine(Vector2 targetPos, float duration, bool localPosition)
    {
        Vector2 startPos;
        if (localPosition) startPos = transform.localPosition;
        else startPos = transform.position;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                if (localPosition) transform.localPosition = targetPos;
                else transform.position = targetPos;
                break;
            }

            Vector2 tempPos = Vector2.Lerp(startPos, targetPos, timer / duration);

            if (localPosition) transform.localPosition = tempPos;
            else transform.position = tempPos;
            yield return null;
        }
    }
}
