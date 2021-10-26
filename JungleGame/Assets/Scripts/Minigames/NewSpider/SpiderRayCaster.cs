using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpiderRayCaster : MonoBehaviour
{
    public static SpiderRayCaster instance;

    public bool isOn = false;
    private UniversalCoinImage selectedCoin = null;
    private BugController selectedBug = null;
    [SerializeField] private Transform selectedCoinParent;
    [SerializeField] private WebBall webBallGlow;

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

            if (raycastResults.Count > 0)
            {
                foreach (var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Bag"))
                    {
                        NewSpiderGameManager.instance.EvaluateSelectedSpiderCoin(ChallengeWordDatabase.ElkoninValueToActionWord(selectedCoin.value), selectedCoin);
                    }
                }
            }

            webBallGlow.chestGlowNo();
            NewSpiderGameManager.instance.ReturnCoinsToPosition();

            selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
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
                    if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        selectedCoin = result.gameObject.GetComponent<UniversalCoinImage>();
                        selectedCoin.gameObject.transform.SetParent(selectedCoinParent);
                        selectedCoin.GetComponent<LerpableObject>().LerpScale(new Vector2(1.25f, 1.25f), 0.25f);
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