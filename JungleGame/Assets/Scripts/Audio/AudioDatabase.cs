﻿using System.Collections;
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
    public AudioClip MainThemeSong;
    public AudioClip FroggerGameSong;
    public AudioClip TurntablesGameSong;

    /* 
    ################################################
    #   TUTORIAL AUDIO DATABASE
    ################################################
    */

    [Header("Frogger Tutorial")]
    public AudioClip FroggerTutorial_1;
    public AudioClip FroggerTutorial_2;
    public AudioClip FroggerTutorial_3;

    [Header("Turntables Tutorial")]
    public AudioClip TurntablesTutorial_1;
    public AudioClip TurntablesTutorial_2;
    public AudioClip TurntablesTutorial_3;
    public AudioClip TurntablesTutorial_4;

    [Header("Rummage Tutorial")]
    public AudioClip RummageTutorial_1;
    public AudioClip RummageTutorial_2;
    public AudioClip RummageTutorial_3;

    [Header("Spiderweb Tutorial")]
    public AudioClip SpiderwebTutorial_1;
    public AudioClip SpiderwebTutorial_2;
    public AudioClip SpiderwebTutorial_3;
    public AudioClip SpiderwebTutorial_4;

    /* 
    ################################################
    #   BOAT GAME AUDIO DATABASE
    ################################################
    */
    
    [Header("Boat Game Audio")]
    public AudioClip[] boat_game_audio;

    /* 
    ################################################
    #   FX DATABASE
    ################################################
    */

    [Header("FX Database")]
    public AudioClip testSound1;

    [Header("Universal Sounds")]
    public AudioClip RightChoice;
    public AudioClip WrongChoice;
    public AudioClip WinTune;
    public AudioClip Whoosh;

    [Header("Blips")]
    public AudioClip HappyBlip;
    public AudioClip SadBlip;
    public AudioClip NeutralBlip;
    public AudioClip CreateBlip;
    public AudioClip LeftBlip;
    public AudioClip RightBlip;

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

    public AudioClip RiverFlowing;

    [Header("Key Jingles")]
    public AudioClip[] KeyJingleArray;

    [Header("Turntables Game")]
    public AudioClip BreezeLoop;
    public AudioClip QuarryLoop;
    public AudioClip RocksSlidingLoop;

    public AudioClip ErrieGlow;
    public AudioClip KeyLatch;
    public AudioClip KeyTap;
    public AudioClip KeyUnlock;

    public AudioClip LargeRockSlide;

    public AudioClip MoveStoneStart;
    public AudioClip MoveStoneLoop;
    public AudioClip MoveStoneEnd;
}