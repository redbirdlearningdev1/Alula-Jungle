using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SubstitutionPair", order = 2)]
public class SubstitutionPair : ScriptableObject
{
    public ChallengeWord word1;
    public ChallengeWord word2;
    public int swipeIndex;
}
