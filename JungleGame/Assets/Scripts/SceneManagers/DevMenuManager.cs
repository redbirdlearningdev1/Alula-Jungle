using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DevMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown navDropdown;
    [SerializeField] private TMP_Dropdown storyGameDropdown;
    [SerializeField] private TMP_Dropdown profileDropdown;
    [SerializeField] private TMP_Dropdown storyBeatDropdown;

    [SerializeField] private Toggle fastTalkieToggle;

    [SerializeField] private TMP_InputField profileInput;

    private string[] sceneNames = new string[] {  
        "ScrollMap",
        "SplashScene",
        "DevMenu", 

        "FroggerGame",
        "NewBoatGame",
        "SeaShellGame",
        "TurntablesGame",
        "RummageGame",
        "NewPirateGame",
        "NewSpiderGame",

        "WordFactoryBlending",
        "WordFactorySubstituting",
        "WordFactoryDeleting",
        "TigerPawCoins",
        "TigerPawPhotos",
        "NewPasswordGame"

        };

    private string[] storyGames = new string[] {
        "1 - The Prologue",
        "2 - The Beginning"
    };

    private List<StudentPlayerData> datas;                        

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // remove music
        AudioManager.instance.StopMusic();

        // add all build scenes to navDropdown
        List<string> sceneNamesList = new List<string>(sceneNames);
        navDropdown.AddOptions(sceneNamesList);

        // add story game options to dropdown
        List<string> storyGamesList = new List<string>(storyGames);
        storyGameDropdown.AddOptions(storyGamesList);

        // set story beat dropdown options
        string[] storyBeats = System.Enum.GetNames(typeof(StoryBeat));
        for(int i = 0; i < storyBeats.Length; i++)
        {
            storyBeats[i] = "" + i + " - " + storyBeats[i];
        }
        List<string> storyBeatsList = new List<string>(storyBeats);
        storyBeatDropdown.AddOptions(storyBeatsList);

        // set current story beat to be active
        storyBeatDropdown.value = (int)StudentInfoSystem.GetCurrentProfile().currStoryBeat;

        // setup profiles
        profileDropdown.onValueChanged.AddListener(delegate { OnProfileDropdownChanged(); });
        SetupProfileDropdown();

        // show UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        SettingsManager.instance.ToggleWagonButtonActive(true);
    }

    /* 
    ################################################
    #   UTILITY SECTION
    ################################################
    */

    public void OnFastTalkiesToggled()
    {
        TalkieManager.instance.SetFastTalkies(fastTalkieToggle.isOn);
    }

    /* 
    ################################################
    #   PROFILE SECTION
    ################################################
    */

    private void SetupProfileDropdown()
    {
        // refresh database
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        // set profile dropdown
        datas = StudentInfoSystem.GetAllStudentDatas();
        List<string> profileList = new List<string>();
        for (int i = 1; i < 4; i++)
        {
            string str = i + " - ";
            if (datas[i - 1].active)
                str += datas[i - 1].name;
            else
                str += "empty";
            profileList.Add(str);
        }
        profileDropdown.ClearOptions();
        profileDropdown.AddOptions(profileList);
        
        // set current profile

        switch (StudentInfoSystem.GetCurrentProfile().studentIndex)
        {
            case StudentIndex.student_1:
                profileDropdown.value = 0;
                break;
            case StudentIndex.student_2:
                profileDropdown.value = 1;
                break;
            case StudentIndex.student_3:
                profileDropdown.value = 2;
                break;
        }        
    }

    public void OnProfileDropdownChanged()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        int value = profileDropdown.value;
        switch (value)
        {
            case 0:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
                break;
            case 1:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_2);
                break;
            case 2:
                StudentInfoSystem.SetStudentPlayer(StudentIndex.student_3);
                break;
        }
        // update settings
        SettingsManager.instance.LoadSettingsFromProfile();
    }

    public void OnProfileResetPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(StudentInfoSystem.GetCurrentProfile().studentIndex);
        SettingsManager.instance.LoadSettingsFromProfile();
        SetupProfileDropdown();
    }

    public void OnAllProfilesResetPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(StudentIndex.student_1);
        StudentInfoSystem.ResetProfile(StudentIndex.student_2);
        StudentInfoSystem.ResetProfile(StudentIndex.student_3);
        SetupProfileDropdown();
        StudentInfoSystem.RemoveCurrentStudentPlayer();
    }

    public void OnRenameProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        StudentInfoSystem.GetCurrentProfile().name = profileInput.text;
        StudentInfoSystem.SaveStudentPlayerData();
        SetupProfileDropdown();
    }

    public void OnGiveGoldCoinsPressed()
    {
        // play audio blip
        AudioManager.instance.PlayKeyJingle();
        DropdownToolbar.instance.AwardGoldCoins(999);
    }

    public void OnUnlockStickerShopPressed()
    {
        // play audio blip
        AudioManager.instance.PlayKeyJingle();
        StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
    }

    public void OnUnlockEverythingButtonPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        var studentData = StudentInfoSystem.GetCurrentProfile();
        // unlock everything - (update this as game is developed)
        studentData.active = true;
        studentData.mapLimit = 4; // update this l8tr
        studentData.goldCoins = 999;

        studentData.stickerTutorial = true;
        studentData.froggerTutorial = true;
        studentData.turntablesTutorial = true;
        studentData.spiderwebTutorial = true;
        studentData.rummageTutorial = true;
        studentData.seashellTutorial = true;
        studentData.pirateTutorial = true;

        studentData.currStoryBeat = StoryBeat.COUNT;
        studentData.unlockedStickerButton = true;

        // map data
        // Gorilla Village
        studentData.mapData.GV_house1.isFixed = true;
        studentData.mapData.GV_house1.stars = 3;

        studentData.mapData.GV_house2.isFixed = true;
        studentData.mapData.GV_house2.stars = 3;

        studentData.mapData.GV_fire.isFixed = true;
        studentData.mapData.GV_fire.stars = 3;

        studentData.mapData.GV_statue.isFixed = true;
        studentData.mapData.GV_statue.stars = 3;

        studentData.mapData.GV_signPost_unlocked = true;
        studentData.mapData.GV_signPost_stars = 3;

        // Mudslide
        studentData.mapData.MS_logs.isFixed = true;
        studentData.mapData.MS_logs.stars = 3;

        studentData.mapData.MS_pond.isFixed = true;
        studentData.mapData.MS_pond.stars = 3;

        studentData.mapData.MS_ramp.isFixed = true;
        studentData.mapData.MS_ramp.stars = 3;

        studentData.mapData.MS_tower.isFixed = true;
        studentData.mapData.MS_tower.stars = 3;

        studentData.mapData.MS_signPost_unlocked = true;
        studentData.mapData.MS_signPost_stars = 3;

        // Orc Village
        studentData.mapData.OV_houseL.isFixed = true;
        studentData.mapData.OV_houseL.stars = 3;

        studentData.mapData.OV_houseS.isFixed = true;
        studentData.mapData.OV_houseS.stars = 3;

        studentData.mapData.OV_statue.isFixed = true;
        studentData.mapData.OV_statue.stars = 3;

        studentData.mapData.OV_fire.isFixed = true;
        studentData.mapData.OV_fire.stars = 3;

        studentData.mapData.OV_signPost_unlocked = true;
        studentData.mapData.OV_signPost_stars = 3;

        // Spooky Forest
        studentData.mapData.SF_lamp.isFixed = true;
        studentData.mapData.SF_lamp.stars = 3;

        studentData.mapData.SF_shrine.isFixed = true;
        studentData.mapData.SF_shrine.stars = 3;

        studentData.mapData.SF_spider.isFixed = true;
        studentData.mapData.SF_spider.stars = 3;

        studentData.mapData.SF_web.isFixed = true;
        studentData.mapData.SF_web.stars = 3;

        studentData.mapData.SF_signPost_unlocked = true;
        studentData.mapData.SF_signPost_stars = 3;

        // Orc Camp
        studentData.mapData.OC_axe.isFixed = true;
        studentData.mapData.OC_axe.stars = 3;

        studentData.mapData.OC_bigTent.isFixed = true;
        studentData.mapData.OC_bigTent.stars = 3;

        studentData.mapData.OC_smallTent.isFixed = true;
        studentData.mapData.OC_smallTent.stars = 3;

        studentData.mapData.OC_fire.isFixed = true;
        studentData.mapData.OC_fire.stars = 3;

        studentData.mapData.OC_signPost_unlocked = true;
        studentData.mapData.OC_signPost_stars = 3;

        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void OnFixAllUnlockedIconsPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        int currMapLimit = StudentInfoSystem.GetCurrentProfile().mapLimit;

        // gorilla village
        if (currMapLimit >= 2)
        {
            FixIconsUpTo(MapLocation.GorillaVillage);
        }

        // mudslide 
        if (currMapLimit >= 3)
        {
            FixIconsUpTo(MapLocation.Mudslide);
        }

        // orc village
        if (currMapLimit >= 4)
        {
            FixIconsUpTo(MapLocation.OrcVillage);
        }

        // orc village
        if (currMapLimit >= 5)
        {
            FixIconsUpTo(MapLocation.SpookyForest);
        }

        // orc village
        if (currMapLimit >= 6)
        {
            FixIconsUpTo(MapLocation.OrcCamp);
        }

        StudentInfoSystem.SaveStudentPlayerData();
    }

    /* 
    ################################################
    #   STORY BEAT SECTION
    ################################################
    */

    // TODO finish story beats
    public void OnSetStoryBeatPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        int beat = storyBeatDropdown.value;

        StudentInfoSystem.GetCurrentProfile().currStoryBeat = (StoryBeat)beat;

        // skip tutorials
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = true;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = true;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = true;
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = true;
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = true;
        // challenge game tutorials
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryBuildingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = true;
        
        switch ((StoryBeat)beat)
        {
            case StoryBeat.InitBoatGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 0;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;
            
            /* 
            ################################################
            #   GORILLA VILLAGE
            ################################################
            */

            case StoryBeat.UnlockGorillaVillage:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 1;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.GorillaVillageIntro:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.PrologueStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.RedShowsStickerButton:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().goldCoins = 4;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.VillageRebuilt:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.GorillaVillage_challengeGame_1:  
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.GorillaVillage_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetChallengeGame(MapLocation.GorillaVillage, 1);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.GorillaVillage_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetChallengeGame(MapLocation.GorillaVillage, 2);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.VillageChallengeDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;
            
            /* 
            ################################################
            #   MUDSLIDE
            ################################################
            */

            case StoryBeat.MudslideUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.Mudslide_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.Mudslide_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetChallengeGame(MapLocation.Mudslide, 1);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.Mudslide_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetChallengeGame(MapLocation.Mudslide, 2);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.MudslideDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            /* 
            ################################################
            #   ORC VILLAGE
            ################################################
            */

            case StoryBeat.OrcVillageUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcVillageMeetClogg:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcVillage_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcVillage_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetChallengeGame(MapLocation.OrcVillage, 1);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcVillage_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetChallengeGame(MapLocation.OrcVillage, 2);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcVillageDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            /* 
            ################################################
            #   SPOOKY FOREST
            ################################################
            */

            case StoryBeat.SpookyForestUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.OrcVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;
            
            case StoryBeat.BeginningStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.SpookyForestPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.SpookyForest_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.SpookyForest_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetChallengeGame(MapLocation.SpookyForest, 1);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.SpookyForest_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetChallengeGame(MapLocation.SpookyForest, 2);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.SpookyForestDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.SpookyForest);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            /* 
            ################################################
            #   ORC CAMP
            ################################################
            */

            case StoryBeat.OrcCampUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;
            
            case StoryBeat.OrcCampPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcCamp_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcCamp_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetChallengeGame(MapLocation.OrcCamp, 1);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcCamp_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetChallengeGame(MapLocation.OrcCamp, 2);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            case StoryBeat.OrcCampDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetActionWordPool(MapLocation.OrcCamp);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
                break;

            /* 
            ################################################
            #   ETC...
            ################################################
            */
        }

        StudentInfoSystem.SaveStudentPlayerData();
    }

    private void SetActionWordPool(MapLocation location)
    {
        StudentInfoSystem.GetCurrentProfile().actionWordPool.Clear();

        // chapter 1 action words
        if (location == MapLocation.GorillaVillage || location == MapLocation.Mudslide || location == MapLocation.OrcVillage ||
            location == MapLocation.SpookyForest)
        {   
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.listen);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.poop);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.orcs);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.think);
        }
        // chapter 2 action words
        if (location == MapLocation.SpookyForest || location == MapLocation.OrcCamp)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.hello);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.spider);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.explorer);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.scared);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.thatguy);
        }
    }

    // TODO: finish this when story beats complete
    private void FixIconsUpTo(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.NONE:
                SetMapIcons(MapLocation.GorillaVillage, false);
                SetMapIcons(MapLocation.Mudslide, false);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                break;

            case MapLocation.GorillaVillage:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, false);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                break;

            case MapLocation.Mudslide:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                break;
            
            case MapLocation.OrcVillage:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                break;

            case MapLocation.SpookyForest:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, false);
                break;

            case MapLocation.OrcCamp:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                break;
            
        }
    }

    private void SetMapIcons(MapLocation location, bool isFixed)
    {
        switch (location)
        {
            case MapLocation.GorillaVillage:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed = isFixed;

                break;

            case MapLocation.Mudslide:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.isFixed = isFixed;

                break;
            
            case MapLocation.OrcVillage:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.isFixed = isFixed;

                break;
            
            case MapLocation.SpookyForest:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed = isFixed;

                break;

            case MapLocation.OrcCamp:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.isFixed = isFixed;

                break;
        }
    }

    private void SetChallengeGamesUpTo(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.NONE:
                SetChallengeGame(MapLocation.GorillaVillage, 0);
                SetChallengeGame(MapLocation.Mudslide, 0);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                break;

            case MapLocation.GorillaVillage:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 0);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                break;

            case MapLocation.Mudslide:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                break;
            
            case MapLocation.OrcVillage:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                break;

            case MapLocation.SpookyForest:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                break;

            case MapLocation.OrcCamp:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                break;
        }
    }

    private void SetChallengeGame(MapLocation location, int num)
    {
        switch (location)
        {
            case MapLocation.GorillaVillage:
                StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaVillage);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_signPost_unlocked = true;
                }
                break;

            case MapLocation.Mudslide:
                StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.Mudslide);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
                }
                break;

            case MapLocation.OrcVillage:
                StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcVillage);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_signPost_unlocked = true;
                }
                break;

            case MapLocation.SpookyForest:
                StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.SpookyForest);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_signPost_unlocked = true;
                }
                break;

            case MapLocation.OrcCamp:
                StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.OrcCamp);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_signPost_unlocked = true;
                }
                break;
        }
    }

    /* 
    ################################################
    #   NAVIGATION SECTION
    ################################################
    */

    public void OnGoToSceneButtonPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        GameManager.instance.LoadScene(sceneNames[navDropdown.value], true, 0.5f, true);
    }

    public void OnPlayStoryGamePressed()
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        GameManager.instance.storyGameData = GameManager.instance.storyGameDatas[storyGameDropdown.value];
        GameManager.instance.LoadScene("StoryGame", true);
    }

    public void OnRestartGamePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);

        GameManager.instance.RestartGame();
    }

    public void OnExitAppPressed()
    {
        StartCoroutine(ExitDelayRoutine());
    }

    private IEnumerator ExitDelayRoutine()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        // fade out to black
        FadeObject.instance.FadeOut(3f);
        yield return new WaitForSeconds(3f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
