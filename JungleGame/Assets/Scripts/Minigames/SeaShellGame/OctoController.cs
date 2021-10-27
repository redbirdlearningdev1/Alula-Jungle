using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctoController : MonoBehaviour
{
    public static OctoController instance;

    public Animator octoAnimator;
    public Animator tenticleAnimator;
    public UniversalCoinImage coin;

    [Header("Positions")]
    public Transform coinStartPos;
    public Transform coinHolderPos;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        // reset coin
        coin.SetTransparency(0f, false);
        coin.transform.position = coinStartPos.position;
    }


    public void PlaceNewCoin(ActionWordEnum value)
    {
        coin.SetActionWordValue(value);
        StartCoroutine(PlaceNewCoinRoutine());
    }

    private IEnumerator PlaceNewCoinRoutine()
    {
        tenticleAnimator.Play("armGrab");
        yield return new WaitForSeconds(0.1f);
        coin.GetComponent<LerpableObject>().LerpPosition(coinHolderPos.position, 0.2f, false);
        coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.currImage, 1f, 0.2f);
    }
}
