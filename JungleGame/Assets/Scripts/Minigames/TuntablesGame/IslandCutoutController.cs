using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IslandCutoutController : MonoBehaviour
{
    private bool holdingIsland;
    public Transform originalPos;
    public float moveSpeed;

    void Update()
    {
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
                    if (result.gameObject.name == "IslandOutline")
                    {
                        isCorrect = true;
                    }
                }
            }

            if (isCorrect)
            {
                
            }
            else
            {
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

    private void ReturnIslandToPos()
    {
        StartCoroutine(ReturnToOriginalPosRoutine(originalPos.position));
    }

    private IEnumerator ReturnToOriginalPosRoutine(Vector3 target)
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
}
