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
        StartCoroutine(FlapWingsRoutine(true));
        animator.Play("PreFly");
    }

    public void SayAudio(ActionWordEnum word, bool interactableAfter = true)
    {
        StartCoroutine(SayAudioRoutine(word, interactableAfter));
    }

    private IEnumerator SayAudioRoutine(ActionWordEnum word, bool interactableAfter = true)
    {
        interactable = false;
        
        StartCoroutine(FlapWingsRoutine(false));
        animator.Play("PreFly");
        yield return new WaitForSeconds(0.1f);

        // play audio
        AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(word).audio);
        yield return new WaitForSeconds(1.25f);

        animator.Play("EndFly");
        yield return new WaitForSeconds(0.25f);


        interactable = interactableAfter;
    }

    private IEnumerator FlapWingsRoutine(bool loop)
    {
        if (loop)
        {
            while (true)
            {
                // play sound
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                // play sound
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BirdWingFlap, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
        }
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
            SayAudio(PrintingGameManager.instance.correctValue, false);

            // stop wiggling if tutorial
            if (PrintingGameManager.instance.playTutorial && PrintingGameManager.instance.t_waitingForPlayer)
            {
                PrintingGameManager.instance.t_waitingForPlayer = false;
                interactable = false;
                GetComponent<WiggleController>().StopWiggle();
            }

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
