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

    private List<ChallengeWord> globalWordList;
    private List<ChallengeWord> unusedWordList;
    private List<ChallengeWord> usedWordList;

    private ChallengeWord currentWord;
    private List<UniversalCoinImage> currentCoins;
    private bool playingCoinAudio;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        PregameSetup();
    }

    private void PregameSetup()
    {
        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive
        TigerController.instance.ResetCards();

        // get global challenge word list
        ChallengeWordDatabase.InitCreateGlobalList(true);
        globalWordList = ChallengeWordDatabase.globalChallengeWordList;
        // unused list init
        usedWordList = new List<ChallengeWord>();
        // used list init
        unusedWordList = new List<ChallengeWord>();
        unusedWordList.AddRange(globalWordList);



        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // init game delay
        yield return new WaitForSeconds(0.5f);

        // open emerald head
        EmeraldHead.instance.animator.Play("OpenMouth");
        yield return new WaitForSeconds(1.5f);

        // choose challenge word + play enter animation
        currentWord = GetNewChallengeWord();
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

        yield return new WaitForSeconds(1f);

        // say each letter + glow / grow coin
        foreach (var coin in currentCoins)
        {
            GlowAndPlayAudioCoin(coin);
            yield return new WaitForSeconds(2f);
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


        
    }

    public void GlowAndPlayAudioCoin(UniversalCoinImage coin)
    {
        if (playingCoinAudio)
            return;

        if (currentCoins.Contains(coin))
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

        yield return new WaitForSeconds(1.5f);
        coin.LerpSize(normalCoinSize, 0.25f);
        coin.ToggleGlowOutline(false);

        playingCoinAudio = false;
    }

    private ChallengeWord GetNewChallengeWord()
    {
        // restock unused list
        if (unusedWordList.Count <= 0)
        {
            unusedWordList = new List<ChallengeWord>();
            unusedWordList.AddRange(globalWordList);

            usedWordList.Clear();
        }   
        
        // get random word
        int index = Random.Range(0, unusedWordList.Count);
        ChallengeWord word = unusedWordList[index];

        // update lists
        usedWordList.Add(word);
        unusedWordList.Remove(word);

        return word;
    }
}
