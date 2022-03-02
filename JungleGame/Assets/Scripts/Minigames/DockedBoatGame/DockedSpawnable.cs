using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockedSpawnable : MonoBehaviour
{
    public void DestroyMe()
    {
        GameObject.Destroy(this);
    }
}
