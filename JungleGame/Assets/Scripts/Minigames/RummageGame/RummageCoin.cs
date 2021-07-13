using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RummageCoin : MonoBehaviour
{
    public ActionWordEnum type;
    public int logIndex;
    public Transform clothPos;
    public Transform pileParent;
    public Vector3 pileMovement1;
    public Vector3 pileMovement2;
    public Vector3 Origin;
    private Vector3 scaleNormal = new Vector3(.67f, .67f, 0f);
    private Vector3 scaleSmall = new Vector3(.33f, .33f, 0f);
    public float moveSpeed = 5f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private Image image;
    private bool audioPlaying;

    // original vars
    private bool originalSet = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play(type.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();
    }

    void Update()
    {

    }

    public void ReturnToCloth()
    {
        StartCoroutine(ReturnToClothRoutine(clothPos.position));
    }
    public void setOrigin()
    {
        Origin = transform.position;
    }

    private IEnumerator ReturnToClothRoutine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
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
                transform.SetParent(pileParent);
                yield break;
            }

            yield return null;
        }
    }

    public void grow()
    {
        StartCoroutine(growRoutine(scaleNormal));
    }
    public void shrink()
    {
        StartCoroutine(growRoutine(scaleSmall));
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
            timer += Time.deltaTime * 2;
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

    public void BounceIn1()
    {
        StartCoroutine(BounceOutRoutine(Origin));
    }
    public void BounceOut1()
    {
        StartCoroutine(BounceOutRoutine(pileMovement1));
    }
    public void BounceToCloth()
    {
        StartCoroutine(BounceOutRoutine(clothPos.position));
    }

    private IEnumerator BounceOutRoutine(Vector3 target)
    {
        Debug.Log("Here");
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 0.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * 1;
            if (timer < maxTime)
            {
                transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                transform.position = target;
                transform.SetParent(pileParent);
                yield break;
            }

            yield return null;
        }
    }


    public void SetCoinType(ActionWordEnum type)
    {
        this.type = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();
        animator.Play(type.ToString());
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

    public void ToggleVisibility(bool opt, bool smooth)
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
        if (opt) { end = 1f; }
        float timer = 0f;
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

    
}
