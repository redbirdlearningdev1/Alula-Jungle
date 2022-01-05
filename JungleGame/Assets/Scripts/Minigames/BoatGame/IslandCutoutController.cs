using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IslandCutoutController : MonoBehaviour
{
    public static IslandCutoutController instance;

    public bool holdingIsland;
    public bool isOn = true;

    public Transform originalPos;
    public Transform oceanPos;
    public Transform mainIslandParent;
    public float moveSpeed;
    public Image outline;
    public Image islandCutout;
    public Image mainIsland;
    public WiggleController outlineWiggleController;
    public WiggleController cutoutWiggleController;

    // follow transform varibales
    private bool followTransform = false;
    private Transform transformToFollow;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // follow transform position (only x)
        if (followTransform)
        {
            mainIsland.transform.position = new Vector3(transformToFollow.position.x, mainIsland.transform.position.y, 1f);
            return;
        }

        // return if off
        if (!isOn)
            return;

        // drag n drop island :3
        if (Input.GetMouseButton(0) && holdingIsland)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(transform.position, mousePosWorldSpace, moveSpeed);
            transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && holdingIsland)
        {
            holdingIsland = false;

            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    print (result.gameObject.name);
                    if (result.gameObject.name == "IslandOutlineTarget")
                    {
                        isCorrect = true;
                        isOn = false;
                    }
                }
            }

            if (isCorrect)
            {
                StartCoroutine(MoveIslandToOcean());
            }
            else
            {
                // return island to boat panel
                ReturnIslandToPos();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    //print ("result: " + result.gameObject.name);

                    if (result.gameObject.transform.name == "IslandCutout")
                    {
                        holdingIsland = true;

                        // play sound effect
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
                    }
                }
            }
        }
    }

    private IEnumerator MoveIslandToOcean()
    {
        // turn off wheel control
        BoatWheelController.instance.isOn = false;
        BoatWheelController.instance.LetGoOfWheel();

        // stop wiggle
        cutoutWiggleController.StopWiggle();
        
        // move island to correct spot + set new parent
        GoToOceanSpot();

        yield return new WaitForSeconds(0.25f);
        // play sound effect
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PlacedIslandSplash, 1f);

        // remove island outline
        StartCoroutine(LerpOutlineAlpha());
        // make island cutout invisible
        islandCutout.GetComponent<LerpableObject>().LerpImageAlpha(islandCutout, 0f, 0.1f);
        // make main island visible
        mainIsland.GetComponent<LerpableObject>().LerpImageAlpha(mainIsland, 1f, 0.1f);
        yield return new WaitForSeconds(2f);

        // set island parent
        mainIsland.transform.SetParent(mainIslandParent);

        // center boat to face main island + center main island
        NewParallaxController.instance.CenterOnIsland(transform);
        FollowTransformPosition(outline.GetComponent<Transform>());
            
        // wait until finished centering on island
        while (NewParallaxController.instance.centeringOnIsland)
            yield return null;

        // stop following island outline
        StopFollowingTransform();

        // turn off island update
        isOn = false;

        // switch to vertical parallaxing
        NewParallaxController.instance.verticalParallax = true;
    }

    private void ReturnIslandToPos()
    {
        StartCoroutine(ReturnToPosRoutine(originalPos.position));
    }

    private void GoToOceanSpot()
    {
        StartCoroutine(ReturnToPosRoutine(oceanPos.position));
    }

    private IEnumerator ReturnToPosRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator LerpOutlineAlpha()
    {
        float timer = 0f;
        float start = 1f;
        float end = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 3f)
            {
                outline.color = new Color(1f, 1f, 1f, 0f);
                break;
            }

            float a = Mathf.Lerp(start, end, timer / 1f);
            Color color = new Color(1f, 1f, 1f, a);
            outline.color = color;
            yield return null;
        }
    }

    private void FollowTransformPosition(Transform objectToFollow)
    {
        transformToFollow = objectToFollow;
        followTransform = true;
    }

    private void StopFollowingTransform()
    {
        followTransform = false;
        transformToFollow = null;
    }
}
