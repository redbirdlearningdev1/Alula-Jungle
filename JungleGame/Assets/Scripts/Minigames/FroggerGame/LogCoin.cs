using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogCoin : MonoBehaviour
{
    public ActionWordEnum type;
    public int logIndex;
    public Transform logPos;
    public Transform logParent;
    public float moveSpeed = 5f;

    private Animator animator;
    private BoxCollider2D myCollider;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public GlowOutlineController glowController;
    private bool audioPlaying;

    // original vars
    private bool originalSet = false;

    void Awake() 
    {
        animator = GetComponent<Animator>();
        //print (type.ToString());
        animator.Play(type.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        spriteRenderer = GetComponent<SpriteRenderer>();
        glowController = GetComponent<GlowOutlineController>();
    }

    /* 
    ################################################
    #   POSITION FUNCTIONS
    ################################################
    */

    public void ReturnToLog()
    {
        StartCoroutine(ReturnToOriginalPosRoutine(logPos.position));
    }

    private IEnumerator ReturnToOriginalPosRoutine(Vector3 target)
    {
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
                transform.SetParent(logParent);
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

    /* 
    ################################################
    #   AUDIO FUNCTIONS
    ################################################
    */

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

    /* 
    ################################################
    #   VISIBILITY FUNCTIONS
    ################################################
    */

    public void ToggleVisibility(bool opt, bool smooth)
    {
        if (smooth)
            StartCoroutine(ToggleVisibilityRoutine(opt));
        else
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
            Color temp = spriteRenderer.color;
            if (opt) { temp.a = 1f; }
            else {temp.a = 0; }
            spriteRenderer.color = temp;
        }
    }   

    private IEnumerator ToggleVisibilityRoutine(bool opt)
    {
        float end = 0f;
        if (opt) { end = 1f; }
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            Color temp = spriteRenderer.color;
            temp.a = Mathf.Lerp(temp.a, end, timer);
            spriteRenderer.color = temp;

            if (spriteRenderer.color.a == end)
            {
                break;
            }
            yield return null;
        }
    }
}
