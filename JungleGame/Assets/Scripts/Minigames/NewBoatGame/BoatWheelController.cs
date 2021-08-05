using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatWheelController : MonoBehaviour
{
    public static BoatWheelController instance;

    [Header("Boat Panel")]
    public Transform boatPannel;
    public float shakeSpeed;
    public float shakeAmount;

    [Header("Wheel Values")]
    public float leftAngle;
    public float rightAngle;
    public float moveDuration;

    private float wheelAngle;
    private Coroutine currentRoutine;
    [HideInInspector] public bool holdingWheel;
    private bool isLeft;

    public bool isOn = true;

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

        if (Input.GetMouseButton(0) && holdingWheel)
        {
            if (isLeft)
            {
                // parallax to the left
                NewParallaxController.instance.SetBoatDirection(BoatParallaxDirection.Left);
            }
            else
            {
                // parallax to the right
                NewParallaxController.instance.SetBoatDirection(BoatParallaxDirection.Right);
            }
        }
        else if (Input.GetMouseButtonUp(0) && holdingWheel)
        {
            holdingWheel = false;
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            ResetWheel();
            // set the boat direction to be none (still)
            NewParallaxController.instance.SetBoatDirection(BoatParallaxDirection.Still);
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
                        ToggleBoatPannelShake();
                    }
                    else if (result.gameObject.transform.name == "RightWheelButton")
                    {
                        holdingWheel = true;
                        RotateWheelRight();
                        ToggleBoatPannelShake();
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
        isLeft = false; // moving to the left
    }

    public void RotateWheelRight()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(RotateWheelRoutine(rightAngle, moveDuration));
        isLeft = true; // moving to the right
    }

    public void ResetWheel()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(RotateWheelRoutine(0f, moveDuration / 2));
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

    public void ToggleBoatPannelShake()
    {
        StartCoroutine(ShakeObjectRoutine(boatPannel));
    }

    private IEnumerator ShakeObjectRoutine(Transform obj)
    {
        Vector3 originalPos = obj.position;

        while (holdingWheel)
        {
            Vector3 pos = originalPos;
            pos.y = originalPos.y + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            obj.position = pos;
            yield return null;
        }

        obj.position = originalPos;
    }
}
