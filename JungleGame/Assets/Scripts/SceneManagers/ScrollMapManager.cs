 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMapManager : MonoBehaviour
{
    [Header("Dev Stuff")]
    public bool overideMapLimit;
    [Range(0, 8)] public int mapLimitNum;


    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private GameObject[] mapLocations; // the images that make up the map
    [SerializeField] private List<Transform> cameraLocations; // the positions where the camera stops at
    [SerializeField] private List<float> fogLocations; // the positions where the fog is placed

    public float staticMapYPos;
    private int mapLimit;
    private int mapPosIndex;
    private bool navButtonsDisabled;
    public float transitionTime;
    public float bumpAnimationTime;
    public float bumpAmount;

    [Header("Birb")]
    [SerializeField] private GameObject birb;
    [SerializeField] private Transform leftBirbBounds;
    [SerializeField] private Transform rightBirbBounds;
    private bool moveBirb;

    [Header("Sticker")]
    [SerializeField] public GameObject stickerCart;
    [SerializeField] public GameObject toolBar;
    [SerializeField] public GameObject Book,Board,BackWindow,Gecko;
    [SerializeField] public Animator Wagon;
    [SerializeField] public Animator GeckoAnim;
    private bool stickerButtonsDisabled;
    public float stickerTransitionTime;
    private Vector3 cartStartPosition;
    private Vector3 cartOnScreenPosition = new Vector3(0f, -1f, 0f);
    private Vector3 toolBarStartPosition;
    private Vector3 toolBarOnScreenPosition = new Vector3(0f, 0f, 0f);

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
        // cartStartPosition = stickerCart.transform.position;
        // toolBarStartPosition = toolBar.transform.position;
        // Book.SetActive(false);
        // Board.SetActive(false);
        // BackWindow.SetActive(false);
        // stickerCart.SetActive(false);
        // toolBar.SetActive(false); 
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

    }

    void Update()
    {
        // if (moveBirb)
        // {
        //     float xPos = 0;
        //     float mousePos = Input.mousePosition.x;

        //     if (mousePos > rightBirbBounds.position.x)
        //         xPos = rightBirbBounds.position.x;
        //     else if (mousePos < leftBirbBounds.position.x)
        //         xPos = leftBirbBounds.position.x;
        //     else 
        //         xPos = mousePos;

        //     /*  
        //     if (xPos < prevBirbPos + birbThresh && xPos > prevBirbPos - birbThresh)
        //         StartCoroutine(SetBirbSpriteDelay(birbPressed, 0.2f));
        //     else
        //         StartCoroutine(SetBirbSpriteDelay(birbMove, 0.2f));
        //     prevBirbPos = xPos;
        //     */

        //     birb.transform.position = new Vector3(xPos, birb.transform.position.y, birb.transform.position.z); // move birb
        //     float birbPercent = Mathf.InverseLerp(leftBirbBounds.position.x, rightBirbBounds.position.x, birb.transform.position.x);
        //     SetMapPosition(birbPercent); // set the position of the map
        // }
        // else
        // {
        //     float mapPos = GetMapPositionPercentage(Map.transform.position.x);
        //     float birbPos = Mathf.Lerp(leftBirbBounds.position.x, rightBirbBounds.position.x, mapPos);
        //     birb.transform.position = new Vector3(birbPos, birb.transform.position.y, birb.transform.position.z);
            

        //     /*
        //     if (birbImage.sprite != birbNorm)
        //         StartCoroutine(SetBirbSpriteDelay(birbNorm, 0.2f));
        //     */
        // }
    }

    /* 
    ################################################
    #   MAP NAVIGATION BUTTONS
    ################################################
    */

    

    private IEnumerator StickerInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickerButtonsDisabled = false;
    }

    public void DoRollOn()
    {
        StartCoroutine(RollOnScreen());
    }

    public IEnumerator RollOnScreen()
    {
        stickerCart.SetActive(true);
        toolBar.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Wagon.Play("WagonRollIn");
        StartCoroutine(RollOnScreenRoutine(cartOnScreenPosition, toolBarOnScreenPosition));
        yield return new WaitForSeconds(3.05f);
        Wagon.Play("WagonStop");
        yield return new WaitForSeconds(1.15f);
        Wagon.Play("Idle");
        Book.SetActive(true);
        Board.SetActive(true);
        BackWindow.SetActive(true);
        Gecko.SetActive(true);
        GeckoAnim.Play("geckoIntro");


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //yield return new WaitForSeconds(20f);
        //GeckoAnim.Play("geckoFall 0");
    }

    private IEnumerator RollOnScreenRoutine(Vector3 target, Vector3 target2)
    {
        Debug.Log("Here");
        Vector3 currStart = stickerCart.transform.position;
        Vector3 currStart2 = toolBar.transform.position;
        float timer = 0f;
        float maxTime = .75f;
        //animator.Play("orcWalk");
        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            if (timer < maxTime)
            {
                stickerCart.transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
                toolBar.transform.position = Vector3.Lerp(currStart2, target2, timer / maxTime);
            }
            else
            {
                stickerCart.transform.position = target;
                toolBar.transform.position = target2;
                yield break;
            }
            
            
            yield return null;
        }
    }


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
    #   BIRB BUTTONS 
    ################################################
    */

    // public void BirbButtonDown()
    // {
    //     moveBirb = true;
    //     birb.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
    //     //StartCoroutine(SetBirbSpriteDelay(birbPressed, 0.2f));
    // }

    // public void BirbButtonUp()
    // {   
    //     moveBirb = false;
    //     birb.transform.localScale = new Vector3(1f, 1f, 1f);
    //     GoToNearestMapLocation();
    //     //StartCoroutine(SetBirbSpriteDelay(birbNorm, 0.2f));
    // }

    /* 
    ################################################
    #   DEV FUNCTIONS 
    ################################################
    */

    private List<GameObject> mapIcons = new List<GameObject>();

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
