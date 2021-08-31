using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmeraldTigerHolder : MonoBehaviour
{
    public static EmeraldTigerHolder instance;

    public List<Sprite> sprites;
    private int numCoins;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetNumberCoins(int amount)
    {
        if (numCoins == amount)
            return;

        if (amount < 0 || amount > 3)
            return;

        numCoins = amount;

        StartCoroutine(SwitchCoinSprite(amount));
    }

    private IEnumerator SwitchCoinSprite(int num)
    {
        GetComponent<WiggleController>().StartWiggle();
        yield return new WaitForSeconds(0.25f);
        GetComponent<Image>().sprite = sprites[num];
        yield return new WaitForSeconds(0.25f);
        GetComponent<WiggleController>().StopWiggle();
    }
}
