using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapAnim
{
    BoatIntro,
    RevealGorillaVillage,
    GorillaVillageIntro,
    RedShowsStickerButton,
    DarwinForcesLesterInteraction,
    GorillaVillageRebuilt,
    GorillaVillageDefeated,
    MudslideRebuilt,


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
                marcus.characterAnimator.Play("marcusLose");
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
                marcus.characterAnimator.Play("marcusLose");
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
                marcus.characterAnimator.Play("marcusLose");
                // place brutus in GV
                brutus.mapAnimator.Play("BrutusGVPos");
                brutus.characterAnimator.Play("brutusLose");
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

        animationDone = false;
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

        animationDone = false;
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
        
        animationDone = false;
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

        // set tiger stuff
        if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == GameType.None)
        {
            GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
            julius.gameType = newGameType;
            StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = newGameType;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }

        julius.ShowExclamationMark(true);
        julius.interactable = true;
        julius.characterAnimator.Play("aTigerTwitch");

        animationDone = false;
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

        animationDone = false;
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

        animationDone = false;
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

        animationDone = false;
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

        // set tiger stuff
        if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
        {
            GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
            julius.gameType = newGameType;
            StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
            
        julius.ShowExclamationMark(true);
        julius.interactable = true;
        julius.GetComponent<Animator>().Play("aTigerTwitch");

        animationDone = false;
    }


    /* 
    ################################################
    #   CHALLENGE GAMES
    ################################################
    */

    private IEnumerator ChallengeGame1Routine(MapLocation location)
    {
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        // set julius's challenge game
        SetJuliusChallengeGame(location);

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

        animationDone = false;
    }

    private IEnumerator ChallengeGame2Routine(MapLocation location)
    {
        // set marcus stuff
        bool firstTime = SetMarcusChallengeGame(location);

        if (firstTime)
        {

        }
        else
        {

        }

        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

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

        animationDone = false;
    }

    private IEnumerator ChallengeGame3Routine(MapLocation location)
    {
        // set tiger stuff
        if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType == GameType.None)
        {
            GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
            MapAnimationController.instance.brutus.gameType = newGameType;
            StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = newGameType;
            StudentInfoSystem.SaveStudentPlayerData();

            // play marcus loses + brutus challenges
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_loses__brutus_challenges);
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
        else
        {
            MapAnimationController.instance.brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType;
        }

        // play correct lose talkies
        if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            // play brutus wins
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.brutus_wins);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }
        else if (
            StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
            StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        {
            // play brutus wins again
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.brutus_wins_again);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }

        animationDone = false;
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
                    StudentInfoSystem.SaveStudentPlayerData();
                    return true;
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType;
                    return false;
                }

            case MapLocation.Mudslide:
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    julius.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                    StudentInfoSystem.SaveStudentPlayerData();
                    return
                }
                else
                {
                    julius.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType;
                }
                break;
        }
    }

    // i hate that i have to do this this way but i have no choice :,)
    private void SetMarcusChallengeGame(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.GorillaVillage:
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType == GameType.None)
                {
                    print ("you beat julius!");
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    marcus.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = newGameType;
                    StudentInfoSystem.SaveStudentPlayerData();

                    // play julius loses + marcus challenges
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_loses__marcus_challenges);
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
                else
                {
                    marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType;
                }
                break;
        }
    }
}
