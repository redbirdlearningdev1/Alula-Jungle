using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsController : MonoBehaviour
{
    public static BallsController instance;

    public List<Ball> balls;

    public GameObject holder1;
    public GameObject holder2;

    public Transform ballDropPos;


    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ResetBalls()
    {
        holder1.SetActive(true);

        foreach (Ball ball in balls)
        {
            ball.transform.localPosition = ball.resetPos;
            ball.TogglePhysics(true);
        }
    }
    
    public void ShowBalls()
    {
        holder2.SetActive(true);
        holder1.SetActive(false);
    }

    public void ReleaseBalls()
    {
        holder2.SetActive(false);
        holder1.SetActive(true);
    }

    public void ReDropBall(Ball ball)
    {
        StartCoroutine(ReDropBallRoutine(ball.GetComponent<LerpableObject>()));
    }

    private IEnumerator ReDropBallRoutine(LerpableObject ball)
    {
        ball.LerpScale(new Vector2(0f, 0f), 0.2f);
        yield return new WaitForSeconds(0.2f);
        ball.transform.localPosition = ballDropPos.localPosition;
        ball.transform.localScale = new Vector3(1f, 1f, 1f);
        ball.GetComponent<Ball>().TogglePhysics(true);
    }
}
