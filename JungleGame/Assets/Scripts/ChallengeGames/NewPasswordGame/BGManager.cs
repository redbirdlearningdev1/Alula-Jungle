using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGManager : MonoBehaviour
{
    public static BGManager instance;

    public float moveSpeed;
    public float moveAmount;

    private int currBG = 0;
    private bool firstTime = true;
    private bool movingBG = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void MoveToNextSection()
    {
        if (movingBG)
            return;

        movingBG = true;

        // move bgs
        if (firstTime)
        {   
            firstTime = false;
            StartCoroutine(MoveBGs(800));
        }
        else
            StartCoroutine(MoveBGs(moveAmount));
    }

    private IEnumerator MoveBGs(float moveAmount)
    {
        float deltaMove = 0;

        while (true)
        {
            foreach(Transform t in this.transform)
            {
                Vector3 pos = t.localPosition;
                pos.x -= moveSpeed;
                t.localPosition = pos;
            }

            // stop moving once move amount is complete
            deltaMove += moveSpeed;
            if (deltaMove >= moveAmount)
            {
                break;
            }

            yield return null;
        }

        movingBG = false; 
    }
}
