using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleController : MonoBehaviour
{
    public AnimationCurve curve;
    public float multiplier;
    public float curveLength;

    private bool wiggle;
    private float randomTimeAddition = 0f;
    private float timer = 0f;
    private Vector3 startRotation;
    private bool setRotation = false;

    void Awake()
    {
        startRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        if (wiggle)
        {
            timer += Time.deltaTime;
            var quat = Quaternion.Euler(startRotation.x, startRotation.y, startRotation.z + curve.Evaluate(timer + randomTimeAddition) * multiplier);
            transform.rotation = quat;
            
            // reset timer
            if (timer >= curveLength)
            {
                timer = 0f;
            }
        }
        else
        {
            if (!setRotation)
            {
                transform.rotation = Quaternion.Euler(startRotation);
                setRotation = true;
            }
        }
    }

    public void StartWiggle(bool randomTime = false)
    {
        // only add random time if passed though the start function
        if (randomTime)
            randomTimeAddition = Random.Range(0.1f, 0.5f);
        else
            randomTimeAddition = 0f;

        // set start rotation and position
        startRotation = transform.rotation.eulerAngles;

        wiggle = true;
        setRotation = false;
    }

    public void StopWiggle()
    {
        wiggle = false;
        timer = 0f;
        // reset position and rotation
        transform.rotation = Quaternion.Euler(startRotation);
    }
}
