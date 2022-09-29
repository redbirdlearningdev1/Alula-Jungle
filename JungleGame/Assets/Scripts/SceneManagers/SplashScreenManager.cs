using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;


public class SplashScreenManager : MonoBehaviour
{
    public static SplashScreenManager instance;

    public Animator mainAnimator;

    [Header("Splash Screen BGs")]
    public Image BG1_image;
    public Image BG2_image;
    public Image BG3_image;
    public VideoPlayer BG4_player;
    public Image BG5_image;
    public VideoPlayer BG6_player;
    public Image BG6_image;
    public Image BG7_image;

    [Header("Addressable References")]
    [SerializeField] AssetReferenceAtlasedSprite BGImage_1BG;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_2Back;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_3BackCh0;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_3BackCh1;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_5MidFront;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_6Front;
    [SerializeField] AssetReferenceAtlasedSprite BGImage_7Front;
    [SerializeField] AssetReference BGVideo_4MidCh0;
    [SerializeField] AssetReference BGVideo_4MidCh3;
    [SerializeField] AssetReference BGVideo_4MidCh4;
    [SerializeField] AssetReference BGVideo_4MidCh6;
    [SerializeField] AssetReference BGVideo_6FrontCh2;
    [SerializeField] AssetReference BGVideo_6FrontCh5;

    [Header("Handles")]
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BG1 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BG2 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BG3 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());
    ReferenceObj<AsyncOperationHandle<VideoClip>> handle_BG4 = new ReferenceObj<AsyncOperationHandle<VideoClip>>(new AsyncOperationHandle<VideoClip>());
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BG5 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());
    ReferenceObj<AsyncOperationHandle<VideoClip>> handle_BGVideo6 = new ReferenceObj<AsyncOperationHandle<VideoClip>>(new AsyncOperationHandle<VideoClip>());
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BGImage6 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());
    ReferenceObj<AsyncOperationHandle<Sprite>> handle_BG7 = new ReferenceObj<AsyncOperationHandle<Sprite>>(new AsyncOperationHandle<Sprite>());

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

    public LerpableObject reportButton;
    private Vector3 reportButtonPos;

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

    [Header("Addressables")]
    Dictionary<int, AsyncOperationHandle<Sprite>> avatarHandles = new Dictionary<int, AsyncOperationHandle<Sprite>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void UnloadAvatar(int profileIndex)
    {
        if (avatarHandles.ContainsKey(profileIndex))
        {
            AsyncOperationHandle<Sprite> avatarHandle = avatarHandles[profileIndex];
            Addressables.Release(avatarHandle);
            avatarHandles.Remove(profileIndex);
        }
    }

    public void UnloadAllAvatars()
    {
        foreach (KeyValuePair<int, AsyncOperationHandle<Sprite>> pair in avatarHandles)
        {
            AsyncOperationHandle<Sprite> avatarHandle = pair.Value;
            Addressables.Release(avatarHandle);
        }
        avatarHandles.Clear();
    }

    IEnumerator LoadAvatar(Image imageToSet, int profileIndex, Image editImage = null)
    {
        AsyncOperationHandle<Sprite> avatarHandle;
        if (avatarHandles.ContainsKey(profileIndex))
        {
            avatarHandle = avatarHandles[profileIndex];
        }
        else
        {
            avatarHandle = GameManager.instance.avatars[profileIndex].LoadAssetAsync<Sprite>();
            avatarHandles.Add(profileIndex, avatarHandle);
        }

        yield return avatarHandle;

        imageToSet.sprite = avatarHandle.Result;

        if (editImage != null)
        {
            editImage.sprite = avatarHandle.Result;
        }
    }

    void Start()
    {
        Addressables.InitializeAsync();
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0f);
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
        //StartCoroutine(LoadAndPlayVideo(BG3_player, BGVideo_4MidCh6));
        //BG3_animator.Play("3_Ch0");
        //BG4_animator.Play("4_Ch0");
        //BG6_animator.Play("6_Ch0");
        UnloadAllAvatars();
        // set correct chapter animations


        if (currProfile != null)
        {
            // get most recent chapter
            Chapter chapter = currProfile.currentChapter;

            StartCoroutine(LoadSplashImage(BG1_image, BGImage_1BG, handle_BG1));
            StartCoroutine(LoadSplashImage(BG2_image, BGImage_2Back, handle_BG2));
            StartCoroutine(LoadSplashImage(BG5_image, BGImage_5MidFront, handle_BG5));
            StartCoroutine(LoadSplashImage(BG7_image, BGImage_7Front, handle_BG7));
            BG6_player.enabled = true;
            BG6_player.GetComponent<RawImage>().enabled = true;
            BG6_image.enabled = false;

            switch (chapter)
            {
                case Chapter.chapter_0:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh0, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh0, handle_BG4));
                    BG6_player.enabled = false;
                    BG6_player.GetComponent<RawImage>().enabled = false;
                    BG6_image.enabled = true;
                    StartCoroutine(LoadSplashImage(BG6_image, BGImage_6Front, handle_BGImage6));
                    break;
                case Chapter.chapter_1:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh0, handle_BG4));
                    BG6_player.enabled = false;
                    BG6_player.GetComponent<RawImage>().enabled = false;
                    BG6_image.enabled = true;
                    StartCoroutine(LoadSplashImage(BG6_image, BGImage_6Front, handle_BGImage6));
                    break;
                case Chapter.chapter_2:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh0, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh2, handle_BGVideo6));
                    break;
                case Chapter.chapter_3:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh3, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh2, handle_BGVideo6));
                    break;
                case Chapter.chapter_4:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh4, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh2, handle_BGVideo6));
                    break;
                case Chapter.chapter_5:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh4, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh5, handle_BGVideo6));
                    break;
                case Chapter.chapter_6:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh6, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh5, handle_BGVideo6));
                    break;
                default:
                    StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh1, handle_BG3));
                    StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh6, handle_BG4));
                    StartCoroutine(LoadAndPlayVideo(BG6_player, BGVideo_6FrontCh5, handle_BGVideo6));
                    break;
            }
        }
        else
        {
            StartCoroutine(LoadSplashImage(BG1_image, BGImage_1BG, handle_BG1));
            StartCoroutine(LoadSplashImage(BG2_image, BGImage_2Back, handle_BG2));
            StartCoroutine(LoadSplashImage(BG3_image, BGImage_3BackCh0, handle_BG3));
            StartCoroutine(LoadAndPlayVideo(BG4_player, BGVideo_4MidCh0, handle_BG4));
            StartCoroutine(LoadSplashImage(BG5_image, BGImage_5MidFront, handle_BG5));
            BG6_player.enabled = false;
            BG6_player.GetComponent<RawImage>().enabled = false;
            BG6_image.enabled = true;
            StartCoroutine(LoadSplashImage(BG6_image, BGImage_6Front, handle_BGImage6));
            StartCoroutine(LoadSplashImage(BG7_image, BGImage_7Front, handle_BG7));

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
        // StartCoroutine(LoadAvatar(selectedProfileImage, profileAvatarIndex, editProfileImage));

        // move practice button off-screen
        practiceButtonPos = practiceButton.transform.localPosition;
        practiceButton.transform.localPosition = new Vector3(practiceButtonPos.x, practiceButtonPos.y + 150f, 0f);

        // move report button off-screen
        reportButtonPos = reportButton.transform.localPosition;
        reportButton.transform.localPosition = new Vector3(reportButtonPos.x, reportButtonPos.y + 150f, 0f);

        // start screen tap delay
        StartCoroutine(ScreenTapDelay());

        SetUpProfiles();
    }

    private IEnumerator LoadAndPlayVideo(VideoPlayer player, AssetReference video, ReferenceObj<AsyncOperationHandle<VideoClip>> handle)
    {
        handle.Value = video.LoadAssetAsync<VideoClip>();
        yield return handle.Value;
        player.clip = handle.Value.Result;

        player.Play();
    }
    private IEnumerator LoadSplashImage(Image image, AssetReferenceAtlasedSprite spriteRef, ReferenceObj<AsyncOperationHandle<Sprite>> handle)
    {
        handle.Value = spriteRef.LoadAssetAsync<Sprite>();
        yield return handle.Value;
        image.sprite = handle.Value.Result;
    }

    private void UnloadSplashScreen()
    {
        if (handle_BG1.Value.IsValid())
        {
            Addressables.Release(handle_BG1.Value);
            BG1_image.sprite = null;
        }
        if (handle_BG2.Value.IsValid())
        {
            Addressables.Release(handle_BG2.Value);
            BG2_image.sprite = null;
        }
        if (handle_BG3.Value.IsValid())
        {
            Addressables.Release(handle_BG3.Value);
            BG3_image.sprite = null;
        }
        if (handle_BG4.Value.IsValid())
        {
            Addressables.Release(handle_BG4.Value);
            BG4_player.clip = null;
        }
        if (handle_BG5.Value.IsValid())
        {
            Addressables.Release(handle_BG5.Value);
            BG5_image.sprite = null;
        }
        if (handle_BGVideo6.Value.IsValid())
        {
            Addressables.Release(handle_BGVideo6.Value);
            BG6_player.clip = null;
        }
        if (handle_BGImage6.Value.IsValid())
        {
            Addressables.Release(handle_BGImage6.Value);
            BG6_image.sprite = null;
        }
        if (handle_BG7.Value.IsValid())
        {
            Addressables.Release(handle_BG7.Value);
            BG7_image.sprite = null;
        }
    }

    private IEnumerator ScreenTapDelay()
    {
        //Resources.UnloadUnusedAssets();
        yield return new WaitForSeconds(3f);
        screenTapReady = true;
    }

    void Update()
    {
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !screenTapped && screenTapReady)
        {
            screenTapped = true;
            //Debug.LogError("Video Clip: " + BG4_player.clip.name);
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
            StartCoroutine(LoadAvatar(profile1Image, data1.profileAvatar));
            //profile1Image.sprite = GameManager.instance.avatars[data1.profileAvatar];
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
            StartCoroutine(LoadAvatar(profile2Image, data2.profileAvatar));
            //profile2Image.sprite = GameManager.instance.avatars[data2.profileAvatar];
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
            StartCoroutine(LoadAvatar(profile3Image, data3.profileAvatar));
            //profile3Image.sprite = GameManager.instance.avatars[data3.profileAvatar];
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
        yield return new WaitForSeconds(0.2f);
        SetUpProfiles();
        yield return new WaitForSeconds(2.8f);
        UnloadSplashScreen();

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

        // enable practice / report button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
            reportButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
            reportButton.GetComponent<Button>().interactable = false;
        }

        // show practice button
        practiceButton.LerpYPos(practiceButtonPos.y - 50, 0.2f, true);
        reportButton.LerpYPos(reportButtonPos.y - 50, 0.2f, true);
        yield return new WaitForSeconds(0.2f);
        practiceButton.LerpYPos(practiceButtonPos.y, 0.2f, true);
        reportButton.LerpYPos(reportButtonPos.y, 0.2f, true);
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
        StudentInfoSystem.GetCurrentProfile().active = true; // this profile is now active!!
        StudentInfoSystem.SaveStudentPlayerData();

        // update analytics profile
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        string profile = data.name + "_" + data.uniqueID;
        AnalyticsManager.SwitchProfile(profile);

        SetUpProfiles();
        selectedProfile = 0;
        SelectProfile((int)newProfileIndex + 1);


        // turn on start button
        startButton.interactable = true;
        startbuttonBox.sprite = boxGreen;
        startbuttonText.sprite = textWhite;

        profileSelectWindow.interactable = true;
        profileSelectWindow.blocksRaycasts = true;

        // enable practice / report button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
            reportButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
            reportButton.GetComponent<Button>().interactable = false;
        }

        //// ANALYTICS : send profile_created event
        // StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "avatar_id", profileAvatarIndex },
            { "game_version", GameManager.currentGameVersion },
            { "profile_name", newProfileName },
            { "student_index", ((int)newProfileIndex + 1) },
            { "unique_id", data.uniqueID }
        };            
        AnalyticsManager.SendCustomEvent("profile_created", parameters);
    }

    private void LoadProfileAndContinue(StudentIndex index)
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.HappyBlip, 1f);
        // set profile to be current player
        StudentInfoSystem.SetStudentPlayer(index);
        // load scroll map scene
        UnloadAllAvatars();
        GameManager.instance.LoadScene("ScrollMap", true, 0.5f, true);
    }

    public void SelectProfile(int profileNum)
    {
        StartCoroutine(LoadAvatar(selectedProfileImage, profileAvatarIndex));
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

        if (profileAvatarIndex != data1.profileAvatar && profileAvatarIndex != data2.profileAvatar && profileAvatarIndex != data3.profileAvatar)
        {
            UnloadAvatar(profileAvatarIndex);
        }

        // reduce index, iff less than 0, return to max number
        profileAvatarIndex--;
        if (profileAvatarIndex < 0)
            profileAvatarIndex = GameManager.instance.avatars.Count - 1;


        StartCoroutine(LoadAvatar(selectedProfileImage, profileAvatarIndex, editProfileImage));
        //selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];
        //editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

        selectedProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
        editProfileImage.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.9f, 0.9f), Vector2.one, 0.1f, 0.1f);
    }

    public void OnRightArrowPressed()
    {
        // play audio blip
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        if (profileAvatarIndex != data1.profileAvatar && profileAvatarIndex != data2.profileAvatar && profileAvatarIndex != data3.profileAvatar)
        {
            UnloadAvatar(profileAvatarIndex);
        }

        // increase index, iff greater than max, return to 0
        profileAvatarIndex++;
        if (profileAvatarIndex >= GameManager.instance.avatars.Count)
            profileAvatarIndex = 0;

        StartCoroutine(LoadAvatar(selectedProfileImage, profileAvatarIndex, editProfileImage));
        //selectedProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];
        //editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

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
        StartCoroutine(LoadAvatar(editProfileImage, profileAvatarIndex));
        //editProfileImage.sprite = GameManager.instance.avatars[profileAvatarIndex];

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
        
        // update analytics profile
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        string profile = data.name + "_" + data.uniqueID;
        AnalyticsManager.SwitchProfile(profile);

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

        // enable practice / report button if profiles exist
        if (data1.active || data2.active || data3.active)
        {
            practiceButton.GetComponent<Button>().interactable = true;
            reportButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            practiceButton.GetComponent<Button>().interactable = false;
            reportButton.GetComponent<Button>().interactable = false;
        }

        // update analytics profile
        StudentPlayerData data = StudentInfoSystem.GetCurrentProfile();
        string profile = data.name + "_" + data.uniqueID;
        AnalyticsManager.SwitchProfile(profile);
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
        UnloadAllAvatars();
        GameManager.instance.LoadScene("PracticeScene", true);
    }

    public void OnReportButtonPressed()
    {
        UnloadAllAvatars();
        GameManager.instance.LoadScene("ReportScene", true);
    }
}
