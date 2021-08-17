using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct MapLocationIcons
{
    public List<MapIcon> mapIcons;
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
    private bool showStars = true;

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

    [Header("Sticker Video Players")]
    public VideoPlayer commonVP;
    public VideoPlayer uncommonVP;
    public VideoPlayer rareVP;
    public VideoPlayer legendaryVP;

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
            SetMapLimit(StudentInfoSystem.currentStudentPlayer.mapLimit); // load map limit from SIS

        if (StudentInfoSystem.currentStudentPlayer != null)
        {
            // load in map data from profile
            MapDataLoader.instance.LoadMapData(StudentInfoSystem.currentStudentPlayer.mapData);
        }

        // get current game event
        StoryBeat playGameEvent = StoryBeat.InitBoatGame; // default event
        if (overideGameEvent && GameManager.instance.devModeActivated)
        {
            playGameEvent = gameEvent;
            
            StudentInfoSystem.currentStudentPlayer.currStoryBeat = gameEvent;
            StudentInfoSystem.SaveStudentPlayerData();
        }
        else
        {
            // get event from current profile if not null
            if (StudentInfoSystem.currentStudentPlayer != null)
            {
                playGameEvent = StudentInfoSystem.currentStudentPlayer.currStoryBeat;
            }
        }
        
        revealNavUI = true;
        revealGMUI = true;

        // place gorilla in GV
        MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.gorillaGVPosSTART.position;

        
        /* 
        ################################################
        #   GAME EVENTS (REPAIR MAP ICON)
        ################################################
        */

        if (GameManager.instance.repairMapIconID)
        {
            print ("repairing map icon: " + GameManager.instance.GetID());
            DisableAllMapIcons();

            yield return new WaitForSeconds(1f);

            StartCoroutine(RepairMapIcon(GameManager.instance.GetID()));
            while (repairingMapIcon)
                yield return null;
            
            EnableAllMapIcons();
        }

        /* 
        ################################################
        #   GAME EVENTS (STORY BEATS)
        ################################################
        */
        // check for game events
        CheckForGameEvent();
    }

    private void AfterGameEventStuff()
    {
        // show UI
        if (revealNavUI)
        {
            ToggleNavButtons(true);
        }

        // show stars on current map location
        StartCoroutine(ToggleMapIconStarsRoutine(true, mapPosIndex));

        // show GM UI
        if (revealGMUI)
        {
            SettingsManager.instance.ToggleMenuButtonActive(true);
            // show sticker button if unlocked
            if (StudentInfoSystem.currentStudentPlayer.unlockedStickerButton)
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
        if (StudentInfoSystem.currentStudentPlayer != null)
        {
            playGameEvent = StudentInfoSystem.currentStudentPlayer.currStoryBeat;
        }

        StartCoroutine(CheckForScrollMapGameEvents(playGameEvent));
        // wait here while game event stuff is happening
        while (waitingForGameEventRoutine)
            yield return null;

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
            DisableAllMapIcons();

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
            DisableAllMapIcons();

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
            gorilla.ShowExclamationMark(true);

            StartCoroutine(UnlockMapArea(2, true));
            gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(10f);

            // play dock 2 talkie + wait for talkie to finish
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            gorilla.interactable = true;

            // update SIS
            if (!overideGameEvent)
            {
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }
        else if (playGameEvent == StoryBeat.GorillaVillageIntro)
        {
            DisableAllMapIcons();
            showStars = false;

            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.PrologueStoryGame)
        {
            showStars = false;

            gorilla.ShowExclamationMark(true);
            gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.RedShowsStickerButton)
        {
            // darwin quips
            gorilla.interactable = true;

            // check if player has enough coins
            if (StudentInfoSystem.currentStudentPlayer.goldCoins >= 3)
            {
                // play red notices lester talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.red_notices_lester);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;

                // save to sis and continue
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }
        else if (playGameEvent == StoryBeat.VillageRebuilt)
        {
            // darwin quips
            gorilla.interactable = true;

            // make sure player has done the sticker tutorial
            if (!StudentInfoSystem.currentStudentPlayer.stickerTutorial)
            {
                // play darwin forces talkie
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.darwin_forces);
                while (TalkieManager.instance.talkiePlaying)
                    yield return null;
            }
            else
            {
                // make sure player has rebuilt all the GV map icons
                if (StudentInfoSystem.currentStudentPlayer.mapData.GV_house1.isFixed &&
                    StudentInfoSystem.currentStudentPlayer.mapData.GV_house2.isFixed &&
                    StudentInfoSystem.currentStudentPlayer.mapData.GV_statue.isFixed &&
                    StudentInfoSystem.currentStudentPlayer.mapData.GV_fire.isFixed)
                {
                    // make sure we are at gorilla village
                    mapPosIndex = 2;
                    // move map to next right map location
                    float x = GetXPosFromMapLocationIndex(mapPosIndex);
                    StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, 2f));

                    yield return new WaitForSeconds(2.5f);

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
                    MapAnimationController.instance.TigerAndMonkiesWalkIn();
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;

                    // play village rebuilt talkie 3
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.villageRebuilt_3);
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;

                    // challenge game begins
                    MapAnimationController.instance.TigerAndMonkiesChallengePos();
                    // wait for animation to be done
                    while (!MapAnimationController.instance.animationDone)
                        yield return null;

                    // make challenge games active

                    // save to sis and continue
                    StudentInfoSystem.AdvanceStoryBeat();
                    StudentInfoSystem.SaveStudentPlayerData();
                }
            }
        }
        else if (playGameEvent == StoryBeat.ChallengeGames_GorillaVillage)
        {
            // place gorilla off-screen
            MapAnimationController.instance.gorilla.transform.position = MapAnimationController.instance.offscreenPos.position;

            // place tiger and monkies on screen
            MapAnimationController.instance.tiger.transform.position = MapAnimationController.instance.tigerChallengePos.position;
            MapAnimationController.instance.marcus.transform.position = MapAnimationController.instance.marcusChallengePos.position;
            MapAnimationController.instance.brutus.transform.position = MapAnimationController.instance.brutusChallengePos.position;

            // make challenge games active
        }
        else if (playGameEvent == StoryBeat.COUNT) // default
        {
            // darwin quips
            gorilla.interactable = true;
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

    private IEnumerator ToggleMapIconStarsRoutine(bool opt, int location)
    {
        // show stars of current location
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
    }

    public void DisableAllMapIcons()
    {
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = false;
        }
    }

    public void EnableAllMapIcons()
    {
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = true;
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
        StudentInfoSystem.currentStudentPlayer.mapLimit = mapIndex;
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

        LetterboxController.instance.ShowTextSmooth("1 - Gorilla Village");

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

    /* 
    ################################################
    #   MAP NAVIGATION BUTTONS
    ################################################
    */

    public void OnGoLeftPressed()
    {
        if (navButtonsDisabled) return;

        // player cannot input for 'transitionTime' seconds
        navButtonsDisabled = true;
        StartCoroutine(NavInputDelay(transitionTime));

        int prevMapPos = mapPosIndex;

        mapPosIndex--;
        if (mapPosIndex < minMapLimit)
        {
            print ("left bump!");
            mapPosIndex = minMapLimit;
            StartCoroutine(BumpAnimation(true));
            return;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleMapIconStarsRoutine(false, prevMapPos));
        // show stars on current map location
        StartCoroutine(ToggleMapIconStarsRoutine(true, mapPosIndex));

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LeftBlip, 1f);

        // move map to next left map location
        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));
    }

    public void OnGoRightPressed()
    {
        if (navButtonsDisabled) return;

        // player cannot input for 'transitionTime' seconds
        navButtonsDisabled = true;
        StartCoroutine(NavInputDelay(transitionTime));

        int prevMapPos = mapPosIndex;

        mapPosIndex++;
        // cant scroll past map limit
        if (mapPosIndex > mapLimit)
        {
            print ("you hit da limit!");
            mapPosIndex = mapLimit;
            StartCoroutine(BumpAnimation(false));
            return;
        }
        // cant scroll past map end
        else if (mapPosIndex > cameraLocations.Count - 1)
        {
            print ("right bump!");
            mapPosIndex = cameraLocations.Count - 1;
            StartCoroutine(BumpAnimation(false));
            return;
        }

        // hide stars from prev map pos
        StartCoroutine(ToggleMapIconStarsRoutine(false, prevMapPos));
        // show stars on current map location
        StartCoroutine(ToggleMapIconStarsRoutine(true, mapPosIndex));

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightBlip, 1f);
        
        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, transitionTime));
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
        print ("index: " + index);
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