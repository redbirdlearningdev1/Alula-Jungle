using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class SpiderRayCaster : MonoBehaviour
{
    public static SpiderRayCaster instance;

    public bool isOn = false;
    private UniversalCoinImage selectedCoin = null;
    private BugController selectedBug = null;
    public float moveSpeed;
    [SerializeField] private Transform selectedCoinParent;
    [SerializeField] private WebBall webBallGlow;

    private bool playTutorialPart = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // return if settings window is open
        if (SettingsManager.instance.settingsWindowOpen)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedCoin)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedCoin.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - moveSpeed, Time.deltaTime * 60));
            selectedCoin.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedCoin)
        {

            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Bag"))
                    {
                        NewSpiderGameManager.instance.EvaluateSelectedSpiderCoin(ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value), selectedCoin);
                    }
                }
            }

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

            NewSpiderGameManager.instance.ReturnCoinsToPosition();

            // grow webball
            WebBall.instance.GetComponent<LerpableObject>().LerpScale(Vector2.one, 0.2f);
            WebBall.instance.GetComponent<WiggleController>().StopWiggle();

            selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
            selectedCoin = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        selectedCoin = result.gameObject.GetComponent<UniversalCoinImage>();
                        selectedCoin.gameObject.transform.SetParent(selectedCoinParent);
                        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.25f, 1.25f), 0.25f);

                        // grow webball
                        WebBall.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                        WebBall.instance.GetComponent<WiggleController>().StartWiggle();
                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedBug = result.gameObject.GetComponent<BugController>();

                        if (NewSpiderGameManager.instance.playTutorial && NewSpiderGameManager.instance.tutorialEvent == 1 && !playTutorialPart)
                        {
                            playTutorialPart = true;
                            StartCoroutine(NextSpiderwebTutorialPart());
                        }
                        else
                        {
                            selectedBug.PlayPhonemeAudio();
                        }
                    }
                }
            }
        }
    }

    private IEnumerator NextSpiderwebTutorialPart()
    {
        // turn off raycaster
        isOn = false;
        // remove glow
        selectedBug.ToggleGlow(false);

        selectedBug.PlayPhonemeAudio();

        yield return new WaitForSeconds(1f);

        // play tutorial audio
        List<AssetReference> clips = new List<AssetReference>();
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro3);
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro4);
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clips);
        yield return new WaitForSeconds(10f);

        NewSpiderGameManager.instance.ContinueTutorialPart();
    }
}