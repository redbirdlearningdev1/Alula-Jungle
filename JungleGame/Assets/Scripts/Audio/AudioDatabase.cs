using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioDatabase : MonoBehaviour
{
    public static AudioDatabase instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /* 
    ################################################
    #   MUSIC DATABASE
    ################################################
    */

    [Header("Music Database")]
    public AudioClip JungleGameTestSong;

    /* 
    ################################################
    #   FX DATABASE
    ################################################
    */

    [Header("FX Database")]
    public AudioClip testSound1;

    [Header("Frogger Game")]
    public AudioClip waterPlopLarge;
    public AudioClip waterPlopMed;
    public AudioClip waterPlopSmall;
}