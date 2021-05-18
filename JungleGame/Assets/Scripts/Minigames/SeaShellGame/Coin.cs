using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    
    public ActionWordEnum type;

    [SerializeField] public List<Sprite> images;
    public Image image;
    private bool audioPlaying;
    private Vector3 coinHolderPosition = new Vector3(0, 3.15f, 0f);
    private Vector3 offStagePosition = new Vector3(-2, 10f, 0f);
    private Vector3 originalPosition = new Vector3(0, 2.75f, 0f);
    private Vector3 correctPosition = new Vector3(-3.8f, 4.25f, 0f);
    private Vector3 chestPosition = new Vector3(-3.8f, 3f, 0f);
    private Vector3 chestTwoPosition = new Vector3(-5.5f, 1.5f, 0f);
    private Vector3 scaleNormal = new Vector3(.8f, .8f, 0f);
    private Vector3 scaleChange = new Vector3(.3f, .3f, 0f);
    // original vars
    private bool originalSet = false;
    private int moveSpeed = 2;
    private Animator animator;
    
    void Awake()
    {

        RectTransform rt = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
        GetComponent<Animator>().enabled = false;
        image = GetComponent<Image>();
    }

    void Update()
    {

    }

    public void shrink()
    {
        StartCoroutine(shrinkRoutine(scaleChange));
    }

    private IEnumerator shrinkRoutine(Vector3 target)
    {
        Vector3 currStart = transform.localScale;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.localScale = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.localScale = target;

                yield break;
            }

            yield return null;
        }
    }
    public void grow()
    {
        StartCoroutine(growRoutine(scaleNormal));
    }

    private IEnumerator growRoutine(Vector3 target)
    {
        Vector3 currStart = transform.localScale;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 5;
            if (timer < maxTime)
            {
                transform.localScale = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.localScale = target;

                yield break;
            }

            yield return null;
        }
    }






    public void GoToCoinHolder()
    {
        StartCoroutine(GoToCoinHolderRoutine(coinHolderPosition));
    }

    private IEnumerator GoToCoinHolderRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }


    public void GoToOffStage()
    {
        StartCoroutine(GoToOffStageRoutine(offStagePosition));
    }

    private IEnumerator GoToOffStageRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }

    public void GoToStartingPosition()
    {
        StartCoroutine(GoToStartingPositionRoutine(originalPosition));
    }

    private IEnumerator GoToStartingPositionRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }

    public void GoToCorrectPosition()
    {
        StartCoroutine(GoToCorrectPositionRoutine(correctPosition));
    }

    private IEnumerator GoToCorrectPositionRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 2;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }

    public void GoToChestPosition()
    {
        StartCoroutine(GoToChestPositionRoutine(chestPosition));
    }

    private IEnumerator GoToChestPositionRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;


        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 2;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }
    public void GoToChestTwoPosition()
    {
        StartCoroutine(GoToChestTwoPositionRoutine(chestTwoPosition));
    }

    private IEnumerator GoToChestTwoPositionRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        Debug.Log(currStart);
        float timer = 0f;
        float maxTime = 0.5f;


        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 2;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;

                yield break;
            }

            yield return null;
        }
    }


    public void SetCoinType(ActionWordEnum type, int num)
    {
        image.sprite = images[num];
        this.type = type;

        // get animator if null

    }

    public void PlayPhonemeAudio()
    {
        if (!audioPlaying)
        {
            StartCoroutine(PlayPhonemeAudioRoutine());
        }
    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(type);
        yield return new WaitForSeconds(1f);
        audioPlaying = false;
    }

    public void ToggleVisibilityCoin(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!image)
                image = GetComponent<Image>();
            Color temp = image.color;
            if (opt) { temp.a = 1f; }
            else { temp.a = 0; }
            image.color = temp;
        }
    }

    private IEnumerator ToggleVisibilityRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 2f; }
        float timer = 0f;
        GoToCoinHolder();
        while (true)
        {
            timer += Time.deltaTime;
            Color temp = image.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            image.color = temp;

            if (image.color.a == end)
            {
                break;
            }
            yield return null;
        }



    }
    public void ToggleVisibilityTwoCoin(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityTwoRoutine(opt));
        else
        {
            if (!image)
                image = GetComponent<Image>();
            Color temp = image.color;
            if (opt) { temp.a = 1f; }
            else { temp.a = 0; }
            image.color = temp;
        }
    }

    private IEnumerator ToggleVisibilityTwoRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 2f; }
        float timer = 0f;
        //GoToCoinHolder();
        while (true)
        {
            timer += Time.deltaTime;
            Color temp = image.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            image.color = temp;

            if (image.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
    public void coinAnimation(string type)
    {
        
        StartCoroutine(coinRoutine(type));
    }

    private IEnumerator coinRoutine(string type)
    {
        Debug.Log(type);
        GetComponent<Animator>().enabled = true;
        animator.Play(type);
        yield return new WaitForSeconds(0f);
    }
}
