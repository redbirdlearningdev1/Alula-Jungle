using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAnimationController : MonoBehaviour
{
    public static MapAnimationController instance;
    [HideInInspector] public bool animationDone = false; // used to determine when the current animation is complete

    public Transform offscreenPos;

    [Header("Boat")]
    public GameObject boat;
    public Transform boatOffMapPos;
    public Transform boatInOceanPos;
    public Transform boatNotDockedPos;
    public Transform boatDockedPos;

    [Header("Gorilla")]
    public GameObject gorilla;
    public Transform gorillaGVPosSTART;
    public Transform gorillaGVPosEND;

    [Header("Tiger")]
    public Animator tigerScreenSwipeAnim;
    public GameObject tiger;
    public Transform tigerGVPosSTART;
    public Transform tigerGVPosEND;

    [Header("Brutus")]
    public GameObject brutus;
    public Transform brutusGVPosSTART;
    public Transform brutusGVPosEND;

    [Header("Marcus")]
    public GameObject marcus;
    public Transform marcusGVPosSTART;
    public Transform marcusGVPosEND;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // place all characters off screen
        boat.transform.position = offscreenPos.position;
        gorilla.transform.position = offscreenPos.position;
        tiger.transform.position = offscreenPos.position;
        brutus.transform.position = offscreenPos.position;
        marcus.transform.position = offscreenPos.position;
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

    public void TigerAndMonkiesWalkOut()
    {
        StartCoroutine(TigerAndMonkiesWalkOutRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkOutRoutine()
    {
        animationDone = false;

        // place characters in start pos
        tiger.transform.position = tigerGVPosSTART.position;
        marcus.transform.position = marcusGVPosSTART.position;
        brutus.transform.position = brutusGVPosSTART.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerGVPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusGVPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusGVPosEND.position, 5f, true));
        yield return new WaitForSeconds(5f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
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
