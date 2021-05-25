using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxController : MonoBehaviour
{
    public static ParallaxController instance;

    [Header("Horizontal Parallax")]
    public GameObject sky;
    public GameObject backIslands;
    public GameObject ocean;
    public GameObject frontIslands;

    public float skyMoveMult;
    public float backIslandsMoveMult;
    public float oceanMoveMult;
    public float frontIslandsMoveMult;

    [Range(-1,1)] public float horizontalParallaxPos = 0.0f; // 0 is center, -1 is far left, 1 is far right
    private float prevHorizontalParallaxPos;
    public float horizontalParallaxSpeed = 0.1f;
    private const float returnToCenterTime = 1f;

    [Header("Vertical Parallax")]
    public GameObject smallIsland;
    public GameObject mainIsland;

    [HideInInspector] public bool verticalParallax;
    [Range(0,1)] public float verticalParallaxPos = 0.0f;
    private float prevVerticalParallaxPos;

    public float smallIslandScale;
    public float mainIslandScale;
    public float oceanScale;
    public float skyScale;
    public float backIslandScale;
    public Transform smallIslandStart;
    public Transform smallIslandEnd;
    public Transform mainIslandStart;
    public Transform mainIslandEnd;
    public Transform oceanStart;
    public Transform oceanEnd;

    private List<GameObject> objects;
    private List<float> multipliers;

    void Awake()
    {
        if (instance == null)
            instance = this;

        CreateLists();
    }

    private void CreateLists()
    {
        // add parallax objects to list
        objects = new List<GameObject>();
        objects.Add(sky);
        objects.Add(backIslands);
        objects.Add(ocean);
        objects.Add(frontIslands);

        // add mults to list
        multipliers = new List<float>();
        multipliers.Add(skyMoveMult);
        multipliers.Add(backIslandsMoveMult);
        multipliers.Add(oceanMoveMult);
        multipliers.Add(frontIslandsMoveMult);
    }

    void Update()
    {
        // HORIZONTAL PARALLAX
        if (!verticalParallax)
        {
            // return if the prev parallax pos is the same
            if (horizontalParallaxPos != prevHorizontalParallaxPos)
                prevHorizontalParallaxPos = horizontalParallaxPos;
            else
                return;

            // create lists if they are null
            if (objects == null || multipliers == null)
                CreateLists();

            // parallaxing happens here
            for (int i = 0; i < objects.Count; i++)
            {
                Vector3 pos = new Vector3(multipliers[i] * horizontalParallaxPos, 0f, 0f);
                objects[i].transform.position = pos;
            }
        }
        // VERTICAL PARALLAX
        else
        {
            // get throttle speed
            float num = BoatThrottleController.instance.GetThrottleSpeed();
            verticalParallaxPos += num * 0.002f;

            // return if the prev parallax pos is the same
            if (verticalParallaxPos != prevVerticalParallaxPos)
                prevVerticalParallaxPos = verticalParallaxPos;
            else
                return;

            // set small island pos and scale
            float smallScale = Mathf.Lerp(1f, smallIslandScale, verticalParallaxPos);
            Vector3 smallPos = Vector3.Lerp(smallIslandStart.position, smallIslandEnd.position, verticalParallaxPos);
            smallIsland.transform.localScale = new Vector3(smallScale, smallScale, 1f);
            smallIsland.transform.position = smallPos;

            // set main island pos and scale
            float mainScale = Mathf.Lerp(1f, mainIslandScale, verticalParallaxPos);
            Vector3 mainPos = Vector3.Lerp(mainIslandStart.position, mainIslandEnd.position, verticalParallaxPos);
            mainIsland.transform.localScale = new Vector3(mainScale, mainScale, 1f);
            mainIsland.transform.position = mainPos;

            // set ocean pos and scale
            float oScale = Mathf.Lerp(1f, oceanScale, verticalParallaxPos);
            Vector3 oPos = Vector3.Lerp(oceanStart.position, oceanEnd.position, verticalParallaxPos);
            ocean.transform.localScale = new Vector3(oScale, oScale, 1f);
            ocean.transform.position = oPos;

            // set sky scale
            float sScale = Mathf.Lerp(1f, skyScale, verticalParallaxPos);
            sky.transform.localScale = new Vector3(sScale, sScale, 1f);

            // set back island scale
            float iScale = Mathf.Lerp(1f, backIslandScale, verticalParallaxPos);
            backIslands.transform.localScale = new Vector3(iScale, iScale, 1f);
        }
    }

    public void MoveParallax(bool right)
    {
        // make speed negative if going right -->
        var delta = horizontalParallaxSpeed;
        if (right) delta *= -1;

        var temp = horizontalParallaxPos += delta;

        // make sure value is capped bwtween -1 and 1
        if (temp > 1)
            temp = 1f;
        else if (temp < -1)
            temp = -1f;

        horizontalParallaxPos = temp;
    }

    public void LerpToCenter()
    {
        StartCoroutine(LerpToCenterRoutine());
    }

    private IEnumerator LerpToCenterRoutine()
    {
        float timer = 0f;
        float start = horizontalParallaxPos;
        float end = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > returnToCenterTime)
            {
                horizontalParallaxPos = 0f;
                break;
            }

            horizontalParallaxPos = Mathf.Lerp(start, end, timer / returnToCenterTime);
            yield return null;
        }
    }
}
