using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class UniversalCoinImage : MonoBehaviour
{
    public CoinType coinType;
    private CoinType prevCoinType;
    private bool coinSet;
    public ElkoninValue value;

    // coin objects
    [SerializeField] private ActionWordCoin actionWordCoin;
    [SerializeField] private ConsonantCoin consonantCoin;
    [SerializeField] private Image goldImage;
    [SerializeField] private Image silverImage;
    [SerializeField] private SpriteShakeController shakeController;
    private Image currImage;

    void Awake()
    {
        // set images
        if (coinType == CoinType.ActionWordCoin)
            currImage = goldImage;
        else if (coinType == CoinType.ConsonantCoin)
            currImage = silverImage;
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
                //print ("switching to gold!");
                value = ElkoninValue.empty_gold;
                currImage = goldImage;
                shakeController.SetTransform(goldImage.transform);

                consonantCoin.gameObject.SetActive(false);
                actionWordCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(value));
                actionWordCoin.gameObject.SetActive(true);
            }
            else if (coinType == CoinType.ConsonantCoin && (int)value <= ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_gold)
            {
                //print ("switching to silver!");
                value = ElkoninValue.empty_silver;
                currImage = silverImage;
                shakeController.SetTransform(silverImage.transform);

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

    // public void SetLayer(int layer)
    // {
    //     goldImage.sortingOrder = layer;
    //     silverImage.sortingOrder = layer;
    // }

    public void SetValue(ElkoninValue value)
    {
        //print ("value: " + value);
        if ((int)value <= ChallengeWordDatabase.elkonin_word_separator)
        {
            //print ("action word!");
            prevCoinType = CoinType.ActionWordCoin;
            coinType = CoinType.ActionWordCoin;
            this.value = value;
            currImage = goldImage;
            shakeController.SetTransform(goldImage.transform);

            consonantCoin.gameObject.SetActive(false);
            actionWordCoin.gameObject.SetActive(true);
            actionWordCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToActionWord(value));
        }
        else if ((int)value > ChallengeWordDatabase.elkonin_word_separator || value == ElkoninValue.empty_silver)
        {
            // print ("consonant coin!");
            prevCoinType = CoinType.ConsonantCoin;
            coinType = CoinType.ConsonantCoin;
            this.value = value;
            currImage = silverImage;
            shakeController.SetTransform(silverImage.transform);

            actionWordCoin.gameObject.SetActive(false);
            consonantCoin.gameObject.SetActive(true);
            consonantCoin.SetCoinType(ChallengeWordDatabase.ElkoninValueToConsonantEnum(value));
        }
    }

    public void SetActionWordValue(ActionWordEnum value)
    {
        if (coinType != CoinType.ActionWordCoin)
            return;

        actionWordCoin.SetCoinType(value);
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

    public void ToggleRaycastTarget(bool opt)
    {
        GetComponent<Image>().raycastTarget = opt;
    }

    public void ToggleGlowOutline(bool opt)
    {
        ImageGlowController.instance.SetImageGlow(currImage, opt);
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
            if (!currImage)
                currImage = GetComponent<Image>();
            Color temp = currImage.color;
            if (opt) { temp.a = 1f; }
            else {temp.a = 0; }
            currImage.color = temp;
        }
    }

    public void SetTransparency(float alpha, bool smooth)
    {
        if (smooth)
            StartCoroutine(SetTransparencyRoutine(alpha));
        else
        {
            if (!currImage)
                currImage = GetComponent<Image>();
            Color temp = currImage.color;
            temp.a = alpha;
            currImage.color = temp;
        }
    }

    private IEnumerator SetTransparencyRoutine(float alpha)
    {
        float end = alpha;
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            Color temp = currImage.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            currImage.color = temp;

            if (currImage.color.a == end)
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
            Color temp = currImage.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            currImage.color = temp;

            if (currImage.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
}
