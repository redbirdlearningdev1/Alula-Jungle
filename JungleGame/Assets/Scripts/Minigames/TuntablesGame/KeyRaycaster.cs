using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyRaycaster : MonoBehaviour
{
    public bool isOn = false;
    public float keyMoveSpeed = 0.1f;
    public Vector3 oddOffset;
    public Vector3 evenOffset;

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
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedKey.transform.position, mousePosWorldSpace, keyMoveSpeed);

            if (selectedKey.keyName == "k2" || selectedKey.keyName == "k4")
            {
                pos += evenOffset;
            }
            else
            {
                pos += oddOffset;
            }
            
            selectedKey.transform.position = pos;
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
                    // print (result.gameObject.name);
                    if (result.gameObject.transform.CompareTag("RockLock"))
                    {
                        isCorrect = TurntablesGameManager.instance.EvaluateSelectedKey(selectedKey);
                    }
                }
            }

            if (isCorrect)
            {
                
            }
            else
            {
                selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
                selectedKey.ReturnToRope();
                selectedKey = null;
            }

            // rock lock glow effect off
            ImageGlowController.instance.SetImageGlow(RockLock.instance.image, false);
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
                        //selectedKey.SetLayer(3);
                        selectedKey.PlayAudio();
                        selectedKey.gameObject.transform.SetParent(selectedKeyParent);
                        selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1.25f, 1.25f), 0.25f);
                        // rock lock glow effect on
                        ImageGlowController.instance.SetImageGlow(RockLock.instance.image, true, GlowValue.glow_1_025);
                    }
                }
            }
        }
    }
}
