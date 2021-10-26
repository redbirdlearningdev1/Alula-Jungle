using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintingGameManager : MonoBehaviour
{
    public static PrintingGameManager instance;

    private MapIconIdentfier mapID = MapIconIdentfier.None;

    private List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedCoinPool;
    private List<ActionWordEnum> usedCoinPool;

    private ActionWordEnum correctValue;

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
                ImageGlowController.instance.SetImageGlow(ball.GetComponent<Image>(), true, GlowValue.glow_1_00);
            }
                
            i++;
        }

        yield return new WaitForSeconds(1f);
        BallsController.instance.ShowBalls();
        yield return new WaitForSeconds(1f);
        PirateRopeController.instance.DropRope();

        // turn on raycaster
        PrintingRayCaster.instance.isOn = true;
    }

    public bool EvaluateSelectedBall(ActionWordEnum ball)
    {
        // turn off raycaster
        PrintingRayCaster.instance.isOn = false;

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
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Load");
        
        yield return new WaitForSeconds(0.5f);
        CannonController.instance.cannonAnimator.Play("Shoot");
        CannonController.instance.explosionAnimator.Play("hit");

        yield return new WaitForSeconds(0.5f);
        PirateRopeController.instance.printingCoin.SetActionWordValue(correctValue);

    }

    private IEnumerator IncorrectBallRoutine()
    {
        yield return null;
    }

    private IEnumerator WinRoutine()
    {
        yield return null;
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
