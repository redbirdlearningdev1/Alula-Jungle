using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class LesterButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable;
    private bool isPressed = false;

    public Animator lesterAnimator;

    public float sleepTime;
    public float blinkTime;
    private float sleepTimer = 0f;
    private float blinkTimer = 0f;

    private bool isAsleep = false;
    private bool isBlinking = false;

    void Update()
    {
        if (isAsleep)
            return;

        // lester falls asleep after some time
        // if awake - lester blinks if idle
        if (!isAsleep && !isBlinking)
        {
            sleepTimer += Time.deltaTime;
            if (sleepTimer >= sleepTime)
            {
                isAsleep = true;
                lesterAnimator.Play("geckoFallAsleep");
                sleepTimer = 0f;
            }

            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkTime)
            {
                isBlinking = true;
                blinkTimer = 0f;
                StartCoroutine(LesterBlinkRoutine());
            }
        }
    }

    private IEnumerator LesterBlinkRoutine()
    {
        lesterAnimator.Play("geckoBlink");
        yield return new WaitForSeconds(1.2f);
        isBlinking = false;

    }


    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    private bool isOver = false;

    void OnMouseOver()
    {
        // skip if not interactable OR playing talkie OR minigamewheel out OR settings window open OR royal decree open
        if (!interactable)
            return;
        
        if (!isOver)
        {
            isOver = true;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
    }

    void OnMouseExit()
    {
        if (isOver)
        {
            isOver = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable || !isOver)
            return;

        if (!isPressed)
        {
            isPressed = true;
            GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && isOver)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

            // open sticker board menu
            OpenNewStickerMenu();
        }
    }

    public void OpenNewStickerMenu()
    {
        if (isAsleep)
        {
            isAsleep = false;
            lesterAnimator.Play("geckoWakeUp");
        }
        
    }
}
