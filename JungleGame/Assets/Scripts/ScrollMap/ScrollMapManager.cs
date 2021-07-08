 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMapManager : MonoBehaviour
{
    [Header("Dev Stuff")]
    public bool overideMapLimit;
    [Range(0, 8)] public int mapLimitNum;

    public bool overideGameEvent;
    public LinearGameEvent gameEvent;

    private List<GameObject> mapIcons = new List<GameObject>();

    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private GameObject[] mapLocations; // the images that make up the map
    [SerializeField] private List<Transform> cameraLocations; // the positions where the camera stops at
    [SerializeField] private List<float> fogLocations; // the positions where the fog is placed
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Animations")]
    public float staticMapYPos;

    private int mapLimit;
    private int mapPosIndex;
    private bool navButtonsDisabled;
    
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;

    [Header("Map Characters")]
    public MapCharacter GV_Gorilla;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // play test song
        AudioManager.instance.PlaySong(AudioDatabase.instance.MainThemeSong);
    }

    void Start()
    {
        StartCoroutine(DelayedStart(0.1f));
    }

    private IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        // start at pos index 0
        mapPosIndex = 0;
        SetMapPosition(mapPosIndex);

        // map limit
        if (overideMapLimit)
            SetMapLimit(mapLimitNum); // set manual limit
        else
            SetMapLimit(StudentInfoSystem.currentStudentPlayer.mapLimit); // load map limit from SIS

        if (StudentInfoSystem.currentStudentPlayer != null)
        {
            // load in map data from profile
            MapDataLoader.instance.LoadMapData(StudentInfoSystem.currentStudentPlayer.mapData);
        }

        // check for game events
        LinearGameEvent playGameEvent = LinearGameEvent.InitBoatGame; // default event
        if (overideGameEvent)
        {
            playGameEvent = gameEvent;
        }
        else
        {
            // get event from current profile if not null
            if (StudentInfoSystem.currentStudentPlayer != null)
                playGameEvent = StudentInfoSystem.currentStudentPlayer.currGameEvent;
        }


        // check for game events
        if (playGameEvent == LinearGameEvent.UnlockGorillaVillage)
        {
            DisableAllMapIcons();
            StartCoroutine(UnlockMapArea(1));
            GV_Gorilla.ShowExclamationMark(true);
            GV_Gorilla.interactable = true;
        }
        else if (playGameEvent == LinearGameEvent.WelcomeStoryGame)
        {
            DisableAllMapIcons();
            GV_Gorilla.ShowExclamationMark(true);
            GV_Gorilla.interactable = true;
        }
        else if (playGameEvent == LinearGameEvent.StickerTutorial)
        {
            
        }




        // save progress to profile
        if (!overideGameEvent)
        {
            StudentInfoSystem.currentStudentPlayer.currGameEvent = (LinearGameEvent)((int)LinearGameEvent.UnlockGorillaVillage + 1);
            StudentInfoSystem.SaveStudentPlayerData();
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

    private IEnumerator UnlockMapArea(int mapIndex)
    {
        RaycastBlockerController.instance.CreateRaycastBlocker("UnlockMapArea");

        yield return new WaitForSeconds(1f);

        // remove nav UI and show Letterbox view
        TurnOffNavigationUI(true);
        LetterboxController.instance.ToggleLetterbox(true);

        yield return new WaitForSeconds(1f);

        mapLimit = mapIndex;
        // move map to next right map location
        float x = GetXPosFromMapLocationIndex(mapIndex);
        StartCoroutine(MapSmoothTransition(Map.localPosition.x, x, 2f));

        yield return new WaitForSeconds(2.5f);

        // move fog out of the way
        FogController.instance.MoveFogAnimation(fogLocations[mapIndex], 3f);

        LetterboxController.instance.ShowTextSmooth("1 - Gorilla Village");

        yield return new WaitForSeconds(2f);

        // move letterbox out of the way
        LetterboxController.instance.ToggleLetterbox(false);

        yield return new WaitForSeconds(2f);

        // show UI again
        TurnOffNavigationUI(false);

        // save unlock to sis profile
        StudentInfoSystem.currentStudentPlayer.mapLimit = mapIndex;
        StudentInfoSystem.SaveStudentPlayerData();

        RaycastBlockerController.instance.RemoveRaycastBlocker("UnlockMapArea");
    }

    private void TurnOffNavigationUI(bool opt)
    {
        // do nothing if already same as opt
        if (opt == navButtonsDisabled)
            return;

        // enable button
        if (!opt)
        {   
            leftButton.interactable = true;
            rightButton.interactable = true;

            StartCoroutine(SmoothImageAlpha(leftButton.GetComponent<Image>(), 0f, 1f, 0.5f));
            StartCoroutine(SmoothImageAlpha(rightButton.GetComponent<Image>(), 0f, 1f, 0.5f));
        }
        // disable button
        else
        {
            leftButton.interactable = false;
            rightButton.interactable = false;

            StartCoroutine(SmoothImageAlpha(leftButton.GetComponent<Image>(), 1f, 0f, 0.5f));
            StartCoroutine(SmoothImageAlpha(rightButton.GetComponent<Image>(), 1f, 0f, 0.5f));
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

        mapPosIndex--;
        if (mapPosIndex < 0)
        {
            print ("left bump!");
            mapPosIndex = 0;
            StartCoroutine(BumpAnimation(true));
            return;
        }

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
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, GetXPosFromMapLocationIndex(0), (bumpAnimationTime / 2)));
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
