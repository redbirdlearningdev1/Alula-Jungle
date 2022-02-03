using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapAnim
{
    BoatIntro,
    RevealGorillaVillage,
    GorillaVillageIntro,
    GorillaVillageRebuilt,
    GorillaVillageDefeated
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

            case MapAnim.GorillaVillageRebuilt:
                StartCoroutine(GorillaVillageRebuilt());
                break;

            case MapAnim.GorillaVillageDefeated:
                StartCoroutine(GorillaVillageDefeated());
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // gorilla exclamation point
        darwin.ShowExclamationMark(true);
        darwin.mapAnimator.Play("DarwinGVPos");

        // unlock gorilla village
        ScrollMapManager.instance.UnlockMapArea(MapLocation.GorillaVillage, true);
        yield return new WaitForSeconds(10f);

        // play dock 2 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        // play gorilla intro 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_3);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_4);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        yield return new WaitForSeconds(1f);

        // turn gorilla to face left
        darwin.FlipCharacterToLeft();

        // play gorilla intro 5
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_5);
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
        yield break;
    }

    private IEnumerator GorillaVillageRebuilt()
    {
        // make darwin inactive
        darwin.interactable = false;

        yield return new WaitForSeconds(1f);

        // play village rebuilt talkie 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // move darwin off screen
        darwin.FlipCharacterToRight();
        darwin.characterAnimator.Play("gorillaWalk");
        darwin.mapAnimator.Play("DarwinWalkOutGV");

        // wait for animation to be done
        yield return new WaitForSeconds(GetAnimationTime(darwin.mapAnimator, "DarwinWalkOutGV"));

        // play village rebuilt talkie 2
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_3);
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
    }

    private IEnumerator GorillaVillageDefeated()
    {
        // play village challenge 1
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_1);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_2);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_3);
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
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideIntro);
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
    }
}
