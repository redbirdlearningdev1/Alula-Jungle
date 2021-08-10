﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryGameSegment
{
    [Header("Read Text")]
    public bool readText;
    public bool writeText;
    public string text;
    public AudioClip textAudio;
    public bool actAsActionWord;
    
    [Header("Action Word")]
    public bool containsActionWord;
    public bool requireMicInput;
    public string actionWordText;
    public ActionWordEnum actionWord;
    public AudioClip wordAudio;

    [Header("Extra Text")]
    public bool containsPostText;
    public string postText;
}

public enum StoryGameBackground
{
    Beginning, Emerging, FollowRed, Prologue, Resolution
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 0)]
public class StoryGameData : GameData
{
    [Header("Story Game Data")]
    public string storyName;
    public StoryGameBackground background;
    public List<StoryGameSegment> segments;
}
