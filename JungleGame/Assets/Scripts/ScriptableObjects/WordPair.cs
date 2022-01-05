using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordPair", order = 2)]
public class WordPair : ScriptableObject
{
    public ActionWordEnum soundCoin;
    public PairType pairType;
    public ChallengeWord word1;
    public ChallengeWord word2;
    public int index;
}

public enum PairType
{
    sub, del_add
}
