using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatThrottleController : MonoBehaviour
{
    public static BoatThrottleController instance;

    private bool holdingThrottle;
    private const float maxY = -1.7f;
    private const float minY = -4.2f;
    private const float posX = 4.3f;

    public bool isOn = false;
    public ThrottleButton throttleButton;

    void Awake()
    {
        if (instance == null)
            instance = this;

        Vector3 startPos = new Vector3(posX, minY, 0f);
        transform.position = startPos;
    }

    void Update()
    {
        // return if off
        if (!isOn)
            return;

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
            throttleButton.ToggleScalePressed(false);
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
                        throttleButton.ToggleScalePressed(true);
                    }
                }
            }
        }
    }

    public float GetThrottleSpeed()
    {
        return Mathf.InverseLerp(minY, maxY, transform.position.y);
    }
}
