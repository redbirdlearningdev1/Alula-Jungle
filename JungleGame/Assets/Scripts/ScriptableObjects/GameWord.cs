using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameWord", order = 2)]
public class GameWord : ScriptableObject
{
    public string _name;
    public AudioClip audio;
    public ElkoninValue elkoninValue;
    public string animation;
}
