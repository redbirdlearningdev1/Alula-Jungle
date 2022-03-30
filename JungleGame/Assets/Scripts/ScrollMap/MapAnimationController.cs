using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum MapAnim
{
    BoatIntro,

    /// CHAPTER 1
    // gorilla village
    RevealGorillaVillage,
    GorillaVillageIntro,
    RedShowsStickerButton,
    DarwinForcesLesterInteraction,
    GorillaVillageRebuilt,
    GorillaVillageDefeated,
    // mudslide
    MudslideRebuilt,
    MudslideDefeated,
    // orc village
    OrcVillageRebuilt,
    OrcVillageDefeated,

    /// CHAPTER 2
    // gorilla village
    SpookyForestIntro,
    SpookyForestRebuilt,
    SpookyForestDefeated,
    // orc camp
    OrcCampRebuilt,
    OrcCampDefeated,
    // gorilla poop
    GorillaPoopRebuilt,
    GorillaPoopDefeated,

    /// CHAPTER 3
    // windy cliff
    WindyCliffIntro,
    WindyCliffRebuilt,
    WindyCliffDefeated,
    // pirate ship
    PirateShipRebuilt,
    PirateShipDefeated,

    /// CHAPTER 4
    // mermaid beach
    MermaidBeachIntro,
    MermaidBeachRebuilt,
    MermaidBeachDefeated,
    // ruins
    RuinsRebuilt,
    RuinsDefeated,

    /// CHAPTER 5
    // exit jungle
    ExitJungleIntro,
    ExitJungleRebuilt,
    ExitJungleDefeated,
    // gorilla study
    GorillaStudyIntro,
    GorillaStudyRebuilt,
    GorillaStudyDefeated,
    // monkeys
    MonkeysRebuilt,
    MonkeysDefeated,

    /// CHAPTER 6
    PalaceIntro,
    PreBossBattle,


    // all challenge games
    ChallengeGame1,
    ChallengeGame2,
    ChallengeGame3,

    // all boss battles
    BossBattle1,
    BossBattle2,
    BossBattle3,

    EndBossBattle
}

[ExecuteInEditMode]
public class MapAnimationController : MonoBehaviour
{
    public static MapAnimationController instance;
    [HideInInspector] public bool animationDone = false; // used to determine when the current animation is complete

    public Animator tigerSwipeAnim;

    [Header("Characters")]
    public MapCharacter boat;
    public MapCharacter darwin;
    public MapCharacter clogg;
    public MapCharacter julius;
    public MapCharacter brutus;
    public MapCharacter marcus;
    public MapCharacter taxiBird;

    [Header("Dev Test Walk-ins and Walk-outs")]
    public MapLocation mapLocationToTest;


    void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(TestWalkInWalkOut());
            }
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;

        // place all characters off screen
        boat.mapAnimator.Play("BoatOffScreenPos");
        darwin.mapAnimator.Play("DarwinOffScreenPos");
        julius.mapAnimator.Play("JuliusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        clogg.mapAnimator.Play("CloggOffScreenPos");
        taxiBird.mapAnimator.Play("TaxiBirdOffScreenPos");

        boat.GetComponent<Image>().raycastTarget = false;
        darwin.GetComponent<Image>().raycastTarget = false;
        julius.GetComponent<Image>().raycastTarget = false;
        brutus.GetComponent<Image>().raycastTarget = false;
        marcus.GetComponent<Image>().raycastTarget = false;
        clogg.GetComponent<Image>().raycastTarget = false;
        taxiBird.GetComponent<Image>().raycastTarget = false;
    }

    private IEnumerator TestWalkInWalkOut()
    {
        switch (mapLocationToTest)
        {
            default:
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                break;

            case MapLocation.GorillaVillage:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaVillage].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInGV");
                marcus.mapAnimator.Play("MarcusWalkInGV");
                brutus.mapAnimator.Play("BrutusWalkInGV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInGV"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutGV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGV"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutGV");
                brutus.mapAnimator.Play("BrutusWalkOutGV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGV"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.Mudslide:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.Mudslide].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInMS");
                marcus.mapAnimator.Play("MarcusWalkInMS");
                brutus.mapAnimator.Play("BrutusWalkInMS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInMS"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutMS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMS"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutMS");
                brutus.mapAnimator.Play("BrutusWalkOutMS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutMS"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.OrcVillage:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcVillage].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInOV");
                marcus.mapAnimator.Play("MarcusWalkInOV");
                brutus.mapAnimator.Play("BrutusWalkInOV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInOV"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutOV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutOV"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutOV");
                brutus.mapAnimator.Play("BrutusWalkOutOV");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutOV"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.SpookyForest:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.SpookyForest].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInSF");
                marcus.mapAnimator.Play("MarcusWalkInSF");
                brutus.mapAnimator.Play("BrutusWalkInSF");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInSF"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutSF");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutSF"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutSF");
                brutus.mapAnimator.Play("BrutusWalkOutSF");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutSF"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.OrcCamp:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcCamp].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInOC");
                marcus.mapAnimator.Play("MarcusWalkInOC");
                brutus.mapAnimator.Play("BrutusWalkInOC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInOC"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutOC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutOC"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutOC");
                brutus.mapAnimator.Play("BrutusWalkOutOC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutOC"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.GorillaPoop:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaPoop].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInGP");
                marcus.mapAnimator.Play("MarcusWalkInGP");
                brutus.mapAnimator.Play("BrutusWalkInGP");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkInGP"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutGP");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGP"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutGP");
                brutus.mapAnimator.Play("BrutusWalkOutGP");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGP"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.WindyCliff:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.WindyCliff].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInWC");
                marcus.mapAnimator.Play("MarcusWalkInWC");
                brutus.mapAnimator.Play("BrutusWalkInWC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInWC"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutWC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutWC");
                brutus.mapAnimator.Play("BrutusWalkOutWC");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutWC"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.PirateShip:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.PirateShip].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInPS");
                marcus.mapAnimator.Play("MarcusWalkInPS");
                brutus.mapAnimator.Play("BrutusWalkInPS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInPS"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutPS");
                brutus.mapAnimator.Play("BrutusWalkOutPS");

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutPS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutPS"));

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutPS"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.MermaidBeach:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.MermaidBeach].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInMB");
                marcus.mapAnimator.Play("MarcusWalkInMB");
                brutus.mapAnimator.Play("BrutusWalkInMB");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInMB"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutMB");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMB"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutMB");
                brutus.mapAnimator.Play("BrutusWalkOutMB");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutMB"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                // make sure scroll map is on Ruins 1!!!
                ScrollMapManager.instance.GoToMapPosition(MapLocation.Ruins1);
                yield return new WaitForSeconds(2f);

                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.Ruins1].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInR");
                marcus.mapAnimator.Play("MarcusWalkInR");
                brutus.mapAnimator.Play("BrutusWalkInR");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInR"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutR");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutR"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutR");
                brutus.mapAnimator.Play("BrutusWalkOutR");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutR"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.ExitJungle:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.ExitJungle].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInEJ");
                marcus.mapAnimator.Play("MarcusWalkInEJ");
                brutus.mapAnimator.Play("BrutusWalkInEJ");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInEJ"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutEJ");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutEJ"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutEJ");
                brutus.mapAnimator.Play("BrutusWalkOutEJ");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutEJ"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.GorillaStudy:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaStudy].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                marcus.characterAnimator.Play("marcusWalkIn");
                brutus.characterAnimator.Play("brutusWalkIn");

                julius.mapAnimator.Play("JuliusWalkInGS");
                marcus.mapAnimator.Play("MarcusWalkInGS");
                brutus.mapAnimator.Play("BrutusWalkInGS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInGS"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");
                marcus.characterAnimator.Play("marcusBroken");
                brutus.characterAnimator.Play("brutusBroken");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutGS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGS"));

                // monkies go hehe and haha then run off too
                marcus.characterAnimator.Play("marcusWin");
                brutus.characterAnimator.Play("brutusWin");

                yield return new WaitForSeconds(1f);

                marcus.characterAnimator.Play("marcusTurn");
                brutus.characterAnimator.Play("brutusTurn");

                yield return new WaitForSeconds(0.25f);

                marcus.mapAnimator.Play("MarcusWalkOutGS");
                brutus.mapAnimator.Play("BrutusWalkOutGS");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGS"));
                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;

            case MapLocation.Monkeys:
                // set fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[(int)MapLocation.Monkeys].fogLocation;

                // tiger and monkies walk in
                julius.characterAnimator.Play("tigerWalk");
                julius.mapAnimator.Play("JuliusWalkInM");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInM"));

                // idle animations
                julius.characterAnimator.Play("aTigerIdle");

                yield return new WaitForSeconds(2f);

                // tiger runs off screen
                julius.characterAnimator.Play("aTigerTurn");

                yield return new WaitForSeconds(0.25f);

                julius.mapAnimator.Play("JuliusWalkOutM");

                // wait for animation to be done
                yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutM"));

                // reset fog location
                FogController.instance.mapXpos = ScrollMapManager.instance.mapLocations[StudentInfoSystem.GetCurrentProfile().mapLimit].fogLocation;
                break;
        }

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;
    }

    public void PlaceCharactersOnMap(StoryBeat storyBeat)
    {
        GameManager.instance.SendLog(this, "placing characters on map for story beat: \'" + storyBeat + "\'");

        /// default positions on all story beats:
        if (storyBeat >= StoryBeat.GorillaVillageIntro && storyBeat < StoryBeat.RedShowsStickerButton)
        {
            // place boat in dock
            boat.mapAnimator.Play("BoatDockedPos");
            boat.GetComponent<MapIcon>().interactable = false;
            boat.GetComponent<Image>().raycastTarget = false;
        }
        else if (storyBeat >= StoryBeat.RedShowsStickerButton)
        {
            // place boat in dock
            boat.mapAnimator.Play("BoatDockedPos");
            boat.GetComponent<MapIcon>().interactable = true;
            boat.GetComponent<MapIcon>().fixedCollider.enabled = true;
            boat.GetComponent<Image>().raycastTarget = true; 
        }
        if (!(storyBeat >= StoryBeat.OrcCampUnlocked && storyBeat <= StoryBeat.OrcCampDefeated) && (ScrollMapManager.instance.GetCurrentMapLocation() != MapLocation.OrcCamp))
        {
            // place clogg in OV
            clogg.mapAnimator.Play("CloggOVPos");
            clogg.GetComponent<Image>().raycastTarget = true;
            clogg.interactable = true;
        }

        /// story beat specific character positions
        switch (storyBeat)
        {
            default:
                break;

            case StoryBeat.InitBoatGame:
            case StoryBeat.UnlockGorillaVillage:
                break;

            case StoryBeat.GorillaVillageIntro:
            case StoryBeat.PrologueStoryGame:
                // place darwin in GV
                darwin.ShowExclamationMark(true);
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.RedShowsStickerButton:
                // place darwin in GV
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.VillageRebuilt:
                // place darwin in GV
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.GorillaVillage_challengeGame_1:
                // place julius in GV
                julius.mapAnimator.Play("JuliusGVPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in GV
                marcus.mapAnimator.Play("MarcusGVPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in GV
                brutus.mapAnimator.Play("BrutusGVPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaVillage_challengeGame_2:
                // place julius in GV
                julius.mapAnimator.Play("JuliusGVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GV
                marcus.mapAnimator.Play("MarcusGVPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in GV
                brutus.mapAnimator.Play("BrutusGVPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaVillage_challengeGame_3:
                // place julius in GV
                julius.mapAnimator.Play("JuliusGVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GV
                marcus.mapAnimator.Play("MarcusGVPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GV
                brutus.mapAnimator.Play("BrutusGVPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.VillageChallengeDefeated:
                // place julius in GV
                julius.mapAnimator.Play("JuliusGVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GV
                marcus.mapAnimator.Play("MarcusGVPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GV
                brutus.mapAnimator.Play("BrutusGVPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.MudslideUnlocked:
                break;

            case StoryBeat.Mudslide_challengeGame_1:
                // place julius in MS
                julius.mapAnimator.Play("JuliusMSPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in MS
                marcus.mapAnimator.Play("MarcusMSPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in MS
                brutus.mapAnimator.Play("BrutusMSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.Mudslide_challengeGame_2:
                // place julius in MS
                julius.mapAnimator.Play("JuliusMSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MS
                marcus.mapAnimator.Play("MarcusMSPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in MS
                brutus.mapAnimator.Play("BrutusMSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.Mudslide_challengeGame_3:
                // place julius in MS
                julius.mapAnimator.Play("JuliusMSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MS
                marcus.mapAnimator.Play("MarcusMSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in MS
                brutus.mapAnimator.Play("BrutusMSPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.MudslideDefeated:
                // place julius in MS
                julius.mapAnimator.Play("JuliusMSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MS
                marcus.mapAnimator.Play("MarcusMSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in MS
                brutus.mapAnimator.Play("BrutusMSPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.OrcVillageMeetClogg:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.ShowExclamationMark(true);
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                break;

            case StoryBeat.OrcVillageUnlocked:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                break;

            case StoryBeat.OrcVillage_challengeGame_1:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in OV
                marcus.mapAnimator.Play("MarcusOVPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in OV
                brutus.mapAnimator.Play("BrutusOVPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.OrcVillage_challengeGame_2:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OV
                marcus.mapAnimator.Play("MarcusOVPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in OV
                brutus.mapAnimator.Play("BrutusOVPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.OrcVillage_challengeGame_3:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OV
                marcus.mapAnimator.Play("MarcusOVPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in OV
                brutus.mapAnimator.Play("BrutusOVPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.OrcVillageDefeated:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OV
                marcus.mapAnimator.Play("MarcusOVPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in OV
                brutus.mapAnimator.Play("BrutusOVPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.SpookyForestUnlocked:
            case StoryBeat.BeginningStoryGame:
                // place darwin in SF
                darwin.mapAnimator.Play("DarwinSFPos");
                darwin.FlipCharacterToRight();
                darwin.ShowExclamationMark(true);
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.SpookyForestPlayGames:
                // place darwin in SF
                darwin.mapAnimator.Play("DarwinSFPos");
                darwin.FlipCharacterToRight();
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.SpookyForest_challengeGame_1:
                // place julius in SF
                julius.mapAnimator.Play("JuliusSFPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in SF
                marcus.mapAnimator.Play("MarcusSFPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in SF
                brutus.mapAnimator.Play("BrutusSFPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.SpookyForest_challengeGame_2:
                // place julius in SF
                julius.mapAnimator.Play("JuliusSFPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in SF
                marcus.mapAnimator.Play("MarcusSFPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in SF
                brutus.mapAnimator.Play("BrutusSFPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.SpookyForest_challengeGame_3:
                // place julius in SF
                julius.mapAnimator.Play("JuliusSFPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in SF
                marcus.mapAnimator.Play("MarcusSFPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in SF
                brutus.mapAnimator.Play("BrutusSFPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.SpookyForestDefeated:
                // place julius in SF
                julius.mapAnimator.Play("JuliusSFPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in SF
                marcus.mapAnimator.Play("MarcusSFPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in SF
                brutus.mapAnimator.Play("BrutusSFPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.OrcCampUnlocked:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.ShowExclamationMark(true);
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                break;

            case StoryBeat.OrcCampPlayGames:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.interactable = true;
                break;

            case StoryBeat.OrcCamp_challengeGame_1:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.FlipCharacterToRight();
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in OC
                marcus.mapAnimator.Play("MarcusOCPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in OC
                brutus.mapAnimator.Play("BrutusOCPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.OrcCamp_challengeGame_2:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.FlipCharacterToRight();
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OC
                marcus.mapAnimator.Play("MarcusOCPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                clogg.FlipCharacterToRight();
                marcus.interactable = true;
                // place brutus in OC
                brutus.mapAnimator.Play("BrutusOCPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.OrcCamp_challengeGame_3:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.FlipCharacterToRight();
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OC
                marcus.mapAnimator.Play("MarcusOCPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in OC
                brutus.mapAnimator.Play("BrutusOCPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.OrcCampDefeated:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.GetComponent<Image>().raycastTarget = true;
                clogg.FlipCharacterToRight();
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OC
                marcus.mapAnimator.Play("MarcusOCPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in OC
                brutus.mapAnimator.Play("BrutusOCPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.GorillaPoopPlayGames:
                break;

            case StoryBeat.GorillaPoop_challengeGame_1:
                // place julius in GP
                julius.mapAnimator.Play("JuliusGPPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in GP
                marcus.mapAnimator.Play("MarcusGPPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in GP
                brutus.mapAnimator.Play("BrutusGPPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaPoop_challengeGame_2:
                // place julius in GP
                julius.mapAnimator.Play("JuliusGPPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GP
                marcus.mapAnimator.Play("MarcusGPPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in GP
                brutus.mapAnimator.Play("BrutusGPPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaPoop_challengeGame_3:
                // place julius in GP
                julius.mapAnimator.Play("JuliusGPPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GP
                marcus.mapAnimator.Play("MarcusGPPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GP
                brutus.mapAnimator.Play("BrutusGPPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.GorillaPoopDefeated:
                // place julius in GP
                julius.mapAnimator.Play("JuliusGPPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GP
                marcus.mapAnimator.Play("MarcusGPPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GP
                brutus.mapAnimator.Play("BrutusGPPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.WindyCliffUnlocked:
            case StoryBeat.FollowRedStoryGame:
                // place darwin in WC
                darwin.mapAnimator.Play("DarwinWCPos");
                darwin.ShowExclamationMark(true);
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.WindyCliffPlayGames:
                // place darwin in WC
                darwin.mapAnimator.Play("DarwinWCPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.WindyCliff_challengeGame_1:
                // place julius in WC
                julius.mapAnimator.Play("JuliusWCPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in WC
                marcus.mapAnimator.Play("MarcusWCPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in WC
                brutus.mapAnimator.Play("BrutusWCPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.WindyCliff_challengeGame_2:
                // place julius in WC
                julius.mapAnimator.Play("JuliusWCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in WC
                marcus.mapAnimator.Play("MarcusWCPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in WC
                brutus.mapAnimator.Play("BrutusWCPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.WindyCliff_challengeGame_3:
                // place julius in WC
                julius.mapAnimator.Play("JuliusWCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in WC
                marcus.mapAnimator.Play("MarcusWCPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in WC
                brutus.mapAnimator.Play("BrutusWCPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.WindyCliffDefeated:
                // place julius in WC
                julius.mapAnimator.Play("JuliusWCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in WC
                marcus.mapAnimator.Play("MarcusWCPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in WC
                brutus.mapAnimator.Play("BrutusWCPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.PirateShip_challengeGame_1:
                // place julius in PS
                julius.mapAnimator.Play("JuliusPSPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in PS
                marcus.mapAnimator.Play("MarcusPSPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in PS
                brutus.mapAnimator.Play("BrutusPSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.PirateShip_challengeGame_2:
                // place julius in PS
                julius.mapAnimator.Play("JuliusPSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in PS
                marcus.mapAnimator.Play("MarcusPSPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in PS
                brutus.mapAnimator.Play("BrutusPSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.PirateShip_challengeGame_3:
                // place julius in PS
                julius.mapAnimator.Play("JuliusPSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in PS
                marcus.mapAnimator.Play("MarcusPSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in PS
                brutus.mapAnimator.Play("BrutusPSPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.PirateShipDefeated:
                // place julius in PS
                julius.mapAnimator.Play("JuliusPSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in PS
                marcus.mapAnimator.Play("MarcusPSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in PS
                brutus.mapAnimator.Play("BrutusPSPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.MermaidBeachUnlocked:
            case StoryBeat.EmergingStoryGame:
                // place darwin in MB
                darwin.mapAnimator.Play("DarwinMBPos");
                darwin.ShowExclamationMark(true);
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.MermaidBeachPlayGames:
                // place darwin in MB
                darwin.mapAnimator.Play("DarwinMBPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.MermaidBeach_challengeGame_1:
                // place julius in MB
                julius.mapAnimator.Play("JuliusMBPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in MB
                marcus.mapAnimator.Play("MarcusMBPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in MB
                brutus.mapAnimator.Play("BrutusMBPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.MermaidBeach_challengeGame_2:
                // place julius in MB
                julius.mapAnimator.Play("JuliusMBPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MB
                marcus.mapAnimator.Play("MarcusMBPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in MB
                brutus.mapAnimator.Play("BrutusMBPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.MermaidBeach_challengeGame_3:
                // place julius in MMB
                julius.mapAnimator.Play("JuliusMBPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MB
                marcus.mapAnimator.Play("MarcusMBPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in MB
                brutus.mapAnimator.Play("BrutusMBPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.MermaidBeachDefeated:
                // place julius in MB
                julius.mapAnimator.Play("JuliusMBPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in MB
                marcus.mapAnimator.Play("MarcusMBPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in MB
                brutus.mapAnimator.Play("BrutusMBPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.Ruins_challengeGame_1:
                // place julius in R
                julius.mapAnimator.Play("JuliusRPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in R
                marcus.mapAnimator.Play("MarcusRPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in R
                brutus.mapAnimator.Play("BrutusRPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.Ruins_challengeGame_2:
                // place julius in R
                julius.mapAnimator.Play("JuliusRPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in R
                marcus.mapAnimator.Play("MarcusRPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in R
                brutus.mapAnimator.Play("BrutusRPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.Ruins_challengeGame_3:
                // place julius in R
                julius.mapAnimator.Play("JuliusRPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in R
                marcus.mapAnimator.Play("MarcusRPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in R
                brutus.mapAnimator.Play("BrutusRPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.RuinsDefeated:
                // place julius in R
                julius.mapAnimator.Play("JuliusRPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in R
                marcus.mapAnimator.Play("MarcusRPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in R
                brutus.mapAnimator.Play("BrutusRPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.ExitJungleUnlocked:
            case StoryBeat.ResolutionStoryGame:
                // place darwin in EJ
                darwin.mapAnimator.Play("DarwinEJPos");
                darwin.ShowExclamationMark(true);
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.ExitJunglePlayGames:
                // place darwin in EJ
                darwin.mapAnimator.Play("DarwinEJPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.ExitJungle_challengeGame_1:
                // place julius in EJ
                julius.mapAnimator.Play("JuliusEJPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in EJ
                marcus.mapAnimator.Play("MarcusEJPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in EJ
                brutus.mapAnimator.Play("BrutusEJPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.ExitJungle_challengeGame_2:
                // place julius in EJ
                julius.mapAnimator.Play("JuliusEJPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in EJ
                marcus.mapAnimator.Play("MarcusEJPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in EJ
                brutus.mapAnimator.Play("BrutusEJPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.ExitJungle_challengeGame_3:
                // place julius in EJ
                julius.mapAnimator.Play("JuliusEJPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in EJ
                marcus.mapAnimator.Play("MarcusEJPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in EJ
                brutus.mapAnimator.Play("BrutusEJPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.ExitJungleDefeated:
                // place julius in EJ
                julius.mapAnimator.Play("JuliusEJPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in EJ
                marcus.mapAnimator.Play("MarcusEJPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in EJ
                brutus.mapAnimator.Play("BrutusEJPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.GorillaStudyUnlocked:
                // place darwin in GS
                darwin.mapAnimator.Play("DarwinGSPos");
                darwin.ShowExclamationMark(true);
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.GorillaStudyPlayGames:
                // place darwin in GS
                darwin.mapAnimator.Play("DarwinGSPos");
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                break;

            case StoryBeat.GorillaStudy_challengeGame_1:
                // place julius in GS
                julius.mapAnimator.Play("JuliusGSPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in GS
                marcus.mapAnimator.Play("MarcusGSPos");
                marcus.characterAnimator.Play("marcusBroken");
                // place brutus in GS
                brutus.mapAnimator.Play("BrutusGSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaStudy_challengeGame_2:
                // place julius in GS
                julius.mapAnimator.Play("JuliusGSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GS
                marcus.mapAnimator.Play("MarcusGSPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in GS
                brutus.mapAnimator.Play("BrutusGSPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;

            case StoryBeat.GorillaStudy_challengeGame_3:
                // place julius in GS
                julius.mapAnimator.Play("JuliusGSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GS
                marcus.mapAnimator.Play("MarcusGSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GS
                brutus.mapAnimator.Play("BrutusGSPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.GorillaStudyDefeated:
                // place julius in GS
                julius.mapAnimator.Play("JuliusGSPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in GS
                marcus.mapAnimator.Play("MarcusGSPos");
                marcus.characterAnimator.Play("marcusFixed");
                // place brutus in GS
                brutus.mapAnimator.Play("BrutusGSPos");
                brutus.characterAnimator.Play("brutusFixed");
                break;

            case StoryBeat.MonkeysPlayGames:
                // place monkeys in pos
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.Monkeys_challengeGame_1:
                // place julius in M
                julius.mapAnimator.Play("JuliusMPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.Monkeys_challengeGame_2:
                // place julius in M
                julius.mapAnimator.Play("JuliusMPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.FlipCharacterToRight();
                marcus.ShowExclamationMark(true);
                marcus.GetComponent<Image>().raycastTarget = true;
                marcus.interactable = true;
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.Monkeys_challengeGame_3:
                // place julius in M
                julius.mapAnimator.Play("JuliusMPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusFixed");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusWin");
                brutus.FlipCharacterToRight();
                brutus.ShowExclamationMark(true);
                brutus.GetComponent<Image>().raycastTarget = true;
                brutus.interactable = true;
                break;

            case StoryBeat.MonkeysDefeated:
                // place julius in M
                julius.mapAnimator.Play("JuliusMPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusFixed");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusFixed");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.PalaceIntro:
                // place taxi bird in PI
                taxiBird.mapAnimator.Play("TaxiBirdPIPos");
                taxiBird.GetComponent<Image>().raycastTarget = true;
                taxiBird.interactable = true;
                // place darwin in PI
                darwin.mapAnimator.Play("DarwinPIPos");
                darwin.FlipCharacterToLeft();
                darwin.GetComponent<Image>().raycastTarget = true;
                darwin.interactable = true;
                darwin.ShowExclamationMark(true);
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.PreBossBattle:
                // place taxi bird in PI
                taxiBird.mapAnimator.Play("TaxiBirdPIPos");
                taxiBird.GetComponent<Image>().raycastTarget = true;
                taxiBird.interactable = true;
                // place darwin in P
                darwin.FlipCharacterToRight();
                darwin.mapAnimator.Play("DarwinPPos");
                darwin.interactable = false;
                darwin.GetComponent<Image>().raycastTarget = false;
                // place julius in P
                julius.ShowExclamationMark(true);
                julius.mapAnimator.Play("JuliusPPos");
                julius.characterAnimator.Play("sTigerIdle");
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.BossBattle1:
            case StoryBeat.BossBattle2:
            case StoryBeat.BossBattle3:
                // place taxi bird in PI
                taxiBird.mapAnimator.Play("TaxiBirdPIPos");
                taxiBird.GetComponent<Image>().raycastTarget = true;
                taxiBird.interactable = true;
                // place darwin in P
                darwin.FlipCharacterToRight();
                darwin.mapAnimator.Play("DarwinPPos");
                darwin.GetComponent<Image>().raycastTarget = false;
                darwin.interactable = false;
                // place julius in P
                julius.ShowExclamationMark(true);
                julius.mapAnimator.Play("JuliusPPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.GetComponent<Image>().raycastTarget = true;
                julius.interactable = true;
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.EndBossBattle:
                // place taxi bird in PI
                taxiBird.mapAnimator.Play("TaxiBirdPIPos");
                taxiBird.GetComponent<Image>().raycastTarget = true;
                taxiBird.interactable = true;
                // place darwin in P
                darwin.FlipCharacterToRight();
                darwin.mapAnimator.Play("DarwinPPos");
                darwin.GetComponent<Image>().raycastTarget = false;
                darwin.interactable = false;
                // place julius in P
                julius.mapAnimator.Play("JuliusPPos");
                julius.characterAnimator.Play("sTigerIdle");
                julius.interactable = false;
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

            case StoryBeat.FinishedGame:
                // place taxi bird in PI
                taxiBird.mapAnimator.Play("TaxiBirdPIPos");
                taxiBird.GetComponent<Image>().raycastTarget = true;
                taxiBird.interactable = true;
                // place marcus in M
                marcus.mapAnimator.Play("MarcusMPos");
                marcus.characterAnimator.Play("marcusBroken");
                marcus.FlipCharacterToRight();
                // place brutus in M
                brutus.mapAnimator.Play("BrutusMPos");
                brutus.characterAnimator.Play("brutusBroken");
                brutus.FlipCharacterToRight();
                break;

        }

    }

    public void PlayChallengeGameMapAnim(MapAnim animation, MapLocation location)
    {
        animationDone = false;

        switch (animation)
        {
            case MapAnim.ChallengeGame1:
                StartCoroutine(ChallengeGame1Routine(location));
                break;

            case MapAnim.ChallengeGame2:
                StartCoroutine(ChallengeGame2Routine(location));
                break;

            case MapAnim.ChallengeGame3:
                StartCoroutine(ChallengeGame3Routine(location));
                break;
        }
    }

    public void PlayPreBossBattleGameMapAnim(MapAnim animation)
    {
        animationDone = false;

        switch (animation)
        {
            case MapAnim.BossBattle1:
                StartCoroutine(PreBossBattle1Routine());
                break;

            case MapAnim.BossBattle2:
                StartCoroutine(PreBossBattle2Routine());
                break;

            case MapAnim.BossBattle3:
                StartCoroutine(PreBossBattle3Routine());
                break;
        }
    }

    public void PlayBossBattleGame(MapAnim animation)
    {
        animationDone = false;

        switch (animation)
        {
            case MapAnim.BossBattle1:
            case MapAnim.BossBattle2:
            case MapAnim.BossBattle3:
                StartCoroutine(BossBattleRoutine(animation));
                break;
        }
    }

    public void PlayMapAnim(MapAnim animation)
    {
        animationDone = false;

        // hide stars on map
        ScrollMapManager.instance.HideStarsAtCurrentLocation();

        // play appropriate animation routine
        switch (animation)
        {
            case MapAnim.BoatIntro:
                StartCoroutine(BoatIntro());
                break;

            case MapAnim.RevealGorillaVillage:
                StartCoroutine(RevealGorillaVillage());
                break;

            case MapAnim.GorillaVillageIntro:
                StartCoroutine(GorillaVillageIntro());
                break;

            case MapAnim.RedShowsStickerButton:
                StartCoroutine(RedShowsStickerButton());
                break;

            case MapAnim.DarwinForcesLesterInteraction:
                StartCoroutine(DarwinForcesLesterInteraction());
                break;

            case MapAnim.GorillaVillageRebuilt:
                StartCoroutine(GorillaVillageRebuilt());
                break;

            case MapAnim.GorillaVillageDefeated:
                StartCoroutine(GorillaVillageDefeated());
                break;

            case MapAnim.MudslideRebuilt:
                StartCoroutine(MudslideRebuilt());
                break;

            case MapAnim.MudslideDefeated:
                StartCoroutine(MudslideDefeated());
                break;

            case MapAnim.OrcVillageRebuilt:
                StartCoroutine(OrcVillageRebuilt());
                break;

            case MapAnim.OrcVillageDefeated:
                StartCoroutine(OrcVillageDefeated());
                break;

            case MapAnim.SpookyForestIntro:
                StartCoroutine(SpookyForestIntro());
                break;

            case MapAnim.SpookyForestRebuilt:
                StartCoroutine(SpookyForestRebuilt());
                break;

            case MapAnim.SpookyForestDefeated:
                StartCoroutine(SpookyForestDefeated());
                break;

            case MapAnim.OrcCampRebuilt:
                StartCoroutine(OrcCampRebuilt());
                break;

            case MapAnim.OrcCampDefeated:
                StartCoroutine(OrcCampDefeated());
                break;

            case MapAnim.GorillaPoopRebuilt:
                StartCoroutine(GorillaPoopRebuilt());
                break;

            case MapAnim.GorillaPoopDefeated:
                StartCoroutine(GorillaPoopDefeated());
                break;

            case MapAnim.WindyCliffIntro:
                StartCoroutine(WindyCliffIntro());
                break;

            case MapAnim.WindyCliffRebuilt:
                StartCoroutine(WindyCliffRebuilt());
                break;

            case MapAnim.WindyCliffDefeated:
                StartCoroutine(WindyCliffDefeated());
                break;

            case MapAnim.PirateShipRebuilt:
                StartCoroutine(PirateShipRebuilt());
                break;

            case MapAnim.PirateShipDefeated:
                StartCoroutine(PirateShipDefeated());
                break;

            case MapAnim.MermaidBeachIntro:
                StartCoroutine(MermaidBeachIntro());
                break;

            case MapAnim.MermaidBeachRebuilt:
                StartCoroutine(MermaidBeachRebuilt());
                break;

            case MapAnim.MermaidBeachDefeated:
                StartCoroutine(MermaidBeachDefeated());
                break;

            case MapAnim.RuinsRebuilt:
                StartCoroutine(RuinsRebuilt());
                break;

            case MapAnim.RuinsDefeated:
                StartCoroutine(RuinsDefeated());
                break;

            case MapAnim.ExitJungleIntro:
                StartCoroutine(ExitJungleIntro());
                break;

            case MapAnim.ExitJungleRebuilt:
                StartCoroutine(ExitJungleRebuilt());
                break;

            case MapAnim.ExitJungleDefeated:
                StartCoroutine(ExitJungleDefeated());
                break;

            case MapAnim.GorillaStudyIntro:
                StartCoroutine(GorillaStudyIntro());
                break;

            case MapAnim.GorillaStudyRebuilt:
                StartCoroutine(GorillaStudyRebuilt());
                break;

            case MapAnim.GorillaStudyDefeated:
                StartCoroutine(GorillaStudyDefeated());
                break;

            case MapAnim.MonkeysRebuilt:
                StartCoroutine(MonkeysRebuilt());
                break;

            case MapAnim.MonkeysDefeated:
                StartCoroutine(MonkeysDefeated());
                break;

            case MapAnim.PalaceIntro:
                StartCoroutine(PalaceIntro());
                break;

            case MapAnim.PreBossBattle:
                StartCoroutine(PreBossBattle());
                break;

            case MapAnim.EndBossBattle:
                StartCoroutine(EndBossBattle());
                break;

            default:
                Debug.LogError("Could not start map animation: " + animation);
                break;
        }
    }

    // get the length of an animation clip
    public float GetAnimationTime(Animator anim, string animName)
    {
        RuntimeAnimatorController rac = anim.runtimeAnimatorController;
        foreach (var clip in rac.animationClips)
        {
            if (clip.name == animName)
            {
                return clip.length;
            }
        }

        return 0f;
    }

    private IEnumerator BoatIntro()
    {
        yield return new WaitForSeconds(1f);

        boat.mapAnimator.Play("BoatIntro");
        yield return new WaitForSeconds(GetAnimationTime(boat.mapAnimator, "BoatIntro"));

        // wiggle boat
        boat.ShowExclamationMark(true);
        boat.GetComponent<Image>().raycastTarget = true;
        boat.GetComponent<MapIcon>().interactable = true;
        boat.GetComponent<MapIcon>().fixedCollider.enabled = true;
        boat.GetComponent<WiggleController>().StartWiggle();

        animationDone = true;
    }

    private IEnumerator RevealGorillaVillage()
    {
        yield return new WaitForSeconds(1f);

        boat.mapAnimator.Play("BoatDock");
        yield return new WaitForSeconds(GetAnimationTime(boat.mapAnimator, "BoatDock"));

        // play dock 1 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("Dock_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // gorilla exclamation point
        darwin.mapAnimator.Play("DarwinGVPos");

        // unlock gorilla village
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaVillage, true);
        yield return new WaitForSeconds(10f);

        // play dock 2 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("Dock_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // advance story beat
        StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.GorillaVillage);

        yield return new WaitForSeconds(1f);

        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);

        animationDone = true;
    }

    private IEnumerator GorillaVillageIntro()
    {
        // remove exclamation mark from gorilla
        darwin.ShowExclamationMark(false);
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // play gorilla intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GorillaIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // play gorilla intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GorillaIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInGV");
        marcus.mapAnimator.Play("MarcusWalkInGV");
        brutus.mapAnimator.Play("BrutusWalkInGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInGV"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // destroy village
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.DestroyArea, 0.5f);
        // destroy gorilla village assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[2].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // turn gorilla to face right
        darwin.FlipCharacterToRight();

        // play gorilla intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GorillaIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGV"));

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutGV");
        brutus.mapAnimator.Play("BrutusWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "MarcusWalkOutGV"));

        // play gorilla intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GorillaIntro_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // play gorilla intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SectionIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make gorilla interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // save to SIS and exit to scroll map
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed = false;

        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.GorillaVillage);
        ScrollMapManager.instance.UpdateMapIcons(true);

        animationDone = true;
    }

    private IEnumerator RedShowsStickerButton()
    {
        // play red notices lester talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RedLester_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // unlock button in SIS
        StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
        // add glow + wiggle
        StickerSystem.instance.ToggleStickerButtonWiggleGlow(true);

        // save to sis and continue
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator DarwinForcesLesterInteraction()
    {
        // play darwin forces talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ForceLester_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // add glow + wiggle
        StickerSystem.instance.ToggleStickerButtonWiggleGlow(true);

        animationDone = true;
    }

    private IEnumerator GorillaVillageRebuilt()
    {
        // make darwin inactive
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        yield return new WaitForSeconds(1f);

        // play village rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutGV"));

        // play village rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInGV");
        marcus.mapAnimator.Play("MarcusWalkInGV");
        brutus.mapAnimator.Play("BrutusWalkInGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInGV"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play village rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.GorillaVillage);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.characterAnimator.Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator GorillaVillageDefeated()
    {
        // play village challenge 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGV"));

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutGV");
        brutus.mapAnimator.Play("BrutusWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGV"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play village challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // gv sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaVillage].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);

        // // place temp copy over talkie bg
        // var tempSignPost = TempObjectPlacer.instance.PlaceNewObject(mapIconsAtLocation[2].signPost.gameObject, mapIconsAtLocation[2].signPost.transform.localPosition);
        // tempSignPost.GetComponent<SignPostController>().interactable = false;
        // tempSignPost.GetComponent<SignPostController>().SetStars(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars);

        // play village challenge 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("VillageDefeated_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // // remove temp temp signpost
        // TempObjectPlacer.instance.RemoveObject();

        // before unlocking mudslide - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[(int)MapLocation.Mudslide].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock mudslide
        ScrollMapManager.instance.UnlockMapArea(MapLocation.Mudslide, false);
        yield return new WaitForSeconds(10f);

        // play mudslide intro talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaVillage].signPost.HideSignPost();

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.Mudslide);
        ScrollMapManager.instance.UpdateMapIcons(true);
        ScrollMapManager.instance.RevealStarsAtCurrentLocation();
        StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.Mudslide);

        animationDone = true;
    }

    private IEnumerator MudslideRebuilt()
    {
        // play mudslide rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInMS");
        marcus.mapAnimator.Play("MarcusWalkInMS");
        brutus.mapAnimator.Play("BrutusWalkInMS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInMS"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play mudslide rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.Mudslide);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator MudslideDefeated()
    {
        // play mudslide defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMS"));

        // play mudslide challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutMS");
        brutus.mapAnimator.Play("BrutusWalkOutMS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutMS"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play mudslide challenge 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MudslideDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // MS sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Mudslide].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking orc village - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[4].mapIcons)
            icon.SetFixed(false, false, true);

        // place clogg in village
        clogg.mapAnimator.Play("CloggOVPos");

        // unlock orc village
        ScrollMapManager.instance.UnlockMapArea(MapLocation.OrcVillage, false);
        yield return new WaitForSeconds(10f);

        // play orc village intro talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OVillageIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Mudslide].signPost.HideSignPost();

        // make clogg interactable
        clogg.GetComponent<Image>().raycastTarget = true;
        clogg.interactable = true;
        clogg.ShowExclamationMark(true);

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
        StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.OrcVillage);

        animationDone = true;
    }

    private IEnumerator OrcVillageRebuilt()
    {
        // play orc village rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OVillageRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInOV");
        marcus.mapAnimator.Play("MarcusWalkInOV");
        brutus.mapAnimator.Play("BrutusWalkInOV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInOV"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // turn gorilla to face right
        clogg.FlipCharacterToRight();

        // play orc village rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OVillageRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.OrcVillage);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator OrcVillageDefeated()
    {
        // play orc village defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OVillageDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutOV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutOV"));

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutOV");
        brutus.mapAnimator.Play("BrutusWalkOutOV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutOV"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play orc village defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OVillageDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // OV sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcVillage].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking spooky forest - set objects to be repaired
        foreach (var icon in ScrollMapManager.instance.mapLocations[5].mapIcons)
            icon.SetFixed(true, false, true);

        // place darwin in spooky forest
        darwin.mapAnimator.Play("DarwinSFPos");
        darwin.FlipCharacterToRight();
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // unlock spooky forest
        ScrollMapManager.instance.UnlockMapArea(MapLocation.SpookyForest, false);
        yield return new WaitForSeconds(10f);

        // darwin is interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcVillage].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2; // new chapter!
        StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.SpookyForest);

        animationDone = true;
    }

    private IEnumerator SpookyForestIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // spider intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInSF");
        marcus.mapAnimator.Play("MarcusWalkInSF");
        brutus.mapAnimator.Play("BrutusWalkInSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInSF"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // spider intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy spider forest
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.DestroyArea, 0.5f);
        // destroy spider forest assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[5].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // spider intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutSF"));

        // spider intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutSF");
        brutus.mapAnimator.Play("BrutusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "MarcusWalkOutSF"));

        // spider intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p5"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // spider intro 6
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderIntro_1_p6"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed = false;

        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);

        animationDone = true;
    }

    private IEnumerator SpookyForestRebuilt()
    {
        // play spooky forest rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutSF"));

        // play spooky forest rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInSF");
        marcus.mapAnimator.Play("MarcusWalkInSF");
        brutus.mapAnimator.Play("BrutusWalkInSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInSF"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play spooky forest rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.SpookyForest);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator SpookyForestDefeated()
    {
        // play spooky forest defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutSF"));

        // play spooky forest challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutSF");
        brutus.mapAnimator.Play("BrutusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutSF"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play spooky forest challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("SpiderDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.SpookyForest].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking orc camp - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[6].mapIcons)
            icon.SetFixed(false, false, true);

        // place clogg in orc camp
        clogg.mapAnimator.Play("CloggOCPos");
        clogg.GetComponent<Image>().raycastTarget = false;
        clogg.interactable = false;

        // unlock orc camp
        ScrollMapManager.instance.UnlockMapArea(MapLocation.OrcCamp, false);
        yield return new WaitForSeconds(10f);

        // clogg is interactable
        clogg.ShowExclamationMark(true);
        clogg.GetComponent<Image>().raycastTarget = true;
        clogg.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.SpookyForest].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.OrcCamp);

        animationDone = true;
    }

    private IEnumerator OrcCampRebuilt()
    {
        // play spooky forest rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OCampRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInOC");
        marcus.mapAnimator.Play("MarcusWalkInOC");
        brutus.mapAnimator.Play("BrutusWalkInOC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInOC"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // turn gorilla to face right
        clogg.FlipCharacterToRight();

        // play spooky forest rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OCampRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.OrcCamp);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator OrcCampDefeated()
    {
        // play OC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OCampDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutOC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutOC"));

        // play OC challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OCampDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutOC");
        brutus.mapAnimator.Play("BrutusWalkOutOC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutOC"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play spooky forest challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("OCampDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // OC sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcCamp].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking gorilla poop - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[7].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock gorilla poop
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaPoop, false);
        yield return new WaitForSeconds(10f);

        // play gorilla poop intro
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.OrcCamp].signPost.HideSignPost();

        // enable icons in GP
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.GorillaPoop);
        ScrollMapManager.instance.UpdateMapIcons(true);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 7;
        StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.GorillaPoop);

        animationDone = true;
    }

    private IEnumerator GorillaPoopRebuilt()
    {
        // play GP rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInGP");
        marcus.mapAnimator.Play("MarcusWalkInGP");
        brutus.mapAnimator.Play("BrutusWalkInGP");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInGP"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play spooky forest rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.GorillaPoop);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator GorillaPoopDefeated()
    {
        // play GP defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutGP");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGP"));

        // play OC challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutGP");
        brutus.mapAnimator.Play("BrutusWalkOutGP");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGP"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play GP challenge 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PoopDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaPoop].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // place darwin in WC
        darwin.mapAnimator.Play("DarwinWCPos");
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // before unlocking windy cliff - set objects to be fixed
        foreach (var icon in ScrollMapManager.instance.mapLocations[(int)MapLocation.WindyCliff].mapIcons)
            icon.SetFixed(true, false, true);

        // unlock windy cliff
        ScrollMapManager.instance.UnlockMapArea(MapLocation.WindyCliff, false);
        yield return new WaitForSeconds(10f);

        // make darwin interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaPoop].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 8;
        StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_3; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.WindyCliff);

        animationDone = true;
    }

    private IEnumerator WindyCliffIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // windy cliff intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInWC");
        marcus.mapAnimator.Play("MarcusWalkInWC");
        brutus.mapAnimator.Play("BrutusWalkInWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInWC"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        darwin.FlipCharacterToRight();

        // windy cliff intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy cliff
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.DestroyArea, 0.5f);
        // destroy cliff assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[8].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // windy cliff intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

        // windy cliff intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffIntro_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutWC");
        brutus.mapAnimator.Play("BrutusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

        // spider intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffIntro_1_p5"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.isFixed = false;

        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToRight();

        animationDone = true;
    }

    private IEnumerator WindyCliffRebuilt()
    {
        // play WC rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutWC"));

        // play WC rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInWC");
        marcus.mapAnimator.Play("MarcusWalkInWC");
        brutus.mapAnimator.Play("BrutusWalkInWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInWC"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play WC rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.WindyCliff);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator WindyCliffDefeated()
    {
        // play WC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

        // play WC defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutWC");
        brutus.mapAnimator.Play("BrutusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutWC"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play WC defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // WC sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.WindyCliff].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);

        // before unlocking pirate ship - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[9].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock pirate ship
        ScrollMapManager.instance.UnlockMapArea(MapLocation.PirateShip, false);
        yield return new WaitForSeconds(10f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.WindyCliff].signPost.HideSignPost();

        // play PS intro 
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PirateIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // enable icons in PS
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.PirateShip);
        ScrollMapManager.instance.UpdateMapIcons(true);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 9;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.PirateShip);

        animationDone = true;
    }

    private IEnumerator PirateShipRebuilt()
    {
        // play PS rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PirateRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInPS");
        marcus.mapAnimator.Play("MarcusWalkInPS");
        brutus.mapAnimator.Play("BrutusWalkInPS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInPS"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play PS rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PirateRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.PirateShip);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator PirateShipDefeated()
    {
        // play WC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PirateDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play WC defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("CliffDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutPS");
        brutus.mapAnimator.Play("BrutusWalkOutPS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutPS"));

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutPS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutPS"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play PS defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PirateDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // PS sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.PirateShip].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);

        // place darwin in MB
        darwin.mapAnimator.Play("DarwinMBPos");
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // unlock mermaid beach
        ScrollMapManager.instance.UnlockMapArea(MapLocation.MermaidBeach, false);
        yield return new WaitForSeconds(10f);

        // make darwin interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.PirateShip].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 10;
        StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_4; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.MermaidBeach);

        animationDone = true;
    }

    private IEnumerator MermaidBeachIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // mermaid beach intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInMB");
        marcus.mapAnimator.Play("MarcusWalkInMB");
        brutus.mapAnimator.Play("BrutusWalkInMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInMB"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        darwin.FlipCharacterToRight();

        // mermaid beach intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy mermaid beach
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.DestroyArea, 0.5f);
        // destroy mermaid beach assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[10].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // mermaid beach intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMB"));

        // mermaid beach intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidIntro_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutMB");
        brutus.mapAnimator.Play("BrutusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "MarcusWalkOutMB"));

        // mermaid beach intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidIntro_1_p5"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.isFixed = false;

        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToLeft();

        animationDone = true;
    }

    private IEnumerator MermaidBeachRebuilt()
    {
        // play MB rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutMB"));

        // play MB rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInMB");
        marcus.mapAnimator.Play("MarcusWalkInMB");
        brutus.mapAnimator.Play("BrutusWalkInMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInMB"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play MB rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.MermaidBeach);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.interactable = true;
        julius.GetComponent<Image>().raycastTarget = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator MermaidBeachDefeated()
    {
        // play MB defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMB"));

        // play MB defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutMB");
        brutus.mapAnimator.Play("BrutusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutMB"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play MB defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MermaidDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // MB sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.MermaidBeach].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking ruins 1 - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[11].mapIcons)
            icon.SetFixed(false, false, true);
        // before unlocking ruins 2 - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[12].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock ruins
        ScrollMapManager.instance.UnlockMapArea(MapLocation.Ruins2, false);
        yield return new WaitForSeconds(10f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.MermaidBeach].signPost.HideSignPost();

        // play R intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.Ruins2);
        ScrollMapManager.instance.UpdateMapIcons(true);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 12;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.Ruins1);

        animationDone = true;
    }

    private IEnumerator RuinsRebuilt()
    {
        // make sure scroll map is on Ruins 1!!!
        ScrollMapManager.instance.GoToMapPosition(MapLocation.Ruins1);
        yield return new WaitForSeconds(2f);

        // play R rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInR");
        marcus.mapAnimator.Play("MarcusWalkInR");
        brutus.mapAnimator.Play("BrutusWalkInR");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInR"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play R rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.Ruins1);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.interactable = true;
        julius.GetComponent<Image>().raycastTarget = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator RuinsDefeated()
    {
        // make sure scroll map is on Ruins 1!!!
        ScrollMapManager.instance.GoToMapPosition(MapLocation.Ruins1);
        yield return new WaitForSeconds(2f);

        // play R defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutR");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutR"));

        // play R defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutR");
        brutus.mapAnimator.Play("BrutusWalkOutR");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutR"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play R defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RuinsDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // R sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Ruins1].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // place darwin in EJ
        darwin.mapAnimator.Play("DarwinEJPos");

        // before unlocking exit jungle - set objects to be fixed
        foreach (var icon in ScrollMapManager.instance.mapLocations[(int)MapLocation.ExitJungle].mapIcons)
            icon.SetFixed(true, false, true);

        // unlock exit jungle
        ScrollMapManager.instance.UnlockMapArea(MapLocation.ExitJungle, false);
        yield return new WaitForSeconds(10f);

        // make darwin interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Ruins1].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 13;
        StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_5; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.ExitJungle);

        animationDone = true;
    }

    private IEnumerator ExitJungleIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // exit jungle intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInEJ");
        marcus.mapAnimator.Play("MarcusWalkInEJ");
        brutus.mapAnimator.Play("BrutusWalkInEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInEJ"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        darwin.FlipCharacterToRight();

        // exit jungle intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy exit jungle
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.DestroyArea, 0.5f);
        // destroy mermaid beach assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[13].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // exit jungle intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutEJ"));

        // exit jungle intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitIntro_1_p4"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutEJ");
        brutus.mapAnimator.Play("BrutusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "MarcusWalkOutEj"));

        // exit jungle intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitIntro_1_p5"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.isFixed = false;

        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.ShowExclamationMark(true);

        animationDone = true;
    }

    private IEnumerator ExitJungleRebuilt()
    {
        // play EJ rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutEJ"));

        // play EJ rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInEJ");
        marcus.mapAnimator.Play("MarcusWalkInEJ");
        brutus.mapAnimator.Play("BrutusWalkInEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInEJ"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play EJ rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.ExitJungle);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator ExitJungleDefeated()
    {
        // play EJ defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutEJ"));

        // play EJ defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutEJ");
        brutus.mapAnimator.Play("BrutusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutEJ"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play MB defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ExitDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // EJ sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.ExitJungle].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking gorilla study - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[14].mapIcons)
            icon.SetFixed(false, false, true);

        // place darwin in gorilla study
        darwin.mapAnimator.Play("DarwinGSPos");
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // unlock gorilla study
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaStudy, false);
        yield return new WaitForSeconds(10f);

        // make darwin interactable
        darwin.ShowExclamationMark(true);
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.ExitJungle].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 14;
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.GorillaStudy);

        animationDone = true;
    }

    private IEnumerator GorillaStudyIntro()
    {
        darwin.interactable = false;
        darwin.GetComponent<Image>().raycastTarget = false;
        darwin.ShowExclamationMark(false);

        // gorilla study intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.interactable = true;

        // change enabled map sections
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.GorillaStudy);
        ScrollMapManager.instance.UpdateMapIcons(true);

        animationDone = true;
    }

    private IEnumerator GorillaStudyRebuilt()
    {
        // play GS rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutGS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutGS"));

        // play GS rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        marcus.characterAnimator.Play("marcusWalkIn");
        brutus.characterAnimator.Play("brutusWalkIn");

        julius.mapAnimator.Play("JuliusWalkInGS");
        marcus.mapAnimator.Play("MarcusWalkInGS");
        brutus.mapAnimator.Play("BrutusWalkInGS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInGS"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play GS rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampRebuilt_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.GorillaStudy);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator GorillaStudyDefeated()
    {
        // play GS defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutGS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGS"));

        // play GS defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        yield return new WaitForSeconds(1f);

        marcus.characterAnimator.Play("marcusTurn");
        brutus.characterAnimator.Play("brutusTurn");

        yield return new WaitForSeconds(0.25f);

        marcus.mapAnimator.Play("MarcusWalkOutGS");
        brutus.mapAnimator.Play("BrutusWalkOutGS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(marcus.mapAnimator, "MarcusWalkOutGS"));

        // place tiger and monkies off screen
        julius.transform.localScale = Vector3.zero;
        marcus.transform.localScale = Vector3.zero;
        brutus.transform.localScale = Vector3.zero;

        julius.mapAnimator.Play("JuliusOffScreenPos");
        marcus.mapAnimator.Play("MarcusOffScreenPos");
        brutus.mapAnimator.Play("BrutusOffScreenPos");

        yield return new WaitForSeconds(0.1f);

        julius.transform.localScale = Vector3.one;
        marcus.transform.localScale = Vector3.one;
        brutus.transform.localScale = Vector3.one;

        // play GS defeated 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("GCampDefeated_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // GS sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaStudy].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);


        // before unlocking monkeys - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[15].mapIcons)
            icon.SetFixed(false, false, true);

        // place monkeys in pos
        marcus.mapAnimator.Play("MarcusMPos");
        marcus.characterAnimator.Play("marcusBroken");
        marcus.FlipCharacterToRight();
        brutus.mapAnimator.Play("BrutusMPos");
        brutus.characterAnimator.Play("brutusBroken");
        brutus.FlipCharacterToRight();

        // unlock monkeys
        ScrollMapManager.instance.UnlockMapArea(MapLocation.Monkeys, false);
        yield return new WaitForSeconds(10f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaStudy].signPost.HideSignPost();

        // play M intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyIntro_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play M intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyIntro_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play M intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyIntro_1_p3"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 15;
        StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // change enabled map sections
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.Monkeys);
        ScrollMapManager.instance.UpdateMapIcons(true);

        // update settings map
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.Monkeys);
        ScrollSettingsWindowController.instance.UpdateMapSprite();

        animationDone = true;
    }

    private IEnumerator MonkeysRebuilt()
    {
        // play M rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyRebuilt_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger and monkies walk in
        julius.characterAnimator.Play("tigerWalk");
        julius.mapAnimator.Play("JuliusWalkInM");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkInM"));

        // idle animations
        julius.characterAnimator.Play("aTigerIdle");
        marcus.characterAnimator.Play("marcusBroken");
        brutus.characterAnimator.Play("brutusBroken");

        yield return new WaitForSeconds(0.5f);

        // play M rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyRebuilt_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make challenge games active
        yield return new WaitForSeconds(0.5f);

        // set julius challenge game
        SetJuliusChallengeGame(MapLocation.Monkeys);

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        julius.ShowExclamationMark(true);
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator MonkeysDefeated()
    {
        // play GS defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyDefeated_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutM");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutM"));

        // play GS defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("MonkeyDefeated_1_p2"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // monkies go hehe and haha then run off too
        marcus.characterAnimator.Play("marcusWin");
        brutus.characterAnimator.Play("brutusWin");

        // place tiger off screen
        julius.transform.localScale = Vector3.zero;
        julius.mapAnimator.Play("JuliusOffScreenPos");
        yield return new WaitForSeconds(0.1f);
        julius.transform.localScale = Vector3.one;

        // M sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Monkeys].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_stars, false);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Trumpet, 0.25f);
        yield return new WaitForSeconds(1f);

        // place taxi bird in PI
        taxiBird.mapAnimator.Play("TaxiBirdPIPos");
        taxiBird.GetComponent<Image>().raycastTarget = true;
        taxiBird.interactable = true;
        // place darwin in PI
        darwin.mapAnimator.Play("DarwinPIPos");
        darwin.GetComponent<Image>().raycastTarget = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToLeft();
        darwin.interactable = true;
        // place julius in P
        julius.mapAnimator.Play("JuliusPPos");
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;

        // unlock palace
        ScrollMapManager.instance.UnlockMapArea(MapLocation.PalaceIntro, false);
        yield return new WaitForSeconds(10f);

        // hide signpost
        ScrollMapManager.instance.mapLocations[(int)MapLocation.Monkeys].signPost.HideSignPost();

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 16;
        StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_6; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // update settings map
        ScrollSettingsWindowController.instance.UpdateMapSprite();
        ScrollSettingsWindowController.instance.UpdateRedPos(MapLocation.PalaceIntro);

        animationDone = true;
    }

    private IEnumerator PalaceIntro()
    {
        // remove darwin's exclamation point
        darwin.ShowExclamationMark(false);
        julius.GetComponent<Image>().raycastTarget = false;
        darwin.interactable = false;

        // play Final Boss 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("FinalBoss_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin into palace
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutPI");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutPI"));

        // place darwin in palace pos
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaIdle");
        darwin.mapAnimator.Play("DarwinPPos");

        // place julius in palace pos
        julius.ShowExclamationMark(true);
        julius.characterAnimator.Play("sTigerIdle");
        julius.mapAnimator.Play("JuliusPPos");
        julius.GetComponent<Image>().raycastTarget = true;
        julius.interactable = true;

        // show arrow to go up to palace
        PalaceArrow.instance.ShowArrow();
        PalaceArrow.instance.GetComponent<WiggleController>().StartWiggle();

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator PreBossBattle()
    {
        // remove julius's exclamation point
        julius.ShowExclamationMark(false);

        // remove down arrow
        PalaceArrowDown.instance.HideArrow();

        // play Final Boss 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("FinalBoss_2_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // julius angry and ready to fight
        julius.characterAnimator.Play("aTigerTwitch");
        julius.ShowExclamationMark(true);

        yield return new WaitForSeconds(1f);

        // show down arrow
        PalaceArrowDown.instance.ShowArrow();

        // show boss battle bar
        BossBattleBar.instance.ShowBar();

        // advance story beat
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator EndBossBattle()
    {
        yield return new WaitForSeconds(1f);

        // play last win challenge game
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBWin_1_p1"));
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin towards julius
        darwin.mapAnimator.Play("DarwinWalkTowardsJulius");
        darwin.characterAnimator.Play("gorillaWalk");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkTowardsJulius"));

        // move darwin towards julius
        darwin.mapAnimator.Play("DarwinNextJuliusPos");
        darwin.characterAnimator.Play("gorillaIdle");

        yield return new WaitForSeconds(1f);

        // play end talkies
        GameManager.instance.LoadScene("EndScene", true, 0.5f, false);

        animationDone = true;
    }


















    /* 
    ################################################
    #   BOSS BATTLE GAMES
    ################################################
    */

    private IEnumerator BossBattleRoutine(MapAnim mapAnim)
    {
        // remove julius exclamation mark
        julius.ShowExclamationMark(false);

        if (mapAnim == MapAnim.BossBattle1)
        {
            // play julius try again talkie
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle || StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
            {
                int random = Random.Range(0, 2);

                if (random == 0)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p1"));
                }
                else if (random == 1)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p2"));
                }
            }
            else
            {
                // play BBChallenge_1_p1 talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBChallenge_1_p1"));
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (mapAnim == MapAnim.BossBattle2)
        {
            // play julius try again talkie
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle || StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
            {
                int random = Random.Range(0, 2);

                if (random == 0)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p1"));
                }
                else if (random == 1)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p2"));
                }
            }
            else
            {
                // play BBChallenge_1_p2 talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBChallenge_1_p2"));
            }
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (mapAnim == MapAnim.BossBattle3)
        {

            // play julius try again talkie
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle || StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
            {
                int random = Random.Range(0, 2);

                if (random == 0)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p1"));
                }
                else if (random == 1)
                {
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBTryAgain_1_p2"));
                }
            }
            else
            {
                // play BBChallenge_1_p3 talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBChallenge_1_p3"));
            }
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else
        {
            yield break;
        }

        if (TalkieManager.instance.yesNoChoices.Count == 1)
        {
            // if player chooses yes
            if (TalkieManager.instance.yesNoChoices[0])
            {
                TalkieManager.instance.yesNoChoices.Clear();

                // play boss battle game
                GameManager.instance.playingBossBattleGame = true;
                GameType bossBattleGame = AISystem.DetermineBossBattleGame();
                GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(bossBattleGame), true, 0.5f, true);

            }
            else // if the player chooses no, break and do not go to next game scene
            {
                // show julius exclamation mark again
                julius.ShowExclamationMark(true);

                TalkieManager.instance.yesNoChoices.Clear();
                yield break;
            }
        }
        else
        {
            TalkieManager.instance.yesNoChoices.Clear();
            Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
        }

        animationDone = true;
    }


    private IEnumerator PreBossBattle1Routine()
    {
        // only continue with talkies if just played a boss battle game
        if (!GameManager.instance.playingBossBattleGame)
        {
            yield return new WaitForSeconds(1f);

            // show boss battle bar
            BossBattleBar.instance.ShowBar();

            yield return new WaitForSeconds(2f);

            // show down arrow
            PalaceArrowDown.instance.ShowArrow();
            PalaceArrowDown.instance.interactable = true;

            animationDone = true;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        // play correct player lose talkies
        if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle)
        {
            // play boss battle quips 1
            int random = Random.Range(0, 3);

            if (random == 0)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_1_p1"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_1_p2"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_1_p3"));
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        // show boss battle bar
        BossBattleBar.instance.ShowBar();

        yield return new WaitForSeconds(1f);

        // show down arrow
        PalaceArrowDown.instance.ShowArrow();
        PalaceArrowDown.instance.interactable = true;


        animationDone = true;
    }

    private IEnumerator PreBossBattle2Routine()
    {
        // only continue with talkies if just played a boss battle game
        if (!GameManager.instance.playingBossBattleGame)
        {
            yield return new WaitForSeconds(1f);

            // show boss battle bar
            BossBattleBar.instance.ShowBar();

            yield return new WaitForSeconds(2f);

            // show down arrow
            PalaceArrowDown.instance.ShowArrow();
            PalaceArrowDown.instance.interactable = true;

            animationDone = true;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        // play story beat talkie 1
        if (GameManager.instance.newBossBattleStoryBeat)
        {
            GameManager.instance.newBossBattleStoryBeat = false;
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBStory_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        // play correct player lose talkies
        else if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle)
        {
            // play boss battle quips 2
            int random = Random.Range(0, 3);

            if (random == 0)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_2_p1"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_2_p2"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_2_p3"));
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        // show boss battle bar
        BossBattleBar.instance.ShowBar();

        yield return new WaitForSeconds(1f);

        // show down arrow
        PalaceArrowDown.instance.ShowArrow();
        PalaceArrowDown.instance.interactable = true;

        animationDone = true;
    }

    private IEnumerator PreBossBattle3Routine()
    {
        // only continue with talkies if just played a boss battle game
        if (!GameManager.instance.playingBossBattleGame)
        {
            yield return new WaitForSeconds(1f);

            // show boss battle bar
            BossBattleBar.instance.ShowBar();

            yield return new WaitForSeconds(2f);

            // show down arrow
            PalaceArrowDown.instance.ShowArrow();
            PalaceArrowDown.instance.interactable = true;

            animationDone = true;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        // play story beat talkie 2
        if (GameManager.instance.newBossBattleStoryBeat)
        {
            GameManager.instance.newBossBattleStoryBeat = false;
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBStory_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        // play correct player lose talkies
        else if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle)
        {
            // play boss battle quips 1
            int random = Random.Range(0, 3);

            if (random == 0)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_3_p1"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_3_p2"));
            }
            else if (random == 1)
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBQuip_3_p3"));
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseBossBattle &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseBossBattle)
        {
            // play julius wins boss battle again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BBLose_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        // show boss battle bar
        BossBattleBar.instance.ShowBar();

        yield return new WaitForSeconds(1f);

        // show down arrow
        PalaceArrowDown.instance.ShowArrow();
        PalaceArrowDown.instance.interactable = true;

        animationDone = true;
    }
















    /* 
    ################################################
    #   CHALLENGE GAMES
    ################################################
    */

    private IEnumerator ChallengeGame1Routine(MapLocation location)
    {
        // get current chapter
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        // set julius's challenge game
        bool firstTime = SetJuliusChallengeGame(location);

        print("playing challenge game? -> " + GameManager.instance.playingChallengeGame);

        // only continue with talkies if just played a challenge game
        if (!GameManager.instance.playingChallengeGame)
        {
            animationDone = true;
            yield break;
        }

        // play correct player lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("julius wins first time");

            // play julius wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_1_p1"));
                    break;

                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_1_p2"));
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_1_p3"));
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("julius wins every other time");

            // play julius wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        animationDone = true;
    }

    private IEnumerator ChallengeGame2Routine(MapLocation location)
    {
        // get current chapter
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        // set marcus stuff
        bool firstTime = SetMarcusChallengeGame(location);

        if (firstTime)
        {
            // play julius loses
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJulius_2_p1"));
                    break;

                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJulius_2_p2"));
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJulius_2_p3"));
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            if (TalkieManager.instance.yesNoChoices.Count == 1)
            {
                // if player chooses yes - go to marcus challenge game
                if (TalkieManager.instance.yesNoChoices[0])
                {
                    TalkieManager.instance.yesNoChoices.Clear();
                    ScrollMapManager.instance.updateGameManagerBools = false; // do not update GM bools
                    marcus.GoToGameDataSceneImmediately(true);
                }
                else
                {
                    TalkieManager.instance.yesNoChoices.Clear();
                }
            }
            else
            {
                TalkieManager.instance.yesNoChoices.Clear();
                Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
            }

        }

        // only continue with talkies if just played a challenge game
        if (!GameManager.instance.playingChallengeGame)
        {
            animationDone = true;
            yield break;
        }

        // play correct lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("marcus wins first time");

            // play marcus wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaMarcusWins_1_p1"));
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaMarcusWins_1_p2"));
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("marcus wins every other time");

            // play marcus wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        animationDone = true;
    }

    private IEnumerator ChallengeGame3Routine(MapLocation location)
    {
        // get current chapter
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        // set brutus stuff
        bool firstTime = SetBrutusChallengeGame(location);

        if (firstTime)
        {
            // play marcus loses
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaMarcus_2_p1"));
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaMarcus_2_p2"));
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            if (TalkieManager.instance.yesNoChoices.Count == 1)
            {
                // if player chooses yes go to brutus challenge game
                if (TalkieManager.instance.yesNoChoices[0])
                {
                    TalkieManager.instance.yesNoChoices.Clear();
                    ScrollMapManager.instance.updateGameManagerBools = false; // do not update GM bools
                    brutus.GoToGameDataSceneImmediately(true);
                }
                else // if the player chooses no
                {
                    TalkieManager.instance.yesNoChoices.Clear();
                }
            }
            else
            {
                TalkieManager.instance.yesNoChoices.Clear();
                Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
            }
        }

        // only continue with talkies if just played a challenge game
        if (!GameManager.instance.playingChallengeGame)
        {
            animationDone = true;
            yield break;
        }

        // play correct lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("brutus wins first time");

            // play brutus wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaBrutusWins_1_p1"));
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaBrutusWins_1_p2"));
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print("brutus wins every other time");

            // play marcus wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaJuliusWins_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        animationDone = true;
    }














    // i hate that i have to do this this way but i have no choice :,)
    private bool SetJuliusChallengeGame(MapLocation location)
    {
        bool firstTime = false;

        switch (location)
        {
            case MapLocation.GorillaVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Mudslide:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.SpookyForest:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcCamp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaPoop:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.WindyCliff:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.PirateShip:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.MermaidBeach:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.ExitJungle:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaStudy:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Monkeys:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType;
                    firstTime = false;
                }
                break;
        }

        // advance story beat and save to SIS
        if (firstTime)
        {
            StudentInfoSystem.SaveStudentPlayerData();
        }

        return firstTime;
    }

    // i hate that i have to do this this way but i have no choice :,)
    private bool SetMarcusChallengeGame(MapLocation location)
    {
        bool firstTime = false;

        switch (location)
        {
            case MapLocation.GorillaVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType;
                    firstTime = false;
                }
                // set game manager stuff
                break;

            case MapLocation.Mudslide:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.SpookyForest:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcCamp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaPoop:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.WindyCliff:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.PirateShip:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.MermaidBeach:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.ExitJungle:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaStudy:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Monkeys:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType;
                    firstTime = false;
                }
                break;
        }

        // advance story beat and save to SIS
        if (firstTime)
        {
            StudentInfoSystem.SaveStudentPlayerData();
        }

        return firstTime;
    }

    // i hate that i have to do this this way but i have no choice :,)
    private bool SetBrutusChallengeGame(MapLocation location)
    {
        bool firstTime = false;

        switch (location)
        {
            case MapLocation.GorillaVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Mudslide:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.SpookyForest:
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.OrcCamp:
                if (StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaPoop:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.WindyCliff:
                if (StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.PirateShip:
                if (StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.MermaidBeach:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                if (StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.ExitJungle:
                if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.GorillaStudy:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType;
                    firstTime = false;
                }
                break;

            case MapLocation.Monkeys:
                if (StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    brutus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType = newGameType;
                    firstTime = true;
                }
                else
                {
                    brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType;
                    firstTime = false;
                }
                break;
        }

        // advance story beat and save to SIS
        if (firstTime)
        {
            StudentInfoSystem.SaveStudentPlayerData();
        }

        return firstTime;
    }
}