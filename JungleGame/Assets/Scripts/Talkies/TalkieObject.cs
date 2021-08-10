using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TalkieCharacter
{
    None, Red, Wally, Darwin, Lester, Brutus, Marcus, Julius, Clogg, Spindle
}

public enum TalkieMouth
{
    None, Open, Closed
}

public enum TalkieEyes
{
    None, Inwards, Player, Closed, Outwards
}


public enum ActiveCharacter
{
    Left, Right
}

[System.Serializable]
public struct TalkieSegment
{
    [Header("Left Character")]
    public TalkieCharacter leftCharacter;
    public int leftEmotionNum;
    public TalkieMouth leftMouthEnum;
    public TalkieEyes leftEyesEnum;

    [Header("Right Character")]
    public TalkieCharacter rightCharacter;
    public int rightEmotionNum;
    public TalkieMouth rightMouthEnum;
    public TalkieEyes rightEyesEnum;

    [Header("Active Character")]
    public ActiveCharacter activeCharacter; // which character is talking?

    [Header("Audio")]
    public AudioClip audioClip; // audio to play
    public string audioClipName; // audio to play
    public string audioString; // subtitles
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TalkieObject", order = 2)]
public class TalkieObject : ScriptableObject
{
    public string talkieName;
    public List<TalkieSegment> segmnets;
}