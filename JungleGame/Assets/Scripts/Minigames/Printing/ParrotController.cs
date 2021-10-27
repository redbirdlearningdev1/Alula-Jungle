using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParrotController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static ParrotController instance;

    public bool interactable;
    public float pressedScaleChange;
    private bool isPressed;
    public Animator animator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void CelebrateAnimation(float duration)
    {
        StartCoroutine(CelebrateAnimationRoutine(duration));
    }
    private IEnumerator CelebrateAnimationRoutine(float duration)
    {
        animator.Play("Celebrate");
        yield return new WaitForSeconds(duration);
        animator.Play("Idle");
    }

    public void SadAnimation(float duration)
    {
        StartCoroutine(SadAnimationRoutine(duration));
    }
    private IEnumerator SadAnimationRoutine(float duration)
    {
        animator.Play("No");
        yield return new WaitForSeconds(duration);
        animator.Play("Idle");
    }

    public void WinAnimation()
    {
        animator.Play("PreFly");
    }

    public void SayAudio(ActionWordEnum word)
    {
        StartCoroutine(SayAudioRoutine(word));
    }

    private IEnumerator SayAudioRoutine(ActionWordEnum word)
    {
        interactable = false;

        animator.Play("PreFly");
        yield return new WaitForSeconds(0.1f);

        // play audio
        AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(word).audio);
        yield return new WaitForSeconds(1.25f);

        animator.Play("EndFly");
        yield return new WaitForSeconds(0.25f);

        interactable = true;
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

        if (!isPressed)
        {
            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            SayAudio(PrintingGameManager.instance.correctValue);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
