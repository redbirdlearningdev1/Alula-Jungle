using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActionWord", order = 2)]
public class ActionWord : ScriptableObject
{
    public string word;
    public AudioClip audio;

    // movement animation
    // redbird (ex: ă, ĕ, ĭ, etc.)
    // reference (ex: short a, short e, short i, etc.)
    public string exampleWord;
    // icon
}
