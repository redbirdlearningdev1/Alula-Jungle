using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintingGameManager : MonoBehaviour
{
    public static PrintingGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;

    public bool glowCorrectCoin = false;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;

    [HideInInspector] public ActionWordEnum correctValue;
    private int timesMissed = 0;
    private int timesCorrect = 0;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        if (!instance)
        {
            instance = this;
        }

        // get mapID
        mapID = GameManager.instance.mapID;
    }

    void Start()
    {
        PregameSetup();
    }

    private void PregameSetup()
    {
        // reset game parts
        PrintingRayCaster.instance.isOn = false;
        BallsController.instance.ResetBalls();
        PirateRopeController.instance.ResetRope();
        ParrotController.instance.interactable = false;

        // add ambiance sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.SeaAmbiance, 0.25f, "sea_ambiance");

        globalCoinPool = new List<ActionWordEnum>();

        // Create Global Coin List
        if (mapID != MapIconIdentfier.None)
        {
            globalCoinPool.AddRange(StudentInfoSystem.GetCurrentProfile().actionWordPool);
        }
        else
        {
            globalCoinPool.AddRange(GameManager.instance.GetGlobalActionWordList());
        }
        
        unusedCoinPool = new List<ActionWordEnum>();
        unusedCoinPool.AddRange(globalCoinPool);

        // start game
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        // reset rope
        PirateRopeController.instance.ResetRope();

        // get correct value
        int correctIndex = Random.Range(0, BallsController.instance.balls.Count);

        // make new used word list and add current correct word
        usedCoinPool = new List<ActionWordEnum>();

        // set ball values
        int i = 0;
        foreach (Ball ball in BallsController.instance.balls)
        {
            // set ball
            ActionWordEnum value = GetUnusedWord();
            ball.SetValue(value);

            // find correct value
            if (i == correctIndex)
            {
                correctValue = value;
                // for testing purposes
                if (glowCorrectCoin)
                    ImageGlowController.instance.SetImageGlow(ball.GetComponent<Image>(), true, GlowValue.glow_1_00);
            }
            else
            {
                // for testing purposes
                if (glowCorrectCoin)
                    ImageGlowController.instance.SetImageGlow(ball.GetComponent<Image>(), false);
            }
                
            i++;
        }

        yield return new WaitForSeconds(1f);
        BallsController.instance.ShowBalls();
        yield return new WaitForSeconds(1f);
        PirateRopeController.instance.DropRope();
        yield return new WaitForSeconds(0.5f);
        ParrotController.instance.SayAudio(correctValue);
        yield return new WaitForSeconds(1f);

        // turn on raycaster + parrot
        PrintingRayCaster.instance.isOn = true;
    }

    public bool EvaluateSelectedBall(ActionWordEnum ball)
    {
        // turn off raycaster + parrot
        PrintingRayCaster.instance.isOn = false;
        ParrotController.instance.StopAllCoroutines();
        ParrotController.instance.interactable = false;

        // correct!
        if (ball == correctValue)
        {
            StartCoroutine(CorrectBallRoutine());
            return true;
        }
        // incorrcet!
        else
        {
            StartCoroutine(IncorrectBallRoutine());
            return false;
        }
    }

    private IEnumerator CorrectBallRoutine()
    {
        timesCorrect++;

        // parrot animation
        ParrotController.instance.CelebrateAnimation(3f);

        // load cannon
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Load");
        
        // shoot cannon
        yield return new WaitForSeconds(0.25f);
        CannonController.instance.cannonAnimator.Play("Shoot");
        CannonController.instance.explosionAnimator.Play("hit");
        yield return new WaitForSeconds(0.15f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonShoot, 0.5f);
        yield return new WaitForSeconds(0.3f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonHitCoin, 0.5f);

        // drop coin into chest
        yield return new WaitForSeconds(0.5f);
        PirateRopeController.instance.printingCoin.SetActionWordValue(correctValue);
        yield return new WaitForSeconds(0.1f);
        PirateRopeController.instance.DropCoinAnimation();

        // upgrade chest
        yield return new WaitForSeconds(1.25f);
        PirateChest.instance.UpgradeChest();
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);
        AudioManager.instance.PlayCoinDrop();
        yield return new WaitForSeconds(1f);

        if (timesCorrect >= 4)
        {
            StartCoroutine(WinRoutine());
            yield break;
        }

        // reset balls and start new round
        BallsController.instance.ResetBalls();
        StartCoroutine(StartGame());
    }

    private IEnumerator IncorrectBallRoutine()
    {
        timesMissed++;

        // parrot animation
        ParrotController.instance.SadAnimation(3f);

        // load cannon
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Load");
        
        // shoot cannon
        yield return new WaitForSeconds(0.25f);
        CannonController.instance.cannonAnimator.Play("Shoot");
        CannonController.instance.explosionAnimator.Play("miss");
        yield return new WaitForSeconds(0.15f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonShoot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonFall, 0.25f);

        // raise rope
        PirateRopeController.instance.RaiseRopeAnimation();
        yield return new WaitForSeconds(2f);

        // reset balls and start new round
        BallsController.instance.ResetBalls();
        StartCoroutine(StartGame());
    }

    private IEnumerator WinRoutine()
    {
        // parrot fly!!!
        ParrotController.instance.WinAnimation();

        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);
        yield return new WaitForSeconds(2f);

        // calculate and show stars
        StarAwardController.instance.AwardStarsAndExit(CalculateStars());
    }

    private int CalculateStars()
    {
        if (timesMissed <= 0)
            return 3;
        else if (timesMissed > 0 && timesMissed <= 2)
            return 2;
        else
            return 1;
    }

    private ActionWordEnum GetUnusedWord()
    {
        // reset unused pool if empty
        if (unusedCoinPool.Count <= 0)
        {
            unusedCoinPool.Clear();
            unusedCoinPool.AddRange(globalCoinPool);
        }

        int index = Random.Range(0, unusedCoinPool.Count);
        ActionWordEnum word = unusedCoinPool[index];

        // make sure word is not being used or already successfully completed
        if (usedCoinPool.Contains(word))
        {
            unusedCoinPool.Remove(word);
            return GetUnusedWord();
        }

        unusedCoinPool.Remove(word);
        usedCoinPool.Add(word);
        return word;
    }
}
