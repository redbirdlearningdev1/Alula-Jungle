using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatParallaxDirection
{
    Still, Left, Right
}

[ExecuteInEditMode]
public class NewParallaxController : MonoBehaviour
{
    public static NewParallaxController instance;

    public BoatParallaxDirection direction;
    public float spacing;
    [Range(0, 10)] public float speedMultiplier = 1f;

    [Header("Sky Stuff")]
    public Transform sky1;
    public Transform sky2;
    public Transform skyMid1;
    public Transform skyMid2;
    public float skySpeed;

    [Header("Back Island Stuff")]
    public Transform bIsland1;
    public Transform bIsland2;
    public Transform bIslandMid1;
    public Transform bIslandMid2;
    public float bIslandSpeed;

    [Header("Ocean Stuff")]
    public Transform ocean1;
    public Transform ocean2;
    public Transform oceanMid1;
    public Transform oceanMid2;
    public float oceanSpeed;

    [Header("Front Island Stuff")]
    public Transform fIsland1;
    public Transform fIsland2;
    public Transform fIslandMid1;
    public Transform fIslandMid2;
    public float fIslandSpeed;

    [Header("Vertical Paralax")]
    [HideInInspector] public bool verticalParallax = false;
    [Range(0,1)] public float verticalParallaxPos = 0f;
    public float vertParallaxSpeed;
    public Transform islandPosition;

    private float prevVerticalParallaxPos;
    private bool startVerticalParallax = false;
    [HideInInspector] public bool centeringOnIsland = false;
    
    [Header("Scale Multipliers")]
    public float skyScale;
    public float backIslandScale;
    public float oceanScale;
    public float frontIslandScale;
    public float mainIslandScale;

    [Header("End Positions")]
    public Transform oceanEndPos;
    public Transform backIslandEndPos;
    public Transform frontIslandEndPos;
    public Transform mainIslandEndPos;

    private Vector3 oceanStartPos;
    private Vector3 backIslandStartPos;
    private Vector3 frontIslandStartPos;
    private Vector3 mainIslandStartPos;

    [Header("Containers")]
    public GameObject skyContainer;
    public GameObject oceanContainer;
    public GameObject backIslandContainer;
    public GameObject frontIslandContainer;
    public GameObject mainIslandContainer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // move background objects
        if (direction == BoatParallaxDirection.Left)
        {
            CalculateObjectParallaxLeft(sky1, sky2, skyMid1, skyMid2, skySpeed);
            CalculateObjectParallaxLeft(bIsland1, bIsland2, bIslandMid1, bIslandMid2, bIslandSpeed);
            CalculateObjectParallaxLeft(ocean1, ocean2, oceanMid1, oceanMid2, oceanSpeed);
            CalculateObjectParallaxLeft(fIsland1, fIsland2, fIslandMid1, fIslandMid2, fIslandSpeed);
        }
        else if (direction == BoatParallaxDirection.Right)
        {
            CalculateObjectParallaxRight(sky1, sky2, skyMid1, skyMid2, skySpeed);
            CalculateObjectParallaxRight(bIsland1, bIsland2, bIslandMid1, bIslandMid2, bIslandSpeed);
            CalculateObjectParallaxRight(ocean1, ocean2, oceanMid1, oceanMid2, oceanSpeed);
            CalculateObjectParallaxRight(fIsland1, fIsland2, fIslandMid1, fIslandMid2, fIslandSpeed);
        }

        // VERTICAL PARALLAX
        if (verticalParallax)
        {
            if (!startVerticalParallax)
            {
                startVerticalParallax = true;
                oceanStartPos = oceanContainer.transform.position;
                backIslandStartPos = backIslandContainer.transform.position;
                frontIslandStartPos = frontIslandContainer.transform.position;
                mainIslandStartPos = mainIslandContainer.transform.position;
            }

            // get throttle speed
            float num = BoatThrottleController.instance.GetThrottleSpeed();
            verticalParallaxPos += num * vertParallaxSpeed * Time.deltaTime;

            // return if the prev parallax pos is the same
            if (verticalParallaxPos != prevVerticalParallaxPos)
            {
                prevVerticalParallaxPos = verticalParallaxPos;

                // arrived on island
                if (verticalParallaxPos > 1f)
                {
                    verticalParallax = false;
                    NewBoatGameManager.instance.ArrivedAtIsland();
                }
            }
            else
                return;
            
            // set sky scale
            float sScale = Mathf.Lerp(1f, skyScale, verticalParallaxPos);
            skyContainer.transform.localScale = new Vector3(sScale, sScale, 1f);

            // set ocean pos and scale
            float oScale = Mathf.Lerp(1f, oceanScale, verticalParallaxPos);
            Vector3 oPos = Vector3.Lerp(oceanStartPos, oceanEndPos.position, verticalParallaxPos);
            oceanContainer.transform.localScale = new Vector3(oScale, oScale, 1f);
            oceanContainer.transform.position = oPos;

            // set back island scale
            float iScale = Mathf.Lerp(1f, backIslandScale, verticalParallaxPos);
            Vector3 iPos = Vector3.Lerp(backIslandStartPos, backIslandEndPos.position, verticalParallaxPos);
            backIslandContainer.transform.localScale = new Vector3(iScale, iScale, 1f);
            backIslandContainer.transform.position = iPos;

            // set front island pos and scale
            float fScale = Mathf.Lerp(1f, frontIslandScale, verticalParallaxPos);
            Vector3 fPos = Vector3.Lerp(frontIslandStartPos, frontIslandEndPos.position, verticalParallaxPos);
            frontIslandContainer.transform.localScale = new Vector3(fScale, fScale, 1f);
            frontIslandContainer.transform.position = fPos;

            // set main island pos and scale
            float mainScale = Mathf.Lerp(1f, mainIslandScale, verticalParallaxPos);
            Vector3 mainPos = Vector3.Lerp(mainIslandStartPos, mainIslandEndPos.position, verticalParallaxPos);
            mainIslandContainer.transform.localScale = new Vector3(mainScale, mainScale, 1f);
            mainIslandContainer.transform.position = mainPos;
        }
    }

    public void SetBoatDirection(BoatParallaxDirection newDir)
    {
        direction = newDir;
    }

    private void CalculateObjectParallaxLeft(Transform obj1, Transform obj2, Transform mid1, Transform mid2, float speed)
    {
        obj1.position = new Vector3(obj1.position.x - speed * speedMultiplier * Time.deltaTime, obj1.position.y, 0f);
        obj2.position = new Vector3(obj2.position.x - speed * speedMultiplier * Time.deltaTime, obj2.position.y, 0f);

        // check if both middles are to the left of x = 0
        if (mid1.position.x < 0 && mid2.position.x < 0)
        {
            // move the leftmost object to the right of the other
            if (mid1.position.x < mid2.position.x)
            {
                // move obj 1 to the right of obj 2
                float newPos = obj2.localPosition.x + obj2.GetComponent<RectTransform>().rect.width - spacing;
                obj1.transform.localPosition = new Vector3(newPos, obj1.transform.localPosition.y, 0f);
            }
            else
            {
                // move obj 2 to the right of obj 1
                float newPos = obj1.localPosition.x + obj1.GetComponent<RectTransform>().rect.width - spacing;
                obj2.transform.localPosition = new Vector3(newPos, obj2.transform.localPosition.y, 0f);
            }
        }
    }

    private void CalculateObjectParallaxRight(Transform obj1, Transform obj2, Transform mid1, Transform mid2, float speed)
    {
        obj1.position = new Vector3(obj1.position.x + speed * speedMultiplier * Time.deltaTime, obj1.position.y, 0f);
        obj2.position = new Vector3(obj2.position.x + speed * speedMultiplier * Time.deltaTime, obj2.position.y, 0f);

        // check if both middles are to the right of x = 0
        if (mid1.position.x > 0 && mid2.position.x > 0)
        {
            // move the rightmost object to the left of the other
            if (mid1.position.x > mid2.position.x)
            {
                // move obj 1 to the right of obj 2
                float newPos = obj2.localPosition.x - obj2.GetComponent<RectTransform>().rect.width + spacing;
                obj1.transform.localPosition = new Vector3(newPos, obj1.transform.localPosition.y, 0f);
            }
            else
            {
                // move obj 2 to the right of obj 1
                float newPos = obj1.localPosition.x - obj1.GetComponent<RectTransform>().rect.width + spacing;
                obj2.transform.localPosition = new Vector3(newPos, obj2.transform.localPosition.y, 0f);
            }
        }
    }

    public void CenterOnIsland(Transform mainIsland)
    {   
        StartCoroutine(CenterOnIslandRoutine());
    }

    private IEnumerator CenterOnIslandRoutine()
    {
        centeringOnIsland = true;

        // play boat move sound effect
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.BoatMoveRumble, 0.25f, "boat_move");

        while (islandPosition.position.x > 0)
        {
            direction = BoatParallaxDirection.Left;
            yield return null;
        }
        while (islandPosition.position.x < 0)
        {
            direction = BoatParallaxDirection.Right;
            yield return null;
        }
        direction = BoatParallaxDirection.Still;

        centeringOnIsland = false;

        // stop sound effect
        AudioManager.instance.StopFX("boat_move");

        // continue boat game event
        NewBoatGameManager.instance.IslandCentered();
    }
}
