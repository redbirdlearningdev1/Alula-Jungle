using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevDrawer : MonoBehaviour
{
    public Image tabImage;
    public LerpableObject drawer;

    public Sprite arrowUpTab;
    public Sprite arrowDownTab;

    private bool drawerShown = false;
    private bool drawerAnimating = false;

    public float hiddenPos;
    public float shownPos;

    void Start()
    {
        if (!GameManager.instance.devModeActivated)
        {
            drawer.gameObject.SetActive(false);
        }
        else
        {
            drawer.gameObject.SetActive(false);
        }
    }

    public void ToggleDevDrawer()
    {
        if (drawerAnimating)
            return;
        drawerAnimating = true;

        drawerShown = !drawerShown;
        if (drawerShown)
        {
            StartCoroutine(ShowDrawer());
        }
        else
        {
            StartCoroutine(HideDrawer());
        }
    }

    private IEnumerator ShowDrawer()
    {
        drawer.LerpYPos(shownPos, 0.2f, false);
        tabImage.sprite = arrowDownTab;
        yield return new WaitForSeconds(0.2f);
        drawerAnimating = false;
    }

    private IEnumerator HideDrawer()
    {
        drawer.LerpYPos(hiddenPos, 0.2f, false);
        tabImage.sprite = arrowUpTab;
        yield return new WaitForSeconds(0.2f);
        drawerAnimating = false;
    }

    // button functions
    
    public void OnDevMenuButtonPressed()
    {
        ToggleDevDrawer();
        GameManager.instance.LoadScene("DevMenu", true);
    }

    public void OnSkipTalkieButtonPressed()
    {
        ToggleDevDrawer();
        TalkieManager.instance.SkipTalkie();
    }

    public void OnSkipGameButtonPressed()
    {
        ToggleDevDrawer();
        GameManager.instance.SkipCurrentGame();
    }

    public void OnConsoleButtonPressed()
    {
        ToggleDevDrawer();
        GameManager.instance.OpenConsole();
    }

}
