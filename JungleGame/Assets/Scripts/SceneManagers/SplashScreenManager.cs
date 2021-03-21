using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{

    [SerializeField] CanvasGroup profileSelectWindow;
    [SerializeField] Button startButton;
    [SerializeField] Button profile1Button;
    [SerializeField] Button profile2Button;
    [SerializeField] Button profile3Button;
    [SerializeField] Image profile1Image;
    [SerializeField] Image profile2Image;
    [SerializeField] Image profile3Image;
    private int selectedProfile = 0;
    private bool screenTapped = false;

    public Color selectedColor;
    public Color unselectedColor;

    void Awake() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // set up profile window
        startButton.interactable = false;
        profile1Image.color = unselectedColor;
        profile2Image.color = unselectedColor;
        profile3Image.color = unselectedColor;
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        profileSelectWindow.alpha = 0f;
        
    }

    void Update() 
    {
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !screenTapped)
        {
            screenTapped = true;
            StartCoroutine(RevealProfileWindow(0.5f));
        }
    }

    private IEnumerator RevealProfileWindow(float totalTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            profileSelectWindow.alpha = Mathf.Lerp(0f, 1f, timer / totalTime);

            if (timer > totalTime)
                break;
            yield return null;
        }

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;
        profileSelectWindow.alpha = 1f;
    }

    public void OnStartPressed()
    {
        startButton.interactable = false;
        GameManager.instance.LoadScene("JungleWelcomeScene", true);
    }

    public void SelectProfile(int profileNum)
    {
        if (profileNum <= 0 || profileNum > 3)
            return;
        
        selectedProfile = profileNum;
        startButton.interactable = true;
        profile1Image.color = unselectedColor;
        profile2Image.color = unselectedColor;
        profile3Image.color = unselectedColor;

        switch (profileNum)
        {
            case 1:
                profile1Image.color = selectedColor;
                break;
            case 2:
                profile2Image.color = selectedColor;
                break;
            case 3:
                profile3Image.color = selectedColor;
                break;
        }
    }
}
