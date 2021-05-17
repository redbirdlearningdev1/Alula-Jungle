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
    public const float defaultTurnDuration = 2f;

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

    public void RotateToAngle(float newAngle, bool clockwise, float duration = defaultTurnDuration)
    {
        StartCoroutine(RotateToAngleRoutine(newAngle, clockwise, duration));
    }

    private IEnumerator RotateToAngleRoutine(float newAngle, bool clockwise, float duration)
    {
        int directionMultiplier = clockwise ? 1 : -1;

        float start = doorAngle;
        float end = newAngle;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                doorAngle = end;
                SetDoorAngle();
                break;
            }

            doorAngle = Mathf.LerpAngle(start, end, timer / duration);
            SetDoorAngle();

            yield return null;
        }
    }
}
