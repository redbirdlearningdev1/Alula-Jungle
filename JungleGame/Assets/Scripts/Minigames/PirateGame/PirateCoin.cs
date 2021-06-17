using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateCoin : MonoBehaviour
{
    public ActionWordEnum type;
    public int logIndex;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public GlowOutlineController glowController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        glowController = GetComponent<GlowOutlineController>();
        SetCoinType(type);
    }

    public void SetCoinType(ActionWordEnum type)
    {
        this.type = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();
        animator.Play(type.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
