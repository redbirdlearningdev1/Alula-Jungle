using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogType
{
    Small, Medium, Large
}

public class SingleLog : MonoBehaviour
{
    public LogType logType;
    private Animator animator;
    private bool isUp = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void LogRise()
    {
        if (!isUp)
        {
            // get animator if null
            if (!animator)
                Awake();
            animator.Play("log_rise");
            isUp = true;

            // play audio
            switch (logType)
            {
                case LogType.Small:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.LogRiseSmall, 0.5f);
                    break;
                case LogType.Medium:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.LogRiseMed, 0.5f);
                    break;
                case LogType.Large:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.LogRiseLarge, 0.5f);
                    break;
            }
        }
    }

    public void LogSink()
    {
        if (isUp)
        {
            // get animator if null
            if (!animator)
                Awake();
            animator.Play("log_sink");
            isUp = false;

            // play audio
            switch (logType)
            {
                case LogType.Small:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.WaterSplashSmall, 0.5f);
                    break;
                case LogType.Medium:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.WaterSplashMed, 0.5f);
                    break;
                case LogType.Large:
                    AudioManager.instance.PlayFX(AudioDatabase.instance.WaterSplashLarge, 0.5f);
                    break;
            }
        }
    }
}
