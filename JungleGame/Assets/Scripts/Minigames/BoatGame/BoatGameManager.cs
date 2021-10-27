using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatGameManager : MonoBehaviour
{
    public static BoatGameManager instance;

    private bool arrived = false;

    void Awake()
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        if (!instance)
        {
            instance = this;
        }
    }


    public void ArrivedAtIsland()
    {
        if (!arrived)
        {
            arrived = true;
            StartCoroutine(ArrivedAtIslandRoutine());
        }
    }

    private IEnumerator ArrivedAtIslandRoutine()
    {
        // play blip sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        // save to SIS
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        yield return new WaitForSeconds(2f);
        
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
    }

    public void ButtonSoundFX()
    {
        // play blip sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
    }

    public void LeftWheelButton()
    {
        // play blip sound
        //AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.LeftBlip, 1f);
    }

    public void RightWheelButton()
    {
        // play blip sound
        //AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightBlip, 1f);
    }
}
