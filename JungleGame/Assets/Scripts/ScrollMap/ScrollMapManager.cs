﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMapManager : MonoBehaviour
{
    public static ScrollMapManager instance;

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
    private bool navButtonsDisabled = true;
    
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;

    [Header("Map Characters")]
    public MapIcon boat;
    public MapCharacter Boat_Gorilla;
    public MapCharacter GV_Gorilla;

    [Header("Stickers")]
    [SerializeField] private GameObject stickerCart;
    [SerializeField] private Image cartRaycastBlocker;
    [SerializeField] private GameObject Book,Board,BackWindow,Gecko;
    [SerializeField] private Animator Wagon;
    [SerializeField] private Animator GeckoAnim;
    private bool stickerCartOut;
    private bool cartBusy;
    private bool stickerButtonsDisabled;
    public float stickerTransitionTime;

    public Transform cartStartPosition;
    public Transform cartOnScreenPosition;
    public Transform cartOffScreenPosition;

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

        // sticker stuff
        // disable raycast blocker
        cartRaycastBlocker.color = new Color(0f, 0f, 0f, 0f);
        cartRaycastBlocker.raycastTarget = false;

        Book.SetActive(false);
        Board.SetActive(false);
        BackWindow.SetActive(false);
        stickerCart.SetActive(false);

        // disable UI
        leftButton.interactable = false;
        rightButton.interactable = false;
        navButtonsDisabled = true;
        Boat_Gorilla.gameObject.SetActive(false);

        // start at prev position
        mapPosIndex = GameManager.instance.prevMapPosition;
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
        
        bool revealNavUI = true;

        // check for game events
        if (playGameEvent == LinearGameEvent.InitBoatGame)
        {   
            revealNavUI = false;
            DisableAllMapIcons();

            // wiggle boat
            boat.interactable = true;
            boat.GetComponent<SpriteWiggleController>().StartWiggle();
        }
        else if (playGameEvent == LinearGameEvent.WelcomeStoryGame)
        {
            revealNavUI = false;
            DisableAllMapIcons();

            // reveal boat gorilla
            Boat_Gorilla.gameObject.SetActive(true);
            Boat_Gorilla.ShowExclamationMark(true);
            Boat_Gorilla.interactable = true;
        }
        else if (playGameEvent == LinearGameEvent.UnlockGorillaVillage)
        {
            revealNavUI = false;
            DisableAllMapIcons();
            StartCoroutine(UnlockMapArea(1));
            GV_Gorilla.ShowExclamationMark(true);
            GV_Gorilla.interactable = true;

            // update SIS
            if (!overideGameEvent)
            {
                StudentInfoSystem.AdvanceLinearGameEvent();
                StudentInfoSystem.SaveStudentPlayerData();
            }
        }
        else if (playGameEvent == LinearGameEvent.PrologueStoryGame)
        {
            DisableAllMapIcons();
            GV_Gorilla.ShowExclamationMark(true);
            GV_Gorilla.interactable = true;
        }
        else if (playGameEvent == LinearGameEvent.StickerTutorial)
        {
            
        }

        // show UI
        if (revealNavUI)
        {
            print ("reveal UI");
            yield return new WaitForSeconds(0.5f);
            TurnOffNavigationUI(false);
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
        GameManager.instance.prevMapPosition = mapPosIndex;
    }

    private IEnumerator UnlockMapArea(int mapIndex)
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

        RaycastBlockerController.instance.RemoveRaycastBlocker("UnlockMapArea");
    }

    private void TurnOffNavigationUI(bool opt, bool smooth = true)
    {
        // do nothing if already same as opt
        if (opt == navButtonsDisabled)
            return;

        // enable button
        if (!opt)
        {   
            leftButton.interactable = true;
            rightButton.interactable = true;

            if (smooth)
            {
                StartCoroutine(SmoothImageAlpha(leftButton.GetComponent<Image>(), 0f, 1f, 0.5f));
                StartCoroutine(SmoothImageAlpha(rightButton.GetComponent<Image>(), 0f, 1f, 0.5f));
            }
            else 
            {
                leftButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                rightButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
        // disable button
        else
        {
            leftButton.interactable = false;
            rightButton.interactable = false;

            if (smooth)
            {
                StartCoroutine(SmoothImageAlpha(leftButton.GetComponent<Image>(), 1f, 0f, 0.5f));
                StartCoroutine(SmoothImageAlpha(rightButton.GetComponent<Image>(), 1f, 0f, 0.5f));
            }
            else 
            {
                leftButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                rightButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            }   
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
    #   STICKER METHODS
    ################################################
    */

    private IEnumerator StickerInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickerButtonsDisabled = false;
    }

    public void ToggleCart()
    {
        if (cartBusy)
            return;

        if (!stickerCartOut)
            StartCoroutine(RollOnScreen());
        else
            StartCoroutine(RollOffScreen());

        stickerCartOut = !stickerCartOut;
        cartBusy = true;
    }

    private IEnumerator RollOnScreen()
    {
        print ("rolling on!");
        stickerCart.transform.position = cartStartPosition.position;
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // activate raycast blocker
        StartCoroutine(CartRaycastBlockerRoutine(true));
        cartRaycastBlocker.raycastTarget = true;

        stickerCart.SetActive(true);
        Wagon.Play("WagonRollIn");
        StartCoroutine(RollToTargetRoutine(cartOnScreenPosition.position));
        yield return new WaitForSeconds(3.05f);
        Wagon.Play("WagonStop");
        yield return new WaitForSeconds(1.15f);
        Wagon.Play("Idle");
        Book.SetActive(true);
        Board.SetActive(true);
        BackWindow.SetActive(true);
        Gecko.SetActive(true);
        GeckoAnim.Play("geckoIntro");

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cartBusy = false;
    }

    private IEnumerator RollOffScreen()
    {
        print ("rolling off!");
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // deactivate raycast blocker
        StartCoroutine(CartRaycastBlockerRoutine(false));

        // roll off screen
        stickerCart.SetActive(true);
        Wagon.Play("WagonRollIn");
        StartCoroutine(RollToTargetRoutine(cartOffScreenPosition.position));
        yield return new WaitForSeconds(3f);
        // return cart to start pos
        stickerCart.transform.position = cartStartPosition.position;
        Wagon.Play("Idle");

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cartRaycastBlocker.raycastTarget = false;
        cartBusy = false;
    }

    private IEnumerator RollToTargetRoutine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = stickerCart.transform.position;
        float timer = 0f;
        float maxTime = .75f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            if (timer < maxTime)
            {
                stickerCart.transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                stickerCart.transform.position = target;
                yield break;
            }
            
            yield return null;
        }
    }

    private IEnumerator CartRaycastBlockerRoutine(bool opt)
    {
        float start, end;
        if (opt)
        {
            start = 0f;
            end = 0.75f;
        }
        else
        {
            start = 0.75f;
            end = 0f;
        }
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                cartRaycastBlocker.color = new Color(0f, 0f, 0f, end);
                break;
            }

            float tempAlpha = Mathf.Lerp(start, end, timer / 1f);
            cartRaycastBlocker.color = new Color(0f, 0f, 0f, tempAlpha);
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
