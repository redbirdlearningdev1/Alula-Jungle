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
    public AudioClip MainThemeSong;
    public AudioClip FroggerGameSong;
    public AudioClip TurntablesGameSong;
    public AudioClip Sunrise_LouieZong;

    public AudioClip[] FroggerSongSplit;
    public AudioClip[] TurntablesSongSplit;
    public AudioClip[] RummageSongSplit;
    public AudioClip[] SpiderwebSongSplit;
    public AudioClip[] SeashellsSongSplit;
    public AudioClip[] PirateSongSplit;

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
    #   STICKER VOICEOVERS DATABASE
    ################################################
    */

    [Header("Sticker Voiceovers Audio")]
    public List<AudioClip> commonStickerVoiceovers;
    public List<AudioClip> uncommonStickerVoiceovers;
    public List<AudioClip> rareStickerVoiceovers;
    public List<AudioClip> legendaryStickerVoiceovers;

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
    public AudioClip DestroyArea;
    public AudioClip Trumpet;

    [Header("Blips")]
    public AudioClip HappyBlip;
    public AudioClip SadBlip;
    public AudioClip NeutralBlip;
    public AudioClip CreateBlip;
    public AudioClip LeftBlip;
    public AudioClip RightBlip;

    [Header("Boat Game")]
    public AudioClip AmbientEngineRumble;
    public AudioClip AmbientOceanLoop;
    public AudioClip AmbientSeagullsLoop;
    public AudioClip BoatHorn;
    public AudioClip BoatMoveRumble;
    public AudioClip EngineStart;
    public AudioClip FoundIslandSparkle;
    public AudioClip PlacedIslandSplash;
    public AudioClip TurnWheelLeft;
    public AudioClip TurnWheelRight;

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

    [Header("Seashell Game")]
    public AudioClip BubbleRise;
    public AudioClip CoinFlip;
    public AudioClip CoinOnRock;
    public AudioClip Conch1;
    public AudioClip Conch2;
    public AudioClip Pop;
    public AudioClip SandDrop;
    public AudioClip SeaAmbiance;
    public AudioClip WaterRipples;
    public AudioClip WaterSplash;
    public AudioClip WaveCrash;

    [Header("Pirate Game")]
    public AudioClip BirdWingFlap;
    public AudioClip CannonDink;
    public AudioClip CannonFall;
    public AudioClip CannonHitCoin;
    public AudioClip CannonShoot;
    public AudioClip CannonLoad;
    public AudioClip RopeDown;
    public AudioClip RopeUp;

    [Header("Rummage Game")]
    public AudioClip ForestAmbiance;
    public AudioClip HealFixItem;
    public AudioClip ScrollRoll;
    public AudioClip WalkGrass;
    public AudioClip WoodRummage;

    [Header("Spiderweb Game")]
    public AudioClip BugFlyIn;
    public AudioClip BugFlyOut;
    public AudioClip WebBoing;
    public AudioClip WebSwoop;
    public AudioClip WebWhip;


    [Header("Word Factory Games")]
    public AudioClip BoxSlide;
    public AudioClip CameraClick;
    public AudioClip CoinDink;
    public AudioClip CoinRattle;
    public AudioClip EmeraldSlide;
    public AudioClip EmeraldSlideShort;
    public AudioClip GlassDink1;
    public AudioClip GlassDink2;
    public AudioClip LeaveWater;
    public AudioClip MagicReveal;
    public AudioClip MedWhoosh;
    public AudioClip PanDown;
    public AudioClip PanUp;
    public AudioClip PolaroidCrunch;
    public AudioClip PolaroidFall;
    public AudioClip PolaroidRattle;
    public AudioClip PolaroidUnravel;
    public AudioClip SelectBoop;
    public AudioClip SmallWhoosh;
    public AudioClip TigerSwipe;
}