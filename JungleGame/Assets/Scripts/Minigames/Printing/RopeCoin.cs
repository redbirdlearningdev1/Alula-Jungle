using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RopeCoin : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static RopeCoin instance;

    public bool interactable;
    public float pressedScaleChange;
    private bool isPressed;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SayAudio(ActionWordEnum word, bool interactableAfter = true)
    {
        StartCoroutine(SayAudioRoutine(word, interactableAfter));
    }

    private IEnumerator SayAudioRoutine(ActionWordEnum word, bool interactableAfter = true)
    {
        interactable = false;

        // play audio
        AudioManager.instance.PlayTalk(GameManager.instance.GetActionWord(word).audio);
        yield return new WaitForSeconds(1.25f);

        GetComponent<UniversalCoinImage>().LerpScale(Vector2.one, 0.2f);

        interactable = interactableAfter;
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
            GetComponent<UniversalCoinImage>().LerpScale(new Vector2(0.9f, 0.9f), 0.2f);
            GetComponent<UniversalCoinImage>().ShakeCoin(1.25f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

        if (isPressed)
        {
            // play audio blip
            SayAudio(PrintingGameManager.instance.correctValue, true);

            // stop wiggling if tutorial
            if (PrintingGameManager.instance.playTutorial && PrintingGameManager.instance.t_waitingForPlayer)
            {
                PrintingGameManager.instance.t_waitingForPlayer = false;
                interactable = false;
            }

            isPressed = false;
        }
    }
}
