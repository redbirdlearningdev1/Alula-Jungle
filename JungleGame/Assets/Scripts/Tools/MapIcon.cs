using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameData gameData;

    public bool canBeFixed;
    public bool isFire;

    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite fixedSprite;

    private MeshRenderer meshRenderer;
    private Image img;
    private Animator animator;
    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;
    private bool isFixed = false;

    void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        img = GetComponent<Image>();
        if (isFire) animator = GetComponent<Animator>();
    }

    public void SetOutineColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void SetFixed(bool opt)
    {
        if (!canBeFixed) return;

        if (!isFire)
        {
            if (!opt) img.sprite = brokenSprite;
            else img.sprite = fixedSprite;
        }
        else
        {
            animator.SetBool("isBroken", !opt);
        }
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
            GameHelper.NewLevelPopup(gameData);
        }
    }
}
