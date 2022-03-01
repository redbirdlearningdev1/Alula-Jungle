using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellRayCaster : MonoBehaviour
{
    public static ShellRayCaster instance;

    public bool isOn = false;
    private SeaShell selectedShell = null;
    public  Transform selectedShellParent;

    public float moveSpeed;

    private bool canGrabShell = true;

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
            // set selected coin parent if not set
            if (selectedShell.transform.parent != selectedShellParent)
            {
                selectedShell.gameObject.transform.SetParent(selectedShellParent);
            }

            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedShell.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - moveSpeed, Time.deltaTime * 60));
            selectedShell.transform.position = pos;
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
                        if (!canGrabShell)
                            return;

                        StartCoroutine(DelayShellGrab());
                        selectedShell = result.gameObject.GetComponent<SeaShell>();
                        selectedShell.SelectShell();
                        selectedShell.gameObject.transform.SetParent(selectedShellParent);
                    }
                }
            }
        }
    }

    private IEnumerator DelayShellGrab()
    {
        canGrabShell = false;
        yield return new WaitForSeconds(1f);
        canGrabShell = true;
    }
}
