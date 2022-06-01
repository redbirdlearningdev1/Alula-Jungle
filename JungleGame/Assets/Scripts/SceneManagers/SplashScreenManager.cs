using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashScreenManager : MonoBehaviour
{
    public static SplashScreenManager instance;

    public Animator mainAnimator;

    [Header("Splash Screen BGs")]
    public Animator BG3_animator;
    public Animator BG4_animator;
    public Animator BG6_animator;

    [Header("Profile Windows")]
    [SerializeField] CanvasGroup profileSelectWindow;
    [SerializeField] NewProfileWindow newProfileWindow;
    [SerializeField] EditProfileWindow editProfileWindow;

    [SerializeField] Button startButton;

    [SerializeField] Button profile1Button;
    [SerializeField] Button profile2Button;
    [SerializeField] Button profile3Button;

    [SerializeField] Button editProfile1Button;
    [SerializeField] Button editProfile2Button;
    [SerializeField] Button editProfile3Button;

    [SerializeField] Image profile1Image;
    [SerializeField] Image profile2Image;
    [SerializeField] Image profile3Image;

    [SerializeField] TextMeshProUGUI profile1Text;
    [SerializeField] TextMeshProUGUI profile2Text;
    [SerializeField] TextMeshProUGUI profile3Text;

    public Animator profile1SelectAnimator;
    public Animator profile2SelectAnimator;
    public Animator profile3SelectAnimator;

    public LerpableObject winCrown1;
    public LerpableObject winCrown2;
    public LerpableObject winCrown3;

    public LerpableObject practiceButton;
    private Vector3 practiceButtonPos;

    [SerializeField] TMP_InputField newProfileInput;

    [SerializeField] WiggleController tapTextWiggleController;
    public TextMeshProUGUI tapText;

    private StudentPlayerData data1;
    private StudentPlayerData data2;
    private StudentPlayerData data3;

    private int selectedProfile = 0;
    private bool screenTapped = false;
    private bool screenTapReady = false;
    private bool switchingProfiles = false;

    public float selectedAlpha;
    public float unselectedAlpha;

    public float selectedScale;
    public float unselectedScale;

    public float lerpSpeed;

    private StudentIndex newProfileIndex;
    private int profileAvatarIndex;
    public Image selectedProfileImage;
    public Sprite emptyAvatarSprite;


    [Header("Edit Profile")]
    public Image editProfileImage;
    public TMP_InputField editProfileInput;
    public LerpableObject confirmDeleteProfileWindow;
    public LerpableObject confirmDeleteProfileBG;

    [Header("Button Stuff")]
    public Image startbuttonBox;
    public Image startbuttonText;
    public Sprite boxGreen;
    public Sprite boxBrown;
    public Sprite textWhite;
    public Sprite textGreen;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // end practice mode iff needed
        GameManager.instance.practiceModeON = false;
        GameManager.instance.practiceModeCounter.text = "";

        // stop music 
        AudioManager.instance.StopMusic();

        // play song
        AudioManager.instance.PlaySong(AudioDatabase.instance.SplashScreenSong);

        // close confirmation window
        confirmDeleteProfileWindow.transform.localScale = Vector2.zero;
        confirmDeleteProfileBG.LerpImageAlpha(confirmDeleteProfileBG.GetComponent<Image>(), 0f, 0f);
        confirmDeleteProfileBG.GetComponent<Image>().raycastTarget = false;

        // get all three profiles
        var profiles = StudentInfoSystem.GetAllStudentDatas();

        StudentPlayerData currProfile = null;
        // determine most recently used profile + set current student
        foreach (var profile in profiles)
        {
            if (profile.mostRecentProfile)
            {
                StudentInfoSystem.SetStudentPlayer(profile.studentIndex);
                currProfile = profile;
            }
        }

        if (currProfile != null)
        {
            GameManager.instance.SendLog("SplashScreenManager", "current profile: " + currProfile.name);
            GameManager.instance.SendLog("SplashScreenManager", "current chapter: " + currProfile.currentChapter);
        }
        // if no most recently used profile - set profile 1 as current profile
        else
        {
            StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
        }

        // default to chapter 0 animations
        BG3_animator.Play("3_Ch0");
        BG4_animator.Play("4_Ch0");
        BG6_animator.Play("6_Ch0");

        // set correct chapter animations
        if (currProfile != null)
        {
            // get most recent chapter
            Chapter chapter = currProfile.currentChapter;
            
            if (chapter > Chapter.chapter_1)
            {
                BG3_animator.Play("3_Ch1");
            }

            if (chapter > Chapter.chapter_2)
            {
                BG6_animator.Play("6_Ch2");
            }

            if (chapter > Chapter.chapter_3)
            {
                BG4_animator.Play("4_Ch3");
            }

            if (chapter > Chapter.chapter_4)
            {
                BG4_animator.Play("4_Ch4");
            }

            if (chapter > Chapter.chapter_5)
            {
                BG6_animator.Play("6_Ch5");
            }
            
            if (chapter > Chapter.chapter_6)
            {
                BG4_animator.Play("4_Ch6");
            }
        }

        // turn off start button
        startButton.interactable = false;
        startbuttonBox.sprite = boxBrown;
        startbuttonText.sprite = textGreen;

        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        profileSelectWindow.alpha = 0f;

        // set edit profile buttons to be off
        editProfile1Button.transform.localScale = Vector3.zero;
        editProfile2Button.transform.localScale = Vector3.zero;
        editProfile3Button.transform.localScale = Vector3.zero;

        // wiggle text controller
        tapTextWiggleController.StartWiggle();

        // set images
        selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];
        editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        // move practice button off-screen
        practiceButtonPos = practiceButton.transform.localPosition;
        practiceButton.transform.localPosition = new Vector3(practiceButtonPos.x, practiceButtonPos.y + 150f, 0f);

        // start screen tap delay
        StartCoroutine(ScreenTapDelay());

        SetUpProfiles();
    }

    private IEnumerator ScreenTapDelay()
    {
        yield return new WaitForSeconds(3f);
        screenTapReady = true;
    }

    void Update()
    {
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !screenTapped && screenTapReady)
        {
            screenTapped = true;

            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
            StartCoroutine(RevealProfileWindow(0.5f));
        }
    }

    private void SetUpProfiles()
    {
        // set up profiles
        // profile 1
        data1 = StudentInfoSystem.GetStudentData(StudentIndex.student_1);
        if (data1.active)
        {
            profile1Text.text = data1.name;
            profile1Image.sprite = GameManager.instance.avatars[data1.profileAvatar];
        }
        else
        {
            profile1Text.text = "create profile";
            profile1Image.sprite = emptyAvatarSprite;
        }

        // profile 2
        data2 = StudentInfoSystem.GetStudentData(StudentIndex.student_2);
        if (data2.active)
        {
            profile2Text.text = data2.name;
            profile2Image.sprite = GameManager.instance.avatars[data2.profileAvatar];
        }
        else
        {
            profile2Text.text = "create profile";
            profile2Image.sprite = emptyAvatarSprite;
        }

        // profile 3
        data3 = StudentInfoSystem.GetStudentData(StudentIndex.student_3);
        if (data3.active)
        {
            profile3Text.text = data3.name;
            profile3Image.sprite = GameManager.instance.avatars[data3.profileAvatar];
        }
        else
        {
            profile3Text.text = "create profile";
            profile3Image.sprite = emptyAvatarSprite;
        }
    }

    private void SetImageAlpha(Image img, float a)
    {
        Color c;
        c = img.color;
        c.a = a;
        img.color = c;
    }

    private IEnumerator RevealProfileWindow(float totalTime)
    {
        // wiggle text controller
        tapTextWiggleController.StopWiggle();

        mainAnimator.Play("Tapped");

        yield return new WaitForSeconds(3f);

        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            var windowAlpha = Mathf.Lerp(0f, 1f, timer / totalTime);
            var textAlpha = Mathf.Lerp(1f, 0f, timer / totalTime);
            profileSelectWindow.alpha = windowAlpha;
            tapText.color = new Color(1f, 1f, 1f, textAlpha);

            if (timer > totalTime)
                break;
            yield return null;
        }

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;
        profileSelectWindow.alpha = 1f;

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

        // enable practice button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
        }

        // show practice button
        practiceButton.LerpYPos(practiceButtonPos.y - 50, 0.2f, true);
        yield return new WaitForSeconds(0.2f);
        practiceButton.LerpYPos(practiceButtonPos.y, 0.2f, true);
        yield return new WaitForSeconds(0.2f);

        SetUpWinCrowns();
    }

    private void SetUpWinCrowns()
    {
        // show crowns if profile has won game
        data1 = StudentInfoSystem.GetStudentData(StudentIndex.student_1);
        if (data1.active && data1.currStoryBeat >= StoryBeat.FinishedGame)
        {
            winCrown1.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
            winCrown1.GetComponent<BobController>().StartBob();
        }
        else
        {
            // remove crown if crown is active
            if (winCrown1.transform.localScale.x > 0f)
            {
                winCrown1.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
                winCrown1.GetComponent<BobController>().StopBob();
            }
        }

        // profile 2
        data2 = StudentInfoSystem.GetStudentData(StudentIndex.student_2);
        if (data2.active && data2.currStoryBeat >= StoryBeat.FinishedGame)
        {
            winCrown2.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
            winCrown2.GetComponent<BobController>().StartBob();
        }
        else
        {
            // remove crown if crown is active
            if (winCrown2.transform.localScale.x > 0f)
            {
                winCrown2.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
                winCrown2.GetComponent<BobController>().StopBob();
            }
        }

        // profile 3
        data3 = StudentInfoSystem.GetStudentData(StudentIndex.student_3);
        if (data3.active && data3.currStoryBeat >= StoryBeat.FinishedGame)
        {
            winCrown3.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.2f, 0.2f);
            winCrown3.GetComponent<BobController>().StartBob();
        }
        else
        {
            // remove crown if crown is active
            if (winCrown3.transform.localScale.x > 0f)
            {
                winCrown3.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
                winCrown3.GetComponent<BobController>().StopBob();
            }
        }
    }

    public void OnStartPressed()
    {
        // turn off start button
        startButton.interactable = false;
        startbuttonBox.sprite = boxBrown;
        startbuttonText.sprite = textGreen;

        // load selected profile or make new profile if inactive
        switch (selectedProfile)
        {
            case 1:
                if (data1.active)
                    LoadProfileAndContinue(StudentIndex.student_1);
                break;
            case 2:
                if (data2.active)
                    LoadProfileAndContinue(StudentIndex.student_2);
                break;
            case 3:
                if (data3.active)
                    LoadProfileAndContinue(StudentIndex.student_3);
                break;
        }
    }

    private void OpenNewProfileWindow(StudentIndex index)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);

        // make start button not interactable
        startButton.interactable = false;
        startbuttonBox.sprite = boxBrown;
        startbuttonText.sprite = textGreen;

        newProfileIndex = index;
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;

        newProfileWindow.OpenWindow();
    }

    public void EnableProfileInteraction(bool unselectProfile)
    {
        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;

        // unselect current profile
        if (unselectProfile)
        {
            switch (selectedProfile)
            {
                case 0: 
                    break;
                case 1: 
                    profile1SelectAnimator.Play("UnselectProfile"); 
                    break;
                case 2: 
                    profile2SelectAnimator.Play("UnselectProfile"); 
                    break;
                case 3: 
                    profile3SelectAnimator.Play("UnselectProfile"); 
                    break;
            }
            selectedProfile = 0;
        }
        else
        {
            // turn on start button
            startButton.interactable = true;
            startbuttonBox.sprite = boxGreen;
            startbuttonText.sprite = textWhite;
        }
    }

    public void CreateNewProfilePressed(string newProfileName)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(newProfileIndex);
        StudentInfoSystem.RemoveCurrentStudentPlayer();
        StudentInfoSystem.SetStudentPlayer(newProfileIndex);
        StudentInfoSystem.GetCurrentProfile().name = newProfileName;
        StudentInfoSystem.GetCurrentProfile().profileAvatar = profileAvatarIndex;
        StudentInfoSystem.GetCurrentProfile().active = true; // this profile is now active!!!
        StudentInfoSystem.SaveStudentPlayerData();

        SetUpProfiles();
        selectedProfile = 0;
        SelectProfile((int)newProfileIndex + 1);

        // turn on start button
        startButton.interactable = true;
        startbuttonBox.sprite = boxGreen;
        startbuttonText.sprite = textWhite;

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;

        // enable practice button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
        }
    }

    private void LoadProfileAndContinue(StudentIndex index)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);
        // set profile to be current player
        StudentInfoSystem.SetStudentPlayer(index);
        // load scroll map scene
        GameManager.instance.LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void SelectProfile(int profileNum)
    {
        // cannot select already selected profile
        if (selectedProfile == profileNum)
            return;

        if (switchingProfiles)
            return;
        switchingProfiles = true;
        
        if (profileNum <= 0 || profileNum > 3)
            return;

        // select correct profile
        StartCoroutine(SelectProfileRoutine(profileNum));
        
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // check if profile is empty
        switch (selectedProfile)
        {
            case 0: 
                break;
            case 1: 
                if (!data1.active)
                {
                    OpenNewProfileWindow(StudentIndex.student_1);
                    return;
                }
                break;
            case 2: 
                if (!data2.active)
                {
                    OpenNewProfileWindow(StudentIndex.student_2);
                    return;
                }
                break;
            case 3: 
                if (!data3.active)
                {
                    OpenNewProfileWindow(StudentIndex.student_3);
                    return;
                }
                break;
        }

        // turn on start button
        startButton.interactable = true;
        startbuttonBox.sprite = boxGreen;
        startbuttonText.sprite = textWhite;
    }

    private IEnumerator SelectProfileRoutine(int profileIndex)
    {
        switch (selectedProfile)
        {
            case 0: 
                break;
            case 1: 
                profile1SelectAnimator.Play("UnselectProfile");
                if (data1.active)
                    editProfile1Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
            case 2: 
                profile2SelectAnimator.Play("UnselectProfile");
                if (data2.active)
                    editProfile2Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
            case 3: 
                profile3SelectAnimator.Play("UnselectProfile");
                if (data3.active)
                    editProfile3Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
        }

        selectedProfile = profileIndex;

        yield return new WaitForSeconds(0.1f);

        switch (selectedProfile)
        {
            case 1: 
                profile1SelectAnimator.Play("SelectProfile");
                profile1Image.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.2f, 0.2f);
                if (data1.active)
                    editProfile1Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
                break;

            case 2: 
                profile2SelectAnimator.Play("SelectProfile");
                profile2Image.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.2f, 0.2f);
                if (data2.active)
                    editProfile2Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
                break;

            case 3: 
                profile3SelectAnimator.Play("SelectProfile");
                profile3Image.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.2f, 0.2f);
                if (data3.active)
                    editProfile3Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
                break;
        }

        switchingProfiles = false;
    }

    public void OnLeftArrowPressed()
    {   
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // reduce index, iff less than 0, return to max number
        profileAvatarIndex--;
        if (profileAvatarIndex < 0)
            profileAvatarIndex = GameManager.instance.avatars.Count - 1;

        selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];
        editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        selectedProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        editProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
    }

    public void OnRightArrowPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // increase index, iff greater than max, return to 0
        profileAvatarIndex++;
        if (profileAvatarIndex >= GameManager.instance.avatars.Count)
            profileAvatarIndex = 0;

        selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];
        editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        selectedProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        editProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
    }

    /* 
    ################################################
    #   EDIT PROFILE WINDOW
    ################################################
    */

    public void OpenEditProfileWindow(int num)
    {
        if (num == 1)
        {
            OpenEditProfileWindow(StudentIndex.student_1);
        }
        else if (num == 2)
        {
            OpenEditProfileWindow(StudentIndex.student_2);
        }
        else if (num == 3)
        {
            OpenEditProfileWindow(StudentIndex.student_3);
        }
    }

    private void OpenEditProfileWindow(StudentIndex index)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);

        // set correct avatar
        newProfileIndex = index;
        profileAvatarIndex = StudentInfoSystem.GetStudentData(index).profileAvatar;
        editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        // set correct name
        editProfileInput.text = StudentInfoSystem.GetStudentData(index).name;

        // make start button not interactable
        startButton.interactable = false;
        startbuttonBox.sprite = boxBrown;
        startbuttonText.sprite = textGreen;

        newProfileIndex = index;
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;

        editProfileWindow.OpenWindow();
    }

    public void UpdateProfilePressed(string newName)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.SetStudentPlayer(newProfileIndex);
        StudentInfoSystem.GetCurrentProfile().name = newName;
        StudentInfoSystem.GetCurrentProfile().profileAvatar = profileAvatarIndex;
        StudentInfoSystem.SaveStudentPlayerData();

        SetUpProfiles();
        selectedProfile = 0;
        SelectProfile((int)newProfileIndex + 1);

        // turn on start button
        startButton.interactable = true;
        startbuttonBox.sprite = boxGreen;
        startbuttonText.sprite = textWhite;

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;
    }

    public void OnDeleteProfileButtonPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // open confirmation window
        confirmDeleteProfileWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
        confirmDeleteProfileBG.LerpImageAlpha(confirmDeleteProfileBG.GetComponent<Image>(), 0.8f, 0.5f);
        confirmDeleteProfileBG.GetComponent<Image>().raycastTarget = true;
    }

    public void OnYesDeleteProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SadBlip, 1f);

        // close confirmation window
        confirmDeleteProfileWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        confirmDeleteProfileBG.LerpImageAlpha(confirmDeleteProfileBG.GetComponent<Image>(), 0f, 0.5f);
        confirmDeleteProfileBG.GetComponent<Image>().raycastTarget = false;

        // close edit profile window
        editProfileWindow.OnCloseWindowButtonPressed();

        // reset profile
        StudentInfoSystem.ResetProfile(newProfileIndex);

        SetUpProfiles();
        SetUpWinCrowns();

        // unselect current profile
        switch (selectedProfile)
        {
            case 0: 
                break;
            case 1: 
                profile1SelectAnimator.Play("UnselectProfile");
                editProfile1Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
            case 2: 
                profile2SelectAnimator.Play("UnselectProfile");
                editProfile2Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
            case 3: 
                profile3SelectAnimator.Play("UnselectProfile");
                editProfile3Button.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
                break;
        }
        selectedProfile = 0;

        // turn off start button
        startButton.interactable = false;
        startbuttonBox.sprite = boxBrown;
        startbuttonText.sprite = textGreen;

        // enable practice button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
        }
    }

    public void OnNoDeleteProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // close confirmation window
        confirmDeleteProfileWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        confirmDeleteProfileBG.LerpImageAlpha(confirmDeleteProfileBG.GetComponent<Image>(), 0f, 0.5f);
        confirmDeleteProfileBG.GetComponent<Image>().raycastTarget = false;
    }

    public void OnPracticeButtonPressed()
    {
        GameManager.instance.LoadScene("PracticeScene", true);
    }
}
