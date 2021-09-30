using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockLock : MonoBehaviour
{
    public static RockLock instance;
    public Image image;

    [Header("Shake Variables")]
    public float shakeSpeed; // how fast it shakes
    public float shakeAmount; // how much it shakes
    public float shakeDuration; // how long shake lasts

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    public void ShakeRock()
    {
        StartCoroutine(ShakeRockLockRoutine(shakeDuration));
    }

    private IEnumerator ShakeRockLockRoutine(float duration)
    {
        bool switchedIcon = false;
        float timer = 0f;
        Vector3 originalPos = image.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= duration / 2 && !switchedIcon)
            {
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
}
