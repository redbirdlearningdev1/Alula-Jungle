using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobController : MonoBehaviour
{
    public AnimationCurve curve;
    public float multiplier;
    private bool bob = false;
    private float timer = 0f;
    private Vector3 defaultPos;

    private bool setDefaultPos = false;

    void Update()
    {
        if (bob)
        {
            timer += Time.deltaTime;
            var tempPosY = defaultPos.y + curve.Evaluate(timer) * multiplier;
            transform.position = new Vector3(transform.position.x, tempPosY, transform.position.z);
            
            // reset timer
            if (timer >= 5f)
            {
                timer = 0f;
            }
        }
    }

    public void StartBob()
    {
        if (!setDefaultPos)
        {
            setDefaultPos = true;
            defaultPos = transform.position;
        }
        bob = true;
    }

    public void StopBob()
    {
        bob = false;
    }
}
