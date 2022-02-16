using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordFactoryRaycaster : MonoBehaviour
{
    public static WordFactoryRaycaster instance;

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
                    if (result.gameObject.transform.CompareTag("PolaroidTarget"))
                    {
                        WordFactoryBlendingManager.instance.EvaluatePolaroid(selectedObject.GetComponent<Polaroid>());
                        hitTarget = true;
                    }
                }
            }

            // start polaroid wiggle if didnt hit target
            if (!hitTarget)
            {
                WordFactoryBlendingManager.instance.TogglePolaroidsWiggle(true);
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
            }
            else
            {
                // audio fx
                AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SelectBoop, 0.5f);
            }

            // return polaroids to appropriate pos
            WordFactoryBlendingManager.instance.ResetPolaroids();
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
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        selectedObject.GetComponent<Polaroid>().LerpScale(1.25f, 0.1f);
                        // play audio
                        StartCoroutine(PlayPolaroidAudio(selectedObject.GetComponent<Polaroid>().challengeWord.audio));
                        // stop polaroid wiggle
                        WordFactoryBlendingManager.instance.TogglePolaroidsWiggle(false);
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 0.5f);
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        WordFactoryBlendingManager.instance.PlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());
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
