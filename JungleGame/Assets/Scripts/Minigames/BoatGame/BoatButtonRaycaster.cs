using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoatButtonRaycaster : MonoBehaviour
{
    public bool isOn;
    BoatButton currentButton;

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && currentButton)
        {

        }
        else if (Input.GetMouseButtonUp(0) && currentButton)
        {
            currentButton.SetPressedSprite(false);
            currentButton = null;
        }
        //     // send raycast to check for bag
        //     var pointerEventData = new PointerEventData(EventSystem.current);
        //     pointerEventData.position = Input.mousePosition;
        //     var raycastResults = new List<RaycastResult>();
        //     EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        //     bool isCorrect = false;
        //     if(raycastResults.Count > 0)
        //     {
        //         foreach(var result in raycastResults)
        //         {
        //             print (result.gameObject.name);
        //             if (result.gameObject.transform.CompareTag("RockLock"))
        //             {
        //                 isCorrect = TurntablesGameManager.instance.EvaluateSelectedKey(selectedKey);
        //             }
        //         }
        //     }

        //     if (isCorrect)
        //     {
                
        //     }
        //     else
        //     {
        //         selectedKey.ReturnToRope();
        //         selectedKey = null;
        //     }

        //     // rock lock glow effect off
        //     RockLock.instance.glowController.ToggleGlowOutline(false);
        // }

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
                    if (result.gameObject.transform.CompareTag("BoatButton"))
                    {
                        currentButton = result.gameObject.GetComponent<BoatButton>();
                        currentButton.SetPressedSprite(true);
                        //print ("button_press: " + currentButton.id);
                    }
                }
            }
        }
    }
}
