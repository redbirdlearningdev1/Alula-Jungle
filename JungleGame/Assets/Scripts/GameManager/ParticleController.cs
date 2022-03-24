using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;

    public bool isOn = false;
    public TalkieCharacter currentParticleCharacter;

    // darwin
    public GameObject darwinPaperStack;
    public GameObject darwinBook;
    public GameObject darwinPaper;

    public GameObject redFeather;
    public GameObject wallyHay;

    public GameObject marcusbananaParticle;
    public GameObject brutusbananaParticle;

    public GameObject juliusJewel1;
    public GameObject juliusJewel2;
    public GameObject juliusJewel3;

    public GameObject lesterChest1;
    public GameObject lesterChest2;
    public GameObject lesterChest3;
    public GameObject lesterChest4;

    public GameObject cloggAxe;
    public GameObject cloggAxeReverse;

    public GameObject bubblesBubble;

    public GameObject ollieCoin;

    public GameObject spindleBug1;
    public GameObject spindleBug2;
    public GameObject spindleBug3;
    
    public GameObject celesteSand;
    public GameObject celesteShell1;
    public GameObject celesteShell2;
    public GameObject celesteShell3;

    public GameObject sylvieSand;
    public GameObject sylvieShell1;
    public GameObject sylvieShell2;
    public GameObject sylvieShell3;

    public GameObject taxiFeather;

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
                //print ("random number: " + randomNum);

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
                        currentParticle = redFeather;
                        break;

                    case TalkieCharacter.Wally: 
                        currentParticle = wallyHay;
                        break;

                    case TalkieCharacter.Marcus: 
                        currentParticle = marcusbananaParticle;
                        break;
                        
                    case TalkieCharacter.Brutus: 
                        currentParticle = brutusbananaParticle;
                        break;

                    case TalkieCharacter.Julius: 
                        DetermineJuliusParticle();
                        break;

                    case TalkieCharacter.Lester: 
                        if (randomNum < 0.4f)
                        {
                            currentParticle = lesterChest1;
                        }
                        else if (randomNum >= 0.4f && randomNum < 0.7f)
                        {
                            currentParticle = lesterChest2;
                        }
                        else if (randomNum >= 0.7f && randomNum < 0.9f)
                        {
                            currentParticle = lesterChest3;
                        }
                        else
                        {
                            currentParticle = lesterChest4;
                        }
                        break;

                    case TalkieCharacter.Clogg:
                        if (randomNum < 0.5f)
                        {
                            currentParticle = cloggAxe;
                        }
                        else
                        {
                            currentParticle = cloggAxeReverse;
                        }
                        break;

                    case TalkieCharacter.Bubbles:
                        currentParticle = bubblesBubble;
                        break;

                    case TalkieCharacter.Spindle: 
                        if (randomNum < 0.33f)
                        {
                            currentParticle = spindleBug1;
                        }
                        else if (randomNum >= 0.33f && randomNum < 0.66f)
                        {
                            currentParticle = spindleBug2;
                        }
                        else
                        {
                            currentParticle = spindleBug3;
                        }
                        break;

                    case TalkieCharacter.Ollie: 
                        currentParticle = ollieCoin;
                        break;

                    case TalkieCharacter.Celeste: 
                        if (randomNum < 0.995f)
                        {
                            currentParticle = celesteSand;
                        }
                        else
                        {
                            int randomShell = Random.Range(0, 3);
                            switch (randomShell)
                            {
                                case 0:
                                    currentParticle = celesteShell1;
                                    break;
                                case 1:
                                    currentParticle = celesteShell2;
                                    break;
                                case 2:
                                    currentParticle = celesteShell3;
                                    break;
                            }
                        }
                        break;

                    case TalkieCharacter.Sylvie: 
                        if (randomNum < 0.995f)
                        {
                            currentParticle = sylvieSand;
                        }
                        else
                        {
                            int randomShell = Random.Range(0, 3);
                            switch (randomShell)
                            {
                                case 0:
                                    currentParticle = sylvieShell1;
                                    break;
                                case 1:
                                    currentParticle = sylvieShell2;
                                    break;
                                case 2:
                                    currentParticle = sylvieShell3;
                                    break;
                            }
                        }
                        break;

                    case TalkieCharacter.Taxi: 
                        currentParticle = taxiFeather;
                        break;
                }

                // set new rate
                currentRate = currentParticle.GetComponent<FunParticle>().rate;

                // spawn particle
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0f;

                // print ("current particle: " + currentParticle);
                GameObject particle = Instantiate(currentParticle, position, Quaternion.identity, this.transform);
                particle.GetComponent<FunParticle>().StartParticle();

                // play pop sound effect!
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.1f, "particle_spawn", 0.7f);
            }
        }
    }

    private void DetermineJuliusParticle()
    {
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        switch (currChapter)
        {
            case Chapter.chapter_0:
            case Chapter.chapter_1:
            case Chapter.chapter_2:
            case Chapter.chapter_3:
                currentParticle = juliusJewel1;
                break;
            case Chapter.chapter_4:
            case Chapter.chapter_5:
                currentParticle = juliusJewel2;
                break;
            case Chapter.chapter_final:
                currentParticle = juliusJewel3;
                break;
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
