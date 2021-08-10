using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonWindowController : MonoBehaviour
{
    public static WagonWindowController instance;

    [Header("Stickers")]
    [SerializeField] private GameObject wagon;
    [SerializeField] private GameObject Book, Board, BackWindow, Gecko;
    [SerializeField] private Animator Wagon;
    [SerializeField] private Animator GeckoAnim;
    private bool stickerCartOut;
    private bool cartBusy;
    private bool stickerButtonsDisabled;

    public Transform cartStartPosition;
    public Transform cartOnScreenPosition;
    public Transform cartOffScreenPosition;

    [Header("Confirm Purchace Window")]
    [SerializeField] private GameObject window;
    public float hiddenScale;
    public float maxScale;
    public float normalScale;
    private bool windowUp;

    public float longScaleTime;
    public float shortScaleTime;


    void Awake()
    {
        if (instance == null)
            instance = this;

        // sticker stuff
        Book.SetActive(false);
        Board.SetActive(false);
        BackWindow.SetActive(false);

        // reset window
        window.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
    }

    /* 
    ################################################
    #   CONFIRM PURCHACE WINDOW METHODS
    ################################################
    */

    public void NewWindow()
    {
        // return if another window is up
        if (windowUp)
            return;

        windowUp = true;
        StartCoroutine(NewWindowRoutine());
    }

    public void OnYesButtonPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        windowUp = false;
    }

    public void OnNoButtonPressed()
    {
        // play sound
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

        windowUp = false;
        // hide window 
        StartCoroutine(ShrinkObject(window));

        // remove default background and raycast blocker
        DefaultBackground.instance.Deactivate();
    }

    private IEnumerator NewWindowRoutine()
    {
        // add default background and raycast blocker
        DefaultBackground.instance.Activate();

        yield return new WaitForSeconds(0.5f);

        // show window
        StartCoroutine(GrowObject(window));
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
        window.transform.localScale = new Vector3(hiddenScale, hiddenScale, 0f);
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
        print ("rolling in!");
        wagon.transform.position = cartStartPosition.position;

        // disable nav buttons
        ScrollMapManager.instance.TurnOffNavigationUI(true);
        
        // activate raycast blocker + background
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerCartBlocker");
        BehindUIBackground.instance.Activate();

        Wagon.Play("WagonRollIn");
        StartCoroutine(RollToTargetRoutine(cartOnScreenPosition.position));
        yield return new WaitForSeconds(2.95f);
        Wagon.Play("WagonStop");
        yield return new WaitForSeconds(1f);
        Wagon.Play("Idle");

        Book.SetActive(true);
        Board.SetActive(true);
        BackWindow.SetActive(true);
        Gecko.SetActive(true);
        GeckoAnim.Play("geckoIntro");

        // deactivate raycast blocker
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerCartBlocker");
        cartBusy = false;
    }

    private IEnumerator RollOffScreen()
    {
        print ("rolling off!");

        // activate raycast blocker + background
        RaycastBlockerController.instance.CreateRaycastBlocker("StickerCartBlocker");

        // roll off screen
        Wagon.Play("WagonRollIn");
        Book.SetActive(false);
        Board.SetActive(false);
        BackWindow.SetActive(false);
        Gecko.SetActive(false);

        StartCoroutine(RollToTargetRoutine(cartOffScreenPosition.position));
        yield return new WaitForSeconds(3f);
        // return cart to start pos
        wagon.transform.position = cartStartPosition.position;
        Wagon.Play("Idle");

        // deactivate raycast blocker + background
        RaycastBlockerController.instance.RemoveRaycastBlocker("StickerCartBlocker");
        BehindUIBackground.instance.Deactivate();
        cartBusy = false;

        // enable nav buttons
        ScrollMapManager.instance.TurnOffNavigationUI(false);
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
}
