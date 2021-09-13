using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StudentPlayerData
{
    public string version;
    public StudentIndex studentIndex; // differentiate btwn student profiles
    public bool active; // bool to determine if someone has created this student player
    public string name; // name of student
    public int totalStars; // total number of stars
    public int mapLimit; // how far player can move on map

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

    // map data
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
    COUNT
}

/* 
################################################
#   MAP DATA
################################################
*/

[System.Serializable]
public class MapIconData
{
    public bool isFixed;
    public int stars;
}

[System.Serializable]
public class ChallengeGameData
{
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
