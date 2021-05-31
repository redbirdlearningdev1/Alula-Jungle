using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IslandCutoutController : MonoBehaviour
{
    public static IslandCutoutController instance;

    private bool holdingIsland;
    private bool isOn = true;

    public Transform originalPos;
    public Transform oceanPos;
    public float moveSpeed;
    public SpriteRenderer outline;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // return if off
        if (!isOn)
            return;

        if (Input.GetMouseButton(0) && holdingIsland)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
 
            transform.position = mousePos;
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
                    if (result.gameObject.transform.name == "IslandButton")
                    {
                        holdingIsland = true;
                    }
                }
            }
        }
    }

    private IEnumerator MoveIslandToOcean()
    {
        // move island to correct spot + set new parent
        GoToOceanSpot();
        transform.SetParent(oceanPos.transform);

        // turn off island update
        isOn = false;

        // remove island outline
        StartCoroutine(LerpOutlineAlpha());

        yield return new WaitForSeconds(1f);

        // center boat to face main island
        ParallaxController.instance.LerpToCenter();

        // disable wheel control
        BoatWheelController.instance.isOn = false;

        yield return new WaitForSeconds(1f);

        // enable throtle control
        BoatThrottleController.instance.isOn = true;

        // switch to vertical parallaxing
        ParallaxController.instance.verticalParallax = true;
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
            if (timer > 1f)
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
}
