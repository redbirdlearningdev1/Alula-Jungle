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
    private SpriteRenderer spriteRenderer;

    void Update()
    {
        if (test && !testing)
        {
            testing = true;
            ShakeObject(3f);
        }
    }

    public void SetSpriteRenderer(SpriteRenderer renderer)
    {
        spriteRenderer = renderer;
    }

    public void ShakeObject(float duration)
    {
        // get sprite renderer if null
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(ShakeObjectRoutine(duration));
    }

    private IEnumerator ShakeObjectRoutine(float duration)
    {
        float timer = 0f;
        Vector3 originalPos = spriteRenderer.transform.position;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                spriteRenderer.transform.position = originalPos;
                break;
            }

            Vector3 pos = originalPos;
            pos.x = originalPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            spriteRenderer.transform.position = pos;
            yield return null;
        }
        testing = false;
    }
}
