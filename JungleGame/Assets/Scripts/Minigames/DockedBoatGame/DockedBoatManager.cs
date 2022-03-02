using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockedBoatManager : MonoBehaviour
{
    public static DockedBoatManager instance;

    [Header("Holders")]
    [SerializeField] private GameObject AnimationHolder;

    [Header("Animators")]
    [SerializeField] private Animator spiderAnim;

    [Header("Prefabs")]
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private GameObject bubblesPrefab;

    [Header("Boat Buttons")]
    public BoatButton greenButton;
    public BoatButton blueButton;
    public BoatButton micButton;
    public BoatButton soundButton;
    public BoatButton escapeButton;


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
    }

    public void BlueButtonPressed()
    {
        if (DockedBoatWheelController.instance.isOn)
        {
            // Stomp Engine Rumble FX
            AudioManager.instance.StopFX("Engine_Rumble");
            DockedBoatWheelController.instance.isOn = false;
        }
        else
        {
            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EngineStart, 0.5f);
            AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientEngineRumble, 0.2f, "Engine_Rumble");
            DockedBoatWheelController.instance.isOn = true;
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
}
