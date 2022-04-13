using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

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
    public MicrophoneIndicator micIndicator;
    public float audioInputThreshold;
    private bool waitingForMicButton = false;

    private int boatGameEvent = 0;
    private List<AssetReference> audiosToRepeat;
    [HideInInspector] public bool repeatAudio = false;
    private bool playingRepeatAudios = false;
    private float repeatTimer = 0f;
    private float repeatDuration;
    private int repeatTimes = 0;

    private bool waitForBlueButton = true;
    private bool waitForGreenButton = true;
    private bool waitingForMicInput = false;


    private Coroutine repeatAudioRoutine;
    private Coroutine boatGameRoutine;

    void Awake()
    {
        if (instance == null)
            instance = this;

        GameManager.instance.SceneInit();

        // stop music
        AudioManager.instance.StopMusic();

        // play ambient sounds
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientOceanLoop, 0.1f);
        AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientSeagullsLoop, 0.1f);

        // initialize list
        audiosToRepeat = new List<AssetReference>();
    }

    void Start()
    {
        boatGameRoutine = StartCoroutine(ContinueBoatGame());
    }

    void Update()
    {
        // dev stuff for fx audio testing
        if (GameManager.instance.devModeActivated)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    StopAllCoroutines();
                    TalkieManager.instance.StopTalkieSystem();
                    StartCoroutine(WinBoatGame());
                }
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
                    repeatAudioRoutine = StartCoroutine(PlayRepeatAudios());

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
        else
        {
            if (repeatAudioRoutine != null)
            {
                StopCoroutine(repeatAudioRoutine);
                repeatAudioRoutine = null;
                repeatTimer = 0f;
                playingRepeatAudios = false;
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
                audiosToRepeat.Clear();
                AudioManager.instance.StopTalk();

                // play sound effect
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.FoundIslandSparkle, 1f);

                StopCoroutine(boatGameRoutine);
                boatGameRoutine = StartCoroutine(ContinueBoatGame());
            }
        }

        // continue boat game when microphone input is detected
        if (boatGameEvent == 4 && waitingForMicInput)
        {
            // get mic input and determine if input is loud enough
            float volumeLevel = MicInput.MicLoudness * 200;
            if (volumeLevel >= audioInputThreshold)
            {
                micIndicator.AudioInputDetected();
                boatGameEvent++;

                // stop repeating audio
                repeatAudio = false;
                audiosToRepeat.Clear();
                AudioManager.instance.StopTalk();

                StopCoroutine(boatGameRoutine);
                boatGameRoutine = StartCoroutine(ContinueBoatGame());
            }
        }
    }

    private IEnumerator MicrophoneNotWorkingRoutine()
    {
        // stop repeating audio
        repeatAudio = false;
        audiosToRepeat.Clear();
        AudioManager.instance.StopTalk();

        // show no input on microphone
        micIndicator.NoInputDetected();

        // red voiceover 13
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[13]));
        yield return cd.coroutine;
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[13]);
        yield return new WaitForSeconds(cd.GetResult() + 0.5f);

        // red voiceover 14
        CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[14]));
        yield return cd0.coroutine;
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[14]);
        yield return new WaitForSeconds(cd0.GetResult() + 0.5f);

        // red voiceover 15
        CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[15]));
        yield return cd1.coroutine;
        AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[15]);
        yield return new WaitForSeconds(cd1.GetResult() + 0.5f);

        // repeat
        audiosToRepeat = new List<AssetReference>();
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[13]);
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[14]);
        audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[15]);
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
            {
                // reset audio repeat timer
                playingRepeatAudios = false;
                repeatTimer = 0f;
                AudioManager.instance.StopTalk();
                yield break;
            }

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(audio));
            yield return cd.coroutine;

            AudioManager.instance.PlayTalk(audio);
            yield return new WaitForSeconds(cd.GetResult() + 0.5f);
        }

        // reset audio repeat timer
        playingRepeatAudios = false;
        repeatTimer = 0f;
    }

    private IEnumerator ContinueBoatGame()
    {
        GameManager.instance.SendLog(this, "current boat event: " + boatGameEvent);

        switch (boatGameEvent)
        {
            case 0:
                // turn off controls
                BoatWheelController.instance.isOn = false;
                BoatThrottleController.instance.isOn = false;
                IslandCutoutController.instance.isOn = false;

                if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.InitBoatGame)
                {
                    // play talkie and wait for it to finish
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("BoatGame_1_p1"));
                    while (TalkieManager.instance.talkiePlaying)
                    {
                        yield return null;
                    }
                    yield return new WaitForSeconds(1f);
                }

                // red voiceover 0
                CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[0]));
                yield return cd.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[0]);
                yield return new WaitForSeconds(cd.GetResult() + 0.5f);

                // turn on blue button glow + wiggle
                ImageGlowController.instance.SetImageGlow(blueButton.GetComponent<Image>(), true, GlowValue.glow_1_025);
                blueButton.wiggleController.StartWiggle();
                waitForBlueButton = false;

                // red voiceover 1
                CoroutineWithData<float> cd0 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[1]));
                yield return cd0.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[1]);
                yield return new WaitForSeconds(cd0.GetResult() + 0.5f);

                audiosToRepeat = new List<AssetReference>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[1]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;
                break;

            case 1:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // red voiceover 2
                CoroutineWithData<float> cd1 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[2]));
                yield return cd1.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[2]);
                yield return new WaitForSeconds(cd1.GetResult() + 0.5f);

                // turn on wheel
                BoatWheelController.instance.isOn = true;

                // red voiceover 3
                CoroutineWithData<float> cd2 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[3]));
                yield return cd2.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[3]);
                yield return new WaitForSeconds(cd2.GetResult() + 0.5f);

                // red voiceover 4
                CoroutineWithData<float> cd3 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[4]));
                yield return cd3.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[4]);
                yield return new WaitForSeconds(cd3.GetResult() + 0.5f);

                audiosToRepeat = new List<AssetReference>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[2]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[3]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[4]);
                repeatTimer = 0f;
                repeatDuration = 20f;
                repeatAudio = true;

                // wiggle island outline
                IslandCutoutController.instance.outlineWiggleController.StartWiggle();
                break;

            case 2:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // turn on island cutout
                IslandCutoutController.instance.isOn = true;
                IslandCutoutController.instance.cutoutWiggleController.StartWiggle();

                // red voiceover 5
                CoroutineWithData<float> cd4 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[5]));
                yield return cd4.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[5]);
                yield return new WaitForSeconds(cd4.GetResult() + 0.5f);

                // red voiceover 6
                CoroutineWithData<float> cd5 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[6]));
                yield return cd5.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[6]);
                yield return new WaitForSeconds(cd5.GetResult() + 0.5f);

                // red voiceover 7
                CoroutineWithData<float> cd6 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[7]));
                yield return cd6.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[7]);
                yield return new WaitForSeconds(cd6.GetResult() + 0.5f);

                audiosToRepeat = new List<AssetReference>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[5]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[6]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[7]);
                repeatTimer = 0f;
                repeatDuration = 20f;
                repeatAudio = true;

                break;

            case 3:
                // wait for sounds to finish
                yield return new WaitForSeconds(2f);

                // turn on throttle glow + wiggle
                ImageGlowController.instance.SetImageGlow(BoatThrottleController.instance.GetComponent<Image>(), true, GlowValue.glow_1_025);
                BoatThrottleController.instance.wiggleController.StartWiggle();

                // enable throtle control
                BoatThrottleController.instance.isOn = true;

                // red voiceover 8
                CoroutineWithData<float> cd7 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[8]));
                yield return cd7.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[8]);
                yield return new WaitForSeconds(cd7.GetResult() + 0.5f);

                // red voiceover 9
                CoroutineWithData<float> cd8 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[9]));
                yield return cd8.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[9]);
                yield return new WaitForSeconds(cd8.GetResult() + 0.5f);

                audiosToRepeat = new List<AssetReference>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[9]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;

                break;

            case 4:
                // red voiceover 10
                CoroutineWithData<float> cd9 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[10]));
                yield return cd9.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[10]);
                yield return new WaitForSeconds(cd9.GetResult() + 0.5f);

                // red voiceover 11
                CoroutineWithData<float> cd10 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[11]));
                yield return cd10.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[11]);
                yield return new WaitForSeconds(cd10.GetResult() + 0.5f);

                // red voiceover 12
                CoroutineWithData<float> cd11 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[12]));
                yield return cd11.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[12]);
                yield return new WaitForSeconds(cd11.GetResult() + 0.5f);

                audiosToRepeat = new List<AssetReference>();
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[10]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[11]);
                audiosToRepeat.Add(AudioDatabase.instance.boat_game_audio[12]);
                repeatTimer = 0f;
                repeatDuration = 5f;
                repeatAudio = true;

                // turn on audio indicator
                micIndicator.ShowIndicator();

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
                micIndicator.HideIndicator();

                // red voiceover 26
                CoroutineWithData<float> cd12 = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(AudioDatabase.instance.boat_game_audio[16]));
                yield return cd12.coroutine;
                AudioManager.instance.PlayTalk(AudioDatabase.instance.boat_game_audio[16]);
                yield return new WaitForSeconds(cd12.GetResult() + 0.5f);

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
            yield return new WaitForSeconds(2f);
            // exit boat game
            GameManager.instance.LoadScene("ScrollMap", true, 3f, true);
        }
        else
        {
            if (StudentInfoSystem.GetCurrentProfile().currBoatEncounter == BoatEncounter.FirstTime)
            {
                StudentInfoSystem.GetCurrentProfile().currBoatEncounter = BoatEncounter.SecondTime;
                StudentInfoSystem.SaveStudentPlayerData();
            }

            GameManager.instance.finishedBoatGame = true;
            GameManager.instance.LoadScene("DockedBoatGame", true, 3f, true);
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
            case BoatButtonID.Sound:
                AudioButtonPressed();
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
            audiosToRepeat.Clear();
            AudioManager.instance.StopTalk();

            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.BoatHorn, 0.5f);

            StopCoroutine(boatGameRoutine);
            boatGameRoutine = StartCoroutine(ContinueBoatGame());
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
            audiosToRepeat.Clear();
            AudioManager.instance.StopTalk();

            // play sound effects
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.EngineStart, 0.5f);
            AudioManager.instance.PlayFX_loop(AudioDatabase.instance.AmbientEngineRumble, 0.2f);

            StopCoroutine(boatGameRoutine);
            boatGameRoutine = StartCoroutine(ContinueBoatGame());
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
            audiosToRepeat.Clear();
            AudioManager.instance.StopTalk();

            StopCoroutine(boatGameRoutine);
            boatGameRoutine = StartCoroutine(ContinueBoatGame());
        }
    }

    public void AudioButtonPressed()
    {
        SettingsManager.instance.ToggleSettingsWindow();
    }

    public void IslandCentered()
    {
        boatGameEvent++;

        // stop repeating audio
        repeatAudio = false;
        audiosToRepeat.Clear();
        AudioManager.instance.StopTalk();

        // turn off island cutout
        IslandCutoutController.instance.isOn = false;

        // play happy sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.RightChoice, 0.5f);

        StopCoroutine(boatGameRoutine);
        boatGameRoutine = StartCoroutine(ContinueBoatGame());
    }

    public void ArrivedAtIsland()
    {
        boatGameEvent++;

        // stop boat panel shake
        BoatWheelController.instance.ToggleBoatPannelShake(false);
        // move throttle down
        BoatThrottleController.instance.StopThrottle();

        // stop repeating audio
        repeatAudio = false;
        audiosToRepeat.Clear();
        AudioManager.instance.StopTalk();

        StopCoroutine(boatGameRoutine);
        boatGameRoutine = StartCoroutine(ContinueBoatGame());
    }
}
