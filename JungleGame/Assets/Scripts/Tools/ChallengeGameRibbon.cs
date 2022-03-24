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

    [Header("Ribbon Sprites")]
    public Image starRibbon;
    public Sprite ribbonStars0;
    public Sprite ribbonStars1;
    public Sprite ribbonStars2;
    public Sprite ribbonStars3;

    [Header("Logo Sprites")]
    public Sprite wordFactorySubstitutionLogo;
    public Sprite wordFactoryBlendingLogo;
    public Sprite wordFactoryBuildingLogo;
    public Sprite wordFactoryDeletingLogo;
    public Sprite tigerPawCoinsLogo;
    public Sprite tigerPawPhotosLogo;
    public Sprite passwordLogo;

    private GameType myGameType;

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

    public void OpenRibbon(GameType gameType)
    {
        myGameType = gameType;
        StartCoroutine(OpenRibbonRoutine(gameType));
    }

    private IEnumerator OpenRibbonRoutine(GameType challengeGameType)
    {
        // set logo
        logo.GetComponent<Image>().sprite = GetGameLogo(challengeGameType);

        // set stars
        SetRibbonStars();

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
            case GameType.WordFactoryBuilding:
                return wordFactoryBuildingLogo;
            case GameType.WordFactoryDeleting:
                return wordFactoryDeletingLogo;
            case GameType.TigerPawPhotos:
                return tigerPawPhotosLogo;
            case GameType.TigerPawCoins:
                return tigerPawCoinsLogo;
            case GameType.Password:
                return passwordLogo;
        }
    }

    private void SetRibbonStars()
    {
        int stars = 0;

        // determine map area + get sis data
        switch (ScrollMapManager.instance.GetCurrentMapLocation())
        {
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                GameManager.instance.SendError(this, "Somehow you managed to get challenge games in an invalid area???");
                break;

            case MapLocation.GorillaVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars;
                break;
                
            case MapLocation.Mudslide:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars;
                break;

             case MapLocation.OrcVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars;
                break;
            
            case MapLocation.SpookyForest:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars;
                break;

            case MapLocation.OrcCamp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars;
                break;

            case MapLocation.GorillaPoop:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars;
                break;

            case MapLocation.WindyCliff:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars;
                break;

            case MapLocation.PirateShip:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars;
                break;

            case MapLocation.MermaidBeach:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.stars;
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars;
                break;

            case MapLocation.ExitJungle:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars;
                break;

            case MapLocation.GorillaStudy:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars;
                break;

            case MapLocation.Monkeys:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars;
                else if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType == myGameType)
                    stars = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars;
                break;
        }

        // place correct sprite
        switch (stars)
        {
            default:
            case 0:
                starRibbon.sprite = ribbonStars0;
                break;
            case 1:
                starRibbon.sprite = ribbonStars1;
                break;
            case 2:
                starRibbon.sprite = ribbonStars2;
                break;
            case 3:
                starRibbon.sprite = ribbonStars3;
                break;
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
            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);

            RoyalDecreeController.instance.OpenConfirmWindow(myGameType);
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
