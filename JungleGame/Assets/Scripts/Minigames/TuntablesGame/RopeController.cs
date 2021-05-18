using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public static RopeController instance;

    [Header("Positions")]
    [SerializeField] private Transform preRopePos;
    [SerializeField] private Transform bouncePos;
    [SerializeField] private Transform normalRopePos;
    [SerializeField] private Transform postRopePos;
    public float moveTime; // time it takes for rope to move to a new position
    
    [Header("RopePrefab")]
    public GameObject ropePrefab;
    private GameObject currentRope;

    private List<Key> keys;
    private Coroutine currentRoutine;


    void Awake()
    {
        if (!instance)
            instance = this;
        keys = new List<Key>();
    }

    public List<Key> GetKeys()
    {
        return keys;
    }

    public void InitNewRope()
    {
        // delete old rope if exists
        if (currentRope)
            GameObject.Destroy(currentRope);
        
        // instantiate new rope
        currentRope = Instantiate(ropePrefab, preRopePos.position, Quaternion.identity, transform);

        // get keys from new rope
        Key[] foundKeys = currentRope.GetComponentsInChildren<Key>();
        keys.Clear();
        keys.AddRange(foundKeys);
    }

    public void MoveFromInitToNormal()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        // move to normal pos
        currentRoutine = StartCoroutine(BounceToNormal());
    }

    public void MoveFromNormalToEnd()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        // move to the end
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(postRopePos, moveTime, true));
    }

    private IEnumerator BounceToNormal()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        // move to bounce pos
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(bouncePos, moveTime, true));
        yield return new WaitForSeconds(moveTime);
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(normalRopePos, 0.1f, false));
    }

    private IEnumerator MoveRopeToPosRoutine(Transform newPos, float time, bool animate)
    {
        float timer = 0f;
        Vector3 start = currentRope.transform.position;
        Vector3 end = newPos.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                if (animate)
                {
                    foreach (Key k in keys)
                        k.StopMovingAnimation();
                }
                currentRope.transform.position = end;
                break;
            }

            Vector3 pos = Vector3.Lerp(start, end, timer / time);
            currentRope.transform.position = pos;
            yield return null;
        }
    }
}
