using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMapManager : MonoBehaviour
{
    [Header("Map Navigation")]
    [SerializeField] private RectTransform Map;
    [SerializeField] private GameObject[] mapLocations;
    [SerializeField] private float mapMinX;
    [SerializeField] private float mapMaxX;
    private int mapPosIndex;
    public float transitionTime;

    [Header("Birb")]
    [SerializeField] private GameObject birb;
    [SerializeField] private Transform leftBirbBounds;
    [SerializeField] private Transform rightBirbBounds;
    public float birbIdleMoveAmount;
    private bool moveBirb;
    

    void Awake()
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();

        mapPosIndex = 0; // start at pos index 0
        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.transform.position.x, Map.transform.position.x - x, transitionTime));

        mapMinX = 0;
        mapMaxX = 0;
        int i = 0;
        foreach(GameObject location in mapLocations)
        {
            RectTransform rt = (RectTransform)mapLocations[i].transform;
            mapMaxX += rt.rect.width;
            i++;
        }
    }

    void Update()
    {
        if (moveBirb)
        {
            float xPos = 0;
            float mousePos = Input.mousePosition.x;

            if (mousePos > rightBirbBounds.position.x)
                xPos = rightBirbBounds.position.x;
            else if (mousePos < leftBirbBounds.position.x)
                xPos = leftBirbBounds.position.x;
            else 
                xPos = mousePos;

            /*  
            if (xPos < prevBirbPos + birbThresh && xPos > prevBirbPos - birbThresh)
                StartCoroutine(SetBirbSpriteDelay(birbPressed, 0.2f));
            else
                StartCoroutine(SetBirbSpriteDelay(birbMove, 0.2f));
            prevBirbPos = xPos;
            */

            birb.transform.position = new Vector3(xPos, birb.transform.position.y, birb.transform.position.z); // move birb
            float birbPercent = Mathf.InverseLerp(leftBirbBounds.position.x, rightBirbBounds.position.x, birb.transform.position.x);
            SetMapPosition(birbPercent); // set the position of the map
        }
        else
        {
            float xPos = Mathf.Lerp(leftBirbBounds.position.x, rightBirbBounds.position.x, GetMapPositionPercentage());

            if (Mathf.Abs(birb.transform.position.x - xPos) > birbIdleMoveAmount)
            {
                if (xPos > birb.transform.position.x)
                {
                    birb.transform.position = new Vector3(birb.transform.position.x + birbIdleMoveAmount, birb.transform.position.y, birb.transform.position.z);
                }
                else
                {
                    birb.transform.position = new Vector3(birb.transform.position.x - birbIdleMoveAmount, birb.transform.position.y, birb.transform.position.z);
                }
            }
            

            /*
            if (birbImage.sprite != birbNorm)
                StartCoroutine(SetBirbSpriteDelay(birbNorm, 0.2f));
            */
        }
    }

    /* 
    ################################################
    #   MAP NAVIGATION 
    ################################################
    */

    public void OnGoLeftPressed()
    {
        mapPosIndex--;
        if (mapPosIndex < 0)
            mapPosIndex = 0;

        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.transform.position.x, Map.transform.position.x - x, transitionTime));
    }

    public void OnGoRightPressed()
    {
        mapPosIndex++;
        if (mapPosIndex > mapLocations.Length - 1)
            mapPosIndex = mapLocations.Length - 1;

        float x = GetXPosFromMapLocationIndex(mapPosIndex);
        StartCoroutine(MapSmoothTransition(Map.transform.position.x, Map.transform.position.x - x, transitionTime));
    }

    private void GoToNearestMapLocation()
    {
        float minX = float.MaxValue;
        int i = 0;
        foreach(GameObject location in mapLocations)
        {
            if (Mathf.Abs(location.transform.position.x) < Mathf.Abs(minX))
            {
                minX = GetXPosFromMapLocationIndex(i);
                mapPosIndex = i;
            }
            i++;
        }
        
        StartCoroutine(MapSmoothTransition(Map.transform.position.x, Map.transform.position.x - minX, transitionTime));
    }

    public float GetMapPositionPercentage()
    {
        float num = (float)mapPosIndex / ((float)mapLocations.Length - 1);
        return num;
    }

    private void SetMapPosition(float percent)
    {
        if (percent >= 0f && percent <= 1f)
        {
            float tempX = Mathf.Lerp(mapMinX, mapMaxX, percent) * -1;
            Map.transform.localPosition = new Vector3(tempX, Map.transform.localPosition.y, Map.transform.localPosition.z);
        }
    }

    private float GetXPosFromMapLocationIndex(int index)
    {
        RectTransform rt = (RectTransform)mapLocations[index].transform;
        print ("index: " + index + ", pos: " + mapLocations[index].transform.position.x + ", width: " + rt.rect.width);
        return mapLocations[index].transform.position.x;
    }

    private IEnumerator MapSmoothTransition(float start, float end, float transitionTime)
    {
        GameHelper.SetRaycastBlocker(true);
        float timer = 0f;
        while (timer <= 1.0)
        {
            timer += Time.deltaTime / transitionTime;
            float pos = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer));
            Map.position = new Vector3(pos, 0f, 0f);
            yield return null;
        }
        GameHelper.SetRaycastBlocker(false);
    }

    /* 
    ################################################
    #   UI BUTTONS 
    ################################################
    */

    public void OnSettingsButtonPressed()
    {
        GameHelper.LoadScene("SettingsScene", true);
    }

    public void OnTrophyRoomButtonPressed()
    {
        GameHelper.LoadScene("TrophyRoomScene", true);
    }
    
    /* 
    ################################################
    #   BIRB BUTTONS 
    ################################################
    */

    public void BirbButtonDown()
    {
        moveBirb = true;
        //StartCoroutine(SetBirbSpriteDelay(birbPressed, 0.2f));
    }

    public void BirbButtonUp()
    {   
        moveBirb = false;
        GoToNearestMapLocation();
        //StartCoroutine(SetBirbSpriteDelay(birbNorm, 0.2f));
    }
}
