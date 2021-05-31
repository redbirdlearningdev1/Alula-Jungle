using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AnimatedIcon 
{
    none, fire, shrine, lamp
}

public enum StarLocation
{
    up, down, none
}

public class MapIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Game Data")]
    public GameData gameData;

    [Header("Animation Stuff")]
    public bool canBeFixed;
    public AnimatedIcon animatedIcon;

    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite fixedSprite;

    private MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private Animator repairAnimator;
    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;
    private bool isFixed = false;

    [Header("Stars")]
    public StarLocation starLocation;
    // [SerializeField] private GameObject upStarObj;
    // [SerializeField] private GameObject downStarObj;
    [SerializeField] private Star[] upStars;
    [SerializeField] private Star[] downStars;
    private Star[] currentStars;


    void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animatedIcon != AnimatedIcon.none) animator = GetComponent<Animator>();
        if (repairAnimator) repairAnimator.Play("defaultAnimation");

        // configure current stars
        InitStars();
    }

    private void InitStars()
    {
        switch (starLocation)
        {
            case StarLocation.up:
                currentStars = upStars;
                break;
            case StarLocation.down:
                currentStars = downStars;
                break;
            default:
            case StarLocation.none:
                currentStars = null;
                break;
        }

        // set stars to empty or filled based on SIS data
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
                if (!opt) spriteRenderer.sprite = brokenSprite;
                else spriteRenderer.sprite = fixedSprite;
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
