using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChallengeWord", order = 2)]
public class ChallengeWord : ScriptableObject
{
    public string word;
    public AudioClip audio;
    public List<string> elkoninList; // list of elkonin stuff (change laster)
}