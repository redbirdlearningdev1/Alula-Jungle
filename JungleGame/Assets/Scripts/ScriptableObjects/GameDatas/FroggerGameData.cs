using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FroggerGameData", order = 0)]
public class FroggerGameData : GameData
{
    [Header("Frogger Game Data")]
    public List<ActionWordEnum> wordPool;
}
