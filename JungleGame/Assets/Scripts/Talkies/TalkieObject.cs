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

public enum TalkieEndAction
{
    None,
    UnlockStickerButton
}

public enum TalkieYesNoAction
{
    None,
    // pre darwin talkie
    PreDarwin_yes,
    PreDarwin_no,

    // julius challenges talkie
    JuliusChalllenges_yes,
    JuliusChalllenges_no,

    // julius loses and marcus challenges
    JuliusLosesMarcusChallenges_yes,
    JuliusLosesMarcusChallenges_no,

    // marcus challenges
    MarcusChallenges_yes,
    MarcusChallenges_no,

    // marcus loses and brutus challenges
    MarcusLosesBrutusChallenges_yes,
    MarcusLosesBrutusChallenges_no,

    // brutus challenges
    BrutusChallenges_yes,
    BrutusChallenges_no,

    // prince challenge sign quips
    ChallengeSignQuip_yes,
    ChallengeSignQuip_no
}

[System.Serializable]
public struct TalkieSegment
{
    [Header("Talkie Options")]
    public bool endTalkieAfterThisSegment; // ends talkie after this segment regardles of index in list

    [Header("Yes No Action")] // before continuing the talkie, player must choose between yes and no
    public bool requireYN;
    public TalkieYesNoAction onYes;
    public TalkieYesNoAction onNo;

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
    public bool quipsCollection = false; // instead of a series of segments - this object is a collection of quips (only one will randomly play)

    [Header("Before Talkie Options")]
    public TalkieStart start;
    public bool addBackgroundBeforeTalkie = true; // by default, the background goes away
    public bool addLetterboxBeforeTalkie = true; // by default, the letterbox goes away

    [Header("After Talkie Options")]
    public TalkieEnding ending;
    public bool removeBackgroundAfterTalkie = true; // by default, the background goes away
    public bool removeLetterboxAfterTalkie = true; // by default, the letterbox goes away

    public TalkieEndAction endAction = TalkieEndAction.None; // an action is performed after the talkie finishes

    [Header("Segments")]
    public List<TalkieSegment> segmnets;
}