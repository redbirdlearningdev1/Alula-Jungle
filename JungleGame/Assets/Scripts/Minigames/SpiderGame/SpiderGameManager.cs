using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderGameManager : MonoBehaviour
{
    [SerializeField] private GameObject web;

    [SerializeField] private GameObject bug;
    [SerializeField] private SpriteRenderer bugGlow;

    [SerializeField] private GameObject spider;

    [SerializeField] private GameObject bag;
    [SerializeField] private Sprite[] bagStates;
    private int bagState = 0;

    [SerializeField] private GameObject[] coins;
    [SerializeField] private SpriteRenderer[] coinGlows;
    private GameObject selectedCoin;

    private ActionWordEnum correctCoin = ActionWordEnum.poop;
    private int correctCoinIndex = 2;

 //   private ActionWordEnum correctCoin = ActionWordEnum.baby;

    private bool inputDisabled;

    private bool audioPlaying;

    private bool introPlayed;

    [SerializeField] private GameObject[] fails;
    private int failState;

    [SerializeField] private Animator spiderGrab;
    [SerializeField] private Animator treeGrab;

    public static SpiderGameManager instance;

    public List<ActionWordEnum> globalCoinPool;
    private List<ActionWordEnum> unusedWrongCoinPool;
    private List<ActionWordEnum> unusedCorrectCoinPool;

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
        StartCoroutine(MoveObject(bug.transform, Vector3.down * 5 + Vector3.right * 5, 5f, 0f, false));
        bug.GetComponent<Animator>().SetTrigger("bugLand");
    }

    // Update is called once per frame
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
                        SelectCoin(hit.transform.gameObject);
                        if(introPlayed != true)
                        {
                            coins[correctCoinIndex].GetComponent<GlowOutlineController>().SetGlowSettings(0f, 0, Color.white, false);
                            bugGlow.enabled = false;
                            introPlayed = true;
                        }
                    }
                }
                else if (hit.collider.CompareTag("Bug"))
                {
                    bugGlow.enabled = false;
                    Debug.Log("Bug");
                    Debug.Log("Coin");
                    if (!inputDisabled)
                    {
                        bug.GetComponent<Animator>().SetTrigger("bugPressed");
                        if (!audioPlaying)
                        {
                            StartCoroutine(PlayPhonemeAudioRoutine());
                        }
                    }
                    if(introPlayed != true)
                    {
                        coins[correctCoinIndex].GetComponent<GlowOutlineController>().SetGlowSettings(10f, 10, Color.white, false);
                    }
                }
            }
        }
    }

    public void OnBugLand()
    {
        web.GetComponent<Animator>().SetTrigger("bugLands");
        if(introPlayed != true)
        {
            bugGlow.enabled = true;
        }

    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(correctCoin);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    private void SelectCoin(GameObject coin)
    {
        inputDisabled = true;
        selectedCoin = coin;
        spider.GetComponent<Animator>().SetTrigger("go");
        StartCoroutine(MoveObject(coin.transform, Vector3.up, 0.7f, 0f, false));
        HandleAnswer(coin);
    }

    private IEnumerator MoveObject(Transform obj, Vector3 target, float speed, float delay, bool shrink)
    {
        yield return new WaitForSeconds(delay);
        float timer = 0f;
        float maxTime = speed;
        Vector3 currTarget = obj.position + target;
        Vector3 currStart = obj.position;

        Vector3 currScale = obj.localScale;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 5f;
            if (timer < maxTime)
            {
                obj.position = Vector3.Lerp(currStart, currTarget, timer / maxTime);
                if (shrink)
                {
                    obj.localScale = Vector3.Lerp(currScale, Vector3.zero, timer / maxTime);
                }
            }
            else
            {
                if (shrink) //this will be called when the coin is going in the bag
                {
                    bagState++;
                    bag.GetComponent<Image>().sprite = bagStates[bagState];
                }
                yield break;
            }
            yield return null;
        }
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
                if (shrink) //this will be called when the coin is going in the bag
                {
                    bagState++;
                    bag.GetComponent<Image>().sprite = bagStates[bagState];
                }
                yield break;
            }
            yield return null;
        }
    }

    private void HandleAnswer(GameObject coin)
    {
        if(coin.GetComponent<LogCoin>().type == correctCoin)
        {
            spider.GetComponent<Animator>().SetBool("correct", true);
            spiderGrab.SetInteger("coin", coin.GetComponent<LogCoin>().logIndex);
            bug.GetComponent<Animator>().SetTrigger("bugTakeoff");
            StartCoroutine(MoveObject(coin.transform, Vector3.down, 0.7f, .7f, false));
            StartCoroutine(MoveObject(bug.transform, Vector3.up * 5 + Vector3.right * 5, 3f, .35f, false));
            //StartCoroutine(MoveObject(coin.GetComponent<RectTransform>(), bag.GetComponent<RectTransform>().anchoredPosition, 4f, 1.3f, true));
            foreach (GameObject g in coins)
            {
                if (g != coin)
                {
                    StartCoroutine(MoveObject(g.transform, Vector3.down * 3f, 0.7f, 1.5f, false));
                }
            }
        }
        else
        {
            spider.GetComponent<Animator>().SetBool("correct", false);
            bug.GetComponent<Animator>().SetTrigger("bugWrap");
            treeGrab.SetTrigger("grab");
            StartCoroutine(MoveObject(coin.transform, Vector3.down, 0.7f, .7f, false));
            //StartCoroutine(MoveObject(bug.transform, Vector3.up * 5, 3f, 1.5f, false));
            failState++;
            fails[failState].SetActive(true);
            foreach (GameObject g in coins)
            {
                StartCoroutine(MoveObject(g.transform, Vector3.down * 3f, 0.7f, 1.5f, false));
            }
        }
    }

    public void Reset()
    {
        if(bagState == 4 || failState == 4)
        {
            GameManager.instance.LoadScene("MinigameDemoScene", true, 3f);
            return;
        }
        
        bug.GetComponent<RectTransform>().anchoredPosition = new Vector3(-373, 316, 0);
        var xPos = -200f; //first coin starts here

        //correct coin manipulation
        if (unusedCorrectCoinPool.Count == 0)
        {
            unusedCorrectCoinPool.AddRange(globalCoinPool);
        }
        ActionWordEnum type = unusedCorrectCoinPool[Random.Range(0, unusedCorrectCoinPool.Count)];
        unusedCorrectCoinPool.Remove(type);
        correctCoin = type;
        correctCoinIndex = Random.Range(0, 3);
        coins[correctCoinIndex].GetComponent<LogCoin>().SetCoinType(correctCoin);

        //incorrect coin manipulation
        if (unusedWrongCoinPool.Count <= 2)
        {
            unusedWrongCoinPool = new List<ActionWordEnum>();
            unusedWrongCoinPool.AddRange(globalCoinPool);
        }
        for(int i = 0; i < 4; i++)
        {
            if(i != correctCoinIndex)
            {
                ActionWordEnum wType = unusedWrongCoinPool[Random.Range(0, unusedWrongCoinPool.Count)];
                unusedWrongCoinPool.Remove(wType);
                if(wType != type) //make sure we don't get two of the same coin
                {
                    coins[i].GetComponent<LogCoin>().SetCoinType(wType);
                }
                else
                {
                    wType = unusedWrongCoinPool[Random.Range(0, unusedWrongCoinPool.Count)];
                    unusedWrongCoinPool.Remove(wType);
                }
            }
        }

        foreach (GameObject g in coins)
        {
            g.GetComponent<RectTransform>().anchoredPosition = new Vector3(xPos, -284, 0);
            g.transform.localScale = new Vector3(1, 1, 1);
            xPos += 110f; //coin offset
        }
        StartCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(0f);
        StartCoroutine(MoveObject(bug.transform, Vector3.down * 5 + Vector3.right * 5, 5f, 0f, false));
        bug.GetComponent<Animator>().SetTrigger("bugLand");
        foreach (GameObject g in coins)
        {
            StartCoroutine(MoveObject(g.transform, Vector3.up * 3f, 0.7f, 1.5f, false)); //put coins back
        }
        StartCoroutine(PlayPhonemeAudioRoutine());
        inputDisabled = false;
    }

    public void BugWrap()
    {
        StartCoroutine(MoveObject(bug.transform, Vector3.up * 5, 3f, 0f, false));
    }

    public void CoinGrab()
    {
        StartCoroutine(MoveObject(selectedCoin.GetComponent<RectTransform>(), bag.GetComponent<RectTransform>().anchoredPosition, 4f, 0f, true));
    }
}
