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
    public static string default_version = "1.7"; 
    // 1.6 added stickers to SIS

    public static string default_name = "empty";
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



    public static void SaveStudentData(StudentPlayerData data, bool makeInactive = false)
    {
        // saving data to profile makes it active
        if (!data.active)
            data.active = true;
        if (makeInactive)
            data.active = false;

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

    public static StudentPlayerData LoadStudentData(StudentIndex index)
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
            ResetStudentData(index);
            return LoadStudentData(index);
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
        new_data.totalStars =   default_stars;

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

        // story + map data
        new_data.unlockedStickerButton = default_unlockedStickerButton;
        new_data.currStoryBeat = default_gameEvent;
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

        // stickers
        new_data.unlockedStickers = new List<Sticker>();

        // save data as incative profile
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
