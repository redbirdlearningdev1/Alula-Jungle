using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInput : MonoBehaviour 
{
    private AudioSource audioSource;
    public float volumeLevel { get; private set; }
    public int micDeviceToUse = 0;

    // Use this for initialization
    IEnumerator Start() 
    {
        // request access to microphone
        yield return Application.RequestUserAuthorization (UserAuthorization.WebCam | UserAuthorization.Microphone);

        if (Application.HasUserAuthorization (UserAuthorization.Microphone)) 
        {
            foreach (string device in Microphone.devices)
            {
                Debug.Log("Microphone name: " + device);
            }
            if (Microphone.devices.Length == 0) 
            {
                Debug.LogError("No microphone detected");
            } 
            else 
            {
                audioSource = gameObject.AddComponent<AudioSource>() as AudioSource;
                if (audioSource == null) 
                {
                    Debug.LogError("Created AudioSource is null");
                }
            }
        } 
        else 
        {
            Debug.LogError("User denined access to microphone");
        }
    }

    // Update is called once per time unit (normally 0.2 seconds)
    void FixedUpdate() 
    {
        if (audioSource != null) 
        {
            if (!Microphone.IsRecording(null))
            {
                audioSource.clip = Microphone.Start(Microphone.devices[micDeviceToUse], true, 999, 44100);
            } 
            else 
            {
                //get mic volume
                int dec = 128;
                float[] waveData = new float[dec];
                int micPosition = Microphone.GetPosition(null)-(dec+1); // null means the first microphone
                audioSource.clip.GetData(waveData, micPosition);
                
                // Getting a peak on the last 128 samples
                float levelMax = 0;
                for (int i = 0; i < dec; i++) 
                {
                    float wavePeak = waveData[i] * waveData[i];
                    if (levelMax < wavePeak)
                        levelMax = wavePeak;
                }
                volumeLevel = Mathf.Sqrt(Mathf.Sqrt(levelMax));
                //Debug.Log("Current mic volume level: " + volumeLevel.ToString());
            }
        }
    }
 }
