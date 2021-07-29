using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatParallaxDirection
{
    Still, Left, Right
}

[ExecuteInEditMode]
public class NewParallaxController : MonoBehaviour
{
    public BoatParallaxDirection direction;
    public float spacing;
    [Range(0, 10)] public float speedMultiplier = 1f;

    [Header("Sky Stuff")]
    public Transform sky1;
    public Transform sky2;
    public Transform skyMid1;
    public Transform skyMid2;
    public float skySpeed;

    [Header("Back Island Stuff")]
    public Transform bIsland1;
    public Transform bIsland2;
    public Transform bIslandMid1;
    public Transform bIslandMid2;
    public float bIslandSpeed;

    [Header("Ocean Stuff")]
    public Transform ocean1;
    public Transform ocean2;
    public Transform oceanMid1;
    public Transform oceanMid2;
    public float oceanSpeed;

    [Header("Front Island Stuff")]
    public Transform fIsland1;
    public Transform fIsland2;
    public Transform fIslandMid1;
    public Transform fIslandMid2;
    public float fIslandSpeed;

    void Update()
    {
        // move background objects
        if (direction == BoatParallaxDirection.Left)
        {
            CalculateObjectParallaxLeft(sky1, sky2, skyMid1, skyMid2, skySpeed);
            CalculateObjectParallaxLeft(bIsland1, bIsland2, bIslandMid1, bIslandMid2, bIslandSpeed);
            CalculateObjectParallaxLeft(ocean1, ocean2, oceanMid1, oceanMid2, oceanSpeed);
            CalculateObjectParallaxLeft(fIsland1, fIsland2, fIslandMid1, fIslandMid2, fIslandSpeed);
        }
        else if (direction == BoatParallaxDirection.Right)
        {
            CalculateObjectParallaxRight(sky1, sky2, skyMid1, skyMid2, skySpeed);
            CalculateObjectParallaxRight(bIsland1, bIsland2, bIslandMid1, bIslandMid2, bIslandSpeed);
            CalculateObjectParallaxRight(ocean1, ocean2, oceanMid1, oceanMid2, oceanSpeed);
            CalculateObjectParallaxRight(fIsland1, fIsland2, fIslandMid1, fIslandMid2, fIslandSpeed);
        }
    }

    private void CalculateObjectParallaxLeft(Transform obj1, Transform obj2, Transform mid1, Transform mid2, float speed)
    {
        obj1.position = new Vector3(obj1.position.x - speed * speedMultiplier * Time.deltaTime, 0f, 0f);
        obj2.position = new Vector3(obj2.position.x - speed * speedMultiplier * Time.deltaTime, 0f, 0f);

        // check if both middles are to the left of x = 0
        if (mid1.position.x < 0 && mid2.position.x < 0)
        {
            // move the leftmost object to the right of the other
            if (mid1.position.x < mid2.position.x)
            {
                // move obj 1 to the right of obj 2
                float newPos = obj2.localPosition.x + obj2.GetComponent<RectTransform>().rect.width - spacing;
                obj1.transform.localPosition = new Vector3(newPos, 0f, 0f);
            }
            else
            {
                // move obj 2 to the right of obj 1
                float newPos = obj1.localPosition.x + obj1.GetComponent<RectTransform>().rect.width - spacing;
                obj2.transform.localPosition = new Vector3(newPos, 0f, 0f);
            }
        }
    }

    private void CalculateObjectParallaxRight(Transform obj1, Transform obj2, Transform mid1, Transform mid2, float speed)
    {
        obj1.position = new Vector3(obj1.position.x + speed * speedMultiplier * Time.deltaTime, 0f, 0f);
        obj2.position = new Vector3(obj2.position.x + speed * speedMultiplier * Time.deltaTime, 0f, 0f);

        // check if both middles are to the right of x = 0
        if (mid1.position.x > 0 && mid2.position.x > 0)
        {
            // move the rightmost object to the left of the other
            if (mid1.position.x > mid2.position.x)
            {
                // move obj 1 to the right of obj 2
                float newPos = obj2.localPosition.x - obj2.GetComponent<RectTransform>().rect.width + spacing;
                obj1.transform.localPosition = new Vector3(newPos, 0f, 0f);
            }
            else
            {
                // move obj 2 to the right of obj 1
                float newPos = obj1.localPosition.x - obj1.GetComponent<RectTransform>().rect.width + spacing;
                obj2.transform.localPosition = new Vector3(newPos, 0f, 0f);
            }
        }
    }
}
