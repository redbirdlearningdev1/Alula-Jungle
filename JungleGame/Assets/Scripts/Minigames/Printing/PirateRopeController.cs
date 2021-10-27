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
    public Transform printingCoinPos;

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
        printingCoin.SetActionWordValue(ActionWordEnum._blank);
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
        StartCoroutine(DropCoinAnimationRoutine());
    }

    private IEnumerator DropCoinAnimationRoutine()
    {
        animator.Play("DropCoin");
        yield return new WaitForSeconds(2f);

        // return rope to origin
        Vector2 bouncePos = ropeDropPos.position;
        bouncePos.y -= 0.5f;
        lerpableObject.LerpPosition(bouncePos, 0.3f, false);
        yield return new WaitForSeconds(0.3f);

        lerpableObject.LerpPosition(ropeOrigin.position, 0.2f, false);
        yield return new WaitForSeconds(1f);

        // reset rope
        printingCoin.transform.localScale = new Vector3(1f, 1f, 1f);
        printingCoin.transform.localPosition = printingCoinPos.localPosition;
        animator.Play("IdleRope");
    }

    public void RaiseRopeAnimation()
    {
        StartCoroutine(RaiseRopeAnimationRoutine());
    }

    private IEnumerator RaiseRopeAnimationRoutine()
    {
        // return rope to origin
        Vector2 bouncePos = ropeDropPos.position;
        bouncePos.y -= 0.5f;
        lerpableObject.LerpPosition(bouncePos, 0.3f, false);
        yield return new WaitForSeconds(0.3f);

        lerpableObject.LerpPosition(ropeOrigin.position, 0.2f, false);
        yield return new WaitForSeconds(1f);

        // reset rope
        printingCoin.transform.localScale = new Vector3(1f, 1f, 1f);
        printingCoin.transform.localPosition = printingCoinPos.localPosition;
        animator.Play("IdleRope");
    }
}
