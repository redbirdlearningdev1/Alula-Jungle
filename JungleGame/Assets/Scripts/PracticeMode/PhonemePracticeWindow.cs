using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhonemePracticeWindow : MonoBehaviour
{
    public LerpableObject myWindow;
    public LerpableObject exitButton;
    public Color selectedColor;
    public Color nonselectedColor;

    // current profile parts
    private int currentProfile;
    private List<StudentPlayerData> profiles;
    public Image profileImage;
    public TextMeshProUGUI profileText;
    // phonemes
    private List<ActionWordEnum> currentPhonemes;
    public Button selectPhonemesButton;
    public Button allPhonemesButton;
    // games
    private int currentGames;
    public Button games10Button;
    public Button games20Button;
    public Button gamesXButton;
    public TextMeshProUGUI gamesXText;
    // versions
    private bool moveVersion;
    private bool soundVersion;
    private bool iconVersion;
    public Button moveButton;
    public Button soundButton;
    public Button iconButton;

    public Button startPracticeButton;

    void Awake()
    {
        // hide ExitButton
        exitButton.transform.localScale = Vector3.zero;
    }

    private void ResetWindow()
    {
        // get all profiles
        profiles = new List<StudentPlayerData>();
        List<StudentPlayerData> allProfiles = StudentInfoSystem.GetAllStudentDatas();
        // determine active profiles
        foreach (var profile in allProfiles)
        {
            if (profile.active)
            {
                profiles.Add(profile);
            }
        }

        // only allow practice iff more than 0 active profiles
        currentProfile = 0;
        if (profiles.Count > 0)
        {
            profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
            profileText.text = profiles[currentProfile].name;
            startPracticeButton.interactable = true;
        }
        else
        {
            // no active profiles
            startPracticeButton.interactable = false;
        }

        currentPhonemes = new List<ActionWordEnum>();
        currentPhonemes.AddRange(GameManager.instance.GetGlobalActionWordList());
        selectPhonemesButton.image.color = nonselectedColor;
        allPhonemesButton.image.color = selectedColor;

        currentGames = 20;
        games10Button.image.color = nonselectedColor;
        games20Button.image.color = selectedColor;
        gamesXButton.image.color = nonselectedColor;
        gamesXText.text = "x";

        moveVersion = false;
        soundVersion = true;
        iconVersion = true;
        moveButton.image.color = nonselectedColor;
        soundButton.image.color = selectedColor;
        iconButton.image.color = selectedColor;
    }

    public void OpenWindow()
    {
        StartCoroutine(OpenWindowRoutine());
    }

    private IEnumerator OpenWindowRoutine()
    {
        ResetWindow();
        myWindow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);

        yield return new WaitForSeconds(0.4f);

        // show ExitButton
        exitButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
    }

    public void CloseWindow()
    {
        StartCoroutine(CloseWindowRoutine());
    }

    private IEnumerator CloseWindowRoutine()
    {
        // hide ExitButton
        exitButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        myWindow.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
        PracticeSceneManager.instance.RemoveWindowBG();
    }

    public void OnProfileButtonPressed()
    {
        currentProfile++;
        if (currentProfile > profiles.Count - 1)
        {
            currentProfile = 0;
        }

        // swap current profile
        profileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        profileImage.sprite = GameManager.instance.avatars[profiles[currentProfile].profileAvatar];
        profileText.text = profiles[currentProfile].name;
    }

    public void OnSelectPhonemesPressed()
    {
        selectPhonemesButton.image.color = selectedColor;
        allPhonemesButton.image.color = nonselectedColor;

        // open phonemes select window
        PhonemeSelectWindow.instance.OpenWindow(currentPhonemes, ReturnLocation.phonemePracticeWindow);
    }

    public void ReturnSelectedPhonemes(List<ActionWordEnum> selectedPhonemes)
    {
        currentPhonemes.Clear();
        currentPhonemes.AddRange(selectedPhonemes);
    }

    public void ReturnNumGames(int numGames)
    {
        currentGames = numGames;

        gamesXText.text = currentGames.ToString() + "*";
    }

    public void OnAllPhonemesPressed()
    {
        selectPhonemesButton.image.color = nonselectedColor;
        allPhonemesButton.image.color = selectedColor;       

        currentPhonemes = new List<ActionWordEnum>();
        currentPhonemes.AddRange(GameManager.instance.GetGlobalActionWordList());
    }

    public void On10GamesButtonPressed()
    {
        currentGames = 10;
        games10Button.image.color = selectedColor;
        games20Button.image.color = nonselectedColor;
        gamesXButton.image.color = nonselectedColor;
        gamesXText.text = "x";
    }

    public void On20GamesButtonPressed()
    {
        currentGames = 20;
        games10Button.image.color = nonselectedColor;
        games20Button.image.color = selectedColor;
        gamesXButton.image.color = nonselectedColor;
        gamesXText.text = "x";
    }

    public void OnXGamesButtonPressed()
    {
        games10Button.image.color = nonselectedColor;
        games20Button.image.color = nonselectedColor;
        gamesXButton.image.color = selectedColor;

        // open select number of games window
        GamesSelectWindow.instance.OpenWindow(currentGames, ReturnLocation.phonemePracticeWindow);
    }

    public void OnMoveButtonPressed()
    {
        moveVersion = !moveVersion;

        if (moveVersion)
        {
            moveButton.image.color = selectedColor;
        }
        else
        {
            moveButton.image.color = nonselectedColor;
        }
    }

    public void OnSoundButtonPressed()
    {
        soundVersion = !soundVersion;

        if (soundVersion)
        {
            soundButton.image.color = selectedColor;
        }
        else
        {
            soundButton.image.color = nonselectedColor;
        }
    }

    public void OnIconButtonPressed()
    {
        iconVersion = !iconVersion;

        if (iconVersion)
        {
            iconButton.image.color = selectedColor;
        }
        else
        {
            iconButton.image.color = nonselectedColor;
        }
    }

    public void OnStartPracticeButtonPressed()
    {

    }
}
