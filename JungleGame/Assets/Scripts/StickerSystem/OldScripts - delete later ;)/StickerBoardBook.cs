using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;



public class StickerBoardBook : MonoBehaviour
{
    /*
    public static StickerBoardBook instance;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isPressed = false;
    
    public bool isOn = true;

    void Awake()
    {
        if (instance == null)
            instance = this;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */
    /*

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOn)
            return;

        if (!isPressed)
        {
            isPressed = true;
            animator.Play("BookSelected");

            WagonWindowController.instance.ToggleBuyBoardWindow();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
        }
    }
    */
}
