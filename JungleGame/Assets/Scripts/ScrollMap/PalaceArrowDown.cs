using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PalaceArrowDown : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static PalaceArrowDown instance;

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
        // return if arrow already hidden
        if (arrow.transform.localScale.x == 1f)
            return;

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        arrow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
        interactable = true;
    }

    public void HideArrow()
    {
        // return if arrow already hidden
        if (arrow.transform.localScale.x == 0f)
            return;
        
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
        // skip if not interactable OR playing talkie OR minigamewheel out OR settings window open OR royal decree open OR wagon open
        if (!interactable || 
            TalkieManager.instance.talkiePlaying || 
            MinigameWheelController.instance.minigameWheelOut || 
            SettingsManager.instance.settingsWindowOpen || 
            RoyalDecreeController.instance.isOpen ||
            StickerSystem.instance.wagonOpen)
            
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
            HidePalace();
        }
    }

    public void HidePalace()
    {   
        StartCoroutine(HidePalaceRoutine());
    }

    private IEnumerator HidePalaceRoutine()
    {
        // stop wiggle
        GetComponent<WiggleController>().StopWiggle();

        // remove arrow
        interactable = false;
        HideArrow();

        // show boss battle bar
        BossBattleBar.instance.HideBar();
        yield return new WaitForSeconds(1f);

        // enable all map locations
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.PalaceIntro);

        // remove player input
        ScrollMapManager.instance.ToggleNavButtons(false);
        RaycastBlockerController.instance.CreateRaycastBlocker("exiting_palace");

        yield return new WaitForSeconds(0.2f);

        // move camera to pre palace pos
        ScrollMapManager.instance.PanOutOfPalace();

        yield return new WaitForSeconds(5f);

        // show up arrow
        PalaceArrow.instance.ShowArrow();
        PalaceArrow.instance.interactable = true;

        // add player input
        ScrollMapManager.instance.ToggleNavButtons(true);
        RaycastBlockerController.instance.RemoveRaycastBlocker("exiting_palace");
    }
}
