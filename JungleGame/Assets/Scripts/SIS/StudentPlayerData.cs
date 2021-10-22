﻿using System.Collections;
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

    // coins
    public int goldCoins;

    // settings options
    public float masterVol;
    public float musicVol;
    public float fxVol;
    public float talkVol;
    public int micDevice;

    // tutorial bools
    public bool stickerTutorial;
    public bool froggerTutorial;
    public bool turntablesTutorial;
    public bool spiderwebTutorial;
    public bool rummageTutorial;

    // game progression
    public StoryBeat currStoryBeat;
    public bool unlockedStickerButton;
    public bool firstTimeLoseChallengeGame;
    public bool everyOtherTimeLoseChallengeGame;

    public List<ActionWordEnum> actionWordPool;
    public List<ChallengeWord> challengeWordPool;

    // map data
    public Chapter currentChapter; // completed chapter
    public int mapLimit; // how far player can move on scroll map
    public MapData mapData;

    // sticker data
    public List<InventoryStickerData> stickerInventory;
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
    MudslideRebuilt, // 14

    OrcVillageMeetClogg, // 15
    OrcVillageUnlocked, // 16
    OrcVillage_challengeGame_1, // 17
    OrcVillage_challengeGame_2, // 18
    OrcVillage_challengeGame_3, // 19
    OrcVillageRebuilt, // 20
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
    chapter_6,
    endGame_7
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
        this.count = 0;
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
