using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BugController : MonoBehaviour
{
    public ActionWordEnum type;
    public Vector3 WebLand;
    public Vector3 WebLeave1;
    public Vector3 WebLeave2;
    public Vector3 WebEat;
    public Vector3 Origin;
    private Vector3 scaleNormal = new Vector3(.6f, .6f, 0f);
    private Vector3 scaleSmall = new Vector3(.45f, .45f, 0f);
    public float moveSpeed = 1f;

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
        //setOrigin();
    }

    public void StartToWeb()
    {
        shrink();
        StartCoroutine(ReturnToWebRoutine(WebLand));
        StartCoroutine(landRoutine());
    }

    public void setOrigin()
    {
        Origin = transform.position;
    }

    public void goToOrigin()
    {
        grow();
        StartCoroutine(ReturnToOriginRoutine(Origin));
    }

    public void die()
    {
        StartCoroutine(dieRoutine());
    }

    private IEnumerator dieRoutine()
    {
        yield return new WaitForSeconds(0f);
        animator.Play("Wrapped");
    }

    private IEnumerator landRoutine()
    {
        yield return new WaitForSeconds(1.20f);
        animator.Play("Land");
        yield return new WaitForSeconds(.5f);
        animator.Play("Still");
    }

    private IEnumerator ReturnToWebRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = 1.5f;

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

    private IEnumerator ReturnToOriginRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = .25f;

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
        float timer = 0f;
        float maxTime = 1.5f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * moveSpeed;
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

    public void takeOff()
    {
        StartCoroutine(takeOffRoutine());
    }

    private IEnumerator takeOffRoutine()
    {

        animator.Play("Takeoff");
        yield return new WaitForSeconds(0f);

    }

    public void leaveWeb()
    {
        animator.Play("Takeoff");
        StartCoroutine(leaveWebRoutine(WebLeave1));

    }

    public void leaveWeb2()
    {

        StartCoroutine(leaveWebRoutine(WebLeave2));
    }

    public void webGetEat()
    {

        StartCoroutine(leaveWebRoutine(WebEat));
    }

    private IEnumerator leaveWebRoutine(Vector3 target)
    {
        Vector3 currStart = transform.position;
        float timer = 0f;
        float maxTime = .75f;

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
        animator.Play("Twitch");
        
        audioPlaying = true;
        AudioManager.instance.PlayPhoneme(type);
        yield return new WaitForSeconds(.8f);
        animator.Play("Still");
        yield return new WaitForSeconds(.5f);
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
