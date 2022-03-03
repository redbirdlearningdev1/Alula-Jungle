using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class LoadSaveSystem
{
    public static string default_name = "new profile";
    public static int    default_stars = 0;
    public static int    default_map_limit = 0;

    public static int   default_micDevice = 0;

    public static bool  default_tutorial = false;

    public static StoryBeat default_gameEvent = StoryBeat.InitBoatGame;
    public static bool default_unlockedStickerButton = false;

    public static int   default_mapDataStars = 0;
    public static bool  default_mapDataFixed = false;

    public static int default_gold_coins = 0;

    public static void SaveStudentData(StudentPlayerData data)
    {        
        string jsonData = JsonUtility.ToJson(data);
        string path = GetStudentDataPath(data.studentIndex);

        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                //Debug.Log("json data: " + jsonData);
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

            // check to make sure data is correct version
            if (studentData.version != GameManager.currentGameVersion)
            {
                GameManager.instance.SendError("LoadSaveSystem", "Student data is wrong version - deleting old profile");

                // create new profile file
                if (createNewIfNull)
                {
                    GameManager.instance.SendLog("LoadSaveSystem", "profile not found, making a new profile!");
                    ResetStudentData(index);
                    return LoadStudentData(index, false);
                }
                else
                {
                    return null;
                }
            }

            if (studentData != null)
                return studentData;
            else
                GameManager.instance.SendLog("LoadSaveSystem", "StudentPlayerData is null, returning null.");
        }
        else
        {
            // Debug.Log("Path not found: " + path.ToString() + "\nReturning null");
            // create new profile file
            if (createNewIfNull)
            {
                GameManager.instance.SendLog("LoadSaveSystem", "profile not found, making a new profile!");
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
        new_data.version =      GameManager.currentGameVersion;
        new_data.name =         default_name;
        new_data.active = false;
        new_data.mostRecentProfile = false;
        new_data.minigamesPlayed = 0;

        new_data.lastGamePlayed = GameType.None;
        new_data.gameBeforeLastPlayed = GameType.None;

        new_data.starsLastGamePlayed = 0;
        new_data.starsGameBeforeLastPlayed = 0;

        new_data.starsFrogger = 0;
        new_data.starsSeashell = 0;
        new_data.starsRummage = 0;
        new_data.starsTurntables = 0;
        new_data.starsPirate = 0;
        new_data.starsSpiderweb = 0;

        new_data.totalStarsFrogger = 0;
        new_data.totalStarsSeashell = 0;
        new_data.totalStarsRummage = 0;
        new_data.totalStarsTurntables = 0;
        new_data.totalStarsPirate = 0;
        new_data.totalStarsSpiderweb = 0;
        
        new_data.profileAvatar = 11;

        // coins
        new_data.goldCoins = default_gold_coins;

        // settings
        new_data.masterVol =    AudioManager.default_masterVol;
        new_data.musicVol =     AudioManager.default_musicVol;
        new_data.fxVol =        AudioManager.default_fxVol;
        new_data.talkVol =      AudioManager.default_talkVol;
        new_data.micDevice =    default_micDevice;

        // talkie options
        new_data.talkieSubtitles = true;
        new_data.talkieFast = false;
        new_data.talkieParticles = true;

        // tutorials
        new_data.stickerTutorial =      default_tutorial;
        new_data.froggerTutorial =      default_tutorial;
        new_data.turntablesTutorial =   default_tutorial;
        new_data.spiderwebTutorial =    default_tutorial;
        new_data.rummageTutorial =      default_tutorial;
        new_data.pirateTutorial =       default_tutorial;
        new_data.seashellTutorial =     default_tutorial;

        new_data.wordFactoryBlendingTutorial =      default_tutorial;
        new_data.wordFactoryBuildingTutorial =      default_tutorial;
        new_data.wordFactoryDeletingTutorial =      default_tutorial;
        new_data.wordFactorySubstitutingTutorial =  default_tutorial;
        new_data.tigerPawCoinsTutorial =    default_tutorial;
        new_data.tigerPawPhotosTutorial =   default_tutorial;
        new_data.passwordTutorial =         default_tutorial;

        // pools
        new_data.actionWordPool = new List<ActionWordEnum>();
        
        // royal rumble data
        new_data.royalRumbleActive = false;
        new_data.royalRumbleID = MapIconIdentfier.None;
        new_data.royalRumbleGame = GameType.None;

        // story + map data
        new_data.unlockedStickerButton = default_unlockedStickerButton;
        new_data.currStoryBeat = default_gameEvent;
        new_data.currBoatEncounter = BoatEncounter.FirstTime;
        new_data.firstTimeLoseChallengeGame = false;
        new_data.everyOtherTimeLoseChallengeGame = false;
        new_data.firstGuradsRoyalRumble = true;
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

        // spooky forest
        new_data.mapData.SF_lamp = new MapIconData();
        new_data.mapData.SF_shrine = new MapIconData();
        new_data.mapData.SF_spider = new MapIconData();
        new_data.mapData.SF_web = new MapIconData();

        new_data.mapData.SF_lamp.isFixed = true;
        new_data.mapData.SF_shrine.isFixed = true;
        new_data.mapData.SF_spider.isFixed =   true;
        new_data.mapData.SF_web.isFixed = true;

        new_data.mapData.SF_lamp.stars =   default_mapDataStars;
        new_data.mapData.SF_shrine.stars =   default_mapDataStars;
        new_data.mapData.SF_spider.stars =     default_mapDataStars;
        new_data.mapData.SF_web.stars =   default_mapDataStars;

        new_data.mapData.SF_challenge1 = new ChallengeGameData();
        new_data.mapData.SF_challenge2 = new ChallengeGameData();
        new_data.mapData.SF_challenge3 = new ChallengeGameData();

        new_data.mapData.SF_challenge1.stars = default_stars;
        new_data.mapData.SF_challenge2.stars = default_stars;
        new_data.mapData.SF_challenge3.stars = default_stars;

        new_data.mapData.SF_challenge1.gameType = GameType.None;
        new_data.mapData.SF_challenge2.gameType = GameType.None;
        new_data.mapData.SF_challenge3.gameType = GameType.None;

        new_data.mapData.SF_signPost_unlocked = false;
        new_data.mapData.SF_signPost_stars = 0;


        // gorilla poop
        new_data.mapData.GP_house1 = new MapIconData();
        new_data.mapData.GP_house2 = new MapIconData();
        new_data.mapData.GP_rock1 = new MapIconData();
        new_data.mapData.GP_rock2 = new MapIconData();

        new_data.mapData.GP_house1.isFixed = true;
        new_data.mapData.GP_house2.isFixed = true;
        new_data.mapData.GP_rock1.isFixed =   true;
        new_data.mapData.GP_rock2.isFixed = true;

        new_data.mapData.GP_house1.stars =   default_mapDataStars;
        new_data.mapData.GP_house2.stars =   default_mapDataStars;
        new_data.mapData.GP_rock1.stars =     default_mapDataStars;
        new_data.mapData.GP_rock2.stars =   default_mapDataStars;

        new_data.mapData.GP_challenge1 = new ChallengeGameData();
        new_data.mapData.GP_challenge2 = new ChallengeGameData();
        new_data.mapData.GP_challenge3 = new ChallengeGameData();

        new_data.mapData.GP_challenge1.stars = default_stars;
        new_data.mapData.GP_challenge2.stars = default_stars;
        new_data.mapData.GP_challenge3.stars = default_stars;

        new_data.mapData.GP_challenge1.gameType = GameType.None;
        new_data.mapData.GP_challenge2.gameType = GameType.None;
        new_data.mapData.GP_challenge3.gameType = GameType.None;

        new_data.mapData.GP_signPost_unlocked = false;
        new_data.mapData.GP_signPost_stars = 0;


        // windy cliff
        new_data.mapData.WC_ladder = new MapIconData();
        new_data.mapData.WC_lighthouse = new MapIconData();
        new_data.mapData.WC_octo = new MapIconData();
        new_data.mapData.WC_rock = new MapIconData();
        new_data.mapData.WC_sign = new MapIconData();
        new_data.mapData.WC_statue = new MapIconData();

        new_data.mapData.WC_ladder.isFixed = true;
        new_data.mapData.WC_lighthouse.isFixed = true;
        new_data.mapData.WC_octo.isFixed =   true;
        new_data.mapData.WC_rock.isFixed = true;
        new_data.mapData.WC_sign.isFixed = true;
        new_data.mapData.WC_statue.isFixed = true;

        new_data.mapData.WC_ladder.stars =   default_mapDataStars;
        new_data.mapData.WC_lighthouse.stars =   default_mapDataStars;
        new_data.mapData.WC_octo.stars =     default_mapDataStars;
        new_data.mapData.WC_rock.stars =   default_mapDataStars;
        new_data.mapData.WC_sign.stars =   default_mapDataStars;
        new_data.mapData.WC_statue.stars =   default_mapDataStars;

        new_data.mapData.WC_challenge1 = new ChallengeGameData();
        new_data.mapData.WC_challenge2 = new ChallengeGameData();
        new_data.mapData.WC_challenge3 = new ChallengeGameData();

        new_data.mapData.WC_challenge1.stars = default_stars;
        new_data.mapData.WC_challenge2.stars = default_stars;
        new_data.mapData.WC_challenge3.stars = default_stars;

        new_data.mapData.WC_challenge1.gameType = GameType.None;
        new_data.mapData.WC_challenge2.gameType = GameType.None;
        new_data.mapData.WC_challenge3.gameType = GameType.None;

        new_data.mapData.WC_signPost_unlocked = false;
        new_data.mapData.WC_signPost_stars = 0;


        // pirate ship
        new_data.mapData.PS_boat = new MapIconData();
        new_data.mapData.PS_bridge = new MapIconData();
        new_data.mapData.PS_front = new MapIconData();
        new_data.mapData.PS_parrot = new MapIconData();
        new_data.mapData.PS_sail = new MapIconData();
        new_data.mapData.PS_wheel = new MapIconData();

        new_data.mapData.PS_boat.isFixed = true;
        new_data.mapData.PS_bridge.isFixed = true;
        new_data.mapData.PS_front.isFixed =   true;
        new_data.mapData.PS_parrot.isFixed = true;
        new_data.mapData.PS_sail.isFixed = true;
        new_data.mapData.PS_wheel.isFixed = true;

        new_data.mapData.PS_boat.stars =   default_mapDataStars;
        new_data.mapData.PS_bridge.stars =   default_mapDataStars;
        new_data.mapData.PS_front.stars =     default_mapDataStars;
        new_data.mapData.PS_parrot.stars =   default_mapDataStars;
        new_data.mapData.PS_sail.stars =   default_mapDataStars;
        new_data.mapData.PS_wheel.stars =   default_mapDataStars;

        new_data.mapData.PS_challenge1 = new ChallengeGameData();
        new_data.mapData.PS_challenge2 = new ChallengeGameData();
        new_data.mapData.PS_challenge3 = new ChallengeGameData();

        new_data.mapData.PS_challenge1.stars = default_stars;
        new_data.mapData.PS_challenge2.stars = default_stars;
        new_data.mapData.PS_challenge3.stars = default_stars;

        new_data.mapData.PS_challenge1.gameType = GameType.None;
        new_data.mapData.PS_challenge2.gameType = GameType.None;
        new_data.mapData.PS_challenge3.gameType = GameType.None;

        new_data.mapData.PS_signPost_unlocked = false;
        new_data.mapData.PS_signPost_stars = 0;


        // mermaid beach
        new_data.mapData.MB_bucket = new MapIconData();
        new_data.mapData.MB_castle = new MapIconData();
        new_data.mapData.MB_ladder = new MapIconData();
        new_data.mapData.MB_mermaids = new MapIconData();
        new_data.mapData.MB_rock = new MapIconData();
        new_data.mapData.MB_umbrella = new MapIconData();

        new_data.mapData.MB_bucket.isFixed = true;
        new_data.mapData.MB_castle.isFixed = true;
        new_data.mapData.MB_ladder.isFixed =   true;
        new_data.mapData.MB_mermaids.isFixed = true;
        new_data.mapData.MB_rock.isFixed = true;
        new_data.mapData.MB_umbrella.isFixed = true;

        new_data.mapData.MB_bucket.stars =   default_mapDataStars;
        new_data.mapData.MB_castle.stars =   default_mapDataStars;
        new_data.mapData.MB_ladder.stars =     default_mapDataStars;
        new_data.mapData.MB_mermaids.stars =   default_mapDataStars;
        new_data.mapData.MB_rock.stars =   default_mapDataStars;
        new_data.mapData.MB_umbrella.stars =   default_mapDataStars;

        new_data.mapData.MB_challenge1 = new ChallengeGameData();
        new_data.mapData.MB_challenge2 = new ChallengeGameData();
        new_data.mapData.MB_challenge3 = new ChallengeGameData();

        new_data.mapData.MB_challenge1.stars = default_stars;
        new_data.mapData.MB_challenge2.stars = default_stars;
        new_data.mapData.MB_challenge3.stars = default_stars;

        new_data.mapData.MB_challenge1.gameType = GameType.None;
        new_data.mapData.MB_challenge2.gameType = GameType.None;
        new_data.mapData.MB_challenge3.gameType = GameType.None;

        new_data.mapData.MB_signPost_unlocked = false;
        new_data.mapData.MB_signPost_stars = 0;


        // ruins 1 + 2
        new_data.mapData.R_arch = new MapIconData();
        new_data.mapData.R_caveRock = new MapIconData();
        new_data.mapData.R_face = new MapIconData();
        new_data.mapData.R_lizard1 = new MapIconData();
        new_data.mapData.R_lizard2 = new MapIconData();
        new_data.mapData.R_pyramid = new MapIconData();

        new_data.mapData.R_arch.isFixed = true;
        new_data.mapData.R_caveRock.isFixed = true;
        new_data.mapData.R_face.isFixed =   true;
        new_data.mapData.R_lizard1.isFixed = true;
        new_data.mapData.R_lizard2.isFixed = true;
        new_data.mapData.R_pyramid.isFixed = true;

        new_data.mapData.R_arch.stars =   default_mapDataStars;
        new_data.mapData.R_caveRock.stars =   default_mapDataStars;
        new_data.mapData.R_face.stars =     default_mapDataStars;
        new_data.mapData.R_lizard1.stars =   default_mapDataStars;
        new_data.mapData.R_lizard2.stars =   default_mapDataStars;
        new_data.mapData.R_pyramid.stars =   default_mapDataStars;

        new_data.mapData.R_challenge1 = new ChallengeGameData();
        new_data.mapData.R_challenge2 = new ChallengeGameData();
        new_data.mapData.R_challenge3 = new ChallengeGameData();

        new_data.mapData.R_challenge1.stars = default_stars;
        new_data.mapData.R_challenge2.stars = default_stars;
        new_data.mapData.R_challenge3.stars = default_stars;

        new_data.mapData.R_challenge1.gameType = GameType.None;
        new_data.mapData.R_challenge2.gameType = GameType.None;
        new_data.mapData.R_challenge3.gameType = GameType.None;

        new_data.mapData.R_signPost_unlocked = false;
        new_data.mapData.R_signPost_stars = 0;


        // exit jungle
        new_data.mapData.EJ_bridge = new MapIconData();
        new_data.mapData.EJ_puppy = new MapIconData();
        new_data.mapData.EJ_sign = new MapIconData();
        new_data.mapData.EJ_torch = new MapIconData();

        new_data.mapData.EJ_bridge.isFixed = true;
        new_data.mapData.EJ_puppy.isFixed = true;
        new_data.mapData.EJ_sign.isFixed =   true;
        new_data.mapData.EJ_torch.isFixed = true;

        new_data.mapData.EJ_bridge.stars =   default_mapDataStars;
        new_data.mapData.EJ_puppy.stars =   default_mapDataStars;
        new_data.mapData.EJ_sign.stars =     default_mapDataStars;
        new_data.mapData.EJ_torch.stars =   default_mapDataStars;

        new_data.mapData.EJ_challenge1 = new ChallengeGameData();
        new_data.mapData.EJ_challenge2 = new ChallengeGameData();
        new_data.mapData.EJ_challenge3 = new ChallengeGameData();

        new_data.mapData.EJ_challenge1.stars = default_stars;
        new_data.mapData.EJ_challenge2.stars = default_stars;
        new_data.mapData.EJ_challenge3.stars = default_stars;

        new_data.mapData.EJ_challenge1.gameType = GameType.None;
        new_data.mapData.EJ_challenge2.gameType = GameType.None;
        new_data.mapData.EJ_challenge3.gameType = GameType.None;

        new_data.mapData.EJ_signPost_unlocked = false;
        new_data.mapData.EJ_signPost_stars = 0;


        // exit jungle
        new_data.mapData.GS_fire = new MapIconData();
        new_data.mapData.GS_statue = new MapIconData();
        new_data.mapData.GS_tent1 = new MapIconData();
        new_data.mapData.GS_tent2 = new MapIconData();

        new_data.mapData.GS_fire.isFixed = true;
        new_data.mapData.GS_statue.isFixed = true;
        new_data.mapData.GS_tent1.isFixed =   true;
        new_data.mapData.GS_tent2.isFixed = true;

        new_data.mapData.GS_fire.stars =   default_mapDataStars;
        new_data.mapData.GS_statue.stars =   default_mapDataStars;
        new_data.mapData.GS_tent1.stars =     default_mapDataStars;
        new_data.mapData.GS_tent2.stars =   default_mapDataStars;

        new_data.mapData.GS_challenge1 = new ChallengeGameData();
        new_data.mapData.GS_challenge2 = new ChallengeGameData();
        new_data.mapData.GS_challenge3 = new ChallengeGameData();

        new_data.mapData.GS_challenge1.stars = default_stars;
        new_data.mapData.GS_challenge2.stars = default_stars;
        new_data.mapData.GS_challenge3.stars = default_stars;

        new_data.mapData.GS_challenge1.gameType = GameType.None;
        new_data.mapData.GS_challenge2.gameType = GameType.None;
        new_data.mapData.GS_challenge3.gameType = GameType.None;

        new_data.mapData.GS_signPost_unlocked = false;
        new_data.mapData.GS_signPost_stars = 0;


        // monkeys
        new_data.mapData.M_bananas = new MapIconData();
        new_data.mapData.M_flower = new MapIconData();
        new_data.mapData.M_guards = new MapIconData();
        new_data.mapData.M_tree = new MapIconData();

        new_data.mapData.M_bananas.isFixed = true;
        new_data.mapData.M_flower.isFixed = true;
        new_data.mapData.M_guards.isFixed =   true;
        new_data.mapData.M_tree.isFixed = true;

        new_data.mapData.M_bananas.stars =   default_mapDataStars;
        new_data.mapData.M_flower.stars =   default_mapDataStars;
        new_data.mapData.M_guards.stars =     default_mapDataStars;
        new_data.mapData.M_tree.stars =   default_mapDataStars;

        new_data.mapData.M_challenge1 = new ChallengeGameData();
        new_data.mapData.M_challenge2 = new ChallengeGameData();
        new_data.mapData.M_challenge3 = new ChallengeGameData();

        new_data.mapData.M_challenge1.stars = default_stars;
        new_data.mapData.M_challenge2.stars = default_stars;
        new_data.mapData.M_challenge3.stars = default_stars;

        new_data.mapData.M_challenge1.gameType = GameType.None;
        new_data.mapData.M_challenge2.gameType = GameType.None;
        new_data.mapData.M_challenge3.gameType = GameType.None;

        new_data.mapData.M_signPost_unlocked = false;
        new_data.mapData.M_signPost_stars = 0;
        

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


        // save data as inactive profile
        new_data.active = false;
        SaveStudentData(new_data);
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
