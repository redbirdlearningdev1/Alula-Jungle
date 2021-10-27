using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateGameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] balls;
    private GameObject selectedBall;

    [SerializeField] private GameObject parrot;
    [SerializeField] private SpriteRenderer parrotGlow;

    [SerializeField] private GameObject cannon;

    [SerializeField] private GameObject shot;

    [SerializeField] private GameObject coin;

    [SerializeField] private GameObject rope;

    [SerializeField] private GameObject chest;

    [SerializeField] private GameObject[] fails;
    private int failState = -1;

    [SerializeField] private Sprite[] chestStates;
    private int chestState = -1;

    public bool win; //temporary

    private bool goNext = true;

    private ActionWordEnum correctCoin = ActionWordEnum.poop;
    private int correctCoinIndex = 2;

    public List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedWrongCoinPool;
    private List<ActionWordEnum> unusedCorrectCoinPool;

    private bool inputDisabled;

    private bool audioPlaying;

    private bool introPlayed;

    public static PirateGameManager instance;

    public void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();
        
        if (!instance)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray);

            if (hit)
            {
                if (hit.collider.CompareTag("Coin"))
                {
                    Debug.Log("Coin");
                    if (!inputDisabled)
                    {
                        SelectBall(hit.transform.gameObject);
                        if (introPlayed != true)
                        {
                            //balls[correctCoinIndex].GetComponent<GlowOutlineController>().SetGlowSettings(0f, 0, Color.white, false);
                            parrotGlow.enabled = false;
                            introPlayed = true;
                        }
                    }
                }
                else if (hit.collider.CompareTag("Parrot"))
                {
                    parrotGlow.enabled = false;
                    Debug.Log("Bug");
                    Debug.Log("Coin");
                    if (!inputDisabled)
                    {
                        if (!audioPlaying)
                        {
                            StartCoroutine(PlayPhonemeAudioRoutine());
                        }
                    }
                    if (introPlayed != true)
                    {
                        //balls[correctCoinIndex].GetComponent<GlowOutlineController>().SetGlowSettings(10f, 10, Color.white, false);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: add music and ambiance
        AudioManager.instance.StopMusic();

        globalCoinPool = new List<ActionWordEnum>();
        unusedWrongCoinPool = new List<ActionWordEnum>();
        unusedCorrectCoinPool = new List<ActionWordEnum>();

        string[] coins = System.Enum.GetNames(typeof(ActionWordEnum));
        for (int i = 0; i < coins.Length; i++)
        {
            ActionWordEnum type = (ActionWordEnum)System.Enum.Parse(typeof(ActionWordEnum), coins[i]);
            globalCoinPool.Add(type);
        }
        globalCoinPool.Remove(ActionWordEnum.SIZE);
        globalCoinPool.Remove(ActionWordEnum._blank);
        unusedWrongCoinPool.AddRange(globalCoinPool);
        unusedCorrectCoinPool.AddRange(globalCoinPool);

        StartCoroutine(PlayPhonemeAudioRoutine());

        /*StartCoroutine(MoveObject(balls[2].GetComponent<RectTransform>(), cannon.GetComponent<RectTransform>().anchoredPosition, 4f, 3f, true, false));
        StartCoroutine(MoveObject(balls[1].GetComponent<RectTransform>(), balls[1].GetComponent<RectTransform>().anchoredPosition + new Vector2(110, 0), 4f, 3f, false, true));
        StartCoroutine(MoveObject(balls[0].GetComponent<RectTransform>(), balls[0].GetComponent<RectTransform>().anchoredPosition + new Vector2(110, 0), 4f, 3f, false, true));*/
    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(correctCoin);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    private void SelectBall(GameObject ball)
    {
        inputDisabled = true;
        selectedBall = ball;
        StartCoroutine(MoveObject(selectedBall.GetComponent<RectTransform>(), cannon.GetComponent<RectTransform>().anchoredPosition, 4f, 0f, true, false));
        for(int i = 0; i < selectedBall.GetComponent<PirateCoin>().logIndex; i++)
        {
            StartCoroutine(MoveObject(balls[i].GetComponent<RectTransform>(), balls[i].GetComponent<RectTransform>().anchoredPosition + new Vector2(110, 0), 4f, 0f, false, true));
        }

        if (selectedBall.GetComponent<PirateCoin>().type == correctCoin)
        {
            win = true;
            chestState++;
        }
        else
        {
            win = false;
            failState++;
        }
    }

    private IEnumerator MoveObject(RectTransform obj, Vector2 target, float speed, float delay, bool shrink, bool rotate)
    {
        yield return new WaitForSeconds(delay);
        float timer = 0f;
        float maxTime = speed;
        Vector3 currTarget = target;
        Debug.Log(currTarget);
        Vector3 currStart = obj.anchoredPosition;
        Vector3 currRotation = obj.eulerAngles;
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
                if (rotate)
                {
                    obj.eulerAngles = Vector3.Lerp(currRotation, currRotation + new Vector3(0f,0f,-360f), timer / maxTime);
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
            coin.GetComponent<LogCoin>().SetCoinType(selectedBall.GetComponent<PirateCoin>().type);
            rope.SetActive(false);
            StartCoroutine(MoveObject(coin.GetComponent<RectTransform>(), chest.GetComponent<RectTransform>().anchoredPosition, 4f, .3f, true, false));
        }
        else
        {
            fails[failState].SetActive(true);
        }
        StartCoroutine(PreReset());
    }

    private IEnumerator PreReset()
    {
        yield return new WaitForSeconds(2f);
        Reset();
    }


    public void Reset()
    {
        // win game!
        if (chestState == 2 || failState == 3)
        {
            StartCoroutine(WinGameRoutine());
            return;
        }
        
        //correct coin manipulation
        if (unusedCorrectCoinPool.Count == 0)
        {
            unusedCorrectCoinPool.AddRange(globalCoinPool);
        }
        ActionWordEnum type = unusedCorrectCoinPool[Random.Range(0, unusedCorrectCoinPool.Count)];
        unusedCorrectCoinPool.Remove(type);
        correctCoin = type;
        correctCoinIndex = Random.Range(0, 3);
        balls[correctCoinIndex].GetComponent<PirateCoin>().SetCoinType(correctCoin);

        //incorrect coin manipulation
        if (unusedWrongCoinPool.Count <= 2)
        {
            unusedWrongCoinPool = new List<ActionWordEnum>();
            unusedWrongCoinPool.AddRange(globalCoinPool);
        }
        for (int i = 0; i < 4; i++)
        {
            if (i != correctCoinIndex)
            {
                ActionWordEnum wType = unusedWrongCoinPool[Random.Range(0, unusedWrongCoinPool.Count)];
                unusedWrongCoinPool.Remove(wType);
                if (wType != type) //make sure we don't get two of the same coin
                {
                    balls[i].GetComponent<PirateCoin>().SetCoinType(wType);
                }
                else
                {
                    wType = unusedWrongCoinPool[Random.Range(0, unusedWrongCoinPool.Count)];
                    unusedWrongCoinPool.Remove(wType);
                }
            }
        }

        rope.SetActive(true);
        coin.GetComponent<RectTransform>().anchoredPosition = new Vector3(221, 99, 0);
        coin.transform.localScale = new Vector3(1, 1, 1);
        coin.GetComponent<LogCoin>().SetCoinType(ActionWordEnum._blank);

        var xPos = -200f; //first coin starts here
        foreach (GameObject g in balls)
        {
            g.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, -150, 0);
            g.transform.localScale = new Vector3(1, 1, 1);
            xPos += 110f; //coin offset
        }
        StartCoroutine(Begin());
    }

    private IEnumerator WinGameRoutine()
    {
        // play win tune
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        yield return new WaitForSeconds(3f);

        // TODO: finish tutorial stuff
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(0f);
        StartCoroutine(PlayPhonemeAudioRoutine());
        inputDisabled = false;
        goNext = true;
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
