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
    #   IN-GAME AUDIO DATABASE
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

    public List<AudioClip> froggerEncouragementClips;

    [Header("Pirate Game")]
    public AudioClip pirateIntro1;
    public AudioClip pirateIntro2;
    public AudioClip pirateIntro3;
    public AudioClip pirateIntro4;
    public AudioClip pirateIntro5;

    public AudioClip pirateReminder1;
    public AudioClip pirateReminder2;

    public List<AudioClip> pirateEncouragementClips;
}
