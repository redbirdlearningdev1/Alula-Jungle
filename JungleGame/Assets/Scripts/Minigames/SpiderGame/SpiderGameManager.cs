using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderGameManager : MonoBehaviour
{
    [SerializeField] private GameObject web;

    [SerializeField] private GameObject bug;

    [SerializeField] private GameObject spider;

    [SerializeField] private GameObject bag;
    [SerializeField] private Sprite[] bagStates;
    private int bagState = 0;

    [SerializeField] private GameObject[] coins;
    private GameObject selectedCoin;
    private CoinType correctCoin = CoinType.awCoin;
    private bool inputDisabled;

    [SerializeField] private GameObject[] fails;
    private int failState;

    public static SpiderGameManager instance;

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
                    }
                }
                else if (hit.collider.CompareTag("Bug"))
                {
                    Debug.Log("Bug");
                    Debug.Log("Coin");
                    if (!inputDisabled)
                    {
                        bug.GetComponent<Animator>().SetTrigger("bugPressed");
                    }
                }
            }
        }
    }

    public void OnBugLand()
    {
        web.GetComponent<Animator>().SetTrigger("bugLands");
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

    private void HandleAnswer(GameObject coin)
    {
        if(coin.GetComponent<Coin>().coinType == correctCoin)
        {
            spider.GetComponent<Animator>().SetBool("correct", true);
            bug.GetComponent<Animator>().SetTrigger("bugTakeoff");
            StartCoroutine(MoveObject(coin.transform, Vector3.down, 0.7f, .7f, false));
            StartCoroutine(MoveObject(bug.transform, Vector3.up * 5 + Vector3.right * 5, 1f, .35f, false));
            StartCoroutine(MoveObject(coin.transform, Vector3.up * 3 + Vector3.right * 2.5f, 4f, 1.5f, true));
            foreach (GameObject g in coins)
            {
                if (g != selectedCoin)
                {
                    StartCoroutine(MoveObject(g.transform, Vector3.down * 3f, 0.7f, 1.5f, false));
                }
            }
        }
        else
        {
            spider.GetComponent<Animator>().SetBool("correct", false);
            bug.GetComponent<Animator>().SetTrigger("bugWrap");
            StartCoroutine(MoveObject(coin.transform, Vector3.down, 0.7f, .7f, false));
            StartCoroutine(MoveObject(bug.transform, Vector3.up * 5, 3f, .7f, false));
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
        bug.GetComponent<RectTransform>().anchoredPosition = new Vector3(-373, 316, 0);
        var xPos = -200f; //first coin starts here
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
        inputDisabled = false;
    }
}
