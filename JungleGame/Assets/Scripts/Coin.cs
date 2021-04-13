﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int logIndex;
    public Transform logPos;
    public Transform logParent;
    public float moveSpeed = 5f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private Image image;
    private bool audioPlaying;

    // original vars
    private bool originalSet = false;

    void Awake() 
    {
        animator = GetComponent<Animator>();
        animator.Play(coinType.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();
    }

    void Update()
    {

    }

    public void ReturnToLog()
    {
        StartCoroutine(ReturnToOriginalPosRoutine(logPos.position));
    }

    private IEnumerator ReturnToOriginalPosRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                transform.SetParent(logParent);
                yield break;
            }

            yield return null;
        }
    }

    public void SetCoinType(CoinType type)
    {
        coinType = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();
        animator.Play(coinType.ToString());
    }

    public void PlayPhonemeAudio()
    {
        if (!audioPlaying)
        {
            StartCoroutine(PlayPhonemeAudioRoutine());
        }
    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        Phoneme phoneme = (Phoneme)coinType;
        AudioManager.instance.PlayPhoneme(phoneme);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    public void ToggleVisibility(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!image)
                image = GetComponent<Image>();
            Color temp = image.color;
            if (opt) { temp.a = 1f; }
            else {temp.a = 0; }
            image.color = temp;
        }
    }   

    private IEnumerator ToggleVisibilityRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 1f; }
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            Color temp = image.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            image.color = temp;

            if (image.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
}
