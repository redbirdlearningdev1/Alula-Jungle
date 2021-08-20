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
    [SerializeField] private List<UniversalCoin> waterCoins;
    private UniversalCoin currWaterCoin;
    [SerializeField] private List<Transform> waterCoinPos;
    [SerializeField] private GameObject universalCoin;
    [SerializeField] private GameObject tigerSwipe;

    [Header("MoveableCanvas")]
    [SerializeField] private Transform moveableCanvas;
    [SerializeField] private Transform normalPos;
    [SerializeField] private Transform bottomPos;

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
    private ChallengeWord currentWord;
    private ElkoninValue currentTargetValue;
    private ChallengeWord currentTargetWord;
    private int currentSwipeIndex;

    private List<UniversalCoin> currentCoins;
    private int numWins = 0;
    private int numMisses = 0;
    private bool playingCoinAudio = false;

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    [Header("Testing")] // ache -> bake
    public bool overrideWord;
    public ChallengeWord testChallengeWord;
    public ChallengeWord targetChallengeWord;
    public List<ElkoninValue> coinOptions;
    public int swipeIndex;

    private void Start() 
    {
        if (instance == null)
            instance = this;
        
        GameManager.instance.SceneInit();

        PregameSetup();
    }

    void Update()
    {
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                StartCoroutine(WinRoutine());
            }
        }
    }

    private void PregameSetup()
    {   
        // create word lists
        ChallengeWordDatabase.InitCreateGlobalList();
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

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

        // set polariod challenge word
        if (overrideWord)
        {
            currentWord = testChallengeWord;
            tigerPolaroid.SetPolaroid(currentWord);
        }
        
        yield return new WaitForSeconds(2f);

        // move tiger polaroid to middle pos
        tigerPolaroid.LerpScale(1f, 1.5f);
        tigerPolaroid.MovePolaroid(polaroid_middlePos.position, 1.5f);

        yield return new WaitForSeconds(2f);

        // shake tiger polaroid
        tigerPolaroid.GetComponent<SpriteShakeController>().ShakeObject(1f);

        // place all real frames behind tiger polaroid and make scale 0
        foreach (var frame in framesReal)
        {
            frame.transform.position = polaroid_middlePos.position;
            StartCoroutine(LerpObjectScale(frame.transform, 0f, 0f));
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

        yield return new WaitForSeconds(0.5f);

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
        currentCoins = new List<UniversalCoin>();
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, framesReal[i].position, Quaternion.identity, boxCoinParent);
            var coin = coinObj.GetComponent<UniversalCoin>();
            coin.ToggleVisibility(false, false);
            coin.ToggleVisibility(true, true);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            coin.SetLayer(1);
            currentCoins.Add(coin);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1f);


        // say polaroid word
        foreach (var coin in currentCoins)
        {
            GlowAndPlayAudioCoin(coin);
            yield return new WaitForSeconds(1.5f);
        }
        yield return new WaitForSeconds(1f);

        // shake coin
        currentCoins[swipeIndex].ShakeCoin(1f);
        yield return new WaitForSeconds(0.25f);

        // tiger swipe away coin
        var swipe = Instantiate(tigerSwipe, framesReal[swipeIndex].position, Quaternion.identity, swipeParent);
        swipe.GetComponent<TigerSwipe>().PlayTigerSwipe();
        tigerAnimator.Play("TigerSwipe");
        yield return new WaitForSeconds(0.25f);

        // remove coin
        currentCoins[swipeIndex].ToggleVisibility(false, true);
        redAnimator.Play("Lose");

        yield return new WaitForSeconds(1.5f);
        redAnimator.Play("Win");
        
        // remove coin from current coins list
        currentCoins[swipeIndex].gameObject.SetActive(false);
        currentCoins.RemoveAt(swipeIndex);

        // place water coins
        for (int i = 0; i < 4; i++)
        {
            waterCoins[i].SetValue(coinOptions[i]);
            StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinPos[i].position, 0f));
            waterCoins[i].SetLayer(2);
        }

        // pan camera down to bottom position + move polaroid down
        StartCoroutine(LerpMoveObject(moveableCanvas, bottomPos.position, 1f));
        StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidBottomPos_right.position, 1f));
        tigerPolaroid.LerpRotation(-8f, 1f);
        yield return new WaitForSeconds(2f);

        // bring red polaroid into game
        currentTargetWord = targetChallengeWord;
        redPolaroid.SetPolaroid(targetChallengeWord);
        redPolaroid.LerpScale(1f, 1.5f);
        redPolaroid.LerpRotation(8f, 1.5f);
        redPolaroid.MovePolaroid(polaroidBottomPos_left.position, 1.5f);
        currentTargetValue = currentTargetWord.elkoninList[swipeIndex];

        yield return new WaitForSeconds(1.5f);

        WordFactorySubstituteRaycaster.instance.isOn = true;
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

    public void GlowAndPlayAudioCoin(UniversalCoin coin)
    {
        if (playingCoinAudio)
            return;

        // check lists
        if (currentCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
        }
        // water coins
        else if (waterCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin, true));
        }
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoin coin, bool waterCoin = false)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        // water coin differential
        if (!waterCoin) coin.LerpSize(expandedCoinSize, 0.25f);
        else  coin.LerpSize(waterExpandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1.5f);
        // return if current water coin
        if (coin == currWaterCoin)
        {
            coin.ToggleGlowOutline(false);
            playingCoinAudio = false;
            yield break;
        }

        // water coin differential
        if (!waterCoin) coin.LerpSize(normalCoinSize, 0.25f);
        else  coin.LerpSize(waterNormalCoinSize, 0.25f);

        coin.ToggleGlowOutline(false);

        playingCoinAudio = false;
    }

    public void ResetWaterCoins()
    {
        for (int i = 0; i < 4; i++)
        {
            if (waterCoins[i] != currWaterCoin)
            {
                StartCoroutine(LerpMoveObject(waterCoins[i].transform, waterCoinPos[i].position, 0.1f));
                waterCoins[i].LerpSize(waterNormalCoinSize, 0.25f);
            }
        }
    }

    public void EvaluateWaterCoin(UniversalCoin coin)
    {
        if (coin.value == currentTargetValue)
        {
            currWaterCoin = coin;

            // scale water coin correctly
            coin.LerpSize(normalCoinSize, 0.25f);

            // lerp coin to frame position
            StartCoroutine(LerpMoveObject(coin.transform, framesReal[swipeIndex].position, 0.25f));
            StartCoroutine(PostRound(true));
        }
        else
        {
            // scale water coin correctly
            coin.LerpSize(normalCoinSize, 0.25f);

            // lerp coin to frame position
            StartCoroutine(LerpMoveObject(coin.transform, framesReal[swipeIndex].position, 0.25f));
            StartCoroutine(PostRound(true));

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

            // slide tiger polaroid under red polaroid
            tigerPolaroid.transform.SetAsFirstSibling();
            tigerPolaroid.SetLayer(0);
            redPolaroid.SetLayer(2);
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, redPolaroid.transform.position, 0.8f));
            yield return new WaitForSeconds(1f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 1f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 1f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_left.position, 1f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_left.position, 1f));
            yield return new WaitForSeconds(1f);

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
            redCardCount++;
        }
        else
        {
            numMisses++;

            // slide tiger polaroid under red polaroid
            redPolaroid.transform.SetAsFirstSibling();
            redPolaroid.SetLayer(0);
            tigerPolaroid.SetLayer(2);
            StartCoroutine(LerpMoveObject(redPolaroid.transform, tigerPolaroid.transform.position, 0.8f));
            yield return new WaitForSeconds(1f);

            // shink and move both polaroids towards red
            StartCoroutine(LerpObjectScale(redPolaroid.transform, 0f, 1f));
            StartCoroutine(LerpObjectScale(tigerPolaroid.transform, 0f, 1f));
            StartCoroutine(LerpMoveObject(redPolaroid.transform, polaroidStartPos_right.position, 1f));
            StartCoroutine(LerpMoveObject(tigerPolaroid.transform, polaroidStartPos_right.position, 1f));
            yield return new WaitForSeconds(1f);

            // play correct animations
            redAnimator.Play("Lose");
            tigerAnimator.Play("TigerWin");

            // give red one polaroid
            tigerCards[tigerCardCount].SetActive(true);
            // animate card init
            switch (redCardCount)
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
            tigerCardCount++;
        }

        redAnimator.Play("Idle");
        tigerAnimator.Play("TigerIdle");

        // pan camera up
        StartCoroutine(LerpMoveObject(moveableCanvas, normalPos.position, 1f));

        // place polaroids in start pos
        tigerPolaroid.LerpScale(0f, 0.001f);
        tigerPolaroid.transform.position = polaroidStartPos_right.position;
        tigerPolaroid.LerpRotation(0, 0001f);

        redPolaroid.LerpScale(0f, 0.001f);
        redPolaroid.transform.position = polaroidStartPos_left.position;
        redPolaroid.LerpRotation(0, 0001f);

        // place all real frames behind tiger polaroid and make scale 0
        foreach (var frame in framesReal)
        {
            frame.transform.position = polaroid_middlePos.position;
            StartCoroutine(LerpObjectScale(frame.transform, 0f, 1f));
            StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 1f, 1f));
        }
        yield return new WaitForSeconds (1f);

        // get rid of coins
        foreach(var coin in currentCoins)
        {
            coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.05f);
        }
        // reset water coins
        currWaterCoin = null;
        ResetWaterCoins();
        yield return new WaitForSeconds(0.5f);
        foreach (var waterCoin in waterCoins)
        {
            waterCoin.SetValue(ElkoninValue.empty_silver);
        }
        yield return new WaitForSeconds(1f);

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

        // start next round
        StartCoroutine(NewRound());
    }

    private IEnumerator WinRoutine()
    {
        print ("you win!");
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(1f);

        // update SIS
        if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2)
        {
            StudentInfoSystem.currentStudentPlayer.firstTimeLoseChallengeGame = false;
            StudentInfoSystem.currentStudentPlayer.everyOtherTimeLoseChallengeGame = false;
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }

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
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");
        
        yield return new WaitForSeconds(1f);

        // update SIS
        if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.GorillaVillage_challengeGame_2)
        {
            // first time losing
            if (!StudentInfoSystem.currentStudentPlayer.firstTimeLoseChallengeGame)
                StudentInfoSystem.currentStudentPlayer.firstTimeLoseChallengeGame = true;
            else
            {
                // every other time losing
                if (!StudentInfoSystem.currentStudentPlayer.everyOtherTimeLoseChallengeGame)
                {
                    StudentInfoSystem.currentStudentPlayer.everyOtherTimeLoseChallengeGame = true;
                }
            }
            StudentInfoSystem.SaveStudentPlayerData();
        }

        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}