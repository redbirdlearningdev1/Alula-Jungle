using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MapLocation
{
    NONE,
    GorillaVillage,
    Mudslide,
    OrcVillage,
    COUNT
}

[System.Serializable]
public struct MapLocationIcons
{
    public MapLocation location;
    public List<MapIcon> mapIcons;
    public SignPostController signPost;
}

public class ScrollMapManager : MonoBehaviour
{
    public static ScrollMapManager instance;

    [Header("Dev Stuff")]
    public bool overideMapLimit;
    [Range(0, 8)] public int mapLimitNum;

    public bool overideGameEvent;
    public StoryBeat gameEvent;

    public bool overrideStartPosition;
    public int startPos;

    private List<GameObject> mapIcons = new List<GameObject>();
    private bool repairingMapIcon = false;

    private bool revealNavUI = false;
    private bool revealGMUI = false;
    private bool waitingForGameEventRoutine = false;

    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private GameObject[] mapLocations; // the images that make up the map
    [SerializeField] private List<Transform> cameraLocations; // the positions where the camera stops at
    [SerializeField] private List<float> fogLocations; // the positions where the fog is placed
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Map Icons @ Locations")]
    public List<MapLocationIcons> mapIconsAtLocation;
    [HideInInspector] public bool showStars = true;
    [HideInInspector] public bool disableIcons = false;

    [Header("Animations")]
    public float staticMapYPos;

    public AnimationCurve curve;
    public float multiplier;

    private int mapLimit;
    private int mapPosIndex;
    private int minMapLimit = 1;
    private bool navButtonsDisabled = true;
    
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;

    [Header("Map Characters")]
    public MapIcon boat;
    public MapCharacter gorilla;
    public MapCharacter tiger;
    public MapCharacter marcus;
    public MapCharacter brutus;
    public MapCharacter clogg;


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

        // override start map pos
        if (overrideStartPosition && GameManager.instance.devModeActivated)
        {
            mapPosIndex = startPos;
        }
        else
        {
            // start at prev position (or at 1 by default)
            mapPosIndex = GameManager.instance.prevMapPosition;
        }
        GameManager.instance.SendLog(this, "starting scrollmap on position: " + mapPosIndex);
        SetMapPosition(mapPosIndex);

        // map limit
        if (overideMapLimit && GameManager.instance.devModeActivated)
            SetMapLimit(mapLimitNum); // set manual limit
        else
            SetMapLimit(StudentInfoSystem.GetCurrentProfile().mapLimit); // load map limit from SIS

        // load in map data from profile + disable all map icons
        MapDataLoader.instance.LoadMapData(StudentInfoSystem.GetCurrentProfile().mapData);
        DisableAllMapIcons();

        // get current game event
        StoryBeat playGameEvent = StoryBeat.InitBoatGame; // default event
        if (overideGameEvent && GameManager.instance.devModeActivated)
        {
            playGameEvent = gameEvent;
            
            StudentInfoSystem.GetCurrentProfile().currStoryBeat = gameEvent;
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else
        {
            playGameEvent = StudentInfoSystem.GetCurrentProfile().currStoryBeat;
        }
        
        revealNavUI = true;
        revealGMUI = true;

        /* 
        ################################################
        #   GAME EVENTS (place characters on scroll map)
        ################################################
        */

        PlaceCharactersOnScrollMap(playGameEvent);
        
        /* 
        ################################################
        #   GAME EVENTS (REPAIR MAP ICON)
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
        CheckForGameEvent();
    }

    private void PlaceCharactersOnScrollMap(StoryBeat playGameEvent)
    {
        if (playGameEvent == StoryBeat.InitBoatGame)
        {   
            
        }
        else if (playGameEvent == StoryBeat.UnlockGorillaVillage)
        {
            // place gorilla in GV
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
        }
        else if (playGameEvent == StoryBeat.GorillaVillageIntro)
        {
            // place gorilla in GV
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
        }
        else if (playGameEvent == StoryBeat.PrologueStoryGame)
        {
            // place gorilla in GV
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
        }
        else if (playGameEvent == StoryBeat.RedShowsStickerButton)
        {
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
        }
        else if (playGameEvent == StoryBeat.VillageRebuilt)
        {
            // place gorilla in GV
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_1)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerGVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusGVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusGVChallengePos.position;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_2)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerGVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusGVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusGVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_3)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerGVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusGVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusGVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
        }
        else if (playGameEvent == StoryBeat.VillageChallengeDefeated)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerGVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusGVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusGVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
            // make brutus sad
            brutus.GetComponent<Animator>().Play("brutusFixed");
        }
        else if (playGameEvent == StoryBeat.MudslideUnlocked)
        {
            
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_1)
        {       
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerMSChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusMSChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusMSChallengePos.position;
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_2)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerMSChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusMSChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusMSChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_3)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerMSChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusMSChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusMSChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
        }
        else if (playGameEvent == StoryBeat.MudslideRebuilt)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerMSChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusMSChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusMSChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
            // make brutus sad
            brutus.GetComponent<Animator>().Play("brutusFixed");

        }
        else if (playGameEvent == StoryBeat.OrcVillageMeetClogg) // default
        {
            // place clogg in village
            clogg.transform.position = MapAnimationController.instance.cloggOVPosDEFAULT.position;
        }
        else if (playGameEvent == StoryBeat.OrcVillageUnlocked)
        {
            // place clogg in village
            clogg.transform.position = MapAnimationController.instance.cloggOVPosDEFAULT.position;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_1)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerOVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusOVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusOVChallengePos.position;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_2)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerOVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusOVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusOVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_3)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerOVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusOVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusOVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
        }
        else if (playGameEvent == StoryBeat.OrcVillageRebuilt)
        {
            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerOVChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusOVChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusOVChallengePos.position;

            // make tiger sad
            tiger.GetComponent<Animator>().Play("sTigerIdle");
            // make marcus sad (ANGRY) 
            marcus.GetComponent<Animator>().Play("marcusFixed");
            // make brutus sad
            brutus.GetComponent<Animator>().Play("brutusFixed");
        }
        else if (playGameEvent == StoryBeat.COUNT) // default
        {
            // unlock everything
            
        }
    }

    private void AfterGameEventStuff()
    {
        // show UI
        if (revealNavUI)
        {
            ToggleNavButtons(true);
        }

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, mapPosIndex));

        // enable map icons
        if (!disableIcons)
            EnableAllMapIcons();

        // show GM UI
        if (revealGMUI)
        {
            SettingsManager.instance.ToggleMenuButtonActive(true);
            // show sticker button if unlocked
            if (StudentInfoSystem.GetCurrentProfile().unlockedStickerButton)
                SettingsManager.instance.ToggleWagonButtonActive(true);
        }
    }

    public void CheckForGameEvent()
    {
        StartCoroutine(CheckForGameEventRoutine());
    }   
    private IEnumerator CheckForGameEventRoutine()
    {
        StoryBeat playGameEvent = StoryBeat.InitBoatGame; // default event
        // get event from current profile if not null
        playGameEvent = StudentInfoSystem.GetCurrentProfile().currStoryBeat;

        GameManager.instance.SendLog(this, "Current Story Beat: " + playGameEvent);

        StartCoroutine(CheckForScrollMapGameEvents(playGameEvent));
        // wait here while game event stuff is happening
        while (waitingForGameEventRoutine)
            yield return null;

        yield return new WaitForSeconds(0.5f);

        AfterGameEventStuff();
    }

    private IEnumerator CheckForScrollMapGameEvents(StoryBeat playGameEvent)
    {
        waitingForGameEventRoutine = true;

        if (playGameEvent == StoryBeat.InitBoatGame)
        {   
            SetMapPosition(0);
            revealNavUI = false;
            revealGMUI = false;
            showStars = false;
            disableIcons = true;

            // intro boat animation
            MapAnimationController.instance.BoatOceanIntro();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            // wiggle boat
            boat.interactable = true;
            boat.GetComponent<WiggleController>().StartWiggle();
            //boat.GetComponent<GlowOutlineController>().ToggleGlowOutline(true); turned off bcause looks weird
        }
        else if (playGameEvent == StoryBeat.UnlockGorillaVillage)
        {
            revealNavUI = false;
            revealGMUI = false;
            showStars = false;
            disableIcons = true;

            // place camera on dock location + fog
            SetMapPosition(1);
            SetMapLimit(1);

            // bring boat into dock
            MapAnimationController.instance.DockBoat();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            // play dock 1 talkie + wait for talkie to finish
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // place gorilla in GV
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;
            gorilla.ShowExclamationMark(true);

            StartCoroutine(UnlockMapArea(2, true));
            gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(10f);

            // play dock 2 talkie + wait for talkie to finish
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // advance story beat
            StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1; // new chapter!
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();

            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.GorillaVillageIntro)
        {
            SetMapPosition(2);
            showStars = false;
            disableIcons = true;

            // make gorilla interactable
            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.PrologueStoryGame)
        {
            SetMapPosition(2);
            EnableAllMapIcons();
            showStars = false;

            // make gorilla interactable
            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.RedShowsStickerButton)
        {
            SetMapPosition(2);

            // make gorilla interactable
            gorilla.interactable = true;

            // check if player has enough coins
            if (StudentInfoSystem.GetCurrentProfile().goldCoins >= 3)
            {
                // play red notices lester talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.red_notices_lester);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // unlock button in SIS
                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                SettingsManager.instance.ToggleWagonButtonActive(true);
                // add glow + wiggle
                SettingsManager.instance.ToggleStickerButtonWiggle(true);

                // save to sis and continue
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }
        else if (playGameEvent == StoryBeat.VillageRebuilt)
        {
            SetMapPosition(2);

            // make sure player has done the sticker tutorial
            if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
            {
                // play darwin forces talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.darwin_forces);
                while (TalkieManager.instance.talkiePlaying)
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
                    // make darwin inactive
                    gorilla.interactable = false;

                    // play village rebuilt talkie 1
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_1);
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;

                    // move darwin off screen
                    MapAnimationController.instance.GorillaExitAnimationGV();
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;

                    // play village rebuilt talkie 2
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_2);
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;

                    // tiger and monkies walk in
                    MapAnimationController.instance.TigerAndMonkiesWalkInGV();
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;

                    // play village rebuilt talkie 3
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_3);
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;

                    // challenge game begins
                    MapAnimationController.instance.TigerAndMonkiesChallengePosGV();
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;

                    // make challenge games active
                    yield return new WaitForSeconds(0.5f);

                    // set tiger stuff
                    if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == GameType.None)
                    {
                        GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                        tiger.gameType = newGameType;
                        StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = newGameType;
                        StudentInfoSystem.SaveStudentPlayerData();
                    }
                    else
                    {
                        tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType;
                    }
                        
                    tiger.ShowExclamationMark(true);
                    tiger.interactable = true;
                    tiger.GetComponent<Animator>().Play("aTigerTwitch");

                    // set game manager stuff
                    GameManager.instance.mapID = MapIconIdentfier.GV_challenge_1;
                    GameManager.instance.playingChallengeGame = true;

                    // save to sis and continue
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = tiger.gameType;
                    StudentInfoSystem.AdvanceStoryBeat();
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                else
                {
                    // darwin quips
                    gorilla.interactable = true;
                }
            }
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_1)
        {
            SetMapPosition(2);

            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.GV_challenge_1;
            GameManager.instance.playingChallengeGame = true;

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
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_2)
        {
            SetMapPosition(2);

            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.GV_challenge_2;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.GorillaVillage_challengeGame_3)
        {
            SetMapPosition(2);

            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType;
            }
            
            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.GV_challenge_3;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.VillageChallengeDefeated)
        {
            SetMapPosition(2);

            yield return new WaitForSeconds(1f);

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
            mapIconsAtLocation[2].signPost.ShowSignPost(0);
            mapIconsAtLocation[2].signPost.GetComponent<SignPostController>().interactable = false;
            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked = true;
            StudentInfoSystem.SaveStudentPlayerData();

            yield return new WaitForSeconds(2f);

            // place temp copy over talkie bg
            var tempSignPost = TempObjectPlacer.instance.PlaceNewObject(mapIconsAtLocation[2].signPost.gameObject, mapIconsAtLocation[2].signPost.transform.localPosition);
            tempSignPost.GetComponent<SignPostController>().interactable = false;
            tempSignPost.GetComponent<SignPostController>().SetStars(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars);

            // play village challenge 3
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageChallengeDefeated_3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // remove temp temp signpost
            TempObjectPlacer.instance.RemoveObject();

            // before unlocking mudslide - set objects to be destroyed
            foreach (var icon in mapIconsAtLocation[3].mapIcons)
                icon.SetFixed(false, false, true);

            // unlock mudslide
            StartCoroutine(UnlockMapArea(3, false));
            gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(7f);

            // play mudslide intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.mudslideIntro);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            mapIconsAtLocation[2].signPost.GetComponent<SignPostController>().interactable = true;

            // save to SIS
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.MudslideUnlocked)
        {
            SetMapPosition(3);

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
                    GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    tiger.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                else
                {
                    tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType;
                }
                    
                tiger.ShowExclamationMark(true);
                tiger.interactable = true;
                tiger.GetComponent<Animator>().Play("aTigerTwitch");

                // set game manager stuff
                GameManager.instance.mapID = MapIconIdentfier.MS_challenge_1;
                GameManager.instance.playingChallengeGame = true;

                // save to sis and continue
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = tiger.gameType;
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {

            }
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_1)
        {
            SetMapPosition(3);

            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.MS_challenge_1;
            GameManager.instance.playingChallengeGame = true;

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
            SetMapPosition(3);
            
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.MS_challenge_2;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.Mudslide_challengeGame_3)
        {
            SetMapPosition(3);
            
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType;
            }
            
            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.MS_challenge_3;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.MudslideRebuilt)
        {
            SetMapPosition(3);
    
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
            mapIconsAtLocation[2].signPost.ShowSignPost(0);
            mapIconsAtLocation[2].signPost.GetComponent<SignPostController>().interactable = false;
            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
            StudentInfoSystem.SaveStudentPlayerData();

            // before unlocking orc village - set objects to be destroyed
            foreach (var icon in mapIconsAtLocation[4].mapIcons)
                icon.SetFixed(false, false, true);

            // place clogg in village
            clogg.transform.position = MapAnimationController.instance.cloggOVPosDEFAULT.position;

            // unlock orc village
            StartCoroutine(UnlockMapArea(4, false));
            gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(9f);

            // play orc village intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageIntro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // play orc village intro talkie
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.orcVillageIntro_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);
            mapIconsAtLocation[3].signPost.GetComponent<SignPostController>().interactable = true;

            // make clogg interactable
            clogg.interactable = true;
            clogg.ShowExclamationMark(true);

            // disable map icons
            showStars = false;
            disableIcons = true;

            // save to SIS
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.OrcVillageMeetClogg) // default
        {
            SetMapPosition(4);
            disableIcons = true;
            showStars = false;

            // make clogg interactable
            clogg.interactable = true;
            clogg.ShowExclamationMark(true);
        }
        else if (playGameEvent == StoryBeat.OrcVillageUnlocked)
        {
            SetMapPosition(4);

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
                    GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.OrcVillage);
                    tiger.gameType = newGameType;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = newGameType;
                    StudentInfoSystem.SaveStudentPlayerData();
                }
                else
                {
                    tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType;
                }
                    
                tiger.ShowExclamationMark(true);
                tiger.interactable = true;
                tiger.GetComponent<Animator>().Play("aTigerTwitch");

                // set game manager stuff
                GameManager.instance.mapID = MapIconIdentfier.OV_challenge_1;
                GameManager.instance.playingChallengeGame = true;

                // save to sis and continue
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = tiger.gameType;
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
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
            SetMapPosition(4);

            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.OrcVillage);
                tiger.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                tiger.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.OV_challenge_1;
            GameManager.instance.playingChallengeGame = true;

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
            SetMapPosition(4);
            
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.OrcVillage);
                marcus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                marcus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType;
            }

            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.OV_challenge_2;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    marcus.GoToGameDataSceneImmediately();
                }
            }

            marcus.GetComponent<Animator>().Play("marcusLose");
            marcus.ShowExclamationMark(true);
            marcus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.OrcVillage_challengeGame_3)
        {
            SetMapPosition(4);
            
            // set tiger stuff
            if (StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType == GameType.None)
            {
                GameType newGameType = StudentInfoSystem.GetChallengeGameType(MapLocation.OrcVillage);
                brutus.gameType = newGameType;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = newGameType;
                StudentInfoSystem.SaveStudentPlayerData();
            }
            else
            {
                brutus.gameType = StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType;
            }
            
            // set game manager stuff
            GameManager.instance.mapID = MapIconIdentfier.OV_challenge_3;
            GameManager.instance.playingChallengeGame = true;

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
            else
            {
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
                    // continue to marcus challenge game
                    brutus.GoToGameDataSceneImmediately();
                }
            }

            brutus.GetComponent<Animator>().Play("brutusLose");
            brutus.ShowExclamationMark(true);
            brutus.interactable = true;
        }
        else if (playGameEvent == StoryBeat.OrcVillageRebuilt)
        {
            SetMapPosition(4);

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

            // gv sign post springs into place
            mapIconsAtLocation[4].signPost.ShowSignPost(0);
            mapIconsAtLocation[4].signPost.GetComponent<SignPostController>().interactable = false;
            // Save to SIS
            StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2; // new chapter!
            StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = true;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else if (playGameEvent == StoryBeat.COUNT) // default
        {
            // unlock everything
            
        }

        waitingForGameEventRoutine = false;
    }

    private IEnumerator RepairMapIcon(MapIconIdentfier id)
    {
        repairingMapIcon = true;

        MapIcon icon = MapDataLoader.instance.GetMapIconFromID(id);
        icon.SetFixed(true, true, true);
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

    private IEnumerator ToggleLocationRoutine(bool opt, int location)
    {
        // show / hide stars of current location
        if (showStars)
        {
            foreach (var mapicon in mapIconsAtLocation[location].mapIcons)
            {
                if (opt)
                    mapicon.RevealStars();
                else
                    mapicon.HideStars();

                yield return null;
            }
        }

        // show / hide sign post
        if (mapIconsAtLocation[location].signPost != null)
        {
            if (opt)
            {
                // check SIS if signpost unlocked
                switch (mapIconsAtLocation[location].location)
                {
                    case MapLocation.NONE:
                        break;
                    case MapLocation.GorillaVillage:
                        if (StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked)
                            mapIconsAtLocation[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars);
                        break;
                    case MapLocation.Mudslide:
                        if (StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked)
                            mapIconsAtLocation[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars);
                        break;
                    case MapLocation.OrcVillage:
                        if (StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked)
                            mapIconsAtLocation[location].signPost.ShowSignPost(StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_stars);
                        break;
                    // etc ...
                }
            }
            else
            {
                mapIconsAtLocation[location].signPost.HideSignPost();
            } 
        }
    }

    public void DisableAllMapIcons()
    {
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = false;
            item.HideStars();
        }
    }

    public void EnableAllMapIcons()
    {
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = true;
            if (showStars)
                item.RevealStars();
        }
    }

    public void SetPrevMapPos()
    {
        print ("setting prev map pos to: " + mapPosIndex);
        GameManager.instance.prevMapPosition = mapPosIndex;
    }

    private IEnumerator UnlockMapArea(int mapIndex, bool leaveLetterboxUp = false)
    {
        // save unlock to sis profile
        StudentInfoSystem.GetCurrentProfile().mapLimit = mapIndex;
        StudentInfoSystem.SaveStudentPlayerData();

        RaycastBlockerController.instance.CreateRaycastBlocker("UnlockMapArea");

        yield return new WaitForSeconds(1f);

        // how Letterbox view
        LetterboxController.instance.ToggleLetterbox(true);

        yield return new WaitForSeconds(1f);

        mapLimit = mapIndex;
        mapPosIndex = mapIndex;
        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(mapIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, 2f));

        yield return new WaitForSeconds(2.5f);

        // move fog out of the way
        FogController.instance.MoveFogAnimation(fogLocations[mapIndex], 3f);

        switch (mapIndex)
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
        }
        

        yield return new WaitForSeconds(2f);

        // move letterbox out of the way
        if (!leaveLetterboxUp)
            LetterboxController.instance.ToggleLetterbox(false);

        yield return new WaitForSeconds(2f);

        // show UI again
        ToggleNavButtons(true);

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
        switch (mapPosIndex)
        {
            default:
            case 0:
            case 1:
                return MapLocation.NONE;
            case 2:
                return MapLocation.GorillaVillage;
            case 3:
                return MapLocation.Mudslide;
            case 4:
                return MapLocation.OrcVillage;
        }
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

        int prevMapPos = mapPosIndex;

        mapPosIndex--;
        if (mapPosIndex < minMapLimit)
        {
            print ("left bump!");
            mapPosIndex = minMapLimit;
            StartCoroutine(BumpAnimation(true));
            yield break;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleLocationRoutine(false, prevMapPos));

        yield return new WaitForSeconds(0.1f);

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LeftBlip, 1f);

        // move map to next left map location
        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));

        yield return new WaitForSeconds(0.25f);

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, mapPosIndex));
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

        int prevMapPos = mapPosIndex;

        mapPosIndex++;
        // cant scroll past map limit
        if (mapPosIndex > mapLimit)
        {
            print ("you hit da limit!");
            mapPosIndex = mapLimit;
            StartCoroutine(BumpAnimation(false));
            yield break;
        }
        // cant scroll past map end
        else if (mapPosIndex > cameraLocations.Count - 1)
        {
            print ("right bump!");
            mapPosIndex = cameraLocations.Count - 1;
            StartCoroutine(BumpAnimation(false));
            yield break;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleLocationRoutine(false, prevMapPos));

        yield return new WaitForSeconds(0.1f);

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightBlip, 1f);
        
        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));

        yield return new WaitForSeconds(0.25f);

        // show stars on current map location
        StartCoroutine(ToggleLocationRoutine(true, mapPosIndex));
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
        if (index >= 0 && index < cameraLocations.Count)
        {
            FogController.instance.mapXpos = fogLocations[index];
            mapLimit = index;
        }
    }

    private void SetMapPosition(int index)
    {
        if (index >= 0 && index < cameraLocations.Count)
        {
            mapPosIndex = index;
            float tempX = GetXPosFromMapLocationIndex(index);
            //print ("index: " + index + ", pos: " + tempX);
            Map.localPosition = new Vector3(tempX, staticMapYPos, 0f);
        }   
    }

    private float GetXPosFromMapLocationIndex(int index)
    {
        //print ("index: " + index + ", pos: " + cameraLocations[index].localPosition.x);
        return cameraLocations[index].localPosition.x;
    }

    private IEnumerator MapSmoothTransition(float start, float end, float transitionTime)
    {
        //GameManager.instance.SetRaycastBlocker(true);
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

        //GameManager.instance.SetRaycastBlocker(false);
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