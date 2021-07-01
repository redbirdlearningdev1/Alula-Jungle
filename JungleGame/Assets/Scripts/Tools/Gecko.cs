using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using UnityEngine.UI;




public class Gecko : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [Header("Game Data")]
    //public GameData gameData;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] public GameObject common;
    [SerializeField] public GameObject uncommon;
    [SerializeField] public GameObject rare;
    [SerializeField] public GameObject legendary;

    [SerializeField] public GameObject board;
    [SerializeField] public GameObject book;

    [SerializeField] private GameObject CurrentSticker;
    [SerializeField] private List<GameObject> CommonSticker;

    [SerializeField] private List<GameObject> UncommonSticker;
    [SerializeField] private List<GameObject> RareSticker;
    [SerializeField] private List<GameObject> LegendarySticker;


    private Vector3 scaleNormal = new Vector3(1f, 1f, 0f);
    private Vector3 scaleSmall = new Vector3(.5f, .5f, 0f);
    private Vector3 scaleLarge = new Vector3(1.5f, 1.5f, 0f);

    private bool isPressed = false;
    private bool isFixed = false;
    private bool stickerSelected = false;
    private int rand;
    private int randC;
    private int randU;
    private int randR;
    private int randL;
    private int pityU = 0;
    private int pityR = 0;
    private int pityL = 0;



    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        CurrentSticker.gameObject.SetActive(false);


    }




    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isPressed)
        {
            if(!stickerSelected)
            {
                isPressed = true;
                Debug.Log("HERE");

                rand = Random.Range(0, 51);
                Debug.Log("This is rand: " + rand);
                if(rand <35 && (!(pityL >=50) && !(pityR >= 25) && !(pityU >=10)))
                {
                    Debug.Log(CommonSticker.Count);
                    randC = Random.Range(0, CommonSticker.Count);
                    CurrentSticker.GetComponent<Image>().sprite = CommonSticker[randC].GetComponent<Image>().sprite;
                    CurrentSticker.GetComponent<RectTransform>().sizeDelta = CommonSticker[randC].GetComponent<RectTransform>().sizeDelta;
                    shrink();
                    common.gameObject.SetActive(true);
                    Debug.Log("This is pityU: " + pityU);
                    Debug.Log("This is pityR: " + pityR);
                    Debug.Log("This is pityL: " + pityL);
                }
                else if((rand >= 35 && rand < 45 && (!(pityL >= 50) && !(pityR >= 25))) || pityU >= 10)
                {
                    randU = Random.Range(0, UncommonSticker.Count);
                    CurrentSticker.GetComponent<Image>().sprite = UncommonSticker[randU].GetComponent<Image>().sprite;
                    CurrentSticker.GetComponent<RectTransform>().sizeDelta = UncommonSticker[randU].GetComponent<RectTransform>().sizeDelta;
                    shrink();
                    uncommon.gameObject.SetActive(true);

                    pityU = 0;
                }
                else if ((rand >= 45 && rand < 50 && (!(pityL >= 50) && !(pityU >= 10))) || pityR >= 25)
                {
                    randR = Random.Range(0, RareSticker.Count);
                    CurrentSticker.GetComponent<Image>().sprite = RareSticker[randR].GetComponent<Image>().sprite;
                    CurrentSticker.GetComponent<RectTransform>().sizeDelta = RareSticker[randR].GetComponent<RectTransform>().sizeDelta;
                    shrink();

                    rare.gameObject.SetActive(true);
                    pityR = 0;
                }
                else if (rand >= 50 || pityL >=50)
                {
                    randL = Random.Range(0, LegendarySticker.Count);
                    CurrentSticker.GetComponent<Image>().sprite = LegendarySticker[randL].GetComponent<Image>().sprite;
                    CurrentSticker.GetComponent<RectTransform>().sizeDelta = LegendarySticker[randL].GetComponent<RectTransform>().sizeDelta;
                    shrink();
                    legendary.gameObject.SetActive(true);
                    pityL = 0;
                }

                //animator.Play("geckoWakeup 0");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                StartCoroutine(StickerWait());

            }
            if(stickerSelected)
            {
                isPressed = true;
                Debug.Log("HERE");
                CurrentSticker.gameObject.SetActive(false);
                common.gameObject.SetActive(false);
                uncommon.gameObject.SetActive(false);
                rare.gameObject.SetActive(false);
                legendary.gameObject.SetActive(false);
                stickerSelected = false;
                book.gameObject.SetActive(true);
                board.gameObject.SetActive(true);
            }

        }
    }

    private IEnumerator StickerWait()
    {
        pityU++;
        pityR++;
        pityL++;
        yield return new WaitForSeconds(4f);
        book.gameObject.SetActive(false);
        board.gameObject.SetActive(false);
        //CurrentSticker.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        CurrentSticker.gameObject.SetActive(true);
        grow();
        yield return new WaitForSeconds(.3f);
        
        Stabalize();
        yield return new WaitForSeconds(1f);
        //common.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Can Leave Scene");
        stickerSelected = true;
    }
    public void grow()
    {
        StartCoroutine(growRoutine(scaleLarge));
    }
    public void Stabalize()
    {
        StartCoroutine(growRoutine(scaleNormal));
    }
    public void shrink()
    {
        StartCoroutine(growRoutine(scaleSmall));
    }

    private IEnumerator growRoutine(Vector3 target)
    {
        Vector3 currStart = CurrentSticker.transform.localScale;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 1.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 4f;
            if (timer < maxTime)
            {
                CurrentSticker.transform.localScale = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.localScale = target;

                yield break;
            }

            yield return null;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;
            Debug.Log(isPressed);

        }
    }

}
