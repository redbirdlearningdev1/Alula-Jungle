using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoard : MonoBehaviour
{
    public static StickerBoard instance;

    public LerpableObject stickerInventoryWindow;

    public Transform selectedStickerParent;
    public Transform placedStickerParent;
    public bool stickerInventoryActive = false;
    private bool canPlaceSticker = false;
    private bool waitingToGlueSticker = false;

    [Header("Sticker Limits")]
    public Vector2 xLimits;
    public Vector2 yLimits;

    private Transform currentSticker = null;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // close inventory
        stickerInventoryWindow.LerpScale(new Vector2(0f, 1f), 0.0001f);

        // deactivate self
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (currentSticker != null && !waitingToGlueSticker)
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 0f;
            currentSticker.transform.position = mousePos;
        }

        // open and close inventory with sticker
        if (currentSticker != null)
        {
            if (HideInventoryArea.instance.CheckIfMouseOverObject() && !canPlaceSticker)
            {
                canPlaceSticker = true;
                ToggleStickerInventory(false);
            }

            if (OpenInventoryArea.instance.CheckIfMouseOverObject() && canPlaceSticker)
            {
                canPlaceSticker = false;
                ToggleStickerInventory(true);
            }
        }
    }

    public void ReturnCurrentStickerToInventory()
    {
        if (currentSticker != null)
        {
            StartCoroutine(ReturnStickerToInventoryRoutine());
        }
    }

    private IEnumerator ReturnStickerToInventoryRoutine()
    {
        // stop moving sticker
        waitingToGlueSticker = true;

        // open inventory
        ToggleStickerInventory(true);

        yield return new WaitForSeconds(0.25f);

        // return sticker to inventory
        currentSticker.GetComponent<StickerImage>().ReturnStickerImageToParent();
        currentSticker.GetComponent<Image>().raycastTarget = true;
        currentSticker = null;

        canPlaceSticker = false;
        waitingToGlueSticker = false;
    }

    public void SetCurrentSticker(Transform newSticker)
    {
        // can only get new sticker if none are being edited on the board
        if (currentSticker != null)
            return;

        canPlaceSticker = false;
        currentSticker = newSticker;
        currentSticker.SetParent(selectedStickerParent);
        currentSticker.GetComponent<Image>().raycastTarget = false;
    }

    public void PlaceCurrentStickerDown()
    {
        // place sticker on board
        if (canPlaceSticker && !stickerInventoryActive)
        {   
            // if sticker is off-screen return to inventory
            if (StickerOffScreen())
            {
                // return to inventory
                ReturnCurrentStickerToInventory();
                return;
            }

            currentSticker.SetParent(placedStickerParent);

            // activate correct button based on y position
            if (currentSticker.localPosition.y > 0)
                currentSticker.GetComponent<StickerImage>().placeButtonBottom.gameObject.SetActive(true);
            else
                currentSticker.GetComponent<StickerImage>().placeButtonTop.gameObject.SetActive(true);
            currentSticker.GetComponent<Image>().raycastTarget = true;
            waitingToGlueSticker = true;
            return;
        }
        ReturnCurrentStickerToInventory();
    }

    public void PickUpCurrentSticker()
    {
        // pick up sticker to move to a different location
        currentSticker.SetParent(selectedStickerParent);
        // disable both buttons
        currentSticker.GetComponent<StickerImage>().placeButtonBottom.gameObject.SetActive(false);
        currentSticker.GetComponent<StickerImage>().placeButtonTop.gameObject.SetActive(false);
        currentSticker.GetComponent<Image>().raycastTarget = false;
        waitingToGlueSticker = false;
    }

    public void GlueCurrentSticker()
    {
        // can glue iff sticker is on the the sticker board
        if (canPlaceSticker && !stickerInventoryActive && waitingToGlueSticker)
        {
            canPlaceSticker = false;
            currentSticker.GetComponent<StickerImage>().UseOneSticker();
            currentSticker.SetParent(placedStickerParent);
            currentSticker = null;

            // Save to SIS
        }
    }

    public bool StickerOffScreen()
    {
        if (currentSticker != null)
        {
            //print ("current sticker pos: " + currentSticker.localPosition);

            // check x pos
            if (currentSticker.localPosition.x > xLimits.x && currentSticker.localPosition.x < xLimits.y)
            {
                // check y pos
                if (currentSticker.localPosition.y > yLimits.x && currentSticker.localPosition.y < yLimits.y)
                {
                    return false; // sticker is in range 
                }
            }
        }

        return true;
    }

    public void OnStickerInventoryPressed()
    {
        stickerInventoryActive = !stickerInventoryActive;

        // open window
        if (stickerInventoryActive)
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);
        }
        // close window
        else
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);
        }
    }

    public void ToggleStickerInventory(bool opt)
    {
        if (opt == stickerInventoryActive)
            return;
        stickerInventoryActive = opt;

        // open window
        if (stickerInventoryActive)
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);
        }
        // close window
        else
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);
        }
    }
}
