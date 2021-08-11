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

    private int mapLimit;
    private int mapPosIndex;
    private int minMapLimit = 1;
    private bool navButtonsDisabled = true;
    
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;

    [Header("Map Characters")]
    public MapIcon boat;
    public MapCharacter GV_Gorilla;

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

        // check for game events
        StoryBeat playGameEvent = StoryBeat.InitBoatGame; // default event
        if (overideGameEvent && GameManager.instance.devModeActivated)
        {
            playGameEvent = gameEvent;
        }
        else
        {
            // get event from current profile if not null
            if (StudentInfoSystem.currentStudentPlayer != null)
                playGameEvent = StudentInfoSystem.currentStudentPlayer.currStoryBeat;
        }
        
        bool revealNavUI = true;
        bool revealGMUI = true;

        /* 
        ################################################
        #   GAME EVENTS (STORY BEATS)
        ################################################
        */
        // check for game events
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
            boat.GetComponent<SpriteWiggleController>().StartWiggle();
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

            StartCoroutine(UnlockMapArea(2, true));
            GV_Gorilla.ShowExclamationMark(true);

            yield return new WaitForSeconds(10f);

            // play dock 2 talkie + wait for talkie to finish
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            GV_Gorilla.interactable = true;

            // update SIS
            if (!overideGameEvent)
            {
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }
        else if (playGameEvent == StoryBeat.PrologueStoryGame)
        {
            DisableAllMapIcons();
            showStars = false;

            GV_Gorilla.ShowExclamationMark(true);
            GV_Gorilla.interactable = true;
        }
        else if (playGameEvent == StoryBeat.StickerTutorial)
        {
            // check if player has enough coins
            if (StudentInfoSystem.currentStudentPlayer.goldCoins >= 3)
            {
                // play talkie here i guess

                // save to sis and continue
                StudentInfoSystem.currentStudentPlayer.unlockedStickerButton = true;
                StudentInfoSystem.AdvanceStoryBeat();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }

        // show UI
        if (revealNavUI)
        {
            yield return new WaitForSeconds(0.5f);
            TurnOffNavigationUI(false);
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

    private void DisableAllMapIcons()
    {
        var list = GetMapIcons();
        foreach(var item in list)
        {
            item.interactable = false;
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
        TurnOffNavigationUI(false);

        RaycastBlockerController.instance.RemoveRaycastBlocker("UnlockMapArea");
    }

    public void TurnOffNavigationUI(bool opt)
    {
        // do nothing if already same as opt
        if (opt == navButtonsDisabled)
            return;

        // enable button
        if (!opt)
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

            leftButton.GetComponent<NavButtonController>().isOn = false;
            rightButton.GetComponent<NavButtonController>().isOn = false;

            // turn off glow line
            leftButton.GetComponent<NavButtonController>().TurnOffButton();
            rightButton.GetComponent<NavButtonController>().TurnOffButton();
        }

        navButtonsDisabled = opt;
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

    // private void GoToNearestMapLocation()
    // {
    //     float currPercent = GetMapPositionPercentage(Map.position.x);
    //     //print ("current location percent: " + currPercent);

    //     float minDist = float.MaxValue;
    //     int minIndex = 0;
    //     for (int i = 0; i < cameraLocations.Count; i++)
    //     {
    //         float indexPercent = GetMapPositionPercentage(i);
    //         float dist = Mathf.Abs(currPercent - indexPercent);
    //         //print ("location: " + i + ", percent: " + indexPercent + ", distance from current: " + dist);

    //         if (dist < minDist)
    //         {
    //             minDist = dist;
    //             minIndex = i;
    //         }
    //     }

    //     //print ("nearest location: " + minIndex + ", distance from current: " + minDist);
    //     mapPosIndex = minIndex;
    //     float xPos = GetXPosFromMapLocationIndex(minIndex);
    //     StartCoroutine(MapSmoothTransition(Map.position.x, Map.position.x - xPos, transitionTime));
    // }

    // public float GetMapPositionPercentage(int posIndex)
    // {
    //     float num = (float)posIndex / ((float)cameraLocations.Count - 1);
    //     return num;
    // }

    // public float GetMapPositionPercentage(float posX)
    // {
    //     posX *= -1;
    //     float num = Mathf.InverseLerp(mapMinX, mapMaxX, posX);
    //     //print ("xPos: " + posX + ", mapMinX: " + mapMinX + ", mapMaxX: " + mapMaxX + ", PERCENT: " + num);
    //     return num;
    // }

    // private void SetMapPosition(float percent)
    // {
    //     if (percent >= 0f && percent <= 1f)
    //     {
    //         float tempX = Mathf.Lerp(mapMinX, mapMaxX, percent) * -1;
    //         //print ("percent: " + percent + ", pos: " + tempX);
    //         Map.position = new Vector3(tempX, Map.position.y, Map.position.z);
    //     }
    // }

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
            mapIcon.GetComponent<MapIcon>().SetFixed(opt);
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
