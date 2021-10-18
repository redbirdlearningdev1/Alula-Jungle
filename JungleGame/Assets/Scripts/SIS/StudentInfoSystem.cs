﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StudentInfoSystem
{
    private static StudentPlayerData currentStudentPlayer;

    public static StudentPlayerData GetCurrentProfile()
    {
        // be default - use profile 1
        if (currentStudentPlayer == null)
        {
            SetStudentPlayer(StudentIndex.student_1);
        }
        return currentStudentPlayer;
    }

    public static void SetStudentPlayer(StudentIndex index)
    {
        SaveStudentPlayerData();
        currentStudentPlayer = LoadSaveSystem.LoadStudentData(index); // load new student data
        SettingsManager.instance.LoadSettingsFromProfile(); // load profile settings
        DropdownToolbar.instance.LoadToolbarDataFromProfile(); // load profile coins
        GameManager.instance.SendLog("StudentInfoSystem", "current profile set to: " + index);
    }

    public static void RemoveCurrentStudentPlayer()
    {
        currentStudentPlayer = null;
    }

    public static void SaveStudentPlayerData()
    {
        if (currentStudentPlayer != null)
            LoadSaveSystem.SaveStudentData(currentStudentPlayer);  // save current student data
        else
            Debug.Log("Current student player is null.");
    }

    public static StudentPlayerData GetStudentData(StudentIndex index)
    {
        return LoadSaveSystem.LoadStudentData(index);
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

    public static GameType GetChallengeGameType(MapLocation location)
    {
        // create list of challenge game options
        List<GameType> challengeGameOptions = new List<GameType>();
        challengeGameOptions.Add(GameType.WordFactoryBlending);
        challengeGameOptions.Add(GameType.WordFactoryBuilding);
        challengeGameOptions.Add(GameType.WordFactoryDeleting);
        challengeGameOptions.Add(GameType.WordFactorySubstituting);
        challengeGameOptions.Add(GameType.TigerPawCoins);
        challengeGameOptions.Add(GameType.TigerPawPhotos);


        // remove options that are already used in this area
        switch (location)
        {
            case MapLocation.GorillaVillage:
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType);
                challengeGameOptions.Remove(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType);
                break;

            // add other cases here
        }

        // return random index
        int index = Random.Range(0, challengeGameOptions.Count);
        return challengeGameOptions[index];
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

            // increment sticker count by one
            currentStudentPlayer.stickerInventory[FindInventoryIndex(sticker)].count++;
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
