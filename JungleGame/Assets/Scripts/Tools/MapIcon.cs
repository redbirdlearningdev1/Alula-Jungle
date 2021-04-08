using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AnimatedIcon 
{
    none, fire, shrine, lamp
}

public class MapIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameData gameData;

    public bool canBeFixed;
    public AnimatedIcon animatedIcon;

    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite fixedSprite;

    private MeshRenderer meshRenderer;
    private Image img;
    private Animator animator;
    [SerializeField] private Animator repairAnimator;
    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;
    private bool isFixed = false;

    void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        img = GetComponent<Image>();
        if (animatedIcon != AnimatedIcon.none) animator = GetComponent<Animator>();
        if (repairAnimator) repairAnimator.Play("defaultAnimation");
    }

    public void SetOutineColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void SetFixed(bool opt)
    {
        if (!canBeFixed) return;

        switch (animatedIcon)
        {
            default:
            case AnimatedIcon.none:
                if (!opt) img.sprite = brokenSprite;
                else img.sprite = fixedSprite;
                break;
            case AnimatedIcon.fire:
                if (!opt) animator.Play("fireBroken");
                else animator.Play("fireFixed");
                break;
            case AnimatedIcon.shrine:
                if (!opt) animator.Play("shrineBroken");
                else animator.Play("shrineFixed");
                break;
            case AnimatedIcon.lamp:
                if (!opt) animator.Play("lampBroken");
                else animator.Play("lampFixed");
                break;
        }
        // play repair animation
        if (opt) if (repairAnimator) repairAnimator.Play("repairAnimation");
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
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            GameManager.instance.NewLevelPopup(gameData);
        }
    }
}
