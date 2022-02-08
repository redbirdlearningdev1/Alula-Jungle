using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteParticle : MonoBehaviour
{
    public void Delete(float time)
    {
        StartCoroutine(DeleteParticleRoutine(time));
    }

    private IEnumerator DeleteParticleRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        // lerp scale
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.5f, 1.5f), new Vector2(0f, 0f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.25f);

        // delete object
        Destroy(gameObject);
    }
}
