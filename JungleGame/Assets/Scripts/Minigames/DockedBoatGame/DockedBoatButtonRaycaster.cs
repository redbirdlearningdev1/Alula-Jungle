using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DockedBoatButtonRaycaster : MonoBehaviour
{
    public bool isOn;
    BoatButton currentButton;

    void Update()
    {
        // return if off or if Talkie is playing, else do thing
        if (!isOn || TalkieManager.instance.talkiePlaying)
            return;

        if (Input.GetMouseButtonUp(0) && currentButton)
        {
            currentButton.SetPressedSprite(false);
            currentButton = null;
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
                    if (result.gameObject.transform.CompareTag("BoatButton"))
                    {
                        currentButton = result.gameObject.GetComponent<BoatButton>();
                        currentButton.SetPressedSprite(true);
                        
                        
                        DockedBoatManager.instance.BoatButtonPressed(currentButton.id);
                    }
                }
            }
        }
    }
}
