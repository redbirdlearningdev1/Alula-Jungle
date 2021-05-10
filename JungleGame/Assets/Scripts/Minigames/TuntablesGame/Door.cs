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

    public void RotateToAngle(float newAngle)
    {
        StartCoroutine(RotateToAngleRoutine(newAngle));
    }

    private IEnumerator RotateToAngleRoutine(float newAngle)
    {
        print ("rotating!");
        Quaternion end = Quaternion.Euler(0f, 0f, newAngle);
        Quaternion start = Quaternion.Euler(0f, 0f, doorAngle);

        while (true)
        {
            if (transform.rotation == end)
            {
                doorAngle = newAngle;
                break;
            }

            transform.rotation = Quaternion.Lerp(start, end, Time.deltaTime * rotationalSpeed);
            yield return null;
        }

        print ("done!");
    }
}
