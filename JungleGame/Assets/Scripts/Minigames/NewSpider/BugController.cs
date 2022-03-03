using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BugType 
{
    Ladybug, Bee, Light
}

public class BugController : MonoBehaviour
{
    public static BugController instance;


    public ActionWordEnum type;
    public Transform WebLand;
    public Transform flyOffScreenPos;
    public Transform eattenPos;
    public Transform origin;
    public float moveSpeed = 1f;

    private BugType currentBugType;
    private Animator animator;
    private BoxCollider2D myCollider;
    public Image image;
    private bool audioPlaying;

    void Awake()
    {
        if (instance == null)
            instance = this;

        animator = GetComponent<Animator>();

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        image = GetComponent<Image>();
        
        // select random bug type
        currentBugType = (BugType)Random.Range(0, 3);
    }

    public void StartToWeb()
    {
        // play bug fly sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BugFlyIn, 1f);

        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugFly");
                break;
            case BugType.Bee:
                animator.Play("BeeFly");
                break;
            case BugType.Light:
                animator.Play("LightningFly");
                break;
        }
        
        StartCoroutine(ReturnToWebRoutine(WebLand.position));
        StartCoroutine(landRoutine());
    }

    public void goToOrigin()
    {
        // select new bug type
        List<BugType> bugChoices = new List<BugType>();
        bugChoices.Add(BugType.Ladybug);
        bugChoices.Add(BugType.Bee);
        bugChoices.Add(BugType.Light);
        bugChoices.Remove(currentBugType);
        currentBugType = bugChoices[Random.Range(0, 2)];

        transform.position = origin.position;
    }

    public void die()
    {
        StartCoroutine(dieRoutine());
    }

    private IEnumerator dieRoutine()
    {
        yield return new WaitForSeconds(0f);

        // play bug wrap sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WebSwoop, 0.5f);

        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugWrapped");
                break;
            case BugType.Bee:
                animator.Play("BeeWrapped");
                break;
            case BugType.Light:
                animator.Play("LightningWrapped");
                break;
        }
    }

    private IEnumerator landRoutine()
    {
        yield return new WaitForSeconds(1.20f);
        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugLand");
                break;
            case BugType.Bee:
                animator.Play("BeeLand");
                break;
            case BugType.Light:
                animator.Play("LightningLand");
                break;
        }

        yield return new WaitForSeconds(.5f);

        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugStill");
                break;
            case BugType.Bee:
                animator.Play("BeeStill");
                break;
            case BugType.Light:
                animator.Play("LightningStill");
                break;
        }
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

    private IEnumerator takeOffRoutine()
    {
        Vector2 pos = transform.position;
        Vector2 tempPos = pos;
        tempPos.y -= 0.25f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.2f, false);
        yield return new WaitForSeconds(.2f);
        tempPos = pos;
        tempPos.y += 0.15f;

        GetComponent<LerpableObject>().LerpPosition(tempPos, 0.1f, false);
        yield return new WaitForSeconds(.1f);

        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugTakeoff");
                break;
            case BugType.Bee:
                animator.Play("BeeTakeoff");
                break;
            case BugType.Light:
                animator.Play("LightningTakeoff");
                break;
        }

        GetComponent<LerpableObject>().LerpPosition(flyOffScreenPos.position, 0.8f, false);
    }

    public void leaveWeb()
    {
        StartCoroutine(takeOffRoutine());
    }

    public void webGetEat()
    {
        StartCoroutine(webGetEatRoutine());
    }

    private IEnumerator webGetEatRoutine()
    {
        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugWrapped");
                break;
            case BugType.Bee:
                animator.Play("BeeWrapped");
                break;
            case BugType.Light:
                animator.Play("LightningWrapped");
                break;
        }

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
        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugTwitch");
                break;
            case BugType.Bee:
                animator.Play("BeeTwitch");
                break;
            case BugType.Light:
                animator.Play("LightningTwitch");
                break;
        }
        BugBounce();

        switch (currentBugType)
        {
            case BugType.Ladybug:
                animator.Play("LadybugStill");
                break;
            case BugType.Bee:
                animator.Play("BeeStill");
                break;
            case BugType.Light:
                animator.Play("LightningStill");
                break;
        }
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

        // play web bounce sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WebBoing, 0.5f);

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
