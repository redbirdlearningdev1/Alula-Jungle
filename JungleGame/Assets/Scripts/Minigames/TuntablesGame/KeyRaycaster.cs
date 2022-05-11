using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class KeyRaycaster : MonoBehaviour
{
    public static KeyRaycaster instance;
    public bool isOn = false;
    public Transform selectedKeyParent;

    // private variables
    private Key selectedKey;
    private bool playedKeyTutorialPart = false;

    // move speeds
    public float moveSpeed;

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
        if (Input.GetMouseButton(0) && selectedKey)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedKey.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - moveSpeed, Time.deltaTime * 60));
            selectedKey.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedKey)
        {
            // send raycast to check for rock lock
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("RockLock"))
                    {
                        isCorrect = TurntablesGameManager.instance.EvaluateKey(selectedKey);
                    }
                }
            }

            // reset lock rock
            TurntablesGameManager.instance.rockLock.LerpScale(new Vector2(1f, 1f), 0.2f);
            TurntablesGameManager.instance.rockLock.GetComponent<WiggleController>().StopWiggle();
            TurntablesGameManager.instance.rockLock.GetComponent<RockLock>().ToggleGlow(false);

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

            // move key into rock lock if correct
            if (isCorrect)
                selectedKey.MoveIntoRockLock();
            else
                selectedKey.ReturnToRope();

            selectedKey = null;
        }


        // on pointer down
        if (Input.GetMouseButtonDown(0))
        {
            // return if already selected a key
            if (selectedKey)
                return;

            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Key"))
                    {
                        if (!result.gameObject.GetComponentInParent<Key>().interactable)
                            return;

                        selectedKey = result.gameObject.GetComponentInParent<Key>();
                        selectedKey.interactable = false;
                        selectedKey.PlayAudio();

                        // play tutorial intro 3 if tutorial
                        if (TurntablesGameManager.instance.playTutorial && !playedKeyTutorialPart)
                        {
                            playedKeyTutorialPart = true;

                            StartCoroutine(TutorialPopupRoutine());
                            return;
                        }
                        // set new pivot
                        selectedKey.GetComponent<RectTransform>().pivot = selectedKey.grabPivot;
                        // set parent
                        selectedKey.gameObject.transform.SetParent(selectedKeyParent);
                        // make key larger
                        selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1.5f, 1.5f), 0.2f);

                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        // show lock rock
                        TurntablesGameManager.instance.rockLock.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                        TurntablesGameManager.instance.rockLock.GetComponent<WiggleController>().StartWiggle();
                        TurntablesGameManager.instance.rockLock.GetComponent<RockLock>().ToggleGlow(true);

                        return;
                    }
                }
            }
        }
    }

    private IEnumerator TutorialPopupRoutine()
    {
        isOn = false;
        selectedKey.PlayAudio();

        yield return new WaitForSeconds(1f);

        // play tutorial audio 3
        AssetReference clip = GameIntroDatabase.instance.turntablesIntro3;
        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clip));
        yield return cd.coroutine;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Red, clip);
        yield return new WaitForSeconds(cd.GetResult() + 1f);

        // reset key
        selectedKey.ReturnToRope();
        selectedKey = null;

        isOn = true;
    }
}
