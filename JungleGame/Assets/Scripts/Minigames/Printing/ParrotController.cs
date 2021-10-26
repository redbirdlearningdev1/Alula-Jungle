using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotController : MonoBehaviour
{
    public static ParrotController instance;

    public Animator animator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
