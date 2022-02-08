using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordFactoryDeletingRaycaster : MonoBehaviour
{
    public static WordFactoryDeletingRaycaster instance;

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

            bool hitTarget = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    //print ("found: " + result.gameObject.name);

                    if (result.gameObject.transform.CompareTag("CoinTarget"))
                    {
                        WordFactoryDeletingManager.instance.EvaluateCoin(selectedObject.GetComponent<UniversalCoinImage>());
                        hitTarget = true;
                    }
                }
            }

            if (!hitTarget)
            {
                // return coins to frame
                WordFactoryDeletingManager.instance.ReturnCoinsToFrame();
                // close emerald tiger mouth
                EmeraldTigerHolder.instance.CloseMouth();
            }

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);
        
            // scale up tiger
            EmeraldTigerHolder.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
            // read coin raycast
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
                        result.gameObject.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(0.8f, 0.8f), new Vector2(1f, 1f), 0.1f, 0.1f);
                        StartCoroutine(PlayPolaroidAudio(result.gameObject.GetComponent<Polaroid>().challengeWord.audio));
                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        // play audio
                        WordFactoryDeletingManager.instance.PlayAudioCoin(result.gameObject.GetComponent<UniversalCoinImage>());

                        // select object
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        // remove coin raycast
                        selectedObject.GetComponent<UniversalCoinImage>().ToggleRaycastTarget(false);
                        
                        // scale up tiger
                        EmeraldTigerHolder.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.25f);
                        // open emerald tiger mouth
                        EmeraldTigerHolder.instance.OpenMouth();
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
