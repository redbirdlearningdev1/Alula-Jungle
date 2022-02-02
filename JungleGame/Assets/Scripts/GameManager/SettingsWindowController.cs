using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        if (instance == null)
            instance = this;

        currentTab = SettingsTab.none;

        audioWindow.transform.localScale = new Vector3(0f, 0f, 0f);
        gameWindow.transform.localScale = new Vector3(0f, 0f, 1f);
        exitWindow.transform.localScale = new Vector3(0f, 0f, 1f);
        mapWindow.transform.localScale = new Vector3(0f, 0f, 1f);

        audioTab.transform.localScale = new Vector3(0f, 0f, 0f);
        gameTab.transform.localScale = new Vector3(0f, 0f, 0f);
        exitTab.transform.localScale = new Vector3(0f, 0f, 0f);
        mapTab.transform.localScale = new Vector3(0f, 0f, 0f);
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
        StartCoroutine(TabPressedRoutine(SettingsTab.audio));
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
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        gameTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        mapTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.1f);
    }

    private IEnumerator ShowTabs()
    {
        audioTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        gameTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        exitTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        mapTab.SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.1f);
    }
}
