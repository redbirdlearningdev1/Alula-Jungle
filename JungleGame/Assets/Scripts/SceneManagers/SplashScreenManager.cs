using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Splash Screen BGs")]
    public Animator BG3_animator;
    public Animator BG4_animator;
    public Animator BG6_animator;

    [Header("Profile Windows")]
    [SerializeField] CanvasGroup profileSelectWindow;
    [SerializeField] CanvasGroup newProfileWindow;

    [SerializeField] Button startButton;
    [SerializeField] Button newProfileButton;

    [SerializeField] Button profile1Button;
    [SerializeField] Button profile2Button;
    [SerializeField] Button profile3Button;

    [SerializeField] Image profile1Image;
    [SerializeField] Image profile2Image;
    [SerializeField] Image profile3Image;

    [SerializeField] TextMeshProUGUI profile1Text;
    [SerializeField] TextMeshProUGUI profile2Text;
    [SerializeField] TextMeshProUGUI profile3Text;

    [SerializeField] TMP_InputField newProfileInput;

    [SerializeField] WiggleController tapTextWiggleController;
    public TextMeshProUGUI tapText;

    private StudentPlayerData data1;
    private StudentPlayerData data2;
    private StudentPlayerData data3;

    private int selectedProfile = 0;
    private bool screenTapped = false;

    public float selectedAlpha;
    public float unselectedAlpha;

    public float selectedScale;
    public float unselectedScale;

    public float lerpSpeed;

    public Sprite selectedSprite;

    private StudentIndex newProfileIndex;

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

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

            if (chapter > Chapter.chapter_6)
            {
                BG4_animator.Play("4_Ch6");
            }
        }

        // set up profile window
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "select profile";
        SetImageAlpha(profile1Image, unselectedAlpha);
        SetImageAlpha(profile2Image, unselectedAlpha);
        SetImageAlpha(profile3Image, unselectedAlpha);
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        profileSelectWindow.alpha = 0f;

        // set up new profile window
        newProfileWindow.interactable = false;
        newProfileWindow.blocksRaycasts = false;
        newProfileWindow.alpha = 0f;

        // wiggle text controller
        tapTextWiggleController.StartWiggle();

        SetUpProfiles();
    }

    void Update() 
    {
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !screenTapped)
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
            profile1Image.sprite = selectedSprite; // TODO: custom icon selected
        }

        // profile 2
        data2 = StudentInfoSystem.GetStudentData(StudentIndex.student_2);
        if (data2.active)
        {
            profile2Text.text = data2.name;
            profile2Image.sprite = selectedSprite; // TODO: custom icon selected
        }

        // profile 3
        data3 = StudentInfoSystem.GetStudentData(StudentIndex.student_3);
        if (data3.active)
        {
            profile3Text.text = data3.name;
            profile3Image.sprite = selectedSprite; // TODO: custom icon selected
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
        StudentInfoSystem.SetStudentPlayer(newProfileIndex);
        StudentInfoSystem.GetCurrentProfile().name = newProfileInput.text;
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
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);
        // set profile to be current player
        StudentInfoSystem.SetStudentPlayer(index);
        // load scroll map scene
        GameManager.instance.LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void SelectProfile(int profileNum)
    {
        if (profileNum <= 0 || profileNum > 3)
            return;
        
        selectedProfile = profileNum;

        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // make start button interactabl
        if (!startButton.interactable)
        {
            startButton.interactable = true;
        }

        SetImageAlpha(profile1Image, unselectedAlpha);
        SetImageAlpha(profile2Image, unselectedAlpha);
        SetImageAlpha(profile3Image, unselectedAlpha);

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

        if (studentData.active)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "begin game!";
        }
        else
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "create new profile!";
        }
    }
}
