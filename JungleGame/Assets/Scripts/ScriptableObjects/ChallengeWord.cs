using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum ElkoninValue
{
    // empty coins
    empty_gold,
    empty_silver,

    // action words
    mudslide,
    listen,
    poop,
    orcs,
    think,

    hello,
    spider,
    explorer,
    scared,
    thatguy,

    choice,
    strongwind,
    pirate,
    gorilla,
    sounds,
    give,

    backpack,
    frustrating,
    bumphead,
    baby,
    hit,

    // consonants
    b,
    c,
    ch,
    d,
    f,
    g,
    h,
    j,
    k,
    l,
    m,
    n,
    p,
    qu,
    r,
    s, 
    sh,
    t,
    th,
    v,
    w,
    x,
    y,
    z,

    COUNT
}

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChallengeWord", order = 2)]
public class ChallengeWord : ScriptableObject
{
    public string word;
    public AssetReference audio;
    public Sprite sprite;
    public List<ElkoninValue> elkoninList;
    public int elkoninCount;
    public ActionWordEnum set;
}