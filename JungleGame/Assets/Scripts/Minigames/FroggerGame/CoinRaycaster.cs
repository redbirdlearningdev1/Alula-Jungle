using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoinRaycaster : MonoBehaviour
{
    public bool isOn = false;
    public float coinMoveSpeed = 0.1f;
    private const float buildCoinMoveSpeed = 0.5f;

    private LogCoin selectedCoin = null;
    [SerializeField] private Transform selectedCoinParent;

    void Awake()
    {
#if UNITY_EDITOR
#else
        coinMoveSpeed = buildCoinMoveSpeed;
#endif
    }

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

            Vector3 pos = Vector3.Lerp(selectedCoin.transform.position, mousePosWorldSpace, coinMoveSpeed);
            selectedCoin.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedCoin)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Bag"))
                    {
                        isCorrect = FroggerGameManager.instance.EvaluateSelectedCoin(selectedCoin);
                    }
                }
            }

            // bag glow effect off
            ImageGlowController.instance.SetImageGlow(Bag.instance.GetComponent<Image>(), false);      

            if (!isCorrect)
                selectedCoin.ReturnToLog();
            selectedCoin = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Coin"))
                    {
                        selectedCoin = result.gameObject.GetComponent<LogCoin>();
                        selectedCoin.PlayPhonemeAudio();
                        selectedCoin.gameObject.transform.SetParent(selectedCoinParent);

                        // bag glow effect off
                        ImageGlowController.instance.SetImageGlow(Bag.instance.GetComponent<Image>(), true);
                    } 
                }
            }
        }
    }
}
