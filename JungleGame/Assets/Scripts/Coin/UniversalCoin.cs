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

                consonantCoin.gameObject.SetActive(false);
                actionWordCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(value));
                actionWordCoin.gameObject.SetActive(true);
            }
            else if (coinType == CoinType.ConsonantCoin && (int)value <= ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_gold)
            {
                // print ("switching to silver!");
                value = ElkoninValue.empty_silver;

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

    public void SetValue(ElkoninValue value)
    {
        //print ("value: " + value);
        if ((int)value <= ChallengeWordDatabase.elkonin_word_separator)
        {
            //print ("action word!");
            prevCoinType = CoinType.ActionWordCoin;
            coinType = CoinType.ActionWordCoin;
            this.value = value;

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

            actionWordCoin.gameObject.SetActive(false);
            consonantCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToConsonantEnum(value));
            consonantCoin.gameObject.SetActive(true);
        }
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
        glowConroller.ToggleGlowOutline(opt);
    }
}
