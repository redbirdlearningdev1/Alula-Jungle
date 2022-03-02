using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockedBoatManager : MonoBehaviour
{
    public static DockedBoatManager instance;

    [Header("Animators")]
    public DockedSpiderController spider;

    [Header("Animation Clips")]
    public AnimationClip birdsClip;
    public AnimationClip bubblesClip;

    [Header("Boat Buttons")]
    public BoatButton greenButton;
    public BoatButton blueButton;
    public BoatButton micButton;
    public BoatButton soundButton;
    public BoatButton escapeButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private GameObject bubblesPrefab;

    [Header("Random Bird Spawn Settings")]
    [SerializeField] private List<Transform> birdSpawnPoints;
    [SerializeField] private List<Color> birdColors;
    [SerializeField] private float birdScaleMin;
    [SerializeField] private float birdScaleMax;
    [SerializeField][Range(0f, 1f)] private float percentBirdsToSpawn;
    [SerializeField] private float birdSpawnDelay;
    [SerializeField] private float birdSpeedMin;
    [SerializeField] private float birdSpeedMax;


    [Header("Random Bubble Spawn Settings")]
    [SerializeField] private Transform bubbleSpawnpoint;
    [SerializeField] private Transform bubbleSpawnHitbox;
    [SerializeField] private Transform bubbleSpawnHitboxLeftBound;
    [SerializeField] private Transform bubbleSpawnHitboxTopBound;
    [SerializeField] private float bubbleScaleMin;
    [SerializeField] private float bubbleScaleMax;
    [SerializeField] private float bubbleSpawnIntervalMin;
    [SerializeField] private float bubbleSpawnIntervalMax;
    [SerializeField] private float bubbleSpeedMin;
    [SerializeField] private float bubbleSpeedMax;

    private int birdCoroutines;
    private int currentBubbleCoroutinesCount;
    private Coroutine bubblesCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        DockedBoatWheelController.instance.isOn = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BoatButtonPressed(BoatButtonID id)
    {
        // make fx sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        switch (id)
        {
            case BoatButtonID.Blue:
                BlueButtonPressed();
                break;
            case BoatButtonID.Green:
                GreenButtonPressed();
                break;
            case BoatButtonID.Mic:
                MicrophoneButtonPressed();
                break;
            case BoatButtonID.Escape:
                EscapeButtonPressed();
                break;
        }
    }

    public void GreenButtonPressed()
    {
        // play sound effects
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BoatHorn, 0.5f);
        SpawnBirds();
    }

    public void BlueButtonPressed()
    {
        if (DockedBoatWheelController.instance.isOn)
        {
            // Stomp Engine Rumble FX
            AudioManager.instance.StopFX("Engine_Rumble");
            DockedBoatWheelController.instance.isOn = false;
            StopEngineBubbles();
        }
        else
        {
            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EngineStart, 0.5f);
            AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientEngineRumble, 0.2f, "Engine_Rumble");
            DockedBoatWheelController.instance.isOn = true;
            StartEngineBubbles();
        }

    }

    public void MicrophoneButtonPressed()
    {
        //TODO: Find a purpose for the Mic Button
    }

    public void EscapeButtonPressed()
    {
        //TODO: Return to scrollmap
    }

    public void StartEngineBubbles()
    {
        if (bubblesCoroutine != null)
        {
            StopCoroutine(bubblesCoroutine);
        }

        bubblesCoroutine = StartCoroutine(SpawnBubblesCoroutine());
    }

    // Handle spawning multiple bubbles at once
    IEnumerator SpawnBubblesCoroutine()
    {
        while (DockedBoatWheelController.instance.isOn)
        {
            StartCoroutine(SpawnBubbleCoroutine());
            yield return new WaitForSeconds(Random.Range(bubbleSpawnIntervalMin, bubbleSpawnIntervalMax));
        }
    }

    // Handles a single bubble from spawn to destruction
    IEnumerator SpawnBubbleCoroutine()
    {
        currentBubbleCoroutinesCount++;
        // Spawn the bubble
        GameObject bubble = Instantiate(bubblesPrefab, bubbleSpawnpoint);

        // Randomly scale the bubble
        Vector3 bubbleScale = bubble.transform.localScale;
        bubble.transform.localScale = bubbleScale * Random.Range(bubbleScaleMin, bubbleScaleMax);
        bubbleScale = bubble.transform.localScale;
        bubble.transform.localScale = new Vector3(bubbleScale.x * (Random.Range(0, 2) * 2 - 1), bubbleScale.y, bubbleScale.z);

        // Place bubble in the right position with random offset
        Vector3 bubbleBoxPos = bubbleSpawnHitbox.position;
        float bubbleOffsetX = bubbleBoxPos.x - bubbleSpawnHitboxLeftBound.position.x;
        float bubbleOffsetY = bubbleSpawnHitboxTopBound.position.y - bubbleBoxPos.y;
        bubble.transform.position = new Vector3(bubbleBoxPos.x + Random.Range(-1 * bubbleOffsetX, bubbleOffsetX), bubbleBoxPos.y + Random.Range(-1 * bubbleOffsetY, bubbleOffsetY), bubbleBoxPos.z);

        // Set random animation play speed
        float bubbleSpeed = Random.Range(bubbleSpeedMin, bubbleSpeedMax);
        bubble.gameObject.GetComponent<Animator>().SetFloat("SpeedMultiplier", bubbleSpeed);

        // Wait for bubble to despawn
        yield return new WaitForSeconds(bubblesClip.length * ( 1.0f / bubbleSpeed));

        // Destroy Bubble
        Destroy(bubble);
        currentBubbleCoroutinesCount--;
    }

    public void StopEngineBubbles()
    {
        if (bubblesCoroutine != null)
        {
            StopCoroutine(bubblesCoroutine);
            bubblesCoroutine = null;
        }
    }

    public void SpawnBirds()
    {
        if (birdCoroutines == 0)
        {
            foreach (Transform spawnPoint in birdSpawnPoints)
            {
                if (Random.Range(0f, 1f) < percentBirdsToSpawn)
                {
                    StartCoroutine(SpawnBirdsCoroutine(Random.Range(0f, birdSpawnDelay), spawnPoint));
                }
            }
        }
    }

    IEnumerator SpawnBirdsCoroutine(float secondsToWait, Transform spawnPoint)
    {
        birdCoroutines++;
        
        // Wait the starting amount before spawning bird
        yield return new WaitForSeconds(secondsToWait);

        // Spawn bird
        GameObject bird = Instantiate(birdPrefab, spawnPoint);

        // Randomly scale the bird size
        Vector3 birdLocalScale = bird.transform.localScale;
        bird.transform.localScale = birdLocalScale * Random.Range(birdScaleMin, birdScaleMax);
        birdLocalScale = bird.transform.localScale;
        bird.transform.localScale = new Vector3(birdLocalScale.x * (Random.Range(0, 2) * 2 - 1), birdLocalScale.y, birdLocalScale.z);

        // Randomly choose the bird color
        bird.gameObject.GetComponent<Image>().color = birdColors[Random.Range(0, birdColors.Count)];

        // Set random animation play speed
        float birdSpeed = Random.Range(birdSpeedMin, birdSpeedMax);
        bird.gameObject.GetComponent<Animator>().SetFloat("SpeedMultiplier", birdSpeed);

        // Wait for bird animation to finish playing
        yield return new WaitForSeconds(birdsClip.length * (1.0f / birdSpeed));

        // Destroy the bird
        Destroy(bird);
        birdCoroutines--;
    }
}
