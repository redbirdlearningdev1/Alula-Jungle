using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameWord", order = 2)]
public class GameWord : ScriptableObject
{
    public string _name;
    public AssetReference audio;
    public ElkoninValue elkoninValue;
    public string animation;
}
