using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryGameSegment
{
    public float duration;
    public AudioClip audio;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/StoryGameData", order = 1)]
public class StoryGameData : ScriptableObject
{
    public List<StoryGameSegment> segments;
}
