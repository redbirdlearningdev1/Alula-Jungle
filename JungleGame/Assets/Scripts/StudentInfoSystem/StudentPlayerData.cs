using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StudentIndex
{
    student_1, student_2, student_3
}

[System.Serializable]
public class StudentPlayerData
{
    public StudentIndex studentIndex; // differentiate btwn student profiles
    public bool active; // bool to determine if someone has created this student player
    public string name; // name of student
    public int totalStars; // total number of stars
    // can add many more things here!
}
