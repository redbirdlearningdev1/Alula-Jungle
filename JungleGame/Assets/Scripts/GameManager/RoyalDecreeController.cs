using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoyalDecreeController : MonoBehaviour
{
    public static RoyalDecreeController instance;

    [Header("Window Pieces")]
    public LerpableObject dim_bg;
    public LerpableObject window;
    public LerpableObject scroll;
    public List<ChallengeGameRibbon> ribbons;

    [Header("Confirm Window")]
    public LerpableObject confirmWindow;
    public LerpableObject dim_bg_2;
    private bool confirmWindowUp = false;

    [Header("Positions")]
    public float scrollHiddenY;
    public float scrollShownY;

    private bool isOpen = false;
    private bool waitToOpen = false;

    private ChallengeGameTriad currTriad;
    private GameType currGameType;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // hide UI
        scroll.transform.localPosition = new Vector3(scroll.transform.localPosition.x, scrollHiddenY, 0f);
        window.transform.localScale = new Vector3(1f, 0f, 1f);

        dim_bg.SetImageAlpha(dim_bg.GetComponent<Image>(), 0f);
        dim_bg.GetComponent<Image>().raycastTarget = false;

        confirmWindow.transform.localScale = new Vector3(1f, 0f, 1f);
        dim_bg_2.SetImageAlpha(dim_bg_2.GetComponent<Image>(), 0f);
        dim_bg_2.GetComponent<Image>().raycastTarget = false;
    }

    public void ToggleWindow(int triadIndex)
    {
        if (waitToOpen)
            return;
        
        waitToOpen = true;

        isOpen = !isOpen;

        // open window
        if (isOpen)
        {
            StartCoroutine(OpenWindowRoutine(triadIndex));
        }
        // close window
        else 
        {
            StartCoroutine(CloseWindowRoutine());
        }
    }

    private IEnumerator OpenWindowRoutine(int index)
    {
        print ("index: " + index);

        // get challenge game triads
        ChallengeGameTriad triad = GameManager.instance.challengeGameTriads[index];

        print ("triad: " + triad);

        List<GameType> challengeGames = new List<GameType>();

        challengeGames.Add(triad.juliusGame1);
        challengeGames.Add(triad.marcusGame2);
        challengeGames.Add(triad.brutusGame3);

        // dim bg
        dim_bg.LerpImageAlpha(dim_bg.GetComponent<Image>(), 0.65f, 0.5f);
        dim_bg.GetComponent<Image>().raycastTarget = true;

        scroll.LerpPosition(new Vector2(scroll.transform.localPosition.x, scrollShownY), 0.25f, true);
        yield return new WaitForSeconds(0.25f);

        window.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.25f, 0.1f);
        yield return new WaitForSeconds(0.5f);
        
        int count = 0;
        foreach (var ribbon in ribbons)
        {
            ribbon.OpenRibbon(challengeGames[count]);
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        waitToOpen = false;
    }

    private IEnumerator CloseWindowRoutine()
    {
        // close confirm window if open
        if (confirmWindowUp)
        {
            StartCoroutine(CloseConfirmWindowRoutine());
            yield return new WaitForSeconds(0.5f);
        }

        foreach (var ribbon in ribbons)
        {
            ribbon.CloseRibbon();
            yield return new WaitForSeconds(0.1f);
        }

        // un-dim bg
        dim_bg.LerpImageAlpha(dim_bg.GetComponent<Image>(), 0f, 0.5f);
        dim_bg.GetComponent<Image>().raycastTarget = false;

        window.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);

        scroll.LerpPosition(new Vector2(scroll.transform.localPosition.x, scrollHiddenY), 0.25f, true);
        yield return new WaitForSeconds(0.25f);

        // remove temp signpost
        TempObjectPlacer.instance.RemoveObject();

        waitToOpen = false;
    }

    /* 
    ################################################
    #   CONFRIM WINDOW
    ################################################
    */

    public void OpenConfirmWindow(GameType gameType)
    {
        if (confirmWindowUp)
            return;
        
        currGameType = gameType;
        confirmWindowUp = true;

        StartCoroutine(OpenConfirmWindowRoutine());
    }

    private IEnumerator OpenConfirmWindowRoutine()
    {
        // dim bg
        dim_bg_2.LerpImageAlpha(dim_bg_2.GetComponent<Image>(), 0.65f, 0.5f);
        dim_bg_2.GetComponent<Image>().raycastTarget = true;

        confirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);
    }

    public void CloseConfirmWindow()
    {
        StartCoroutine(CloseConfirmWindowRoutine());
    }

    private IEnumerator CloseConfirmWindowRoutine()
    {
        // un-dim bg
        dim_bg_2.LerpImageAlpha(dim_bg_2.GetComponent<Image>(), 0f, 0.5f);
        dim_bg_2.GetComponent<Image>().raycastTarget = false;

        confirmWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);

        confirmWindowUp = false;
    }

    public void OnYesPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // go to game scene
        GameManager.instance.LoadScene(currGameType, true);
    }   

    public void OnNoPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        StartCoroutine(CloseConfirmWindowRoutine());
    }
}
