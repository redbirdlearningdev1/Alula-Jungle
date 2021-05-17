using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyRaycaster : MonoBehaviour
{
    public bool isOn = false;
    private Key selectedKey = null;
    [SerializeField] private Transform selectedKeyParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedKey)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            selectedKey.transform.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedKey)
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
                    if (result.gameObject.transform.CompareTag("RockLock"))
                    {
                        //isCorrect = FroggerGameManager.instance.EvaluateSelectedCoin(selectedCoin);
                    }
                }
            }

            selectedKey.ReturnToRope();
            selectedKey = null;
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
                    if (result.gameObject.transform.CompareTag("Key"))
                    {
                        selectedKey = result.gameObject.GetComponentInParent<Key>();
                        selectedKey.PlayAudio();
                        selectedKey.gameObject.transform.SetParent(selectedKeyParent);
                    }
                }
            }
        }
    }
}
