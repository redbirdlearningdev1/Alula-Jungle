using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RummageCoinRaycaster : MonoBehaviour
{
    public bool isOn = false;
    public bool pileChosen = false;
    private RummageCoin selectedRummageCoin = null;
    private pileRummage pile = null;
    [SerializeField]  private chest Chester;
    [SerializeField] private List<pileRummage> piles;
    [SerializeField] private Transform selectedCoinParent;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedRummageCoin)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;
            selectedRummageCoin.transform.position = mousePosWorldSpace;
        }
        else if (Input.GetMouseButtonUp(0) && selectedRummageCoin)
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
                        Debug.Log(selectedRummageCoin.type);
                        isCorrect = RummageGameManager.instance.EvaluateSelectedRummageCoin(selectedRummageCoin.type);
                    }
                }
            }
            Chester.chestGlowNo();
            selectedRummageCoin.ReturnToCloth();
            selectedRummageCoin = null;
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
                        selectedRummageCoin = result.gameObject.GetComponent<RummageCoin>();
                        selectedRummageCoin.PlayPhonemeAudio();
                        selectedRummageCoin.gameObject.transform.SetParent(selectedCoinParent);
                        Chester.chestGlow();

                    }
                    if (result.gameObject.transform.CompareTag("Pile"))
                    {
                        //Debug.Log(result.gameObject.ToString());

                        if (result.gameObject.ToString() == "dirtyp1 (UnityEngine.GameObject)")
                        {

                            piles[0].pileChose();
                        }
                        if (result.gameObject.ToString() == "dirtyp2 (UnityEngine.GameObject)")
                        {

                            piles[1].pileChose();
                        }
                        if (result.gameObject.ToString() == "dirtyp3 (UnityEngine.GameObject)")
                        {

                            piles[2].pileChose();
                        }
                        if (result.gameObject.ToString() == "dirtyp4 (UnityEngine.GameObject)")
                        {

                            piles[3].pileChose();
                        }
                        if (result.gameObject.ToString() == "dirtyp5 (UnityEngine.GameObject)")
                        {

                            piles[4].pileChose();
                        }



                    }
                }
            }

        }
    }

}
