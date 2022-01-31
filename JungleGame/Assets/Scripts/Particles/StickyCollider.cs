using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyCollider : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject axeObject;

    private bool isOn = true;
    private Transform followTransform;

    void OnTriggerEnter2D(Collider2D col)
    {   
        // only stick to sticky tagged stuff
        if(col.tag != "Sticky")
        {
            return;
        }

        // turn off on first collision
        if (!isOn)
            return;
        isOn = false;


        followTransform = col.gameObject.transform;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;

        axeObject.GetComponent<FunParticle>().SetNormalScale();
    }

    void Update()
    {
        if (followTransform != null)
        {
            this.transform.position = followTransform.position;
            this.transform.rotation = followTransform.rotation;
        }
    }
}
