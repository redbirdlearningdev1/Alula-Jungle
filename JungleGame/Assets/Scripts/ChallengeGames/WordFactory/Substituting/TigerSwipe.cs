using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerSwipe : MonoBehaviour
{
    public void PlayTigerSwipe()
    {
        GetComponent<Animator>().Play("tigerSwipe");
        StartCoroutine(DeleteRoutine(1f));
    }

    private IEnumerator DeleteRoutine(float time)
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
