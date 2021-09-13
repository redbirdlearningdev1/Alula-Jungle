using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmeraldHead : MonoBehaviour
{
    public static EmeraldHead instance;

    public Animator animator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
