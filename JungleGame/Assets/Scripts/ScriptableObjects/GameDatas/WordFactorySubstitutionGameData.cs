using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordFactorySubstitutionGameData", order = 0)]
public class WordFactorySubstitutionGameData : GameData
{
    [Header("Word Factory Game Data")]
    public List<ChallengeWord> challengeWordPool;
}

