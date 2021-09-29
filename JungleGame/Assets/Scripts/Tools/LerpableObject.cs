using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpableObject : MonoBehaviour
{
    private Coroutine scaleRoutine;
    private Coroutine squishyRoutine;
    private Coroutine posRoutine;
    private Coroutine colorRoutine;
    private Coroutine followTransformRoutine;

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

    public void LerpPosToTransform(Transform followTransform, float duration, bool localPosition)
    {
        if (followTransformRoutine != null)
            StopCoroutine(followTransformRoutine);

        followTransformRoutine = StartCoroutine(LerpPosToTransformRoutine(followTransform, duration, localPosition));
    }

    private IEnumerator LerpPosToTransformRoutine(Transform followTransform, float duration, bool localPosition)
    {
        float timer = 0f;
        Vector3 startPos;
        if (localPosition) startPos = transform.localPosition;
        else startPos = transform.position; 

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                if (localPosition) transform.localPosition = followTransform.position;
                else transform.position = followTransform.position;
                break;
            }

            Vector3 tempPos;
            if (localPosition) tempPos = Vector3.Lerp(startPos, followTransform.position, timer / duration);
            else tempPos = Vector3.Lerp(startPos, followTransform.position, timer / duration);

            if (localPosition) transform.localPosition = tempPos;
            else transform.position = tempPos;

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

    public void LerpYPos(float targetY, float duration, bool localPosition)
    {
        if (posRoutine != null)
            StopCoroutine(posRoutine);

        posRoutine = StartCoroutine(LerpYPosRoutine(targetY, duration, localPosition));
    }

    private IEnumerator LerpYPosRoutine(float targetY, float duration, bool localPosition)
    {
        float startY;
        if (localPosition) startY = transform.localPosition.y;
        else startY = transform.position.y;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                if (localPosition) transform.localPosition = new Vector3(transform.localPosition.x, targetY, 1f);
                else transform.position = new Vector3(transform.position.x, targetY, 1f);
                break;
            }

            float tempY = Mathf.Lerp(startY, targetY, timer / duration);

            if (localPosition) transform.localPosition = new Vector3(transform.localPosition.x, tempY, 1f);
                else transform.position = new Vector3(transform.position.x, tempY, 1f);
            yield return null;
        }
    }

    public void LerpXPos(float targetX, float duration, bool localPosition)
    {
        if (posRoutine != null)
            StopCoroutine(posRoutine);

        posRoutine = StartCoroutine(LerpXPosRoutine(targetX, duration, localPosition));
    }

    private IEnumerator LerpXPosRoutine(float targetX, float duration, bool localPosition)
    {
        float startX;
        if (localPosition) startX = transform.localPosition.x;
        else startX = transform.position.x;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                if (localPosition) transform.localPosition = new Vector3(targetX, transform.localPosition.y, 1f);
                else transform.position = new Vector3(targetX, transform.position.y, 1f);
                break;
            }

            float tempX = Mathf.Lerp(startX, targetX, timer / duration);

            if (localPosition) transform.localPosition = new Vector3(tempX, transform.localPosition.y, 1f);
                else transform.position = new Vector3(tempX, transform.position.y, 1f);
            yield return null;
        }
    }

    public void SquishyScaleLerp(Vector2 maxScale, Vector2 normalScale, float maxSpeed, float normalSpeed)
    {
        if (squishyRoutine != null)
        {
            StopCoroutine(squishyRoutine);
            squishyRoutine = null;
            
            scaleRoutine = StartCoroutine(LerpScaleRoutine(normalScale, normalSpeed));
            return;
        }

        squishyRoutine = StartCoroutine(SquishyScaleLerpRoutine(maxScale, normalScale, maxSpeed, normalSpeed));
    }

    private IEnumerator SquishyScaleLerpRoutine(Vector2 maxScale, Vector2 normalScale, float maxSpeed, float normalSpeed)
    {
        LerpScale(maxScale, maxSpeed);
        yield return new WaitForSeconds(maxSpeed);
        LerpScale(normalScale, normalSpeed);
        yield return new WaitForSeconds(normalSpeed);
        squishyRoutine = null;
    }

    public void SetImageAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    public void LerpImageAlpha(Image image, float alpha, float duration)
    {
        if (colorRoutine != null)
            StopCoroutine(colorRoutine);

        colorRoutine = StartCoroutine(LerpImageAlphaRoutine(image, alpha, duration));
    }

    private IEnumerator LerpImageAlphaRoutine(Image image, float alpha, float duration)
    {
        Color startColor = image.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        float startAlpha = image.color.a;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                image.color = targetColor;
                break;
            }

            var tempColor = Color.Lerp(startColor, targetColor, timer / duration);
            image.color = tempColor;
            yield return null;
        }
    }

    public void LerpSpriteAlpha(SpriteRenderer sprite, float alpha, float duration)
    {
        if (colorRoutine != null)
            StopCoroutine(colorRoutine);

        colorRoutine = StartCoroutine(LerpSpriteAlphaRoutine(sprite, alpha, duration));
    }

    private IEnumerator LerpSpriteAlphaRoutine(SpriteRenderer sprite, float alpha, float duration)
    {
        Color startColor = sprite.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        float startAlpha = sprite.color.a;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                sprite.color = targetColor;
                break;
            }

            var tempColor = Color.Lerp(startColor, targetColor, timer / duration);
            sprite.color = tempColor;
            yield return null;
        }
    }
}
