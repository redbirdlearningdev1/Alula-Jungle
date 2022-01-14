using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    Darwin_crate, Red_feather, Wally_stars, Monkey_bananas, Julius_swirls, Lester_sand, Clogg_swirls, Spindle_bugs, Ollie_feathers, Celeste_sand, Sylvie_sand, Taxi_feathers
}

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public bool isOn = false;
    public ParticleType particleType;

    public GameObject darwinCrateParticle;
    public GameObject redFeatherParticle;
    public GameObject wallyStarsParticle;

    public GameObject bananaParticle;
    public GameObject juliusSwirlParticle;

    public GameObject lesterSandParticle;
    public GameObject cloggSwirlParticle;
    public GameObject spindleBugParticle;
    public GameObject ollieFeatherParticle;
    public GameObject celesteSandParticle;
    public GameObject sylvieSandParticle;
    public GameObject taxiFeatherParticle;

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
                case ParticleType.Darwin_crate: 
                    currentParticle = darwinCrateParticle;
                    break;
                
                case ParticleType.Red_feather: 
                    currentParticle = redFeatherParticle;
                    break;

                case ParticleType.Wally_stars: 
                    currentParticle = wallyStarsParticle;
                    break;

                case ParticleType.Monkey_bananas: 
                    currentParticle = bananaParticle;
                    break;

                case ParticleType.Julius_swirls: 
                    currentParticle = juliusSwirlParticle;
                    break;

                case ParticleType.Lester_sand: 
                    currentParticle = lesterSandParticle;
                    break;

                case ParticleType.Clogg_swirls: 
                    currentParticle = cloggSwirlParticle;
                    break;

                case ParticleType.Spindle_bugs: 
                    currentParticle = spindleBugParticle;
                    break;

                case ParticleType.Ollie_feathers: 
                    currentParticle = ollieFeatherParticle;
                    break;

                case ParticleType.Celeste_sand: 
                    currentParticle = celesteSandParticle;
                    break;

                case ParticleType.Sylvie_sand: 
                    currentParticle = sylvieSandParticle;
                    break;

                case ParticleType.Taxi_feathers: 
                    currentParticle = taxiFeatherParticle;
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

    public void SetActiveParticles(TalkieCharacter character)
    {
        if (character == TalkieCharacter.Red)
        {
            particleType = ParticleType.Red_feather;
        }
        else if(character == TalkieCharacter.Darwin)
        {
            particleType = ParticleType.Darwin_crate;
        }
        else if(character == TalkieCharacter.Wally)
        {
            particleType = ParticleType.Wally_stars;
        }
        else if(character == TalkieCharacter.Marcus || character == TalkieCharacter.Brutus)
        {
            particleType = ParticleType.Monkey_bananas;
        }
        else if(character == TalkieCharacter.Julius)
        {
            particleType = ParticleType.Julius_swirls;
        }
        else if(character == TalkieCharacter.Lester)
        {
            particleType = ParticleType.Lester_sand;
        }
        else if(character == TalkieCharacter.Clogg)
        {
            particleType = ParticleType.Clogg_swirls;
        }
        else if(character == TalkieCharacter.Spindle)
        {
            particleType = ParticleType.Spindle_bugs;
        }
        else if(character == TalkieCharacter.Ollie)
        {
            particleType = ParticleType.Ollie_feathers;
        }
        else if(character == TalkieCharacter.Celeste)
        {
            particleType = ParticleType.Celeste_sand;
        }
        else if(character == TalkieCharacter.Sylvie)
        {
            particleType = ParticleType.Sylvie_sand;
        }
        else if(character == TalkieCharacter.Taxi)
        {
            particleType = ParticleType.Taxi_feathers;
        }
    }
}
