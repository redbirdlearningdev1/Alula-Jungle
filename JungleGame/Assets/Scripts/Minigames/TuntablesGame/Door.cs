using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Door : MonoBehaviour
{
    public bool isCenterDoor;
    [SerializeField] private ActionWordEnum currentIcon;
    [SerializeField] private Image iconImage;

    [Range(0.0f, 360.0f)]
    public float doorAngle;
    public float rotationalSpeed;

    public void SetDoorIcon(ActionWordEnum icon)
    {
        currentIcon = icon;

        if (!isCenterDoor)
            iconImage.sprite = GameManager.instance.GetActionWord(currentIcon).doorIcon;
        else
            iconImage.sprite = GameManager.instance.GetActionWord(currentIcon).centerIcon;
    } 

    void Update()
    {
        // set angle if in editor mode
        if (Application.isEditor)
            SetDoorAngle();
    }

    private void SetDoorAngle()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, doorAngle);
    }

    public void RotateToAngle(int newAngle, bool clockwise)
    {
        StartCoroutine(RotateToAngleRoutine(newAngle, clockwise));
    }

    private IEnumerator RotateToAngleRoutine(int newAngle, bool clockwise)
    {
        // return if angle value is invalid
        if (newAngle < 0f || newAngle > 360f)
            yield break;

        float temp = doorAngle;

        while (true)
        {
            float prev = temp;
            if (clockwise)
            { 
                temp += rotationalSpeed;
                if (prev <= newAngle && temp >= newAngle)
                {
                    // set the door's rotation
                    doorAngle = temp;
                    SetDoorAngle();
                    yield break;
                }
            }
            else
            {
                temp -= rotationalSpeed;
                if (prev >= newAngle && temp <= newAngle)
                {
                    // set the door's rotation
                    doorAngle = temp;
                    SetDoorAngle();
                    yield break;
                }
            }
            
            if (temp > 360f)
            {
                float overflow = temp - 360f;
                temp = overflow;
            }
            else if (temp < 0)
            {
                float overflow = Mathf.Abs(temp);
                temp = 360f - overflow;
            }

            // set the door's rotation
            doorAngle = temp;
            SetDoorAngle();

            yield return null;
        }
    }
}
