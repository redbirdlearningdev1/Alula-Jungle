using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RummageCoinRaycaster : MonoBehaviour
{
    public static RummageCoinRaycaster instance;

    public bool isOn = false;
    public bool pileChosen = false;
    private RummageCoin selectedRummageCoin = null;
    public float moveSpeed;
    [SerializeField] private chest Chester;
    [SerializeField] private List<pileRummage> piles;
    [SerializeField] private Transform selectedCoinParent;

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
        if (Input.GetMouseButton(0) && selectedRummageCoin)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedRummageCoin.transform.position, mousePosWorldSpace, 1 - Mathf.Pow(1 - moveSpeed, Time.deltaTime * 60));
            selectedRummageCoin.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedRummageCoin)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            bool hitChest = false;
            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Bag"))
                    {
                        hitChest = true;
                        isCorrect = RummageGameManager.instance.EvaluateSelectedRummageCoin(selectedRummageCoin);
                    }
                }
            }

            if (hitChest)
            {
                // make other coins not interactable
                RummageGameManager.instance.SetCoinsInteractable(false);
            }
            else
            {
                // make other coin interactable
                RummageGameManager.instance.SetCoinsInteractable(true);
            }

            if (isCorrect)
                selectedRummageCoin.GoToChest();
            else
                selectedRummageCoin.ReturnToCloth();

            // audio fx
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 0.8f);

            Chester.ToggleScaleAndWiggle(false);
            // selectedRummageCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(2f, 2f), 0.1f);
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
                        var coin = result.gameObject.GetComponent<RummageCoin>();
                        if (!coin.interactable)
                            continue;

                        // make other coin not interactable
                        RummageGameManager.instance.SetCoinsInteractable(false);

                        // audio fx
                        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.CoinDink, 0.5f, "coin_dink", 1.2f);

                        selectedRummageCoin = coin;
                        selectedRummageCoin.PlayPhonemeAudio();
                        selectedRummageCoin.gameObject.transform.SetParent(selectedCoinParent);
                        selectedRummageCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(2.25f, 2.25f), 0.1f);
                        Chester.ToggleScaleAndWiggle(true);
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