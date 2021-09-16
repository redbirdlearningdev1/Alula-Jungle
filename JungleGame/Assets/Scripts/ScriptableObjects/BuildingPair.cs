using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BuildingPair", order = 2)]
public class BuildingPair : ScriptableObject
{
    public ChallengeWord word1;
    public ChallengeWord word2;
    public int addIndex;
}
