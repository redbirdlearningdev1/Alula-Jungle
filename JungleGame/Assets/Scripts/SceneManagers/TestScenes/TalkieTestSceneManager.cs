using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkieTestSceneManager : MonoBehaviour
{
    public TMP_Dropdown talkieDropdown;

    private TalkieObject currentTalkie;

    public static List<TalkieObject> globalTalkieList;

    private bool isPaused = false;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // remove music
        AudioManager.instance.StopMusic();
    }

    void Start()
    {
        // show settings button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // get global talkie list and 
        globalTalkieList = TalkieDatabase.instance.GetGlobalTalkieList();

        // add talkie objects to dropdown
        List<string> talkieStringList = new List<string>();
        for(int i = 0; i < globalTalkieList.Count; i++)
        {
            talkieStringList.Add("" + i + " - " + globalTalkieList[i].name);
        }
        talkieDropdown.ClearOptions();
        talkieDropdown.AddOptions(talkieStringList);
        talkieDropdown.value = 0;
    }

    void Update()
    {
        // press space to pause talkie
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void OnPlayTalkiePressed()
    {
        currentTalkie = globalTalkieList[talkieDropdown.value];
        TalkieManager.instance.PlayTalkie(currentTalkie);
    }
}
