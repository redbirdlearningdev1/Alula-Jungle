using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteWiggleController : MonoBehaviour
{
    public AnimationCurve curve;
    public float multiplier;
    private bool wiggle;
    private float randomTimeAddition = 0f;

    void Update()
    {
        if (wiggle)
        {
            var quat = Quaternion.Euler(0f, 0f, curve.Evaluate(Time.time + randomTimeAddition) * multiplier);
            transform.rotation = quat;
        }
    }

    public void StartWiggle(bool randomTime = false)
    {
        randomTimeAddition = Random.Range(0.1f, 0.5f);
        wiggle = true;
    }

    public void StopWiggle()
    {
        wiggle = false;
    }
}
