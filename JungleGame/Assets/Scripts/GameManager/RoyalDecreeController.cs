using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalDecreeController : MonoBehaviour
{
    public static RoyalDecreeController instance;

    [Header("Window Pieces")]
    public LerpableObject window;
    public LerpableObject scroll;
    public List<ChallengeGameRibbon> ribbons;

    [Header("Positions")]
    public float scrollHiddenY;
    public float scrollShownY;

    private bool isOpen = false;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // hide UI
        scroll.transform.localPosition = new Vector3(scroll.transform.localPosition.x, scrollHiddenY, 0f);
        window.transform.localScale = new Vector3(1f, 0f, 1f);
    }

    public void ToggleWindow(int triadIndex)
    {
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
        // get challenge game triads
        ChallengeGameTriad triad = GameManager.instance.challengeGameTriads[index];
        List<GameType> gameTypes = new List<GameType>();

        gameTypes.Add(triad.juliusGame.gameType);
        gameTypes.Add(triad.marcusGame.gameType);
        gameTypes.Add(triad.brutusGame.gameType);

        scroll.LerpPosition(new Vector2(scroll.transform.localPosition.x, scrollShownY), 0.25f, true);
        yield return new WaitForSeconds(0.25f);

        window.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.25f, 0.1f);
        yield return new WaitForSeconds(0.5f);
        
        int count = 0;
        foreach (var ribbon in ribbons)
        {
            ribbon.OpenRibbon(gameTypes[count]);
            yield return new WaitForSeconds(0.1f);
            count++;
        }
    }

    private IEnumerator CloseWindowRoutine()
    {
        foreach (var ribbon in ribbons)
        {
            ribbon.CloseRibbon();
            yield return new WaitForSeconds(0.1f);
        }

        window.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.5f);

        scroll.LerpPosition(new Vector2(scroll.transform.localPosition.x, scrollHiddenY), 0.25f, true);
        yield return new WaitForSeconds(0.25f);
    }
}
