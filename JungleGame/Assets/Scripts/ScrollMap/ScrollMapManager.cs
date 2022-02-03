﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapLocation
{
    Ocean,
    BoatHouse,
    GorillaVillage,
    Mudslide,
    OrcVillage,
    SpookyForest,
    OrcCamp,
    GorillaPoop,
    WindyCliff,
    PirateShip,
    MermaidBeach,
    Ruins1,
    Ruins2,
    ExitJungle,
    GorillaStudy,
    Monkeys,
    PalaceIntro,
    COUNT
}

[System.Serializable]
public class MapLocationData
{
    public MapLocation location;
    public List<MapIcon> mapIcons;
    public Transform cameraLocation;
    public float fogLocation;
    public SignPostController signPost;
    public bool enabled;
}

public class ScrollMapManager : MonoBehaviour
{
    public static ScrollMapManager instance;

    private List<GameObject> mapIcons = new List<GameObject>();
    private bool repairingMapIcon = false;

    private bool activateMapNavigation = false;
    private bool revealGMUI = false;
    private bool waitingForGameEventRoutine = false;

    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Map Icons @ Locations")]
    public List<MapLocationData> mapLocations;

    [Header("Animations")]
    public float staticMapYPos;

    public AnimationCurve curve;
    public float multiplier;

    private int mapLimit;
    private int currMapLocation;
    private int minMapLimit = 1;
    private bool navButtonsDisabled = true;
    
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;


    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // play test song
        AudioManager.instance.PlaySong(AudioDatabase.instance.MainThemeSong);
    }

    void Start()
    {
        StartCoroutine(DelayedStart(0f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        // disable UI
        leftButton.interactable = false;
        rightButton.interactable = false;
        navButtonsDisabled = true;
        activateMapNavigation = true;
        revealGMUI = true;

        // load in map data from profile + disable all map icons
        MapDataLoader.instance.LoadMapData(StudentInfoSystem.GetCurrentProfile().mapData);
        DisableAllMapIcons(true);

        // set map location
        int index = StudentInfoSystem.GetCurrentProfile().mapLimit;
        SetMapPosition(index);
        SetMapLimit(index);

        // remove game manager stuff
        GameManager.instance.playingRoyalRumbleGame = false;
        GameManager.instance.playingChallengeGame = false;

        // get current game event
        StoryBeat playGameEvent = StudentInfoSystem.GetCurrentProfile().currStoryBeat;
        GameManager.instance.SendLog(this, "Current Story Beat: " + playGameEvent);

        /* 
        ################################################
        #   GAME EVENTS (place characters on scroll map)
        ################################################
        */

        MapAnimationController.instance.PlaceCharactersOnMap(playGameEvent);
        
        /* 
        ################################################
        #   GAME EVENTS (repair map icon)
        ################################################
        */

        if (GameManager.instance.repairMapIconID)
        {
            yield return new WaitForSeconds(1f);

            StartCoroutine(RepairMapIcon(GameManager.instance.mapID));

            while (repairingMapIcon)
                yield return null;
        }

        /* 
        ################################################
        #   GAME EVENTS (STORY BEATS)
        ################################################
        */

        // check for game events
        CheckForScrollMapGameEvent(playGameEvent);
        // wait here while game event stuff is happening
        while (waitingForGameEventRoutine)
            yield return null;

        /* 
        ################################################
        #   GAME EVENTS (AFTER STORY BEAT)
        ################################################
        */
        
        // show UI
        if (activateMapNavigation)
            ToggleNavButtons(true);
        
        // update map icons
        UpdateMapIcons();

        // show GM UI
        if (revealGMUI)
        {
            SettingsManager.instance.ToggleMenuButtonActive(true);
            // show sticker button if unlocked
            if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
                SettingsManager.instance.ToggleWagonButtonActive(true);
        }

        // show stars on current map location
        yield return new WaitForSeconds(1f);
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));

        // set RR banner on map icon
        MapDataLoader.instance.SetRoyalRumbleBanner();
    }

    public void CheckForScrollMapGameEvent(StoryBeat playGameEvent)
    {
        StartCoroutine(GameEventRoutine(playGameEvent));
    }

    private IEnumerator GameEventRoutine(StoryBeat playGameEvent)
    {
        // game event in progress
        waitingForGameEventRoutine = true;

        // default bool values
        activateMapNavigation = true;
        revealGMUI = true;

        // enable map sections up to current section
        EnableMapSectionsUpTo(mapLocations[currMapLocation].location);

        if (playGameEvent == StoryBeat.InitBoatGame)
        {   
            // change scroll map bools
            activateMapNavigation = false;
            revealGMUI = false;
            minMapLimit = 0;

            // play boat ocean intro animation
            MapAnimationController.instance.PlayMapAnim(MapAnim.BoatIntro);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.UnlockGorillaVillage)
        {
            // change scroll map bools
            activateMapNavigation = false;
            revealGMUI = true;

            // start on boat dock
            SetMapPosition((int)MapLocation.BoatHouse);
            SetMapLimit((int)MapLocation.BoatHouse);

            // bring boat into dock
            MapAnimationController.instance.PlayMapAnim(MapAnim.RevealGorillaVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaVillageIntro)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.BoatHouse);
        }
        else if (playGameEvent == StoryBeat.RedShowsStickerButton)
        {
            // check if player has enough coins
            if (StudentInfoSystem.GetCurrentProfile().goldCoins >= 4)
            {
                // play red notices lester talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.red_notices_lester);
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
            }
        }
        
        else if (playGameEvent == StoryBeat.VillageRebuilt)
        {
            // make sure player has done the sticker tutorial
            if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
            {
                // play darwin forces talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.darwin_forces);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                yield return new WaitForSeconds(1f);

                // add glow + wiggle
                SettingsManager.instance.ToggleStickerButtonWiggleGlow(true);
            }
            else
            {
                // make sure player has rebuilt all the GV map icons
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed)
                {
                    // bring boat into dock
                    MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaVillageRebuilt);
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;
                }
            }
        }
        /*
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_1)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                print ("julius wins first time");

                // play julius wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                print ("julius wins every other time");

                // play julius wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            tiger.interactable = true;
            tiger.ShowExclamationMark(true);
            tiger.GetComponent<Animator>().Play("aTigerTwitch");
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_2)
        {
            // set marcus stuff
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

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                print ("marcus wins first time");

                // play marcus wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                print ("marcus wins every other time");

                // play marcus wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_3)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                brutus.gameType = newGameType;
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
                    brutus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType;
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

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.VillageChallengeDefeated)
        {
            // play village challenge 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger runs off screen
            MapAnimationController.instance.TigerRunAwayDefeatedGV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            // monkies go hehe and haha then run off too
            MapAnimationController.instance.MonkeyExitAnimationDefeatedGV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            // place tiger and monkies off screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.offscreenPos.position;

            // play village challenge 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            // gv sign post springs into place
            mapLocations[2].signPost.ShowSignPost(0, false);
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
            foreach (var icon in mapLocations[3].mapIcons)
                icon.SetFixed(false, false, true);

            // unlock mudslide
            StartCoroutine(UnlockMapArea(3, false));
            yield return new WaitForSeconds(10f);

            // play mudslide intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideIntro);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            mapLocations[2].signPost.GetComponent<SignPostController>().SetInteractability(true);

            // save to SIS
            StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
            ScrollMapManager.instance.EnableMapSectionsUpTo(MapLocation.Mudslide);
            ScrollMapManager.instance.UpdateMapIcons();
            ScrollMapManager.instance.RevealStarsAtCurrentLocation();
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.MudslideUnlocked)
        {
            // make sure player has rebuilt all the MS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.isFixed)
            {
                // play mudslide rebuilt talkie 1
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideRebuilt_1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // move tiger and monkies onscreen
                MapAnimationController.instance.TigerAndMonkiesWalkInMS();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // play mudslide rebuilt talkie 2
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideRebuilt_2);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // move tiger and monkies to challenge pos
                MapAnimationController.instance.TigerAndMonkiesChallengePosMS();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // make challenge games active
                yield return new WaitForSeconds(0.5f);

                // set tiger stuff
                if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    tiger.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                    StudentInfoSystem.AdvanceStoryBeat();
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                    
                tiger.ShowExclamationMark(true);
                tiger.interactable = true;
                tiger.GetComponent<Animator>().Play("aTigerTwitch"); 
            }
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_1)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            tiger.interactable = true;
            tiger.ShowExclamationMark(true);
            tiger.GetComponent<Animator>().Play("aTigerTwitch");
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_2)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.MS_challenge_2;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_3)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.MS_challenge_3;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType;
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

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.MudslideDefeated)
        {
            // map pos
            EnableMapSectionsUpTo(MapLocation.Mudslide);
    
            // play mudslide defeated 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideChallengeDefeated_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger runs off screen
            MapAnimationController.instance.TigerRunAwayDefeatedMS();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(2f);

            // play mudslide challenge 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideChallengeDefeated_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // monkies go hehe and haha then run off too
            MapAnimationController.instance.MonkeyExitAnimationDefeatedMS();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // place tiger and monkies off screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.offscreenPos.position;

            // play mudslide challenge 3
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideChallengeDefeated_3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // gv sign post springs into place
            mapLocations[2].signPost.ShowSignPost(0, false);
            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
            StudentInfoSystem.SaveStudentPlayerData();

            // before unlocking orc village - set objects to be destroyed
            foreach (var icon in mapLocations[4].mapIcons)
                icon.SetFixed(false, false, true);

            // place clogg in village
            clogg.transform.position = MapAnimationController.instance.cloggOVPosDEFAULT.position;

            // unlock orc village
            StartCoroutine(UnlockMapArea(4, false));
            yield return new WaitForSeconds(10f);

            // play orc village intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageIntro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // play orc village intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageIntro_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);
            mapLocations[3].signPost.GetComponent<SignPostController>().SetInteractability(true);

            // make clogg interactable
            clogg.interactable = true;
            clogg.ShowExclamationMark(true);

            // save to SIS
            StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.OrcVillageMeetClogg) // default
        {
            // make clogg interactable
            clogg.interactable = true;
            clogg.ShowExclamationMark(true);
        }
        else if (playGameEvent == StoryBeat.OrcVillageUnlocked)
        {
            // make sure player has rebuilt all the OV map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.isFixed)
            {
                // play orc village rebuilt talkie 1
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageRebuilt_1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // move tiger and monkies onscreen
                MapAnimationController.instance.TigerAndMonkiesWalkInOV();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // play orc village rebuilt talkie 2
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageRebuilt_2);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // move tiger and monkies to challenge pos
                MapAnimationController.instance.TigerAndMonkiesChallengePosOV();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // make challenge games active
                    yield return new WaitForSeconds(0.5f);

                // set tiger stuff
                if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    tiger.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = newGameType;
                    StudentInfoSystem.AdvanceStoryBeat();
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                    
                tiger.ShowExclamationMark(true);
                tiger.interactable = true;
                tiger.GetComponent<Animator>().Play("aTigerTwitch");
            }
            else
            {
                // place clogg in village
                clogg.transform.position = MapAnimationController.instance.cloggOVPosDEFAULT.position;

                // make clogg interactable
                clogg.interactable = true;
            }
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_1)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            yield return new WaitForSeconds(1f);

            tiger.interactable = true;
            tiger.ShowExclamationMark(true);
            tiger.GetComponent<Animator>().Play("aTigerTwitch");
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_2)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.OV_challenge_2;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_3)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.OV_challenge_3;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType;
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

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.OrcVillageDefeated)
        {
            // play orc village defeated 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageChallengeDefeated_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger runs off screen
            MapAnimationController.instance.TigerRunAwayDefeatedOV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(2f);

            // monkies go hehe and haha then run off too
            MapAnimationController.instance.MonkeyExitAnimationDefeatedOV();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // play orc village challenge 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageChallengeDefeated_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // place tiger and monkies off screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.offscreenPos.position;

            // ov sign post springs into place
            mapLocations[4].signPost.ShowSignPost(0, false);

            // before unlocking spooky forest - set objects to be repaired
            foreach (var icon in mapLocations[5].mapIcons)
                icon.SetFixed(true, false, true);

            // place darwin in spooky forest
            gorilla.transform.position = MapAnimationController.instance.gorillaSFPosDEFAULT.position;
            gorilla.FlipCharacterToRight();
            gorilla.ShowExclamationMark(true);
            gorilla.interactable = false;

            // unlock spooky forest
            StartCoroutine(UnlockMapArea(5, false));
            yield return new WaitForSeconds(10f);

            // darwin is interactable
            gorilla.interactable = true;

            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
            StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2; // new chapter!
            StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = true;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.SpookyForestUnlocked)
        {
            // darwin interactable
            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.BeginningStoryGame)
        {
            // darwin interactable
            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.SpookyForestPlayGames)
        {
            // make sure player has rebuilt all the OV map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed)
            {
                // play spooky forest rebuilt talkie 1
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForest_1);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // darwin moves off-screen
                MapAnimationController.instance.GorillaExitAnimationSF();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // play spooky forest rebuilt talkie 2
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForest_2);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // tiger and monkies walk in
                MapAnimationController.instance.TigerAndMonkiesWalkInSF();
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;

                // play spooky forest rebuilt talkie 3
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForest_3);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // make challenge games active
                yield return new WaitForSeconds(0.5f);

                // set tiger stuff
                if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType == GameType.None)
                {
                    GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    tiger.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = newGameType;
                    StudentInfoSystem.AdvanceStoryBeat();
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                    
                tiger.ShowExclamationMark(true);
                tiger.interactable = true;
                tiger.GetComponent<Animator>().Play("aTigerTwitch");
            }
            else
            {
                // darwin interactable
                gorilla.ShowExclamationMark(false);
                gorilla.interactable = true;
            }
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_1)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play julius wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.julius_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            yield return new WaitForSeconds(1f);

            tiger.interactable = true;
            tiger.ShowExclamationMark(true);
            tiger.GetComponent<Animator>().Play("aTigerTwitch");
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_2)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.SF_challenge_2;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType;
            }

            // play correct lose talkies
            if (StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                !StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else if (
                StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame &&
                StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
            {
                // play marcus wins again
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.marcus_wins_again);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_3)
        {
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType == GameType.None)
            {
                GameType newGameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = newGameType;
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
                    GameManager.instance.mapID = MapIconIdentfier.SF_challenge_3;
                    GameManager.instance.playingChallengeGame = true;

                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType;
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

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.SpookyForestDefeated)
        {
            // play spooky forest defeated 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForestChallengeDefeated_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger runs off screen
            MapAnimationController.instance.TigerRunAwayDefeatedSF();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // play spooky forest challenge 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForestChallengeDefeated_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // monkies go hehe and haha then run off too
            MapAnimationController.instance.MonkeyExitAnimationDefeatedSF();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // play spooky forest challenge 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.spookyForestChallengeDefeated_3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // place tiger and monkies off screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.offscreenPos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.offscreenPos.position;

            // SF sign post springs into place
            mapLocations[5].signPost.ShowSignPost(0, false);

            // before unlocking orc camp - set objects to be destroyed
            foreach (var icon in mapLocations[6].mapIcons)
                icon.SetFixed(false, false, true);

            // place clogg in orc camp
            clogg.transform.position = MapAnimationController.instance.cloggOCPosDEFAULT1.position;
            clogg.ShowExclamationMark(true);
            clogg.interactable = false;

            // unlock orc camp
            StartCoroutine(UnlockMapArea(6, false));
            yield return new WaitForSeconds(10f);

            // clogg is interactable
            clogg.interactable = true;

            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
            StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked = true;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.OrcCampUnlocked)
        {
            // clogg is interactable
            clogg.ShowExclamationMark(true);
            clogg.interactable = true;
        }
        else if (playGameEvent == StoryBeat.OrcCampPlayGames)
        {
            // clogg is interactable
            clogg.interactable = true;
        }
        else
        {
            // unlock everything
            EnableMapSectionsUpTo(MapLocation.PalaceIntro);
        }
        */

        // game event is over
        waitingForGameEventRoutine = false;
    }

    private IEnumerator RepairMapIcon(MapIconIdentfier id)
    {
        repairingMapIcon = true;

        MapIcon icon = MapDataLoader.instance.GetMapIconFromID(id);
        icon.SetFixed(true, true, true);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HealFixItem, 0.5f);
        yield return new WaitForSeconds(2f);

        GameManager.instance.repairMapIconID = false;
        repairingMapIcon = false;
    }

    public void ShakeMap()
    {
        StartCoroutine(ShakeMapRoutine());
    }
    private IEnumerator ShakeMapRoutine()
    {
        //print ("curve.length: " + curve.length);
        float timer = 0f;
        Vector3 originalPos = Map.position;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 2f)
            {
                Map.position = originalPos;
                break;
            }

            float tempX = originalPos.x + curve.Evaluate(timer) * multiplier;
            Map.position = new Vector3(tempX, originalPos.y, originalPos.z);
            yield return null;
        }
    }

    public void RevealStarsAtCurrentLocation()
    {
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));
    }

    private IEnumerator ToggleLocationRoutine(bool opt, int location)
    {
        // return if section is disabled
        if (!mapLocations[location].enabled)
            yield break;

        // show / hide stars of current location
        foreach (var mapicon in mapLocations[location].mapIcons)
        {
            if (opt)
                mapicon.RevealStars();
            else
                mapicon.HideStars();

            yield return null;
        }

        if (opt)
            EnableSignPostAtLocation(location);
        else
        {
            if (mapLocations[location].signPost != null)
                mapLocations[location].signPost.HideSignPost();
        } 
    }

    private void EnableSignPostAtLocation(int location)
    {
        // show / hide sign post
        if (mapLocations[location].signPost != null)
        {
            // check to see if signpost is already enabled
            if (mapLocations[location].signPost.isEnabled)
            {
                return;
            }

            // check SIS if signpost unlocked
            switch (mapLocations[location].location)
            {
                case MapLocation.Ocean:
                case MapLocation.BoatHouse:
                    return;
                case MapLocation.GorillaVillage:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars, GetMapLocationIcons(MapLocation.GorillaVillage).enabled);
                        return;
                    }
                    break;
                        
                case MapLocation.Mudslide:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars, GetMapLocationIcons(MapLocation.Mudslide).enabled);
                        return;
                    }
                    break;
                        
                case MapLocation.OrcVillage:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_stars, GetMapLocationIcons(MapLocation.OrcVillage).enabled);
                        return;
                    }
                    break;
                        
                case MapLocation.SpookyForest:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_stars, GetMapLocationIcons(MapLocation.SpookyForest).enabled);
                        return;
                    }
                    break;
                        
                case MapLocation.OrcCamp:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_stars, GetMapLocationIcons(MapLocation.OrcCamp).enabled);
                        return;
                    }
                    break;
                // etc ...
            }

            // hide signpost if not unlocked
            mapLocations[location].signPost.HideSignPost();
        }
    }

    public void DisableAllMapIcons(bool hideStars)
    {
        // disable map icons
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = false;

            if (hideStars)
                item.HideStars();
        }

        // disable signPosts
        var signPosts = GetSignPostControllers();
        foreach(var item in signPosts)
        {
            item.SetInteractability(false);
        }
    }

    public void SoftLockMapIcons(float duration)
    {
        StartCoroutine(SoftLockMapIconsRoutine(duration));
    }

    private IEnumerator SoftLockMapIconsRoutine(float duration)
    {
        DisableAllMapIcons(false);
        yield return new WaitForSeconds(duration);
        EnableMapIcons(mapLocations[currMapLocation], false);
    }

    public void UpdateMapIcons()
    {
        // reload data
        MapDataLoader.instance.LoadMapData(StudentInfoSystem.GetCurrentProfile().mapData);
        foreach (var item in mapLocations)
        {
            EnableMapIcons(item, false);
        }
    }

    public void EnableMapIcons(MapLocationData obj, bool revealStars)
    {
        // return if map locatioon has no icons
        if (obj.mapIcons.Count == 0)
            return;

        // enable icons and signpost if section is enabled
        if (obj.enabled)
        {
            foreach (var icon in obj.mapIcons)
            {
                icon.interactable = true;

                if (revealStars)
                    icon.RevealStars();
            }
        }
    }

    public void UnlockMapArea(MapLocation location, bool leaveLetterboxUp = false)
    {
        StartCoroutine(UnlockMapAreaRoutine(location, leaveLetterboxUp));
    }

    private IEnumerator UnlockMapAreaRoutine(MapLocation location, bool leaveLetterboxUp = false)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("UnlockMapArea");

        yield return new WaitForSeconds(1f);

        // how Letterbox view
        LetterboxController.instance.ToggleLetterbox(true);

        yield return new WaitForSeconds(1f);

        currMapLocation = (int)location;
        mapLimit = currMapLocation;

        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(currMapLocation);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, 2f));

        yield return new WaitForSeconds(2.5f);

        // move fog out of the way
        FogController.instance.MoveFogAnimation(mapLocations[currMapLocation].fogLocation, 3f);

        switch (currMapLocation)
        {
            default:
                break;
            case 2:
                LetterboxController.instance.ShowTextSmooth("1 - Gorilla Village");
                break;
            case 3:
                LetterboxController.instance.ShowTextSmooth("2 - Mudslide");
                break;
            case 4:
                LetterboxController.instance.ShowTextSmooth("3 - Orc Village");
                break;
            case 5:
                LetterboxController.instance.ShowTextSmooth("4 - Spooky Forest");
                break;
            case 6:
                LetterboxController.instance.ShowTextSmooth("5 - Orc Camp");
                break;
        }
        

        yield return new WaitForSeconds(2f);

        // move letterbox out of the way
        if (!leaveLetterboxUp)
            LetterboxController.instance.ToggleLetterbox(false);

        yield return new WaitForSeconds(2f);

        RaycastBlockerController.instance.RemoveRaycastBlocker("UnlockMapArea");
    }

    public void ToggleNavButtons(bool opt)
    {
        // enable button
        if (opt)
        {   
            leftButton.interactable = true;
            rightButton.interactable = true;

            leftButton.GetComponent<NavButtonController>().isOn = true;
            rightButton.GetComponent<NavButtonController>().isOn = true;
        }
        // disable button
        else
        {
            leftButton.interactable = false;
            rightButton.interactable = false;

            // turn off glow line
            leftButton.GetComponent<NavButtonController>().TurnOffButton();
            rightButton.GetComponent<NavButtonController>().TurnOffButton();
        }

        navButtonsDisabled = !opt;
    }

    private IEnumerator SmoothImageAlpha(Image img, float startAlpha, float endAlpha, float time)
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                img.color = new Color(1f, 1f, 1f, endAlpha);
                break;
            }

            float temp = Mathf.Lerp(startAlpha, endAlpha, timer / time);
            img.color = new Color(1f, 1f, 1f, temp);

            yield return null;
        }
    }

    public MapLocation GetCurrentMapLocation()
    {
        return mapLocations[currMapLocation].location;
    }

    public void EnableMapSectionsUpTo(MapLocation location)
    {
        // disable all map sections first
        foreach (var loc in mapLocations)
        {
            loc.enabled = false;
        }

        // enable sections
        for (int i = 0; i < mapLocations.Count; i++)
        {
            if (mapLocations[i].location <= location)
            {
                mapLocations[i].enabled = true;
            }
        }
    }

    public MapLocationData GetMapLocationIcons(MapLocation location)
    {
        foreach (var item in mapLocations)
        {
            if (item.location == location)
                return item;
        }
        return null;
    }

    /* 
    ################################################
    #   MAP NAVIGATION BUTTONS
    ################################################
    */

    public void OnGoLeftPressed()
    {
        if (navButtonsDisabled) return;
        navButtonsDisabled = true;
        
        StartCoroutine(LeftPressedRoutine());
    }

    public IEnumerator LeftPressedRoutine()
    {
        StartCoroutine(NavInputDelay(1.25f));
        SoftLockMapIcons(1.25f);

        int prevMapPos = currMapLocation;;

        currMapLocation--;
        if (currMapLocation < minMapLimit)
        {
            print ("left bump!");
            currMapLocation = minMapLimit;
            StartCoroutine(BumpAnimation(true));
            yield break;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleLocationRoutine(false, prevMapPos));

        yield return new WaitForSeconds(0.1f);

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LeftBlip, 1f);

        // move map to next left map location
        float x = GetXPosFromMapLocationIndex(currMapLocation);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));

        yield return new WaitForSeconds(0.25f);

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));
    }

    public void OnGoRightPressed()
    {
        if (navButtonsDisabled) return;

        // player cannot input for 'transitionTime' seconds
        navButtonsDisabled = true;
        
        StartCoroutine(RightPressedRoutine());
    }

    public IEnumerator RightPressedRoutine()
    {
        StartCoroutine(NavInputDelay(1.25f));
        SoftLockMapIcons(1.25f);

        int prevMapPos = currMapLocation;

        currMapLocation++;
        // cant scroll past map limit
        if (currMapLocation > mapLimit)
        {
            print ("you hit da limit!");
            currMapLocation = mapLimit;
            StartCoroutine(BumpAnimation(false));
            yield break;
        }
        // cant scroll past map end
        else if (currMapLocation > mapLocations.Count - 1)
        {
            print ("right bump!");
            currMapLocation = mapLocations.Count - 1;
            StartCoroutine(BumpAnimation(false));
            yield break;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleLocationRoutine(false, prevMapPos));

        yield return new WaitForSeconds(0.1f);

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightBlip, 1f);
        
        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(currMapLocation);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));

        yield return new WaitForSeconds(0.25f);

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));
    }

    private IEnumerator NavInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        navButtonsDisabled = false;
    }

    private IEnumerator BumpAnimation(bool isLeft)
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);

        if (isLeft)
        {
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, Map.localPosition.x + bumpAmount, (bumpAnimationTime / 2)));
            yield return new WaitForSeconds((bumpAnimationTime / 2));
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, GetXPosFromMapLocationIndex(minMapLimit), (bumpAnimationTime / 2)));
        }
        else
        {
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, Map.localPosition.x - bumpAmount, (bumpAnimationTime / 2)));
            yield return new WaitForSeconds((bumpAnimationTime / 2));
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, GetXPosFromMapLocationIndex(mapLimit), (bumpAnimationTime / 2)));
        }
    }

    /* 
    ################################################
    #   MAP NAVIGATION FUNCTIONS
    ################################################
    */

    // set the index where the player can no longer go forward
    public void SetMapLimit(int index)
    {
        // print ("index: " + index);
        if (index >= 0 && index < mapLocations.Count)
        {
            FogController.instance.mapXpos = mapLocations[index].fogLocation;
            mapLimit = index;
        }
    }

    private void SetMapPosition(int index)
    {
        if (index >= 0 && index < mapLocations.Count)
        {
            currMapLocation = index;
            float tempX = GetXPosFromMapLocationIndex(index);
            //print ("index: " + index + ", pos: " + tempX);
            Map.localPosition = new Vector3(tempX, staticMapYPos, 0f);
        }   
    }

    private float GetXPosFromMapLocationIndex(int index)
    {
        return mapLocations[index].cameraLocation.localPosition.x;
    }

    private IEnumerator MapSmoothTransition(float start, float end, float transitionTime)
    {
        float timer = 0f;

        Map.localPosition = new Vector3(start, staticMapYPos, 0f);
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float pos = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer / transitionTime));
            Map.localPosition = new Vector3(pos, staticMapYPos, 0f);
            yield return null;
        }
        Map.localPosition = new Vector3(end, staticMapYPos, 0f);
    }

    /* 
    ################################################
    #   DEV FUNCTIONS 
    ################################################
    */

    public List<MapIcon> GetMapIcons()
    {
        FindObjectsWithTag("MapIcon");
        List<MapIcon> mapIconList = new List<MapIcon>();

        foreach(var obj in mapIcons)
        {
            mapIconList.Add(obj.GetComponent<MapIcon>());
        }

        return mapIconList;
    }

    public List<SignPostController> GetSignPostControllers()
    {
        FindObjectsWithTag("SignPost");
        List<SignPostController> signPosts = new List<SignPostController>();

        foreach(var obj in mapIcons)
        {
            signPosts.Add(obj.GetComponent<SignPostController>());
        }

        return signPosts;
    }

    public void SetMapIconsBroke(bool opt)
    {
        FindObjectsWithTag("MapIcon");
        foreach(GameObject mapIcon in mapIcons)
        {
            mapIcon.GetComponent<MapIcon>().SetFixed(opt, true, false);
        }
    }

    private void FindObjectsWithTag(string _tag)
    {
        mapIcons.Clear();
        Transform parent = Map;
        RecursiveGetChildObject(parent, _tag);
    }
 
    private void RecursiveGetChildObject(Transform parent, string _tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                mapIcons.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                RecursiveGetChildObject(child, _tag);
            }
        }
    }
}