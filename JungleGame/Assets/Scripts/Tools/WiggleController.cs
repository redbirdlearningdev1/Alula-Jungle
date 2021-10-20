using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleController : MonoBehaviour
{
    public AnimationCurve curve;
    public float multiplier;
    private bool wiggle;
    private float randomTimeAddition = 0f;
    private float timer = 0f;

    void Update()
    {
        if (wiggle)
        {
            timer += Time.deltaTime;
            var quat = Quaternion.Euler(0f, 0f, curve.Evaluate(timer + randomTimeAddition) * multiplier);
            transform.rotation = quat;
            
            // reset timer
            if (timer >= curve.length)
            {
                timer = 0f;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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
        timer = 0f;
    }
}
