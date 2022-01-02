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
        background.raycastTarget = false;
        background.GetComponent<LerpableObject>().LerpImageAlpha(background, 0f, 0.5f);

        // show UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        SettingsManager.instance.ToggleWagonButtonActive(true);
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
            GameType RRgame = AISystem.DetermineRoyalRumbleGame();
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

            // play default talkie for now
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.defaultRRTalkie);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // do not go to game if talkie manager says not to
            if (TalkieManager.instance.doNotContinueToGame)
            {
                TalkieManager.instance.doNotContinueToGame = false;

                // remove RR stuff
                wheelBreakAnimator.Play("WheelNone");
                trumpetPlayAnimator.Play("TrumpetIn");
                CloseWheel();
                rrGradient.LerpImageAlpha(rrGradient.GetComponent<Image>(), 0f, 2f);
                rrImage.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);

                yield break;
            }

            GameManager.instance.playingRoyalRumbleGame = true;
            GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(RRgame), true, 0.5f, true);
            yield break;
        }

        GameManager.instance.prevGameTypePlayed = game;
        GameManager.instance.mapID = currentIdentifier;
        GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(game), true, 0.5f, true);
    }
}
