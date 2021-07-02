using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TigerGameRaycaster : MonoBehaviour
{
    public bool isOn = false;
    private Poloroid selectedPoloroid = null;
    private CoinChoice selectedCoin = null;
    [SerializeField] private Transform selectedPoloroidParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedPoloroid)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedPoloroid.transform.position = mousePosWorldSpace;

        }
        else if (Input.GetMouseButtonUp(0) && selectedPoloroid)
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
                        selectedPoloroid.MoveOrigin();
                        isCorrect = TigerGameManager.instance.EvaluateSelectedPoloroid(selectedPoloroid.type, selectedPoloroid);
                    }
                }
            }


            if (isCorrect)
            {


            }
            else
            {
                selectedPoloroid.MoveOrigin();
                selectedPoloroid = null;
            }
            //selectedShell.shadow.gameObject.SetActive(true);
            selectedPoloroid = null;
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
                        selectedPoloroid = result.gameObject.GetComponent<Poloroid>();
                        selectedPoloroid.gameObject.transform.SetParent(selectedPoloroidParent);

                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedCoin = result.gameObject.GetComponent<CoinChoice>();
                        selectedCoin.PlayPhonemeAudio();

                    }
                }
            }
        }
    }
}
