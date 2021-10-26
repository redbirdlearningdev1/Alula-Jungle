using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public static CannonController instance;

    public WiggleController wiggleController;

    public Animator cannonAnimator;
    public Animator explosionAnimator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
