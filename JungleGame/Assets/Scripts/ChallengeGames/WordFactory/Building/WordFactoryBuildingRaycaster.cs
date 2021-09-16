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
    public float lerpedScale;

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

            bool isCorrect = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    //print ("found: " + result.gameObject.name);

                    if (result.gameObject.transform.CompareTag("CoinTarget"))
                    {
                        WordFactoryDeletingManager.instance.EvaluateCoin(selectedObject.GetComponent<UniversalCoinImage>());
                    }
                }
            }

            // wiggle tiger frame
            EmeraldTigerHolder.instance.GetComponent<WiggleController>().StopWiggle();
            // scale up tiger
            EmeraldTigerHolder.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
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
                        StartCoroutine(PlayPolaroidAudio(result.gameObject.GetComponent<Polaroid>().challengeWord.audio));
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // play audio
                        WordFactoryDeletingManager.instance.GlowAndPlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());

                        // select object
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);

                        // remove coin raycast
                        selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);

                        // wiggle tiger frame
                        EmeraldTigerHolder.instance.GetComponent<WiggleController>().StartWiggle();
                        // scale up tiger
                        EmeraldTigerHolder.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.25f);
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
