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
    public float bgBehindSpeed;
    public float bgFrontSpeed;

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
        Debug.Log("Object position: " + obj.position);
        if (true)
        {
            obj.position = new Vector3(obj.position.x - speed * speedMultiplier * Time.deltaTime, obj.position.y, 0f);
        }
    }

    private void CalculateObjectParallaxRight(Transform obj, float speed)
    {
        obj.position = new Vector3(obj.position.x + speed * speedMultiplier * Time.deltaTime, obj.position.y, 0f);

    }
}
