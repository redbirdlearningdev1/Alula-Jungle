using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordFactoryBuildingRaycaster : MonoBehaviour
{
    public static WordFactoryBuildingRaycaster instance;

    public bool isOn = false;
    public float objcetMoveSpeed = 0.1f;

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

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedObject)
        {
            selectedObject.transform.position = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    //print ("found: " + result.gameObject.name);

                    if (result.gameObject.transform.CompareTag("CoinTarget"))
                    {
                        WordFactoryBuildingManager.instance.EvaluateCoin(selectedObject.GetComponent<UniversalCoinImage>());
                    }
                }
            }

            WaterCoinsController.instance.ReturnWaterCoins();

            // stop wiggle empty frame
            WordFactoryBuildingManager.instance.ToggleEmptyFrameWiggle(false);

            // readd coin raycast
            selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(true);
            selectedObject = null;
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
                        // play audio
                        print (result.gameObject);
                        print (result.gameObject.GetComponent<Polaroid>());
                        StartCoroutine(PlayPolaroidAudio(result.gameObject.GetComponent<Polaroid>().challengeWord.audio));
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // play audio
                        WordFactoryBuildingManager.instance.GlowAndPlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());
                    }
                    else if (result.gameObject.transform.CompareTag("WaterCoin"))
                    {
                        // play audio
                        WordFactoryBuildingManager.instance.GlowAndPlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());

                        // select object
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);

                        // remove coin raycast
                        selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);

                        // wiggle empty frame
                        WordFactoryBuildingManager.instance.ToggleEmptyFrameWiggle(true);
                    }
                }
            }
        }
    }

    private IEnumerator PlayPolaroidAudio(AudioClip audio)
    {
        if (polaroidAudioPlaying)
            AudioManager.instance.StopTalk();

        polaroidAudioPlaying = true;

        AudioManager.instance.PlayTalk(audio);
        yield return new WaitForSeconds(audio.length + 0.1f);

        polaroidAudioPlaying = false;
    }
}
