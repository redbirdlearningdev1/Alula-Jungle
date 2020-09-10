﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioInputTestSceneManager : MonoBehaviour
{
    [SerializeField] private Image volumeBar;
    public float timeBetwnVolumeUpdates;
    private float timer = 0f;
    private AudioInput audioInputScript;

    void Awake() 
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();
    }

    void Start()
    {
        audioInputScript = GetComponent<AudioInput>();
    }

    void Update()
    {
        if (timer < timeBetwnVolumeUpdates)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            float volumeLevel = audioInputScript.volumeLevel * 5;
            if (volumeLevel <= 3f)
            {
                volumeBar.color = Color.green;
            }
            else if (volumeLevel <= 4f)
            {
                volumeBar.color = Color.yellow;
            }
            else 
            {
                volumeBar.color = Color.red;
            }
            volumeBar.transform.localScale = new Vector3(0.35f, volumeLevel, 1f);
        }
    }

    public void OnDevMenuPressed()
    {
        GameHelper.LoadScene("DevMenu", true);
    }
}
