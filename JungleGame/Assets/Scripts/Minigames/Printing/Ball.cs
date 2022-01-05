using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public ActionWordEnum type;
    private Animator animator;

    public Vector3 resetPos;

    [Header("Physics Components")]
    public Rigidbody2D rb;
    public CircleCollider2D colider;

    void Awake()
    {
        animator = GetComponent<Animator>();
        string currType = type.ToString() + "b";
        animator.Play(currType);
    }

    public void SetValue(ActionWordEnum type)
    {
        this.type = type;
        // get animator if null
        if (!animator)
            animator = GetComponent<Animator>();
        animator.Play(type.ToString() + "b");
    }

    public void TogglePhysics(bool opt)
    {
        if (opt)
        {
            rb.isKinematic = false;
            colider.enabled = true;
        }
        else
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
            colider.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            // play sound
            float xVelocityNorm = rb.velocity.normalized.x;
            if (xVelocityNorm > 0.7f)
            {
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CannonDink, 0.25f, "dink_sound", xVelocityNorm * 1.1f);
            }
        }
    }
}
