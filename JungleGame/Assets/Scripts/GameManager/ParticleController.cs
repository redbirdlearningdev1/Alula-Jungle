using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    stars, sand, swirl, block, bubble
}

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public bool isOn = false;
    public ParticleType particleType;

    public GameObject starParticle;
    public GameObject sandParticle;
    public GameObject swirlParticle;
    public GameObject blockParticle;
    public GameObject bubbleParticle;

    public List<GameObject> colliderObjects;
    private bool colliderState = false; // off by default

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

        // turn off collider objects
        ToggleColliderObjects(false);
    }

    void Update()
    {
        // return if not on
        if (!isOn)
        {
            // turn off colliders iff on
            if (colliderState)
            {
                colliderState = false;
                ToggleColliderObjects(false);
            }
            return;
        }

        // turn on colliders iff off
        if (!colliderState)
        {
            colliderState = true;
            ToggleColliderObjects(true);
        }

        switchTimer += Time.deltaTime;
        if (switchTimer > switchParticleTime)
        {
            switchTimer = 0f;

            //particleType++;
            //if (particleType > ParticleType.bubble)
            //{
            //    particleType = ParticleType.stars;
            //}
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

                case ParticleType.block: 
                    currentParticle = blockParticle;
                    break;

                case ParticleType.bubble: 
                    currentParticle = bubbleParticle;
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

    private void ToggleColliderObjects(bool opt)
    {
        foreach (var collider in colliderObjects)
        {
            collider.SetActive(opt);
        }
    }

    public void SetActiveParticles(string charName)
    {
        if(charName == "Red" || charName == "Wally")
        {
            particleType = ParticleType.stars;
        }
        else if(charName == "Clogg" || charName == "Ollie" || charName == "Lester")
        {
            particleType = ParticleType.sand;
        }
        else if(charName == "Julius" || charName == "Marcus" || charName == "Brutus")
        {
            particleType = ParticleType.swirl;
        }
        else if(charName == "Bubbles" || charName == "Sylvie" || charName == "Celest")
        {
            particleType = ParticleType.bubble;
        }
        else if(charName == "Darwin" || charName == "Taxi Bird" || charName == "Spindle")
        {
            particleType = ParticleType.block;
        }
       
    }
}
