using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameWheelController : MonoBehaviour
{
    public static MinigameWheelController instance;

    public Animator animator;
    public Image background;
    public LerpableObject backButton;
    public Button wheelButton;

    private bool isSpinning = false;
    private MapIconIdentfier currentIdentifier;

    [Header("Royal Rumble")]
    public LerpableObject rrGradient;
    public LerpableObject rrImage;

    [Header("Animators")]
    public Animator wheelBreakAnimator;
    public Animator guardDestroyAnimator;
    public Animator trumpetPlayAnimator;
    public Animator tigerDestroyAnimator;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // remove background
        background.raycastTarget = false;
        background.GetComponent<LerpableObject>().LerpImageAlpha(background, 0f, 0f);

        // hide back button
        backButton.transform.localScale = new Vector3(0f, 0f, 1f);

        // wheel button disabled
        wheelButton.interactable = false;
        wheelButton.GetComponent<Image>().raycastTarget = false;

        // hide royal rumble stuff
        rrGradient.SetImageAlpha(rrGradient.GetComponent<Image>(), 0f);
        rrImage.transform.localScale = new Vector3(0f, 0f, 1f);
    }

    

    public void RevealWheel(MapIconIdentfier identfier)
    {
        currentIdentifier = identfier;
        StartCoroutine(RevealWheelRoutine());
    }

    private IEnumerator RevealWheelRoutine()
    {
        // remove UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(false);
        SettingsManager.instance.ToggleWagonButtonActive(false);

        // add background
        background.raycastTarget = true;
        background.GetComponent<LerpableObject>().LerpImageAlpha(background, 0.5f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        // show back button
        backButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);

        // wheel button interactable
        isSpinning = false;
        wheelButton.interactable = true;
        wheelButton.GetComponent<Image>().raycastTarget = true;

        // remove map icon quip raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("map_icon_quip");

        animator.Play("wheelReveal");
    }

    public void CloseWheel()
    {
        StartCoroutine(CloseWheelRoutine());
    }

    private IEnumerator CloseWheelRoutine()
    {
        // wheel button interactable
        wheelButton.interactable = false;
        wheelButton.GetComponent<Image>().raycastTarget = false;

        // remove back button
        backButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);

        animator.Play("wheelClose");

        yield return new WaitForSeconds(0.5f);

        // remove background
        background.GetComponent<LerpableObject>().LerpImageAlpha(background, 0f, 0.5f);

        // show UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        SettingsManager.instance.ToggleWagonButtonActive(true);

        yield return new WaitForSeconds(1f);

        background.raycastTarget = false;
    }

    public void OnWheelButtonPressed()
    {
        if (!isSpinning)
        {
            wheelButton.interactable = false;
            StartCoroutine(SpinWheel());
        }
        else
        {
            StartCoroutine(StopWheel());
        }
    }

    private IEnumerator SpinWheel()
    {
        animator.SetBool("finishFrogger", false);
        animator.SetBool("finishTurntables", false);
        animator.SetBool("finishSpiderweb", false);
        animator.SetBool("finishRummage", false);
        animator.SetBool("finishPirate", false);
        animator.SetBool("finishSeashells", false);

        // start spinning wheel
        animator.Play("wheelClick");

        yield return new WaitForSeconds(1f);
        isSpinning = true;
        wheelButton.interactable = true;
    }

    private IEnumerator StopWheel()
    {
        // stop wheel button disabled
        wheelButton.interactable = false;

        // remove beck button
        backButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);

        // determine game type
        GameType game = AISystem.DetermineMinigame(StudentInfoSystem.GetCurrentProfile());

        switch (game)
        {
            case GameType.FroggerGame:
                animator.SetBool("finishFrogger", true);
                break;
            case GameType.TurntablesGame:
                animator.SetBool("finishTurntables", true);
                break;
            case GameType.RummageGame:
                animator.SetBool("finishRummage", true);
                break;
            case GameType.SpiderwebGame:
                animator.SetBool("finishSpiderweb", true);
                break;
            case GameType.PirateGame:
                animator.SetBool("finishPirate", true);
                break;
            case GameType.SeashellGame:
                animator.SetBool("finishSeashells", true);
                break;
        }

        yield return new WaitForSeconds(3f);

        // determine royal rumble
        bool startRR = AISystem.DetermineRoyalRumble(StudentInfoSystem.GetCurrentProfile());
        print ("royal rumble?: " + startRR);

        if (startRR)
        {
            // save royal rumble to SIS
            GameType RRgame = AISystem.DetermineRoyalRumbleGame(StudentInfoSystem.GetCurrentProfile());
            StudentInfoSystem.GetCurrentProfile().royalRumbleGame = RRgame;
            StudentInfoSystem.GetCurrentProfile().royalRumbleActive = true;
            StudentInfoSystem.GetCurrentProfile().royalRumbleID = currentIdentifier;
            StudentInfoSystem.SaveStudentPlayerData();

            // banner on map icon
            MapDataLoader.instance.SetRoyalRumbleBanner();

            // wheel break animation TODO -> change to guards break after chapter 3 or 4
            tigerDestroyAnimator.Play("TigerDestroy");
            yield return new WaitForSeconds(0.5f);

            wheelBreakAnimator.Play("WheelBreak");
            yield return new WaitForSeconds(0.5f);

            rrGradient.LerpImageAlpha(rrGradient.GetComponent<Image>(), 1f, 2f);

            trumpetPlayAnimator.Play("TrumpetPlay");
            yield return new WaitForSeconds(1f);

            rrImage.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);

            yield return new WaitForSeconds(1f);

            // get current chapter
            Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

            // play correct RR talkie based on current chapter
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                    // play julius RR intro
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRJuliusIntro_1_p1"));
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;
                    break;

                case Chapter.chapter_4:
                case Chapter.chapter_5:
                    // play guards RR intro
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_1_p1"));
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;
                    // first guards RR?
                    if (StudentInfoSystem.GetCurrentProfile().firstGuradsRoyalRumble)
                    {
                        // play guards RR intro 2 p1
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_2_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;

                        // save to SIS
                        StudentInfoSystem.GetCurrentProfile().firstGuradsRoyalRumble = false;
                        StudentInfoSystem.SaveStudentPlayerData();
                    }
                    else
                    {
                        // play guards RR intro 2 p2
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_2_p2"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                    }
                    break;

            }

            if (TalkieManager.instance.yesNoChoices.Count == 1)
            {
                // if player chooses yes
                if (TalkieManager.instance.yesNoChoices[0])
                {
                    TalkieManager.instance.yesNoChoices.Clear();
                }
                else // if the player chooses no
                {
                    TalkieManager.instance.yesNoChoices.Clear();

                    // remove RR stuff
                    wheelBreakAnimator.Play("WheelNone");
                    trumpetPlayAnimator.Play("TrumpetIn");
                    CloseWheel();
                    rrGradient.LerpImageAlpha(rrGradient.GetComponent<Image>(), 0f, 2f);
                    rrImage.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);

                    yield break;

                }
            }
            else
            {
                TalkieManager.instance.yesNoChoices.Clear();
                Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
            }

            GameManager.instance.playingRoyalRumbleGame = true;
            GameManager.instance.mapID = currentIdentifier;
            GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(RRgame), true, 0.5f, true);
            yield break;
        }

        GameManager.instance.prevGameTypePlayed = game;
        GameManager.instance.mapID = currentIdentifier;
        GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(game), true, 0.5f, true);
    }
}
