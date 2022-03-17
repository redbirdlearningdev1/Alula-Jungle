using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StudentPlayerData
{
    public string version;
    public StudentIndex studentIndex; // differentiate btwn student profiles
    public bool active; // bool to determine if someone has created this student player
    public bool mostRecentProfile; // is this the most recently opend profile?
    public string name; // name of student
    public int minigamesPlayed;
    
    public GameType lastGamePlayed;
    public GameType gameBeforeLastPlayed;

    public int starsLastGamePlayed;
    public int starsGameBeforeLastPlayed;

    // Track of Stars in minigames
    public int starsFrogger;
    public int starsSeashell;
    public int starsRummage;
    public int starsTurntables;
    public int starsPirate;
    public int starsSpiderweb;

    public int totalStarsFrogger;
    public int totalStarsSeashell;
    public int totalStarsRummage;
    public int totalStarsTurntables;
    public int totalStarsPirate;
    public int totalStarsSpiderweb;

    // Track of Stars in Challengegame
    
    public int starsBlend;
    public int starsSub;
    public int starsDel;
    public int starsBuild;
    public int starsPass;
    public int starsTPawCoin;
    public int starsTPawPol;

    public int blendPlayed;
    public int subPlayed;
    public int delPlayed;
    public int buildPlayed;
    public int passPlayed;
    public int tPawCoinPlayed;
    public int tPawPolPlayed;

    public int tPawCoinCounter;

    public int rRumblePlayed;

    public ChallengeWord lastWordFaced;
    public WordPair lastWordPairFaced;

    //public int totalStarsPotential;
    public int profileAvatar;

    // coins
    public int goldCoins;

    // settings options
    public float masterVol;
    public float musicVol;
    public float fxVol;
    public float talkVol;
    public int micDevice;
    // talkie options
    public bool talkieSubtitles;
    public bool talkieFast;
    public bool talkieParticles;


    // tutorial bools
    public bool stickerTutorial;
    public bool froggerTutorial;
    public bool turntablesTutorial;
    public bool spiderwebTutorial;
    public bool rummageTutorial;
    public bool pirateTutorial;
    public bool seashellTutorial;

    public bool wordFactoryBlendingTutorial;
    public bool wordFactoryBuildingTutorial;
    public bool wordFactoryDeletingTutorial;
    public bool wordFactorySubstitutingTutorial;
    public bool tigerPawCoinsTutorial;
    public bool tigerPawPhotosTutorial;
    public bool passwordTutorial;

    // game progression
    public StoryBeat currStoryBeat;
    public BoatEncounter currBoatEncounter;
    public bool unlockedStickerButton;

    public bool firstTimeLoseChallengeGame;
    public bool everyOtherTimeLoseChallengeGame;

    public bool firstTimeLoseBossBattle;
    public bool everyOtherTimeLoseBossBattle;
    public int bossBattlePoints;
    public GameType[] bossBattleGameQueue;

    public bool firstGuradsRoyalRumble;
    public List<ActionWordEnum> actionWordPool;

    // royal rumble data
    public bool royalRumbleActive;
    public MapIconIdentfier royalRumbleID;
    public GameType royalRumbleGame;

    // map data
    public Chapter currentChapter; // completed chapter
    public int mapLimit; // how far player can move on scroll map
    public MapData mapData;

    // sticker data
    public List<InventoryStickerData> stickerInventory;
    // unlocked stickers
    public bool[] commonStickerUnlocked;
    public bool[] uncommonStickerUnlocked; 
    public bool[] rareStickerUnlocked; 
    public bool[] legendaryStickerUnlocked;
    // sticker boards
    public StickerBoardData classicStickerBoard;
    public StickerBoardData mossyStickerBoard;
    public StickerBoardData emeraldStickerBoard;
    public StickerBoardData beachStickerBoard;
}

/* 
################################################
#   PROFILE + STORY DATA
################################################
*/

public enum BoatEncounter
{
    FirstTime,
    SecondTime,
    EveryOtherTime,
}

public enum StudentIndex
{
    student_1, student_2, student_3
}

public enum StoryBeat
{
    InitBoatGame, // 0
    UnlockGorillaVillage, // 1
    
    GorillaVillageIntro, // 2
    PrologueStoryGame, // 3
    RedShowsStickerButton, // 4
    VillageRebuilt, // 5  
    GorillaVillage_challengeGame_1, // 6
    GorillaVillage_challengeGame_2, // 7
    GorillaVillage_challengeGame_3, // 8
    VillageChallengeDefeated, // 9

    MudslideUnlocked, // 10
    Mudslide_challengeGame_1, // 11
    Mudslide_challengeGame_2, // 12
    Mudslide_challengeGame_3, // 13
    MudslideDefeated, // 14

    OrcVillageMeetClogg, // 15
    OrcVillageUnlocked, // 16
    OrcVillage_challengeGame_1, // 17
    OrcVillage_challengeGame_2, // 18
    OrcVillage_challengeGame_3, // 19
    OrcVillageDefeated, // 20

    SpookyForestUnlocked, // 21
    BeginningStoryGame, // 22
    SpookyForestPlayGames, // 23
    SpookyForest_challengeGame_1, // 24
    SpookyForest_challengeGame_2, // 25
    SpookyForest_challengeGame_3, // 26
    SpookyForestDefeated, // 27

    OrcCampUnlocked, // 28
    OrcCampPlayGames, // 29
    OrcCamp_challengeGame_1, // 30
    OrcCamp_challengeGame_2, // 31
    OrcCamp_challengeGame_3, // 32
    OrcCampDefeated, // 33

    GorillaPoopPlayGames, // 34
    GorillaPoop_challengeGame_1, // 35
    GorillaPoop_challengeGame_2, // 36
    GorillaPoop_challengeGame_3, // 37
    GorillaPoopDefeated, // 38

    WindyCliffUnlocked, // 39
    FollowRedStoryGame, // 40
    WindyCliffPlayGames, // 41
    WindyCliff_challengeGame_1, // 42
    WindyCliff_challengeGame_2, // 43
    WindyCliff_challengeGame_3, // 45
    WindyCliffDefeated, // 46
    
    PirateShipPlayGames, // 47
    PirateShip_challengeGame_1, // 48
    PirateShip_challengeGame_2, // 49
    PirateShip_challengeGame_3, // 50
    PirateShipDefeated, // 51

    MermaidBeachUnlocked, // 52
    EmergingStoryGame, // 53
    MermaidBeachPlayGames, // 54
    MermaidBeach_challengeGame_1, // 55
    MermaidBeach_challengeGame_2, // 56
    MermaidBeach_challengeGame_3, // 57
    MermaidBeachDefeated, // 58

    RuinsPlayGames, // 59
    Ruins_challengeGame_1, // 61
    Ruins_challengeGame_2, // 62
    Ruins_challengeGame_3, // 63
    RuinsDefeated, // 64

    ExitJungleUnlocked, // 65
    ResolutionStoryGame, // 66
    ExitJunglePlayGames, // 67
    ExitJungle_challengeGame_1, // 68
    ExitJungle_challengeGame_2, // 69
    ExitJungle_challengeGame_3, // 70
    ExitJungleDefeated, // 71

    GorillaStudyUnlocked, // 72
    GorillaStudyPlayGames, // 73
    GorillaStudy_challengeGame_1, // 74
    GorillaStudy_challengeGame_2, // 75
    GorillaStudy_challengeGame_3, // 76
    GorillaStudyDefeated, // 77

    MonkeysPlayGames, // 78
    Monkeys_challengeGame_1, // 79
    Monkeys_challengeGame_2, // 80
    Monkeys_challengeGame_3, // 81
    MonkeysDefeated, // 82

    PalaceIntro,  // 83
    PreBossBattle, // 84
    BossBattle1, // 85
    BossBattle2, // 86
    BossBattle3, // 87
    EndBossBattle, // 88

    COUNT
}

/* 
################################################
#   MAP DATA
################################################
*/

[System.Serializable]
public enum Chapter
{
    chapter_0,
    chapter_1,
    chapter_2,
    chapter_3,
    chapter_4,
    chapter_5,
    chapter_final
}

[System.Serializable]
public class MapIconData
{
    public bool isFixed;
    public int stars;
}

[System.Serializable]
public class ChallengeGameData
{
    public GameType gameType;
    public int stars;
}

[System.Serializable]
public class MapData
{
    // gorilla village
    public MapIconData GV_house1;
    public MapIconData GV_house2;
    public MapIconData GV_fire;
    public MapIconData GV_statue;

    public ChallengeGameData GV_challenge1;
    public ChallengeGameData GV_challenge2;
    public ChallengeGameData GV_challenge3;

    public bool GV_signPost_unlocked;
    public int GV_signPost_stars;

    // mudslide
    public MapIconData MS_logs;
    public MapIconData MS_pond;
    public MapIconData MS_ramp;
    public MapIconData MS_tower;

    public ChallengeGameData MS_challenge1;
    public ChallengeGameData MS_challenge2;
    public ChallengeGameData MS_challenge3;

    public bool MS_signPost_unlocked;
    public int MS_signPost_stars;

    // orc village
    public MapIconData OV_houseL;
    public MapIconData OV_houseS;
    public MapIconData OV_statue;
    public MapIconData OV_fire;

    public ChallengeGameData OV_challenge1;
    public ChallengeGameData OV_challenge2;
    public ChallengeGameData OV_challenge3;

    public bool OV_signPost_unlocked;
    public int OV_signPost_stars;

    // spooky forest
    public MapIconData SF_web;
    public MapIconData SF_shrine;
    public MapIconData SF_lamp;
    public MapIconData SF_spider;

    public ChallengeGameData SF_challenge1;
    public ChallengeGameData SF_challenge2;
    public ChallengeGameData SF_challenge3;

    public bool SF_signPost_unlocked;
    public int SF_signPost_stars;

    // orc camp
    public MapIconData OC_axe;
    public MapIconData OC_bigTent;
    public MapIconData OC_smallTent;
    public MapIconData OC_fire;

    public ChallengeGameData OC_challenge1;
    public ChallengeGameData OC_challenge2;
    public ChallengeGameData OC_challenge3;

    public bool OC_signPost_unlocked;
    public int OC_signPost_stars;

    // gorilla poop
    public MapIconData GP_house1;
    public MapIconData GP_house2;
    public MapIconData GP_rock1;
    public MapIconData GP_rock2;

    public ChallengeGameData GP_challenge1;
    public ChallengeGameData GP_challenge2;
    public ChallengeGameData GP_challenge3;

    public bool GP_signPost_unlocked;
    public int GP_signPost_stars;

    // windy cliff
    public MapIconData WC_statue;
    public MapIconData WC_lighthouse;
    public MapIconData WC_ladder;
    public MapIconData WC_rock;
    public MapIconData WC_sign;
    public MapIconData WC_octo;

    public ChallengeGameData WC_challenge1;
    public ChallengeGameData WC_challenge2;
    public ChallengeGameData WC_challenge3;

    public bool WC_signPost_unlocked;
    public int WC_signPost_stars;

    // pirate ship
    public MapIconData PS_wheel;
    public MapIconData PS_sail;
    public MapIconData PS_boat;
    public MapIconData PS_bridge;
    public MapIconData PS_front;
    public MapIconData PS_parrot;

    public ChallengeGameData PS_challenge1;
    public ChallengeGameData PS_challenge2;
    public ChallengeGameData PS_challenge3;

    public bool PS_signPost_unlocked;
    public int PS_signPost_stars;

    // mermaid beach
    public MapIconData MB_mermaids;
    public MapIconData MB_rock;
    public MapIconData MB_castle;
    public MapIconData MB_bucket;
    public MapIconData MB_umbrella;
    public MapIconData MB_ladder;

    public ChallengeGameData MB_challenge1;
    public ChallengeGameData MB_challenge2;
    public ChallengeGameData MB_challenge3;

    public bool MB_signPost_unlocked;
    public int MB_signPost_stars;

    // ruins 1 + 2
    public MapIconData R_lizard1;
    public MapIconData R_lizard2;
    public MapIconData R_caveRock;
    public MapIconData R_pyramid;
    public MapIconData R_face;
    public MapIconData R_arch;

    public ChallengeGameData R_challenge1;
    public ChallengeGameData R_challenge2;
    public ChallengeGameData R_challenge3;

    public bool R_signPost_unlocked;
    public int R_signPost_stars;

    // exit surgery
    public MapIconData EJ_puppy;
    public MapIconData EJ_bridge;
    public MapIconData EJ_sign;
    public MapIconData EJ_torch;

    public ChallengeGameData EJ_challenge1;
    public ChallengeGameData EJ_challenge2;
    public ChallengeGameData EJ_challenge3;

    public bool EJ_signPost_unlocked;
    public int EJ_signPost_stars;

    // gorilla study
    public MapIconData GS_tent1;
    public MapIconData GS_tent2;
    public MapIconData GS_statue;
    public MapIconData GS_fire;

    public ChallengeGameData GS_challenge1;
    public ChallengeGameData GS_challenge2;
    public ChallengeGameData GS_challenge3;

    public bool GS_signPost_unlocked;
    public int GS_signPost_stars;

    // monkeys
    public MapIconData M_flower;
    public MapIconData M_tree;
    public MapIconData M_bananas;
    public MapIconData M_guards;

    public ChallengeGameData M_challenge1;
    public ChallengeGameData M_challenge2;
    public ChallengeGameData M_challenge3;

    public bool M_signPost_unlocked;
    public int M_signPost_stars;
}

/* 
################################################
#   STICKER DATA
################################################
*/

[System.Serializable]
public class InventoryStickerData
{
    public StickerRarity rarity;
    public int id;
    public int count;

    public InventoryStickerData(Sticker sticker)
    {
        this.rarity = sticker.rarity;
        this.id = sticker.id;
        this.count = 1;
    }
}

public enum StickerBoardType
{
    Classic, Mossy, Emerald, Beach
}

[System.Serializable]
public class StickerBoardData
{
    public StickerBoardType boardType; // what board is it ?
    public bool active; // is the board purchased and unlocked
    public List<StickerData> stickers; // stickers on the board
}

[System.Serializable]
public class StickerData
{
    public StickerRarity rarity;
    public int id;
    public Vector2 boardPos; // where on the board is it located ?
}
