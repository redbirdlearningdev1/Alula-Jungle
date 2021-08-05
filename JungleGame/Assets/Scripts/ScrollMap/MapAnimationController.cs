using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAnimationController : MonoBehaviour
{
    public static MapAnimationController instance;
    [HideInInspector] public bool animationDone = false; // used to determine when the current animation is complete

    [Header("Boat")]
    public GameObject boat;
    public Transform boatOffMapPos;
    public Transform boatInOceanPos;
    public Transform boatNotDockedPos;
    public Transform boatDockedPos;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // used to move object for animation
    private IEnumerator MoveObjectOverTime(GameObject gameObject, Vector3 targetPos, float duration, bool smoothLerp = false)
    {
        float timer = 0f;
        Vector3 startPos = gameObject.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                gameObject.transform.position = targetPos;
                break;
            }

            // choose between smooth and straight animation styles
            Vector3 tempPos;
            if (smoothLerp)
            {
                tempPos = Vector3.Slerp(startPos, targetPos, timer / duration);
            }
            else
            {
                tempPos = Vector3.Lerp(startPos, targetPos, timer / duration);
            }
            
            gameObject.transform.position = tempPos;
            yield return null;
        }
    }

    public void BoatOceanIntro()
    {
        StartCoroutine(BoatOceanIntroRoutine());
    }
    private IEnumerator BoatOceanIntroRoutine()
    {
        animationDone = false;

        // place boat out of view
        boat.transform.position = boatOffMapPos.position;
        yield return new WaitForSeconds(1f);

        // slowly move boat to ocean position
        StartCoroutine(MoveObjectOverTime(boat, boatInOceanPos.position, 6f, true));
        yield return new WaitForSeconds(6f);

        animationDone = true;
    }

    public void DockBoat()
    {
        StartCoroutine(DockBoatRoutine());
    }
    private IEnumerator DockBoatRoutine()
    {
        animationDone = false;

        // place boat out of view
        boat.transform.position = boatNotDockedPos.position;
        yield return new WaitForSeconds(1f);

        // slowly move boat to ocean position
        StartCoroutine(MoveObjectOverTime(boat, boatDockedPos.position, 6f, true));
        yield return new WaitForSeconds(6f);

        animationDone = true;
    }
}
