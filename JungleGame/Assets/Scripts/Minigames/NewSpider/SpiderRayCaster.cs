using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiderRayCaster : MonoBehaviour
{
    public static SpiderRayCaster instance;

    public bool isOn = false;
    private UniversalCoinImage selectedCoin = null;
    private BugController selectedBug = null;
    [SerializeField] private Transform selectedCoinParent;
    [SerializeField] private WebBall webBallGlow;

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

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedCoin)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedCoin.transform.position = mousePosWorldSpace;

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

            webBallGlow.chestGlowNo();
            NewSpiderGameManager.instance.ReturnCoinsToPosition();

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
                        webBallGlow.chestGlow();
                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedBug = result.gameObject.GetComponent<BugController>();

                        if (NewSpiderGameManager.instance.playTutorial && NewSpiderGameManager.instance.tutorialEvent == 1)
                        {
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
        ImageGlowController.instance.SetImageGlow(BugController.instance.image, false);

        selectedBug.PlayPhonemeAudio();

        yield return new WaitForSeconds(1f);

        // play tutorial audio
        List<AudioClip> clips = new List<AudioClip>();
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro3);
        clips.Add(GameIntroDatabase.instance.spiderwebsIntro4);
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomRight.position, false, TalkieCharacter.Spindle, clips);
        yield return new WaitForSeconds(clips[0].length + clips[1].length + 1f);
        
        NewSpiderGameManager.instance.ContinueTutorialPart();
    }
}