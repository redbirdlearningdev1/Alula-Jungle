using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DeletionPair", order = 2)]
public class DeletionPair : ScriptableObject
{
    public ChallengeWord word1;
    public ChallengeWord word2;
    public int swipeIndex;
}
