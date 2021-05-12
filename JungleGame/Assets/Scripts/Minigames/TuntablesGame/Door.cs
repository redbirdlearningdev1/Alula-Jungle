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
        if (Application.isEditor && !Application.isPlaying)
            SetDoorAngle();
    }

    private void SetDoorAngle()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, doorAngle);
    }

    public void RotateToAngle(float newAngle, bool clockwise)
    {
        StartCoroutine(RotateToAngleRoutine(newAngle, clockwise));
    }

    private IEnumerator RotateToAngleRoutine(float newAngle, bool clockwise)
    {
        int directionMultiplier = clockwise ? 1 : -1;
        
        while (true)
        {
            doorAngle += (rotationalSpeed * directionMultiplier);

            if (doorAngle >= 360)
                doorAngle -= 360;
            else if (doorAngle <= 0)
                doorAngle += 360;

            if (Mathf.Abs(doorAngle - newAngle) < rotationalSpeed * 2)
            {
                doorAngle = newAngle;
                SetDoorAngle();
                break;
            }   
            SetDoorAngle();
            yield return null;
        }
    }
}
