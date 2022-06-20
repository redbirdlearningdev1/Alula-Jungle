using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NewPasswordRaycaster : MonoBehaviour
{
    public static NewPasswordRaycaster instance;

    public bool isOn = false;
    public float moveSpeed;

    private GameObject selectedObject = null;
    [SerializeField] private Transform selectedObjectParent;
    public float lerpedScale;
    private bool polaroidAudioPlaying = false;
    private bool hasCoin = false;

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
        if (Input.GetMouseButton(0) && selectedObject)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedObject.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - moveSpeed, Time.deltaTime * 60));
            selectedObject.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool hitTarget = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("PasswordTube") && hasCoin)
                    {
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

                        // add coin to password tube
                        selectedObject.transform.SetParent(PasswordTube.instance.tubeCoinParent);
                        PasswordTube.instance.AddCoin(selectedObject);

                        // reset other coins
                        NewPasswordGameManager.instance.ResetCoins();
                        
                        // add coin raycast
                        selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(true);
                        selectedObject = null;
                        hitTarget = true;
                    }
                    else if (result.gameObject.transform.CompareTag("Target") && !hasCoin)
                    {
                        // reset polaroid position
                        selectedObject.gameObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
                        selectedObject.transform.SetParent(NewPasswordGameManager.instance.polaroidParent);
                        selectedObject.gameObject.GetComponent<LerpableObject>().LerpPosToTransform(NewPasswordGameManager.instance.polaroidOnScreenPos, 0.2f, false);
                        hitTarget = true;
                        selectedObject = null;

                        // evaluate password coin amount
                        NewPasswordGameManager.instance.EvaluateCoins();
                    }
                }
            }

            if (!hitTarget && hasCoin)
            {
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1f);

                // return coin to original position
                selectedObject.transform.SetParent(NewPasswordGameManager.instance.coinParent);
                NewPasswordGameManager.instance.ResetCoins();

                // add coin raycast
                selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(true);
                selectedObject = null;
            }
            else if (!hitTarget && !hasCoin)
            {
                // return polaroid to original position
                selectedObject.transform.SetParent(NewPasswordGameManager.instance.polaroidParent);
                selectedObject.gameObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
                selectedObject.GetComponent<LerpableObject>().LerpPosToTransform(NewPasswordGameManager.instance.polaroidOnScreenPos, 0.2f, false);
                selectedObject = null;
            }

            hasCoin = false;
        }
        // return selected objects to their respective correct destination
        else if (!Input.GetMouseButton(0) && selectedObjectParent.childCount > 0)
        {
            foreach (Transform obj in selectedObjectParent.transform)
            {
                if (obj.tag == "Polaroid")
                {
                    // return polaroid to original position
                    obj.transform.SetParent(NewPasswordGameManager.instance.polaroidParent);
                    obj.gameObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
                    obj.GetComponent<LerpableObject>().LerpPosToTransform(NewPasswordGameManager.instance.polaroidOnScreenPos, 0.2f, false);
                }
                else if (obj.tag == "UniversalCoin")
                {
                    // return coin to original position
                    obj.transform.SetParent(NewPasswordGameManager.instance.coinParent);
                    NewPasswordGameManager.instance.ResetCoins();
                    // add coin raycast
                    obj.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(true);
                }
            }
        }



        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        selectedObject = result.gameObject;
                        selectedObject.transform.SetParent(selectedObjectParent);

                        // play audio
                        selectedObject.gameObject.GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.1f);
                        StartCoroutine(PlayPolaroidAudio(result.gameObject.GetComponent<Polaroid>().challengeWord.audio));
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // select object
                        GameObject foundObject = result.gameObject;

                        // tube coin
                        if (PasswordTube.instance.tubeCoins.Contains(foundObject.GetComponent<UniversalCoinImage>()))
                        {
                            // audio fx
                            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
                            // audio fx
                            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.25f, "coin_dink", 1f);

                            // remove coin raycast
                            foundObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);
                            PasswordTube.instance.RemoveCoin(foundObject); 
                        }
                        else
                        // selectable coin
                        {
                            hasCoin = true;
                            selectedObject = foundObject;
                            selectedObject.transform.SetParent(selectedObjectParent);

                            // audio fx
                            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                            // remove coin raycast
                            selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator PlayPolaroidAudio(AssetReference audioRef)
    {
        if (polaroidAudioPlaying)
            AudioManager.instance.StopTalk();

        polaroidAudioPlaying = true;


        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(audioRef));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(audioRef);
        yield return new WaitForSeconds(cd.GetResult() + 0.1f);

        polaroidAudioPlaying = false;
    }
}
