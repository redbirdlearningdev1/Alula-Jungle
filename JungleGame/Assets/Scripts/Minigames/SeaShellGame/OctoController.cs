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
    public Transform coinIncorrectPos;
    public Transform coinCorrectPos;
    public Transform coinChestPos;

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
        coin.transform.position = coinStartPos.position;
        coin.transform.localScale = new Vector3(1f, 1f, 1f);
        coin.GetComponent<LerpableObject>().SetImageAlpha(coin.currImage, 0f);
        StartCoroutine(PlaceNewCoinRoutine());
    }

    public void PlaceNewCoin(string value)
    {
        coin.UpdateWordText(value);
        coin.transform.position = coinStartPos.position;
        coin.transform.localScale = new Vector3(1f, 1f, 1f);
        coin.GetComponent<LerpableObject>().SetImageAlpha(coin.currImage, 0f);
        StartCoroutine(PlaceNewCoinRoutine());
    }

    private IEnumerator PlaceNewCoinRoutine()
    {
        octoAnimator.Play("octoPlaceCoin");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BubbleRise, 0.5f);
        yield return new WaitForSeconds(0.4f);
        tenticleAnimator.Play("armGrab");
        yield return new WaitForSeconds(0.1f);
        coin.GetComponent<LerpableObject>().LerpPosition(coinHolderPos.position, 0.2f, false);
        coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.currImage, 1f, 0.2f);

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinOnRock, 0.5f);
    }

    public void CoinIncorrect()
    {   
        StartCoroutine(CoinIncorrectRoutine());
    }   

    private IEnumerator CoinIncorrectRoutine()
    {
        // show and play correct shell
        ShellController.instance.ShowCorrectShell();
        yield return new WaitForSeconds(2f);

        // hide shells
        ShellController.instance.HideShells();

        // incorrect animation
        octoAnimator.Play("octoGrabShow");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BubbleRise, 0.5f);
        yield return new WaitForSeconds(0.4f);
        tenticleAnimator.Play("armGrab");
        yield return new WaitForSeconds(0.3f);
        coin.GetComponent<LerpableObject>().LerpPosition(coinIncorrectPos.position, 0.2f, false);
        coin.GetComponent<LerpableObject>().LerpImageAlpha(coin.currImage, 1f, 0.2f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinFlip, 0.5f);
        yield return new WaitForSeconds(0.5f);
        octoAnimator.Play("octoNo");
    }

    public void CoinCorrect()
    {   
        StartCoroutine(CoinCorrectRoutine());
    }   

    private IEnumerator CoinCorrectRoutine()
    {
        octoAnimator.Play("octoGrabShow");
        yield return new WaitForSeconds(0.25f);
        tenticleAnimator.Play("armGrab");
        yield return new WaitForSeconds(0.4f);
        coin.GetComponent<LerpableObject>().LerpPosition(coinCorrectPos.position, 0.8f, false);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(0.8f, 0.8f), 0.6f);
        coin.GetComponent<LerpableObject>().LerpRotation(720f, 0.8f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinFlip, 0.5f);
        yield return new WaitForSeconds(1.5f);
        octoAnimator.Play("octoAway");
        coin.GetComponent<LerpableObject>().LerpPosition(coinChestPos.position, 0.35f, false);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.35f);
        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
        AudioManager.instance.PlayCoinDrop();
        Chest.instance.UpgradeChest();
    }
}
