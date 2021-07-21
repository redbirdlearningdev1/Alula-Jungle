using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordFactoryBlendingManager : MonoBehaviour
{
    public static WordFactoryBlendingManager instance;

    [Header("Game Objects")]
    [SerializeField] private Polaroid[] polaroids; // three in-game polaroids
    [SerializeField] private HorizontalLayoutGroup frameGroup;
    [SerializeField] private Transform[] frames; // six in-game frames
    [SerializeField] private float[] frameSpacing;
    [SerializeField] private Transform coinParent;
    [SerializeField] private Transform polaroidParent;
    [SerializeField] private GameObject universalCoin;

    [Header("Positions")]
    [SerializeField] private Transform polaroidStartPos;
    [SerializeField] private Transform polaroidLandPos;
    [SerializeField] private Transform polaroid0Pos;
    [SerializeField] private Transform polaroid2Pos;

    [Header("Values")]
    [SerializeField] private Vector2 normalCoinSize;
    [SerializeField] private Vector2 expandedCoinSize;
    public List<ChallengeWord> testChallengeWords;

    // other variables
    public ChallengeWord currentWord;
    private List<UniversalCoin> currentCoins;
    private bool playingCoinAudio = false;
    private int numWins = 0;
    private int numMisses = 0;

    private void Start() 
    {
        if (instance == null)
            instance = this;
        
        GameManager.instance.SceneInit();

        StartCoroutine(PregameSetup());
    }

    public bool EvaluatePolaroid(Polaroid polaroid)
    {
        if (polaroid.challengeWord == currentWord)
        {
            numWins++;

            // win game iff 3 or more rounds have been won
            if (numWins >= 3)
                StartCoroutine(WinGameRoutine());
            // else continue to next round
            else
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
        yield return null;
    }

    private IEnumerator FailPolaroidRoutine()
    {
        yield return null;
    }

    private IEnumerator WinGameRoutine()
    {
        yield return null;
    }

    public void ResetPolaroids()
    {
        // reset polaroid scale
        polaroids[0].LerpScale(1f, 0.25f);
        polaroids[1].LerpScale(1f, 0.25f);
        polaroids[2].LerpScale(1f, 0.25f);

        // return to original position
        polaroids[0].MovePolaroid(polaroid0Pos.position, 0.25f);
        polaroids[1].MovePolaroid(polaroidLandPos.position, 0.25f);
        polaroids[2].MovePolaroid(polaroid2Pos.position, 0.25f);

        // set appropriate parent
        polaroids[0].transform.SetParent(polaroidParent);
        polaroids[1].transform.SetParent(polaroidParent);
        polaroids[2].transform.SetParent(polaroidParent);

        // set correct layer
        polaroids[0].SetLayer(0);
        polaroids[1].SetLayer(2);
        polaroids[2].SetLayer(4);
    }

    private IEnumerator PregameSetup()
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("WordFactoryBlending");

        // place polaroids in start pos
        foreach (var pol in polaroids)
        {
            pol.gameObject.transform.position = polaroidStartPos.position;
        }

        // set correct layer
        polaroids[0].SetLayer(0);
        polaroids[1].SetLayer(2);
        polaroids[2].SetLayer(4);

        // set frames to be invisible
        frameGroup.spacing = frameSpacing[0];
        foreach (var frame in frames)
        {
            frame.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            frame.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }

        // set each polaroid to be a challenge word
        int count = 0;
        foreach (var pol in polaroids)
        {
            pol.SetPolaroid(testChallengeWords[count]);
            count++;
        }

        // get current challenge word

        // show polaroids
        StartCoroutine(ShowPolaroids());
        yield return new WaitForSeconds(2f);

        // deactivate unneeded frames
        int unneededFrames = 6 - currentWord.elkoninCount;
        for (int i = 0; i < unneededFrames; i++)
        {
            frames[5 - i].gameObject.SetActive(false);
        }

        // show frames
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(LerpImageAlpha(frames[i].GetComponent<Image>(), 1f, 0.25f));
            StartCoroutine(LerpSpriteAlpha(frames[i].GetComponent<SpriteRenderer>(), 1f, 0.25f));
        }
        
        // move frames
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(LerpFrameSpacing(frameSpacing[currentWord.elkoninCount - 1], 0.5f));
        yield return new WaitForSeconds(0.5f);

        currentCoins = new List<UniversalCoin>();

        // show coins + add to list
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            ElkoninValue value = currentWord.elkoninList[i];
            var coinObj = Instantiate(universalCoin, frames[i].position, Quaternion.identity, coinParent);
            var coin = coinObj.GetComponent<UniversalCoin>();
            coin.SetValue(currentWord.elkoninList[i]);
            coin.SetSize(normalCoinSize);
            currentCoins.Add(coin);
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // play audio in frame order
        for (int i = 0; i < currentWord.elkoninCount; i++)
        {
            StartCoroutine(GlowAndPlayAudioCoinRoutine(currentCoins[i]));
            yield return new WaitForSeconds(1.6f);
        }

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("WordFactoryBlending");
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
