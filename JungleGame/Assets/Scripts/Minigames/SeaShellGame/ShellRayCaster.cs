using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShellRayCaster : MonoBehaviour
{
    public bool isOn = false;
    private SeaShell selectedShell = null;
    [SerializeField] private SpriteRenderer CoinHolderGlow;
    [SerializeField] private Transform selectedShellParent;

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
            CoinHolderGlow.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0) && selectedShell)
        {
            CoinHolderGlow.enabled = false;
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
                        isCorrect = SeaShellGameManager.instance.EvaluateSelectedShell(selectedShell);
                    }
                }
            }

            selectedShell.ReturnToLog();
            if (isCorrect == false)
            {
                selectedShell.shadow.gameObject.SetActive(true);

            }
            //selectedShell.shadow.gameObject.SetActive(true);
            selectedShell = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CoinHolderGlow.enabled = false;
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
                        selectedShell.PlayPhonemeAudio();
                        selectedShell.gameObject.transform.SetParent(selectedShellParent);
                        selectedShell.shadow.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
