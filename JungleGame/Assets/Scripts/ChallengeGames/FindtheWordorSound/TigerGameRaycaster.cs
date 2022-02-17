using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        if (numClick != 2)
        {
            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(word.audio.length + 0.1f);
            numClick++;
        }
        else
        {
            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(word.audio.length + 0.1f);

            List<ElkoninValue> elk = new List<ElkoninValue>();
            elk.AddRange(word.elkoninList);

            for (int i = 0 ; 0 < currentPolaroid.GetComponent<Polaroid>().challengeWord.elkoninCount; i++)
            {
                if(elk[i].ToString() == "empty_gold" || elk[i].ToString() == "empty_silver" )
                {
                    break;
                }

                switch(elk[i])
                {
                    case ElkoninValue.baby:
                        AudioManager.instance.PlayTalk(wordAudioParts[0]);
                        break;

                    case ElkoninValue.backpack:
                        AudioManager.instance.PlayTalk(wordAudioParts[1]);
                        break;
                    
                    case ElkoninValue.bumphead:
                        AudioManager.instance.PlayTalk(wordAudioParts[2]);
                        break;

                    case ElkoninValue.frustrating:
                        AudioManager.instance.PlayTalk(wordAudioParts[3]);
                        break;

                    case ElkoninValue.explorer:
                        AudioManager.instance.PlayTalk(wordAudioParts[4]);
                        break;

                    case ElkoninValue.hello:
                        AudioManager.instance.PlayTalk(wordAudioParts[5]);
                        break;

                    case ElkoninValue.scared:
                        AudioManager.instance.PlayTalk(wordAudioParts[6]);
                        break;

                    case ElkoninValue.spider:
                        AudioManager.instance.PlayTalk(wordAudioParts[7]);
                        break;

                    case ElkoninValue.thatguy:
                        AudioManager.instance.PlayTalk(wordAudioParts[8]);
                        break;

                    case ElkoninValue.gorilla:
                        AudioManager.instance.PlayTalk(wordAudioParts[9]);
                        break;

                    case ElkoninValue.choice:
                        AudioManager.instance.PlayTalk(wordAudioParts[10]);
                        break;

                    case ElkoninValue.give:
                        AudioManager.instance.PlayTalk(wordAudioParts[11]);
                        break;

                    case ElkoninValue.pirate:
                        AudioManager.instance.PlayTalk(wordAudioParts[12]);
                        break;

                    case ElkoninValue.sounds:
                        AudioManager.instance.PlayTalk(wordAudioParts[13]);
                        break;

                    case ElkoninValue.strongwind:
                        AudioManager.instance.PlayTalk(wordAudioParts[14]);
                        break;

                    case ElkoninValue.listen:
                        AudioManager.instance.PlayTalk(wordAudioParts[15]);
                        break;

                    case ElkoninValue.mudslide:
                        AudioManager.instance.PlayTalk(wordAudioParts[16]);
                        break;

                    case ElkoninValue.orcs:
                        AudioManager.instance.PlayTalk(wordAudioParts[17]);
                        break;

                    case ElkoninValue.poop:
                        AudioManager.instance.PlayTalk(wordAudioParts[18]);
                        break;

                    case ElkoninValue.b:
                        AudioManager.instance.PlayTalk(wordAudioParts[19]);
                        break;

                    case ElkoninValue.c:
                        AudioManager.instance.PlayTalk(wordAudioParts[20]);
                        break;

                    case ElkoninValue.ch:
                        AudioManager.instance.PlayTalk(wordAudioParts[21]);
                        break;

                    case ElkoninValue.d:
                        AudioManager.instance.PlayTalk(wordAudioParts[22]);
                        break;

                    case ElkoninValue.f:
                        AudioManager.instance.PlayTalk(wordAudioParts[23]);
                        break;

                    case ElkoninValue.g:
                        AudioManager.instance.PlayTalk(wordAudioParts[24]);
                        break;

                    case ElkoninValue.h:
                        AudioManager.instance.PlayTalk(wordAudioParts[25]);
                        break;

                    case ElkoninValue.j:
                        AudioManager.instance.PlayTalk(wordAudioParts[26]);
                        break;

                    case ElkoninValue.k:
                        AudioManager.instance.PlayTalk(wordAudioParts[27]);
                        break;

                    case ElkoninValue.l:
                        AudioManager.instance.PlayTalk(wordAudioParts[28]);
                        break;

                    case ElkoninValue.m:
                        AudioManager.instance.PlayTalk(wordAudioParts[29]);
                        break;

                    case ElkoninValue.n:
                        AudioManager.instance.PlayTalk(wordAudioParts[30]);
                        break;

                    case ElkoninValue.p:
                        AudioManager.instance.PlayTalk(wordAudioParts[31]);
                        break;

                    case ElkoninValue.qu:
                        AudioManager.instance.PlayTalk(wordAudioParts[32]);
                        break;

                    case ElkoninValue.r:
                        AudioManager.instance.PlayTalk(wordAudioParts[33]);
                        break;

                    case ElkoninValue.s:
                        AudioManager.instance.PlayTalk(wordAudioParts[34]);
                        break;

                    case ElkoninValue.sh:
                        AudioManager.instance.PlayTalk(wordAudioParts[35]);
                        break;

                    case ElkoninValue.t:
                        AudioManager.instance.PlayTalk(wordAudioParts[36]);
                        break;

                    case ElkoninValue.th:
                        AudioManager.instance.PlayTalk(wordAudioParts[37]);
                        break;

                    case ElkoninValue.v:
                        AudioManager.instance.PlayTalk(wordAudioParts[38]);
                        break;

                    case ElkoninValue.w:
                        AudioManager.instance.PlayTalk(wordAudioParts[39]);
                        break;

                    case ElkoninValue.x:
                        AudioManager.instance.PlayTalk(wordAudioParts[40]);
                        break;

                    case ElkoninValue.y:
                        AudioManager.instance.PlayTalk(wordAudioParts[41]);
                        break;

                    case ElkoninValue.z:
                        AudioManager.instance.PlayTalk(wordAudioParts[41]);
                        break;
                }

                yield return new WaitForSeconds(1f);
            }

            AudioManager.instance.PlayTalk(word.audio);
            yield return new WaitForSeconds(word.audio.length + 0.1f);

            numClick = 0;
        }

        polaroidAudioPlaying = false;
        currentPolaroid.GetComponent<WiggleController>().StopWiggle();
        currentPolaroid.GetComponent<Polaroid>().LerpScale(1f, 0.1f);
    }
}
