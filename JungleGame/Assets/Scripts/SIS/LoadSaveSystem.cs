using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LoadSaveSystem
{
    // default student player values
    public static string default_version = "2.0";
    // 1.6 added stickers to SIS

    public static string default_name = "new profile";
    public static int    default_stars = 0;
    public static int    default_map_limit = 0;

    public static int   default_micDevice = 0;

    public static bool  default_stickerTutorial = false;
    public static bool  default_froggerTutorial = false;
    public static bool  default_turntablesTutorial = false;
    public static bool  default_spiderwebTutorial = false;
    public static bool  default_rummageTutorial = false;

    public static StoryBeat default_gameEvent = StoryBeat.InitBoatGame;
    public static bool default_unlockedStickerButton = false;

    public static int   default_mapDataStars = 0;
    public static bool  default_mapDataFixed = false;

    public static int default_gold_coins = 0;



    public static void SaveStudentData(StudentPlayerData data, bool ignoreMakingActive = false)
    {
        if (!ignoreMakingActive)
        {
            // saving data to profile makes it active
            if (!data.active)
                data.active = true;
        }
        
        string jsonData = JsonUtility.ToJson(data);
        string path = GetStudentDataPath(data.studentIndex);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                Debug.Log("json data: " + jsonData);
                writer.Write(jsonData);
                writer.Close();
            }
        }
        // refresh database
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public static StudentPlayerData LoadStudentData(StudentIndex index, bool createNewIfNull)
    {
        string path = GetStudentDataPath(index);
        string file = "{}";

        if (System.IO.File.Exists(path))
        {
            // Read the json text from directly from the *.txt file
            using (StreamReader reader = new StreamReader(path))
            {
                file = reader.ReadToEnd();
                reader.Close();
            }
            
            StudentPlayerData studentData = JsonUtility.FromJson<StudentPlayerData>(file);
            if (studentData != null)
                return studentData;
            else
                Debug.Log("StudentPlayerData is null, returning null.");
        }
        else
        {
            // Debug.Log("Path not found: " + path.ToString() + "\nReturning null");
            // create new profile file
            if (createNewIfNull)
            {
                Debug.Log("profile not found, making new profile!");
                ResetStudentData(index);
                return LoadStudentData(index, false);
            }
        }

        return null;
    }

    public static void ResetStudentData(StudentIndex index)
    {
        StudentPlayerData new_data = new StudentPlayerData();
        new_data.studentIndex = index;

        // set all variables to be default values
        new_data.version =      default_version;
        new_data.name =         default_name;
        new_data.active = false;
        new_data.mostRecentProfile = false;
        new_data.minigamesPlayed = 0;

        // coins
        new_data.goldCoins = default_gold_coins;

        // settings
        new_data.masterVol =    AudioManager.default_masterVol;
        new_data.musicVol =     AudioManager.default_musicVol;
        new_data.fxVol =        AudioManager.default_fxVol;
        new_data.talkVol =      AudioManager.default_talkVol;
        new_data.micDevice =    default_micDevice;

        // tutorials
        new_data.stickerTutorial =      default_stickerTutorial;
        new_data.froggerTutorial =      default_froggerTutorial;
        new_data.turntablesTutorial =   default_turntablesTutorial;
        new_data.spiderwebTutorial =    default_spiderwebTutorial;
        new_data.rummageTutorial =      default_rummageTutorial;

        // pools
        new_data.actionWordPool = new List<ActionWordEnum>();
        new_data.challengeWordPool = new List<ChallengeWord>();

        // story + map data
        new_data.unlockedStickerButton = default_unlockedStickerButton;
        new_data.currStoryBeat = default_gameEvent;
        new_data.firstTimeLoseChallengeGame = false;
        new_data.everyOtherTimeLoseChallengeGame = false;
        
        new_data.mapLimit = 0;
        new_data.currentChapter = Chapter.chapter_0;
        new_data.mapData = new MapData();
        
        // gorilla village
        new_data.mapData.GV_house1 = new MapIconData();
        new_data.mapData.GV_house2 = new MapIconData();
        new_data.mapData.GV_fire = new MapIconData();
        new_data.mapData.GV_statue = new MapIconData();

        new_data.mapData.GV_house1.isFixed = true;
        new_data.mapData.GV_house2.isFixed = true;
        new_data.mapData.GV_fire.isFixed =   true;
        new_data.mapData.GV_statue.isFixed = true;

        new_data.mapData.GV_house1.stars =   default_mapDataStars;
        new_data.mapData.GV_house2.stars =   default_mapDataStars;
        new_data.mapData.GV_fire.stars =     default_mapDataStars;
        new_data.mapData.GV_statue.stars =   default_mapDataStars;

        new_data.mapData.GV_challenge1 = new ChallengeGameData();
        new_data.mapData.GV_challenge2 = new ChallengeGameData();
        new_data.mapData.GV_challenge3 = new ChallengeGameData();

        new_data.mapData.GV_challenge1.stars = default_stars;
        new_data.mapData.GV_challenge2.stars = default_stars;
        new_data.mapData.GV_challenge3.stars = default_stars;

        new_data.mapData.GV_challenge1.gameType = GameType.None;
        new_data.mapData.GV_challenge2.gameType = GameType.None;
        new_data.mapData.GV_challenge3.gameType = GameType.None;

        new_data.mapData.GV_signPost_unlocked = false;
        new_data.mapData.GV_signPost_stars = 0;


        // mudslide
        new_data.mapData.MS_logs = new MapIconData();
        new_data.mapData.MS_pond = new MapIconData();
        new_data.mapData.MS_ramp = new MapIconData();
        new_data.mapData.MS_tower = new MapIconData();

        new_data.mapData.MS_logs.isFixed = true;
        new_data.mapData.MS_pond.isFixed = true;
        new_data.mapData.MS_ramp.isFixed =   true;
        new_data.mapData.MS_tower.isFixed = true;

        new_data.mapData.MS_logs.stars =   default_mapDataStars;
        new_data.mapData.MS_pond.stars =   default_mapDataStars;
        new_data.mapData.MS_ramp.stars =     default_mapDataStars;
        new_data.mapData.MS_tower.stars =   default_mapDataStars;

        new_data.mapData.MS_challenge1 = new ChallengeGameData();
        new_data.mapData.MS_challenge2 = new ChallengeGameData();
        new_data.mapData.MS_challenge3 = new ChallengeGameData();

        new_data.mapData.MS_challenge1.stars = default_stars;
        new_data.mapData.MS_challenge2.stars = default_stars;
        new_data.mapData.MS_challenge3.stars = default_stars;

        new_data.mapData.MS_challenge1.gameType = GameType.None;
        new_data.mapData.MS_challenge2.gameType = GameType.None;
        new_data.mapData.MS_challenge3.gameType = GameType.None;

        new_data.mapData.MS_signPost_unlocked = false;
        new_data.mapData.MS_signPost_stars = 0;


        // orc village
        new_data.mapData.OV_houseL = new MapIconData();
        new_data.mapData.OV_houseS = new MapIconData();
        new_data.mapData.OV_statue = new MapIconData();
        new_data.mapData.OV_fire = new MapIconData();

        new_data.mapData.OV_houseL.isFixed = true;
        new_data.mapData.OV_houseS.isFixed = true;
        new_data.mapData.OV_statue.isFixed =   true;
        new_data.mapData.OV_fire.isFixed = true;

        new_data.mapData.OV_houseL.stars =   default_mapDataStars;
        new_data.mapData.OV_houseS.stars =   default_mapDataStars;
        new_data.mapData.OV_statue.stars =     default_mapDataStars;
        new_data.mapData.OV_fire.stars =   default_mapDataStars;

        new_data.mapData.OV_challenge1 = new ChallengeGameData();
        new_data.mapData.OV_challenge2 = new ChallengeGameData();
        new_data.mapData.OV_challenge3 = new ChallengeGameData();

        new_data.mapData.OV_challenge1.stars = default_stars;
        new_data.mapData.OV_challenge2.stars = default_stars;
        new_data.mapData.OV_challenge3.stars = default_stars;

        new_data.mapData.OV_challenge1.gameType = GameType.None;
        new_data.mapData.OV_challenge2.gameType = GameType.None;
        new_data.mapData.OV_challenge3.gameType = GameType.None;

        new_data.mapData.OV_signPost_unlocked = false;
        new_data.mapData.OV_signPost_stars = 0;


        // stickers
        new_data.stickerInventory = new List<InventoryStickerData>();
        // classic sticker board
        new_data.classicStickerBoard = new StickerBoardData();
        new_data.classicStickerBoard.boardType = StickerBoardType.Classic;
        new_data.classicStickerBoard.active = true; // always active by default
        new_data.classicStickerBoard.stickers = new List<StickerData>();

        // mossy sticker board
        new_data.mossyStickerBoard = new StickerBoardData();
        new_data.mossyStickerBoard.boardType = StickerBoardType.Mossy;
        new_data.mossyStickerBoard.active = false;
        new_data.mossyStickerBoard.stickers = new List<StickerData>();

        // emerald sticker board
        new_data.emeraldStickerBoard = new StickerBoardData();
        new_data.emeraldStickerBoard.boardType = StickerBoardType.Emerald;
        new_data.emeraldStickerBoard.active = false;
        new_data.emeraldStickerBoard.stickers = new List<StickerData>();

        // beach sticker board
        new_data.beachStickerBoard = new StickerBoardData();
        new_data.beachStickerBoard.boardType = StickerBoardType.Beach;
        new_data.beachStickerBoard.active = false;
        new_data.beachStickerBoard.stickers = new List<StickerData>();


        // save data as incative profile
        new_data.active = false;
        SaveStudentData(new_data, true);
        GameManager.instance.SendLog("LoadSaveSystem", "reseting profile " + index);
    }

    private static string GetStudentDataPath(StudentIndex index)
    {
        string path = "";
        
        if (Application.isEditor)
            path = "Assets/Resources/" + index.ToString() + "_data.json"; // example student_2_data.json
        else
            path = Application.persistentDataPath + "/" + index.ToString() + "_data.json";

        return path;
    }
}
