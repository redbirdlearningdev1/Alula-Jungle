using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PasswordTube : MonoBehaviour
{
    public static PasswordTube instance;

    public Animator tubeAnimator;
    public List<LerpableObject> tabs;
    public List<UniversalCoinImage> tubeCoins;
    public Transform tubeCoinParent;

    public bool playingAnimation = false;

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

        // readd coin to bottom + set value and scale to be normal
        coin.transform.position = NewPasswordGameManager.instance.coinOffScreenPos.position;
        coin.transform.localScale = new Vector3(1f, 1f, 1f);
        coin.GetComponent<UniversalCoinImage>().SetValue(ElkoninValue.empty_gold);

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

    public void ShowPolaroidCoins(ChallengeWord word, List<UniversalCoinImage> allCoins, bool isCorrect)
    {
        playingAnimation = true;

        // create list of non tube coins
        List<UniversalCoinImage> extraCoins = new List<UniversalCoinImage>();
        extraCoins.AddRange(allCoins);

        // remove tube coins
        foreach (var tubeCoin in tubeCoins)
        {
            extraCoins.Remove(tubeCoin);
        }

        StartCoroutine(ShowPolaroidCoinsRoutine(word, extraCoins, isCorrect));
    }

    private IEnumerator ShowPolaroidCoinsRoutine(ChallengeWord word, List<UniversalCoinImage> extraCoins, bool isCorrect)
    {
        // add coins if too little
        if (tubeCoins.Count < word.elkoninCount)
        {
            int addCoins = word.elkoninCount - tubeCoins.Count;
            print ("adding coins: " + addCoins);
            
            for (int i = 0; i < addCoins; i++)
            {
                // add coin
                UniversalCoinImage coin = extraCoins[extraCoins.Count - 1];
                AddCoin(coin.gameObject);

                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 0.8f + (0.1f * i));

                // remove from extra coins
                extraCoins.Remove(coin);
                yield return new WaitForSeconds(0.2f);
            }
        }
        // remove coins if too many
        else if (tubeCoins.Count > word.elkoninCount)
        {
            int removeCoins = tubeCoins.Count - word.elkoninCount;
            print ("removing coins: " + removeCoins);

            for (int i = 0; i < removeCoins; i++)
            {
                // remove coin
                UniversalCoinImage coin = tubeCoins[tubeCoins.Count - 1];
                RemoveCoin(coin.gameObject);

                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 1f - (0.1f * i));
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // turn all tube coins into challenge word coins
        int count = 0;
        foreach (var tubeCoin in tubeCoins)
        {
            tubeCoin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.1f);

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 0.8f + (0.1f * count));
            tubeCoin.SetValue(word.elkoninList[count]);

            tubeCoin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        yield return new WaitForSeconds(0.5f);

        // play each coin sound
        if (!isCorrect)
        {
            foreach (var tubeCoin in tubeCoins)
            {
                AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(tubeCoin.value).audio);
                tubeCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                yield return new WaitForSeconds(1.2f);
                tubeCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
            }

            yield return new WaitForSeconds(1f);
        }

        // say polaroid word + animate tube coins
        NewPasswordGameManager.instance.SayPolaroidWord();
        foreach (var tubeCoin in tubeCoins)
        {
            tubeCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
            yield return new WaitForSeconds(0.1f);
        }
        

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(word.audio));
            yield return cd.coroutine;

        yield return new WaitForSeconds(cd.GetResult());

        foreach (var tubeCoin in tubeCoins)
        {
            tubeCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
            yield return new WaitForSeconds(0.1f);
        }

        playingAnimation = false;
    }
}
