using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;



public class StickerBoardBook : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Game Data")]
    //public GameData gameData;





    private MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private Animator BookMovement;

    private bool isPressed = false;
    private bool isFixed = false;




    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
            Debug.Log(isPressed);
            BookMovement.Play("BookSelected");
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
