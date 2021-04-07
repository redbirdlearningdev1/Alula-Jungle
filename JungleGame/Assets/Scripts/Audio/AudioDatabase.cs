using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Song
{
    JungleGameTestSong,
    COUNT
}

public enum Phoneme
{
    a_Phoneme,
    aPhoneme,
    airPhoneme,
    arePhoneme,
    awPhoneme,
    e_Phoneme,
    earPhoneme,
    ePhoneme,
    erPhoneme,
    i_Phoneme,
    iPhoneme,
    o_Phoneme,
    oPhoneme,
    oiPhoneme,
    ooPhoneme,
    orPhoneme,
    owPhoneme,
    u_Phoneme,
    uPhoneme,
    yerPhoneme,
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
    [SerializeField] private AudioClip a_Phoneme;
    [SerializeField] private AudioClip aPhoneme;
    [SerializeField] private AudioClip airPhoneme;
    [SerializeField] private AudioClip arePhoneme;
    [SerializeField] private AudioClip awPhoneme;
    [SerializeField] private AudioClip e_Phoneme;
    [SerializeField] private AudioClip earPhoneme;
    [SerializeField] private AudioClip ePhoneme;
    [SerializeField] private AudioClip erPhoneme;
    [SerializeField] private AudioClip i_Phoneme;
    [SerializeField] private AudioClip iPhoneme;
    [SerializeField] private AudioClip o_Phoneme;
    [SerializeField] private AudioClip oPhoneme;
    [SerializeField] private AudioClip oiPhoneme;
    [SerializeField] private AudioClip ooPhoneme;
    [SerializeField] private AudioClip orPhoneme;
    [SerializeField] private AudioClip owPhoneme;
    [SerializeField] private AudioClip u_Phoneme;
    [SerializeField] private AudioClip uPhoneme;
    [SerializeField] private AudioClip yerPhoneme;

    public AudioClip GetPhonemeFromEnum(Phoneme phoneme)
    {
        switch (phoneme)
        {
            default:
            case Phoneme.a_Phoneme:
                return a_Phoneme;
            case Phoneme.aPhoneme:
                return aPhoneme;
            case Phoneme.airPhoneme:
                return airPhoneme;
            case Phoneme.arePhoneme:
                return arePhoneme;
            case Phoneme.awPhoneme:
                return awPhoneme;
            case Phoneme.e_Phoneme:
                return e_Phoneme;
            case Phoneme.earPhoneme:
                return earPhoneme;
            case Phoneme.ePhoneme:
                return ePhoneme;
            case Phoneme.erPhoneme:
                return erPhoneme;
            case Phoneme.i_Phoneme:
                return i_Phoneme;
            case Phoneme.iPhoneme:
                return iPhoneme;
            case Phoneme.o_Phoneme:
                return o_Phoneme;
            case Phoneme.oPhoneme:
                return oPhoneme;
            case Phoneme.oiPhoneme:
                return oiPhoneme;
            case Phoneme.ooPhoneme:
                return ooPhoneme;
            case Phoneme.orPhoneme:
                return orPhoneme;
            case Phoneme.owPhoneme:
                return owPhoneme;
            case Phoneme.u_Phoneme:
                return u_Phoneme;
            case Phoneme.uPhoneme:
                return uPhoneme;
            case Phoneme.yerPhoneme:
                return yerPhoneme;
        }
    }
}