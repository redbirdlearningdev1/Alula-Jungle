using System.Collections;
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

    NONE
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
    private bool inPalace = false; // is the place in the palace?

    private bool activateMapNavigation = false;
    private bool revealGMUI = false;
    private bool waitingForGameEventRoutine = false;
    [HideInInspector] public bool updateGameManagerBools;

    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Map Icons @ Locations")]
    public List<MapLocationData> mapLocations;
    public Transform prePalaceCamPos;
    public Transform palaceCamPos;

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

        // by deafult - update GM bools
        updateGameManagerBools = true;

        // disable UI
        leftButton.interactable = false;
        rightButton.interactable = false;
        navButtonsDisabled = true;
        activateMapNavigation = true;
        revealGMUI = true;

        // load in map data from profile + disable all map icons
        MapDataLoader.instance.LoadMapData(StudentInfoSystem.GetCurrentProfile().mapData);
        DisableAllMapIcons(true);

        // set map location to be prev location if not null
        if (GameManager.instance.prevMapLocation != MapLocation.NONE)
        {
            SetMapPosition((int)GameManager.instance.prevMapLocation);
            GameManager.instance.prevMapLocation = MapLocation.NONE;
        }
        else
        {   
            SetMapPosition(StudentInfoSystem.GetCurrentProfile().mapLimit);
        }
        SetMapLimit(StudentInfoSystem.GetCurrentProfile().mapLimit);

        // update settings map
        ScrollSettingsWindowController.instance.UpdateRedPos(mapLocations[currMapLocation].location);
        ScrollSettingsWindowController.instance.UpdateMapSprite();

        // get current game event
        StoryBeat playGameEvent = StudentInfoSystem.GetCurrentProfile().currStoryBeat;
        GameManager.instance.SendLog(this, "current story beat: \"" + playGameEvent + "\"");

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
        #   GAME EVENTS (AFTER ROYAL RUMBLE TALKIES)
        ################################################
        */

        if (GameManager.instance.finishedRoyalRumbleGame)
        {
            GameManager.instance.finishedRoyalRumbleGame = false;

            Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

            if (GameManager.instance.wonRoyalRumbleGame)
            {
                // correct player win quip
                switch (currChapter)
                {
                    case Chapter.chapter_0:
                    case Chapter.chapter_1:
                    case Chapter.chapter_2:
                    case Chapter.chapter_3:
                        // play julius loses to player
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRJuliusLost_1_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;
                    case Chapter.chapter_4:
                    case Chapter.chapter_5:
                    case Chapter.chapter_6:
                        // play guard loses to player
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsLost_1_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;
                }
            }
            else
            {
                // correct player lose quip 
                switch (currChapter)
                {
                    case Chapter.chapter_0:
                    case Chapter.chapter_1:
                    case Chapter.chapter_2:
                    case Chapter.chapter_3:
                        // play julius wins to player
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRJuliusWins_1_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;
                    case Chapter.chapter_4:
                    case Chapter.chapter_5:
                    case Chapter.chapter_6:
                        // play guard wins to player
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsWins_1_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                        break;
                }
            }
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

        // remove game manager stuff
        if (updateGameManagerBools)
        {
            GameManager.instance.playingRoyalRumbleGame = false;
            GameManager.instance.playingChallengeGame = false;
            GameManager.instance.playingBossBattleGame = false;
            GameManager.instance.finishedBoatGame = false;
        }
        

        // show palace arrow if past story beat
        if (playGameEvent >= StoryBeat.PreBossBattle && currMapLocation == (int)MapLocation.PalaceIntro)
        {   
            StartCoroutine(DelayShowPalaceArrow());
        }
        
        // show UI
        if (activateMapNavigation)
        {
            StartCoroutine(DelayToggleNavButtons());
        }

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

        // update map icons
        StartCoroutine(DelayUpdateMapIcons(0.5f, false));
    }

    private IEnumerator DelayToggleNavButtons()
    {
        yield return new WaitForSeconds(2f);
        ToggleNavButtons(true);
    }

    private IEnumerator DelayShowGMUI()
    {
        yield return new WaitForSeconds(1f);
        PalaceArrow.instance.ShowArrow();
    }

    private IEnumerator DelayShowPalaceArrow()
    {
        yield return new WaitForSeconds(1f);
        PalaceArrow.instance.ShowArrow();
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

        // enable map sections up to map limit
        EnableMapSectionsUpTo(mapLocations[mapLimit].location);

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

            // start on boat dock
            SetMapPosition((int)MapLocation.BoatHouse);
            SetMapLimit((int)MapLocation.BoatHouse);

            // reveal gorilla village
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
        else if (playGameEvent == StoryBeat.PrologueStoryGame)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.BoatHouse);
        }
        else if (playGameEvent == StoryBeat.RedShowsStickerButton)
        {
            // check if player has enough coins
            if (StudentInfoSystem.GetCurrentProfile().goldCoins >= 4)
            {
                // play red shows sticker button
                MapAnimationController.instance.PlayMapAnim(MapAnim.RedShowsStickerButton);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.VillageRebuilt)
        {
            // make sure player has done the sticker tutorial
            if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
            {
                // play darwin forces lester interaction
                MapAnimationController.instance.PlayMapAnim(MapAnim.DarwinForcesLesterInteraction);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
            else
            {
                // make sure player has rebuilt all the GV map icons
                if (StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed &&
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed)
                {
                    // override map location
                    SetMapPosition((int)MapLocation.GorillaVillage);
                    // play GV rebuilt
                    MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaVillageRebuilt);
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;
                }
            }
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.GorillaVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_2)
        {
            // play challenge game 2 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.GorillaVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_3)
        {
            // play challenge game 3 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.GorillaVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.VillageChallengeDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.GorillaVillage);
            // play village defeated animation
            MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaVillageDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MudslideUnlocked)
        {
            // make sure player has rebuilt all the MS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.Mudslide);
                // play MS rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.MudslideRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.Mudslide);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.Mudslide);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.Mudslide);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MudslideDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.Mudslide);
            // play MS defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.MudslideDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcVillageMeetClogg)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.Mudslide);
        }
        else if (playGameEvent == StoryBeat.OrcVillageUnlocked)
        {
            // make sure player has rebuilt all the OV map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.OrcVillage);
                // play OV rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.OrcVillageRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.OrcVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_2)
        {
            // play challenge game 2 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.OrcVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_3)
        {
            // play challenge game 3 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.OrcVillage);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcVillageDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.OrcVillage);
            // play OV defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.OrcVillageDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.SpookyForestUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.OrcVillage);
        }
        else if (playGameEvent == StoryBeat.BeginningStoryGame)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.OrcVillage);
        }
        else if (playGameEvent == StoryBeat.SpookyForestPlayGames)
        {
            // make sure player has rebuilt all the SF map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.SpookyForest);
                // play SF rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.SpookyForestRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.SpookyForest);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.SpookyForest);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.SpookyForest_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.SpookyForest);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.SpookyForestDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.SpookyForest);
            // play SF defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.SpookyForestDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcCampUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.SpookyForest);
        }
        else if (playGameEvent == StoryBeat.OrcCampPlayGames)
        {
            // make sure player has rebuilt all the OC map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.OrcCamp);
                // play OC rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.OrcCampRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.OrcCamp_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.OrcCamp);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcCamp_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.OrcCamp);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcCamp_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.OrcCamp);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.OrcCampDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.OrcCamp);
            // play SF defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.OrcCampDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaPoopPlayGames)
        {
            // make sure player has rebuilt all the OC map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.isFixed)
            {
                // override map location
            SetMapPosition((int)MapLocation.GorillaPoop);
                // play OC rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaPoopRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.GorillaPoop_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.GorillaPoop);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaPoop_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.GorillaPoop);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaPoop_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.GorillaPoop);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaPoopDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.GorillaPoop);
            // play GP defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaPoopDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.WindyCliffUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.GorillaPoop);
        }
        else if (playGameEvent == StoryBeat.FollowRedStoryGame)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.GorillaPoop);
        }
        else if (playGameEvent == StoryBeat.WindyCliffPlayGames)
        {
            // make sure player has rebuilt all the WC map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.WindyCliff);
                // play WCS rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.WindyCliffRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.WindyCliff_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.WindyCliff);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.WindyCliff_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.WindyCliff);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.WindyCliff_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.WindyCliff);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.WindyCliffDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.WindyCliff);
            // play WC defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.WindyCliffDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.PirateShipPlayGames)
        {
            // make sure player has rebuilt all the PS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.PS_front.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.PirateShip);
                // play PS rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.PirateShipRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.PirateShip_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.PirateShip);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.PirateShip_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.PirateShip);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.PirateShip_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.PirateShip);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.PirateShipDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.PirateShip);
            // play PS defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.PirateShipDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MermaidBeachUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.PirateShip);
        }
        else if (playGameEvent == StoryBeat.MermaidBeachPlayGames)
        {
            // make sure player has rebuilt all the PS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.MermaidBeach);
                // play MB rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.MermaidBeachRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.MermaidBeach_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.MermaidBeach);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MermaidBeach_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.MermaidBeach);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MermaidBeach_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.MermaidBeach);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MermaidBeachDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.MermaidBeach);
            // play PS defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.MermaidBeachDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.RuinsPlayGames)
        {
            // make sure player has rebuilt all the R map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.R_arch.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.R_face.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.Ruins1);
                // play R rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.RuinsRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.Ruins_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.Ruins1);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Ruins_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.Ruins1);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Ruins_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.Ruins1);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.RuinsDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.Ruins1);
            // play R defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.RuinsDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.ExitJungleUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.Ruins2);
        }
        else if (playGameEvent == StoryBeat.ResolutionStoryGame)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.Ruins2);
        }
        else if (playGameEvent == StoryBeat.ExitJunglePlayGames)
        {
            // make sure player has rebuilt all the R map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.ExitJungle);
                // play R rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.ExitJungleRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.ExitJungle_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.ExitJungle);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.ExitJungle_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.ExitJungle);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.ExitJungle_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.ExitJungle);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.ExitJungleDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.ExitJungle);
            // play EJ defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.ExitJungleDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaStudyUnlocked)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.ExitJungle);
        }
        else if (playGameEvent == StoryBeat.GorillaStudyPlayGames)
        {
            // make sure player has rebuilt all the GS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.isFixed)
            {
                // override map location
                SetMapPosition((int)MapLocation.GorillaStudy);
                // play GS rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaStudyRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.GorillaStudy_challengeGame_1)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.GorillaStudy);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaStudy_challengeGame_2)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.GorillaStudy);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaStudy_challengeGame_3)
        {
            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.GorillaStudy);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.GorillaStudyDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.GorillaStudy);
            // play GS defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.GorillaStudyDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MonkeysPlayGames)
        {
            // make sure player has rebuilt all the GS map icons
            if (StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.M_flower.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.M_guards.isFixed &&
                StudentInfoSystem.GetCurrentProfile().mapData.M_tree.isFixed)
            {
                // change enabled map sections
                EnableMapSectionsUpTo(MapLocation.GorillaStudy);
                // override map location
                SetMapPosition((int)MapLocation.Monkeys);
                // play M rebuilt
                MapAnimationController.instance.PlayMapAnim(MapAnim.MonkeysRebuilt);
                // wait for animation to be done
                while (!MapAnimationController.instance.animationDone)
                    yield return null;
            }
        }
        else if (playGameEvent == StoryBeat.Monkeys_challengeGame_1)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.GorillaStudy);

            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame1, MapLocation.Monkeys);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Monkeys_challengeGame_2)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.GorillaStudy);

            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame2, MapLocation.Monkeys);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.Monkeys_challengeGame_3)
        {
            // change enabled map sections
            EnableMapSectionsUpTo(MapLocation.GorillaStudy);

            // play challenge game 1 map animation
            MapAnimationController.instance.PlayChallengeGameMapAnim(MapAnim.ChallengeGame3, MapLocation.Monkeys);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.MonkeysDefeated)
        {
            // override map location
            SetMapPosition((int)MapLocation.Monkeys);
            // play M defeated
            MapAnimationController.instance.PlayMapAnim(MapAnim.MonkeysDefeated);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.PalaceIntro)
        {

        }
        else if (playGameEvent == StoryBeat.PreBossBattle)
        {
            
        }
        else if (playGameEvent == StoryBeat.BossBattle1)
        {   
            // start camera on palace location
            Map.localPosition = new Vector3(prePalaceCamPos.localPosition.x, palaceCamPos.localPosition.y, 0f);
            inPalace = true;
            // play boss battle game 1 map animation
            MapAnimationController.instance.PlayPreBossBattleGameMapAnim(MapAnim.BossBattle1);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.BossBattle2)
        {
            // start camera on palace location
            Map.localPosition = new Vector3(prePalaceCamPos.localPosition.x, palaceCamPos.localPosition.y, 0f);
            inPalace = true;
            // play boss battle game 2 map animation
            MapAnimationController.instance.PlayPreBossBattleGameMapAnim(MapAnim.BossBattle2);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.BossBattle3)
        {
            // start camera on palace location
            Map.localPosition = new Vector3(prePalaceCamPos.localPosition.x, palaceCamPos.localPosition.y, 0f);
            inPalace = true;
            // play boss battle game 3 map animation
            MapAnimationController.instance.PlayPreBossBattleGameMapAnim(MapAnim.BossBattle3);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.EndBossBattle)
        {
            // do not show UI buttons
            revealGMUI = false;
            inPalace = true;
            // start camera on palace location
            Map.localPosition = new Vector3(prePalaceCamPos.localPosition.x, palaceCamPos.localPosition.y, 0f);
            // play end boss battle
            MapAnimationController.instance.PlayMapAnim(MapAnim.EndBossBattle);
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
        }
        else if (playGameEvent == StoryBeat.FinishedGame)
        {
            // start on boat house
            SetMapPosition((int)MapLocation.BoatHouse);
        }

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

    public void HideStarsAtCurrentLocation()
    {
        StartCoroutine(ToggleLocationRoutine(false, currMapLocation));
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

                case MapLocation.GorillaPoop:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_stars, GetMapLocationIcons(MapLocation.GorillaPoop).enabled);
                        return;
                    }
                    break;

                case MapLocation.WindyCliff:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_stars, GetMapLocationIcons(MapLocation.WindyCliff).enabled);
                        return;
                    }
                    break;
                
                case MapLocation.PirateShip:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_stars, GetMapLocationIcons(MapLocation.PirateShip).enabled);
                        return;
                    }
                    break;

                case MapLocation.MermaidBeach:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_stars, GetMapLocationIcons(MapLocation.MermaidBeach).enabled);
                        return;
                    }
                    break;

                case MapLocation.Ruins1:
                case MapLocation.Ruins2:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_unlocked)
                    {
                        mapLocations[(int)MapLocation.Ruins1].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_stars, GetMapLocationIcons(MapLocation.Ruins1).enabled);
                        return;
                    }
                    break;

                case MapLocation.ExitJungle:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_stars, GetMapLocationIcons(MapLocation.ExitJungle).enabled);
                        return;
                    }
                    break;

                case MapLocation.GorillaStudy:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_stars, GetMapLocationIcons(MapLocation.GorillaStudy).enabled);
                        return;
                    }
                    break;

                case MapLocation.Monkeys:
                    if (StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_unlocked)
                    {
                        mapLocations[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_stars, GetMapLocationIcons(MapLocation.Monkeys).enabled);
                        return;
                    }
                    break;
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

    private IEnumerator DelayUpdateMapIcons(float delay, bool revealStars)
    {
        yield return new WaitForSeconds(delay);
        UpdateMapIcons(revealStars);
    }

    public void UpdateMapIcons(bool revealStars)
    {
        // reload data
        MapDataLoader.instance.LoadMapData(StudentInfoSystem.GetCurrentProfile().mapData);
        foreach (var item in mapLocations)
        {
            EnableMapIcons(item, revealStars);
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

    public void SmoothGoToMapLocation(MapLocation location)
    {
        // return if already at map location
        if ((int)location == currMapLocation)
            return;

        // return if location is past map limit
        if ((int)location > mapLimit)
            return;

        StartCoroutine(SmoothGoToMapLocationRoutine(location));
    }

    private IEnumerator SmoothGoToMapLocationRoutine(MapLocation location)
    {
        // remove player input
        RaycastBlockerController.instance.CreateRaycastBlocker("GoToMapLocation");
        ToggleNavButtons(false);

        // close settings window
        SettingsManager.instance.CloseAllSettingsWindows();
        // remove all stars
        DisableAllMapIcons(true);
        // remove GM UI
        SettingsManager.instance.ToggleMenuButtonActive(false);
        // remove sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(false);

        yield return new WaitForSeconds(1f);

        // if in palace, hide battle bar + pan down to palace intro
        if (inPalace)
        {
            inPalace = false;

            // hide UI
            PalaceArrowDown.instance.HideArrow();
            PalaceArrow.instance.HideArrow();

            // hide boss bar if shown
            if (BossBattleBar.instance.barShown)
                BossBattleBar.instance.HideBar();

            // pan camera down to palace intro
            float y = staticMapYPos;
            StartCoroutine(MapSmoothTransitionY(Map.localPosition.y, y, 2f));
            yield return new WaitForSeconds(2.1f);
        }

        currMapLocation = (int)location;

        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(currMapLocation);
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, 2f));

        yield return new WaitForSeconds(2.5f);

        // show current stars + enable map icons
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));
        EnableMapIcons(mapLocations[currMapLocation], true);

        // update settings map
        ScrollSettingsWindowController.instance.UpdateRedPos(location);
        ScrollSettingsWindowController.instance.UpdateMapSprite();

        // add GM UI
        SettingsManager.instance.ToggleMenuButtonActive(true);
        // add sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(true);

        // add player input
        RaycastBlockerController.instance.RemoveRaycastBlocker("GoToMapLocation");
        ToggleNavButtons(true);
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
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, 2f));

        yield return new WaitForSeconds(2.5f);

        // move fog out of the way
        FogController.instance.MoveFogAnimation(mapLocations[currMapLocation].fogLocation, 3f);

        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.FoundIslandSparkle, 0.5f, "section_reveal", 0.6f);

        // set sign
        ChapterEnterVisualController.instance.SetSign(location);
        ChapterEnterVisualController.instance.ShowSign();

        yield return new WaitForSeconds(4f);

        // move letterbox out of the way
        if (!leaveLetterboxUp)
            LetterboxController.instance.ToggleLetterbox(false);

        // hide sign
        ChapterEnterVisualController.instance.HideSign();

        // disable previous location
        StartCoroutine(ToggleLocationRoutine(false, currMapLocation - 1));

        yield return new WaitForSeconds(1f);

        RaycastBlockerController.instance.RemoveRaycastBlocker("UnlockMapArea");
    }

    public void PanIntoPalace()
    {
        StartCoroutine(PanIntoPalaceRoutine());
    }   

    private IEnumerator PanIntoPalaceRoutine()
    {
        // remove GM UI
        SettingsManager.instance.ToggleMenuButtonActive(false);
        // remove sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(false);

        // move map to pre palace pos
        float x = prePalaceCamPos.localPosition.x;
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, 1f));
        yield return new WaitForSeconds(1.2f);

        // pan camera up to palace
        float y = palaceCamPos.localPosition.y;
        StartCoroutine(MapSmoothTransitionY(Map.localPosition.y, y, 3f));
        yield return new WaitForSeconds(3f);

        // add GM UI
        SettingsManager.instance.ToggleMenuButtonActive(true);
        // add sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(true);

        inPalace = true;
    }

    public void PanOutOfPalace()
    {
        StartCoroutine(PanOutOfPalaceRoutine());
    }   

    private IEnumerator PanOutOfPalaceRoutine()
    {
        // remove GM UI
        SettingsManager.instance.ToggleMenuButtonActive(false);
        // remove sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(false);

        // pan camera down to palace intro
        float y = staticMapYPos;
        StartCoroutine(MapSmoothTransitionY(Map.localPosition.y, y, 3f));
        yield return new WaitForSeconds(3f);

        // move map to pre palace pos
        float x = mapLocations[16].cameraLocation.localPosition.x;
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, 1f));
        yield return new WaitForSeconds(1.2f);

        // add GM UI
        SettingsManager.instance.ToggleMenuButtonActive(true);
        // add sticker button if unlocked
        if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
            SettingsManager.instance.ToggleWagonButtonActive(true);

        inPalace = false;
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
            currMapLocation = minMapLimit;
            StartCoroutine(BumpAnimation(true));
            yield break;
        }

        // hide arrow if leaving palace intro
        if (prevMapPos == 16 && StudentInfoSystem.GetCurrentProfile().currStoryBeat > StoryBeat.PalaceIntro)
        {
            // hide arrow to go up to palace
            PalaceArrow.instance.HideArrow();
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleLocationRoutine(false, prevMapPos));

        yield return new WaitForSeconds(0.1f);

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LeftBlip, 1f);

        // move map to next left map location
        float x = GetXPosFromMapLocationIndex(currMapLocation);
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, transitionTime));

        // update map pos
        ScrollSettingsWindowController.instance.UpdateRedPos(mapLocations[currMapLocation].location);

        yield return new WaitForSeconds(0.8f);

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
            currMapLocation = mapLimit;
            StartCoroutine(BumpAnimation(false));
            yield break;
        }
        // cant scroll past map end
        else if (currMapLocation > mapLocations.Count - 1)
        {
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
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, transitionTime));

        // update map pos
        ScrollSettingsWindowController.instance.UpdateRedPos(mapLocations[currMapLocation].location);

        yield return new WaitForSeconds(0.8f);

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, currMapLocation));

        yield return new WaitForSeconds(0.5f);

        // if new location is palace intro - show arrow if past story beat
        if (currMapLocation == 16 && StudentInfoSystem.GetCurrentProfile().currStoryBeat > StoryBeat.PalaceIntro)
        {
            // show arrow to go up to palace
            PalaceArrow.instance.ShowArrow();
        }
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
            StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, Map.localPosition.x + bumpAmount, (bumpAnimationTime / 2)));
            yield return new WaitForSeconds((bumpAnimationTime / 2));
            StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, GetXPosFromMapLocationIndex(minMapLimit), (bumpAnimationTime / 2)));
        }
        else
        {
            StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, Map.localPosition.x - bumpAmount, (bumpAnimationTime / 2)));
            yield return new WaitForSeconds((bumpAnimationTime / 2));
            StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, GetXPosFromMapLocationIndex(mapLimit), (bumpAnimationTime / 2)));
        }
    }

    /* 
    ################################################
    #   OTHER MAP FUNCTIONS
    ################################################
    */

    // set the index where the player can no longer go forward
    public void SetMapLimit(int index)
    {
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
            Map.localPosition = new Vector3(tempX, staticMapYPos, 0f);
        }   
    }

    public void GoToMapPosition(MapLocation location)
    {
        // move map to map location
        float x = GetXPosFromMapLocationIndex((int)location);
        StartCoroutine(MapSmoothTransitionX(Map.localPosition.x, x, 2f));
    }

    private float GetXPosFromMapLocationIndex(int index)
    {
        return mapLocations[index].cameraLocation.localPosition.x;
    }

    private IEnumerator MapSmoothTransitionX(float start, float end, float transitionTime)
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

    private IEnumerator MapSmoothTransitionY(float start, float end, float transitionTime)
    {
        float timer = 0f;
        float currXPos = Map.localPosition.x;

        Map.localPosition = new Vector3(currXPos, staticMapYPos, 0f);
        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float pos = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer / transitionTime));
            Map.localPosition = new Vector3(currXPos, pos, 0f);
            yield return null;
        }
        Map.localPosition = new Vector3(currXPos, end, 0f);
    }

    
    /* 
    ################################################
    #   DEV FUNCTIONS 
    ################################################
    */

    public void ToggleCurrentMapIconColliders(bool opt)
    {
        List<MapIcon> mapIcons = new List<MapIcon>();
        mapIcons.AddRange(mapLocations[currMapLocation].mapIcons);

        foreach (var icon in mapIcons)
        {
            icon.GetCurrentCollider().enabled = opt;
        }
    }

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