using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFactoryDeletingManager : MonoBehaviour
{
    public static WordFactoryDeletingManager instance;

    public Polaroid polaroid; // main polarid used in this game
    public GameObject universalCoinImage; // universal coin prefab
    public Transform coinsParent;
    public Vector2 normalCoinSize;
    public Vector2 expandedCoinSize;

    private List<WordPair> pairPool;
    private WordPair currentPair;
    private ChallengeWord currentWord;
    private UniversalCoinImage currentCoin;

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
                AIData(StudentInfoSystem.GetCurrentProfile());
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
        currentWord = currentPair.word2;
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
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoinImage, VisibleFramesController.instance.frames[i].transform.position, Quaternion.identity, coinsParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            currentCoins.Add(coin);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);

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
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);

        // squish polaroid
        EmeraldHead.instance.animator.Play("SquishPolaroid");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidCrunch, 0.5f);
        yield return new WaitForSeconds(1.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidUnravel, 0.5f);
        yield return new WaitForSeconds(0.5f);

        polaroid.SetPolaroid(currentPair.word1);

        // unsquish polaroid 
        EmeraldHead.instance.animator.Play("UnsquishPolaroid");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EmeraldSlideShort, 0.25f);
        yield return new WaitForSeconds(1.5f);
        // say new challenge word
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        AudioManager.instance.PlayTalk(currentPair.word1.audio);
        yield return new WaitForSeconds(currentPair.word1.audio.length);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // turn on raycaster
        WordFactoryDeletingRaycaster.instance.isOn = true;
        evaluatingCoin = false;
    }

    public void EvaluateCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        // turn off raycaster
        WordFactoryDeletingRaycaster.instance.isOn = false;
        
        // return coins to position (except current coin)
        currentCoin = coin;
        WordFactoryDeletingManager.instance.ReturnCoinsToFrame();

        // win
        if (coin.value == currentPair.word2.elkoninList[currentPair.index])
        {
            // audio fx
            AudioManager.instance.PlayCoinDrop();

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
            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            // move current coin
            currentCoin.GetComponent<LerpableObject>().LerpPosition(EmeraldTigerHolder.instance.transform.position, 0.25f, false);
            currentCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.25f);
            yield return new WaitForSeconds(0.25f);

            // add coin to tiger
            EmeraldTigerHolder.instance.SetNumberCoins(numWins);

            // remove coin from list
            currentCoins.Remove(currentCoin);

            yield return new WaitForSeconds(1f);

            // remove one frame
            InvisibleFrameLayout.instance.SetNumberOfFrames(currentWord.elkoninCount - 1);

            // shrink extra frame
            VisibleFramesController.instance.frames[currentPair.index].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
            yield return new WaitForSeconds(0.2f);

            // move coins to frames
            VisibleFramesController.instance.SetNumberOfFrames(currentWord.elkoninCount - 1);
            VisibleFramesController.instance.LerpFramesToInvisibleFrames();
            int count = 0;
            foreach (var coin in currentCoins)
            {
                coin.GetComponent<LerpableObject>().LerpPosition(InvisibleFrameLayout.instance.frames[count].transform.position, 0.5f, false);
                count++;
            }
            yield return new WaitForSeconds(1f);

            // say new challenge word
            AudioManager.instance.PlayTalk(currentPair.word1.audio);
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
        }
        // lose round
        else
        {
            currentCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(0f, 0f), 0.0f);
            currentCoins.Remove(currentCoin);
            yield return new WaitForSeconds(.5f);
            foreach (var coin in currentCoins)
        {
            PlayAudioCoin(coin);
            yield return new WaitForSeconds(1f);
        }
        AudioManager.instance.PlayTalk(currentPair.word1.audio);
        yield return new WaitForSeconds(currentPair.word1.audio.length);
            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            // return coin to frame
            currentCoin = null;
            WordFactoryDeletingManager.instance.ReturnCoinsToFrame();
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

        // remove coins and frames
        VisibleFramesController.instance.RemoveFrames();
        int i = 0;
        foreach (var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.1f, "water_splash", (1f - 0.25f * i));
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "water_splash", (1f - 0.2f * i));
            yield return new WaitForSeconds(0.05f);
            i++;
        }
        yield return new WaitForSeconds(0.5f);

        // remove coins
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

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

        public void AIData(StudentPlayerData playerData)
    {
        playerData.delPlayed = playerData.delPlayed + 1;
        playerData.starsDel = CalculateStars() + playerData.starsDel;
        
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

        // show stars
        AIData(StudentInfoSystem.GetCurrentProfile());
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

        if (currentCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(0.9f);
        coin.LerpSize(normalCoinSize, 0.25f);

        playingCoinAudio = false;
    }
}
