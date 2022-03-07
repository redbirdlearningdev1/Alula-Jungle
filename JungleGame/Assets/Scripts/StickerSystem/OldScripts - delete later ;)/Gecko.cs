using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Gecko : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public static Gecko instance;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public float scaleNormal;
    public float scalePressed;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOn)
            return;

        if (!isPressed)
        {
            isPressed = true;
            transform.localScale = new Vector3(scalePressed, scalePressed, 1f);
            WagonWindowController.instance.NewWindow();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
            transform.localScale = new Vector3(scaleNormal, scaleNormal, 1f);
        }
    }
}
