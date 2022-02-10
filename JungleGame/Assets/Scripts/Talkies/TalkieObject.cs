using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TalkieCharacter
{
    None, Red, Wally, Darwin, Lester, Brutus, Marcus, Julius, Clogg, Spindle, Bubbles, Ollie, Celeste, Sylvie, Taxi
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
    None, Left, Right, Both
}

public enum TalkieStart
{
    EnterUp,    // characters enter from bottom to top
    EnterSides, // characters enter from respective sides (right from right, left from left)
    EnterLeft,  // characters enter from left side
    EnterRight, // characters enter from right side
}

public enum TalkieEnding
{
    ExitDown,   // characters exit down (default)
    ExitSides,  // characters exit to their respective sides (right to right, left to left)
    ExitLeft,   // characters exit towards Left
    ExitRight   // characters exit towards Right
}

[System.Serializable]
public struct TalkieSegment
{
    [Header("Talkie Options")]
    public bool endTalkieAfterThisSegment; // ends talkie after this segment regardles of index in list

    [Header("Yes No Action")] // before continuing the talkie, player must choose between yes and no
    public bool requireYN;
    public int onYesGoto;
    public int onNoGoto;

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
    public bool quipsCollection = false; // instead of a series of segments - this object is a collection of quips
    public List<int> validQuipIndexes; // where a quip can begin

    [Header("Before Talkie Options")]
    public TalkieStart start;
    public bool addBackgroundBeforeTalkie = true; // by default, the background goes away
    public bool addLetterboxBeforeTalkie = true; // by default, the letterbox goes away

    [Header("After Talkie Options")]
    public TalkieEnding ending;
    public bool removeBackgroundAfterTalkie = true; // by default, the background goes away
    public bool removeLetterboxAfterTalkie = true; // by default, the letterbox goes away

    [Header("Segments")]
    public List<TalkieSegment> segmnets;
}