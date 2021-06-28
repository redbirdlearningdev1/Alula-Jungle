using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrintingRayCaster : MonoBehaviour
{
    public bool isOn = false;
    private BallController selectedBall = null;
    private PrintingCoin selectedPrintCoin = null;
    [SerializeField] private Transform selectedBallParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedBall)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedBall.transform.position = mousePosWorldSpace;

        }
        else if (Input.GetMouseButtonUp(0) && selectedBall)
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
                        selectedBall.MoveBack();
                        selectedBall.ToggleVisibility(false, false);
                        isCorrect = PrintingGameManager.instance.EvaluateSelectedBall(selectedBall.type, selectedBall);
                    }
                }
            }


            if (isCorrect)
            {


            }
            else
            {
                selectedBall.MoveBack();
                selectedBall = null;
            }
            //selectedShell.shadow.gameObject.SetActive(true);
            selectedBall = null;
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
                    if (result.gameObject.transform.CompareTag("Coin"))
                    {
                        selectedBall = result.gameObject.GetComponent<BallController>();
                        selectedBall.gameObject.transform.SetParent(selectedBallParent);

                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedPrintCoin = result.gameObject.GetComponent<PrintingCoin>();
                        selectedPrintCoin.PlayPhonemeAudio();

                    }
                }
            }
        }
    }
}
