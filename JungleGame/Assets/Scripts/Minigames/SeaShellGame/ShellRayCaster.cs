using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellRayCaster : MonoBehaviour
{
    public static ShellRayCaster instance;

    public bool isOn = false;
    private SeaShell selectedShell = null;
    [SerializeField] private Transform selectedShellParent;

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
        if (Input.GetMouseButton(0) && selectedShell)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedShell.transform.position = mousePosWorldSpace;
            
        }
        else if (Input.GetMouseButtonUp(0) && selectedShell)
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
                    if (result.gameObject.transform.CompareTag("CoinHolder"))
                    {
                        isCorrect = SeaShellGameManager.instance.EvaluateSelectedShell(selectedShell.value, selectedShell.shellNum);
                    }
                }
            }

            if (isCorrect)
            {
                selectedShell.CorrectShell();
            }
            else
            {
                selectedShell.UnselectShell();
            }

            selectedShell = null;
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
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedShell = result.gameObject.GetComponent<SeaShell>();
                        selectedShell.SelectShell();
                        selectedShell.gameObject.transform.SetParent(selectedShellParent);
                    }
                }
            }
        }
    }
}
