using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterCoinsController : MonoBehaviour
{
    public static WaterCoinsController instance;

    public List<UniversalCoinImage> waterCoins;
    public List<float> spacings;
    public List<GameObject> activeCoinPos;
    public List<GameObject> inactiveCoinPos;
    public HorizontalLayoutGroup activeCoinsGroup;
    public HorizontalLayoutGroup inactiveCoinsGroup;

    public int numCoins;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ReturnWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            if (waterCoins[i].gameObject.activeSelf && waterCoins[i] != WordFactoryBuildingManager.instance.currentCoin)
            {
                waterCoins[i].GetComponent<LerpableObject>().LerpPosition(activeCoinPos[i].transform.position, 0.25f, false);
                waterCoins[i].LerpSize(WordFactoryBuildingManager.instance.normalCoinSize, 0.2f);
            }
        }
    }

    public void ResetWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            waterCoins[i].GetComponent<LerpableObject>().LerpPosition(inactiveCoinPos[i].transform.position, 0.25f, false);
            waterCoins[i].LerpSize(WordFactoryBuildingManager.instance.normalCoinSize, 0.2f);
        }
    }

    public void SetNumberWaterCoins(int num)
    {
        if (num > 1 && num <= 6)
        {
            numCoins = num;

            // set each coin pos to be active
            foreach(var coin in activeCoinPos)
                coin.SetActive(true);
            foreach(var coin in inactiveCoinPos)
                coin.SetActive(true);

            // deactivate unused pos
            int size = activeCoinPos.Count - numCoins;
            for (int i = 0; i < size; i++)
            {
                activeCoinPos[activeCoinPos.Count - i - 1].SetActive(false);
                inactiveCoinPos[inactiveCoinPos.Count - i - 1].SetActive(false);
            }

            // set correct spacing between frames
            activeCoinsGroup.spacing = spacings[numCoins - 1];
            inactiveCoinsGroup.spacing = spacings[numCoins - 1];

            StartCoroutine(SetWaterCoinsRoutine());
        }
    }

    private IEnumerator SetWaterCoinsRoutine()
    {
        yield return new WaitForSeconds (0.1f);
        // water coin objects
        foreach(var coin in waterCoins)
            coin.gameObject.SetActive(true);
        // deactivate unused coins
        int size = waterCoins.Count - numCoins;
        for (int i = 0; i < size; i++)
        {
            waterCoins[waterCoins.Count - i - 1].gameObject.SetActive(false);
        }
        // place in correct position
        int count = 0;
        foreach(var coin in waterCoins)
        {
            if (coin.gameObject.activeSelf)
            {
                coin.transform.position = inactiveCoinPos[count].transform.position;
            }
            count++;
        }
    }

    public void ShowWaterCoins()
    {
        StartCoroutine(ShowWaterCoinsRoutine());
    }

    private IEnumerator ShowWaterCoinsRoutine()
    {
        for (int i = 0; i < numCoins; i++)
        {
            waterCoins[i].GetComponent<LerpableObject>().LerpPosition(activeCoinPos[i].transform.position, 0.25f, false);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
