using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordTube : MonoBehaviour
{
    public static PasswordTube instance;

    public Animator tubeAnimator;
    public List<LerpableObject> tabs;
    public List<UniversalCoinImage> tubeCoins;
    public Transform tubeCoinParent;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        // hide all tabs
        foreach(var tab in tabs)
            tab.transform.localScale = new Vector3(1f, 0f, 1f);

        // init coin list
        tubeCoins = new List<UniversalCoinImage>();
    }

    public void TurnTube()
    {
        tubeAnimator.Play("TubeTurn");
    }

    public void StopTube()
    {
        tubeAnimator.Play("TubeStill");
    }

    public void AddCoin(GameObject coin)
    {
        if (tubeCoins.Contains(coin.GetComponent<UniversalCoinImage>()))
            return;

        coin.GetComponent<LerpableObject>().LerpPosToTransform(NewPasswordGameManager.instance.coinTubePositions[tubeCoins.Count], 0.2f, false);
        tabs[tubeCoins.Count].GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
        tubeCoins.Add(coin.GetComponent<UniversalCoinImage>());
    }

    public void RemoveCoin(GameObject coin, bool fixTube = true)
    {
        if (!tubeCoins.Contains(coin.GetComponent<UniversalCoinImage>()))
            return;

        // determine coin index + remove tab
        if (fixTube)
        {
            int index = tubeCoins.IndexOf(coin.GetComponent<UniversalCoinImage>());
            tabs[index].GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 0f), 0.2f);
        }
        
        StartCoroutine(RemoveCoinRoutine(coin, fixTube));
    }

    private IEnumerator RemoveCoinRoutine(GameObject coin, bool fixTube)
    {
        UniversalCoinImage uCoin = coin.GetComponent<UniversalCoinImage>();

        // animate coin removal
        tubeCoins.Remove(uCoin);
        coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.2f);

        // fix tube coins
        if (fixTube)
            StartCoroutine(FixTubeCoins());

        // place coin at the end of the list
        coin.transform.SetParent(NewPasswordGameManager.instance.coinParent);
        NewPasswordGameManager.instance.coins.Remove(uCoin);
        NewPasswordGameManager.instance.coins.Add(uCoin);

        // readd coin to bottom
        coin.transform.position = NewPasswordGameManager.instance.coinOffScreenPos.position;
        coin.transform.localScale = new Vector3(1f, 1f, 1f);

        if (fixTube)
            NewPasswordGameManager.instance.ResetCoins();

        // add coin raycast
        uCoin.ToggleRaycastTarget(true);
    }

    private IEnumerator FixTubeCoins()
    {
        // move coins to correct positions
        int i = 0;
        foreach (var coin in tubeCoins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(NewPasswordGameManager.instance.coinTubePositions[i], 0.2f, false);
            i++;
        }

        // fix tabs
        i = 0;
        foreach(var tab in tabs)
        {
            if (i < tubeCoins.Count)
            {
                tab.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
            }
            else
            {
                tab.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 0f), 0.2f);
            }
            i++;
        }

        yield return null;
    }

    public void CorrectCoinsAnimation()
    {
        StartCoroutine(CorrectCoinsAnimationRoutine());
    }

    private IEnumerator CorrectCoinsAnimationRoutine()
    {
        float yPos = tubeCoins[0].transform.position.y;
        foreach (var coin in tubeCoins)
            coin.GetComponent<LerpableObject>().LerpYPos(yPos + 0.5f, 0.1f, false);

        yield return new WaitForSeconds(0.1f);

        foreach (var coin in tubeCoins)
            coin.GetComponent<LerpableObject>().LerpYPos(yPos, 0.1f, false);

        yield return new WaitForSeconds(0.5f);
        RemoveAllCoins();
    }

    public void RemoveAllCoins()
    {
        StartCoroutine(RemoveAllCoinsRoutine());
    }   

    private IEnumerator RemoveAllCoinsRoutine()
    {
        List<UniversalCoinImage> tubeCoinsCopy = new List<UniversalCoinImage>();
        tubeCoinsCopy.AddRange(tubeCoins);

        // remove coins
        foreach (var coin in tubeCoinsCopy)
        {
            RemoveCoin(coin.gameObject, false);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.25f);

        // remove tabs
        foreach(var tab in tabs)
        {
            tab.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 0f), 0.2f);
        }
    }
}
