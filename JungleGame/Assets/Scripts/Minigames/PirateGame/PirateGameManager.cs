using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] balls;

    [SerializeField] private GameObject parrot;

    [SerializeField] private GameObject cannon;

    [SerializeField] private GameObject shot;

    [SerializeField] private GameObject coin;

    [SerializeField] private GameObject rope;

    [SerializeField] private GameObject chest;

    [SerializeField] private GameObject[] fails;

    [SerializeField] private Sprite[] chestStates;
    private int chestState = 0;

    public bool win; //temporary

    private bool goNext = true;

    public static PirateGameManager instance;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveObject(balls[2].GetComponent<RectTransform>(), cannon.GetComponent<RectTransform>().anchoredPosition, 4f, 3f, true));
    }


    private IEnumerator MoveObject(RectTransform obj, Vector2 target, float speed, float delay, bool shrink)
    {
        yield return new WaitForSeconds(delay);
        float timer = 0f;
        float maxTime = speed;
        Vector3 currTarget = target;
        Debug.Log(currTarget);
        Vector3 currStart = obj.anchoredPosition;

        Vector3 currScale = obj.localScale;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 5f;
            if (timer < maxTime)
            {
                obj.anchoredPosition = Vector3.Lerp(currStart, currTarget, timer / maxTime);
                if (shrink)
                {
                    obj.localScale = Vector3.Lerp(currScale, Vector2.zero, timer / maxTime);
                }
            }
            else
            {
                if (goNext) //this will be called when the coin is going in the bag
                {
                    goNext = false;
                    GoNext();
                }
                yield break;
            }
            yield return null;
        }
    }

    public void AfterShot()
    {
        if (win)
        {
            chest.GetComponent<Image>().sprite = chestStates[chestState];
            coin.GetComponent<LogCoin>().SetCoinType(ActionWordEnum.poop);
            StartCoroutine(MoveObject(coin.GetComponent<RectTransform>(), chest.GetComponent<RectTransform>().anchoredPosition, 4f, 1f, true));
            //balls[2].GetComponent<RectTransform>().anchoredPosition = new Vector3(20, -150, 0);
            //balls[2].transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            fails[0].SetActive(true);
            //balls[2].GetComponent<RectTransform>().anchoredPosition = new Vector3(20, -150, 0);
            //balls[2].transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void GoShot()
    {
        shot.GetComponent<Animator>().SetTrigger("go");
    }

    private void GoNext()
    {
        goNext = false;
        cannon.GetComponent<Animator>().SetTrigger("shoot");
        parrot.GetComponent<Animator>().SetTrigger("shoot");
        if (win)
        {
            rope.SetActive(false);
            shot.GetComponent<Animator>().SetBool("correct", true);
            parrot.GetComponent<Animator>().SetBool("correct", true);
        }
        else
        {
            shot.GetComponent<Animator>().SetBool("correct", false);
            parrot.GetComponent<Animator>().SetBool("correct", false);
        }
    }
}
