using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBlocker : MonoBehaviour
{
    public string id;

    public void DestroyBlocker()
    {
        if (gameObject != null)
            Destroy(gameObject);
    }
}
