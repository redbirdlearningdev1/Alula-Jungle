using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryGameSegment
{
    public string text;
    public ActionWordEnum actionWord;
    public AudioClip audio;
    public float audioDuration; // time before action word is read
}

public enum StoryGameBackground
{
    Beginning, Emerging, followRed, Prologue, Resolution
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 0)]
public class StoryGameData : GameData
{
    [Header("Story Game Data")]
    public string storyName;
    public StoryGameBackground background;
    public List<StoryGameSegment> segments;
}
