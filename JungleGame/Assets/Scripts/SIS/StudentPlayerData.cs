using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StudentIndex
{
    student_1, student_2, student_3
}

public enum LinearGameEvent
{
    InitBoatGame, // 0
    UnlockGorillaVillage, // 1
    WelcomeStoryGame, // 2
    StickerTutorial, // 3
    COUNT
}

[System.Serializable]
public class StudentPlayerData
{
    public string version;
    public StudentIndex studentIndex; // differentiate btwn student profiles
    public bool active; // bool to determine if someone has created this student player
    public string name; // name of student
    public int totalStars; // total number of stars
    public int mapLimit; // how far player can move on map

    public int goldCoins;
    public int sliverCoins;
    // can add many more things here!

    // settings options
    public float masterVol;
    public float musicVol;
    public float fxVol;
    public float talkVol;
    public int micDevice;

    // tutorial bools
    public bool froggerTutorial;
    public bool turntablesTutorial;
    public bool spiderwebTutorial;
    public bool rummageTutorial;

    // game progression
    public LinearGameEvent currGameEvent;

    // map data
    public MapData mapData;
}

[System.Serializable]
public class MapIconData
{
    public bool isFixed;
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
}
