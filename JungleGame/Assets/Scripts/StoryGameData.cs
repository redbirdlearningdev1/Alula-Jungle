using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryGameSegment
{
    public string text;
    public AudioClip audio;
    public bool moveWord;
    public ActionWordEnum actionWord;
    public string postText;
    public bool requireInput;
}

public enum StoryGameBackground
{
    Prologue, Beginning, FollowRed, Emerging, Resolution
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 0)]
public class StoryGameData : ScriptableObject
{
    [Header("Story Game Data")]
    public string storyName;
    public StoryGameBackground background;
    public List<StoryGameSegment> segments;
}