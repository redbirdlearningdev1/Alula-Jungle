using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordFactoryBlendingManager : MonoBehaviour
{
    public static WordFactoryBlendingManager instance;

    [Header("Game Objects")]
    [SerializeField] private HorizontalLayoutGroup frameGroup;
    [SerializeField] private Transform[] frames; // six in-game frames
    [SerializeField] private float[] frameSpacing;
    [SerializeField] private Transform coinParent;
    [SerializeField] private GameObject universalCoin;

    [Header("Characters")]
    [SerializeField] private Animator redAnimator;
    [SerializeField] private Animator tigerAnimator;

    [Header("Polaroids")]
    [SerializeField] private Polaroid[] polaroids; // three in-game polaroids
    [SerializeField] private Transform polaroidParent;
    public Color normalGlow;

    [Header("Cards")]
    [SerializeField] private GameObject[] redCards;
    [SerializeField] private GameObject[] tigerCards;
    private int redCardCount = 0;
    private int tigerCardCount = 0;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform polaroid0Pos;
    [SerializeField] private Transform polaroid2Pos;
    [SerializeField] private Transform redPilePos;
    [SerializeField] private Transform tigerPilePos;
    [SerializeField] private Transform away0Pos;
    [SerializeField] private Transform away2Pos;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;

    // other variables
    private ChallengeWord currentWord;
    private List<ChallengeWord> currentWords;
    private Polaroid currentPolaroid;
    private List<UniversalCoin> currentCoins;
    private bool playingCoinAudio = false;
    private int numWins = 0;
    private int numMisses = 0;

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    [Header("Testing")]
    public bool overridePool;
    public List<ChallengeWord> testChallengeWords;
    public int correctIndex;

    

    private void Start() 
    {
        if (instance == null)
            instance = this;
        
        GameManager.instance.SceneInit();

        PregameSetup();
    }

    private void PregameSetup()
    {   
        // turn on settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // create word lists
        ChallengeWordDatabase.InitCreateGlobalList();
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        unusedWordList = globalWordList;
        usedWordList = new List<ChallengeWord>();

        // disable card objects
        foreach (var card in redCards)
        {
            card.SetActive(false);
        }
        foreach (var card in tigerCards)
        {
            card.SetActive(false);
        }

        // begin first round
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = false;

        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        // set correct layer
        polaroids[0].SetLayer(4);
        polaroids[1].SetLayer(0);
        polaroids[2].SetLayer(2);

        // set frames to be invisible
        frameGroup.spacing = frameSpacing[0];
        foreach (var frame in frames)
        {
            frame.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            frame.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }

        // initialize list
        currentWords = new List<ChallengeWord>();
        currentWords.Clear();

        // get current challenge words
        if (overridePool)
        {
            currentWord = testChallengeWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];

            // use override words
            int count = 0;
            foreach (var pol in polaroids)
            {
                pol.SetPolaroid(testChallengeWords[count]);
                count++;
            }
        }
        else
        {   
            // use random words
            foreach (var polaroid in polaroids)
            {
                ChallengeWord word = GetUnusedWord();
                polaroid.SetPolaroid(word);
                currentWords.Add(word);
            }

            correctIndex = Random.Range(0, 3);
            currentWord = currentWords[correctIndex];
            currentPolaroid = polaroids[correctIndex];
        }

        // show polaroids
        StartCoroutine(ShowPolaroids());
        yield return new WaitForSeconds(1.5f);

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            frames[5 - i].gameObject.SetActive(false);
        }
        
        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[currentWord.elkoninCount - 1], 0f));

        // show frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(frames[i].GetComponent<Image>(), 1f, 0.25f));
            StartCoroutine(LerpSpriteAlpha(frames[i].GetComponent<SpriteRenderer>(), 1f, 0.25f));
        }

        currentCoins = new List<UniversalCoin>();
        yield return new WaitForSeconds(0.5f);

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, frames[i].position, Quaternion.identity, coinParent);
            var coin = coinObj.GetComponent<UniversalCoin>();
            coin.ToggleVisibility(false, false);
            coin.ToggleVisibility(true, true);
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            currentCoins.Add(coin);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(StartRound());
    }

    private IEnumerator StartRound()
    {
        // play audio in frame order
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(currentCoins[i]));
            yield return new WaitForSeconds(1.6f);
        }

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
        WordFactoryRaycaster.instance.isOn = true;
    }

    private ChallengeWord GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedWordList.Count <= 0)
        {
            unusedWordList.Clear();
            unusedWordList.AddRange(globalWordList);
        }

        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];

        // make sure word is not being used
        if (usedWordList.Contains(word))
        {
            unusedWordList.Remove(word);
            return GetUnusedWord();
        }

        unusedWordList.Remove(word);
        usedWordList.Add(word);
        return word;
    }

    public bool EvaluatePolaroid(Polaroid polaroid)
    {
        WordFactoryRaycaster.instance.isOn = false;

        if (polaroid.challengeWord == currentWord)
        {
            numWins++;
            currentPolaroid = polaroid;
            StartCoroutine(CorrectPolaroidRoutine());
            return true;
        }
        // wrong answer
        numMisses++;
        StartCoroutine(FailPolaroidRoutine());
        return false;
    }

    private IEnumerator CorrectPolaroidRoutine()
    {
        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine());
        yield return new WaitForSeconds(3.5f);

        // play correct sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);

        // ####################

        // hide coins + frames
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideCoinsAndFrames());

        // animate cards away
        int count = 0;
        foreach(var polaroid in polaroids)
        {
            if (count == 0)
            {
                polaroid.MovePolaroid(away0Pos.position, 0.25f);
            }
            else if (count == 1)
            {
                polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
            }
            else if (count == 2)
            {
                polaroid.MovePolaroid(away2Pos.position, 0.25f);
            }    
            count++;
        }

        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award red new card
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
        
        // animate characters
        redAnimator.Play("Win");
        tigerAnimator.Play("TigerLose");

        yield return new WaitForSeconds(1f);

        // win game iff 3 or more rounds have been won
        if (numWins >= 3)
            StartCoroutine(WinGameRoutine());
        // else continue to next round
        else 
            StartCoroutine(NewRound());
    }

    private IEnumerator FailPolaroidRoutine()
    {
        // reveal the correct polaroid
        StartCoroutine(PolaroidRevealRoutine());
        yield return new WaitForSeconds(3.5f);

        // play incorrect sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 1f);

        // ####################

        // hide coins + frames
        yield return new WaitForSeconds(1f);
        StartCoroutine(HideCoinsAndFrames());

        // animate cards away
        int count = 0;
        foreach(var polaroid in polaroids)
        {
            if (count == 0)
            {
                polaroid.MovePolaroid(away0Pos.position, 0.25f);
            }
            else if (count == 1)
            {
                polaroid.MovePolaroid(polaroidStartPos.position, 0.25f);
            }
            else if (count == 2)
            {
                polaroid.MovePolaroid(away2Pos.position, 0.25f);
            }    
            count++;
        }

        // reset polaroids after delay
        yield return new WaitForSeconds(1f);
        ResetPolaroids(true, true);

        // award tiger new card
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
        tigerCardCount++;
        
        // animate characters
        redAnimator.Play("Lose");
        tigerAnimator.Play("TigerWin");

        yield return new WaitForSeconds(1f);

        if (numMisses >= 3)
        {
            // tiger wins!!!
            // place polaroids in start pos
            foreach (var pol in polaroids)
            {
                pol.gameObject.transform.position = polaroidStartPos.position;
            }

            yield return new WaitForSeconds(1f);

            // show stars
            StarAwardController.instance.AwardStarsAndExit(0);
        }

        StartCoroutine(NewRound());
    }

    private IEnumerator HideCoinsAndFrames()
    {
        // remove coin glows
        foreach (var coin in currentCoins)
        {
            coin.ToggleGlowOutline(false);
            yield return new WaitForSeconds(0.05f);
        }

        // move frames
        StartCoroutine(LerpFrameSpacing(frameSpacing[0], 1f));

        // hide frames
        foreach (var frame in frames)
        {
            StartCoroutine(LerpImageAlpha(frame.GetComponent<Image>(), 0f, 0.25f));
            StartCoroutine(LerpSpriteAlpha(frame.GetComponent<SpriteRenderer>(), 0f, 0.25f));
        }

        // make coins invisible
        foreach (var coin in currentCoins)
        {
            coin.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);

        // deactivate all frames
        foreach (var frame in frames)
        {
            frame.gameObject.SetActive(true);
        }
    }

    private IEnumerator WinGameRoutine()
    {
        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

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

    private IEnumerator PolaroidRevealRoutine()
    {
        // // play audio in frame order
        // for (int i = 0; i < currentWord.elkoninCount; i++)
        // {
        //     StartCoroutine(GlowAndPlayAudioCoinRoutine(currentCoins[i]));
        //     yield return new WaitForSeconds(GameManager.instance.GetGameWord(currentCoins[i].value).audio.length + 0.1f);
        // } 

        yield return new WaitForSeconds(1f);

        // read word aloud to player
        if (currentWord.audio != null)
            AudioManager.instance.PlayTalk(currentWord.audio);

        // glow coins fast
        foreach (var coin in currentCoins)
        {
            coin.ToggleGlowOutline(true);
            yield return new WaitForSeconds(0.05f);
        }

        // wait an appropriate amount of time
        if (currentWord.audio != null)
            yield return new WaitForSeconds(currentWord.audio.length + 0.25f);
        else 
            yield return new WaitForSeconds(2f);

        // reveal correct polaroid 
        currentPolaroid.ToggleGlowOutline(true);
        currentPolaroid.LerpScale(1.15f, 0.5f);
        foreach (var polaroid in polaroids)
        {
            if (!currentPolaroid.Equals(polaroid))
                polaroid.LerpScale(0.85f, 0.5f);
        }
    }

    public void ResetPolaroids(bool instant = false, bool startPos = false)
    {
        float timeLerp = 0.1f;
        if (instant)
            timeLerp = 0f;

        // reset polaroid scale
        polaroids[0].LerpScale(1f, timeLerp);
        polaroids[1].LerpScale(1f, timeLerp);
        polaroids[2].LerpScale(1f, timeLerp);

        // return to start position
        if (startPos)
        {
            polaroids[0].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidStartPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroidStartPos.position, timeLerp);
        }
        // return to original position
        else 
        {
            polaroids[0].MovePolaroid(polaroid0Pos.position, timeLerp);
            polaroids[1].MovePolaroid(polaroidLandPos.position, timeLerp);
            polaroids[2].MovePolaroid(polaroid2Pos.position, timeLerp);
        }
        

        // set appropriate parent
        polaroids[0].transform.SetParent(polaroidParent);
        polaroids[1].transform.SetParent(polaroidParent);
        polaroids[2].transform.SetParent(polaroidParent);

        // set correct layer
        polaroids[0].SetLayer(4);
        polaroids[1].SetLayer(0);
        polaroids[2].SetLayer(2);

        // remove glow
        polaroids[0].ToggleGlowOutline(false);
        polaroids[1].ToggleGlowOutline(false);
        polaroids[2].ToggleGlowOutline(false);

        // reset glow color
        polaroids[0].SetGlowColor(normalGlow);
        polaroids[1].SetGlowColor(normalGlow);
        polaroids[2].SetGlowColor(normalGlow);
    }

    public void GlowAndPlayAudioCoin(UniversalCoin coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin))
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(coin));
        }
    }

    private IEnumerator GlowAndPlayAudioCoinRoutine(UniversalCoin coin)
    {
        playingCoinAudio = true;

        // glow coin
        coin.ToggleGlowOutline(true);
        AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord(coin.value).audio);
        coin.LerpSize(expandedCoinSize, 0.25f);

        yield return new WaitForSeconds(1.5f);
        coin.LerpSize(normalCoinSize, 0.25f);
        coin.ToggleGlowOutline(false);

        playingCoinAudio = false;
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

    private IEnumerator LerpSpriteAlpha(SpriteRenderer image, float targetAlpha, float totalTime)
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
        float start = frameGroup.spacing;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > totalTime)
            {
                frameGroup.spacing = target;
                break;
            }
            float temp = Mathf.Lerp(start, target, timer / totalTime);
            frameGroup.spacing = temp;
            yield return null;
        }
    }

    private IEnumerator ShowPolaroids()
    {   
        // short 0.5s delay
        yield return new WaitForSeconds(0.5f);

        // move polaroids down
        foreach (var polaroid in polaroids)
        {
            polaroid.MovePolaroid(polaroidLandPos.position, 0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        // move each polaroid to their respective spot
        polaroids[0].MovePolaroid(polaroid0Pos.position, 0.5f);
        polaroids[2].MovePolaroid(polaroid2Pos.position, 0.5f);
    }
}