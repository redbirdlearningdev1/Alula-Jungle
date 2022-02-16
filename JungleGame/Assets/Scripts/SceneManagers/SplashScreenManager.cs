using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashScreenManager : MonoBehaviour
{
    public Animator mainAnimator;

    [Header("Splash Screen BGs")]
    public Animator BG3_animator;
    public Animator BG4_animator;
    public Animator BG6_animator;

    [Header("Profile Windows")]
    [SerializeField] CanvasGroup profileSelectWindow;
    [SerializeField] CanvasGroup newProfileWindow;
    [SerializeField] CanvasGroup editProfileWindow;

    [SerializeField] Button startButton;
    [SerializeField] Button newProfileButton;

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


    [Header("Edit Profile")]
    public Image editProfileImage;
    public TMP_InputField editProfileInput;

    [Header("Button Stuff")]
    public Image buttonBG;
    public Image buttonText;
    public Sprite boxGreen;
    public Sprite textWhite;
    public Sprite textGreen;

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // stop music 
        AudioManager.instance.StopMusic();

        // play song
        AudioManager.instance.PlaySong(AudioDatabase.instance.Sunrise_LouieZong);

        // get all three profiles
        var profiles = StudentInfoSystem.GetAllStudentDatas();

        foreach (var profile in profiles)
        {
            print ("profile: " + profile);
        }

        StudentPlayerData currProfile = null;
        // determine most recently used profile
        foreach (var profile in profiles)
        {
            if (profile.mostRecentProfile)
                currProfile = profile;
        }

        if (currProfile != null)
        {
            print ("currProfile: " + currProfile.name);
            print ("currChapter: " + currProfile.currentChapter);
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

            if (chapter > Chapter.chapter_final)
            {
                BG4_animator.Play("4_Ch6");
            }
        }

        // set up profile window
        startButton.interactable = false;
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        profileSelectWindow.alpha = 0f;

        // set up new profile window
        newProfileWindow.interactable = false;
        newProfileWindow.blocksRaycasts = false;
        newProfileWindow.alpha = 0f;

        // set up new profile window
        editProfileWindow.interactable = false;
        editProfileWindow.blocksRaycasts = false;
        editProfileWindow.alpha = 0f;

        // set edit profile buttons to be off
        editProfile1Button.gameObject.SetActive(false);
        editProfile2Button.gameObject.SetActive(false);
        editProfile3Button.gameObject.SetActive(false);

        // wiggle text controller
        tapTextWiggleController.StartWiggle();

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
            editProfile1Button.gameObject.SetActive(true);
        }

        // profile 2
        data2 = StudentInfoSystem.GetStudentData(StudentIndex.student_2);
        if (data2.active)
        {
            profile2Text.text = data2.name;
            profile2Image.sprite = GameManager.instance.avatars[data2.profileAvatar];
            editProfile2Button.gameObject.SetActive(true);
        }

        // profile 3
        data3 = StudentInfoSystem.GetStudentData(StudentIndex.student_3);
        if (data3.active)
        {
            profile3Text.text = data3.name;
            profile3Image.sprite = GameManager.instance.avatars[data3.profileAvatar];
            editProfile3Button.gameObject.SetActive(true);
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

        // show menu button
        SettingsManager.instance.ToggleMenuButtonActive(true);

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
    }

    private IEnumerator RevealNewProfileWindow(float totalTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            newProfileWindow.alpha = Mathf.Lerp(0f, 1f, timer / totalTime);

            if (timer > totalTime)
                break;
            yield return null;
        }

        // new profile avatar is 10 by default (red)
        profileAvatarIndex = 10;
        selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        newProfileWindow.interactable = true;
        newProfileWindow.blocksRaycasts = true;
        newProfileWindow.alpha = 1f;
    }

    private IEnumerator HideNewProfileWindow(float totalTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            newProfileWindow.alpha = Mathf.Lerp(1f, 0f, timer / totalTime);

            if (timer > totalTime)
                break;
            yield return null;
        }

        newProfileWindow.interactable = false;
        newProfileWindow.blocksRaycasts = false;
        newProfileWindow.alpha = 0f;
    }

    public void OnStartPressed()
    {
        startButton.interactable = false;

        // load selected profile or make new profile if inactive
        switch (selectedProfile)
        {
            case 1:
                if (data1.active)
                    LoadProfileAndContinue(StudentIndex.student_1);
                else
                    OpenNewProfileWindow(StudentIndex.student_1);
                break;
            case 2:
                if (data2.active)
                    LoadProfileAndContinue(StudentIndex.student_2);
                else
                    OpenNewProfileWindow(StudentIndex.student_2);
                break;
            case 3:
                if (data3.active)
                    LoadProfileAndContinue(StudentIndex.student_3);
                else
                    OpenNewProfileWindow(StudentIndex.student_3);
                break;
        }
    }

    private void OpenNewProfileWindow(StudentIndex index)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);

        newProfileIndex = index;
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        StartCoroutine(RevealNewProfileWindow(0.5f));
    }

    public void CreateNewProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.ResetProfile(newProfileIndex);
        StudentInfoSystem.RemoveCurrentStudentPlayer();
        StudentInfoSystem.SetStudentPlayer(newProfileIndex);
        StudentInfoSystem.GetCurrentProfile().name = newProfileInput.text;
        StudentInfoSystem.GetCurrentProfile().profileAvatar = profileAvatarIndex;
        StudentInfoSystem.SaveStudentPlayerData();

        SetUpProfiles();

        SelectProfile((int)newProfileIndex + 1);
        startButton.interactable = true;

        StartCoroutine(HideNewProfileWindow(0.5f));
        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;
    }

    private void LoadProfileAndContinue(StudentIndex index)
    {
        // change button sprites
        buttonBG.sprite = boxGreen;
        buttonText.sprite = textGreen;

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);
        // set profile to be current player
        StudentInfoSystem.SetStudentPlayer(index);
        // load scroll map scene
        GameManager.instance.LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void SelectProfile(int profileNum)
    {
        if (switchingProfiles)
            return;
        switchingProfiles = true;
        
        if (profileNum <= 0 || profileNum > 3)
            return;

        // select correct profile
        StartCoroutine(SelectProfileRoutine(profileNum));
        
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // make start button interactable
        if (!startButton.interactable)
        {
            startButton.interactable = true;
            buttonText.sprite = textWhite;
        }

        profile1Image.GetComponent<LerpableObject>().LerpScale(new Vector2(unselectedScale, unselectedScale), lerpSpeed);
        profile2Image.GetComponent<LerpableObject>().LerpScale(new Vector2(unselectedScale, unselectedScale), lerpSpeed);
        profile3Image.GetComponent<LerpableObject>().LerpScale(new Vector2(unselectedScale, unselectedScale), lerpSpeed);

        StudentPlayerData studentData;

        switch (profileNum)
        {
            default:
            case 1:
                SetImageAlpha(profile1Image, selectedAlpha);
                profile1Image.GetComponent<LerpableObject>().LerpScale(new Vector2(selectedScale, selectedScale), lerpSpeed);
                studentData = StudentInfoSystem.GetStudentData(StudentIndex.student_1);
                break;
            case 2:
                SetImageAlpha(profile2Image, selectedAlpha);
                profile2Image.GetComponent<LerpableObject>().LerpScale(new Vector2(selectedScale, selectedScale), lerpSpeed);
                studentData = StudentInfoSystem.GetStudentData(StudentIndex.student_2);
                break;
            case 3:
                SetImageAlpha(profile3Image, selectedAlpha);
                profile3Image.GetComponent<LerpableObject>().LerpScale(new Vector2(selectedScale, selectedScale), lerpSpeed);
                studentData = StudentInfoSystem.GetStudentData(StudentIndex.student_3);
                break;
        }
    }

    private IEnumerator SelectProfileRoutine(int profileIndex)
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

        selectedProfile = profileIndex;

        yield return new WaitForSeconds(0.1f);

        switch (selectedProfile)
        {
            case 1: profile1SelectAnimator.Play("SelectProfile"); break;
            case 2: profile2SelectAnimator.Play("SelectProfile"); break;
            case 3: profile3SelectAnimator.Play("SelectProfile"); break;
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

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;
        StartCoroutine(RevealEditProfileWindow(0.5f));
    }

    public void UpdateProfilePressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CreateBlip, 1f);

        StudentInfoSystem.SetStudentPlayer(newProfileIndex);
        StudentInfoSystem.GetCurrentProfile().name = editProfileInput.text;
        StudentInfoSystem.GetCurrentProfile().profileAvatar = profileAvatarIndex;
        StudentInfoSystem.SaveStudentPlayerData();

        SetUpProfiles();

        SelectProfile((int)newProfileIndex + 1);
        startButton.interactable = true;

        StartCoroutine(HideEditProfileWindow(0.5f));
        editProfileWindow.interactable = true;
        editProfileWindow.blocksRaycasts = true;
    }

    private IEnumerator RevealEditProfileWindow(float totalTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            editProfileWindow.alpha = Mathf.Lerp(0f, 1f, timer / totalTime);

            if (timer > totalTime)
                break;
            yield return null;
        }

        editProfileWindow.interactable = true;
        editProfileWindow.blocksRaycasts = true;
        editProfileWindow.alpha = 1f;
    }

    private IEnumerator HideEditProfileWindow(float totalTime)
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            editProfileWindow.alpha = Mathf.Lerp(1f, 0f, timer / totalTime);

            if (timer > totalTime)
                break;
            yield return null;
        }

        editProfileWindow.interactable = false;
        editProfileWindow.blocksRaycasts = false;
        editProfileWindow.alpha = 0f;
    }
}
