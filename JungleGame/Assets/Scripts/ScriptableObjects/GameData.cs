using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    DevMenu,
    StoryGame,
    COUNT
}

public class GameData : ScriptableObject
{
    public GameType gameType;
    public string sceneName;
}
