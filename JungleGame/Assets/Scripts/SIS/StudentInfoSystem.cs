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
        GameManager.instance.SendLog(GameManager.instance, "current profile set to: " + index);
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
}
