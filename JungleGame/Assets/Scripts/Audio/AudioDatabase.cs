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
    public AudioClip RightChoice;
    public AudioClip WrongChoice;
    public AudioClip WinTune;

    [Header("Coin Drop")]
    public AudioClip[] CoinDropArray;

    [Header("Frogger Game")]
    public AudioClip WaterSplashLarge;
    public AudioClip WaterSplashMed;
    public AudioClip WaterSplashSmall;

    public AudioClip LogRiseLarge;
    public AudioClip LogRiseMed;
    public AudioClip LogRiseSmall;

    public AudioClip WoodThump;
    public AudioClip GrassThump;
}