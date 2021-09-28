using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiderRayCaster : MonoBehaviour
{
    public bool isOn = false;
    private UniversalCoinImage selectedCoin = null;
    private BugController selectedBug = null;
    [SerializeField] private Transform selectedCoinParent;
    [SerializeField] private WebBall webBallGlow;

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
                        print ("ball");
                        webBallGlow.chestGlowNo();
                        isCorrect = NewSpiderGameManager.instance.EvaluateSelectedSpiderCoin(ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value), selectedCoin);
                    }
                }
            }

            if (!isCorrect)
            {
                webBallGlow.chestGlowNo();
                NewSpiderGameManager.instance.ReturnCoinsToPosition();
            }
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
                        selectedCoin = result.gameObject.GetComponent<UniversalCoinImage>();
                        selectedCoin.gameObject.transform.SetParent(selectedCoinParent);
                        webBallGlow.chestGlow();
                    }
                    if (result.gameObject.transform.CompareTag("Shell"))
                    {
                        selectedBug = result.gameObject.GetComponent<BugController>();
                        selectedBug.PlayPhonemeAudio();
                    }
                }
            }
        }
    }
}
