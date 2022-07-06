using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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
    public AssetReference SplashScreenSong;
    public AssetReference ScrollMapSong;
    public AssetReference WagonWindowSong;

    public List<AssetReference> FroggerSongSplit;
    public List<AssetReference> TurntablesSongSplit;
    public List<AssetReference> RummageSongSplit;
    public List<AssetReference> SpiderwebSongSplit;
    public List<AssetReference> SeashellsSongSplit;
    public List<AssetReference> PirateSongSplit;

    public List<AssetReference> challengeGameSongSplit1;
    public List<AssetReference> challengeGameSongSplit2;

    /* 
    ################################################
    #   BOAT GAME AUDIO DATABASE
    ################################################
    */
    
    [Header("Boat Game Audio")]
    public List<AssetReference> boat_game_audio;

    /* 
    ################################################
    #   STICKER VOICEOVERS DATABASE
    ################################################
    */

    [Header("Sticker Voiceovers Audio")]
    public List<AssetReference> commonStickerVoiceovers;
    public List<AssetReference> uncommonStickerVoiceovers;
    public List<AssetReference> rareStickerVoiceovers;
    public List<AssetReference> legendaryStickerVoiceovers;

    /* 
    ################################################
    #   FX DATABASE
    ################################################
    */
    
    [Header("Universal Sounds")]
    public AssetReference RightChoice;
    public AssetReference WrongChoice;
    public AssetReference WinTune;
    public AssetReference Whoosh;
    public AssetReference DestroyArea;
    public AssetReference Trumpet;
    public AssetReference FastForwardSound;

    [Header("Minigame Wheel Sounds")]
    public AssetReference WheelOpen;
    public AssetReference WheelClose;
    public AssetReference WheelPressed;
    public AssetReference WheelSpinning;
    public AssetReference WheelFinished;
    public AssetReference RoyalRumbleIntro_Julius;
    public AssetReference RoyalRumbleIntro_Monkeys;

    [Header("Sticker System Sounds")]
    public AssetReference CartRollIn;
    public AssetReference CartRollOut;
    public AssetReference StickerReveal_Common;
    public AssetReference StickerReveal_Uncommon;
    public AssetReference StickerReveal_Rare;
    public AssetReference StickerReveal_Legendary;

    [Header("Blips")]
    public AssetReference HappyBlip;
    public AssetReference SadBlip;
    public AssetReference NeutralBlip;
    public AssetReference CreateBlip;
    public AssetReference LeftBlip;
    public AssetReference RightBlip;

    [Header("Boat Game")]
    public AssetReference AmbientEngineRumble;
    public AssetReference AmbientOceanLoop;
    public AssetReference AmbientSeagullsLoop;
    public AssetReference BoatHorn;
    public AssetReference BoatMoveRumble;
    public AssetReference EngineStart;
    public AssetReference FoundIslandSparkle;
    public AssetReference PlacedIslandSplash;
    public AssetReference TurnWheelLeft;
    public AssetReference TurnWheelRight;

    [Header("Coin Drop")]
    public List<AssetReference> CoinDropArray;

    [Header("Frogger Game")]
    public AssetReference WaterSplashLarge;
    public AssetReference WaterSplashMed;
    public AssetReference WaterSplashSmall;

    public AssetReference LogRiseLarge;
    public AssetReference LogRiseMed;
    public AssetReference LogRiseSmall;

    public AssetReference WoodThump;
    public AssetReference GrassThump;

    public AssetReference RiverFlowing;

    [Header("Key Jingles")]
    public List<AssetReference> KeyJingleArray;

    [Header("Turntables Game")]
    public AssetReference BreezeLoop;
    public AssetReference QuarryLoop;
    public AssetReference RocksSlidingLoop;

    public AssetReference ErrieGlow;
    public AssetReference KeyLatch;
    public AssetReference KeyTap;
    public AssetReference KeyUnlock;

    public AssetReference LargeRockSlide;

    public AssetReference MoveStoneStart;
    public AssetReference MoveStoneLoop;
    public AssetReference MoveStoneEnd;

    [Header("Seashell Game")]
    public AssetReference BubbleRise;
    public AssetReference CoinFlip;
    public AssetReference CoinOnRock;
    public AssetReference Conch1;
    public AssetReference Conch2;
    public AssetReference Pop;
    public AssetReference SandDrop;
    public AssetReference SeaAmbiance;
    public AssetReference WaterRipples;
    public AssetReference WaterSplash;
    public AssetReference WaveCrash;

    [Header("Pirate Game")]
    public AssetReference BirdWingFlap;
    public AssetReference CannonDink;
    public AssetReference CannonFall;
    public AssetReference CannonHitCoin;
    public AssetReference CannonShoot;
    public AssetReference CannonLoad;
    public AssetReference RopeDown;
    public AssetReference RopeUp;

    [Header("Rummage Game")]
    public AssetReference ForestAmbiance;
    public AssetReference HealFixItem;
    public AssetReference ScrollRoll;
    public AssetReference WalkGrass;
    public AssetReference WoodRummage;

    [Header("Spiderweb Game")]
    public AssetReference BugFlyIn;
    public AssetReference BugFlyOut;
    public AssetReference WebBoing;
    public AssetReference WebSwoop;
    public AssetReference WebWhip;


    [Header("Word Factory Games")]
    public AssetReference BoxSlide;
    public AssetReference CameraClick;
    public AssetReference CoinDink;
    public AssetReference CoinRattle;
    public AssetReference EmeraldSlide;
    public AssetReference EmeraldSlideShort;
    public AssetReference GlassDink1;
    public AssetReference GlassDink2;
    public AssetReference LeaveWater;
    public AssetReference MagicReveal;
    public AssetReference MedWhoosh;
    public AssetReference PanDown;
    public AssetReference PanUp;
    public AssetReference PolaroidCrunch;
    public AssetReference PolaroidFall;
    public AssetReference PolaroidRattle;
    public AssetReference PolaroidUnravel;
    public AssetReference SelectBoop;
    public AssetReference SmallWhoosh;
    public AssetReference TigerSwipe;

    [Header("Tiger Paw Games")]
    public AssetReference CoinsSlideIn;
    public AssetReference CoinsSlideOut;
    public AssetReference PhotosSlideIn;
    public AssetReference PhotosSlideOut;
    public AssetReference GlitterLoop;

    [Header("Password Game")]
    public AssetReference PasswordInitRound;
    public AssetReference PasswordNewRound;
    public AssetReference PasswordWrongRound;
}