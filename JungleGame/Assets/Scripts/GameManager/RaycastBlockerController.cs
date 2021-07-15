using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBlockerController : MonoBehaviour
{
    public static RaycastBlockerController instance;

    [SerializeField] private GameObject blocker;

    private List<RaycastBlocker> blockers;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        blockers = new List<RaycastBlocker>();
    }

    // creates a new raycast blocker with unique id
    public void CreateRaycastBlocker(string id)
    {
        var newblocker = Instantiate(blocker, this.transform).GetComponent<RaycastBlocker>();
        newblocker.id = id;
        blockers.Add(newblocker);
        //GameManager.instance.SendLog(this, "created new raycast blocker - " + id);
    }

    // destroys raycast blocker using id
    public void RemoveRaycastBlocker(string id)
    {
        foreach(var block in blockers)
        {
            if (block.id == id)
            {
                block.DestroyBlocker();
                blockers.Remove(block);
                //GameManager.instance.SendLog(this, "removed raycast blocker - " + id);
                return;
            }
        }
    }

    // destroy all blockers
    public void ClearAllRaycastBlockers()
    {   
        // make list of ids
        List<string> ids = new List<string>();
        foreach(var block in blockers)
        {
            ids.Add(block.id);
        }

        // remove blockers using ids
        foreach(var id in ids)
        {
            RemoveRaycastBlocker(id);
        }

        blockers.Clear();
    }
}
