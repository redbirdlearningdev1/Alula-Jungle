using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BugController : MonoBehaviour
{
    public ActionWordEnum type;
    public Transform WebLand;
    public Transform flyOffScreenPos;
    public Transform eattenPos;
    public Transform origin;
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
        animator.Play("Fly");
        StartCoroutine(ReturnToWebRoutine(WebLand.position));
        StartCoroutine(landRoutine());
    }

    public void goToOrigin()
    {
        transform.position = origin.position;
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
        Vector2 pos = transform.position;
        Vector2 tempPos = pos;
        tempPos.y -= 0.25f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.2f, false);
        yield return new WaitForSeconds(.2f);
        tempPos = pos;
        tempPos.y += 0.15f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.3f, false);
        yield return new WaitForSeconds(.3f);

        animator.Play("Takeoff");
    }

    public void leaveWeb()
    {
        animator.Play("Takeoff");
        GetComponent<LerpableObject>().LerpPosition(flyOffScreenPos.position, 0.8f, false);
    }

    public void webGetEat()
    {
        StartCoroutine(webGetEatRoutine());
    }

    private IEnumerator webGetEatRoutine()
    {
        animator.Play("Wrapped");

        Vector2 pos = transform.position;
        Vector2 tempPos = pos;
        tempPos.y -= 0.25f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.1f, false);
        yield return new WaitForSeconds(.1f);

        GetComponent<LerpableObject>().LerpPosition(eattenPos.position, 0.25f, false);
    }

    public void SetCoinType(ActionWordEnum type)
    {
        this.type = type;
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
        yield return new WaitForSeconds(0.25f);

        WebController.instance.webSmall();
        animator.Play("Twitch");
        BugBounce();

        animator.Play("Still");
        yield return new WaitForSeconds(1f);
    
        audioPlaying = false;
    }

    public void BugBounce()
    {
        StartCoroutine(BugBounceRoutine());
    }

    private IEnumerator BugBounceRoutine()
    {
        Vector2 pos = transform.position;
        Vector2 tempPos = pos;
        tempPos.y -= 0.25f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.2f, false);
        yield return new WaitForSeconds(.2f);
        tempPos = pos;
        tempPos.y += 0.15f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.3f, false);
        yield return new WaitForSeconds(.3f);

        GetComponent<LerpableObject>().LerpPosition(pos, 0.3f, false);
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
