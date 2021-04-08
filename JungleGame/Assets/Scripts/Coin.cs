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
    public float moveSpeed = 5f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private bool audioPlaying;
    private bool isMoving = false;
    private bool readyToDisable = false;

    // original vars
    private bool originalSet = false;
    private Vector3 originalPos;
    private Transform originalParent;

    void Awake() 
    {
        animator = GetComponent<Animator>();
        animator.Play(coinType.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        originalPos = transform.position;
        originalParent = transform.parent;
    }

    void Update()
    {
        if (readyToDisable && !isMoving)
        {
            gameObject.SetActive(false);
        }
    }

    public void ReturnToOriginalPos()
    {
        isMoving = true;
        StartCoroutine(ReturnToOriginalPosRoutine());
    }

    private IEnumerator ReturnToOriginalPosRoutine()
    {
        Vector3 currTarget = originalPos;
        Vector3 currStart = transform.position;
        float timer = 0f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (transform.position != currTarget)
            {
                transform.position = Vector3.Lerp(currStart, currTarget, timer);
            }
            else
            {
                transform.position = originalPos;
                transform.SetParent(originalParent);
                isMoving = false;
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

    public void SafeDisable()
    {
        StartCoroutine(SafeDisableRoutine());
    }

    private IEnumerator SafeDisableRoutine()
    {
        if (!originalSet)
        {
            originalPos = transform.position;
            originalParent = transform.parent;
            originalSet = true;
        }

        // wait for coin to finish moving
        while (isMoving)
            yield return null;

        transform.position = originalPos;
        transform.SetParent(originalParent);
        readyToDisable = true;
    }

    void OnEnable() 
    {
        readyToDisable = false; 
    }
}
