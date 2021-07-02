using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintingGameManager : MonoBehaviour
{
    public static PrintingGameManager instance;


    [SerializeField] private chester chest;
    [SerializeField] private ParrotController Parrot;
    [SerializeField] private ParrotController Explode;
    [SerializeField] private PirateRopeController rope;
    [SerializeField] private CannonController cannon;
    [SerializeField] private PrintingCoin PrintingCoin;
    [SerializeField] private PrintingRayCaster caster;


    private bool gameSetup = false;

    private List<ActionWordEnum> globalBallPool;
    private List<ActionWordEnum> unusedBallPool;

    [SerializeField] private List<BallController> Balls;


    private List<BallController> allBalls = new List<BallController>();
    private int selectedIndex;
    public BallController selectedBall;

    private int winCount = 0;

    private bool firstTimePlaying = true;


    void Awake()
    {
        // every scene must call this in Awake()

        GameManager.instance.SceneInit();

        if (!instance)
        {

            instance = this;

        }

        // dev object stuff
        //devObject.SetActive(GameManager.instance.devModeActivated);
        Debug.Log("In Pregame");
        PregameSetup();
        StartCoroutine(StartGame(0));

    }

    void Update()
    {

    }



    public bool EvaluateSelectedBall(ActionWordEnum ball, BallController correctBall)
    {
        Debug.Log(selectedBall.type);
        
        StartCoroutine(BallMovesShakes(correctBall.getChoice()));
        if (ball == selectedBall.type)
        {
            // success! go on to the next row or win game if on last row
            Debug.Log("YOU DID IT");
            
            if (winCount < 5)
            {
                winCount++;
                StartCoroutine(BallSuccessRoutine(correctBall));
            }
            else
            {
                Debug.Log("YOU DID IT AGAIn");
                StartCoroutine(WinRoutine());
            }

            return true;
        }
        StartCoroutine(BallFailRoutine(correctBall));
        return false;
    }

    private IEnumerator BallMovesShakes(int Index)
    {
        //selectedIndex = 2;
        if (Index == 1)
        {
            Balls[0].MoveOver();
            yield return new WaitForSeconds(1f);
            StartCoroutine(BallZeroShake());
            yield return new WaitForSeconds(.25f);
            StartCoroutine(BallTwoShakeIn());
            yield return new WaitForSeconds(.25f);
            StartCoroutine(BallThreeShakeIn());

            


        }
        else if (Index == 2)
        {
            
            Balls[1].MoveOver();
            StartCoroutine(DelayZero());
            yield return new WaitForSeconds(1f);

            StartCoroutine(BallOneShake());
            yield return new WaitForSeconds(.5f);
            StartCoroutine(BallZeroShake());
            StartCoroutine(BallThreeShakeIn());



        }
        else if (Index == 3)
        {
            Balls[2].MoveOver();
            StartCoroutine(DelayZero());
            StartCoroutine(DelayOne());
            yield return new WaitForSeconds(1f);

            StartCoroutine(BallTwoShake());
            yield return new WaitForSeconds(.5f);
            StartCoroutine(BallOneShake());
            yield return new WaitForSeconds(.5f);
            StartCoroutine(BallZeroShake());
        }
        yield return new WaitForSeconds(0f);
    }
    private IEnumerator BallZeroShake()
    {
        Balls[0].ShakeF();
        yield return new WaitForSeconds(.5f);
        Balls[0].ShakeB();
        yield return new WaitForSeconds(.5f);
        Balls[0].MoveOver();
    }
    private IEnumerator DelayZero()
    {
        yield return new WaitForSeconds(.5f);
        Balls[0].MoveOver();
    }
    private IEnumerator DelayOne()
    {
        yield return new WaitForSeconds(.4f);
        Balls[1].MoveOver();
    }
    private IEnumerator DelayTwo()
    {
        yield return new WaitForSeconds(.5f);
        Balls[2].MoveOver();
    }
    private IEnumerator BallOneShake()
    {
        Balls[1].ShakeF();
        yield return new WaitForSeconds(.5f);
        Balls[1].ShakeB();

        yield return new WaitForSeconds(.5f);
        Balls[1].MoveOver();
    }
    private IEnumerator BallTwoShake()
    {
        Balls[2].ShakeF();
        yield return new WaitForSeconds(.5f);
        Balls[2].ShakeB();
        yield return new WaitForSeconds(.5f);
        Balls[2].MoveOver();
    }
    private IEnumerator BallThreeShake()
    {
        Balls[3].ShakeF();
        yield return new WaitForSeconds(.5f);
        Balls[3].ShakeB();
        yield return new WaitForSeconds(.5f);
        Balls[3].MoveOver();
    }
    private IEnumerator BallOneShakeIn()
    {
        Balls[1].ShakeInF();
        yield return new WaitForSeconds(.5f);
        Balls[1].ShakeInB();
        yield return new WaitForSeconds(.5f);
        Balls[1].MoveBack();
    }
    private IEnumerator BallTwoShakeIn()
    {
        Balls[2].ShakeInF();
        yield return new WaitForSeconds(.5f);
        Balls[2].ShakeInB();
        yield return new WaitForSeconds(.5f);
        Balls[2].MoveBack();

    }
    private IEnumerator BallThreeShakeIn()
    {
        Balls[3].ShakeInF();
        yield return new WaitForSeconds(.5f);
        Balls[3].ShakeInB();
        yield return new WaitForSeconds(.5f);
        Balls[3].MoveBack();
    }



    private IEnumerator BallFailRoutine(BallController ball)
    {

        cannon.Load();
        Parrot.fail();
        yield return new WaitForSeconds(.55f);
        Explode.miss();
        yield return new WaitForSeconds(.5f);
        rope.GoToOrigin();
        PrintingCoin.GoToOrigin();
        yield return new WaitForSeconds(2.1f);
        StartCoroutine(Rollout(ball.getChoice()));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartGame(0));
    }

    private IEnumerator BallSuccessRoutine(BallController ball)
    {
        cannon.Load();
        Parrot.success();
        yield return new WaitForSeconds(.55f);
        Explode.hit();
        yield return new WaitForSeconds(.9f);
        PrintingCoin.SetCoinTypeSuccess(ball.type);
        yield return new WaitForSeconds(.5f);
        rope.breakRope();
        PrintingCoin.drop();
        yield return new WaitForSeconds(.5f);
        PrintingCoin.ToggleVisibility(false, false);
        
        chest.UpgradeChest();
        rope.GoToOrigin();
        yield return new WaitForSeconds(.5f);
        rope.fixRope();
        yield return new WaitForSeconds(2.1f);
        StartCoroutine(Rollout(ball.getChoice()));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartGame(0));
    }
    private IEnumerator Rollout(int index)
    {
        if(index == 0)
        {
            StartCoroutine(RolloutThree());
            //yield return new WaitForSeconds(.05f);
            StartCoroutine(RolloutTwo());
            //yield return new WaitForSeconds(.15f);
            StartCoroutine(RolloutOne());
            //yield return new WaitForSeconds(.05f);

        }
        else if(index == 1)
        {
            StartCoroutine(RolloutThree());
            //yield return new WaitForSeconds(.05f);
            StartCoroutine(RolloutTwo());
            //yield return new WaitForSeconds(.15f);
            StartCoroutine(RolloutZeroSpecial());
            //yield return new WaitForSeconds(.05f);
        }
        else if (index == 2)
        {
            StartCoroutine(RolloutThree());
            //yield return new WaitForSeconds(.05f);
            StartCoroutine(RolloutOneSpecial());
            //yield return new WaitForSeconds(.15f);
            StartCoroutine(RolloutZeroSpecial());
            //yield return new WaitForSeconds(.05f);
        }
        else if (index == 3)
        {
            StartCoroutine(RolloutTwoSpecial());
            //yield return new WaitForSeconds(.05f);
            StartCoroutine(RolloutOneSpecial());
            //yield return new WaitForSeconds(.15f);
            StartCoroutine(RolloutZeroSpecial());
            //yield return new WaitForSeconds(.05f);
        }
        yield return new WaitForSeconds(0f);
    }
    private IEnumerator RolloutZero()
    {
        Balls[0].movePos4();
        yield return new WaitForSeconds(.45f);
        Balls[0].moveOut1();
        yield return new WaitForSeconds(.25f);
        Balls[0].moveOut2();
        yield return new WaitForSeconds(.02f);
        Balls[0].moveOut3();
        yield return new WaitForSeconds(.02f);
        Balls[0].moveOut4();

    }
    private IEnumerator RolloutZeroSpecial()
    {
        Balls[0].movePos4();
        yield return new WaitForSeconds(.15f);
        Balls[0].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[0].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[0].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[0].moveOut4();
    }
    private IEnumerator RolloutOne()
    {
        Balls[1].movePos4();
        yield return new WaitForSeconds(.15f);
        Balls[1].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[1].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[1].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[1].moveOut4();
    }
    private IEnumerator RolloutOneSpecial()
    {
        Balls[1].movePos4();
        yield return new WaitForSeconds(.05f);
        Balls[1].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[1].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[1].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[1].moveOut4();
    }
    private IEnumerator RolloutTwo()
    {
        Balls[2].movePos4();
        yield return new WaitForSeconds(.05f);
        Balls[2].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[2].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[2].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[2].moveOut4();
    }
    private IEnumerator RolloutTwoSpecial()
    {
        Balls[2].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[2].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[2].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[2].moveOut4();
    }
    private IEnumerator RolloutThree()
    {
        Balls[3].moveOut1();
        yield return new WaitForSeconds(.15f);
        Balls[3].moveOut2();
        yield return new WaitForSeconds(.07f);
        Balls[3].moveOut3();
        yield return new WaitForSeconds(.05f);
        Balls[3].moveOut4();
    }

    private IEnumerator WinRoutine()
    {
        yield return new WaitForSeconds(2f);

        // TODO: change maens of finishing game (for now we just return to the scroll map)
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private void PregameSetup()
    {
        Debug.Log("IN PREGAME");
        // create coin list
        rope.ToggleVisibility(false, false);
        PrintingCoin.GoToOrigin();
        foreach (var ball in Balls)
        {
            //Debug.Log(coin);
            allBalls.Add(ball);
        }


        // Create Global Coin List
        globalBallPool = GameManager.instance.GetGlobalActionWordList();
        unusedBallPool = new List<ActionWordEnum>();
        unusedBallPool.AddRange(globalBallPool);

        // disable all coins
        foreach (var ball in allBalls)
        {

            ball.setOrigin();
            Debug.Log(ball.transform.position);
        }



        foreach (var coin in allBalls)
        {
            //coin.gameObject.SetActive(false);
        }



    }

    private void walkThrough()
    {

    }



    private IEnumerator StartGame(int coins)
    {
        rope.ToggleVisibility(false, false);
        Debug.Log("At Start Game");
        rope.GoToOrigin();
        //StartCoroutine(CoinsDown(coins));
        PrintingCoin.GoToOrigin();
        PrintingCoin.grow();
        List<BallController> ballz = Getballs(coins);
        foreach (var ball in ballz)
        {

            ball.ToggleVisibility(false, false);
            ball.setToBaseRotation();
            ball.MoveToOrigin();
            yield return new WaitForSeconds(.1f);

        }
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(ShowBalls(coins));
        yield return new WaitForSeconds(1f);
        StartCoroutine(BallsUp(0));
        Debug.Log("Working?");
        yield return new WaitForSeconds(1f);
        rope.ToggleVisibility(true, true);
        PrintingCoin.moveIn();
        rope.moveIn();
    }

    private IEnumerator ShowBalls(int index)
    {
        Debug.Log("ShowCoins???????????????");
        List<BallController> currentBalls = Getballs(index);
        foreach (var ball in currentBalls)
        {
            // set random type
            if (unusedBallPool.Count == 0)
            {
                unusedBallPool.AddRange(globalBallPool);
            }
            ActionWordEnum type = unusedBallPool[Random.Range(0, unusedBallPool.Count)];
            unusedBallPool.Remove(type);

            ball.SetCoinType(type);
            ball.ToggleVisibility(true, true);
            Debug.Log("ShowCoins");
            yield return new WaitForSeconds(0f);
        }

        //SelectRandomCoin(currRow);
    }

    private IEnumerator BallsUp(int index)
    {
        Debug.Log("ShowCoins");
        List<BallController> coins = Getballs(index);
        foreach (var ball in Balls)
        {

            ball.MoveIn();
            yield return new WaitForSeconds(0f);

        }
        SelectRandomCoin(index);
    }
    private IEnumerator CoinsDown(int index)
    {
        Debug.Log("ShowCoins");
        List<BallController> coins = Getballs(index);
        foreach (var ball in Balls)
        {

            //ball.MoveDown();
            yield return new WaitForSeconds(0f);

        }
        //SelectRandomCoin(index);
    }


    private IEnumerator HideCoins(int index, RummageCoin exceptCoin = null)
    {
        List<BallController> row = Getballs(index);
        foreach (var ball in row)
        {
            if (ball != exceptCoin)
                ball.ToggleVisibility(false, true);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SelectRandomCoin(int index)
    {
        List<BallController> balls = Getballs(index);
        selectedIndex = Random.Range(0, balls.Count);

        print("selected index: " + selectedIndex);
        selectedBall = balls[selectedIndex];
        PrintingCoin.SetCoinType(selectedBall.type);


    }

    private List<BallController> Getballs(int index)
    {
        switch (index)
        {
            default:
            case 0:
                return Balls;


        }
    }
}
