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
    
    // set individual key to be glowing
    public void SetKeyGlow(Key k, bool opt)
    {
        if (keys.Contains(k))
        {
            ImageGlowController.instance.SetImageGlow(k.image, true, GlowValue.glow_1_025);
        }
    }

    // clears all the glow from keys
    public void ClearKeyGlows()
    {
        if (keys != null)
        {
            foreach (var k in keys)
            {
                ImageGlowController.instance.SetImageGlow(k.image, false);
            }
        }
    }

    public List<Key> GetKeys()
    {
        return keys;
    }

    public void AnimateKeysUp()
    {
        // play key jingle
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyLatch, 1f);

        if (keys == null) return;
        foreach (Key k in keys)
            k.StartMovingAnimation();
    }

    public void AnimateKeysDown()
    {
        // play key jingle
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.KeyLatch, 1f);

        if (keys == null) return;
        foreach (Key k in keys)
            k.StopMovingAnimation();
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

        // remove glow from keys
        ClearKeyGlows();
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
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(postRopePos, moveTime));
    }

    private IEnumerator BounceToNormal()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        // move to bounce pos
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(bouncePos, moveTime));
        yield return new WaitForSeconds(moveTime);
        currentRoutine = StartCoroutine(MoveRopeToPosRoutine(normalRopePos, 0.1f));
    }

    private IEnumerator MoveRopeToPosRoutine(Transform newPos, float time)
    {
        float timer = 0f;
        Vector3 start = currentRope.transform.position;
        Vector3 end = newPos.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                currentRope.transform.position = end;
                break;
            }

            Vector3 pos = Vector3.Lerp(start, end, timer / time);
            currentRope.transform.position = pos;
            yield return null;
        }
    }
}
