using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WordFactoryBuildingRaycaster : MonoBehaviour
{
    public static WordFactoryBuildingRaycaster instance;

    public bool isOn = false;
    public float moveSpeed;

    private GameObject selectedObject = null;
    [SerializeField] private Transform selectedObjectParent;

    private bool polaroidAudioPlaying = false;


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

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    //print ("found: " + result.gameObject.name);

                    if (result.gameObject.transform.CompareTag("CoinTarget"))
                    {
                        WordFactoryBuildingManager.instance.EvaluateCoin(selectedObject.GetComponent<UniversalCoinImage>());
                    }
                }
            }

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

            WaterCoinsController.instance.ReturnWaterCoins();

            // read coin raycast
            selectedObject = null;
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
                    if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        // play audio
                        result.gameObject.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), 0.1f, 0.1f);
                        StartCoroutine(PlayPolaroidAudio(result.gameObject.GetComponent<Polaroid>().challengeWord.audio));
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // play audio
                        WordFactoryBuildingManager.instance.PlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());
                    }
                    else if (result.gameObject.transform.CompareTag("WaterCoin"))
                    {
                        // play audio
                        WordFactoryBuildingManager.instance.PlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());

                        // select object
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        // remove coin raycast
                        selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);
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
