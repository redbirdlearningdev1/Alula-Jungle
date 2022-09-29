using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class StoryGameSegment
{
    public string text;
    public AssetReference audio;
    public bool moveWord;
    public ActionWordEnum actionWord;
    public bool requireInput;
    public string postText;
    public bool advanceBG;
}

public enum StoryGameBackground
{
    Prologue, Beginning, FollowRed, Emerging, Resolution, Prologue_Short, Beginning_Short, FollowRed_Short, Emerging_Short, Resolution_Short
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 0)]
public class StoryGameData : ScriptableObject
{
    [Header("Story Game Data")]
    public string storyName;
    public StoryGameBackground background;
    public List<StoryGameSegment> segments;
}