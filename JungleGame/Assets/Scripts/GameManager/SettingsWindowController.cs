using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum SettingsTab
{
    none, audio, game, exit, map
}

public class SettingsWindowController : MonoBehaviour
{
    public static SettingsWindowController instance;

    public LerpableObject audioWindow;
    public LerpableObject gameWindow;
    public LerpableObject exitWindow;
    public LerpableObject mapWindow;

    public LerpableObject audioTab;
    public LerpableObject gameTab;
    public LerpableObject exitTab;
    public LerpableObject mapTab;

    private SettingsTab currentTab;

    private bool isAnimating = false;
    private bool tabsOn = false;

    public Image mapImage;
    public Transform redMarker;

    [Header("Map Sprites")]
    public Sprite map_none;
    public Sprite map_GV;
    public Sprite map_MS;
    public Sprite map_OV;
    public Sprite map_SF;
    public Sprite map_OC;
    public Sprite map_GP;
    public Sprite map_WC;
    public Sprite map_PS;
    public Sprite map_MB;
    public Sprite map_R;
    public Sprite map_EJ;
    public Sprite map_GS;
    public Sprite map_M;
    public Sprite map_P;

    [Header("Map Locations")]
    public Transform redPos_boat;
    public Transform redPos_GV;
    public Transform redPos_MS;
    public Transform redPos_OV;
    public Transform redPos_SF;
    public Transform redPos_OC;
    public Transform redPos_GP;
    public Transform redPos_WC;
    public Transform redPos_PS;
    public Transform redPos_MB;
    public Transform redPos_R;
    public Transform redPos_EJ;
    public Transform redPos_GS;
    public Transform redPos_M;
    public Transform redPos_P;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        currentTab = SettingsTab.none;

        audioWindow.transform.localScale = new Vector3(0f, 0f, 0f);
        gameWindow.transform.localScale = new Vector3(0f, 0f, 1f);
        exitWindow.transform.localScale = new Vector3(0f, 0f, 1f);
        mapWindow.transform.localScale = new Vector3(0f, 0f, 1f);

        audioTab.transform.localScale = new Vector3(0f, 0f, 0f);
        gameTab.transform.localScale = new Vector3(0f, 0f, 0f);
        exitTab.transform.localScale = new Vector3(0f, 0f, 0f);
        mapTab.transform.localScale = new Vector3(0f, 0f, 0f);

        // update map sprite
        UpdateMapSprite();
    }

    public void CloseAllWindows()
    {   
        if (currentTab == SettingsTab.none || isAnimating)
            return;

        StartCoroutine(CloseAllWindowsRoutine());
    }

    private IEnumerator CloseAllWindowsRoutine()
    {
        isAnimating = true;
        
        ToggleTabs(false);

        yield return new WaitForSeconds(0.3f);

        CloseCurrentWindow();

        yield return new WaitForSeconds(0.4f);
        
        currentTab = SettingsTab.none;
        isAnimating = false;
    }

    public void OpenWindow()
    {
        isAnimating = true;
        StartCoroutine(TabPressedRoutine(SettingsTab.map));
    }

    public void AudioTabPressed()
    {
        if (currentTab == SettingsTab.audio || isAnimating)
            return;

        isAnimating = true;
        StartCoroutine(TabPressedRoutine(SettingsTab.audio));
    }

    public void GameTabPressed()
    {
        if (currentTab == SettingsTab.game || isAnimating)
            return;

        isAnimating = true;
        StartCoroutine(TabPressedRoutine(SettingsTab.game));
    }

    public void ExitTabPressed()
    {
        if (currentTab == SettingsTab.exit || isAnimating)
            return;

        isAnimating = true;
        StartCoroutine(TabPressedRoutine(SettingsTab.exit));
    }

    public void MapTabPressed()
    {
        if (currentTab == SettingsTab.map || isAnimating)
            return;

        isAnimating = true;
        StartCoroutine(TabPressedRoutine(SettingsTab.map));
    }


    private IEnumerator TabPressedRoutine(SettingsTab tab)
    {
        // close tabs
        if (tabsOn)
        {
            ToggleTabs(false);
            yield return new WaitForSeconds(0.3f);
        }
            
        // close current window
        if (currentTab != SettingsTab.none)
        {
            CloseCurrentWindow();
            yield return new WaitForSeconds(0.5f);
        }
        
        switch (tab)
        {
            case SettingsTab.audio:
                currentTab = SettingsTab.audio;
                audioWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;

            case SettingsTab.game:
                currentTab = SettingsTab.game;
                gameWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;

            case SettingsTab.exit:
                currentTab = SettingsTab.exit;
                exitWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;

            case SettingsTab.map:
                currentTab = SettingsTab.map;
                mapWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.2f);
                break;
        }

        yield return new WaitForSeconds(0.4f);

        ToggleTabs(true);

        yield return new WaitForSeconds(0.3f);

        isAnimating = false;
    }

    private void CloseCurrentWindow()
    {
        switch (currentTab)
        {
            case SettingsTab.audio:
                audioWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;

            case SettingsTab.game:
                gameWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;

            case SettingsTab.exit:
                exitWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;

            case SettingsTab.map:
                mapWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.2f, 0.2f);
                break;
        }
    }


    public void ToggleTabs(bool opt)
    {
        tabsOn = opt;        

        if (opt)
        {
            StartCoroutine(ShowTabs());
        }
        else
        {
            StartCoroutine(HideTabs());
        }
    }

    private IEnumerator HideTabs()
    {
        mapTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        gameTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
    }

    private IEnumerator ShowTabs()
    {
        mapTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        gameTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
    }

    public void UpdateMapSprite()
    {   
        print ("settings map: " + StudentInfoSystem.GetCurrentProfile().mapLimit);

        // get current map limit
        switch (StudentInfoSystem.GetCurrentProfile().mapLimit)
        {
            case 0:
            case 1:
                mapImage.sprite = map_none;
                break;
            case 2:
                mapImage.sprite = map_GV;
                break;
            case 3:
                mapImage.sprite = map_MS;
                break;
            case 4:
                mapImage.sprite = map_OV;
                break;
            case 5:
                mapImage.sprite = map_SF;
                break;
            case 6:
                mapImage.sprite = map_OC;
                break;
            case 7:
                mapImage.sprite = map_GP;
                break;
            case 8:
                mapImage.sprite = map_WC;
                break;
            case 9:
                mapImage.sprite = map_PS;
                break;
            case 10:
                mapImage.sprite = map_MB;
                break;
            case 11:
            case 12:
                mapImage.sprite = map_R;
                break;
            case 13:
                mapImage.sprite = map_EJ;
                break;
            case 14:
                mapImage.sprite = map_GS;
                break;
            case 15:
                mapImage.sprite = map_M;
                break;
            case 16:
                mapImage.sprite = map_P;
                break;
        }
    }

    public void UpdateRedPos(MapLocation location)
    {
        switch (location)
        {
            case MapLocation.Ocean:
            case MapLocation.BoatHouse:
                redMarker.localPosition = redPos_boat.localPosition;
                break;
            case MapLocation.GorillaVillage:
                redMarker.localPosition = redPos_GV.localPosition;
                break;
            case MapLocation.Mudslide:
                redMarker.localPosition = redPos_MS.localPosition;
                break;
            case MapLocation.OrcVillage:
                redMarker.localPosition = redPos_OV.localPosition;
                break;
            case MapLocation.SpookyForest:
                redMarker.localPosition = redPos_SF.localPosition;
                break;
            case MapLocation.OrcCamp:
                redMarker.localPosition = redPos_OC.localPosition;
                break;
            case MapLocation.GorillaPoop:
                redMarker.localPosition = redPos_GP.localPosition;
                break;
            case MapLocation.WindyCliff:
                redMarker.localPosition = redPos_WC.localPosition;
                break;
            case MapLocation.PirateShip:
                redMarker.localPosition = redPos_PS.localPosition;
                break;
            case MapLocation.MermaidBeach:
                redMarker.localPosition = redPos_MB.localPosition;
                break;
            case MapLocation.Ruins1:
            case MapLocation.Ruins2:
                redMarker.localPosition = redPos_R.localPosition;
                break;
            case MapLocation.ExitJungle:
                redMarker.localPosition = redPos_EJ.localPosition;
                break;
            case MapLocation.GorillaStudy:
                redMarker.localPosition = redPos_GS.localPosition;
                break;
            case MapLocation.Monkeys:
                redMarker.localPosition = redPos_M.localPosition;
                break;
            case MapLocation.PalaceIntro:
                redMarker.localPosition = redPos_P.localPosition;
                break;
        }
    }
}
