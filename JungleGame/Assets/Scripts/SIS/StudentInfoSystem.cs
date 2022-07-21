using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StudentInfoSystem
{
    private static StudentPlayerData currentStudentPlayer;

    public static StudentPlayerData GetCurrentProfile()
    {
        // by default - use profile 1 if in editor
#if UNITY_EDITOR
        if (currentStudentPlayer == null)
        {
            SetStudentPlayer(StudentIndex.student_1);
        }
#endif

        // if current play is null in the game - send error
        if (currentStudentPlayer == null)
        {
            GameManager.instance.SendError("StudentInfoSystem", "Current player is null");
        }
        return currentStudentPlayer;
    }

    public static void SetStudentPlayer(StudentIndex index)
    {
        SaveStudentPlayerData();
        currentStudentPlayer = LoadSaveSystem.LoadStudentData(index, true); // load new student data

        string profile = currentStudentPlayer.name + "_" + currentStudentPlayer.uniqueID;
        AnalyticsManager.SwitchProfile(profile);

        DropdownToolbar.instance.LoadToolbarDataFromProfile(); // load profile coins
        GameManager.instance.SendLog("StudentInfoSystem", "set current profile to: " + index);
        SettingsManager.instance.LoadScrollSettingsFromProfile(); // load in settings
    }

    public static int GetCurrentPlayerTotalStars()
    {
        int total = 0;
        
        // minigames
        total += currentStudentPlayer.starsFrogger;
        total += currentStudentPlayer.starsRummage;
        total += currentStudentPlayer.starsSeashell;
        total += currentStudentPlayer.starsSpiderweb;
        total += currentStudentPlayer.starsTurntables;
        total += currentStudentPlayer.starsPirate;

        // challengegames
        total += currentStudentPlayer.starsBlend;
        total += currentStudentPlayer.starsSub;
        total += currentStudentPlayer.starsBuild;
        total += currentStudentPlayer.starsDel;
        total += currentStudentPlayer.starsTPawCoin;
        total += currentStudentPlayer.starsTPawPol;
        total += currentStudentPlayer.starsPass;

        return total;
    }

    private static void SetMostRecentProfile(StudentIndex index)
    {
        var data1 = LoadSaveSystem.LoadStudentData(StudentIndex.student_1, true);
        var data2 = LoadSaveSystem.LoadStudentData(StudentIndex.student_2, true);
        var data3 = LoadSaveSystem.LoadStudentData(StudentIndex.student_3, true);

        data1.mostRecentProfile = false;
        data2.mostRecentProfile = false;
        data3.mostRecentProfile = false;

        switch (index)
        {
            case StudentIndex.student_1:
                data1.mostRecentProfile = true;
                break;
            case StudentIndex.student_2:
                data2.mostRecentProfile = true;
                break;
            case StudentIndex.student_3:
                data3.mostRecentProfile = true;
                break;
        }

        // GameManager.instance.SendLog("StudentInfoSystem", "set most recent profile to: " + index);

        LoadSaveSystem.SaveStudentData(data1);
        LoadSaveSystem.SaveStudentData(data2);
        LoadSaveSystem.SaveStudentData(data3);
    }

    public static void RemoveCurrentStudentPlayer()
    {
        currentStudentPlayer = null;
    }

    public static void SaveStudentPlayerData()
    {
        if (currentStudentPlayer != null)
        {
            LoadSaveSystem.SaveStudentData(currentStudentPlayer);  // save current student data
            SetMostRecentProfile(currentStudentPlayer.studentIndex); // make profile most recent
            GameManager.instance.SendLog("StudentInfoSystem", "saving current player data - " + currentStudentPlayer.studentIndex);
        }
        else
            GameManager.instance.SendLog("StudentInfoSystem", "could not save player data - current player is null");
    }

    public static StudentPlayerData GetStudentData(StudentIndex index)
    {
        return LoadSaveSystem.LoadStudentData(index, true);
    }

    public static List<StudentPlayerData> GetAllStudentDatas()
    {
        var datas = new List<StudentPlayerData>();
        datas.Add(GetStudentData(StudentIndex.student_1));
        datas.Add(GetStudentData(StudentIndex.student_2));
        datas.Add(GetStudentData(StudentIndex.student_3));

        return datas;
    }

    public static void ResetProfile(StudentIndex index)
    {
        GameManager.instance.SendLog("SIS", "reseting profile: " + index);

        LoadSaveSystem.ResetStudentData(index);
    }

    public static void AdvanceStoryBeat()
    {
        currentStudentPlayer.currStoryBeat = (StoryBeat)((int)currentStudentPlayer.currStoryBeat + 1);
    }

    // PLAYER GAME SIMULATION METHODS

    public static void ResetGameSimulationProfile()
    {
        ResetProfile(StudentIndex.game_simulation_profile);
    }


    /* 
    ################################################
    #   STICKER METHODS
    ################################################
    */

    public static void ResetStickerSimulationProfile()
    {
        ResetProfile(StudentIndex.sticker_simulation_profile);
    }

    public static bool InventoryContainsSticker(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            foreach (var item in currentStudentPlayer.stickerInventory)
            {
                if (item.id == sticker.id && item.rarity == sticker.rarity)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static int FindInventoryIndex(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            int count = 0;
            foreach (var item in currentStudentPlayer.stickerInventory)
            {
                if (item.id == sticker.id && item.rarity == sticker.rarity)
                {
                    return count;
                }
                count++;
            }
        }
        // return invalid index
        return -1;
    }


    public static int GetStickerCount(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            if (InventoryContainsSticker(sticker))
            {
                return currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)].count;
            }
        }
        return 0;
    }

    public static void AddStickerToInventory(Sticker sticker, bool updateText)
    {
        if (currentStudentPlayer != null)
        {
            // add sticker to inventory if not already in list
            if (!InventoryContainsSticker(sticker))
            {
                var newData = new InventoryStickerData(sticker);
                currentStudentPlayer.stickerInventory.Add(newData);
                SaveStudentPlayerData();

                if (updateText)
                    DropdownToolbar.instance.UpdateSilverCoins();
            }
            else
            {
                // increment sticker count by one
                currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)].count++;
            }
        }
    }

    public static void RemoveStickerFromInventory(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            // check to see if sticker exists
            if (InventoryContainsSticker(sticker))
            {
                // decrement by one
                currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)].count--;
                
                // if count is 0, remove sticker from inventory
                if (currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)].count <= 0)
                {
                    var emptySticker = currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)];
                    currentStudentPlayer.stickerInventory.Remove(emptySticker);
                }

                SaveStudentPlayerData();
                DropdownToolbar.instance.UpdateSilverCoins();
            }   
        }
    }

    public static void GlueStickerToBoard(Sticker sticker, Vector2 pos, Vector2 scale, float zAngle, StickerBoardType board)
    {
        StickerData data = new StickerData();

        switch (board)
        {
            case StickerBoardType.Classic:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;
                data.scale = scale;
                data.zAngle = zAngle;

                // add to board list
                currentStudentPlayer.classicStickerBoard.stickers.Add(data);
                break;
            
            case StickerBoardType.Mossy:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;
                data.scale = scale;
                data.zAngle = zAngle;

                // add to board list
                currentStudentPlayer.mossyStickerBoard.stickers.Add(data);
                break;

            case StickerBoardType.Emerald:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;
                data.scale = scale;
                data.zAngle = zAngle;

                // add to board list
                currentStudentPlayer.emeraldStickerBoard.stickers.Add(data);
                break;

            case StickerBoardType.Beach:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;
                data.scale = scale;
                data.zAngle = zAngle;

                // add to board list
                currentStudentPlayer.beachStickerBoard.stickers.Add(data);
                break;
        }
        SaveStudentPlayerData();
    }

    public static void DeleteStickerFromBoard(StickerData stickerData, StickerBoardType board)
    {
        switch (board)
        {
            case StickerBoardType.Classic:
                // search for sticker in board data - delete if found
                StickerBoardData classicBoardData = currentStudentPlayer.classicStickerBoard;
                foreach (var data in classicBoardData.stickers)
                {
                    if (data == stickerData)
                    {
                        classicBoardData.stickers.Remove(data);
                        break;
                    }
                }
                break;
            
            case StickerBoardType.Mossy:
                // search for sticker in board data - delete if found
                StickerBoardData mossyBoardData = currentStudentPlayer.classicStickerBoard;
                foreach (var data in mossyBoardData.stickers)
                {
                    if (data == stickerData)
                    {
                        mossyBoardData.stickers.Remove(data);
                        break;
                    }
                }
                break;

            case StickerBoardType.Emerald:
                // search for sticker in board data - delete if found
                StickerBoardData emeraldBoardData = currentStudentPlayer.classicStickerBoard;
                foreach (var data in emeraldBoardData.stickers)
                {
                    if (data == stickerData)
                    {
                        emeraldBoardData.stickers.Remove(data);
                        break;
                    }
                }
                break;

            case StickerBoardType.Beach:
                // search for sticker in board data - delete if found
                StickerBoardData beachBoardData = currentStudentPlayer.classicStickerBoard;
                foreach (var data in beachBoardData.stickers)
                {
                    if (data == stickerData)
                    {
                        beachBoardData.stickers.Remove(data);
                        break;
                    }
                }
                break;
        }
        SaveStudentPlayerData();
    }

    public static StickerBoardData GetStickerBoardData(StickerBoardType board)
    {
        switch (board)
        {
            default:
            case StickerBoardType.Classic:
                return currentStudentPlayer.classicStickerBoard;
            case StickerBoardType.Mossy:
                return currentStudentPlayer.mossyStickerBoard;
            case StickerBoardType.Emerald:
                return currentStudentPlayer.emeraldStickerBoard;
            case StickerBoardType.Beach:
                return currentStudentPlayer.beachStickerBoard;
        }
    }

    public static int GetTotalStickerCount()
    {
        int totalStickers = 0;
        foreach (var sticker in GetCurrentProfile().stickerInventory)
        {
            totalStickers += sticker.count;
        }
        return totalStickers;
    }


    /////////////////// SAVING DATA FOR PLAYER REPORT ///////////////////

    public static void SavePlayerPhonemeAttempt(ActionWordEnum phoneme, bool success)
    {
        // find phoneme data in current player and update
        foreach (var data in currentStudentPlayer.phonemeData)
        {
            if (data.actionWordEnum == phoneme)
            {
                data.attempts.Add(success);
                SaveStudentPlayerData();
                return;
            }
        }
    }

    public static void SaveOverallMastery()
    {
        // update master level
        int blendNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsBlend / 3);
        int subNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsSub / 3);
        int buildNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsBuild / 3);
        int deleteNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsBlend / 3);
        int tpCoinNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsTPawCoin / 3);
        int tpPhotosNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsTPawPol / 3);
        int passwordNum = 1 + Mathf.FloorToInt(currentStudentPlayer.starsPass / 3);

        float averageNum = (float)(blendNum + subNum + buildNum + deleteNum + tpCoinNum + tpPhotosNum + passwordNum) / 7f;
        averageNum = Mathf.Round(averageNum * 10.0f) * 0.1f;
        
        currentStudentPlayer.overallMasteryPerGame.Add(averageNum);
    }

    public static void SavePlayerPhonemeAttempt(ElkoninValue phoneme, bool success)
    {
        // find phoneme data in current player and update
        foreach (var data in currentStudentPlayer.phonemeData)
        {
            if (data.elkoninValue == phoneme)
            {
                data.attempts.Add(success);
                SaveStudentPlayerData();
                return;
            }
        }
    }

    public static void SavePlayerMinigameRoundAttempt(GameType game, bool _success)
    {
        MinigameRoundData newData = new MinigameRoundData();
        newData.success = _success;
        // save date-time
        System.DateTime dateTime = System.DateTime.Now;
        newData.sec = dateTime.Second;
        newData.min = dateTime.Minute;
        newData.hour = dateTime.Hour;
        newData.day = dateTime.Day;
        newData.month = dateTime.Month;
        newData.year = dateTime.Year;

        switch (game)
        {
            case GameType.FroggerGame:
                currentStudentPlayer.froggerData.Add(newData);
                break;
            
            case GameType.RummageGame:
                currentStudentPlayer.rummageData.Add(newData);
                break;

            case GameType.SeashellGame:
                currentStudentPlayer.seashellsData.Add(newData);
                break;

            case GameType.SpiderwebGame:
                currentStudentPlayer.spiderwebData.Add(newData);
                break;

            case GameType.TurntablesGame:
                currentStudentPlayer.turntablesData.Add(newData);
                break;
            
            case GameType.PirateGame:
                currentStudentPlayer.pirateData.Add(newData);
                break;
        }

        SaveStudentPlayerData();
    }

    public static void SavePlayerChallengeRoundAttempt(GameType game, bool _success, ChallengeWord _word, int _diff)
    {
        ChallengeRoundData newData = new ChallengeRoundData();
        newData.success = _success;
        newData.challengeWord = _word;
        newData.difficulty = _diff;
        // save date-time
        System.DateTime dateTime = System.DateTime.Now;
        newData.sec = dateTime.Second;
        newData.min = dateTime.Minute;
        newData.hour = dateTime.Hour;
        newData.day = dateTime.Day;
        newData.month = dateTime.Month;
        newData.year = dateTime.Year;

        switch (game)
        {
            case GameType.WordFactoryBlending:
                currentStudentPlayer.blendData.Add(newData);
                break;
            
            case GameType.WordFactorySubstituting:
                currentStudentPlayer.subData.Add(newData);
                break;

            case GameType.WordFactoryBuilding:
                currentStudentPlayer.buildData.Add(newData);
                break;

            case GameType.WordFactoryDeleting:
                currentStudentPlayer.deleteData.Add(newData);
                break;

            case GameType.TigerPawCoins:
                currentStudentPlayer.TPCoinsData.Add(newData);
                break;
            
            case GameType.TigerPawPhotos:
                currentStudentPlayer.TPPhotosData.Add(newData);
                break;

            case GameType.Password:
                currentStudentPlayer.passwordData.Add(newData);
                break;
        }

        SaveStudentPlayerData();
    }
}
