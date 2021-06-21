using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryGameSegment
{
    [Header("Read Text")]
    public bool containsText;
    public string text;
    public AudioClip textAudio;
    
    [Header("Action Word")]
    public bool containsActionWord;
    public string actionWordText;
    public ActionWordEnum actionWord;
    public AudioClip wordAudio;
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
