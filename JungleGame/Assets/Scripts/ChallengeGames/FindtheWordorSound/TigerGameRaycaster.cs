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
                        if(selectedObject != lastPolaroid)
                        {
                            numClick = 0;
                        }
                        lastPolaroid = selectedObject;
                        
                        // set upper layer
                        //currentPolaroid.GetComponent<Polaroid>().SetLayer(2);
                        // play audio
                        StartCoroutine(PlayPolaroidAudio(currentPolaroid.GetComponent<Polaroid>().challengeWord.audio, currentPolaroid.GetComponent<Polaroid>().challengeWord.elkoninList));
                    }
                }
            }
        }
    }

    private IEnumerator PlayPolaroidAudio(AudioClip audio, List<ElkoninValue> elk)
    {
        if(numClick != 2)
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
            numClick++;

        }
        else
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

            /////

            for(int i = 0 ; 0 < currentPolaroid.GetComponent<Polaroid>().challengeWord.elkoninCount; i++)
            {
                if(elk[i].ToString() == "empty_gold" || elk[i].ToString() == "empty_silver" )
                {
                    break;
                }
                //AudioManager.instance.PlayTalk(GameManager.instance.GetGameWord((coin.elk[i]).audio));
                Debug.Log(elk[i]);
                switch(elk[i])
                    {
                        case ElkoninValue.baby:
                            AudioManager.instance.PlayTalk(wordAudioParts[0]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.backpack:
                            AudioManager.instance.PlayTalk(wordAudioParts[1]);
                            yield return new WaitForSeconds(1.25f);
                            break;
                        
                        case ElkoninValue.bumphead:
                            AudioManager.instance.PlayTalk(wordAudioParts[2]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.frustrating:
                            AudioManager.instance.PlayTalk(wordAudioParts[3]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.explorer:
                            AudioManager.instance.PlayTalk(wordAudioParts[4]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.hello:
                            AudioManager.instance.PlayTalk(wordAudioParts[5]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.scared:
                            AudioManager.instance.PlayTalk(wordAudioParts[6]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.spider:
                            AudioManager.instance.PlayTalk(wordAudioParts[7]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.thatguy:
                            AudioManager.instance.PlayTalk(wordAudioParts[8]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.gorilla:
                            AudioManager.instance.PlayTalk(wordAudioParts[9]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.choice:
                            AudioManager.instance.PlayTalk(wordAudioParts[10]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.give:
                            AudioManager.instance.PlayTalk(wordAudioParts[11]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.pirate:
                            AudioManager.instance.PlayTalk(wordAudioParts[12]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.sounds:
                            AudioManager.instance.PlayTalk(wordAudioParts[13]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.strongwind:
                            AudioManager.instance.PlayTalk(wordAudioParts[14]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.listen:
                            AudioManager.instance.PlayTalk(wordAudioParts[15]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.mudslide:
                            AudioManager.instance.PlayTalk(wordAudioParts[16]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.orcs:
                            AudioManager.instance.PlayTalk(wordAudioParts[17]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.poop:
                            AudioManager.instance.PlayTalk(wordAudioParts[18]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.b:
                            AudioManager.instance.PlayTalk(wordAudioParts[19]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.c:
                            AudioManager.instance.PlayTalk(wordAudioParts[20]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.ch:
                            AudioManager.instance.PlayTalk(wordAudioParts[21]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.d:
                            AudioManager.instance.PlayTalk(wordAudioParts[22]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.f:
                            AudioManager.instance.PlayTalk(wordAudioParts[23]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.g:
                            AudioManager.instance.PlayTalk(wordAudioParts[24]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.h:
                            AudioManager.instance.PlayTalk(wordAudioParts[25]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.j:
                            AudioManager.instance.PlayTalk(wordAudioParts[26]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.k:
                            AudioManager.instance.PlayTalk(wordAudioParts[27]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.l:
                            AudioManager.instance.PlayTalk(wordAudioParts[28]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.m:
                            AudioManager.instance.PlayTalk(wordAudioParts[29]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.n:
                            AudioManager.instance.PlayTalk(wordAudioParts[30]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.p:
                            AudioManager.instance.PlayTalk(wordAudioParts[31]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.qu:
                            AudioManager.instance.PlayTalk(wordAudioParts[32]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.r:
                            AudioManager.instance.PlayTalk(wordAudioParts[33]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.s:
                            AudioManager.instance.PlayTalk(wordAudioParts[34]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.sh:
                            AudioManager.instance.PlayTalk(wordAudioParts[35]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.t:
                            AudioManager.instance.PlayTalk(wordAudioParts[36]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.th:
                            AudioManager.instance.PlayTalk(wordAudioParts[37]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.v:
                            AudioManager.instance.PlayTalk(wordAudioParts[38]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.w:
                            AudioManager.instance.PlayTalk(wordAudioParts[39]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.x:
                            AudioManager.instance.PlayTalk(wordAudioParts[40]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.y:
                            AudioManager.instance.PlayTalk(wordAudioParts[41]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        case ElkoninValue.z:
                            AudioManager.instance.PlayTalk(wordAudioParts[41]);
                            yield return new WaitForSeconds(1.25f);
                            break;

                        

                    }
            }
            /////

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

            numClick = 0;
        }

    }
}
