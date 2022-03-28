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
        "2 - The Beginning",
        "3 - Follow Red",
        "4 - Emerging",
        "5 - The Resolution"
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

    public void OnTalkieTestSceneButtonPresseds()
    {
        GameManager.instance.LoadScene("TalkieTestScene", true, 0.5f, true);
    }

    public void OnChallengeWordSceneButtonPresseds()
    {
        GameManager.instance.LoadScene("ChallengeWordTestScene", true, 0.5f, true);
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
        studentData.mapLimit = 16;
        studentData.goldCoins = 999;

        studentData.stickerTutorial = true;
        studentData.froggerTutorial = true;
        studentData.turntablesTutorial = true;
        studentData.spiderwebTutorial = true;
        studentData.rummageTutorial = true;
        studentData.seashellTutorial = true;
        studentData.pirateTutorial = true;

        studentData.wordFactoryBlendingTutorial = true;
        studentData.wordFactoryBuildingTutorial = true;
        studentData.wordFactoryDeletingTutorial = true;
        studentData.wordFactorySubstitutingTutorial = true;
        studentData.tigerPawCoinsTutorial = true;
        studentData.tigerPawPhotosTutorial = true;
        studentData.passwordTutorial = true;

        studentData.currStoryBeat = StoryBeat.COUNT;
        studentData.unlockedStickerButton = true;
        studentData.firstGuradsRoyalRumble = false;
        studentData.actionWordPool = new List<ActionWordEnum>();
        studentData.actionWordPool.AddRange(GameManager.instance.GetGlobalActionWordList());

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

        // Gorilla Poop
        studentData.mapData.GP_house1.isFixed = true;
        studentData.mapData.GP_house1.stars = 3;

        studentData.mapData.GP_house2.isFixed = true;
        studentData.mapData.GP_house2.stars = 3;

        studentData.mapData.GP_rock1.isFixed = true;
        studentData.mapData.GP_rock1.stars = 3;

        studentData.mapData.GP_rock2.isFixed = true;
        studentData.mapData.GP_rock2.stars = 3;

        studentData.mapData.GP_signPost_unlocked = true;
        studentData.mapData.GP_signPost_stars = 3;

        // Windy Cliff
        studentData.mapData.WC_ladder.isFixed = true;
        studentData.mapData.WC_ladder.stars = 3;

        studentData.mapData.WC_lighthouse.isFixed = true;
        studentData.mapData.WC_lighthouse.stars = 3;

        studentData.mapData.WC_octo.isFixed = true;
        studentData.mapData.WC_octo.stars = 3;

        studentData.mapData.WC_rock.isFixed = true;
        studentData.mapData.WC_rock.stars = 3;

        studentData.mapData.WC_sign.isFixed = true;
        studentData.mapData.WC_sign.stars = 3;

        studentData.mapData.WC_statue.isFixed = true;
        studentData.mapData.WC_statue.stars = 3;

        studentData.mapData.WC_signPost_unlocked = true;
        studentData.mapData.WC_signPost_stars = 3;

        // Pirate Ship
        studentData.mapData.PS_boat.isFixed = true;
        studentData.mapData.PS_boat.stars = 3;

        studentData.mapData.PS_bridge.isFixed = true;
        studentData.mapData.PS_bridge.stars = 3;

        studentData.mapData.PS_front.isFixed = true;
        studentData.mapData.PS_front.stars = 3;

        studentData.mapData.PS_parrot.isFixed = true;
        studentData.mapData.PS_parrot.stars = 3;

        studentData.mapData.PS_sail.isFixed = true;
        studentData.mapData.PS_sail.stars = 3;

        studentData.mapData.PS_wheel.isFixed = true;
        studentData.mapData.PS_wheel.stars = 3;

        studentData.mapData.PS_signPost_unlocked = true;
        studentData.mapData.PS_signPost_stars = 3;

        // Mermaid Beach
        studentData.mapData.MB_bucket.isFixed = true;
        studentData.mapData.MB_bucket.stars = 3;

        studentData.mapData.MB_castle.isFixed = true;
        studentData.mapData.MB_castle.stars = 3;

        studentData.mapData.MB_ladder.isFixed = true;
        studentData.mapData.MB_ladder.stars = 3;

        studentData.mapData.MB_mermaids.isFixed = true;
        studentData.mapData.MB_mermaids.stars = 3;

        studentData.mapData.MB_rock.isFixed = true;
        studentData.mapData.MB_rock.stars = 3;

        studentData.mapData.MB_umbrella.isFixed = true;
        studentData.mapData.MB_umbrella.stars = 3;

        studentData.mapData.MB_signPost_unlocked = true;
        studentData.mapData.MB_signPost_stars = 3;

        // Ruins
        studentData.mapData.R_arch.isFixed = true;
        studentData.mapData.R_arch.stars = 3;

        studentData.mapData.R_caveRock.isFixed = true;
        studentData.mapData.R_caveRock.stars = 3;

        studentData.mapData.R_face.isFixed = true;
        studentData.mapData.R_face.stars = 3;

        studentData.mapData.R_lizard1.isFixed = true;
        studentData.mapData.R_lizard1.stars = 3;

        studentData.mapData.R_lizard2.isFixed = true;
        studentData.mapData.R_lizard2.stars = 3;

        studentData.mapData.R_pyramid.isFixed = true;
        studentData.mapData.R_pyramid.stars = 3;

        studentData.mapData.R_signPost_unlocked = true;
        studentData.mapData.R_signPost_stars = 3;

        // Exit Jungle
        studentData.mapData.EJ_bridge.isFixed = true;
        studentData.mapData.EJ_bridge.stars = 3;

        studentData.mapData.EJ_puppy.isFixed = true;
        studentData.mapData.EJ_puppy.stars = 3;

        studentData.mapData.EJ_sign.isFixed = true;
        studentData.mapData.EJ_sign.stars = 3;

        studentData.mapData.EJ_torch.isFixed = true;
        studentData.mapData.EJ_torch.stars = 3;

        studentData.mapData.EJ_signPost_unlocked = true;
        studentData.mapData.EJ_signPost_stars = 3;

        // Gorilla Study
        studentData.mapData.GS_fire.isFixed = true;
        studentData.mapData.GS_fire.stars = 3;

        studentData.mapData.GS_statue.isFixed = true;
        studentData.mapData.GS_statue.stars = 3;

        studentData.mapData.GS_tent1.isFixed = true;
        studentData.mapData.GS_tent1.stars = 3;

        studentData.mapData.GS_tent2.isFixed = true;
        studentData.mapData.GS_tent2.stars = 3;

        studentData.mapData.GS_signPost_unlocked = true;
        studentData.mapData.GS_signPost_stars = 3;

        // Monkeys
        studentData.mapData.M_bananas.isFixed = true;
        studentData.mapData.M_bananas.stars = 3;

        studentData.mapData.M_flower.isFixed = true;
        studentData.mapData.M_flower.stars = 3;

        studentData.mapData.M_guards.isFixed = true;
        studentData.mapData.M_guards.stars = 3;

        studentData.mapData.M_tree.isFixed = true;
        studentData.mapData.M_tree.stars = 3;

        studentData.mapData.M_signPost_unlocked = true;
        studentData.mapData.M_signPost_stars = 3;

        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void OnFixAllUnlockedIconsPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        int currMapLimit = StudentInfoSystem.GetCurrentProfile().mapLimit;

        // gorilla village
        if (currMapLimit >= (int)MapLocation.GorillaVillage)
        {
            FixIconsUpTo(MapLocation.GorillaVillage);
        }

        // mudslide 
        if (currMapLimit >= (int)MapLocation.Mudslide)
        {
            FixIconsUpTo(MapLocation.Mudslide);
        }

        // orc village
        if (currMapLimit >= (int)MapLocation.OrcVillage)
        {
            FixIconsUpTo(MapLocation.OrcVillage);
        }

        // spooky forest
        if (currMapLimit >= (int)MapLocation.SpookyForest)
        {
            FixIconsUpTo(MapLocation.SpookyForest);
        }

        // orc camp
        if (currMapLimit >= (int)MapLocation.OrcCamp)
        {
            FixIconsUpTo(MapLocation.OrcCamp);
        }

        // gorilla poop
        if (currMapLimit >= (int)MapLocation.GorillaPoop)
        {
            FixIconsUpTo(MapLocation.GorillaPoop);
        }

        // windy cliff
        if (currMapLimit >= (int)MapLocation.WindyCliff)
        {
            FixIconsUpTo(MapLocation.WindyCliff);
        }

        // pirate ship
        if (currMapLimit >= (int)MapLocation.PirateShip)
        {
            FixIconsUpTo(MapLocation.PirateShip);
        }

        // mermaid beach
        if (currMapLimit >= (int)MapLocation.MermaidBeach)
        {
            FixIconsUpTo(MapLocation.MermaidBeach);
        }

        // ruins
        if (currMapLimit >= (int)MapLocation.Ruins1 || currMapLimit >= (int)MapLocation.Ruins2)
        {
            FixIconsUpTo(MapLocation.Ruins1);
        }

        // exit jungle
        if (currMapLimit >= (int)MapLocation.ExitJungle)
        {
            FixIconsUpTo(MapLocation.ExitJungle);
        }

        // gorilla study
        if (currMapLimit >= (int)MapLocation.GorillaStudy)
        {
            FixIconsUpTo(MapLocation.GorillaStudy);
        }

        // monkeys
        if (currMapLimit >= (int)MapLocation.Monkeys)
        {
            FixIconsUpTo(MapLocation.Monkeys);
        }

        StudentInfoSystem.SaveStudentPlayerData();
    }

    /* 
    ################################################
    #   TUTORIALS SECTION
    ################################################
    */

    public void DisableAllTutorials()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // skip tutorials
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = true;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = true;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = true;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = true;
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = true;
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = true;
        // skip challenge game tutorials
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryBuildingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = true;
        StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = true;
        StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = true;
        StudentInfoSystem.GetCurrentProfile().passwordTutorial = true;

        StudentInfoSystem.SaveStudentPlayerData();
    }

    public void ActivateAllTutorials()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // add tutorials
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = false;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = false;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = false;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = false;
        StudentInfoSystem.GetCurrentProfile().seashellTutorial = false;
        StudentInfoSystem.GetCurrentProfile().pirateTutorial = false;
        // add challenge game tutorials
        StudentInfoSystem.GetCurrentProfile().wordFactoryBlendingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactoryBuildingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactoryDeletingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().wordFactorySubstitutingTutorial = false;
        StudentInfoSystem.GetCurrentProfile().tigerPawCoinsTutorial = false;
        StudentInfoSystem.GetCurrentProfile().tigerPawPhotosTutorial = false;
        StudentInfoSystem.GetCurrentProfile().passwordTutorial = false;

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

        // set star amounts so AI system is happy :)
        StudentInfoSystem.GetCurrentProfile().starsFrogger = 1;
        StudentInfoSystem.GetCurrentProfile().starsSeashell = 1;
        StudentInfoSystem.GetCurrentProfile().starsRummage = 1;
        StudentInfoSystem.GetCurrentProfile().starsTurntables = 1;
        StudentInfoSystem.GetCurrentProfile().starsPirate = 1;
        StudentInfoSystem.GetCurrentProfile().starsSpiderweb = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsFrogger = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsSeashell = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsRummage = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsTurntables = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsPirate = 1;
        StudentInfoSystem.GetCurrentProfile().totalStarsSpiderweb = 1;

        // default stuff
        StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
        StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
        StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 6;
        StudentInfoSystem.GetCurrentProfile().goldCoins = 3;
        StudentInfoSystem.GetCurrentProfile().bossBattlePoints = 0;
        
        switch ((StoryBeat)beat)
        {
            case StoryBeat.InitBoatGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 0;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.Ocean);
                SetActionWordPool(MapLocation.Ocean);

                // remove stars from GV map icons
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 0;

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
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.BoatHouse);

                // remove stars from GV map icons
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 0;

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.GorillaVillageIntro:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.BoatHouse);

                // remove stars from GV map icons
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars = 0;

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.PrologueStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.BoatHouse);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.RedShowsStickerButton:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.BoatHouse);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                StudentInfoSystem.GetCurrentProfile().goldCoins = 4;
                StudentInfoSystem.GetCurrentProfile().minigamesPlayed = 0;
                break;

            case StoryBeat.VillageRebuilt:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.GorillaVillage);
                break;

            case StoryBeat.GorillaVillage_challengeGame_1:  
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetActionWordPool(MapLocation.GorillaVillage);
                break;

            case StoryBeat.GorillaVillage_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetChallengeGame(MapLocation.GorillaVillage, 1);
                SetActionWordPool(MapLocation.GorillaVillage);
                break;

            case StoryBeat.GorillaVillage_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.BoatHouse);
                SetChallengeGame(MapLocation.GorillaVillage, 2);
                SetActionWordPool(MapLocation.GorillaVillage);
                break;

            case StoryBeat.VillageChallengeDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.GorillaVillage);
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

                break;

            case StoryBeat.Mudslide_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.Mudslide);
                break;

            case StoryBeat.Mudslide_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetChallengeGame(MapLocation.Mudslide, 1);
                SetActionWordPool(MapLocation.Mudslide);
                break;

            case StoryBeat.Mudslide_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetChallengeGame(MapLocation.Mudslide, 2);
                SetActionWordPool(MapLocation.Mudslide);
                break;

            case StoryBeat.MudslideDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.Mudslide);
                break;

            /* 
            ################################################
            #   ORC VILLAGE
            ################################################
            */

            case StoryBeat.OrcVillageUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.OrcVillage);
                break;

            case StoryBeat.OrcVillageMeetClogg:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.OrcVillage);
                break;

            case StoryBeat.OrcVillage_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.OrcVillage);
                break;

            case StoryBeat.OrcVillage_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetChallengeGame(MapLocation.OrcVillage, 1);
                SetActionWordPool(MapLocation.OrcVillage);
                break;

            case StoryBeat.OrcVillage_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetChallengeGame(MapLocation.OrcVillage, 2);
                SetActionWordPool(MapLocation.OrcVillage);
                break;

            case StoryBeat.OrcVillageDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 4;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.OrcVillage);
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
                break;
            
            case StoryBeat.BeginningStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);
                break;

            case StoryBeat.SpookyForestPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.OrcVillage);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);
                break;

            case StoryBeat.SpookyForest_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetActionWordPool(MapLocation.SpookyForest);
                break;

            case StoryBeat.SpookyForest_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetChallengeGame(MapLocation.SpookyForest, 1);
                SetActionWordPool(MapLocation.SpookyForest);
                break;

            case StoryBeat.SpookyForest_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.OrcVillage);
                SetChallengeGame(MapLocation.SpookyForest, 2);
                SetActionWordPool(MapLocation.SpookyForest);
                break;

            case StoryBeat.SpookyForestDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 5;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.SpookyForest);
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
                break;
            
            case StoryBeat.OrcCampPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.SpookyForest);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.OrcCamp);
                break;

            case StoryBeat.OrcCamp_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetActionWordPool(MapLocation.OrcCamp);
                break;

            case StoryBeat.OrcCamp_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetChallengeGame(MapLocation.OrcCamp, 1);
                SetActionWordPool(MapLocation.OrcCamp);
                break;

            case StoryBeat.OrcCamp_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.SpookyForest);
                SetChallengeGame(MapLocation.OrcCamp, 2);
                SetActionWordPool(MapLocation.OrcCamp);
                break;

            case StoryBeat.OrcCampDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 6;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetActionWordPool(MapLocation.OrcCamp);
                break;

            /* 
            ################################################
            #   GORILLA POOP
            ###############################################
            */

            case StoryBeat.GorillaPoopPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaPoop;
                FixIconsUpTo(MapLocation.OrcCamp);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.GorillaPoop_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaPoop;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.GorillaPoop_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaPoop;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetChallengeGame(MapLocation.GorillaPoop, 1);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.GorillaPoop_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaPoop;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.OrcCamp);
                SetChallengeGame(MapLocation.GorillaPoop, 2);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.GorillaPoopDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaPoop;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            /* 
            ################################################
            #   WINDY CLIFF
            ###############################################
            */

            case StoryBeat.WindyCliffUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.FollowRedStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetActionWordPool(MapLocation.GorillaPoop);
                break;

            case StoryBeat.WindyCliffPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.GorillaPoop);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetActionWordPool(MapLocation.WindyCliff);
                break;

            case StoryBeat.WindyCliff_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetActionWordPool(MapLocation.WindyCliff);
                break;

            case StoryBeat.WindyCliff_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetChallengeGame(MapLocation.WindyCliff, 1);
                SetActionWordPool(MapLocation.WindyCliff);
                break;

            case StoryBeat.WindyCliff_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.GorillaPoop);
                SetChallengeGame(MapLocation.WindyCliff, 2);
                SetActionWordPool(MapLocation.WindyCliff);
                break;

            case StoryBeat.WindyCliffDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.WindyCliff;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.WindyCliff);
                SetActionWordPool(MapLocation.WindyCliff);
                break;

            /* 
            ################################################
            #   PIRATE SHIP
            ###############################################
            */

            case StoryBeat.PirateShipPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PirateShip;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.WindyCliff);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.PirateShip_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PirateShip;
                FixIconsUpTo(MapLocation.PirateShip);
                SetChallengeGamesUpTo(MapLocation.WindyCliff);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.PirateShip_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PirateShip;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.WindyCliff);
                SetChallengeGame(MapLocation.PirateShip, 1);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.PirateShip_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PirateShip;
                FixIconsUpTo(MapLocation.WindyCliff);
                SetChallengeGamesUpTo(MapLocation.WindyCliff);
                SetChallengeGame(MapLocation.PirateShip, 2);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.PirateShipDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PirateShip;
                FixIconsUpTo(MapLocation.PirateShip);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            /* 
            ################################################
            #   MERMAID BEACH
            ###############################################
            */

            case StoryBeat.MermaidBeachUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.PirateShip);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.EmergingStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.PirateShip);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetActionWordPool(MapLocation.PirateShip);
                break;

            case StoryBeat.MermaidBeachPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.PirateShip);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetActionWordPool(MapLocation.MermaidBeach);
                break;

            case StoryBeat.MermaidBeach_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetActionWordPool(MapLocation.MermaidBeach);
                break;

            case StoryBeat.MermaidBeach_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetChallengeGame(MapLocation.MermaidBeach, 1);
                SetActionWordPool(MapLocation.MermaidBeach);
                break;

            case StoryBeat.MermaidBeach_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.PirateShip);
                SetChallengeGame(MapLocation.MermaidBeach, 2);
                SetActionWordPool(MapLocation.MermaidBeach);
                break;

            case StoryBeat.MermaidBeachDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.MermaidBeach;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.MermaidBeach);
                SetActionWordPool(MapLocation.MermaidBeach);
                break;

            /* 
            ################################################
            #   RUINS
            ###############################################
            */

            case StoryBeat.RuinsPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Ruins2;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.MermaidBeach);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.Ruins_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Ruins2;
                FixIconsUpTo(MapLocation.Ruins2);
                SetChallengeGamesUpTo(MapLocation.MermaidBeach);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.Ruins_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Ruins2;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.MermaidBeach);
                SetChallengeGame(MapLocation.Ruins1, 1);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.Ruins_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Ruins2;
                FixIconsUpTo(MapLocation.MermaidBeach);
                SetChallengeGamesUpTo(MapLocation.MermaidBeach);
                SetChallengeGame(MapLocation.Ruins1, 2);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.RuinsDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Ruins2;
                FixIconsUpTo(MapLocation.Ruins2);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            /* 
            ################################################
            #   EXIT JUNGLE
            ###############################################
            */

            case StoryBeat.ExitJungleUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.Ruins2);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.ResolutionStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.Ruins2);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetActionWordPool(MapLocation.Ruins1);
                break;

            case StoryBeat.ExitJunglePlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.Ruins2);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetActionWordPool(MapLocation.ExitJungle);
                break;

            case StoryBeat.ExitJungle_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetActionWordPool(MapLocation.ExitJungle);
                break;

            case StoryBeat.ExitJungle_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetChallengeGame(MapLocation.ExitJungle, 1);
                SetActionWordPool(MapLocation.ExitJungle);
                break;

            case StoryBeat.ExitJungle_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.Ruins1);
                SetChallengeGame(MapLocation.ExitJungle, 2);
                SetActionWordPool(MapLocation.ExitJungle);
                break;

            case StoryBeat.ExitJungleDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.ExitJungle;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetActionWordPool(MapLocation.ExitJungle);
                break;

            /* 
            ################################################
            #   GORILLA STUDY
            ###############################################
            */

            case StoryBeat.GorillaStudyUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            case StoryBeat.GorillaStudyPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.ExitJungle);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            case StoryBeat.GorillaStudy_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.GorillaStudy);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            case StoryBeat.GorillaStudy_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.GorillaStudy);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetChallengeGame(MapLocation.GorillaStudy, 1);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            case StoryBeat.GorillaStudy_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.GorillaStudy);
                SetChallengeGamesUpTo(MapLocation.ExitJungle);
                SetChallengeGame(MapLocation.GorillaStudy, 2);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            case StoryBeat.GorillaStudyDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.GorillaStudy;
                FixIconsUpTo(MapLocation.GorillaStudy);
                SetChallengeGamesUpTo(MapLocation.GorillaStudy);
                SetActionWordPool(MapLocation.GorillaStudy);
                break;

            /* 
            ################################################
            #   MONKEYS
            ###############################################
            */

            case StoryBeat.MonkeysPlayGames:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Monkeys;
                FixIconsUpTo(MapLocation.GorillaStudy);
                SetChallengeGamesUpTo(MapLocation.GorillaStudy);
                SetActionWordPool(MapLocation.Monkeys);
                break;

            case StoryBeat.Monkeys_challengeGame_1:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Monkeys;
                FixIconsUpTo(MapLocation.Monkeys);
                SetChallengeGamesUpTo(MapLocation.GorillaStudy);
                SetActionWordPool(MapLocation.Monkeys);
                break;

            case StoryBeat.Monkeys_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Monkeys;
                FixIconsUpTo(MapLocation.Monkeys);
                SetChallengeGamesUpTo(MapLocation.GorillaStudy);
                SetChallengeGame(MapLocation.Monkeys, 1);
                SetActionWordPool(MapLocation.Monkeys);
                break;

            case StoryBeat.Monkeys_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Monkeys;
                FixIconsUpTo(MapLocation.Monkeys);
                SetChallengeGamesUpTo(MapLocation.GorillaStudy);
                SetChallengeGame(MapLocation.Monkeys, 2);
                SetActionWordPool(MapLocation.Monkeys);
                break;

            case StoryBeat.MonkeysDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.Monkeys;
                FixIconsUpTo(MapLocation.Monkeys);
                SetChallengeGamesUpTo(MapLocation.Monkeys);
                SetActionWordPool(MapLocation.Monkeys);
                break;

            /* 
            ################################################
            #   PALACE + BOSS BATTLE
            ###############################################
            */

            case StoryBeat.PalaceIntro:
            case StoryBeat.PreBossBattle:
            case StoryBeat.BossBattle1:
            case StoryBeat.BossBattle2:
            case StoryBeat.BossBattle3:
            case StoryBeat.EndBossBattle:
            case StoryBeat.FinishedGame:
            case StoryBeat.COUNT:
                StudentInfoSystem.GetCurrentProfile().mapLimit = (int)MapLocation.PalaceIntro;
                FixIconsUpTo(MapLocation.PalaceIntro);
                SetChallengeGamesUpTo(MapLocation.PalaceIntro);
                SetActionWordPool(MapLocation.PalaceIntro);
                break;
        }

        StudentInfoSystem.SaveStudentPlayerData();
    }

    private void SetActionWordPool(MapLocation location)
    {
        StudentInfoSystem.GetCurrentProfile().actionWordPool.Clear();

        // chapter 1 action words
        if (location <= MapLocation.SpookyForest)
        {   
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.listen);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.poop);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.orcs);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.think);
        }
        // chapter 2 action words
        if (location <= MapLocation.GorillaPoop)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.hello);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.spider);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.explorer);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.scared);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.thatguy);
        }
        // chapter 3 action words
        if (location <= MapLocation.PirateShip)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.choice);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.strongwind);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.pirate);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.gorilla);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.sounds);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.give);
        }
        // chapter 4 action words
        if (location <= MapLocation.Ruins2)
        {
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.backpack);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.frustrating);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.bumphead);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.baby);
        }
    }

    // TODO: finish this when story beats complete
    private void FixIconsUpTo(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_0;
                SetMapIcons(MapLocation.GorillaVillage, false);
                SetMapIcons(MapLocation.Mudslide, false);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.GorillaVillage:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, false);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.Mudslide:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, false);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;
            
            case MapLocation.OrcVillage:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, false);
                SetMapIcons(MapLocation.OrcCamp, false);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.SpookyForest:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, false);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.OrcCamp:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, false);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.GorillaPoop:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_2;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, false);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.WindyCliff:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_3;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, false);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;
            
            case MapLocation.PirateShip:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_3;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, false);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.MermaidBeach:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_4;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, false);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_4;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, true);
                SetMapIcons(MapLocation.ExitJungle, false);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.ExitJungle:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_5;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, true);
                SetMapIcons(MapLocation.ExitJungle, true);
                SetMapIcons(MapLocation.GorillaStudy, false);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.GorillaStudy:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_5;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, true);
                SetMapIcons(MapLocation.ExitJungle, true);
                SetMapIcons(MapLocation.GorillaStudy, true);
                SetMapIcons(MapLocation.Monkeys, false);
                break;

            case MapLocation.Monkeys:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_5;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, true);
                SetMapIcons(MapLocation.ExitJungle, true);
                SetMapIcons(MapLocation.GorillaStudy, true);
                SetMapIcons(MapLocation.Monkeys, true);
                break;

            case MapLocation.PalaceIntro:
                StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_final;
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                SetMapIcons(MapLocation.OrcVillage, true);
                SetMapIcons(MapLocation.SpookyForest, true);
                SetMapIcons(MapLocation.OrcCamp, true);
                SetMapIcons(MapLocation.GorillaPoop, true);
                SetMapIcons(MapLocation.WindyCliff, true);
                SetMapIcons(MapLocation.PirateShip, true);
                SetMapIcons(MapLocation.MermaidBeach, true);
                SetMapIcons(MapLocation.Ruins1, true);
                SetMapIcons(MapLocation.ExitJungle, true);
                SetMapIcons(MapLocation.GorillaStudy, true);
                SetMapIcons(MapLocation.Monkeys, true);
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

            case MapLocation.GorillaPoop:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.isFixed = isFixed;

                break;

            case MapLocation.WindyCliff:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.isFixed = isFixed;

                break;

            case MapLocation.PirateShip:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_front.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.isFixed = isFixed;

                break;

            case MapLocation.MermaidBeach:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.isFixed = isFixed;

                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.R_arch.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.R_face.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.isFixed = isFixed;

                break;

            case MapLocation.ExitJungle:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.isFixed = isFixed;

                break;

            case MapLocation.GorillaStudy:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.isFixed = isFixed;

                break;

            case MapLocation.Monkeys:
                if (!isFixed)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars = 0;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars = 3;
                }

                StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.M_flower.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.M_guards.isFixed = isFixed;
                StudentInfoSystem.GetCurrentProfile().mapData.M_tree.isFixed = isFixed;

                break;
        }
    }

    private void SetChallengeGamesUpTo(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                SetChallengeGame(MapLocation.GorillaVillage, 0);
                SetChallengeGame(MapLocation.Mudslide, 0);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.GorillaVillage:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 0);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.Mudslide:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 0);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;
            
            case MapLocation.OrcVillage:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 0);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.SpookyForest:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 0);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.OrcCamp:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 0);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.GorillaPoop:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 0);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.WindyCliff:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 0);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.PirateShip:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 0);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.MermaidBeach:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 3);
                SetChallengeGame(MapLocation.Ruins1, 0);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.Ruins1:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 3);
                SetChallengeGame(MapLocation.Ruins1, 3);
                SetChallengeGame(MapLocation.ExitJungle, 0);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.ExitJungle:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 3);
                SetChallengeGame(MapLocation.Ruins1, 3);
                SetChallengeGame(MapLocation.ExitJungle, 3);
                SetChallengeGame(MapLocation.GorillaStudy, 0);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.GorillaStudy:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 3);
                SetChallengeGame(MapLocation.Ruins1, 3);
                SetChallengeGame(MapLocation.ExitJungle, 3);
                SetChallengeGame(MapLocation.GorillaStudy, 3);
                SetChallengeGame(MapLocation.Monkeys, 0);
                break;

            case MapLocation.Monkeys:
            case MapLocation.PalaceIntro:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
                SetChallengeGame(MapLocation.OrcVillage, 3);
                SetChallengeGame(MapLocation.SpookyForest, 3);
                SetChallengeGame(MapLocation.OrcCamp, 3);
                SetChallengeGame(MapLocation.GorillaPoop, 3);
                SetChallengeGame(MapLocation.WindyCliff, 3);
                SetChallengeGame(MapLocation.PirateShip, 3);
                SetChallengeGame(MapLocation.MermaidBeach, 3);
                SetChallengeGame(MapLocation.Ruins1, 3);
                SetChallengeGame(MapLocation.ExitJungle, 3);
                SetChallengeGame(MapLocation.GorillaStudy, 3);
                SetChallengeGame(MapLocation.Monkeys, 3);
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

            case MapLocation.GorillaPoop:
                StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaPoop);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_signPost_unlocked = true;
                }
                break;

            case MapLocation.WindyCliff:
                StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.WindyCliff);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_signPost_unlocked = true;
                }
                break;

            case MapLocation.PirateShip:
                StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.PirateShip);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_signPost_unlocked = true;
                }
                break;

            case MapLocation.MermaidBeach:
                StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.MermaidBeach);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_signPost_unlocked = true;
                }
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                    StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.Ruins1);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.R_signPost_unlocked = true;
                }
                break;

            case MapLocation.ExitJungle:
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.ExitJungle);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_signPost_unlocked = true;
                }
                break;

            case MapLocation.GorillaStudy:
                StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.GorillaStudy);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_signPost_unlocked = true;
                }
                break;

            case MapLocation.Monkeys:
                StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_stars = 0;
                StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_unlocked = false;

                if (num == 0)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType = GameType.None;
                }
                else if (num == 1)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars = 0;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                    StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType = AISystem.DetermineChallengeGame(MapLocation.Monkeys);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.M_signPost_unlocked = true;
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

    /* 
    ################################################
    #   STICKER SIMULATIONS
    ################################################
    */

    public void Roll100Stickers()
    {
        RollXAmountOfStickers(100);
    }

    public void Roll1000Stickers()
    {
        RollXAmountOfStickers(1000);
    }

    private void RollXAmountOfStickers(int x)
    {
        // keep track of prev active profile
        StudentIndex prevIndex = StudentInfoSystem.GetCurrentProfile().studentIndex;
        // make new profile + set current profile to be simulation specific
        StudentInfoSystem.ResetStickerSimulationProfile();
        StudentInfoSystem.SetStudentPlayer(StudentIndex.sticker_simulation_profile);

        // roll for sticker x amount of times
        int commonCount = 0;
        int uncommonCount = 0;
        int rareCount = 0;
        int legendaryCount = 0;
        List<int> rareOccurrences = new List<int>();
        List<int> legendaryOccurrences = new List<int>();
        for (int i = 0; i < x; i++)
        {
            Sticker new_sticker = StickerDatabase.instance.RollForSticker();
            StudentInfoSystem.AddStickerToInventory(new_sticker);

            // keep track of rarity stats
            switch (new_sticker.rarity)
            {
                case StickerRarity.Common: commonCount++; break;
                case StickerRarity.Uncommon: uncommonCount++; break;
                case StickerRarity.Rare: 
                    rareCount++;
                    rareOccurrences.Add(i);
                    break;
                case StickerRarity.Legendary: 
                    legendaryCount++;
                    legendaryOccurrences.Add(i);
                    break;
            }
        }
        StudentPlayerData simulatedProfile = StudentInfoSystem.GetCurrentProfile();

        // print out sticker stats
        GameManager.instance.SendLog(this, "PRINTING OUT STICKER STATS FOR " + x + " SIMULATED ROLLS:");
        GameManager.instance.SendLog(this, "unique stickers in inventory: " + simulatedProfile.stickerInventory.Count);
        int totalStickers = 0;
        foreach (var sticker in simulatedProfile.stickerInventory)
        {
            totalStickers += sticker.count;
        }
        GameManager.instance.SendLog(this, "total stickers in inventory: " + totalStickers);

        GameManager.instance.SendLog(this, "\t common stickers:\t" + ((float)commonCount / (float)x) * 100f + "%");
        GameManager.instance.SendLog(this, "\t uncommon stickers:\t" + ((float)uncommonCount / (float)x) * 100f + "%");
        GameManager.instance.SendLog(this, "\t rare stickers:\t\t" + ((float)rareCount / (float)x) * 100f + "%");
        GameManager.instance.SendLog(this, "\t legendary stickers:\t" + ((float)legendaryCount / (float)x) * 100f + "%");

        GameManager.instance.SendLog(this, "\t common stickers unlocked:\t " + CountTrues(simulatedProfile.commonStickerUnlocked) + "/60"); // 60 common stickers
        GameManager.instance.SendLog(this, "\t uncommon stickers unlocked:\t " + CountTrues(simulatedProfile.uncommonStickerUnlocked) + "/36"); // 36 uncommon stickers
        GameManager.instance.SendLog(this, "\t rare stickers unlocked:\t\t " + CountTrues(simulatedProfile.rareStickerUnlocked) + "/12"); // 12 rare stickers
        GameManager.instance.SendLog(this, "\t legendary stickers unlocked:\t " + CountTrues(simulatedProfile.legendaryStickerUnlocked) + "/12"); // 12 legendary stickers

        string rareOccurrencesString = "[";
        foreach (int round in rareOccurrences)
        {
            rareOccurrencesString += round.ToString() + ", ";
        }
        rareOccurrencesString = rareOccurrencesString.Substring(0, rareOccurrencesString.Length - 2);
        rareOccurrencesString += "]";
        GameManager.instance.SendLog(this, "\t rare occurrences:\t" + rareOccurrencesString);

        string legendaryOccurrencesString = "[";
        foreach (int round in legendaryOccurrences)
        {
            legendaryOccurrencesString += round.ToString() + ", ";
        }
        legendaryOccurrencesString = legendaryOccurrencesString.Substring(0, legendaryOccurrencesString.Length - 2);
        legendaryOccurrencesString += "]";
        GameManager.instance.SendLog(this, "\t legendary occurrences:\t" + legendaryOccurrencesString);


        // set current profile to be prev active profile
        StudentInfoSystem.SetStudentPlayer(prevIndex);
    }

    public static int CountTrues(bool[] array)
    {
        int count = 0;
        foreach (bool b in array)
            if (b) count++;
        return count;
    }
}
