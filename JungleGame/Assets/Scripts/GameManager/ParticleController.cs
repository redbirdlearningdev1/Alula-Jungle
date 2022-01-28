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
    public TalkieCharacter currentParticleCharacter;

    // darwin
    public GameObject darwinPaperStack;
    public GameObject darwinBook;
    public GameObject darwinPaper;


    public GameObject redFeatherParticle;
    public GameObject wallyStarsParticle;

    public GameObject marcusbananaParticle;
    public GameObject brutusbananaParticle;
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

        // if mouse button held down
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            delta = mousePos2 - lastPos;
    
            lastPos = Input.mousePosition;
            timer += Time.deltaTime;

            // spawn new particle
            if (timer > currentRate)
            {
                timer = 0f;
                float randomNum = Random.Range(0f, 1f);

                switch (currentParticleCharacter)
                {
                    default:
                    case TalkieCharacter.None: 
                        currentParticle = null;
                        return;

                    case TalkieCharacter.Darwin:
                        if (randomNum < 0.75f)
                        {
                            currentParticle = darwinPaper;
                        }
                        else if (randomNum >= 0.75f && randomNum < 0.9f)
                        {
                            currentParticle = darwinPaperStack;
                        }
                        else
                        {
                            currentParticle = darwinBook;
                        }
                        break;
                    
                    case TalkieCharacter.Red: 
                        currentParticle = redFeatherParticle;
                        break;

                    case TalkieCharacter.Wally: 
                        currentParticle = wallyStarsParticle;
                        break;

                    case TalkieCharacter.Marcus: 
                        currentParticle = marcusbananaParticle;
                        break;
                        
                    case TalkieCharacter.Brutus: 
                        currentParticle = brutusbananaParticle;
                        break;

                    case TalkieCharacter.Julius: 
                        currentParticle = juliusSwirlParticle;
                        break;

                    case TalkieCharacter.Lester: 
                        currentParticle = lesterSandParticle;
                        break;

                    case TalkieCharacter.Clogg: 
                        currentParticle = cloggSwirlParticle;
                        break;

                    case TalkieCharacter.Spindle: 
                        currentParticle = spindleBugParticle;
                        break;

                    case TalkieCharacter.Ollie: 
                        currentParticle = ollieFeatherParticle;
                        break;

                    case TalkieCharacter.Celeste: 
                        currentParticle = celesteSandParticle;
                        break;

                    case TalkieCharacter.Sylvie: 
                        currentParticle = sylvieSandParticle;
                        break;

                    case TalkieCharacter.Taxi: 
                        currentParticle = taxiFeatherParticle;
                        break;
                }

                // set new rate
                currentRate = currentParticle.GetComponent<FunParticle>().rate;

                // spawn particle
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0f;

                print ("current particle: " + currentParticle);
                GameObject particle = Instantiate(currentParticle, position, Quaternion.identity, this.transform);
                particle.GetComponent<FunParticle>().StartParticle();
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

    public void IncreaseCharacterParticle()
    {
        // increase current character by 1 - set to 0 iff at max
        int nextCharacter = (int)currentParticleCharacter;
        nextCharacter++;

        if (nextCharacter > 14)
        {
            nextCharacter = 0;
        }
        currentParticleCharacter = (TalkieCharacter)nextCharacter;
    }

    public void DecreaseCharacterParticle()
    {
        // decrease current character by 1 - set to 14 iff < 0
        int nextCharacter = (int)currentParticleCharacter;
        nextCharacter--;

        if (nextCharacter < 0)
        {
            nextCharacter = 14;
        }
        currentParticleCharacter = (TalkieCharacter)nextCharacter;
    }

    public void SetActiveParticles(TalkieCharacter character)
    {
        currentParticleCharacter = character;
    }
}
