using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChallengeGameRibbon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public LerpableObject logo;
    public LerpableObject ribbon;
    public LerpableObject crown;

    [Header("Logo Sprites")]
    public Sprite wordFactorySubstitutionLogo;
    public Sprite wordFactoryBlendingLogo;
    public Sprite tigerPawCoinsLogo;

    private GameData myGameData;

    [Header("Pressed Values")]
    public float pressedScaleChange;
    public bool interactable;
    private bool isPressed;
    

    void Awake()
    {
        // hide UI
        ribbon.transform.localScale = new Vector3(1f, 0f, 1f);
        logo.transform.localScale = new Vector3 (0f, 0f, 1f);
        crown.transform.localScale = new Vector3(0f, 0f, 1f);
    }

    public void OpenRibbon(GameData gameData)
    {
        myGameData = gameData;
        StartCoroutine(OpenRibbonRoutine(myGameData.gameType));
    }

    private IEnumerator OpenRibbonRoutine(GameType challengeGameType)
    {
        logo.GetComponent<Image>().sprite = GetGameLogo(challengeGameType);

        logo.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.05f);
        yield return new WaitForSeconds(0.25f);

        ribbon.SquishyScaleLerp(new Vector2(1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.1f);

        crown.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
    }

    public void CloseRibbon()
    {
        StartCoroutine(CloseRibbonRoutine());
    }

    private IEnumerator CloseRibbonRoutine()
    {
        crown.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.1f);

        ribbon.SquishyScaleLerp(new Vector2(1f, 1.1f), new Vector2(1f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.1f);

        logo.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.05f);
        yield return new WaitForSeconds(0.25f);
    }


    private Sprite GetGameLogo(GameType challengeGameType)
    {
        switch (challengeGameType)
        {
            default:
            case GameType.WordFactoryBlending:
                return wordFactoryBlendingLogo;
            case GameType.WordFactorySubstituting:
                return wordFactorySubstitutionLogo;
            case GameType.TigerPawCoins:
                return tigerPawCoinsLogo;
        }
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

        if (!isPressed)
        {
            // play sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);

            RoyalDecreeController.instance.OpenConfirmWindow(myGameData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
