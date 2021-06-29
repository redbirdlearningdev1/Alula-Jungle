using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] TextMeshProUGUI profile1Text;
    [SerializeField] TextMeshProUGUI profile2Text;
    [SerializeField] TextMeshProUGUI profile3Text;

    private StudentPlayerData data1;
    private StudentPlayerData data2;
    private StudentPlayerData data3;

    private int selectedProfile = 0;
    private bool screenTapped = false;

    public float selectedAlpha;
    public float unselectedAlpha;

    public Sprite selectedSprite;

    void Start() 
    {
        // every scene must call this in Awake()
        GameManager.instance.SceneInit();

        // play music
        AudioManager.instance.PlaySong(AudioDatabase.instance.MainThemeSong);

        // set up profile window
        startButton.interactable = false;
        SetImageAlpha(profile1Image, unselectedAlpha);
        SetImageAlpha(profile2Image, unselectedAlpha);
        SetImageAlpha(profile3Image, unselectedAlpha);
        profileSelectWindow.interactable = false;
        profileSelectWindow.blocksRaycasts = false;
        profileSelectWindow.alpha = 0f;

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

    void Update() 
    {
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !screenTapped)
        {
            screenTapped = true;
            StartCoroutine(RevealProfileWindow(0.5f));
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

        // load selected profile or make new profile if inactive
        switch (selectedProfile)
        {
            case 1:
                if (data1.active)
                {
                    // load profile
                    StudentInfoSystem.SetStudentPlayer(StudentIndex.student_1);
                    GameManager.instance.LoadScene("JungleWelcomeScene", true);
                }
                else
                {
                    // create profile
                }
                break;
            case 2:
                if (data2.active)
                {
                    // load profile
                    StudentInfoSystem.SetStudentPlayer(StudentIndex.student_2);
                    GameManager.instance.LoadScene("JungleWelcomeScene", true);
                }
                else
                {
                    // create profile
                }
                break;
            case 3:
                if (data3.active)
                {
                    // load profile
                    StudentInfoSystem.SetStudentPlayer(StudentIndex.student_3);
                    GameManager.instance.LoadScene("JungleWelcomeScene", true);
                }
                else
                {
                    // create profile
                }
                break;
        }

       
    }

    public void SelectProfile(int profileNum)
    {
        if (profileNum <= 0 || profileNum > 3)
            return;
        
        selectedProfile = profileNum;

        // make start button interactabl
        if (!startButton.interactable)
            startButton.interactable = true;

        SetImageAlpha(profile1Image, unselectedAlpha);
        SetImageAlpha(profile2Image, unselectedAlpha);
        SetImageAlpha(profile3Image, unselectedAlpha);

        switch (profileNum)
        {
            case 1:
                SetImageAlpha(profile1Image, selectedAlpha);
                break;
            case 2:
                SetImageAlpha(profile2Image, selectedAlpha);
                break;
            case 3:
                SetImageAlpha(profile3Image, selectedAlpha);
                break;
        }
    }
}
