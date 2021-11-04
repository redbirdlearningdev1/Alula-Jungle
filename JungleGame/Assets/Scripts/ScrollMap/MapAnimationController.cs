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

    [Header("Gorilla GV")]
    public GameObject gorilla;
    public Transform gorillaGVPosSTART;
    public Transform gorillaGVPosEND;

    [Header("Gorilla GV")]
    public Transform gorillaSFPosDEFAULT;

    [Header("Orc OV")]
    public Transform cloggOVPosDEFAULT;

    /* 
    ################################################
    #   TIGER POS
    ################################################
    */

    [Header("Tiger GV")]
    public Animator tigerScreenSwipeAnim;
    public GameObject tiger;
    public Transform tigerGVPosSTART;
    public Transform tigerGVPosEND;
    public Transform tigerGVChallengePos;

    [Header("Tiger MS")]
    public Transform tigerMSPosSTART;
    public Transform tigerMSPosEND;
    public Transform tigerMSChallengePos;

    [Header("Tiger OV")]
    public Transform tigerOVPosSTART;
    public Transform tigerOVPosEND;
    public Transform tigerOVChallengePos;

    [Header("Tiger OV")]
    public Transform tigerSFPosSTART;
    public Transform tigerSFPosEND;
    public Transform tigerSFChallengePos;

    /* 
    ################################################
    #   BRUTUS POS
    ################################################
    */

    [Header("Brutus GV")]
    public GameObject brutus;
    public Transform brutusGVPosSTART;
    public Transform brutusGVPosEND;
    public Transform brutusGVChallengePos;

    [Header("Brutus MS")]
    public Transform brutusMSPosSTART;
    public Transform brutusMSPosEND;
    public Transform brutusMSChallengePos;

    [Header("Brutus OV")]
    public Transform brutusOVPosSTART;
    public Transform brutusOVPosEND;
    public Transform brutusOVChallengePos;

    [Header("Brutus OV")]
    public Transform brutusSFPosSTART;
    public Transform brutusSFPosEND;
    public Transform brutusSFChallengePos;

    /* 
    ################################################
    #   MARCUS POS
    ################################################
    */

    [Header("Marcus GV")]
    public GameObject marcus;
    public Transform marcusGVPosSTART;
    public Transform marcusGVPosEND;
    public Transform marcusGVChallengePos;

    [Header("Marcus MS")]
    public Transform marcusMSPosSTART;
    public Transform marcusMSPosEND;
    public Transform marcusMSChallengePos;

    [Header("Marcus OV")]
    public Transform marcusOVPosSTART;
    public Transform marcusOVPosEND;
    public Transform marcusOVChallengePos;

    [Header("Marcus OV")]
    public Transform marcusSFPosSTART;
    public Transform marcusSFPosEND;
    public Transform marcusSFChallengePos;


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

    /* 
    ################################################
    #   SPOOKY FOREST ANIMATIONS
    ################################################
    */

    public void TigerDestroyForest()
    {
        StartCoroutine(TigerDestroyForestRoutine());
    }
    private IEnumerator TigerDestroyForestRoutine()
    {
        animationDone = false;

        // tiger destroys gorilla village
        tiger.GetComponent<Animator>().Play("tigerSwipe");
        tigerScreenSwipeAnim.Play("tigerScreenSwipe");
        // shake screen
        ScrollMapManager.instance.ShakeMap();
        // destroy GV objects one by one
        foreach(var icon in ScrollMapManager.instance.mapIconsAtLocation[5].mapIcons)
        {
            icon.SetFixed(false, true, false);
            yield return new WaitForSeconds(0.1f);
        }
        // set fixed to false
        StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed = false;
        // set stars to 0
        StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars = 0;

        // save to SIS + reload map data
        yield return new WaitForSeconds(2f);
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    public void MonkeyExitAnimationDefeatedSF()
    {
        StartCoroutine(MonkeyExitAnimationDefeatedSFRoutine());
    }
    private IEnumerator MonkeyExitAnimationDefeatedSFRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusSFChallengePos.position;
        brutus.transform.position = brutusSFChallengePos.position;

        // play laugh animations
        marcus.GetComponent<Animator>().Play("marcusWin");
        brutus.GetComponent<Animator>().Play("brutusWin");
        yield return new WaitForSeconds(1.5f);


        // play turn animation
        marcus.GetComponent<Animator>().Play("marcusTurn");
        brutus.GetComponent<Animator>().Play("brutusTurn");
        yield return new WaitForSeconds(0.4f);

        // move to off screen
        StartCoroutine(MoveObjectOverTime(marcus, marcusSFPosSTART.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusSFPosSTART.position, 5f, true));

        yield return new WaitForSeconds(5f);

        animationDone = true;
    }

    public void TigerRunAwayDefeatedSF()
    {
        StartCoroutine(TigerRunAwayDefeatedSFRoutine());
    }
    private IEnumerator TigerRunAwayDefeatedSFRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerSFChallengePos.position;

        // play turn around animation
        tiger.GetComponent<Animator>().Play("aTigerTurn");
        yield return new WaitForSeconds(0.3f);
        // move to off screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerSFPosSTART.position, 2f, true));

        animationDone = true;
    }

    public void TigerAndMonkiesChallengePosSF()
    {
        StartCoroutine(TigerAndMonkiesChallengePosSFRoutine());
    }
    private IEnumerator TigerAndMonkiesChallengePosSFRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerSFPosEND.position;
        marcus.transform.position = marcusSFPosEND.position;
        brutus.transform.position = brutusSFPosEND.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerSFChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusSFChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusSFChallengePos.position, 2f, true));
        yield return new WaitForSeconds(2f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    public void TigerAndMonkiesWalkInSF()
    {
        StartCoroutine(TigerAndMonkiesWalkInSFRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkInSFRoutine()
    {
        animationDone = false;

        // place characters in start pos
        tiger.transform.position = tigerSFPosSTART.position;
        marcus.transform.position = marcusSFPosSTART.position;
        brutus.transform.position = brutusSFPosSTART.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerSFPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusSFPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusSFPosEND.position, 5f, true));
        yield return new WaitForSeconds(5f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    public void MonkeyExitAnimationSF()
    {
        StartCoroutine(MonkeyExitAnimationSFRoutine());
    }
    private IEnumerator MonkeyExitAnimationSFRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusSFPosEND.position;
        brutus.transform.position = brutusSFPosEND.position;

        // play laugh animations
        marcus.GetComponent<Animator>().Play("marcusWin");
        brutus.GetComponent<Animator>().Play("brutusWin");
        yield return new WaitForSeconds(1.5f);


        // play turn animation
        marcus.GetComponent<Animator>().Play("marcusTurn");
        brutus.GetComponent<Animator>().Play("brutusTurn");
        yield return new WaitForSeconds(0.4f);

        // move to off screen
        StartCoroutine(MoveObjectOverTime(marcus, marcusSFPosSTART.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusSFPosSTART.position, 5f, true));

        yield return new WaitForSeconds(5f);

        animationDone = true;
    }

    /* 
    ################################################
    #   ORC VILLAGE ANIMATIONS
    ################################################
    */

    public void MonkeyExitAnimationDefeatedOV()
    {
        StartCoroutine(MonkeyExitAnimationDefeatedOVRoutine());
    }
    private IEnumerator MonkeyExitAnimationDefeatedOVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusOVChallengePos.position;
        brutus.transform.position = brutusOVChallengePos.position;

        // play laugh animations
        marcus.GetComponent<Animator>().Play("marcusWin");
        brutus.GetComponent<Animator>().Play("brutusWin");
        yield return new WaitForSeconds(1.5f);


        // play turn animation
        marcus.GetComponent<Animator>().Play("marcusTurn");
        brutus.GetComponent<Animator>().Play("brutusTurn");
        yield return new WaitForSeconds(0.4f);

        // move to off screen
        StartCoroutine(MoveObjectOverTime(marcus, marcusOVPosSTART.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusOVPosSTART.position, 5f, true));

        yield return new WaitForSeconds(5f);

        animationDone = true;
    }

    public void TigerRunAwayDefeatedOV()
    {
        StartCoroutine(TigerRunAwayDefeatedOVRoutine());
    }
    private IEnumerator TigerRunAwayDefeatedOVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerOVChallengePos.position;

        // play turn around animation
        tiger.GetComponent<Animator>().Play("aTigerTurn");
        yield return new WaitForSeconds(0.3f);
        // move to off screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerOVPosSTART.position, 2f, true));

        animationDone = true;
    }

    public void TigerAndMonkiesChallengePosOV()
    {
        StartCoroutine(TigerAndMonkiesChallengePosOVRoutine());
    }
    private IEnumerator TigerAndMonkiesChallengePosOVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerOVPosEND.position;
        marcus.transform.position = marcusOVPosEND.position;
        brutus.transform.position = brutusOVPosEND.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerOVChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusOVChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusOVChallengePos.position, 2f, true));
        yield return new WaitForSeconds(2f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    public void TigerAndMonkiesWalkInOV()
    {
        StartCoroutine(TigerAndMonkiesWalkInOVRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkInOVRoutine()
    {
        animationDone = false;

        // place characters in start pos
        tiger.transform.position = tigerOVPosSTART.position;
        marcus.transform.position = marcusOVPosSTART.position;
        brutus.transform.position = brutusOVPosSTART.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerOVPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusOVPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusOVPosEND.position, 5f, true));
        yield return new WaitForSeconds(5f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    /* 
    ################################################
    #   MUDSLIDE ANIMATIONS
    ################################################
    */

    public void MonkeyExitAnimationDefeatedMS()
    {
        StartCoroutine(MonkeyExitAnimationDefeatedMSRoutine());
    }
    private IEnumerator MonkeyExitAnimationDefeatedMSRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusMSChallengePos.position;
        brutus.transform.position = brutusMSChallengePos.position;

        // play laugh animations
        marcus.GetComponent<Animator>().Play("marcusWin");
        brutus.GetComponent<Animator>().Play("brutusWin");
        yield return new WaitForSeconds(1.5f);


        // play turn animation
        marcus.GetComponent<Animator>().Play("marcusTurn");
        brutus.GetComponent<Animator>().Play("brutusTurn");
        yield return new WaitForSeconds(0.4f);

        // move to off screen
        StartCoroutine(MoveObjectOverTime(marcus, marcusMSPosSTART.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusMSPosSTART.position, 5f, true));

        yield return new WaitForSeconds(5f);

        animationDone = true;
    }

    public void TigerRunAwayDefeatedMS()
    {
        StartCoroutine(TigerRunAwayDefeatedMSRoutine());
    }
    private IEnumerator TigerRunAwayDefeatedMSRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerMSChallengePos.position;

        // play turn around animation
        tiger.GetComponent<Animator>().Play("aTigerTurn");
        yield return new WaitForSeconds(0.3f);
        // move to off screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerMSPosSTART.position, 2f, true));

        animationDone = true;
    }

    public void TigerAndMonkiesChallengePosMS()
    {
        StartCoroutine(TigerAndMonkiesChallengePosMSRoutine());
    }
    private IEnumerator TigerAndMonkiesChallengePosMSRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerMSPosEND.position;
        marcus.transform.position = marcusMSPosEND.position;
        brutus.transform.position = brutusMSPosEND.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerMSChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusMSChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusMSChallengePos.position, 2f, true));
        yield return new WaitForSeconds(2f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    public void TigerAndMonkiesWalkInMS()
    {
        StartCoroutine(TigerAndMonkiesWalkInMSRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkInMSRoutine()
    {
        animationDone = false;

        // place characters in start pos
        tiger.transform.position = tigerMSPosSTART.position;
        marcus.transform.position = marcusMSPosSTART.position;
        brutus.transform.position = brutusMSPosSTART.position;

        // play correct walk in animation
        tiger.GetComponent<Animator>().Play("tigerWalk");
        marcus.GetComponent<Animator>().Play("marcusWalkIn");
        brutus.GetComponent<Animator>().Play("brutusWalkIn");

        // move characters to end positions on screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerMSPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusMSPosEND.position, 5f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusMSPosEND.position, 5f, true));
        yield return new WaitForSeconds(5f);

        // play correct idle in animation
        tiger.GetComponent<Animator>().Play("aTigerIdle");
        marcus.GetComponent<Animator>().Play("marcusFixed");
        brutus.GetComponent<Animator>().Play("brutusFixed");

        animationDone = true;
    }

    /* 
    ################################################
    #   GORILLA VILLAGE ANIMATIONS
    ################################################
    */

    public void MonkeyExitAnimationDefeatedGV()
    {
        StartCoroutine(MonkeyExitAnimationDefeatedGVRoutine());
    }
    private IEnumerator MonkeyExitAnimationDefeatedGVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        marcus.transform.position = marcusGVChallengePos.position;
        brutus.transform.position = brutusGVChallengePos.position;

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

    public void TigerRunAwayDefeatedGV()
    {
        StartCoroutine(TigerRunAwayDefeatedGVRoutine());
    }
    private IEnumerator TigerRunAwayDefeatedGVRoutine()
    {
        animationDone = false;

        // place characters in end pos
        tiger.transform.position = tigerGVChallengePos.position;

        // play turn around animation
        tiger.GetComponent<Animator>().Play("aTigerTurn");
        yield return new WaitForSeconds(0.3f);
        // move to off screen
        StartCoroutine(MoveObjectOverTime(tiger, tigerGVPosSTART.position, 2f, true));

        animationDone = true;
    }

    public void TigerAndMonkiesChallengePosGV()
    {
        StartCoroutine(TigerAndMonkiesChallengePosGVRoutine());
    }
    private IEnumerator TigerAndMonkiesChallengePosGVRoutine()
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
        StartCoroutine(MoveObjectOverTime(tiger, tigerGVChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(marcus, marcusGVChallengePos.position, 2f, true));
        StartCoroutine(MoveObjectOverTime(brutus, brutusGVChallengePos.position, 2f, true));
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
        gorilla.GetComponent<MapCharacter>().FlipCharacterToRight();

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
            icon.SetFixed(false, true, false);
            yield return new WaitForSeconds(0.1f);
        }
        // set fixed to false
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed = false;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed = false;
        // set stars to 0
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 0;
        StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 0;

        // save to SIS + reload map data
        yield return new WaitForSeconds(2f);
        StudentInfoSystem.SaveStudentPlayerData();

        animationDone = true;
    }

    public void TigerAndMonkiesWalkInGV()
    {
        StartCoroutine(TigerAndMonkiesWalkInGVRoutine());
    }
    private IEnumerator TigerAndMonkiesWalkInGVRoutine()
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
