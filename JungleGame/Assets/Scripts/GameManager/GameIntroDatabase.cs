using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroDatabase : MonoBehaviour
{
    public static GameIntroDatabase instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /* 
    ################################################
    #   FROGGER DATABASE
    ################################################
    */

    [Header("Frogger Game")]
    public AudioClip froggerIntro1;
    public AudioClip froggerIntro2;
    public AudioClip froggerIntro3;
    public AudioClip froggerIntro4;
    public AudioClip froggerIntro5;

    public AudioClip froggerReminder1;
    public AudioClip froggerReminder2;

    public AudioClip froggerEncouragement1;
    public AudioClip froggerEncouragement2;
    public AudioClip froggerEncouragement3;
    public AudioClip froggerEncouragement4;
}
