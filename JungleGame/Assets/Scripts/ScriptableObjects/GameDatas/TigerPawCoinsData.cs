using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TigerPawCoinsData", order = 0)]
public class TigerPawCoinsData : GameData
{
    [Header("Word Factory Game Data")]
    public List<ChallengeWord> challengeWordPool;
}

