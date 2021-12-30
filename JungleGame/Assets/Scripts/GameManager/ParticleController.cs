using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    stars, sand, swirl
}

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public bool isOn = false;
    public ParticleType particleType;

    public GameObject starParticle;
    public GameObject sandParticle;
    public GameObject swirlParticle;

    public float switchParticleTime;
    private float switchTimer = 0f;

    private float timer = 0f;

    public Vector2 delta = Vector2.zero;
    private Vector2 lastPos = Vector2.zero;

    private GameObject currentParticle;
    private float currentRate;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        // return if not on
        if (!isOn)
            return;

        switchTimer += Time.deltaTime;
        if (switchTimer > switchParticleTime)
        {
            switchTimer = 0f;

            particleType++;
            if (particleType > ParticleType.swirl)
            {
                particleType = ParticleType.stars;
            }
        }

        // if mouse button held down
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            delta = mousePos2 - lastPos;
    
            lastPos = Input.mousePosition;
            timer += Time.deltaTime;

            switch (particleType)
            {
                default:
                case ParticleType.stars: 
                    currentParticle = starParticle;
                    
                    break;
                
                case ParticleType.sand: 
                    currentParticle = sandParticle;
                    break;

                case ParticleType.swirl: 
                    currentParticle = swirlParticle;
                    break;
            }

            currentRate = currentParticle.GetComponent<FunParticle>().rate;

            if (timer > currentRate)
            {
                timer = 0f;

                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0f;

                GameObject star = Instantiate(currentParticle, position, Quaternion.identity, this.transform);
                star.GetComponent<FunParticle>().StartParticle();
            }
        }
    }
}
