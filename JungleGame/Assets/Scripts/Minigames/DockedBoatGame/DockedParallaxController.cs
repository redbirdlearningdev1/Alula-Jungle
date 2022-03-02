using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DockedParallaxController : MonoBehaviour
{
    public static DockedParallaxController instance;

    public BoatParallaxDirection direction;
    public float spacing;
    [Range(0, 10)] public float speedMultiplier = 1f;

    [Header("Background")]
    public Transform backgroundBehind;
    public Transform backgroundFront;
    public Transform leftBound;
    public Transform rightBound;
    public float bgBehindSpeed;
    public float bgFrontSpeed;

    private bool isMaxLeft;
    private bool isMaxRight;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        // move background objects
        if (direction == BoatParallaxDirection.Left)
        {
            CalculateObjectParallaxLeft(backgroundFront, bgFrontSpeed);
            CalculateObjectParallaxLeft(backgroundBehind, bgBehindSpeed);
        }
        else if (direction == BoatParallaxDirection.Right)
        {
            CalculateObjectParallaxRight(backgroundFront, bgFrontSpeed);
            CalculateObjectParallaxRight(backgroundBehind, bgBehindSpeed);
        }
    }

    public void SetBoatDirection(BoatParallaxDirection newDir)
    {
        direction = newDir;
    }

    private void CalculateObjectParallaxLeft(Transform obj, float speed)
    {
        if (obj.position.x > leftBound.position.x && !isMaxLeft)
        {
            isMaxRight = false;
            obj.position = new Vector3(obj.position.x - speed * speedMultiplier * Time.deltaTime, obj.position.y, 0f);
        }
        else
        {
            isMaxLeft = true;
        }
    }

    private void CalculateObjectParallaxRight(Transform obj, float speed)
    {
        if (obj.position.x < rightBound.position.x && !isMaxRight)
        {
            isMaxLeft = false;
            obj.position = new Vector3(obj.position.x + speed * speedMultiplier * Time.deltaTime, obj.position.y, 0f); 
        }
        else
        {
            isMaxRight = true;
        }

    }
}
