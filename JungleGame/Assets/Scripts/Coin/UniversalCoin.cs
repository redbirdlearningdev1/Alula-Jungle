using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinType
{
    ActionWordCoin,
    ConsonantCoin
}

[ExecuteInEditMode]
public class UniversalCoin : MonoBehaviour
{
    public CoinType coinType;
    private CoinType prevCoinType;
    private bool coinSet;

    public ElkoninValue value;

    // coin objects
    [SerializeField] private ActionWordCoin actionWordCoin;
    [SerializeField] private ConsonantCoin consonantCoin;
    [SerializeField] private GlowOutlineController glowConroller;
    [SerializeField] private SpriteRenderer glowSpriteRenderer;
    [SerializeField] private SpriteRenderer goldSpriteRenderer;
    [SerializeField] private SpriteRenderer silverSpriteRenderer;
    [SerializeField] private SpriteShakeController shakeController;
    private SpriteRenderer currentSpriteRenderer;

    void Awake()
    {
        // set the glow renderer to be invisible
        glowSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        // set sprite renderer
        if (coinType == CoinType.ActionWordCoin)
            currentSpriteRenderer = goldSpriteRenderer;
        else if (coinType == CoinType.ConsonantCoin)
            currentSpriteRenderer = silverSpriteRenderer;
    }

    void Update()
    {
        if (coinType != prevCoinType)
        {
            coinSet = false;
            prevCoinType = coinType;

            // change value if needed
            // print ("value: " + (int)value + " , separator: " + ChallengeWordDatabase.elkonin_word_separator + ", coinType: " + coinType);
            if (coinType == CoinType.ActionWordCoin && (int)value > ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_silver)
            {
                // print ("switching to gold!");
                value = ElkoninValue.empty_gold;
                currentSpriteRenderer = goldSpriteRenderer;
                shakeController.SetTransform(goldSpriteRenderer.transform);

                consonantCoin.gameObject.SetActive(false);
                actionWordCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(value));
                actionWordCoin.gameObject.SetActive(true);
            }
            else if (coinType == CoinType.ConsonantCoin && (int)value <= ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_gold)
            {
                // print ("switching to silver!");
                value = ElkoninValue.empty_silver;
                currentSpriteRenderer = silverSpriteRenderer;
                shakeController.SetTransform(silverSpriteRenderer.transform);

                actionWordCoin.gameObject.SetActive(false);
                consonantCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToConsonantEnum(value));
                consonantCoin.gameObject.SetActive(true);
            }

            coinSet = true;
        }
    }

    public void PlayAudio()
    {
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(value).audio);
    }

    public void SetLayer(int layer)
    {
        goldSpriteRenderer.sortingOrder = layer;
        silverSpriteRenderer.sortingOrder = layer;
    }

    public void SetValue(ElkoninValue value)
    {
        //print ("value: " + value);
        if ((int)value <= ChallengeWordDatabase.elkonin_word_separator)
        {
            //print ("action word!");
            prevCoinType = CoinType.ActionWordCoin;
            coinType = CoinType.ActionWordCoin;
            this.value = value;
            currentSpriteRenderer = goldSpriteRenderer;
            shakeController.SetTransform(goldSpriteRenderer.transform);

            consonantCoin.gameObject.SetActive(false);
            actionWordCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(value));
            actionWordCoin.gameObject.SetActive(true);
        }
        else if ((int)value > ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_silver)
        {
            // print ("consonant coin!");
            prevCoinType = CoinType.ConsonantCoin;
            coinType = CoinType.ConsonantCoin;
            this.value = value;
            currentSpriteRenderer = silverSpriteRenderer;
            shakeController.SetTransform(silverSpriteRenderer.transform);

            actionWordCoin.gameObject.SetActive(false);
            consonantCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToConsonantEnum(value));
            consonantCoin.gameObject.SetActive(true);
        }
    }

    public void ShakeCoin(float duration)
    {
        shakeController.ShakeObject(duration);
    }

    public void SetSize(Vector2 size)
    {
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void LerpSize(Vector2 targetSize, float totalTime)
    {
        StopAllCoroutines();
        StartCoroutine(LerpSizeRoutine(targetSize ,totalTime));
    }

    private IEnumerator LerpSizeRoutine(Vector2 targetSize, float totalTime)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 startSize = rect.sizeDelta;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                rect.sizeDelta = targetSize;
                break;
            }

            Vector2 tempSize = Vector2.Lerp(startSize, targetSize, timer / totalTime);
            rect.sizeDelta = tempSize;
            yield return null;
        }
    }

    public void ToggleGlowOutline(bool opt)
    {
        if (opt)
            glowSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        else
            glowSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        glowConroller.ToggleGlowOutline(opt);
    }

    /* 
    ################################################
    #   VISIBILITY FUNCTIONS
    ################################################
    */

    public void ToggleVisibility(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!currentSpriteRenderer)
                currentSpriteRenderer = GetComponent<SpriteRenderer>();
            Color temp = currentSpriteRenderer.color;
            if (opt) { temp.a = 1f; }
            else {temp.a = 0; }
            currentSpriteRenderer.color = temp;
        }
    }

    public void SetTransparency(float alpha, bool smooth)
    {
        if (smooth)
            StartCoroutine(SetTransparencyRoutine(alpha));
        else
        {
            if (!currentSpriteRenderer)
                currentSpriteRenderer = GetComponent<SpriteRenderer>();
            Color temp = currentSpriteRenderer.color;
            temp.a = alpha;
            currentSpriteRenderer.color = temp;
        }
    }

    private IEnumerator SetTransparencyRoutine(float alpha)
    {
        float end = alpha;
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            Color temp = currentSpriteRenderer.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            currentSpriteRenderer.color = temp;

            if (currentSpriteRenderer.color.a == end)
            {
                break;
            }
            yield return null;
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
            Color temp = currentSpriteRenderer.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            currentSpriteRenderer.color = temp;

            if (currentSpriteRenderer.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
}
