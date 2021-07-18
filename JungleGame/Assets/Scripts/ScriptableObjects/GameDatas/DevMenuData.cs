using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DevMenuData", order = 3)]
public class DevMenuData : GameData
{
    // this class is just so that MapIcons that do not have a set GameData object can have a copy of this to be taken to the DevMenu
}
