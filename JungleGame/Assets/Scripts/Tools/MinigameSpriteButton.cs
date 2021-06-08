using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MinigameButton
{
    storyGame, turntablesGame, seashellsGame, froggerGame, boatGame, rummageGame, printingGame, spiderwebGame
}

[RequireComponent(typeof(SpriteRenderer))]
public class MinigameSpriteButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool isOn;
    public MinigameButton minigame;
    public float pressedScaleChange;
    private bool isPressed;

    private SpriteRenderer spriteRenderer;

    [Header("Button Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite pressedSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            spriteRenderer.sprite = pressedSprite;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
            spriteRenderer.sprite = defaultSprite;


            // return if off
            if (!isOn)
                return;

            switch (minigame)
            {
                default:
                case MinigameButton.storyGame:
                    GameManager.instance.SendLog(this, "Playing - StoryGame");
                    GameManager.instance.LoadScene("StoryGame", true);
                    break;
                case MinigameButton.turntablesGame:
                    GameManager.instance.SendLog(this, "Playing - TurntablesGame");
                    GameManager.instance.LoadScene("TurntablesGame", true);
                    break;
                case MinigameButton.seashellsGame:
                    GameManager.instance.SendLog(this, "Playing - SeashellGame");
                    GameManager.instance.LoadScene("SeashellGame", true);
                    break;
                case MinigameButton.froggerGame:
                    GameManager.instance.SendLog(this, "Playing - FroggerGame");
                    GameManager.instance.LoadScene("FroggerGame", true);
                    break;
                case MinigameButton.boatGame:
                    GameManager.instance.SendLog(this, "Playing - BoatGame");
                    GameManager.instance.LoadScene("BoatGame", true);
                    break;
                case MinigameButton.rummageGame:
                    GameManager.instance.SendLog(this, "Playing - RummageGame");
                    GameManager.instance.LoadScene("RummageGame", true);
                    break;
                case MinigameButton.printingGame:
                    // GameManager.instance.SendLog(this, "Playing - SeashellsGame");
                    // GameManager.instance.LoadScene("SeashellsGame", true);
                    // break;
                case MinigameButton.spiderwebGame:
                    // GameManager.instance.SendLog(this, "Playing - SeashellsGame");
                    // GameManager.instance.LoadScene("SeashellsGame", true);
                    // break;
                    break;
            }
        }
    }
}
