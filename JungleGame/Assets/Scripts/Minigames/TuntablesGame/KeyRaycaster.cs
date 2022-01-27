using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyRaycaster : MonoBehaviour
{
    public static KeyRaycaster instance;
    public bool isOn = false;
    public Transform selectedKeyParent;

    // private variable
    private Key selectedKey;

    // move speeds
    public float moveSpeed = 0.1f;
    private const float buildmoveSpeed = 0.5f;

    void Awake()
    {
        if (instance == null)
            instance = this;

#if UNITY_EDITOR
#else
        moveSpeed = buildmoveSpeed;
#endif
    }

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedKey)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedKey.transform.position, mousePosWorldSpace, moveSpeed);
            selectedKey.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedKey)
        {   
            // send raycast to check for rock lock
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("RockLock"))
                    {
                        isCorrect = TurntablesGameManager.instance.EvaluateKey(selectedKey);
                    }
                }
            }

            // reset lock rock
            TurntablesGameManager.instance.rockLock.LerpScale(new Vector2(1f, 1f), 0.2f);
            TurntablesGameManager.instance.rockLock.GetComponent<WiggleController>().StopWiggle();

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

            // return key to rope if incorrect
            if (!isCorrect)
                selectedKey.ReturnToRope();
            else
                selectedKey.MoveIntoRockLock();

            // remove glow
            if (TurntablesGameManager.instance.glowCorrectKey)
            {
                ImageGlowController.instance.SetImageGlow(selectedKey.GetComponent<Image>(), false, GlowValue.none);
            }

            selectedKey = null;
        }

        
        // on pointer down
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
                    if (result.gameObject.transform.CompareTag("Key"))
                    {
                        selectedKey = result.gameObject.GetComponentInParent<Key>();
                        selectedKey.PlayAudio();
                        selectedKey.gameObject.transform.SetParent(selectedKeyParent);
                        // make key larger
                        selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1.5f, 1.5f), 0.2f);

                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        // show lock rock
                        TurntablesGameManager.instance.rockLock.LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                        TurntablesGameManager.instance.rockLock.GetComponent<WiggleController>().StartWiggle();
                    } 
                }
            }
        }
    }
}
