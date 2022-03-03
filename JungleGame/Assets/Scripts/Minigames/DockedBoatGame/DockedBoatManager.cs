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
    public GameObject birdPrefab;
    [SerializeField] private GameObject bubblesPrefab;

    [Header("Global Bird Spawn Settings")]
    public List<DockedBirdSpawnPoint> birdSpawnPoints;
    public List<Color> birdColors;
    public float birdScaleMin;
    public float birdScaleMax;
    [Range(0f, 1f)] public float percentBirdsToSpawn;
    public float birdSpawnDelay;
    public float birdSpeedMin;
    public float birdSpeedMax;


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

    [HideInInspector] public int birdCoroutines;
    private int currentBubbleCoroutinesCount;
    private Coroutine bubblesCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        GameManager.instance.SceneInit();
    }

    // Start is called before the first frame update
    void Start()
    {
        DockedBoatWheelController.instance.isOn = false;
        StartCoroutine(PlayDockedBoatTalkie());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayDockedBoatTalkie()
    {
        if (StudentInfoSystem.GetCurrentProfile().currBoatEncounter == BoatEncounter.FirstTime)
        {
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BoatGame_2_p1"));
            while (TalkieManager.instance.talkiePlaying)
            {
                yield return null;
            }

            StudentInfoSystem.GetCurrentProfile().currBoatEncounter = BoatEncounter.EveryOtherTime;
            StudentInfoSystem.SaveStudentPlayerData();

            if (TalkieManager.instance.doNotContinueToGame)
            {
                TalkieManager.instance.doNotContinueToGame = false;
                GameManager.instance.ReturnToScrollMap();
                yield break;
            }
            else
            {
                GameManager.instance.LoadScene("NewBoatGame", true);
                yield break;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currBoatEncounter == BoatEncounter.SecondTime)
        {
            StudentInfoSystem.GetCurrentProfile().currBoatEncounter = BoatEncounter.EveryOtherTime;
            StudentInfoSystem.SaveStudentPlayerData();

            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BoatGame_2_p2"));
            while (TalkieManager.instance.talkiePlaying)
            {
                yield return null;
            }
        }
        else if (StudentInfoSystem.GetCurrentProfile().currBoatEncounter == BoatEncounter.EveryOtherTime)
        {
            if (GameManager.instance.finishedBoatGame)
            {
                GameManager.instance.finishedBoatGame = false;

                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BoatGame_2_p2"));
                while (TalkieManager.instance.talkiePlaying)
                {
                    yield return null;
                }
            }
            else
            {
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BoatGame_3_p1"));
                while (TalkieManager.instance.talkiePlaying)
                {
                    yield return null;
                }

                if (TalkieManager.instance.doNotContinueToGame)
                {
                    TalkieManager.instance.doNotContinueToGame = false;
                    yield break;
                }
                else
                {
                    GameManager.instance.LoadScene("NewBoatGame", true);
                    yield break;
                }
            }
        }
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
        GameManager.instance.ReturnToScrollMap();
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
        yield return new WaitForSeconds(bubblesClip.length * (1.0f / bubbleSpeed));

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
            foreach (DockedBirdSpawnPoint spawnPoint in birdSpawnPoints)
            {
                spawnPoint.SpawnBird();
            }
        }
    }
}
