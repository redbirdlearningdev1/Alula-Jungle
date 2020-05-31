using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentPlayer
{
    public bool active { get; private set; } // bool to determine if someone has created this student player
    public string name { get; private set; } // name of student
    public int totalStars { get; private set; } // total number of stars
    // can add many more things here!
}
