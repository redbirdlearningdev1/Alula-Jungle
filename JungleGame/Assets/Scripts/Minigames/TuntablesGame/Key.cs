using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public ActionWordEnum keyActionWord;
    private bool canBePressed = false;
    private bool isDissipating = false;

    [SerializeField] private Animator animator;
    public Image image;
    public Transform grabPos;

    public string keyName;
    public Transform ropePos;
    public Transform keyParent;
    public float moveSpeed;
    public float dissipateTime;
    public const float scaleMult = 0.99f;

    private Coroutine currentRoutine;

    public void StartMovingAnimation()
    {
        if (isDissipating) return;
        string animation_name = keyName + "_first_move";
        animator.Play(animation_name);
    }

    public void StopMovingAnimation()
    {
        if (isDissipating) return;
        string animation_name = keyName + "_last_move";
        animator.Play(animation_name);
    }

    public void SetKeyActionWord(ActionWordEnum word)
    {
        keyActionWord = word;
        canBePressed = true;
    }

    public void PlayAudio()
    {
        if (!canBePressed)
            return;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        AudioManager.instance.PlayPhoneme(keyActionWord);
        PlaySoundAnimation(1f);

        // play key tap
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyTap, 1f);
    }

    private void PlaySoundAnimation(float duration)
    {
        currentRoutine = StartCoroutine(PlaySoundRoutine(duration));
    }

    private IEnumerator PlaySoundRoutine(float duration)
    {
        string animation_name = keyName + "_audio_play";
        animator.Play(animation_name);

        yield return new WaitForSeconds(duration);
        
        animation_name = keyName + "_still";
        animator.Play(animation_name);
    }

    public void ReturnToRope()
    {
        StartCoroutine(ReturnToOriginalPosRoutine(ropePos.position));
    }

    public void Dissipate()
    {
        isDissipating = true;
        // make key invisible over time
        StartCoroutine(DissipateAndDestroy());
    }

    private IEnumerator DissipateAndDestroy()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > dissipateTime)
            {
                Destroy(this.gameObject);
                break;
            }
            // reduce alpha over time
            float a = Mathf.Lerp(1, 0, timer / dissipateTime);
            image.color = new Color(1f, 1f, 1f, a);
            // decrease in scale over time
            transform.localScale = transform.localScale * scaleMult;
            yield return null;
        }
    }

    private IEnumerator ReturnToOriginalPosRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                transform.SetParent(keyParent);
                yield break;
            }

            yield return null;
        }
    }
}
