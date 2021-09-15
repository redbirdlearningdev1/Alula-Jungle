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

    private int stars;

    public void ShowSignPost(int stars)
    {
        this.stars = stars;

        StartCoroutine(ShowSignPostRoutine());
    }

    private IEnumerator ShowSignPostRoutine()
    {
        print ("turning on sign post!");

        yield return new WaitForSeconds(0.5f);

        interactable = true;
        animator.Play("springUp");

        yield return new WaitForSeconds(0.5f);

        SetStars(stars);
    }

    public void SetStars(int stars)
    {
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
        interactable = false;

        yield return new WaitForSeconds (1f);
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

            // place copy over bg
            var tempSignPost = TempObjectPlacer.instance.PlaceNewObject(this.gameObject, transform.localPosition);
            StartCoroutine(DelaySignPostInteractableRoutine(tempSignPost.GetComponent<SignPostController>()));
            tempSignPost.GetComponent<SignPostController>().SetStars(stars);
        }
    }

    private IEnumerator DelaySignPostInteractableRoutine(SignPostController controller)
    {
        controller.interactable = false;
        yield return new WaitForSeconds(1.5f);
        controller.interactable = true;
    }
}
