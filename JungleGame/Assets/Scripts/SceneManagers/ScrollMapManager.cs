using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMapManager : MonoBehaviour
{
    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map; // full map
    [SerializeField] private GameObject[] mapLocations; // the images that make up the map
    [SerializeField] private List<Transform> cameraLocations; // the positions where the camera stops at
    public float staticMapYPos;
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

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // play test song
        //AudioManager.instance.PlaySong(Song.JungleGameTestSong);
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

        // set birb in correct position
        // float xPos = Mathf.Lerp(leftBirbBounds.position.x, rightBirbBounds.position.x, GetMapPositionPercentage(mapPosIndex));
        // birb.transform.position = new Vector3(xPos, birb.transform.position.y, birb.transform.position.z);
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
        if (mapPosIndex > cameraLocations.Count - 1)
        {
            print ("right bump!");
            mapPosIndex = cameraLocations.Count - 1;
            StartCoroutine(BumpAnimation(false));
            return;
        }
        
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
        print("bump detected!");
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
            StartCoroutine(MapSmoothTransition(Map.localPosition.x, GetXPosFromMapLocationIndex(cameraLocations.Count - 1), (bumpAnimationTime / 2)));
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
        GameManager.instance.SetRaycastBlocker(true);
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

        GameManager.instance.SetRaycastBlocker(false);
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
