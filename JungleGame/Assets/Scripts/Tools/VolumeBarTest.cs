using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeBarTest : MonoBehaviour
{
    private Image volumeBar;
    public float timeBetwnVolumeUpdates;
    private float timer = 0f;
    [SerializeField] private AudioInput audioInputScript;

    void Start()
    {
        volumeBar = GetComponent<Image>();
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
}
