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

    private BuildingPair currentPair;
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

        GameManager.instance.SceneInit();
    }

    void Start()
    {
        PregameSetup();
    }

    private void PregameSetup()
    {
        // remove UI button
        SettingsManager.instance.ToggleWagonButtonActive(false);

        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive
        TigerController.instance.ResetCards();

        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // new pair
        currentPair = GameManager.instance.buildingPairs[Random.Range(0, GameManager.instance.buildingPairs.Count)];

        // init game delay
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
        yield return new WaitForSeconds(0.25f);
        EmeraldHead.instance.animator.Play("EnterPolaroid");

        // set invisible frames
        InvisibleFrameLayout.instance.SetNumberOfFrames(currentWord.elkoninCount);
        VisibleFramesController.instance.SetNumberOfFrames(currentWord.elkoninCount);
        yield return new WaitForSeconds(3f);

        // throw out real frames
        VisibleFramesController.instance.PlaceActiveFrames(polaroid.transform.localPosition);
        VisibleFramesController.instance.MoveFramesToInvisibleFrames();
        yield return new WaitForSeconds(1f);

        // show challenge word coins
        currentCoins = new List<UniversalCoinImage>();
        yield return new WaitForSeconds(0.5f);

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoinImage, VisibleFramesController.instance.frames[i].transform.position, Quaternion.identity, coinsParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.ToggleVisibility(false, false);
            coin.ToggleVisibility(true, true);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            currentCoins.Add(coin);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        // say each letter + glow / grow coin
        foreach (var coin in currentCoins)
        {
            GlowAndPlayAudioCoin(coin);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(0.5f);


        // say challenge word
        AudioManager.instance.PlayTalk(currentWord.audio);
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

        // transform polaroid
        // squish polaroid
        EmeraldHead.instance.animator.Play("SquishPolaroid");
        yield return new WaitForSeconds(1.5f);

        polaroid.SetPolaroid(currentPair.word2);

        // unsquish polaroid 
        EmeraldHead.instance.animator.Play("UnsquishPolaroid");
        yield return new WaitForSeconds(1.5f);

        // add extra frame
        VisibleFramesController.instance.AddFrameSmooth();
        yield return new WaitForSeconds(0.5f);
        
        int count = 0;
        foreach (var coin in currentCoins)
        {
            if (coin.value == currentPair.word2.elkoninList[count])
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(VisibleFramesController.instance.frames[count].transform, 0.75f, false);
            }
            count++;
        }
        yield return new WaitForSeconds(1f);

        // set tag for empty frame
        VisibleFramesController.instance.frames[currentPair.addIndex].tag = "CoinTarget";
        
        // create elkonin pool to choose water coins from
        elkoninPool = new List<ElkoninValue>();
        // add ALL values
        string[] allElkoninValues = System.Enum.GetNames(typeof(ElkoninValue));
        for (int i = 0; i < allElkoninValues.Length; i++)
        {
            elkoninPool.Add((ElkoninValue)System.Enum.Parse(typeof(ElkoninValue), allElkoninValues[i]));
        }
        // remove extra values
        elkoninPool.Remove(ElkoninValue.empty_gold);
        elkoninPool.Remove(ElkoninValue.empty_silver);
        elkoninPool.Remove(ElkoninValue.COUNT);
        // remove specific swipe values
        elkoninPool.Remove(currentPair.word2.elkoninList[currentPair.addIndex]);

        // set water coins
        WaterCoinsController.instance.SetNumberWaterCoins(numWaterCoins);

        int correctIndex = Random.Range(0, numWaterCoins);

        for (int i = 0; i < numWaterCoins; i++)
        {
            if (i == correctIndex)
            {
                WaterCoinsController.instance.waterCoins[i].SetValue(currentPair.word2.elkoninList[currentPair.addIndex]);
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


        // turn on raycaster
        WordFactoryBuildingRaycaster.instance.isOn = true;
        evaluatingCoin = false;
    }

    public void EvaluateCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        // turn off raycaster
        WordFactoryBuildingRaycaster.instance.isOn = false;
        
        // return coins to position (except current coin)
        currentCoin = coin;

        // print ("current coin value: " + currentCoin.value);
        // print ("value looking for: " + currentPair.word1.elkoninList[currentPair.swipeIndex]);

        // win
        if (coin.value == currentPair.word2.elkoninList[currentPair.addIndex])
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
        // win round
        if (win)
        {
            // move current coin
            currentCoin.GetComponent<LerpableObject>().LerpPosition(VisibleFramesController.instance.frames[currentPair.addIndex].transform.position, 0.25f, false);

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
        yield return new WaitForSeconds(1.5f);

        // award card to correct person
        if (win)
        {
            WinCardsController.instance.AddPolaroid();
        }
        else
        {
            TigerController.instance.AddTigerPolaroid();
        }
        yield return new WaitForSeconds(1f);

        // reset water coins
        WaterCoinsController.instance.ResetWaterCoins();

        // remove coins and frames
        VisibleFramesController.instance.MoveFramesOffScreen();
        foreach (var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().LerpPosition(new Vector2(coin.transform.position.x, coin.transform.position.y - 600f), 0.5f, false);
        }
        yield return new WaitForSeconds(1f);

        // remove coins
        currentCoins.Remove(currentCoin);
        foreach (var coin in currentCoins)
        {
            Destroy(coin.gameObject);
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

        // // update SIS
        // if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2)
        // {
        //     StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = false;
        //     StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = false;
        //     StudentInfoSystem.AdvanceStoryBeat();
        //     StudentInfoSystem.SaveStudentPlayerData();
        // }

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
        print ("you lose!");
        
        yield return new WaitForSeconds(1f);

        // // update SIS
        // if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2)
        // {
        //     // first time losing
        //     if (!StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame)
        //         StudentInfoSystem.GetCurrentProfile().firstTimeLoseChallengeGame = true;
        //     else
        //     {
        //         // every other time losing
        //         if (!StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame)
        //         {
        //             StudentInfoSystem.GetCurrentProfile().everyOtherTimeLoseChallengeGame = true;
        //         }
        //     }
        //     StudentInfoSystem.SaveStudentPlayerData();
        // }

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

    public void GlowAndPlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin) || WaterCoinsController.instance.waterCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(0.9f);
        coin.LerpSize(normalCoinSize, 0.25f);
        coin.ToggleGlowOutline(false);

        playingCoinAudio = false;
    }

    public void ToggleEmptyFrameWiggle(bool opt)
    {
        if (opt)
        {
            VisibleFramesController.instance.frames[currentPair.addIndex].GetComponent<WiggleController>().StartWiggle();
            VisibleFramesController.instance.frames[currentPair.addIndex].GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
        else
        {
            VisibleFramesController.instance.frames[currentPair.addIndex].GetComponent<WiggleController>().StopWiggle();
            VisibleFramesController.instance.frames[currentPair.addIndex].GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }
}
