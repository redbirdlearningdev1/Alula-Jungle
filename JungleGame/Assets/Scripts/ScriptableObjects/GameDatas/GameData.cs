using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    DevMenu,
    StoryGame,
    BoatGame,

    FroggerGame,
    TurntablesGame,
    RummageGame,
    SeashellGame,
    PirateGame,
    SpiderwebGame,

    WordFactoryBlending,
    WordFactorySubstituting,
    TigerPawCoins,

    COUNT
}

public class GameData : ScriptableObject
{
    [Header("General Game Data")]
    public GameType gameType;
    public string sceneName;
}
