using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RummageGameData", order = 0)]
public class RummageGameData : GameData
{
    [Header("Rummage Game Data")]
    public List<ActionWordEnum> wordPool;
}
