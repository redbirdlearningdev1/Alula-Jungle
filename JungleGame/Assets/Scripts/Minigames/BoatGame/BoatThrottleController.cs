using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatThrottleController : MonoBehaviour
{
    private bool holdingThrottle;
    private const float maxY = -1.7f;
    private const float minY = -4.2f;
    private const float posX = 4.3f;

    void Update()
    {
        if (Input.GetMouseButton(0) && holdingThrottle)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            mousePos.x = posX;

            if (mousePos.y > maxY)
                mousePos.y = maxY;
            if (mousePos.y < minY)
                mousePos.y = minY;
 
            transform.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && holdingThrottle)
        {
            holdingThrottle = false;
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
                    if (result.gameObject.transform.name == "ThrottleButton")
                    {
                        holdingThrottle = true;
                    }
                }
            }
        }
    }
}
