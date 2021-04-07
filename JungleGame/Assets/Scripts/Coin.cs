using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinType
{
    a_Coin,
    aCoin,
    airCoin,
    areCoin,
    awCoin,
    e_Coin,
    earCoin,
    eCoin,
    erCoin,
    i_Coin,
    iCoin,
    o_Coin,
    oCoin,
    oiCoin,
    ooCoin,
    orCoin,
    owCoin,
    u_Coin,
    uCoin,
    yerCoin,
    COUNT
}

public class Coin : MonoBehaviour
{
    public CoinType coinType;
    private Animator animator;
    private BoxCollider2D myCollider;
    private bool isPressed = false;

    void Awake() 
    {
        animator = GetComponent<Animator>();
        animator.Play(coinType.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;
    }

    public void SetCoinType(CoinType type)
    {
        coinType = type;
        // get animator if null
        if (!animator)
            Awake();
        animator.Play(coinType.ToString());
    }

    public void AudioButton()
    {
        Phoneme phoneme = (Phoneme)coinType;
        AudioManager.instance.PlayPhoneme(phoneme);
    }
}
