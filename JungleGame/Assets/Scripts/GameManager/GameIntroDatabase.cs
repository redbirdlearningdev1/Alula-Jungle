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

    [Header("Seashell Game")]
    public AudioClip seashellIntro1;
    public AudioClip seashellIntro2;
    public AudioClip seashellIntro3;
    public AudioClip seashellIntro4;
    public AudioClip seashellIntro5;

    public AudioClip seashellReminder1;
    public AudioClip seashellReminder2;

    public List<AudioClip> seashellEncouragementClips;

    [Header("Turntables Game")]
    public AudioClip turntablesIntro1;
    public AudioClip turntablesIntro2;
    public AudioClip turntablesIntro3;
    public AudioClip turntablesIntro4;

    public AudioClip turntablesReminder1;
    public AudioClip turntablesReminder2;
    public AudioClip turntablesReminder3;

    public List<AudioClip> turntablesEncouragementClips;

    [Header("Rummage Game")]
    public AudioClip rummageIntro1;
    public AudioClip rummageIntro2;
    public AudioClip rummageIntro3;
    public AudioClip rummageIntro4;
    public AudioClip rummageIntro5;

    public AudioClip rummageReminder1;
    public AudioClip rummageReminder2;
    public AudioClip rummageReminder3;

    public List<AudioClip> rummageEncouragementClips;

    [Header("Spiderwebs Game")]
    public AudioClip spiderwebsIntro1;
    public AudioClip spiderwebsIntro2;
    public AudioClip spiderwebsIntro3;
    public AudioClip spiderwebsIntro4;
    public AudioClip spiderwebsIntro5;

    public AudioClip spiderwebsReminder1;
    public AudioClip spiderwebsReminder2;

    public List<AudioClip> spiderwebsEncouragementClips;
}
