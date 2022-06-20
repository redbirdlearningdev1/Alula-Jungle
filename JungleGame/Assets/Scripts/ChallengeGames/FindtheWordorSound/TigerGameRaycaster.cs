using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TigerGameRaycaster : MonoBehaviour
{
    public static TigerGameRaycaster instance;

    public bool isOn = false;
    public float moveSpeed;

    private GameObject selectedObject = null;
    [SerializeField] private Transform selectedObjectParent;
    public int numClick = 0;
    private bool polaroidAudioPlaying = false;
    private Transform currentPolaroid;
    private GameObject lastPolaroid = null;
    public List<AudioClip> wordAudioParts;


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
                        if (polaroidAudioPlaying)
                            return;

                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        currentPolaroid = result.gameObject.transform;
                        if(selectedObject != lastPolaroid)
                        {
                            numClick = 0;
                        }
                        lastPolaroid = selectedObject;
                        

                        StartCoroutine(PlayPolaroidAudio(currentPolaroid.GetComponent<Polaroid>().challengeWord));
                    }
                }
            }
        }
    }

    public void EndAudioRoutine()
    {
        polaroidAudioPlaying = false;
        numClick = 0;
        currentPolaroid.GetComponent<WiggleController>().StopWiggle();
        currentPolaroid.GetComponent<Polaroid>().LerpScale(1f, 0.1f);
        StopAllCoroutines();
    }

    private IEnumerator PlayPolaroidAudio(ChallengeWord word)
    {
        polaroidAudioPlaying = true;
        currentPolaroid.GetComponent<WiggleController>().StartWiggle();
        currentPolaroid.GetComponent<Polaroid>().LerpScale(1.1f, 0.1f);

        if (numClick != 1)
        {
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(word.audio));
            yield return cd.coroutine;

            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(cd.GetResult() + 0.1f);
            numClick++;
        }
        else
        {
            
            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(word.audio));
            yield return cd.coroutine;

            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(cd.GetResult() + 0.1f);

            List<ElkoninValue> elk = new List<ElkoninValue>();
            elk.AddRange(word.elkoninList);

            for (int i = 0 ; i < currentPolaroid.GetComponent<Polaroid>().challengeWord.elkoninCount; i++)
            {
                if (elk[i].ToString() == "empty_gold" || elk[i].ToString() == "empty_silver" )
                {
                    break;
                }

                AudioManager.instance.PlayPhoneme(elk[i]);

                yield return new WaitForSeconds(1f);
            }

            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(cd.GetResult() + 0.1f);

            numClick = 0;
        }

        polaroidAudioPlaying = false;
        currentPolaroid.GetComponent<WiggleController>().StopWiggle();
        currentPolaroid.GetComponent<Polaroid>().LerpScale(1f, 0.1f);
    }
    
}
