using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordFactorySubstitutingManager : MonoBehaviour
{
    public static WordFactorySubstitutingManager instance;

    [Header("Frames")]
    [SerializeField] private HorizontalLayoutGroup frameTargetGroup;
    [SerializeField] private Transform[] framesReal; // six in-game frames
    [SerializeField] private Transform[] framesTarget;
    [SerializeField] private float[] frameSpacings;

    [Header("Coins")]
    [SerializeField] private Transform boxCoinParent;
    [SerializeField] private Transform swipeParent;
    public Transform waterCoinParent;
    [SerializeField] private List<UniversalCoinImage> waterCoins;
    private UniversalCoinImage currWaterCoin;
    [SerializeField] private List<Transform> waterCoinActivePos;
    [SerializeField] private List<Transform> waterCoinInactivePos;
    [SerializeField] private GameObject universalCoin;
    [SerializeField] private GameObject tigerSwipe;

    [Header("MoveableCanvas")]
    [SerializeField] private Transform moveableCanvas;
    [SerializeField] private Transform normalPos;
    [SerializeField] private Transform bottomPos;
    public GameObject leftArrow;

    [Header("Characters")]
    [SerializeField] private Animator redAnimator;
    [SerializeField] private Animator tigerAnimator;

    [Header("Polaroids")]
    [SerializeField] private Polaroid tigerPolaroid;
    [SerializeField] private Polaroid redPolaroid;

    [Header("Cards")]
    [SerializeField] private GameObject[] redCards;
    [SerializeField] private GameObject[] tigerCards;
    private int redCardCount = 0;
    private int tigerCardCount = 0;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos_left;
    [SerializeField] private Transform polaroidMiddlePos_left;
    [SerializeField] private Transform polaroidBottomPos_left;

    [SerializeField] private Transform polaroidStartPos_right;
    [SerializeField] private Transform polaroidMiddlePos_right;
    [SerializeField] private Transform polaroidBottomPos_right;

    [SerializeField] private Transform polaroid_middlePos;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;
    // water coins
    [SerializeField] private Vector2 waterNormalCoinSize;
    [SerializeField] private Vector2 waterExpandedCoinSize;

    // other variables
    private List<WordPair> pairPool;
    private WordPair currentPair;
    private ChallengeWord currentWord;
    private ElkoninValue currentTargetValue;
    private ChallengeWord currentTargetWord;
    private int currentSwipeIndex;

    private List<UniversalCoinImage> currentCoins;
    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;
    private bool evaluatingCoin = false;

    private List<ElkoninValue> elkoninPool;

    [Header("Testing")] // ache -> bake
    public bool overrideWord;
    public WordPair testObject;

    void Awake()
    {
        if (instance == null)
            instance = this;
        
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    private void Start() 
    {
        PregameSetup();
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

    private void PregameSetup()
    {   
        // turn on settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // add ambiance
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.RiverFlowing, 0.05f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.ForestAmbiance, 0.05f);

        // get pair pool from game manager
        pairPool = new List<WordPair>();
        pairPool.AddRange(GameManager.instance.GetSubstitutionWordPairs());

        // begin first round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        WordFactorySubstituteRaycaster.instance.isOn = false;

        // place polaroids in start pos
        tigerPolaroid.LerpScale(0f, 0.001f);
        tigerPolaroid.transform.position = polaroidStartPos_right.position;
        tigerPolaroid.LerpRotation(0, 0001f);

        redPolaroid.LerpScale(0f, 0.001f);
        redPolaroid.transform.position = polaroidStartPos_left.position;
        redPolaroid.LerpRotation(0, 0001f);

        // hide left arrow
        leftArrow.transform.localScale = new Vector3(0f, 0f, 1f);

        // set polariod challenge word
        if (overrideWord)
        {
            currentPair = testObject;
            currentWord = currentPair.word1;
        }
        else
        {
            currentPair = pairPool[Random.Range(0, pairPool.Count)];
            currentWord = currentPair.word1;
        }
        tigerPolaroid.SetPolaroid(currentWord);
        
        yield return new WaitForSeconds(2f);

        // move tiger polaroid to middle pos
        tigerPolaroid.LerpScale(1f, 1f);
        tigerPolaroid.MovePolaroid(polaroid_middlePos.position, 1f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.5f);
        yield return new WaitForSeconds(1.5f);

        // shake tiger polaroid
        tigerPolaroid.GetComponent<SpriteShakeController>().ShakeObject(2f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PolaroidRattle, 0.5f);

        // place all real frames behind tiger polaroid and make scale 0
        foreach (var frame in framesReal)
        {
            frame.transform.position = polaroid_middlePos.position;
            StartCoroutine(LerpObjectScale(frame.transform, 0f, 0f));
        }

        // activate all frames
        foreach(var frame in framesTarget)
        {
            frame.gameObject.SetActive(true);
        }
        foreach(var frame in framesReal)
        {
            frame.gameObject.SetActive(true);
        }

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            framesReal[5 - i].gameObject.SetActive(false);
            framesTarget[5 - i].gameObject.SetActive(false);
        }
        
        // move target frames
        StartCoroutine(LerpFrameSpacing(frameSpacings[currentWord.elkoninCount - 1], 0f));

        yield return new WaitForSeconds(1f);

        // show + move real frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(framesReal[i].GetComponent<Image>(), 1f, 0f));
            StartCoroutine(LerpMoveObject(framesReal[i], framesTarget[i].position, 1f));
            StartCoroutine(LerpObjectScale(framesReal[i].transform, 1f, 1f));
        }

        yield return new WaitForSeconds(1.5f);

        // place coins
        // show coins + add to list
        currentCoins = new List<UniversalCoinImage>();
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, framesReal[i].position, Quaternion.identity, boxCoinParent);
            var coin = coinObj.GetComponent<UniversalCoinImage>();
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            currentCoins.Add(coin);
            coin.ToggleVisibility(true, false);
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", (1f + 0.25f * i));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);

        // say polaroid elkonin audios
        foreach (var coin in currentCoins)
        {
            PlayAudioCoin(coin);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);

        // say polaroid word
        foreach (var coin in currentCoins)
        {
            coin.LerpSize(expandedCoinSize, 0.25f);
            yield return new WaitForSeconds(0.01f);
        }

        tigerPolaroid.LerpScale(1.1f, 0.1f);  
        AudioManager.instance.PlayTalk(tigerPolaroid.challengeWord.audio);
        yield return new WaitForSeconds(tigerPolaroid.challengeWord.audio.length + 0.5f);
        tigerPolaroid.LerpScale(1f, 0.1f);

        foreach (var coin in currentCoins)
        {
            coin.LerpSize(normalCoinSize, 0.25f);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1f);

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinRattle, 0.1f);
        yield return new WaitForSeconds(0.2f);
        // shake coin
        currentCoins[currentPair.index].ShakeCoin(2f);
        yield return new WaitForSeconds(1f);

        // tiger swipe away coin
        Vector3 swipePos = framesReal[currentPair.index].position;
        swipePos.y += 1f;
        var swipe = Instantiate(tigerSwipe, swipePos, Quaternion.identity, swipeParent);
        swipe.GetComponent<TigerSwipe>().PlayTigerSwipe();
        tigerAnimator.Play("TigerSwipe");
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.TigerSwipe, 0.5f);
        yield return new WaitForSeconds(0.25f);

        // remove coin
        currentCoins[currentPair.index].GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.3f, 1.3f), new Vector2(0f, 0f), 0.1f, 0.1f);
        redAnimator.Play("Lose");

        yield return new WaitForSeconds(2f);
        redAnimator.Play("Win");
        yield return new WaitForSeconds(0.5f);
        
        // remove coin from current coins list
        currentCoins[currentPair.index].gameObject.SetActive(false);
        currentCoins.RemoveAt(currentPair.index);

        // move tiger polaroid out of the way
        StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidMiddlePos_right.position, 1f));
        tigerPolaroid.LerpRotation(-8f, 1f);

        // bring red polaroid into game
        currentTargetWord = currentPair.word2;
        redPolaroid.SetPolaroid(currentTargetWord);
        redPolaroid.LerpScale(1f, 1f);
        redPolaroid.LerpRotation(8f, 1f);
        redPolaroid.MovePolaroid(polaroidMiddlePos_left.position, 1f);
        currentTargetValue = currentTargetWord.elkoninList[currentPair.index];
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MagicReveal, 0.1f, "red_reveal", 0.8f);
        yield return new WaitForSeconds(2f);

        // say red's word
        redPolaroid.LerpScale(1.1f, 0.1f);
        AudioManager.instance.PlayTalk(redPolaroid.challengeWord.audio);
        yield return new WaitForSeconds(redPolaroid.challengeWord.audio.length + 0.25f);
        redPolaroid.LerpScale(1f, 0.1f);

        // chose which index to be the correct 
        int correctIndex = Random.Range(0, 4);

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
        elkoninPool.Remove(currentPair.word2.elkoninList[currentPair.index]);
        elkoninPool.Remove(currentPair.word1.elkoninList[currentPair.index]);

        // place water coins
        ResetWaterCoins();
        for (int i = 0; i < 4; i++)
        {
            if (i == correctIndex)
            {
                waterCoins[i].SetValue(currentPair.word2.elkoninList[currentPair.index]);
            }
            else
            {
                waterCoins[i].SetValue(GetElkoninValue());
            }
        }

        // pan camera down to bottom position + move polaroid down
        StartCoroutine(LerpMoveObject(moveableCanvas, bottomPos.position, 1f));
        StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidBottomPos_right.position, 1f));
        StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidBottomPos_left.position, 1f));
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PanDown, 0.1f);
        yield return new WaitForSeconds(2f);

        // move water coins up
        int count = 0;
        foreach(var coin in waterCoins)
        {
            Vector3 bouncePos = waterCoinActivePos[count].position;
            bouncePos.y += 0.5f;
            waterCoins[count].GetComponent<LerpableObject>().LerpPosition(bouncePos, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
            waterCoins[count].GetComponent<LerpableObject>().LerpPosition(waterCoinActivePos[count].position, 0.2f, false);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaterRipples, 0.1f, "water_splash", (1f + 0.25f * count));
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "water_splash", (1f + 0.25f * count));
            yield return new WaitForSeconds(0.05f);
            count++;
        }
        yield return new WaitForSeconds(1f);

        // reveal left arrow
        leftArrow.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 0.25f);
        // make current frame wiggle
        framesReal[currentPair.index].GetComponent<WiggleController>().StartWiggle();

        WordFactorySubstituteRaycaster.instance.isOn = true;
    }

    private ElkoninValue GetElkoninValue()
    {
        int index = Random.Range(0, elkoninPool.Count);
        ElkoninValue value = elkoninPool[index];
        elkoninPool.RemoveAt(index);
        return value;
    }

    private IEnumerator LerpMoveObject(Transform obj, Vector3 target, float lerpTime)
    {
        Vector3 start = obj.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > lerpTime)
            {
                obj.position = target;
                break;
            }

            Vector3 tempPos = Vector3.Lerp(start, target, timer / lerpTime);
            obj.position = tempPos;
            yield return null;
        }
    }

    private IEnumerator LerpImageAlpha(Image image, float targetAlpha, float totalTime)
    {
        float start = image.color.a;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                image.color = new Color(1f, 1f, 1f, targetAlpha);
                break;
            }
            
            float tempAlpha = Mathf.Lerp(start, targetAlpha, timer / totalTime);
            image.color = new Color(1f, 1f, 1f, tempAlpha);
            yield return null;
        }
    }

    private IEnumerator LerpFrameSpacing(float target, float totalTime)
    {
        float start = frameTargetGroup.spacing;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                frameTargetGroup.spacing = target;
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            frameTargetGroup.spacing = temp;
            yield return null;
        }
    }

    private IEnumerator LerpObjectScale(Transform obj, float target, float totalTime)
    {
        float start = obj.localScale.x;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                obj.localScale = new Vector3(target, target, 1f);
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            obj.localScale = new Vector3(temp, temp, 1f);
            yield return null;
        }
    }

    public void PlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        // check lists
        if (currentCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin));
        }
        // water coins
        else if (waterCoins.Contains(coin))
        {
            StartCoroutine(PlayAudioCoinRoutine(coin, true));
        }
    }

    private IEnumerator PlayAudioCoinRoutine(UniversalCoinImage coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        // water coin differential
        if (!waterCoin) coin.LerpSize(expandedCoinSize, 0.25f);
        else  coin.LerpSize(waterExpandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1f);
        // return if current water coin
        if (coin == currWaterCoin)
        {
            playingCoinAudio = false;
            yield break;
        }

        // water coin differential
        if (!waterCoin) coin.LerpSize(normalCoinSize, 0.25f);
        else  coin.LerpSize(waterNormalCoinSize, 0.25f);


        playingCoinAudio = false;
    }

    public void ReturnWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            if (waterCoins[i] != currWaterCoin)
            {
                StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinActivePos[i].position, 0.25f));
                waterCoins[i].LerpSize(waterNormalCoinSize, 0.2f);
            }
        }
    }

    public void ResetWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            if (waterCoins[i] != currWaterCoin)
            {
                StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinInactivePos[i].position, 0.25f));
                waterCoins[i].LerpSize(waterNormalCoinSize, 0.2f);
            }
        }
    }

    public void EvaluateWaterCoin(UniversalCoinImage coin)
    {
        if (evaluatingCoin)
            return;
        evaluatingCoin = true;

        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SelectBoop, 0.5f);

        // remove left arrow
        leftArrow.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        // make current frame stop wiggle
        framesReal[currentPair.index].GetComponent<WiggleController>().StopWiggle();

        if (coin.value == currentTargetValue)
        {
            currWaterCoin = coin;

            // scale water coin correctly
            coin.LerpSize(normalCoinSize, 0.25f);

            // lerp coin to frame position
            StartCoroutine(LerpMoveObject(coin.transform, framesReal[currentPair.index].position, 0.25f));
            StartCoroutine(PostRound(true));
        }
        else
        {
            currWaterCoin = coin;

            // scale water coin correctly
            coin.LerpSize(normalCoinSize, 0.25f);

            // lerp coin to frame position
            StartCoroutine(LerpMoveObject(coin.transform, framesReal[currentPair.index].position, 0.25f));
            StartCoroutine(PostRound(false));
        }

        // return water coins
        WordFactorySubstitutingManager.instance.ResetWaterCoins();
    }

    private IEnumerator PostRound(bool win)
    {
        WordFactorySubstituteRaycaster.instance.isOn = false;

        yield return new WaitForSeconds(1.5f);

        if (win)
        {   
            numWins++;

            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            // slide tiger polaroid under red polaroid
            tigerPolaroid.transform.SetAsFirstSibling();
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, redPolaroid.transform.position, 0.2f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_left.position, 0.5f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_left.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
            yield return new WaitForSeconds(1.5f);

            // play correct animations
            redAnimator.Play("Win");
            tigerAnimator.Play("TigerLose");

            // give red one polaroid
            redCards[redCardCount].SetActive(true);
            // animate card init
            switch (redCardCount)
            {
                case 0:
                    redCards[redCardCount].GetComponent<Animator>().Play("Card1");
                    break;
                case 1:
                    redCards[redCardCount].GetComponent<Animator>().Play("Card2");
                    break;
                case 2:
                    redCards[redCardCount].GetComponent<Animator>().Play("Card3");
                    break;
            }
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 0.5f);
            redCardCount++;
        }
        else
        {
            numMisses++;

            // play correct sound
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            // slide tiger polaroid under red polaroid
            redPolaroid.transform.SetAsFirstSibling();
            StartCoroutine(LerpMoveObject(redPolaroid.transform, tigerPolaroid.transform.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SmallWhoosh, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 0.5f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_right.position, 0.5f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_right.position, 0.5f));
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.MedWhoosh, 0.5f);
            yield return new WaitForSeconds(1.5f);

            // play correct animations
            redAnimator.Play("Lose");
            tigerAnimator.Play("TigerWin");

            // give red one polaroid
            tigerCards[tigerCardCount].SetActive(true);
            // animate card init
            switch (tigerCardCount)
            {
                case 0:
                    tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card1");
                    break;
                case 1:
                    tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card2");
                    break;
                case 2:
                    tigerCards[tigerCardCount].GetComponent<Animator>().Play("Card3");
                    break;
            }
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 0.5f);
            tigerCardCount++;
        }

        redAnimator.Play("Idle");
        tigerAnimator.Play("TigerIdle");

        // pan camera up
        StartCoroutine(LerpMoveObject(moveableCanvas, normalPos.position, 1f));
        // audio fx
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.PanUp, 0.5f);

        // place polaroids in start pos
        tigerPolaroid.LerpScale(0f, 0.001f);
        tigerPolaroid.transform.position = polaroidStartPos_right.position;
        tigerPolaroid.LerpRotation(0, 0001f);

        redPolaroid.LerpScale(0f, 0.001f);
        redPolaroid.transform.position = polaroidStartPos_left.position;
        redPolaroid.LerpRotation(0, 0001f);

        // get rid of coins
        foreach(var coin in currentCoins)
        {
            coin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        }
        currWaterCoin.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(1f);

        // make frames scale 0
        foreach (var frame in framesReal)
        {
            if (frame.gameObject.activeSelf)
            {
                frame.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
                StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 0f, 0.5f));
            }
        }
        yield return new WaitForSeconds (1f);

        // place all real frames behind tiger polaroid
        foreach (var frame in framesReal)
            frame.transform.position = polaroid_middlePos.position;

        // reset water coins
        currWaterCoin.transform.position = waterCoinInactivePos[0].position;
        currWaterCoin.transform.localScale = new Vector3(1f, 1f, 1f);
        currWaterCoin = null;
        ResetWaterCoins();
        yield return new WaitForSeconds(0.5f);

        // check for win
        if (numWins >= 3)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // check for win
        if (numMisses >= 3)
        {
            StartCoroutine(LoseRoutine());
            yield break;
        }

        // stop evaluating coin
        evaluatingCoin = false;

        // start next round
        StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

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
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");
        
        yield return new WaitForSeconds(1f);

        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}