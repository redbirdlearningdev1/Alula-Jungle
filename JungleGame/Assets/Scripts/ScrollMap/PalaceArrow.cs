using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PalaceArrow : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static PalaceArrow instance;

    public bool interactable;
    private bool isOver;
    private bool isPressed;

    public LerpableObject arrow;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        arrow.transform.localScale = Vector3.zero;
        interactable = false;
    }

    public void ShowArrow()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        arrow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
        interactable = true;
    }

    public void HideArrow()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        arrow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
        interactable = false;
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    void OnMouseOver()
    {
        // skip if not interactable 
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
        // skip if not interactable 
        if (!interactable)
            return;

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

            // show palace
            ShowPalace();
        }
    }

    public void ShowPalace()
    {   
        StartCoroutine(ShowPalaceRoutine());
    }

    private IEnumerator ShowPalaceRoutine()
    {
        // stop wiggle
        GetComponent<WiggleController>().StopWiggle();

        // remove arrow
        interactable = false;
        HideArrow();

        // hide all stars
        ScrollMapManager.instance.DisableAllMapIcons(true);
    
        // remove player input
        ScrollMapManager.instance.ToggleNavButtons(false);
        RaycastBlockerController.instance.CreateRaycastBlocker("entering_palace");

        yield return new WaitForSeconds(0.2f);

        // move camera to pre palace pos
        ScrollMapManager.instance.PanIntoPalace();

        yield return new WaitForSeconds(5f);

        // show boss battle bar
        BossBattleBar.instance.ShowBar();
        yield return new WaitForSeconds(1.5f);

        // show down arrow
        PalaceArrowDown.instance.ShowArrow();
        PalaceArrowDown.instance.interactable = true;

        // add player input
        RaycastBlockerController.instance.RemoveRaycastBlocker("entering_palace");
    }
}
