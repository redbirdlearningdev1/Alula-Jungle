using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoyalDecreeController : MonoBehaviour
{
    public static RoyalDecreeController instance;

    [Header("Window Pieces")]
    public LerpableObject backButton;
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

    [HideInInspector] public bool isOpen = false;
    private bool waitToOpen = false;

    private List<GameType> currTriad;
    private GameType currGameType;
    private MapLocation mapLocation;

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

        // hide back button
        backButton.transform.localScale = new Vector3(0f, 0f, 1f);
    }

    public void OnBackButtonPressed()
    {
        if (waitToOpen)
            return;
        
        waitToOpen = true;
        isOpen = !isOpen;

        StartCoroutine(CloseWindowRoutine(false));
    }

    public void ToggleWindow(MapLocation mapLocation)
    {
        if (waitToOpen)
            return;
        
        waitToOpen = true;

        isOpen = !isOpen;

        this.mapLocation = mapLocation;

        // open window
        if (isOpen)
        {
            StartCoroutine(OpenWindowRoutine());
        }
        // close window
        else 
        {
            StartCoroutine(CloseWindowRoutine(false));
        }
    }

    private IEnumerator OpenWindowRoutine()
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("royal_decree_controller");

        // remove UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(false);
        SettingsManager.instance.ToggleWagonButtonActive(false);

        // disable map nav
        ScrollMapManager.instance.ToggleNavButtons(false);
        
        // get challenge game triads + bring julius on screen
        currTriad = new List<GameType>();

        MapAnimationController.instance.julius.characterAnimator.Play("tigerWalk");
        MapAnimationController.instance.julius.ShowExclamationMark(false);
        
        switch (mapLocation)
        {
            case MapLocation.GorillaVillage:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInGV");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInGV"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GV_challenge3.gameType);
                break;

            case MapLocation.Mudslide:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInMS");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInMS"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MS_challenge3.gameType);
                break;

            case MapLocation.OrcVillage:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInOV");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInOV"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OV_challenge3.gameType);
                break;

            case MapLocation.SpookyForest:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInSF");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInSF"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.SF_challenge3.gameType);
                break;    

            case MapLocation.OrcCamp:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInOC");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInOC"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.OC_challenge3.gameType);
                break;

            case MapLocation.GorillaPoop:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInGP");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInGP"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GP_challenge3.gameType);
                break;

            case MapLocation.WindyCliff:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInWC");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInWC"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.WC_challenge3.gameType);
                break;

            case MapLocation.PirateShip:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInPS");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInPS"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.PS_challenge3.gameType);
                break;

            case MapLocation.MermaidBeach:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInMB");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInMB"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.MB_challenge3.gameType);
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInR");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInR"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.R_challenge3.gameType);
                break;

            case MapLocation.ExitJungle:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInEJ");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInEJ"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.EJ_challenge3.gameType);
                break;

            case MapLocation.GorillaStudy:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInGS");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInGS"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.GS_challenge3.gameType);
                break;

            case MapLocation.Monkeys:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkInM");
                yield return new WaitForSeconds(MapAnimationController.instance.GetAnimationTime(MapAnimationController.instance.julius.mapAnimator, "JuliusWalkInM"));
                MapAnimationController.instance.julius.characterAnimator.Play("aTigerIdle");

                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge1.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge2.gameType);
                currTriad.Add(StudentInfoSystem.GetCurrentProfile().mapData.M_challenge3.gameType);
                break;
        }

        yield return new WaitForSeconds(0.5f);

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("royal_decree_controller");

        // play sign post talkie
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;
        switch (currChapter)
        {
            case Chapter.chapter_0:
            case Chapter.chapter_1:
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaSignPost_1_p1"));
                break;

            case Chapter.chapter_2:
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaSignPost_1_p2"));
                break;

            case Chapter.chapter_3:
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaSignPost_1_p3"));
                break;

            case Chapter.chapter_4:
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaSignPost_1_p4"));
                break;

            case Chapter.chapter_5:
            case Chapter.chapter_final:
                TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("ChaSignPost_1_p5"));
                break;
        }

        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        if (TalkieManager.instance.yesNoChoices.Count == 1)
        {
            // if player chooses yes - do nothing
            if (TalkieManager.instance.yesNoChoices[0])
            {
                TalkieManager.instance.yesNoChoices.Clear();
            }
            else // if player chooses no - exit to scroll map
            {
                TalkieManager.instance.yesNoChoices.Clear();
                StartCoroutine(CloseWindowRoutine(true));
                yield break;
            }
        }
        else
        {
            TalkieManager.instance.yesNoChoices.Clear();
            Debug.LogError("Error: Incorrect number of Yes/No choices for last talkie");
        }

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
            ribbon.OpenRibbon(currTriad[count]);
            yield return new WaitForSeconds(0.1f);
            count++;
        }

        // show back button
        backButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);

        waitToOpen = false;
    }

    private IEnumerator CloseWindowRoutine(bool exitFromTalkie)
    {
        // add raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("exit_challenge_banner");

        // close confirm window if open
        if (confirmWindowUp)
        {
            StartCoroutine(CloseConfirmWindowRoutine());
            yield return new WaitForSeconds(0.5f);
        }

        if (!exitFromTalkie)
        {
            foreach (var ribbon in ribbons)
            {
                ribbon.CloseRibbon();
                yield return new WaitForSeconds(0.1f);
            }

            // remove back button
            backButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);

            // un-dim bg
            dim_bg.LerpImageAlpha(dim_bg.GetComponent<Image>(), 0f, 0.5f);
            dim_bg.GetComponent<Image>().raycastTarget = false;

            window.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
            yield return new WaitForSeconds(0.5f);

            scroll.LerpPosition(new Vector2(scroll.transform.localPosition.x, scrollHiddenY), 0.25f, true);
            yield return new WaitForSeconds(0.25f);
        }

        // remove temp signpost
        TempObjectPlacer.instance.RemoveObject();

        // julius runs off screen
        MapAnimationController.instance.julius.characterAnimator.Play("aTigerTurn");

        yield return new WaitForSeconds(0.25f);

        switch (mapLocation)
        {
            case MapLocation.GorillaVillage:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutGV");
                break;

            case MapLocation.Mudslide:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutMS");
                break;

            case MapLocation.OrcVillage:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutOV");
                break;

            case MapLocation.SpookyForest:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutSF");
                break;    

            case MapLocation.OrcCamp:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutOC");
                break;

            case MapLocation.GorillaPoop:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutGP");
                break;

            case MapLocation.WindyCliff:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutWC");
                break;

            case MapLocation.PirateShip:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutPS");
                break;

            case MapLocation.MermaidBeach:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutMB");
                break;

            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutR");
                break;

            case MapLocation.ExitJungle:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutEJ");
                break;

            case MapLocation.GorillaStudy:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutGS");
                break;

            case MapLocation.Monkeys:
                MapAnimationController.instance.julius.mapAnimator.Play("JuliusWalkOutM");
                break;
        }

        // make julius go off screen
        yield return new WaitForSeconds(2f);
        MapAnimationController.instance.julius.transform.localScale = Vector3.zero;
        MapAnimationController.instance.julius.mapAnimator.Play("JuliusOffScreenPos");
        yield return new WaitForSeconds(0.1f);
        MapAnimationController.instance.julius.transform.localScale = Vector3.one;
        MapAnimationController.instance.PlaceCharactersOnMap(StudentInfoSystem.GetCurrentProfile().currStoryBeat);

        // add UI buttons
        SettingsManager.instance.ToggleMenuButtonActive(true);
        SettingsManager.instance.ToggleWagonButtonActive(true);

        // enble map nav
        ScrollMapManager.instance.ToggleNavButtons(true);

        waitToOpen = false;
        isOpen = false;

        yield return new WaitForSeconds(1f);

        // remove raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("exit_challenge_banner");
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
        GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(currGameType), true);
    }   

    public void OnNoPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        StartCoroutine(CloseConfirmWindowRoutine());
    }
}
