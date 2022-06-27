using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhonemeChangingWindow : MonoBehaviour
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
    // difficulty
    private int diffValue;
    public LerpableObject diffBox;
    public TextMeshProUGUI diffText;
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
    private bool addVersion;
    private bool subVersion;
    private bool delVersion;
    public Button addButton;
    public Button subButton;
    public Button delButton;

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

        // set default values
        diffValue = 1;
        diffText.text = "1";

        currentPhonemes = new List<ActionWordEnum>();
        currentPhonemes.AddRange(GameManager.instance.GetGlobalActionWordList());
        selectPhonemesButton.image.color = nonselectedColor;
        allPhonemesButton.image.color = selectedColor;

        currentGames = 20;
        games10Button.image.color = nonselectedColor;
        games20Button.image.color = selectedColor;
        gamesXButton.image.color = nonselectedColor;
        gamesXText.text = "x";

        addVersion = true;
        subVersion = true;
        delVersion = true;
        addButton.image.color = selectedColor;
        subButton.image.color = selectedColor;
        delButton.image.color = selectedColor;
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

    public void OnLeftArrowDifficultyPressed()
    {
        // decrease difficulty value and cap at 1
        diffValue--;
        if (diffValue < 1)
        {
            diffValue = 1;
        }
        diffBox.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        diffText.text = diffValue.ToString();
    }

    public void OnRightArrowDifficultyPressed()
    {
        // increase difficulty value and cap at 6
        diffValue++;
        if (diffValue > 6)
        {
            diffValue = 6;
        }
        diffBox.SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        diffText.text = diffValue.ToString();
    }

    public void OnSelectPhonemesPressed()
    {
        selectPhonemesButton.image.color = selectedColor;
        allPhonemesButton.image.color = nonselectedColor;

        // open phonemes select window
        PhonemeSelectWindow.instance.OpenWindow(currentPhonemes, ReturnLocation.phonemeChangingWindow);
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
        GamesSelectWindow.instance.OpenWindow(currentGames, ReturnLocation.phonemeChangingWindow);
    }

    public void OnAddButtonPressed()
    {
        addVersion = !addVersion;

        if (addVersion)
        {
            addButton.image.color = selectedColor;
        }
        else
        {
            addButton.image.color = nonselectedColor;
        }
    }

    public void OnSubButtonPressed()
    {
        subVersion = !subVersion;

        if (subVersion)
        {
            subButton.image.color = selectedColor;
        }
        else
        {
            subButton.image.color = nonselectedColor;
        }
    }

    public void OnDelButtonPressed()
    {
        delVersion = !delVersion;

        if (delVersion)
        {
            delButton.image.color = selectedColor;
        }
        else
        {
            delButton.image.color = nonselectedColor;
        }
    }

    public void OnStartPracticeButtonPressed()
    {
        PracticeSceneManager.instance.StartPractice(PracticeModeGame.phoneme_changing, diffValue, currentGames, currentPhonemes, addVersion, subVersion, delVersion, false, false, false);
    }
}
