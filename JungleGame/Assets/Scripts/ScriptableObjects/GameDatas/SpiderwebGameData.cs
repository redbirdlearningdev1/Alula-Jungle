using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpiderwebGameData", order = 0)]
public class SpiderwebGameData : GameData
{
    [Header("Spiderweb Game Data")]
    public List<ActionWordEnum> wordPool;
}
