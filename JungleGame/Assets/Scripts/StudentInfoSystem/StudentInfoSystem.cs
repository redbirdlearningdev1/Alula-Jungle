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
    }

    public static void SaveStudentPlayerData()
    {
        if (currentStudentPlayer != null)
            LoadSaveSystem.SaveStudentData(currentStudentPlayer);  // save current student data
        else
            Debug.Log("Current student player is null.");
    }

    
}
