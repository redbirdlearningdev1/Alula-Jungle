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
        "Password"

        };

    private string[] storyGames = new string[] {
        "0 - Welcome",
        "1 - The Prologue"
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
        
        switch ((StoryBeat)beat)
        {
            case StoryBeat.InitBoatGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 0;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                break;

            case StoryBeat.UnlockGorillaVillage:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 1;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                break;

            case StoryBeat.GorillaVillageIntro:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.NONE);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                break;

            case StoryBeat.PrologueStoryGame:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                break;

            case StoryBeat.RedShowsStickerButton:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.NONE);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = false;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = false;
                break;

            case StoryBeat.VillageRebuilt:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.GorillaVillage_challengeGame_1:  
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.GorillaVillage_challengeGame_2:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetChallengeGame(MapLocation.GorillaVillage, 1);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.GorillaVillage_challengeGame_3:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.NONE);
                SetChallengeGame(MapLocation.GorillaVillage, 2);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.VillageChallengeDefeated:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.GorillaVillage);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.MudslideUnlocked:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.GorillaVillage);
                SetChallengeGamesUpTo(MapLocation.GorillaVillage);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;

            case StoryBeat.MudslideRebuilt:
                StudentInfoSystem.GetCurrentProfile().mapLimit = 3;
                FixIconsUpTo(MapLocation.Mudslide);
                SetChallengeGamesUpTo(MapLocation.Mudslide);
                SetActionWordPool(MapLocation.Mudslide);

                StudentInfoSystem.GetCurrentProfile().unlockedStickerButton = true;
                StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
                break;
        }

        StudentInfoSystem.SaveStudentPlayerData();
    }

    private void SetActionWordPool(MapLocation location)
    {
        StudentInfoSystem.GetCurrentProfile().actionWordPool.Clear();

        if (location <= MapLocation.GorillaVillage)
        {   
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
            StudentInfoSystem.GetCurrentProfile().actionWordPool.Add(ActionWordEnum.mudslide);
        }

        if (location <= MapLocation.Mudslide)
        {
            // add other action words
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
                break;

            case MapLocation.GorillaVillage:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, false);
                break;

            case MapLocation.Mudslide:
                SetMapIcons(MapLocation.GorillaVillage, true);
                SetMapIcons(MapLocation.Mudslide, true);
                break;
        }
    }

    private void SetMapIcons(MapLocation location, bool isFixed)
    {
        StudentInfoSystem.GetCurrentProfile().froggerTutorial = isFixed;
        StudentInfoSystem.GetCurrentProfile().turntablesTutorial = isFixed;
        StudentInfoSystem.GetCurrentProfile().spiderwebTutorial = isFixed;
        StudentInfoSystem.GetCurrentProfile().rummageTutorial = isFixed;

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
        }
    }

    private void SetChallengeGamesUpTo(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.NONE:
                SetChallengeGame(MapLocation.GorillaVillage, 0);
                SetChallengeGame(MapLocation.Mudslide, 0);
                break;

            case MapLocation.GorillaVillage:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 0);
                break;

            case MapLocation.Mudslide:
                SetChallengeGame(MapLocation.GorillaVillage, 3);
                SetChallengeGame(MapLocation.Mudslide, 3);
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

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.GorillaVillage);
                
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

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = GameType.None;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = GameType.None;
                }
                else if (num == 2)
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 0;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = GameType.None;
                }
                else
                {
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.stars = 3;

                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType = StudentInfoSystem.GetChallengeGameType(MapLocation.Mudslide);
                
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_stars = 3;
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_signPost_unlocked = true;
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
