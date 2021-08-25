using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StudentInfoSystem
{
    public static StudentPlayerData currentStudentPlayer { get; private set; }

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

    public static void AddStickerToInventory(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            // add sticker to unlocked stickers if not already in list
            if (!currentStudentPlayer.stickerInventory.Contains(sticker))
            {
                currentStudentPlayer.stickerInventory.Add(sticker);
                SaveStudentPlayerData();
                DropdownToolbar.instance.UpdateSilverCoins();
            }

            // increment sticker count by one
            currentStudentPlayer.stickerInventory.Find(i => i == sticker).count++;
        }
    }

    public static void RemoveStickerFromInventory(Sticker sticker)
    {
        if (currentStudentPlayer != null)
        {
            // check to see if sticker exists
            if (currentStudentPlayer.stickerInventory.Find(i => i.id == sticker.id))
            {
                // decrement by one
                currentStudentPlayer.stickerInventory.Find(i => i.id == sticker.id).count--;
                
                // if count is 0, remove sticker from inventory
                if (currentStudentPlayer.stickerInventory.Find(i => i.id == sticker.id).count <= 0)
                {
                    var emptySticker = currentStudentPlayer.stickerInventory.Find(i => i.id == sticker.id);
                    currentStudentPlayer.stickerInventory.Remove(emptySticker);
                }

                SaveStudentPlayerData();
                DropdownToolbar.instance.UpdateSilverCoins();
            }   
        }
    }

    public static void GlueStickerToBoard(Sticker sticker, Vector2 pos, StickerBoardType board)
    {
        switch (board)
        {
            case StickerBoardType.Classic:
                // make new sticker data
                StickerData data = new StickerData();
                data.stickerObject = sticker;
                data.boardPos = pos;

                // add to board list
                currentStudentPlayer.classicStickerBoard.stickers.Add(data);
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
        }
    }
}
