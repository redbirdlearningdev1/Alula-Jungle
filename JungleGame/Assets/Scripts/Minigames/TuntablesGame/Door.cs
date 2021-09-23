using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Door : MonoBehaviour
{
    public bool isCenterDoor;
    [SerializeField] private ActionWordEnum currentIcon;
    public Image image;

    [Range(0.0f, 360.0f)]
    public float doorAngle;
    public const float defaultTurnDuration = 2f;

    [Header("Shake Variables")]
    public float shakeSpeed; // how fast it shakes
    public float shakeAmount; // how much it shakes
    public float shakeDuration; // how long shake lasts

    public void SetDoorIcon(ActionWordEnum icon)
    {
        currentIcon = icon;

        if (!isCenterDoor)
            image.sprite = GameManager.instance.GetActionWord(currentIcon).doorIcon;
        else
            image.sprite = GameManager.instance.GetActionWord(currentIcon).centerIcon;
    }

    public void ShakeIconSwitch(ActionWordEnum icon)
    {
        StartCoroutine(ShakeIconSwitchRoutine(icon, shakeDuration));
    }

    private IEnumerator ShakeIconSwitchRoutine(ActionWordEnum icon, float duration)
    {
        bool switchedIcon = false;
        float timer = 0f;
        Vector3 originalPos = image.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration / 2 && !switchedIcon)
            {
                SetDoorIcon(icon);
                switchedIcon = true;
            }
            else if (timer > duration)
            {
                image.transform.position = originalPos;
                break;
            }

            Vector3 pos = originalPos;
            pos.x = originalPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            image.transform.position = pos;
            yield return null;
        }
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
