using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatWheelController : MonoBehaviour
{
    public float leftAngle;
    public float rightAngle;
    public float moveDuration;

    private float wheelAngle;
    private Coroutine currentRoutine;
    private bool holdingWheel;

    void Update()
    {
        if (Input.GetMouseButton(0) && holdingWheel)
        {

        }
        else if (Input.GetMouseButtonUp(0) && holdingWheel)
        {
            holdingWheel = false;
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            ResetWheel();
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
                    if (result.gameObject.transform.name == "LeftWheelButton")
                    {
                        holdingWheel = true;
                        RotateWheelLeft();
                    }
                    else if (result.gameObject.transform.name == "RightWheelButton")
                    {
                        holdingWheel = true;
                        RotateWheelRight();
                    }
                }
            }
        }
    }

    public void RotateWheelLeft()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(RotateWheelRoutine(leftAngle, moveDuration));
    }

    public void RotateWheelRight()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(RotateWheelRoutine(rightAngle, moveDuration));
    }

    public void ResetWheel()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(RotateWheelRoutine(0f, moveDuration));
    }

    private IEnumerator RotateWheel(float angle)
    {
        yield return null;
    }

    private void SetWheelAngle()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, wheelAngle);
    }

    private IEnumerator RotateWheelRoutine(float newAngle, float duration)
    {
        float start = wheelAngle;
        float end = newAngle;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                wheelAngle = end;
                SetWheelAngle();
                break;
            }

            wheelAngle = Mathf.LerpAngle(start, end, timer / duration);
            SetWheelAngle();

            yield return null;
        }
    }
}
