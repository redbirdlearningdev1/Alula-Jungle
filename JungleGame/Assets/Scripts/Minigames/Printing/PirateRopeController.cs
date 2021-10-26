using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateRopeController : MonoBehaviour
{
    public static PirateRopeController instance;

    public LerpableObject lerpableObject;
    public Animator animator;

    [Header("Sway Values")]
    public bool swayRope;
    public AnimationCurve swayCurve;
    public float swayCurveDuration;
    private float timer = 0f;

    [Header("Positions")]
    public Transform ropeOrigin;
    public Transform ropeDropPos;

    [Header("Print Coin")]
    public UniversalCoinImage printingCoin;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        if (swayRope)
        {
            timer += Time.deltaTime;
            var quat = Quaternion.Euler(0f, 0f, swayCurve.Evaluate(timer));
            transform.rotation = quat;
            
            // reset timer
            if (timer >= swayCurveDuration)
            {
                timer = 0f;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void ResetRope()
    {
        transform.localPosition = ropeOrigin.localPosition;
    }

    public void DropRope()
    {
        StartCoroutine(DropRopeRoutine());
    }

    private IEnumerator DropRopeRoutine()
    {
        Vector2 bouncePos = ropeDropPos.position;
        bouncePos.y -= 0.5f;
        lerpableObject.LerpPosition(bouncePos, 0.3f, false);
        yield return new WaitForSeconds(0.3f);
        lerpableObject.LerpPosition(ropeDropPos.position, 0.2f, false);
    }

    public void DropCoinAnimation()
    {
        animator.Play("DropCoin");
    }
}
