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
        return currentStudentPlayer;
    }

    public static void SetStudentPlayer(StudentIndex index)
    {
        SaveStudentPlayerData();
        currentStudentPlayer = LoadSaveSystem.LoadStudentData(index, true); // load new student data

        SettingsManager.instance.LoadSettingsFromProfile(); // load profile settings
        DropdownToolbar.instance.LoadToolbarDataFromProfile(); // load profile coins
        GameManager.instance.SendLog("StudentInfoSystem", "set current profile to: " + index);
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

        GameManager.instance.SendLog("StudentInfoSystem", "set most recent profile to: " + index);

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
        LoadSaveSystem.ResetStudentData(index);
    }

    public static void AdvanceStoryBeat()
    {
        currentStudentPlayer.currStoryBeat = (StoryBeat)((int)currentStudentPlayer.currStoryBeat + 1);
    }

    

    /* 
    ################################################
    #   STICKER METHODS
    ################################################
    */

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

    public static void AddStickerToInventory(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            // add sticker to inventory if not already in list
            if (!InventoryContainsSticker(sticker))
            {
                var newData = new InventoryStickerData(sticker);
                currentStudentPlayer.stickerInventory.Add(newData);
                SaveStudentPlayerData();
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

    public static void GlueStickerToBoard(Sticker sticker, Vector2 pos, StickerBoardType board)
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

                // add to board list
                currentStudentPlayer.classicStickerBoard.stickers.Add(data);
                break;
            
            case StickerBoardType.Mossy:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;

                // add to board list
                currentStudentPlayer.mossyStickerBoard.stickers.Add(data);
                break;

            case StickerBoardType.Emerald:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;

                // add to board list
                currentStudentPlayer.emeraldStickerBoard.stickers.Add(data);
                break;

            case StickerBoardType.Beach:
            
                // make new sticker data
                data = new StickerData();
                data.rarity = sticker.rarity;
                data.id = sticker.id;
                data.boardPos = pos;

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
}
