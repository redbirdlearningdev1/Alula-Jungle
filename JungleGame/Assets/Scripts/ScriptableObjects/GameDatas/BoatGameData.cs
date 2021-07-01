using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BoatGameData", order = 0)]
public class BoatGameData : GameData
{
    [Header("Boat Game Data")]
    public string boatName;
}
