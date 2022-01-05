using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TigerGameRaycaster : MonoBehaviour
{
    public static TigerGameRaycaster instance;

    public bool isOn = false;
    public float objcetMoveSpeed = 0.1f;

    private GameObject selectedObject = null;
    [SerializeField] private Transform selectedObjectParent;

    private bool polaroidAudioPlaying = false;
    private Transform currentPolaroid;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
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

            Vector3 pos = Vector3.Lerp(selectedObject.transform.position, mousePosWorldSpace, objcetMoveSpeed);
            selectedObject.transform.position = pos;

        }
        else if (Input.GetMouseButtonUp(0) && selectedObject)
        {

            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Bag"))
                    {
                        TigerGameManager.instance.returnToPos(selectedObject);
                        TigerGameManager.instance.EvaluateWaterCoin(selectedObject.GetComponent<Polaroid>());
                    }
                }
            }

            TigerGameManager.instance.returnToPos(selectedObject);
            //currentPolaroid.GetComponent<Polaroid>().SetLayer(0);
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
                    if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        TigerGameManager.instance.PlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());
                    }
                    else if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        currentPolaroid = result.gameObject.transform;
                        // set upper layer
                        //currentPolaroid.GetComponent<Polaroid>().SetLayer(2);
                        // play audio
                        StartCoroutine(PlayPolaroidAudio(currentPolaroid.GetComponent<Polaroid>().challengeWord.audio));
                    }
                }
            }
        }
    }

    private IEnumerator PlayPolaroidAudio(AudioClip audio)
    {
        if (polaroidAudioPlaying)
        {
            AudioManager.instance.StopTalk();
        }

        currentPolaroid.GetComponent<Polaroid>().LerpScale(1.1f, 0.1f);
        polaroidAudioPlaying = true;

        AudioManager.instance.PlayTalk(audio);
        yield return new WaitForSeconds(audio.length + 0.1f);

        currentPolaroid.GetComponent<Polaroid>().LerpScale(1f, 0.1f);
        polaroidAudioPlaying = false;
    }
}
