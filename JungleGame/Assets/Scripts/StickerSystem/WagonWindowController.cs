using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class WagonWindowController : MonoBehaviour
{
    public static WagonWindowController instance;

    public LerpableObject wagonBackground;
    public Button backButton;

    [Header("Stickers")]
    [SerializeField] private GameObject wagon;
    public GameObject Book, board, BackWindow, gecko;
    [SerializeField] private Animator Wagon;
    public Animator GeckoAnim;
    private bool stickerCartOut;
    private bool cartBusy;
    private bool stickerButtonsDisabled;

    public Transform cartStartPosition;
    public Transform cartOnScreenPosition;
    public Transform cartOffScreenPosition;

    [Header("Sticker Video Players")]
    public VideoPlayer commonVP;
    public VideoPlayer uncommonVP;
    public VideoPlayer rareVP;
    public VideoPlayer legendaryVP;


    [Header("Sticker Reveal")]
    public Transform revealSticker;
    
    public float hiddenRevealScale;
    public float maxRevealScale;
    public float normalRevealScale;

    public float longScaleRevealTime;
    public float shortScaleRevealTime;

    private bool waitingOnPlayerInput = false;

    [Header("Confirm Purchace Window")]
    [SerializeField] private GameObject confirmStickerWindow;
    public float hiddenScale;
    public float maxScale;
    public float normalScale;
    private bool confirmStickerWindowActive;

    public float longScaleTime;
    public float shortScaleTime;


    void Awake()
    {
        if (instance == null)
            instance = this;

        // sticker stuff
        Book.SetActive(false);
        board.SetActive(false);
        BackWindow.SetActive(false);

        // reset window
        confirmStickerWindow.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);

        // activate wagon
        wagon.gameObject.SetActive(true);

        // disable wagon background
        wagonBackground.SetImageAlpha(wagonBackground.GetComponent<Image>(), 0f);

        // hide back button
        backButton.interactable = false;
        backButton.GetComponent<LerpableObject>().SetImageAlpha(backButton.GetComponent<Image>(), 0f);
        backButton.transform.localScale = new Vector3(0f, 0f, 0f);

        // close buy board window
        buyBoardWindow.LerpScale(new Vector2(0f, 1f), 0.0001f);
    }

    void Update() 
    {
        if (waitingOnPlayerInput)
        {
            if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
            {
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);
                waitingOnPlayerInput = false;
            }
        }
    }

    /* 
    ################################################
    #   CONFIRM PURCHACE WINDOW METHODS
    ################################################
    */

    public void NewWindow()
    {
        // return if another window is up
        if (confirmStickerWindowActive)
            return;

        confirmStickerWindowActive = true;

        StartCoroutine(NewWindowRoutine());
    }

    public void OnYesButtonPressed()
    {
        // check to make sure player has sufficent funds
        if (StudentInfoSystem.GetCurrentProfile().goldCoins < 3)
        {
            // play sound
            AudioManager.instance.PlayCoinDrop();
            return;
        }
        // if they do, remove coins from player profile
        else 
        {
            DropdownToolbar.instance.RemoveGoldCoins(3);
        }

        if (backButton.isActiveAndEnabled)
        {
            // hide back button
            backButton.interactable = true;
            backButton.GetComponent<LerpableObject>().LerpImageAlpha(backButton.GetComponent<Image>(), 0f, 0.1f);
            backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.05f);
        }

        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        // hide window 
        StartCoroutine(ShrinkObject(confirmStickerWindow));

        // disable wagon background
        wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0f, 0.1f);

        // hide toolbar
        DropdownToolbar.instance.ToggleToolbar(false);

        StartCoroutine(RevealStickerRoutine());
    }

    private IEnumerator RevealStickerRoutine()
    {
        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // roll for a random sticker
        Sticker sticker = StickerDatabase.instance.RollForSticker();

        print ("you got a sticker! " + sticker.rarity + " " + sticker.id);

        // save sticker to SIS
        StudentInfoSystem.AddStickerToInventory(sticker);

        // fade to black
        FadeObject.instance.FadeOut(2f);
        yield return new WaitForSeconds(2f);
        
        // Hide GM canavas stuff
        SettingsManager.instance.ToggleMenuButtonActive(false);
        SettingsManager.instance.ToggleWagonButtonActive(false);
        wagon.SetActive(false);
        DefaultBackground.instance.Deactivate();

        yield return new WaitForSeconds(0.5f);

        float delay;
        // play correct video player
        switch (sticker.rarity)
        {
            default:
            case StickerRarity.Common:
                commonVP.Play();
                delay = (float)commonVP.length;
                break;

            case StickerRarity.Uncommon:
                uncommonVP.Play();
                delay = (float)uncommonVP.length;
                break;

            case StickerRarity.Rare:
                rareVP.Play();
                delay = (float)rareVP.length;
                break;

            case StickerRarity.Legendary:
                legendaryVP.Play();
                delay = (float)legendaryVP.length;
                break;
        }

        print ("playing video: " + sticker.rarity);
        print ("delay: " + delay);
        
        // Fade back in 
        FadeObject.instance.FadeIn(2f);

        yield return new WaitForSeconds(delay - 1.5f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");

        // reveal sticker here after certain amount of time
        StickerRevealCanvas.instance.RevealSticker(sticker);

        // wait for player input to continue
        waitingOnPlayerInput = true;
        while (waitingOnPlayerInput)
            yield return null;

        // activate raycast blocker
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerVideoBlocker");

        // fade to black
        FadeObject.instance.FadeOut(2f);
        yield return new WaitForSeconds(2f);
        
        // Reveal GM canavas stuff
        SettingsManager.instance.ToggleMenuButtonActive(true);
        SettingsManager.instance.ToggleWagonButtonActive(true);
        wagon.SetActive(true);
        GeckoAnim.Play("idle");
        DefaultBackground.instance.Activate();

        // stop correct video player
        switch (sticker.rarity)
        {
            default:
            case StickerRarity.Common:
                commonVP.Stop();
                break;

            case StickerRarity.Uncommon:
                uncommonVP.Stop();
                break;

            case StickerRarity.Rare:
                rareVP.Stop();
                break;

            case StickerRarity.Legendary:
                legendaryVP.Stop();
                break;
        }

        // hide reveal sticker
        StickerRevealCanvas.instance.HideSticker();
        yield return new WaitForSeconds(0.5f);

        // Fade back in 
        FadeObject.instance.FadeIn(2f);

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerVideoBlocker");
        
        if (backButton.isActiveAndEnabled)
        {
            // show back button
            backButton.interactable = true;
            backButton.GetComponent<LerpableObject>().LerpImageAlpha(backButton.GetComponent<Image>(), 1f, 0.1f);
            backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.01f);
        }

        confirmStickerWindowActive = false;

        // play lester tutorial if first sticker roll
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // wiggle sticker board
            Board.instance.isOn = true;
            Board.instance.GetComponent<WiggleController>().StartWiggle();

            // make gecko unselectable
            Gecko.instance.isOn = false;

            // StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
            // StudentInfoSystem.SaveStudentPlayerData();
        }
    }

    public void OnNoButtonPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        CloseConfirmStickerWindow();
    }

    private void CloseConfirmStickerWindow()
    {
        // hide window 
        StartCoroutine(ShrinkObject(confirmStickerWindow));

        // disable wagon background
        wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0f, 0.1f);

        // hide toolbar
        DropdownToolbar.instance.ToggleToolbar(false);

        confirmStickerWindowActive = false;
    }

    private IEnumerator NewWindowRoutine()
    {
        // show toolbar
        DropdownToolbar.instance.ToggleToolbar(true);

        // enable wagon background
        wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0.75f, 0.1f);

        yield return new WaitForSeconds(0.5f);

        // show window
        StartCoroutine(GrowObject(confirmStickerWindow));
    }

    private IEnumerator GrowObject(GameObject gameObject)
    {
        StartCoroutine(ScaleObjectRoutine(gameObject, longScaleTime, maxScale));
        yield return new WaitForSeconds(longScaleTime);
        StartCoroutine(ScaleObjectRoutine(gameObject, shortScaleTime, normalScale));
    }

    private IEnumerator ShrinkObject(GameObject gameObject)
    {
        StartCoroutine(ScaleObjectRoutine(gameObject, shortScaleTime, maxScale));
        yield return new WaitForSeconds(shortScaleTime);
        StartCoroutine(ScaleObjectRoutine(gameObject, longScaleTime, hiddenScale));

        yield return new WaitForSeconds(longScaleTime);

        // HOPE THIS DOESNT CAUSE PROBLEMS LATER
        //  - at the moment, the only game object that gets shrunken is the game 
        //    window, so we can reset the window afterwards.
        // reset window
        confirmStickerWindow.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
    }

    private IEnumerator ScaleObjectRoutine(GameObject gameObject, float time, float scale)
    {
        float start = gameObject.transform.localScale.x;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                gameObject.transform.localScale = new Vector3(scale, scale, 0f);
                break;
            }

            float temp = Mathf.Lerp(start, scale, timer / time);
            gameObject.transform.localScale = new Vector3(temp, temp, 0f);
            yield return null;
        }
    }

    /* 
    ################################################
    #   STICKER METHODS
    ################################################
    */

    private IEnumerator StickerInputDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stickerButtonsDisabled = false;
    }

    public void ToggleCart()
    {
        if (cartBusy)
            return;
        
        if (!stickerCartOut)
            StartCoroutine(RollOnScreen());
        else
            StartCoroutine(RollOffScreen());

        stickerCartOut = !stickerCartOut;
        cartBusy = true;
    }

    private IEnumerator RollOnScreen()
    {
        wagon.transform.position = cartStartPosition.position;

        // close settings menu if open
        SettingsManager.instance.CloseSettingsWindow();

        // disable nav buttons
        ScrollMapManager.instance.ToggleNavButtons(false);

        // disable ui buttons
        SettingsManager.instance.menuButton.interactable = false;
        SettingsManager.instance.wagonButton.interactable = false;

        // activate raycast blocker + background
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerCartBlocker");
        DefaultBackground.instance.Activate();

        Wagon.Play("WagonRollIn");
        StartCoroutine(RollToTargetRoutine(cartOnScreenPosition.position));
        yield return new WaitForSeconds(2.95f);
        Wagon.Play("WagonStop");
        yield return new WaitForSeconds(1f);
        Wagon.Play("Idle");

        Book.SetActive(true);
        board.SetActive(true);
        BackWindow.SetActive(true);

        // play lester talkies if first time
        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // turn off other buttons
            Board.instance.isOn = false;
            StickerBoardBook.instance.isOn = false;

            // remove back button so player cannot leave >:)
            backButton.gameObject.SetActive(false);

            // enable wagon background
            wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0.75f, 0.5f);

            // play lester intro 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.lester_intro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // disable wagon background
            wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0f, 0.5f);
        }

        gecko.SetActive(true);
        GeckoAnim.Play("geckoIntro");


        if (StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // show back button
            backButton.interactable = true;
            backButton.GetComponent<LerpableObject>().LerpImageAlpha(backButton.GetComponent<Image>(), 1f, 0.1f);
            backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.2f, 0.01f);
        }

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerCartBlocker");
        cartBusy = false;
    }

    private IEnumerator RollOffScreen()
    {
        // activate raycast blocker + background
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerCartBlocker");

        // hide back button
        backButton.interactable = true;
        backButton.GetComponent<LerpableObject>().LerpImageAlpha(backButton.GetComponent<Image>(), 0f, 0.1f);
        backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(0f, 0f), 0.1f, 0.05f);

        // gecko leave anim
        GeckoAnim.Play("geckoLeave");
        yield return new WaitForSeconds(1f);

        // remove windows
        if (buyBoardActive)
        {
            yield return new WaitForSeconds(0.5f);
            ToggleBuyBoardWindow();
        }

        if (confirmStickerWindowActive)
        {
            yield return new WaitForSeconds(0.5f);
            CloseBuyBoardWindowRoutine();
        }

        // roll off screen
        Wagon.Play("WagonRollIn");
        Book.SetActive(false);
        board.SetActive(false);
        BackWindow.SetActive(false);
        gecko.SetActive(false);

        StartCoroutine(RollToTargetRoutine(cartOffScreenPosition.position));
        yield return new WaitForSeconds(3f);
        // return cart to start pos
        wagon.transform.position = cartStartPosition.position;
        Wagon.Play("Idle");

        // deactivate raycast blocker + background
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerCartBlocker");
        DefaultBackground.instance.Deactivate();
        cartBusy = false;

        // enable nav buttons
        ScrollMapManager.instance.ToggleNavButtons(true);

        // enable ui buttons
        SettingsManager.instance.menuButton.interactable = true;
        SettingsManager.instance.wagonButton.interactable = true;

        // check for scroll map game events
        ScrollMapManager.instance.CheckForGameEvent(StudentInfoSystem.GetCurrentProfile().currStoryBeat);
    }

    public void ResetWagonController()
    {
        Book.SetActive(false);
        board.SetActive(false);
        BackWindow.SetActive(false);
        gecko.SetActive(false);

        wagon.transform.position = cartStartPosition.position;
        Wagon.Play("Idle");

        // deactivate raycast blocker + background
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerCartBlocker");
        cartBusy = false;
        DefaultBackground.instance.Deactivate();

        // enable nav buttons
        if (SceneManager.GetActiveScene().name == "ScrollMap")
            ScrollMapManager.instance.ToggleNavButtons(true);

        // hide window 
        StartCoroutine(ShrinkObject(confirmStickerWindow));
    }

    public void OnBackButtonPressed() // ORDER MATTERS HERE!!!
    {
        // close inventory
        if (StickerBoardController.instance.GetCurrentStickerBoard().stickerInventoryActive) 
        {
            StickerBoardController.instance.ToggleInventoryWindow();
        }
        // leave sticker board menu
        else if (StickerBoardController.instance.stickerBoardActive)
        {
            StickerBoardController.instance.ToggleStickerBoardWindow();
            return;
        }
        // leave buy board screen
        else if (buyBoardActive)
        {
            ToggleBuyBoardWindow();
        }
        else if (confirmStickerWindowActive)
        {
            CloseConfirmStickerWindow();
        }
        // leave cart
        else 
        {
            ToggleCart();
        }
    }

    private IEnumerator RollToTargetRoutine(Vector3 target)
    {
        Vector3 currStart = wagon.transform.position;
        float timer = 0f;
        float maxTime = .75f;

        while (true)
        {
            // animate movement
            timer += Time.deltaTime * .25f;
            if (timer < maxTime)
            {
                wagon.transform.position = Vector3.Lerp(currStart, target, timer / maxTime);
            }
            else
            {
                wagon.transform.position = target;
                yield break;
            }
            
            yield return null;
        }
    }

    private IEnumerator CartRaycastBlockerRoutine(bool opt)
    {
        float start, end;
        if (opt)
        {
            start = 0f;
            end = 0.75f;
        }
        else
        {
            start = 0.75f;
            end = 0f;
        }
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                break;
            }

            float tempAlpha = Mathf.Lerp(start, end, timer / 1f);
            yield return null;
        }
    }

    /* 
    ################################################
    #   BUY BOARD METHODS
    ################################################
    */

    public LerpableObject buyBoardWindow;

    private bool buyBoardActive = false;
    private bool buyBoardReady = true;

    public void ToggleBuyBoardWindow()
    {
        if (!buyBoardReady)
            return;

        buyBoardActive = !buyBoardActive;
        buyBoardReady = false;

        // open window
        if (buyBoardActive)
        {
            StartCoroutine(OpenBuyBoardWindowRoutine());
            Gecko.instance.isOn = false;
            Board.instance.isOn = false;
        }
        // close window
        else
        {
            StartCoroutine(CloseBuyBoardWindowRoutine());
            Gecko.instance.isOn = true;
            Board.instance.isOn = true;
        }
    }

    private IEnumerator OpenBuyBoardWindowRoutine()
    {
        buyBoardWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);

        // enable wagon background
        wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0.75f, 0.1f);

        if (!StudentInfoSystem.GetCurrentProfile().stickerTutorial)
        {
            // stop wiggling the book
            StickerBoardBook.instance.GetComponent<WiggleController>().StopWiggle();

            // play lester intro 5
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.lester_intro_5);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // make gecko selectable
            Gecko.instance.isOn = true;
            WagonWindowController.instance.gecko.SetActive(true);
            WagonWindowController.instance.GeckoAnim.Play("geckoIntro");

            // make other things selectable
            Board.instance.isOn = true;
            StickerBoardBook.instance.isOn = true;

            // enable back button
            backButton.gameObject.SetActive(true);
            backButton.interactable = true;
            backButton.GetComponent<LerpableObject>().LerpImageAlpha(backButton.GetComponent<Image>(), 1f, 0.1f);
            backButton.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.1f, 1.1f), new Vector2(1f, 1f), 0.1f, 0.05f);

            // save to SIS
            StudentInfoSystem.GetCurrentProfile().stickerTutorial = true;
            StudentInfoSystem.SaveStudentPlayerData();
        }

        yield return new WaitForSeconds(0.5f);

        buyBoardReady = true;
    }

    private IEnumerator CloseBuyBoardWindowRoutine()
    {
        buyBoardWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);

        // disable wagon background
        wagonBackground.LerpImageAlpha(wagonBackground.GetComponent<Image>(), 0f, 0.1f);

        yield return new WaitForSeconds(0.5f);

        buyBoardReady = true;
    }
}
