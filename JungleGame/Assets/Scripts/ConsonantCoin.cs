using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsonantEnum {
    _blank,

    b,
    c,
    ch,
    d,
    f,
    g,
    h,
    j,
    k,
    l,
    m,
    n,
    p,
    qu,
    r,
    s, 
    sh,
    t,
    th,
    v,
    w,
    x,
    y,
    z,

    SIZE
}

public class ConsonantCoin : MonoBehaviour
{
    public ConsonantEnum type;
    private Animator animator;
    private BoxCollider2D myCollider;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public GlowOutlineController glowController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.Play(type.ToString());

        RectTransform rt = GetComponent<RectTransform>();
        myCollider = gameObject.AddComponent<BoxCollider2D>();
        myCollider.size = rt.sizeDelta;

        spriteRenderer = GetComponent<SpriteRenderer>();
        glowController = GetComponent<GlowOutlineController>();
    }

    public void SetCoinType(ConsonantEnum type)
    {
        this.type = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();
        animator.Play(type.ToString());
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

    public void SetTransparency(float alpha, bool smooth)
    {
        if (smooth)
            StartCoroutine(SetTransparencyRoutine(alpha));
        else
        {
            if (!spriteRenderer)
                spriteRenderer = GetComponent<SpriteRenderer>();
            Color temp = spriteRenderer.color;
            temp.a = alpha;
            spriteRenderer.color = temp;
        }
    }

    private IEnumerator SetTransparencyRoutine(float alpha)
    {
        float end = alpha;
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
