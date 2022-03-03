using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BirdSpawnDirection
{
    Random, Left, Right
}

public class DockedBirdSpawnPoint : MonoBehaviour
{
    [Header("Color Settings")]
    [Tooltip("Override DockedBoatManager settings with these colors")]
    public bool overrideColor;
    public List<Color> birdColors = new List<Color>() { Color.white };

    [Header("Scale Settings")]
    [Tooltip("Override DockedBoatManager settings with this scale")]
    public bool overrideScale;
    public float birdScaleMin = 1f;
    public float birdScaleMax = 1f;

    [Header("Spawn Percent Settings")]
    [Tooltip("Override DockedBoatManager settings with this spawn percent")]
    public bool overrideSpawnPercent;
    [Range(0f, 1f)] public float percentToSpawnBird = 0.5f;

    [Header("Spawn Delay Settings")]
    [Tooltip("Override DockedBoatManager settings with this spawn delay")]
    public bool overrideDelay;
    [SerializeField] private float birdSpawnDelay = 0f;

    [Header("Animation Speed Settings")]
    [Tooltip("Override DockedBoatManager settings with this animation speed")]
    public bool overrideSpeed;
    public float birdSpeedMin = 1f;
    public float birdSpeedMax = 1f;

    [Header("Bird Spawn Direction Settings")]
    public BirdSpawnDirection birdSpawnDirection = BirdSpawnDirection.Random;

    public void Start()
    {
        UpdateBirdSettings();
    }

    public void UpdateBirdSettings()
    {
        if (!overrideColor)
        {
            birdColors = DockedBoatManager.instance.birdColors;
        }
        if (!overrideScale)
        {
            birdScaleMin = DockedBoatManager.instance.birdScaleMin;
            birdScaleMax = DockedBoatManager.instance.birdScaleMax;
        }
        if (!overrideSpawnPercent)
        {
            percentToSpawnBird = DockedBoatManager.instance.percentBirdsToSpawn;
        }
        if (!overrideDelay)
        {
            birdSpawnDelay = DockedBoatManager.instance.birdSpawnDelay;
        }
        if (!overrideSpeed)
        {
            birdSpeedMin = DockedBoatManager.instance.birdSpeedMin;
            birdSpeedMax = DockedBoatManager.instance.birdSpeedMax;
        }
    }

    public void SpawnBird()
    {
        UpdateBirdSettings();
        if (Random.Range(0f, 1f) < percentToSpawnBird)
        {
            StartCoroutine(SpawnBirdCoroutine());
        }
    }

    IEnumerator SpawnBirdCoroutine()
    {
        DockedBoatManager.instance.birdCoroutines++;

        // Wait the starting amount before spawning bird
        yield return new WaitForSeconds(Random.Range(0f, birdSpawnDelay));

        // Spawn bird
        GameObject bird = Instantiate(DockedBoatManager.instance.birdPrefab, transform);

        // Randomly scale the bird size
        Vector3 birdLocalScale = bird.transform.localScale;
        bird.transform.localScale = birdLocalScale * Random.Range(birdScaleMin, birdScaleMax);

        // Set the bird direction
        birdLocalScale = bird.transform.localScale;
        int birdDirection;
        if (birdSpawnDirection == BirdSpawnDirection.Random)
        {
            birdDirection = Random.Range(0, 2) * 2 - 1; 
        }
        else if (birdSpawnDirection == BirdSpawnDirection.Left)
        {
            birdDirection = -1;
        }
        else if (birdSpawnDirection == BirdSpawnDirection.Right)
        {
            birdDirection = 1;
        }
        else
        {
            Debug.LogWarning("Bird Spawn Direction set incorrectly on " + name);
            birdDirection = 1;
        }
        bird.transform.localScale = new Vector3(birdLocalScale.x * birdDirection, birdLocalScale.y, birdLocalScale.z);

        // Randomly choose the bird color
        bird.gameObject.GetComponent<Image>().color = birdColors[Random.Range(0, birdColors.Count)];

        // Set random animation play speed
        float birdSpeed = Random.Range(birdSpeedMin, birdSpeedMax);
        bird.gameObject.GetComponent<Animator>().SetFloat("SpeedMultiplier", birdSpeed);

        // Wait for bird animation to finish playing
        yield return new WaitForSeconds(DockedBoatManager.instance.birdsClip.length * (1.0f / birdSpeed));

        // Destroy the bird
        Destroy(bird);
        DockedBoatManager.instance.birdCoroutines--;
    }
}
