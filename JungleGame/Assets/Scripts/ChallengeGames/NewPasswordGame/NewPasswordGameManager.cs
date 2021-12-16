using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewPasswordGameManager : MonoBehaviour
{
    public static NewPasswordGameManager instance;

    [Header("Game Parts")]
    public List<UniversalCoinImage> coins;
    public Transform coinParent;
    public Transform polaroidParent;
    public Polaroid polaroid;
    public Transform tigerCharacter;
    public Transform marcusCharacter;
    public Transform brutusCharacter;

    [Header("Coin Positions")]
    public Transform coinOffScreenPos;
    public List<Transform> coinOnScreenPositions;
    public List<Transform> coinDownPositions;
    public List<Transform> coinTubePositions;

    [Header("Polaroid Positions")]
    public Transform polaroidOffScreenPos;
    public Transform polaroidOnScreenPos;

    [Header("Character Positions")]
    public Transform tigerOnScreenPos;
    public Transform tigerOffScreenPos;
    public Transform marcusOnScreenPos;
    public Transform marcusOffScreenPos;
    public Transform brutusOnScreenPos;
    public Transform brutusOffScreenPos;

    private ChallengeWord currentWord;
    private int numMisses = 0;

    // challenge word pool
    private List<ChallengeWord> wordPool;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // turn on settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

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
        // reset win cards
        WinCardsController.instance.ResetCards();

        // hide lock
        PasswordLock.instance.HideLock();

        // fill challenge word pool
        wordPool = new List<ChallengeWord>();
        wordPool.AddRange(ChallengeWordDatabase.GetChallengeWords(StudentInfoSystem.GetCurrentProfile().actionWordPool));

        // place charcters off screen
        tigerCharacter.position = tigerOffScreenPos.position;
        marcusCharacter.position = marcusOffScreenPos.position;
        brutusCharacter.position = brutusOffScreenPos.position;

        // make coins empty gold
        foreach(var coin in coins)
            coin.SetValue(ElkoninValue.empty_gold);

        StartCoroutine(NewRound(true));
    }

    private IEnumerator NewRound(bool moveBG)
    {
        // place coins off-screen
        foreach(var coin in coins)
            coin.transform.position = coinOffScreenPos.position;
            
        // place polaroid off-screen
        polaroid.transform.position = polaroidOffScreenPos.position;

        // select challenge word + reset polaroid
        currentWord = wordPool[Random.Range(0, wordPool.Count)];
        wordPool.Remove(currentWord);
        polaroid.SetPolaroid(currentWord);
        polaroid.SetPolaroidAlpha(0f, 0f);
        yield return new WaitForSeconds(1f);

        // move to new section
        if (moveBG)
        {
            PasswordLock.instance.ResetLock();
            BGManager.instance.MoveToNextSection();
            PasswordTube.instance.TurnTube();
            yield return new WaitForSeconds(2f);
            PasswordTube.instance.StopTube();
            yield return new WaitForSeconds(0.5f);

            // play walking animations
            float moveTime = 2f;
            tigerCharacter.GetComponent<Animator>().Play("tigerWalk");
            marcusCharacter.GetComponent<Animator>().Play("marcusWalkIn");
            brutusCharacter.GetComponent<Animator>().Play("brutusWalkIn");

            // move characters on screen
            tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOnScreenPos, moveTime, false);
            marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOnScreenPos, moveTime, false);
            brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOnScreenPos, moveTime, false);
            yield return new WaitForSeconds(moveTime);
        }

        // play idle animations + show lock
        tigerCharacter.GetComponent<Animator>().Play("aTigerIdle");
        marcusCharacter.GetComponent<Animator>().Play("marcusBroken");
        brutusCharacter.GetComponent<Animator>().Play("brutusBroken");
        PasswordLock.instance.ShowLock();
        yield return new WaitForSeconds(1f);

        // show polaroid
        polaroid.GetComponent<LerpableObject>().LerpPosToTransform(polaroidOnScreenPos, 0.5f, false);
        yield return new WaitForSeconds(0.2f);
        polaroid.SetPolaroidAlpha(1f, 0.2f);
        yield return new WaitForSeconds(0.5f);

        // say polaroid word
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        AudioManager.instance.PlayTalk(polaroid.challengeWord.audio);
        yield return new WaitForSeconds(polaroid.challengeWord.audio.length + 0.1f);
        polaroid.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

        // show coins
        int i = 0;
        foreach(var coin in coins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 0.8f + (0.1f * i));
            yield return new WaitForSeconds(0.1f);
            i++;
        }

        // turn on raycaster
        NewPasswordRaycaster.instance.isOn = true;
    }   

    public void ResetCoins()
    {
        int i = 0;
        foreach(var coin in coins)
        {
            if (!PasswordTube.instance.tubeCoins.Contains(coin))
            {
                coin.GetComponent<LerpableObject>().LerpPosToTransform(coinOnScreenPositions[i], 0.25f, false);
                i++;
            }
        }
    }

    public void EvaluateCoins()
    {
        // turn off raycaster
        NewPasswordRaycaster.instance.isOn = false;
        StartCoroutine(EvaluateCoinsRoutine());
    }

    private IEnumerator EvaluateCoinsRoutine()
    {
        // small delay
        yield return new WaitForSeconds(1f);

        // get current coins (not in tube)
        List<Transform> extraCoins = new List<Transform>();
        foreach (Transform t in coinParent)
            extraCoins.Add(t);

        bool winGame = false;

        // determine if correct num of coins
        if (currentWord.elkoninCount == PasswordTube.instance.tubeCoins.Count)
        {
            winGame = true;

            // play right audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

            // unlock lock
            PasswordLock.instance.Unlock();

            // add card to win cards
            WinCardsController.instance.AddPolaroid();

            // coin animations
            PasswordTube.instance.CorrectCoinsAnimation();

            yield return new WaitForSeconds(1f);
            
            // remove extra coins
            RemoveExtraCoins(extraCoins);

            // determine if win
            if (WinCardsController.instance.WinGame())
            {
                StartCoroutine(WinRoutine());
                yield break;
            }
            else
            {
                // tiger run away
                tigerCharacter.GetComponent<Animator>().Play("aTigerTurn");
                yield return new WaitForSeconds(0.25f);
                tigerCharacter.GetComponent<LerpableObject>().LerpPosToTransform(tigerOffScreenPos, 1f, false);
                yield return new WaitForSeconds(1f);

                // monkies walk way
                marcusCharacter.GetComponent<Animator>().Play("marcusTurn");
                brutusCharacter.GetComponent<Animator>().Play("brutusTurn");
                yield return new WaitForSeconds(0.25f);
                marcusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(marcusOffScreenPos, 2f, false);
                brutusCharacter.GetComponent<LerpableObject>().LerpPosToTransform(brutusOffScreenPos, 2f, false);
                yield return new WaitForSeconds(2f);

                // remove lock
                PasswordLock.instance.HideLock();
            }
        }
        else
        {
            numMisses++;

            // play wrong audio
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WrongChoice, 0.5f);

            PasswordLock.instance.UpgradeLock();

            // coin animation reset
            PasswordTube.instance.RemoveAllCoins();

            yield return new WaitForSeconds(0.5f);

            // remove extra coins
            RemoveExtraCoins(extraCoins);

            // determine if lose
            if (PasswordLock.instance.LoseGame())
            {
                StartCoroutine(LoseRoutine());
                yield break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // begin next round
        StartCoroutine(NewRound(winGame));
    }

    private void RemoveExtraCoins(List<Transform> extraCoins)
    {
        int i = 0;
        foreach (var coin in extraCoins)
        {
            coin.GetComponent<LerpableObject>().LerpPosToTransform(coinDownPositions[i], 0.2f, false);
            i++;
        }
    }

    private IEnumerator WinRoutine()
    {
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

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
        yield return new WaitForSeconds(2f);

        // show stars
        StarAwardController.instance.AwardStarsAndExit(0);
    }
}
