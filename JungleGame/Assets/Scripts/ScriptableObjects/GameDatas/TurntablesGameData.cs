using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TurntablesGameData", order = 0)]
public class TurntablesGameData : GameData
{
    [Header("Turntables Game Data")]
    public List<ActionWordEnum> wordPool;
}