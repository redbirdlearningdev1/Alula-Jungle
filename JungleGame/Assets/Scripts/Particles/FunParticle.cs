using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunParticle : MonoBehaviour
{
    public Rigidbody2D body;
    public LerpableObject lerpableObject;
    public Image image;

    public float duration;
    public float rate;
    public bool randomizeRotation;

    public Vector2 forceRange;
    public Vector2 torqueRange;
    public Vector2 scaleRange;

    [Header("Color Stuff")]
    public List<Color> colors;


    public void StartParticle()
    {
        StartCoroutine(StartParticleRoutine());
    }

    private IEnumerator StartParticleRoutine()
    {
        // set scale to 0
        transform.localScale = new Vector3(0f, 0f, 1f);
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);

        // randomize rotation
        if (randomizeRotation)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        // get random vector direction
        Vector2 randomVector = Random.insideUnitCircle;
        randomVector.Normalize();
        randomVector += ParticleController.instance.delta.normalized;
        randomVector *= Random.Range(forceRange.x, forceRange.y);

        // apply force in that direction
        body.AddForce(randomVector);

        // lerp scale
        lerpableObject.SquishyScaleLerp(new Vector2(randomScale * 1.5f, randomScale * 1.5f), new Vector2(randomScale * 1f, randomScale * 1f), 0.1f, 0.1f);

        // apply torque
        body.AddTorque(Random.Range(torqueRange.x, torqueRange.y));

        // set color
        image.color = colors[Random.Range(0, colors.Count)];

        yield return new WaitForSeconds(duration);

        // lerp scale
        lerpableObject.SquishyScaleLerp(new Vector2(randomScale * 1.5f, randomScale * 1.5f), new Vector2(0f, 0f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.25f);

        // delete object
        Destroy(gameObject);
    }
}
