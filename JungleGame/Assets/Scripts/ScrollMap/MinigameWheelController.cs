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
    private List<GameType> minigameOptions; 
    private MapIconIdentfier currentIdentifier;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // create minigame options list
        minigameOptions = new List<GameType>();
        minigameOptions.Add(GameType.FroggerGame);
        minigameOptions.Add(GameType.TurntablesGame);
        minigameOptions.Add(GameType.RummageGame);
        //minigameOptions.Add(GameType.PirateGame);
        minigameOptions.Add(GameType.SpiderwebGame);
        //minigameOptions.Add(GameType.SeashellGame);

        // remove background
        background.raycastTarget = false;
        background.GetComponent<LerpableObject>().LerpImageAlpha(background, 0f, 0f);

        // hide back button
        backButton.transform.localScale = new Vector3(0f, 0f, 1f);

        // wheel button disabled
        wheelButton.interactable = false;
        wheelButton.GetComponent<Image>().raycastTarget = false;
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

        // get random minigame
        int index = Random.Range(0, minigameOptions.Count);
        string gameScene = "FroggerGame";

        switch (minigameOptions[index])
        {
            case GameType.FroggerGame:
                animator.SetBool("finishFrogger", true);
                gameScene = "FroggerGame";
                break;
            case GameType.TurntablesGame:
                animator.SetBool("finishTurntables", true);
                gameScene = "TurntablesGame";
                break;
            case GameType.RummageGame:
                animator.SetBool("finishRummage", true);
                gameScene = "RummageGame";
                break;
            case GameType.SpiderwebGame:
                animator.SetBool("finishSpiderweb", true);
                gameScene = "NewSpiderwebGame";
                break;
            case GameType.PirateGame:
                animator.SetBool("finishPirate", true);
                gameScene = "NewPirateGame";
                break;
            case GameType.SeashellGame:
                animator.SetBool("finishSeashells", true);
                gameScene = "SeaShellGame";
                break;
        }

        yield return new WaitForSeconds(3f);
        GameManager.instance.mapID = currentIdentifier;
        GameManager.instance.LoadScene(gameScene, true, 0.5f, true);
    }
}
