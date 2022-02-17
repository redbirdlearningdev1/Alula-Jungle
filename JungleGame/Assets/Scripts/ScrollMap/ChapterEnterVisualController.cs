using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ChapterEnterVisualController : MonoBehaviour
{
    public static ChapterEnterVisualController instance;

    public Animator animator;

    [Header("Images")]
    public Image chapterImage;
    public Image sectionImage;

    [Header("Chapter Sprites")]
    public Sprite chapter1;
    public Sprite chapter2;
    public Sprite chapter3;
    public Sprite chapter4;
    public Sprite chapter5;
    public Sprite chapter6;

    [Header("Section Sprites")]
    public Sprite GorillaVillageSign;
    public Sprite MudslideSign;
    public Sprite OrcVillageSign;
    public Sprite SpookyForestSign;
    public Sprite OrcCampSign;
    public Sprite GorillaPoopSign;
    public Sprite WindyCliffSign;
    public Sprite PirateShipSign;
    public Sprite MermaidBeachSign;
    public Sprite RuinsSign;
    public Sprite ExitJungleSign;
    public Sprite GorillaStudySign;
    public Sprite BeforeBossSign;
    public Sprite BossSign;
    public Sprite TheEndSign;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetSign(MapLocation mapLocation)
    {
        // set chapter and section sprites
        switch (mapLocation)
        {
            default:
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                sectionImage.sprite = null;
                break;
            case MapLocation.GorillaVillage:
                chapterImage.sprite = chapter1;
                sectionImage.sprite = GorillaVillageSign;
                break;
            case MapLocation.Mudslide:
                chapterImage.sprite = chapter1;
                sectionImage.sprite = MudslideSign;
                break;
            case MapLocation.OrcVillage:
                chapterImage.sprite = chapter1;
                sectionImage.sprite = OrcVillageSign;
                break;
            case MapLocation.SpookyForest:
                chapterImage.sprite = chapter2;
                sectionImage.sprite = SpookyForestSign;
                break;
            case MapLocation.OrcCamp:
                chapterImage.sprite = chapter2;
                sectionImage.sprite = OrcCampSign;
                break;
            case MapLocation.GorillaPoop:
                chapterImage.sprite = chapter2;
                sectionImage.sprite = GorillaPoopSign;
                break;
            case MapLocation.WindyCliff:
                chapterImage.sprite = chapter3;
                sectionImage.sprite = WindyCliffSign;
                break;
            case MapLocation.PirateShip:
                chapterImage.sprite = chapter3;
                sectionImage.sprite = PirateShipSign;
                break;
            case MapLocation.MermaidBeach:
                chapterImage.sprite = chapter4;
                sectionImage.sprite = MermaidBeachSign;
                break;
            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                chapterImage.sprite = chapter4;
                sectionImage.sprite = RuinsSign;
                break;
            case MapLocation.ExitJungle:
                chapterImage.sprite = chapter5;
                sectionImage.sprite = ExitJungleSign;
                break;
            case MapLocation.GorillaStudy:
                chapterImage.sprite = chapter5;
                sectionImage.sprite = GorillaStudySign;
                break; 
            case MapLocation.Monkeys:
                chapterImage.sprite = chapter5;
                sectionImage.sprite = BeforeBossSign;
                break;             
        }
    }

    public void ShowSign()
    {
        animator.Play("ShowPanel");

        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RopeDown, 0.5f);
    }

    public void HideSign()
    {
        animator.Play("HidePanel");

        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RopeUp, 0.5f);
    }
}
