using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class BoardBookButton : MonoBehaviour
{
    public bool interactable;
    private bool isPressed = false;

    public Animator boardBookAnimator;

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

            // open board book menu
            OpenBoardBookMenu();
        }
    }

    public void OpenBoardBookMenu()
    {
        boardBookAnimator.Play("BoardBookClicked");
    }
}
