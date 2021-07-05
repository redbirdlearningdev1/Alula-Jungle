using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TigerCoinRayCaster : MonoBehaviour
{
    public bool isOn = false;
    private CoinChoices selectedCoin = null;
    private Poloroid selectedPoloroidNew = null;
    [SerializeField] private Transform selectedPoloroidParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedCoin)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedCoin.transform.position = mousePosWorldSpace;

        }
        else if (Input.GetMouseButtonUp(0) && selectedCoin)
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
                        selectedCoin.MoveOrigin();
                        isCorrect = TigerCoinGameManager.instance.EvaluateSelectedCoins(selectedCoin.type, selectedCoin);
                    }
                }
            }


            if (isCorrect)
            {


            }
            else
            {
                selectedCoin.MoveOrigin();
                selectedCoin = null;
            }
            //selectedShell.shadow.gameObject.SetActive(true);
            selectedCoin = null;
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
                        selectedCoin = result.gameObject.GetComponent<CoinChoices>();
                        selectedCoin.gameObject.transform.SetParent(selectedPoloroidParent);

                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedPoloroidNew = result.gameObject.GetComponent<Poloroid>();
                        //selectedCoin.PlayPhonemeAudio();

                    }
                }
            }
        }
    }
}
