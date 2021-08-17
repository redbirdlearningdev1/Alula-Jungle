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
    public Transform tigerChallengePos;

    [Header("Brutus")]
    public GameObject brutus;
    public Transform brutusGVPosSTART;
    public Transform brutusGVPosEND;
    public Transform brutusChallengePos;


    [Header("Marcus")]
    public GameObject marcus;
    public Transform marcusGVPosSTART;
    public Transform marcusGVPosEND;
    public Transform marcusChallengePos;


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
    
    void Update()
    {
        // test out animations
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TigerRunAwayGV();
            }
        }
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

    public void TigerAndMonkiesChallengePos()
    {
        StartCoroutine(TigerAndMonkiesChallengePosRoutine());
    }
    private IEnumerator TigerAndMonkiesChallengePosRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerGVPosEND.position;
        marcus.transform.position = marcusGVPosEND.position;
        brutus.transform.position = brutusGVPosEND.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusChallengePos.position, 2f, true));
        yield return new WaitForSeconds(2f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    public void GorillaExitAnimationGV()
    {
        StartCoroutine(GorillaExitAnimationGVRoutine());
    }
    private IEnumerator GorillaExitAnimationGVRoutine()
    {
        animationDone = false;

        yield return new WaitForSeconds(1f);

        // place gorilla in correct location
        gorilla.transform.position = gorillaGVPosSTART.position;

        // play gorilla animation
        gorilla.GetComponent<Animator>().Play("gorillaWalk");
        gorilla.transform.localScale = new Vector3(-1f, 1f, 1f); // flip to face right side

        // move to off screen
        StartCoroutine(MoveObjectOverTime(gorilla, gorillaGVPosEND.position, 7f, true));

        yield return new WaitForSeconds(7f);

        animationDone = true;
    }
    
    public void MonkeyExitAnimationGV()
    {
        StartCoroutine(MonkeyExitAnimationGVRoutine());
    }
    private IEnumerator MonkeyExitAnimationGVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusGVPosEND.position;
        brutus.transform.position = brutusGVPosEND.position;

        // play laugh animations
        marcus.GetComponent<Animator>().Play("marcusWin");
        brutus.GetComponent<Animator>().Play("brutusWin");
        yield return new WaitForSeconds(1.5f);


        // play turn animation
        marcus.GetComponent<Animator>().Play("marcusTurn");
        brutus.GetComponent<Animator>().Play("brutusTurn");
        yield return new WaitForSeconds(0.4f);

        // move to off screen
        StartCoroutine(MoveObjectOverTime(marcus, marcusGVPosSTART.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusGVPosSTART.position, 5f, true));

        yield return new WaitForSeconds(5f);

        animationDone = true;
    }

    public void TigerRunAwayGV()
    {
        StartCoroutine(TigerRunAwayGVRoutine());
    }
    private IEnumerator TigerRunAwayGVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerGVPosEND.position;

        // play turn around animation
        tiger.GetComponent<Animator>().Play("aTigerTurn");
        yield return new WaitForSeconds(0.3f);
        // move to off screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerGVPosSTART.position, 2f, true));

        animationDone = true;
    }

    public void TigerDestroyVillage()
    {
        StartCoroutine(TigerDestroyVillageRoutine());
    }
    private IEnumerator TigerDestroyVillageRoutine()
    {
        animationDone = false;

        // tiger destroys gorilla village
        tiger.GetComponent<Animator>().Play("tigerSwipe");
        tigerScreenSwipeAnim.Play("tigerScreenSwipe");
        // shake screen
        ScrollMapManager.instance.ShakeMap();
        // destroy GV objects one by one
        foreach(var icon in ScrollMapManager.instance.mapIconsAtLocation[2].mapIcons)
        {
            icon.SetFixed(false, true, true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);

        animationDone = true;
    }

    public void TigerAndMonkiesWalkIn()
    {
        StartCoroutine(TigerAndMonkiesWalkInRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkInRoutine()
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
