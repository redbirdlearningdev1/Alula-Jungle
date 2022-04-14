using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class FunParticle : MonoBehaviour
{
    public Rigidbody2D body;
    public LerpableObject lerpableObject;
    public Image image;

    [Header("Particle Perameters")]
    public AssetReference spawnSound;
    public float duration;
    public float rate;
    public bool randomizeRotation;

    public Vector2 forceRange;
    public bool addForceSidedways;

    public Vector2 torqueRange;

    public Vector2 scaleRange;
    public bool reverseObject;

    [Header("Special Particles")]
    public bool isBug;
    public Vector2 bugRotateTime;
    public float bugAngleOffset;
    public bool isBubble;
    public Sprite poppedBubble;


    // private vars
    private float randomScale;
    private float reverseObjectMult = 1f;

    [Header("Color Stuff")]
    public List<Color> colors;

    
    void Awake()
    {
        // reverse object mulitplier
        reverseObjectMult = 1f;
        if (reverseObject)
        {
            reverseObjectMult = -1;
        }
    }

    public void SetNormalScale()
    {
        transform.localScale = new Vector3(randomScale * reverseObjectMult * 1f, randomScale * 1f);
    }

    public void StartParticle()
    {
        StartCoroutine(StartParticleRoutine());
    }

    private IEnumerator StartParticleRoutine()
    {
        // set scale to 0
        transform.localScale = new Vector3(0f, 0f, 1f);
        randomScale = Random.Range(scaleRange.x, scaleRange.y);

        // randomize rotation
        if (randomizeRotation)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        // determine force vector
        Vector2 randomVector = Vector2.one;
        if (addForceSidedways)
        {   
            // shoot left or right
            randomVector += ParticleController.instance.delta.normalized;

            // determine iff left or right;
            if (Random.value < 0.5f)
            {
                randomVector.x *= Random.Range(forceRange.x, forceRange.y);
            }
            else
            {
                randomVector.x *= Random.Range(forceRange.x, forceRange.y) * -1;
            }
        }
        else
        {
            // get random vector direction
            randomVector = Random.insideUnitCircle;
            randomVector.Normalize();
            randomVector += ParticleController.instance.delta.normalized;
            randomVector *= Random.Range(forceRange.x, forceRange.y);
        }

        // apply force in that direction
        body.AddForce(randomVector);

        // set rotation to be towards velocity iff bug
        if (isBug)
        {
            yield return new WaitForSeconds(0.1f);
            print ("current angle: " + transform.eulerAngles.z);

            // rotate bug in direction of velocity
            float angle = (Mathf.Atan(body.velocity.y / body.velocity.x) * Mathf.Rad2Deg) + bugAngleOffset;

            // rotate 180 degrees iff velocity is positive x
            if (body.velocity.x > 0f)
            {
                angle += 180;
            }

            print ("velocity: " + body.velocity);
            print ("calculated velocity angle: " + angle);

            GetComponent<LerpableObject>().LerpRotation(angle, 0.1f);
        }

        // lerp scale
        lerpableObject.SquishyScaleLerp(new Vector2(randomScale * reverseObjectMult * 1.5f, randomScale * 1.5f), new Vector2(randomScale * reverseObjectMult * 1f, randomScale * 1f), 0.1f, 0.1f);

        // apply torque
        float torque = Random.Range(torqueRange.x, torqueRange.y);
        body.angularVelocity = torque;

        // set color if list is not empty
        if (colors.Count > 0)
            image.color = colors[Random.Range(0, colors.Count)];

        // bug stuff
        if (isBug)
        {
            StartCoroutine(BugMovement());
        }

        yield return new WaitForSeconds(duration);

        // lerp scale
        lerpableObject.SquishyScaleLerp(new Vector2(randomScale * reverseObjectMult  * 1.5f, randomScale * 1.5f), new Vector2(0f, 0f), 0.1f, 0.1f);
        
        // bubble stuff
        if (isBubble)
        {
            image.sprite = poppedBubble;
        }

        yield return new WaitForSeconds(0.25f);

        // delete object
        Destroy(gameObject);
    }

    private IEnumerator BugMovement()
    {
        while (true)
        {
            // random duration
            float time = Random.Range(bugRotateTime.x, bugRotateTime.y);

            yield return new WaitForSeconds(time);

            // slow down bug in current direction
            body.velocity *= 0.5f;
            
            // set new direction
            Vector2 randomVector = Vector2.one;
            // get random vector direction
            randomVector = Random.insideUnitCircle;
            randomVector.Normalize();
            randomVector *= Random.Range(forceRange.x, forceRange.y);
            randomVector *= 1.1f;

            // apply force in that direction
            body.AddForce(randomVector);

            yield return new WaitForSeconds(0.1f);
            print ("current angle: " + transform.eulerAngles.z);

            // rotate bug in direction of velocity
            float angle = (Mathf.Atan(body.velocity.y / body.velocity.x) * Mathf.Rad2Deg) + bugAngleOffset;

            // rotate 180 degrees iff velocity is positive x
            if (body.velocity.x > 0f)
            {
                angle += 180;
            }

            print ("velocity: " + body.velocity);
            print ("calculated velocity angle: " + angle);

            GetComponent<LerpableObject>().LerpRotation(angle, 0.1f);
        }
        
    }
}
