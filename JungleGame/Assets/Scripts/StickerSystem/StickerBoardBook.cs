using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;



public class StickerBoardBook : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Game Data")]
    //public GameData gameData;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isPressed = false;
    private bool isFixed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isPressed)
        {
            isPressed = true;
            Debug.Log("HERE");
            animator.Play("BookSelected");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
            Debug.Log(isPressed);
        }
    }
}
