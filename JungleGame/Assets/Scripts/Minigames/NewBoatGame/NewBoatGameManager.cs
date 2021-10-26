using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBoatGameManager : MonoBehaviour
{
    public static NewBoatGameManager instance;

    [Header("Boat Buttons")]
    public BoatButton greenButton;
    public BoatButton blueButton;
    public BoatButton micButton;
    public BoatButton soundButton;
    public BoatButton escapeButton;

    [Header("Island Outline")]
    public Transform islandOutline;

    [Header("Mic Input")]
    public float audioInputThreshold;
    public SpriteRenderer audioInputIndicator;
    private bool waitingForMicButton = false;

    private int boatGameEvent = 0;
    private List<AudioClip> audiosToRepeat;
    [HideInInspector] public bool repeatAudio = false;
    private bool playingRepeatAudios = false;
    private float repeatTimer = 0f;
    private float repeatDuration;
    private int repeatTimes = 0;

    private bool waitForBlueButton = true;
    private bool waitForGreenButton = true;
    private bool waitingForMicInput = false;

    void Awake()
    {
        if (instance == null)
            instance = this;

        GameManager.instance.SceneInit();

        // stop music
        AudioManager.instance.StopMusic();

        // remove audio input indicator
        audioInputIndicator.color = new Color(1f, 1f, 1f, 0f);

        // play ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientOceanLoop, 0.45f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientSeagullsLoop, 0.1f);

        StartCoroutine(ContinueBoatGame());
    }

    void Update()
    {
        // dev stuff for fx audio testing
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                TalkieManager.instance.StopTalkieSystem();
                StartCoroutine(WinBoatGame());
            }
        }

        if (repeatAudio)
        {
            repeatTimer += Time.deltaTime;
            if (repeatTimer > repeatDuration)
            {
                if (!playingRepeatAudios)
                {
                    playingRepeatAudios = true;
                    StartCoroutine(PlayRepeatAudios());

                    // only repeat 3 times on event 4
                    if (boatGameEvent == 4)
                    {
                        repeatTimes++;
                        if (repeatTimes == 3)
                        {
                            StartCoroutine(MicrophoneNotWorkingRoutine());
                        }
                    }
                }
            }
        }

        // continue boat game when found island cutout
        if (boatGameEvent == 1)
        {
            float islandPos = islandOutline.transform.position.x;
            if (islandPos < 5f && islandPos > -5f)
            {
                boatGameEvent++;

                // stop wiggle
                IslandCutoutController.instance.outlineWiggleController.StopWiggle();

                // stop repeating audio
                repeatAudio = false;
                AudioManager.instance.StopTalk();

                // play sound effect
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.FoundIslandSparkle, 1f);

                StartCoroutine(ContinueBoatGame());
            } 
        }

        // continue boat game when microphone input is detected
        if (boatGameEvent == 4 && waitingForMicInput)
        {
            // get mic input and determine if input is loud enough
            float volumeLevel = MicInput.MicLoudness * 200;
            //print ("volume level: " + volumeLevel);
            if (volumeLevel >= audioInputThreshold)
            {
                boatGameEvent++;

                // stop repeating audio
                repeatAudio = false;
                AudioManager.instance.StopTalk();

                StartCoroutine(ContinueBoatGame());
            }
        }
    }

    private IEnumerator MicrophoneNotWorkingRoutine()
    {
        // stop repeating audio
        repeatAudio = false;
        AudioManager.instance.StopTalk();

        // red voiceover 17
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[16]);
        yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[16].length + 0.5f);

        // red voiceover 18
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[17]);
        yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[17].length + 0.5f);

        // red voiceover 19
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[18]);
        yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[18].length + 0.5f);

        // repeat
        audiosToRepeat = new List<AudioClip>();
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[16]);
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[17]);
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[18]);
        repeatTimer = 0f;
        repeatDuration = 10f;
        repeatAudio = true;

        // turn on mic button glow + wiggle
        ImageGlowController.instance.SetImageGlow(micButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
        micButton.wiggleController.StartWiggle();

        waitingForMicButton = true;
    }

    private IEnumerator PlayRepeatAudios()
    {
        foreach (var audio in audiosToRepeat)
        {
            if (!repeatAudio)
                break;
            AudioManager.instance.PlayTalk(audio);
            yield return new WaitForSeconds(audio.length + 0.5f);
        }

        // reset audio repeat timer
        playingRepeatAudios = false;
        repeatTimer = 0f;
    }

    private IEnumerator ContinueBoatGame()
    {
        print ("boat game event: " + boatGameEvent);

        switch (boatGameEvent)
        {
            case 0:
                // turn off controls
                BoatWheelController.instance.isOn = false;
                BoatThrottleController.instance.isOn = false;
                IslandCutoutController.instance.isOn = false;
                yield return new WaitForSeconds(3f);

                // play talkie and wait for it to finish
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.boatGame);
                while (TalkieManager.instance.talkiePlaying)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(1f);

                // red voiceover 4
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[3]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[3].length + 0.5f);

                // red voiceover 5
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[4]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[4].length + 0.5f);

                audiosToRepeat = new List<AudioClip>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[4]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;

                // turn on blue button glow + wiggle
                ImageGlowController.instance.SetImageGlow(blueButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
                blueButton.wiggleController.StartWiggle();
                waitForBlueButton = false;
                break;

            case 1:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // red voiceover 6
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[5]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[5].length + 0.5f);

                // red voiceover 7
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[6]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[6].length + 0.5f);

                // red voiceover 8
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[7]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[7].length + 0.5f);

                audiosToRepeat = new List<AudioClip>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[5]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[6]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[7]);
                repeatTimer = 0f;
                repeatDuration = 20f;
                repeatAudio = true;

                // turn on wheel
                BoatWheelController.instance.isOn = true;

                // wiggle island outline
                IslandCutoutController.instance.outlineWiggleController.StartWiggle();
                break;

            case 2:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // red voiceover 9
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[8]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[8].length + 0.5f);

                // red voiceover 10
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[9]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[9].length + 0.5f);

                // red voiceover 11
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[10]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[10].length + 0.5f);

                audiosToRepeat = new List<AudioClip>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[8]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[9]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[10]);
                repeatTimer = 0f;
                repeatDuration = 20f;
                repeatAudio = true;

                // turn on island cutout
                IslandCutoutController.instance.isOn = true;
                IslandCutoutController.instance.cutoutWiggleController.StartWiggle();
                break;

            case 3:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // red voiceover 12
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[11]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[11].length + 0.5f);

                // red voiceover 13
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[12]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[12].length + 0.5f);

                audiosToRepeat = new List<AudioClip>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[12]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;

                // turn on throttle glow + wiggle
                ImageGlowController.instance.SetImageGlow(BoatThrottleController.instance.GetComponent<Image>(), true, GlowValue.glow_1_025);
                BoatThrottleController.instance.wiggleController.StartWiggle();

                // enable throtle control
                BoatThrottleController.instance.isOn = true;
                break;

            case 4:
                // red voiceover 14
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[13]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[13].length + 0.5f);

                // red voiceover 15
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[14]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[14].length + 0.5f);

                // red voiceover 16
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[15]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[15].length + 0.5f);

                audiosToRepeat = new List<AudioClip>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[13]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[14]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[15]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;

                // turn on audio indicator
                audioInputIndicator.GetComponent<LerpableObject>().LerpSpriteAlpha(audioInputIndicator, 1f, 0.25f);
                audioInputIndicator.GetComponent<WiggleController>().StartWiggle();

                waitingForMicInput = true;
                break;

            case 5:
                // short break between mic input and next event
                yield return new WaitForSeconds(2f);
                
                // make fx sound
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 1f);
                yield return new WaitForSeconds(1f);

                // turn off mic button glow
                ImageGlowController.instance.SetImageGlow(micButton.GetComponent<Image>(), false);
                micButton.wiggleController.StopWiggle();

                // turn off audio indicator
                audioInputIndicator.GetComponent<LerpableObject>().LerpSpriteAlpha(audioInputIndicator, 0f, 0.25f);
                audioInputIndicator.GetComponent<WiggleController>().StopWiggle();

                // red voiceover 20
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[19]);
                yield return new WaitForSeconds(AudioDatabase.instance.boat_game_audio[19].length + 0.5f);

                // turn on green button glow + wiggle
                ImageGlowController.instance.SetImageGlow(greenButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
                greenButton.wiggleController.StartWiggle();
                waitForGreenButton = false;
                break;

            case 6:
                // short break between mic input and next event
                yield return new WaitForSeconds(3f);

                StartCoroutine(WinBoatGame());
                break;
        }
        yield return null;
    }

    // skips the boat game for dev purposes
    private IEnumerator WinBoatGame()
    {
        // play blip sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WinTune, 1f);

        // save to SIS
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.InitBoatGame)
        {
            StudentInfoSystem.AdvanceStoryBeat();
            StudentInfoSystem.SaveStudentPlayerData();
        }
        yield return new WaitForSeconds(2f);

        // exit boat game
        GameManager.instance.LoadScene("ScrollMap", true, 3f);
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
        }
    }

    public void ThrottlePressed()
    {
        // turn off throttle glow + wiggle
        ImageGlowController.instance.SetImageGlow(BoatThrottleController.instance.GetComponent<Image>(), false);
        BoatThrottleController.instance.wiggleController.StopWiggle();
    }

    public void GreenButtonPressed()
    {
        if (boatGameEvent == 5 && !waitForGreenButton)
        {
            boatGameEvent++;

            // turn off blue button glow + stop wiggle
            ImageGlowController.instance.SetImageGlow(greenButton.GetComponent<Image>(), false);
            greenButton.wiggleController.StopWiggle();

            // stop repeating audio
            repeatAudio = false;
            AudioManager.instance.StopTalk();

            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BoatHorn, 0.5f);

            StartCoroutine(ContinueBoatGame());
        }
    }

    public void BlueButtonPressed()
    {   
        if (boatGameEvent == 0 && !waitForBlueButton)
        {
            boatGameEvent++;

            // turn off blue button glow + stop wiggle
            ImageGlowController.instance.SetImageGlow(blueButton.GetComponent<Image>(), false);
            blueButton.wiggleController.StopWiggle();

            // stop repeating audio
            repeatAudio = false;
            AudioManager.instance.StopTalk();

            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EngineStart, 0.5f);
            AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientEngineRumble, 0.2f);

            StartCoroutine(ContinueBoatGame());
        }
    }

    public void MicrophoneButtonPressed()
    {
        if (boatGameEvent == 4 && waitingForMicButton)
        {
            boatGameEvent++;

            // turn off mic button glow + stop wiggle
            ImageGlowController.instance.SetImageGlow(micButton.GetComponent<Image>(), false);
            micButton.wiggleController.StopWiggle();

            // stop repeating audio
            repeatAudio = false;
            AudioManager.instance.StopTalk();

            StartCoroutine(ContinueBoatGame());
        }
    }   

    public void IslandCentered()
    {
        boatGameEvent++;

        // stop repeating audio
        repeatAudio = false;
        AudioManager.instance.StopTalk();

        // turn off island cutout
        IslandCutoutController.instance.isOn = false;

        // play happy sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

        StartCoroutine(ContinueBoatGame());
    }

    public void ArrivedAtIsland()
    {
        boatGameEvent++;

        // stop boat panel shake
        BoatWheelController.instance.ToggleBoatPannelShake(false);
        // move throttle down
        BoatThrottleController.instance.StopThrottle();

        StartCoroutine(ContinueBoatGame());
    }
}
