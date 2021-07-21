using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionWordEnum {
    _blank,

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
    SIZE
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActionWord", order = 2)]
public class ActionWord : GameWord
{
    public ActionWordEnum _enum;

    // turntables game
    public Sprite doorIcon;
    public Sprite frameIcon;
    public Sprite centerIcon;

    //SeaShell Game
    
}
