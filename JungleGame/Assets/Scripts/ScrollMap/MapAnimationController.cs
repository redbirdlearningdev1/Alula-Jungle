using System.Collections;
using System.Collections.Generic;
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

    /// CHAPTER 4
    // exit jungle
    ExitJungleIntro,
    ExitJungleRebuilt,
    ExitJungleDefeated,




    // all challenge games
    ChallengeGame1,
    ChallengeGame2,
    ChallengeGame3,
}

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
    }

    public void PlaceCharactersOnMap(StoryBeat storyBeat)
    {
        switch (storyBeat)
        {
            default:
                // place boat in dock
                boat.mapAnimator.Play("BoatDockedPos");
                break;

            case StoryBeat.InitBoatGame:
            case StoryBeat.UnlockGorillaVillage:
                break;

            case StoryBeat.GorillaVillageIntro:
            case StoryBeat.PrologueStoryGame:
                // place darwin in GV
                darwin.ShowExclamationMark(true);
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.interactable = true;
                break;

            case StoryBeat.RedShowsStickerButton:
                // place darwin in GV
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.interactable = true;
                break;
            
            case StoryBeat.VillageRebuilt:
                // place darwin in GV
                darwin.mapAnimator.Play("DarwinGVPos");
                darwin.interactable = true;
                break;

            case StoryBeat.GorillaVillage_challengeGame_1:
                // place julius in GV
                julius.mapAnimator.Play("JuliusGVPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                clogg.interactable = true;
                break;

            case StoryBeat.OrcVillageUnlocked:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.interactable = true;
                break;

            case StoryBeat.OrcVillage_challengeGame_1:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
                clogg.interactable = true;
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                clogg.interactable = true;
                clogg.FlipCharacterToRight();
                // place julius in OV
                julius.mapAnimator.Play("JuliusOVPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OV
                marcus.mapAnimator.Play("MarcusOVPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.interactable = true;
                // place brutus in OV
                brutus.mapAnimator.Play("BrutusOVPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;
                
            case StoryBeat.OrcVillage_challengeGame_3:
                // place clogg in OV
                clogg.mapAnimator.Play("CloggOVPos");
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
                darwin.interactable = true;
                break;

            case StoryBeat.SpookyForestPlayGames:
                // place darwin in SF
                darwin.mapAnimator.Play("DarwinSFPos");
                darwin.FlipCharacterToRight();
                darwin.interactable = true;
                break;

            case StoryBeat.SpookyForest_challengeGame_1:
                // place julius in SF
                julius.mapAnimator.Play("JuliusSFPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                clogg.interactable = true;
                break;

            case StoryBeat.OrcCampPlayGames:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.interactable = true;
                break;

            case StoryBeat.OrcCamp_challengeGame_1:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                clogg.interactable = true;
                // place julius in OC
                julius.mapAnimator.Play("JuliusOCPos");
                julius.characterAnimator.Play("sTigerIdle");
                // place marcus in OC
                marcus.mapAnimator.Play("MarcusOCPos");
                marcus.characterAnimator.Play("marcusWin");
                marcus.ShowExclamationMark(true);
                marcus.interactable = true;
                // place brutus in OC
                brutus.mapAnimator.Play("BrutusOCPos");
                brutus.characterAnimator.Play("brutusBroken");
                break;
                
            case StoryBeat.OrcCamp_challengeGame_3:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
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
                brutus.interactable = true;
                break;

            case StoryBeat.OrcCampDefeated:
                // place clogg in OC
                clogg.mapAnimator.Play("CloggOCPos");
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

            case StoryBeat.GorillaPoop_challengeGame_1:
                // place julius in GP
                julius.mapAnimator.Play("JuliusGPPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                darwin.interactable = true;
                break;

            case StoryBeat.WindyCliffPlayGames:
                // place darwin in WC
                darwin.mapAnimator.Play("DarwinWCPos");
                darwin.interactable = true;
                break;

            case StoryBeat.WindyCliff_challengeGame_1:
                // place julius in WC
                julius.mapAnimator.Play("JuliusWCPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                darwin.interactable = true;
                break;

            case StoryBeat.MermaidBeachPlayGames:
                // place darwin in MB
                darwin.mapAnimator.Play("DarwinMBPos");
                darwin.interactable = true;
                break;

            case StoryBeat.MermaidBeach_challengeGame_1:
                // place julius in MB
                julius.mapAnimator.Play("JuliusMBPos");
                julius.characterAnimator.Play("aTigerTwitch");
                julius.ShowExclamationMark(true);
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
                darwin.interactable = true;
                break;

            case StoryBeat.ExitJunglePlayGames:
                // place darwin in EJ
                darwin.mapAnimator.Play("DarwinEJPos");
                darwin.interactable = true;
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

    public void PlayMapAnim(MapAnim animation)
    {
        animationDone = false;

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
        }
    }
    
    // get the length of an animation clip
    private float GetAnimationTime(Animator anim, string animName)
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
        boat.GetComponent<MapIcon>().interactable = true;
        boat.GetComponent<WiggleController>().StartWiggle();

        animationDone = true;
    }

    private IEnumerator RevealGorillaVillage()
    {
        yield return new WaitForSeconds(1f);

        boat.mapAnimator.Play("BoatDock");
        yield return new WaitForSeconds(GetAnimationTime(boat.mapAnimator, "BoatDock"));

        // play dock 1 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.Dock_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // gorilla exclamation point
        darwin.ShowExclamationMark(true);
        darwin.mapAnimator.Play("DarwinGVPos");

        // unlock gorilla village
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaVillage, true);
        yield return new WaitForSeconds(10f);

        // play dock 2 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.Dock_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // advance story beat
        StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        yield return new WaitForSeconds(1f);

        darwin.interactable = true;

        animationDone = true;
    }

    private IEnumerator GorillaVillageIntro()
    {
        // remove exclamation mark from gorilla
        darwin.ShowExclamationMark(false);
        darwin.interactable = false;

        // play gorilla intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GorillaIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // play gorilla intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GorillaIntro_1_p2);
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
        // destroy gorilla village assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[2].mapIcons)
        {
            mapIcon.SetFixed(false, true, false);
        }

        yield return new WaitForSeconds(2f);

        // turn gorilla to face right
        darwin.FlipCharacterToRight();

        // play gorilla intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GorillaIntro_1_p3);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GorillaIntro_1_p4);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // turn gorilla to face left
        darwin.FlipCharacterToLeft();

        // play gorilla intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SectionIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // make gorilla interactable
        darwin.ShowExclamationMark(true);
        darwin.interactable = true;
        
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.GorillaVillage);
        ScrollMapManager.instance.UpdateMapIcons();

        // save map icon data to SIS
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed = false;

        // save to SIS and exit to scroll map
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();
        
        animationDone = true;
    }

    private IEnumerator RedShowsStickerButton()
    {
        // play red notices lester talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RedLester_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // unlock button in SIS
        StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
        // add glow + wiggle
        SettingsManager.instance.ToggleStickerButtonWiggleGlow(true);

        // save to sis and continue
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator DarwinForcesLesterInteraction()
    {
        // play darwin forces talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ForceLester_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // add glow + wiggle
        SettingsManager.instance.ToggleStickerButtonWiggleGlow(true);

        animationDone = true;
    }

    private IEnumerator GorillaVillageRebuilt()
    {
        // make darwin inactive
        darwin.interactable = false;

        yield return new WaitForSeconds(1f);

        // play village rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageRebuilt_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutGV"));

        // play village rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageRebuilt_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageRebuilt_1_p3);
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
        julius.interactable = true;
        julius.characterAnimator.Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator GorillaVillageDefeated()
    {
        // play village challenge 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageDefeated_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // gv sign post springs into place
        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaVillage].signPost.ShowSignPost(0, false);
        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked = true;
        StudentInfoSystem.SaveStudentPlayerData();

        yield return new WaitForSeconds(2f);

        // // place temp copy over talkie bg
        // var tempSignPost = TempObjectPlacer.instance.PlaceNewObject(mapIconsAtLocation[2].signPost.gameObject, mapIconsAtLocation[2].signPost.transform.localPosition);
        // tempSignPost.GetComponent<SignPostController>().interactable = false;
        // tempSignPost.GetComponent<SignPostController>().SetStars(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars);

        // play village challenge 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.VillageDefeated_1_p4);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        ScrollMapManager.instance.mapLocations[(int)MapLocation.GorillaVillage].signPost.GetComponent<SignPostController>().SetInteractability(true);

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
        ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.Mudslide);
        ScrollMapManager.instance.UpdateMapIcons();
        ScrollMapManager.instance.RevealStarsAtCurrentLocation();
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator MudslideRebuilt()
    {
        // play mudslide rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideRebuilt_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator MudslideDefeated()
    {
        // play mudslide defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMS"));

        // play mudslide challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MudslideDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // MS sign post springs into place
        ScrollMapManager.instance.mapLocations[3].signPost.ShowSignPost(0, false);
        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
        StudentInfoSystem.SaveStudentPlayerData();

        // before unlocking orc village - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[4].mapIcons)
            icon.SetFixed(false, false, true);

        // place clogg in village
        clogg.mapAnimator.Play("CloggOVPos");

        // unlock orc village
        ScrollMapManager.instance.UnlockMapArea(MapLocation.OrcVillage, false);
        yield return new WaitForSeconds(10f);

        // play orc village intro talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // play orc village intro talkie
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageIntro_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);
        ScrollMapManager.instance.mapLocations[3].signPost.GetComponent<SignPostController>().SetInteractability(true);

        // make clogg interactable
        clogg.interactable = true;
        clogg.ShowExclamationMark(true);

        // save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator OrcVillageRebuilt()
    {
        // play orc village rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageRebuilt_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator OrcVillageDefeated()
    {
        // play orc village defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OVillageDefeated_1_p1);
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

        // OV sign post springs into place
        ScrollMapManager.instance.mapLocations[4].signPost.ShowSignPost(0, false);

        // before unlocking spooky forest - set objects to be repaired
        foreach (var icon in ScrollMapManager.instance.mapLocations[5].mapIcons)
            icon.SetFixed(true, false, true);

        // place darwin in spooky forest
        darwin.mapAnimator.Play("DarwinSFPos");
        darwin.FlipCharacterToRight();
        darwin.ShowExclamationMark(true);
        darwin.interactable = false;

        // unlock spooky forest
        ScrollMapManager.instance.UnlockMapArea(MapLocation.SpookyForest, false);
        yield return new WaitForSeconds(10f);

        // darwin is interactable
        darwin.interactable = true;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2; // new chapter!
        StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator SpookyForestIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.interactable = false;

        // spider intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy village
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        // destroy gorilla village assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[5].mapIcons)
        {
            mapIcon.SetFixed(false, true, true);
        }

        yield return new WaitForSeconds(2f);

        // spider intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutSF"));

        // spider intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p4);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p5);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // spider intro 6
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderIntro_1_p6);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);

        animationDone = true;
    }

    private IEnumerator SpookyForestRebuilt()
    {
        // play spooky forest rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderRebuilt_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutSF"));

        // play spooky forest rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderRebuilt_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderRebuilt_1_p3);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator SpookyForestDefeated()
    {
        // play spooky forest defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutSF");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutSF"));

        // play spooky forest challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.SpiderDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[5].signPost.ShowSignPost(0, false);

        // before unlocking orc camp - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[6].mapIcons)
            icon.SetFixed(false, false, true);

        // place clogg in orc camp
        clogg.mapAnimator.Play("CloggOCPos");
        clogg.ShowExclamationMark(true);
        clogg.interactable = false;

        // unlock orc camp
        ScrollMapManager.instance.UnlockMapArea(MapLocation.OrcCamp, false);
        yield return new WaitForSeconds(10f);

        // clogg is interactable
        clogg.interactable = true;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator OrcCampRebuilt()
    {
        // play spooky forest rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampRebuilt_1_p1);
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

        // play spooky forest rebuilt talkie 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator OrcCampDefeated()
    {
        // play OC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutOC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutOC"));

        // play OC challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.OCampDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[6].signPost.ShowSignPost(0, false);

        // before unlocking gorilla poop - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[7].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock gorilla poop
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaPoop, false);
        yield return new WaitForSeconds(10f);

        // play gorilla poop intro
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 7;
        StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator GorillaPoopRebuilt()
    {
        // play GP rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopRebuilt_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator GorillaPoopDefeated()
    {
        // play GP defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutGP");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutGP"));

        // play OC challenge 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PoopDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[7].signPost.ShowSignPost(0, false);

        // place darwin in WC
        darwin.mapAnimator.Play("DarwinWCPos");
        darwin.ShowExclamationMark(true);
        darwin.interactable = false;

        // unlock windy cliff
        ScrollMapManager.instance.UnlockMapArea(MapLocation.WindyCliff, false);
        yield return new WaitForSeconds(10f);

        // make darwin interactable
        darwin.interactable = true;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 8;
        StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_3; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator WindyCliffIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.interactable = false;

        // windy cliff intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffIntro_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffIntro_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy cliff
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        // destroy cliff assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[8].mapIcons)
        {
            mapIcon.SetFixed(false, true, true);
        }

        yield return new WaitForSeconds(2f);

        // windy cliff intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffIntro_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

        // windy cliff intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffIntro_1_p4);
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
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "MarcusWalkOutWC"));

        // spider intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffIntro_1_p5);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToRight();

        animationDone = true;
    }

    private IEnumerator WindyCliffRebuilt()
    {
        // play WC rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffRebuilt_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutWC"));

        // play WC rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffRebuilt_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffRebuilt_1_p3);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator WindyCliffDefeated()
    {
        // play WC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutWC");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutWC"));

        // play WC defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // SF sign post springs into place
        ScrollMapManager.instance.mapLocations[8].signPost.ShowSignPost(0, false);

        // before unlocking pirate ship - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[9].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock pirate ship
        ScrollMapManager.instance.UnlockMapArea(MapLocation.PirateShip, false);
        yield return new WaitForSeconds(10f);

        // play PS intro 
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PirateIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 9;
        StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator PirateShipRebuilt()
    {
        // play PS rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PirateRebuilt_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PirateRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator PirateShipDefeated()
    {
        // play WC defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PirateDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutPS");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutPS"));

        // play WC defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.CliffDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.PirateDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // PS sign post springs into place
        ScrollMapManager.instance.mapLocations[9].signPost.ShowSignPost(0, false);

        // place darwin in MB
        darwin.mapAnimator.Play("DarwinMBPos");
        darwin.ShowExclamationMark(true);
        darwin.interactable = false;

        // unlock mermaid beach
        ScrollMapManager.instance.UnlockMapArea(MapLocation.MermaidBeach, false);
        yield return new WaitForSeconds(10f);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 10;
        StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_4; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator MermaidBeachIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.interactable = false;

        // mermaid beach intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidIntro_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidIntro_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy mermaid beach
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        // destroy mermaid beach assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[10].mapIcons)
        {
            mapIcon.SetFixed(false, true, true);
        }

        yield return new WaitForSeconds(2f);

        // mermaid beach intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidIntro_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMB"));

        // mermaid beach intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidIntro_1_p4);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidIntro_1_p5);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToLeft();

        animationDone = true;
    }

    private IEnumerator MermaidBeachRebuilt()
    {
        // play MB rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidRebuilt_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutMB"));

        // play MB rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidRebuilt_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidRebuilt_1_p3);
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
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator MermaidBeachDefeated()
    {
        // play MB defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutMB");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutMB"));

        // play MB defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.MermaidDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // MB sign post springs into place
        ScrollMapManager.instance.mapLocations[10].signPost.ShowSignPost(0, false);

        // before unlocking ruins 1 - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[11].mapIcons)
            icon.SetFixed(false, false, true);
        // before unlocking ruins 2 - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[12].mapIcons)
            icon.SetFixed(false, false, true);

        // unlock ruins
        ScrollMapManager.instance.UnlockMapArea(MapLocation.Ruins2, false);
        yield return new WaitForSeconds(10f);

        // play R intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsIntro_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 12;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator RuinsRebuilt()
    {
        // make sure scroll map is on Ruins 1!!!
        ScrollMapManager.instance.GoToMapPosition(MapLocation.Ruins1);
        yield return new WaitForSeconds(1f);

        // play R rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsRebuilt_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsRebuilt_1_p2);
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
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator RuinsDefeated()
    {
        // play R defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutR");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutR"));

        // play R defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.RuinsDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // R sign post springs into place
        ScrollMapManager.instance.mapLocations[12].signPost.ShowSignPost(0, false);

        // place darwin in EJ
        darwin.mapAnimator.Play("DarwinEJPos");
        darwin.ShowExclamationMark(true);
        darwin.interactable = false;

        // unlock exit jungle
        ScrollMapManager.instance.UnlockMapArea(MapLocation.ExitJungle, false);
        yield return new WaitForSeconds(10f);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 12;
        StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked = true;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_5; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    private IEnumerator ExitJungleIntro()
    {
        darwin.ShowExclamationMark(false);
        darwin.interactable = false;

        // exit jungle intro 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitIntro_1_p1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitIntro_1_p2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // destroy exit jungle
        tigerSwipeAnim.Play("tigerScreenSwipe");
        julius.characterAnimator.Play("tigerSwipe");
        ScrollMapManager.instance.ShakeMap();
        // destroy mermaid beach assets
        foreach (var mapIcon in ScrollMapManager.instance.mapLocations[13].mapIcons)
        {
            mapIcon.SetFixed(false, true, true);
        }

        yield return new WaitForSeconds(2f);

        // exit jungle intro 3
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitIntro_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutEJ"));

        // exit jungle intro 4
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitIntro_1_p4);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitIntro_1_p5);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // save to SIS and exit to scroll map
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        // make darwin interactable
        darwin.interactable = true;
        darwin.ShowExclamationMark(true);
        darwin.FlipCharacterToLeft();

        animationDone = true;
    }

    private IEnumerator ExitJungleRebuilt()
    {
        // play EJ rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitRebuilt_1_p1);
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

        // play EJ rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitRebuilt_1_p2);
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
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = true;
    }

    private IEnumerator ExitJungleDefeated()
    {
        // play EJ defeated 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitDefeated_1_p1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // tiger runs off screen
        julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        julius.mapAnimator.Play("JuliusWalkOutEJ");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(julius.mapAnimator, "JuliusWalkOutEJ"));

        // play EJ defeated 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitDefeated_1_p2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ExitDefeated_1_p3);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // EJ sign post springs into place
        ScrollMapManager.instance.mapLocations[13].signPost.ShowSignPost(0, false);

        // before unlocking gorilla study - set objects to be destroyed
        foreach (var icon in ScrollMapManager.instance.mapLocations[14].mapIcons)
            icon.SetFixed(false, false, true);

        // place darwin in gorilla study
        darwin.mapAnimator.Play("DarwinGSPos");
        darwin.ShowExclamationMark(true);
        darwin.interactable = false;

        // unlock gorilla study
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaStudy, false);
        yield return new WaitForSeconds(10f);

        // Save to SIS
        StudentInfoSystem.GetCurrentProfile().mapLimit = 14;
        StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_unlocked = true;
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

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

        // play correct player lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("julius wins first time");

            // play julius wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_1_p1);
                    break;

                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_1_p2);
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_1_p3);
                    break;
            }
            
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("julius wins every other time");

            // play julius wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_2_p1);
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
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_2_p1);
                    break;

                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_2_p2);
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJulius_2_p3);
                    break;
            }
            
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // do not go to game if talkie manager says not to
            if (TalkieManager.instance.doNotContinueToGame)
            {
                TalkieManager.instance.doNotContinueToGame = false;
            }
            else
            {
                // set game manager stuff
                GameManager.instance.mapID = MapIconIdentfier.GV_challenge_2;
                GameManager.instance.playingChallengeGame = true;

                // continue to marcus challenge game
                marcus.GoToGameDataSceneImmediately();
            }
        }

        // play correct lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("marcus wins first time");

            // play marcus wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaMarcusWins_1_p1);
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaMarcusWins_1_p2);
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("marcus wins every other time");

            // play marcus wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_2_p1);
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
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaMarcus_2_p1);
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaMarcus_2_p2);
                    break;
            }
            
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // do not go to game if talkie manager says not to
            if (TalkieManager.instance.doNotContinueToGame)
            {
                TalkieManager.instance.doNotContinueToGame = false;
            }
            else
            {
                // set game manager stuff
                GameManager.instance.mapID = MapIconIdentfier.GV_challenge_3;
                GameManager.instance.playingChallengeGame = true;

                // continue to marcus challenge game
                MapAnimationController.instance.brutus.GoToGameDataSceneImmediately();
            }
        }

        // play correct lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("brutus wins first time");

            // play brutus wins
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                case Chapter.chapter_4:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaBrutusWins_1_p1);
                    break;

                case Chapter.chapter_5:
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaBrutusWins_1_p2);
                    break;
            }

            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            print ("brutus wins every other time");

            // play marcus wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.ChaJuliusWins_2_p1);
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
