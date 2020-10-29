using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StoryGameSegment
{
    public float duration;
    public AudioClip audio;
    public ActionWord actionWord;
}

[System.Serializable]
public class StoryGameImage
{
    public Sprite sprite;
    public Vector2 resolution;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 0)]
public class StoryGameData : GameData
{
    public List<StoryGameSegment> segments;
    public List<StoryGameImage> scrollingBackgroundImages;
}
