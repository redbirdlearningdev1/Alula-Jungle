using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PasswordRaycaster : MonoBehaviour
{
    public bool isOn = false;
    public float objcetMoveSpeed = 0.1f;

    private GameObject selectedObject = null;
    private string polar;
    [SerializeField] private Transform selectedObjectParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedObject)
        {
            if (polar == "Polaroid")
            {
                Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosWorldSpace.z = 0f;

                Vector3 pos = Vector3.Lerp(selectedObject.transform.position, mousePosWorldSpace, objcetMoveSpeed);
                selectedObject.transform.position = pos;
                
            }
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            polar = "";
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
                        PasswordGameManager.instance.returnToPos(selectedObject);
                        PasswordGameManager.instance.EvaluateCoinLockIn(selectedObject.GetComponent<Polaroid>());
                    }
                }
            }
            
            PasswordGameManager.instance.returnToPos(selectedObject);
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
                    if (result.gameObject.transform.CompareTag("WaterCoin"))
                    {
                        if(polar != "Polaroid")
                        {
                            selectedObject = result.gameObject;
                            PasswordGameManager.instance.SlotIn(selectedObject.GetComponent<UniversalCoin>(), selectedObject);
                        }

                    }
                    if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        polar = "Polaroid";
                        //WordFactorySubstitutingManager.instance.GlowAndPlayAudioCoin(result.gameObject.GetComponent<UniversalCoin>());
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);

                    }
                }
            }
        }
    }
}
