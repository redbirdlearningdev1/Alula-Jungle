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
                        isCorrect = RummageGameManager.instance.EvaluateSelectedRummageCoin(selectedRummageCoin);
                    }
                }
            }
            Chester.chestGlowNo();
            //selectedRummageCoin.ReturnToCloth();
            selectedRummageCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(2f, 2f), 0.1f);
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
                        selectedRummageCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(2.25f, 2.25f), 0.1f);
                        Chester.chestGlow();
                    }
                    if (result.gameObject.transform.CompareTag("Pile"))
                    {
                        if (result.gameObject.name == "Pile1")
                        {
                            piles[0].pileChose();
                        }
                        if (result.gameObject.name == "Pile2")
                        {
                            piles[1].pileChose();
                        }
                        if (result.gameObject.name == "Pile3")
                        {
                            piles[2].pileChose();
                        }
                        if (result.gameObject.name == "Pile4")
                        {
                            piles[3].pileChose();
                        }
                        if (result.gameObject.name == "Pile5")
                        {
                            piles[4].pileChose();
                        }
                    }
                }
            }
        }
    }
}