using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteShakeController : MonoBehaviour
{
    [Header("Shake Variables")]
    public bool test;
    private bool testing;

    public float shakeSpeed; // how fast it shakes
    public float shakeAmount; // how much it shakes
    private Transform tform;

    void Update()
    {
        if (test && !testing)
        {
            testing = true;
            ShakeObject(3f);
        }
    }

    public void SetTransform(Transform tform)
    {
        this.tform = tform;
    }

    public void ShakeObject(float duration)
    {
        // get sprite renderer if null
        if (tform == null)
            tform = GetComponent<Transform>();

        StartCoroutine(ShakeObjectRoutine(duration));
    }

    private IEnumerator ShakeObjectRoutine(float duration)
    {
        float timer = 0f;
        Vector3 originalPos = tform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                tform.position = originalPos;
                break;
            }

            Vector3 pos = originalPos;
            pos.x = originalPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            tform.position = pos;
            yield return null;
        }
        testing = false;
    }
}
