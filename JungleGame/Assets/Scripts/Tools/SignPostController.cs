using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SignPostController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int challengeGameTriadIndex;

    public Animator animator;

    public float pressedScaleChange;

    public bool interactable;
    private bool isPressed;

    void Awake()
    {

    }

    public void ShowSignPost(int stars)
    {
        StartCoroutine(ShowSignPostRoutine(stars));
    }

    private IEnumerator ShowSignPostRoutine(int stars)
    {
        print ("turning on sign post!");

        yield return new WaitForSeconds(0.5f);

        interactable = true;
        animator.Play("springUp");

        yield return new WaitForSeconds(0.5f);

        switch (stars)
        {
            default:
            case 0:
                animator.Play("show0stars");
                break;
            case 1:
                animator.Play("show1star");
                break;
            case 2:
                animator.Play("show2stars");
                break;
            case 3:
                animator.Play("show3stars");
                break;
            case 4:
                animator.Play("show4stars");
                break;
        }
    }

    public void HideSignPost()
    {
        StartCoroutine(HideSignPostRoutine());
    }

    private IEnumerator HideSignPostRoutine()
    {
        print ("turning off sign post!");

        interactable = false;

        yield return new WaitForSeconds (0.5f);
        animator.Play("hidden");
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
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);

            // open window 
            RoyalDecreeController.instance.ToggleWindow(challengeGameTriadIndex);
        }
    }
}
