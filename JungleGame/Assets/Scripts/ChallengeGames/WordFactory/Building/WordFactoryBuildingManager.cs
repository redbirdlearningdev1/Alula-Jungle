using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFactoryBuildingManager : MonoBehaviour
{
    public static WordFactoryBuildingManager instance;

    public Polaroid polaroid; // main polarid used in this game
    public GameObject universalCoinImage; // universal coin prefab
    public Transform coinsParent;
    public Vector2 normalCoinSize;
    public Vector2 expandedCoinSize;

    [Header("Water Coins")]
    public int numWaterCoins;
    private List<ElkoninValue> elkoninPool;
    private List<ElkoninValue> lockedPool;

    private List<WordPair> pairPool;
    private WordPair currentPair;
    private ChallengeWord currentWord;
    [HideInInspector] public UniversalCoinImage currentCoin;

    private List<UniversalCoinImage> currentCoins;
    private bool playingCoinAudio;
    private bool evaluatingCoin = false;

    private int numWins = 0;
    private int numMisses = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Update()
    {
        // dev stuff for skipping minigame
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                // play win tune
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
                // calculate and show stars
                StarAwardController.instance.AwardStarsAndExit(3);
            }
        }
    }

    void Start()
    {
        // turn on settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        PregameSetup();
    }

    private void PregameSetup()
    {
        // get pair pool from game manager
        pairPool = new List<WordPair>();
        pairPool.AddRange(ChallengeWordDatabase.GetAddDeleteWordPairs(StudentInfoSystem.GetCurrentProfile().actionWordPool));

        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive
        TigerController.instance.ResetCards();

        lockedPool = new List<ElkoninValue>();
        // add all action words excpet unlocked ones
        foreach (var coin in GameManager.instance.actionWords)
        {
            if (!StudentInfoSystem.GetCurrentProfile().actionWordPool.Contains(ChallengeWordDatabase.ElkoninValueToActionWord(coin.elkoninValue)))
                lockedPool.Add(coin.elkoninValue);
        }

        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // new pair
        currentPair = pairPool[Random.Range(0, pairPool.Count)];

        // init game delay
        yield return new WaitForSeconds(1f);

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlide, 0.25f);
        yield return new WaitForSeconds(0.5f);

        // open emerald head
        EmeraldHead.instance.animator.Play("OpenMouth");
        yield return new WaitForSeconds(1.5f);

        // choose challenge word + play enter animation
        currentWord = currentPair.word1;
        polaroid.SetPolaroid(currentWord);
        yield return new WaitForSeconds(1f);

        // play start animations
        TigerController.instance.tigerAnim.Play("TigerSwipe");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
        yield return new WaitForSeconds(0.25f);
        EmeraldHead.instance.animator.Play("EnterPolaroid");
        yield return new WaitForSeconds(0.25f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink1, 0.5f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink2, 0.5f, "glass_dink", 1.5f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink1, 0.5f, "glass_dink", 1.2f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.GlassDink2, 0.5f, "glass_dink", 0.8f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);

        // set invisible frames
        InvisibleFrameLayout.instance.SetNumberOfFrames(currentWord.elkoninCount);
        VisibleFramesController.instance.SetNumberOfFrames(currentWord.elkoninCount);
        yield return new WaitForSeconds(3f);

        // throw out real frames
        VisibleFramesController.instance.PlaceActiveFrames(polaroid.transform.localPosition);
        VisibleFramesController.instance.MoveFramesToInvisibleFrames();
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f);
        yield return new WaitForSeconds(1f);

        // show challenge word coins
        currentCoins = new List<UniversalCoinImage>();
        yield return new WaitForSeconds(0.5f);

        // show coins + add to list
        int i = 0;
        for (i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoinImage, VisibleFramesController.instance.frames[i].transform.position, Quaternion.identity, coinsParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.transform.localScale = new Vector3(0f, 0f, 1f);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
            currentCoins.Add(coin);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        // say each letter + grow coin
        foreach (var coin in currentCoins)
        {
            PlayAudioCoin(coin);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.5f);


        // say challenge word
        AudioManager.instance.PlayTalk(currentWord.audio);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        foreach (var coin in currentCoins)
        {
            coin.LerpSize(expandedCoinSize, 0.25f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        foreach (var coin in currentCoins)
        {
            coin.LerpSize(normalCoinSize, 0.25f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.25f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // transform polaroid
        // squish polaroid
        EmeraldHead.instance.animator.Play("SquishPolaroid");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidCrunch, 0.5f);
        yield return new WaitForSeconds(1.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidUnravel, 0.5f);
        yield return new WaitForSeconds(0.5f);

        polaroid.SetPolaroid(currentPair.word2);

        // unsquish polaroid 
        EmeraldHead.instance.animator.Play("UnsquishPolaroid");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.5f);
        yield return new WaitForSeconds(1.5f);

        // hide coins
        i = 0;
        foreach (var coin in currentCoins)
        {
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
        yield return new WaitForSeconds(0.5f);

        // add extra frame
        VisibleFramesController.instance.AddFrameSmooth();
        yield return new WaitForSeconds(1f);
        
        // int count = 0;
        foreach (var coin in currentCoins)
        {
            // determine index of coin value in word 2
            i = 0;
            foreach (var value in currentPair.word2.elkoninList)
            {
                if (coin.value == value && i != currentPair.index)
                {
                    coin.GetComponent<LerpableObject>().LerpPosToTransform(VisibleFramesController.instance.frames[i].transform, 0f, false);
                    break;
                }
                i++;
            }
        }
        yield return new WaitForSeconds(0.5f);
        // show coins
        i = 0;
        foreach (var coin in currentCoins)
        {
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1.5f - 0.25f * i));
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
        yield return new WaitForSeconds(1f);

        // set tag for empty frame
        VisibleFramesController.instance.frames[currentPair.index].tag = "CoinTarget";
        
        // create elkonin pool to choose water coins from
        elkoninPool = new List<ElkoninValue>();
        // add ALL values
        string[] allElkoninValues = System.Enum.GetNames(typeof(ElkoninValue));
        for (i = 0; i < allElkoninValues.Length; i++)
        {
            elkoninPool.Add((ElkoninValue)System.Enum.Parse(typeof(ElkoninValue), allElkoninValues[i]));
        }
        // remove extra values
        elkoninPool.Remove(ElkoninValue.empty_gold);
        elkoninPool.Remove(ElkoninValue.empty_silver);
        elkoninPool.Remove(ElkoninValue.COUNT);
        // remove specific swipe values
        elkoninPool.Remove(currentPair.word2.elkoninList[currentPair.index]);
        // remove any locked action word coins
        foreach(var value in lockedPool)
        {
            elkoninPool.Remove(value);
        }

        // set water coins
        WaterCoinsController.instance.SetNumberWaterCoins(numWaterCoins);

        int correctIndex = Random.Range(0, numWaterCoins);
        for (i = 0; i < numWaterCoins; i++)
        {
            if (i == correctIndex)
            {
                WaterCoinsController.instance.waterCoins[i].SetValue(currentPair.word2.elkoninList[currentPair.index]);
            }
            else 
            {
                // get random value
                ElkoninValue value = elkoninPool[Random.Range(0, elkoninPool.Count)];
                elkoninPool.Remove(value);

                WaterCoinsController.instance.waterCoins[i].SetValue(value);
            }
        }
        yield return new WaitForSeconds(0.5f);
        
        // reveal water coins
        WaterCoinsController.instance.ShowWaterCoins();
        yield return new WaitForSeconds(1f);

        // make frame wiggle
        ToggleEmptyFrameWiggle(true);

        // turn on raycaster
        WordFactoryBuildingRaycaster.instance.isOn = true;
        evaluatingCoin = false;
    }

    public void EvaluateCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SelectBoop, 0.5f);

        // stop wiggle
        ToggleEmptyFrameWiggle(false);

        // turn off raycaster
        WordFactoryBuildingRaycaster.instance.isOn = false;
        
        // return coins to position (except current coin)
        currentCoin = coin;

        // win
        if (coin.value == currentPair.word2.elkoninList[currentPair.index])
        {
            numWins++;
            StartCoroutine(PostRound(true));
        }
        // lose 
        else
        {
            numMisses++;
            currentCoin = coin;
            StartCoroutine(PostRound(false));
        }
    }

    private IEnumerator PostRound(bool win)
    {
        // move current coin
        currentCoin.GetComponent<LerpableObject>().LerpPosition(VisibleFramesController.instance.frames[currentPair.index].transform.position, 0.25f, false);
        currentCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
        yield return new WaitForSeconds(2f);

        // win round
        if (win)
        {
            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // add coin to list
            currentCoins.Add(currentCoin);
            yield return new WaitForSeconds(1f);

            // say new challenge word
            AudioManager.instance.PlayTalk(currentPair.word2.audio);
            foreach (var coin in currentCoins)
            {
                coin.LerpSize(expandedCoinSize, 0.25f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(1f);
            foreach (var coin in currentCoins)
            {
                coin.LerpSize(normalCoinSize, 0.25f);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        // lose round
        else
        {
            // return coin to frame
            currentCoin = null;
            WaterCoinsController.instance.ReturnWaterCoins();

            // play incorrect sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);
            yield return new WaitForSeconds(1f);
        }

        // eat the polaroid
        EmeraldHead.instance.animator.Play("EatPolaroid");
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidCrunch, 0.5f);
        yield return new WaitForSeconds(1.5f);

        // award card to correct person
        if (win)
        {
            WinCardsController.instance.AddPolaroid();
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 0.5f);
        }
        else
        {
            TigerController.instance.AddTigerPolaroid();
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
        }
        yield return new WaitForSeconds(1f);

        // reset water coins
        WaterCoinsController.instance.ResetWaterCoins();

        // remove coins and frames
        VisibleFramesController.instance.RemoveFrames();
        foreach (var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        }
        yield return new WaitForSeconds(1f);

        // remove word coins
        currentCoins.Remove(currentCoin);
        foreach (var coin in currentCoins)
        {
            Destroy(coin);
        }
        currentCoins.Clear();

        // win or lose game ?
        if (numWins >= 3)
            StartCoroutine(WinRoutine());
        else if (numMisses >= 3)
            StartCoroutine(LoseRoutine());
        else 
            StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        print ("you win!");

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        // show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private int CalculateStars()
    {
        if (numMisses <= 0)
            return 3;
        else if (numMisses > 0 && numMisses <= 2)
            return 2;
        else
            return 1;
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(1f);
        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }

    public void ReturnCoinsToFrame()
    {
        int count = 0;
        foreach (var coin in currentCoins)
        {
            if (coin != currentCoin)
            {
                coin.GetComponent<LerpableObject>().LerpPosition(InvisibleFrameLayout.instance.frames[count].transform.position, 0.25f, false);
            }
            count++;
        }
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin) || WaterCoinsController.instance.waterCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);

        yield return new WaitForSeconds(0.9f);
        coin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);

        playingCoinAudio = false;
    }

    public void ToggleEmptyFrameWiggle(bool opt)
    {
        if (opt)
        {
            VisibleFramesController.instance.frames[currentPair.index].GetComponent<WiggleController>().StartWiggle();
            VisibleFramesController.instance.frames[currentPair.index].GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
        else
        {
            VisibleFramesController.instance.frames[currentPair.index].GetComponent<WiggleController>().StopWiggle();
            VisibleFramesController.instance.frames[currentPair.index].GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }
}
