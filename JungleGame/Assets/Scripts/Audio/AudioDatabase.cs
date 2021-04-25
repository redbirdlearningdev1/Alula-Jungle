using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Song
{
    JungleGameTestSong,
    COUNT
}

public class AudioDatabase : MonoBehaviour
{
    /* 
    ################################################
    #   MUSIC DATABASE
    ################################################
    */

    [Header("Music Database")]
    [SerializeField] private AudioClip JungleGameTestSong;

    public AudioClip GetSongFromEnum(Song song)
    {
        switch (song)
        {
            default:
            case Song.JungleGameTestSong:
                return JungleGameTestSong;
        }
    }

    /* 
    ################################################
    #   PHONEME DATABASE
    ################################################
    */

    [Header("Phoneme Database")]
    [SerializeField] private AudioClip mudslide;
    [SerializeField] private AudioClip listen;
    [SerializeField] private AudioClip poop;
    [SerializeField] private AudioClip orcs;
    [SerializeField] private AudioClip think;

    [SerializeField] private AudioClip hello;
    [SerializeField] private AudioClip spider;
    [SerializeField] private AudioClip explorer;
    [SerializeField] private AudioClip scared;
    [SerializeField] private AudioClip thatguy;

    [SerializeField] private AudioClip choice;
    [SerializeField] private AudioClip strongwind;
    [SerializeField] private AudioClip pirate;
    [SerializeField] private AudioClip gorilla;
    [SerializeField] private AudioClip sounds;
    [SerializeField] private AudioClip give;

    [SerializeField] private AudioClip backpack;
    [SerializeField] private AudioClip frustrating;
    [SerializeField] private AudioClip bumphead;
    [SerializeField] private AudioClip baby;

    public AudioClip GetPhonemeFromEnum(ActionWordEnum type)
    {
        switch (type)
        {
            default:
            case ActionWordEnum.mudslide:
                return mudslide;
            case ActionWordEnum.listen:
                return listen;
            case ActionWordEnum.poop:
                return poop;
            case ActionWordEnum.orcs:
                return orcs;
            case ActionWordEnum.think:
                return think;

            case ActionWordEnum.hello:
                return hello;
            case ActionWordEnum.spider:
                return spider;
            case ActionWordEnum.explorer:
                return explorer;
            case ActionWordEnum.scared:
                return scared;
            case ActionWordEnum.thatguy:
                return thatguy;

            case ActionWordEnum.choice:
                return choice;
            case ActionWordEnum.strongwind:
                return strongwind;
            case ActionWordEnum.pirate:
                return pirate;
            case ActionWordEnum.gorilla:
                return gorilla;
            case ActionWordEnum.sounds:
                return sounds;
            case ActionWordEnum.give:
                return give;

            case ActionWordEnum.backpack:
                return backpack;
            case ActionWordEnum.frustrating:
                return frustrating;
            case ActionWordEnum.bumphead:
                return bumphead;
            case ActionWordEnum.baby:
                return baby;
        }
    }
}