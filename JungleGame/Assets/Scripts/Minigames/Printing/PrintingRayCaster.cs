using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrintingRayCaster : MonoBehaviour
{
    public static PrintingRayCaster instance;

    public bool isOn = false;
    private Ball selectedBall = null;
    [SerializeField] private Transform selectedBallParent;

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
                    if (result.gameObject.transform.CompareTag("Cannon"))
                    {
                        isCorrect = PrintingGameManager.instance.EvaluateSelectedBall(selectedBall.type);
                        BallsController.instance.ReleaseBalls();
                    }
                }
            }

            BallsController.instance.ReDropBall(selectedBall);
            selectedBall = null;

            // cannon stuff
            CannonController.instance.wiggleController.StopWiggle();
            CannonController.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
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
                    if (result.gameObject.transform.CompareTag("Ball"))
                    {
                        selectedBall = result.gameObject.GetComponent<Ball>();
                        selectedBall.TogglePhysics(false);
                        selectedBall.GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
                        selectedBall.GetComponent<LerpableObject>().LerpRotation(0f, 0.2f);
                        selectedBall.gameObject.transform.SetParent(selectedBallParent);

                        // cannon stuff
                        CannonController.instance.wiggleController.StartWiggle();
                        CannonController.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.2f);
                    }
                }
            }
        }
    }
}
