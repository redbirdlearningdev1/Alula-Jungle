using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerBoard : MonoBehaviour
{
    public StickerBoardType boardType;
    public LerpableObject stickerInventoryWindow;

    public Transform selectedStickerParent;
    public Transform placedStickerParent;
    [HideInInspector] public bool stickerInventoryActive = false;
    private bool canPlaceSticker = false;
    private bool waitingToGlueSticker = false;
    private bool holdingSticker = false;
    private bool overrodeStickerCount = false;

    [Header("Sticker Limits")]
    public Vector2 xLimits;
    public Vector2 yLimits;

    private Transform currentSticker = null;
    private InventorySticker currentInventorySticker = null;

    [Header("Sticker Inventory")]
    public GameObject inventoryStickerPrefab;
    public GameObject imageStickerPrefab;
    public Transform inventoryParent;

    void Awake()
    {
        // close inventory
        stickerInventoryWindow.LerpScale(new Vector2(0f, 1f), 0.0001f);

        // reset bools
        ResetBools();
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

    public void ResetBools()
    {
        canPlaceSticker = false;
        waitingToGlueSticker = false;
        holdingSticker = false;
        overrodeStickerCount = false;
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
        if (!stickerInventoryActive)
        {   
            ToggleStickerInventory(true);
            yield return new WaitForSeconds(0.25f);
        }

        // update inventory count
        if (overrodeStickerCount)
        {
            //print ("returning to original count");
            overrodeStickerCount = false;
            currentInventorySticker.UpdateStickerCount(StudentInfoSystem.GetStickerCount(currentInventorySticker.myStickerObject));
        }

        // return sticker to inventory
        currentSticker.GetComponent<StickerImage>().ReturnStickerImageToParent();
        currentSticker.GetComponent<Image>().raycastTarget = true;
        currentSticker = null;
        currentInventorySticker = null;

        // canPlaceSticker = false;
        // waitingToGlueSticker = false;
        ResetBools();
    }

    public void SetCurrentSticker(InventorySticker newSticker, Transform stickerImage)
    {
        if (currentInventorySticker == newSticker)
            return;

        // can only get new sticker if none are being edited on the board
        if (currentSticker != null)
            return;

        currentInventorySticker = newSticker;

        canPlaceSticker = false;
        currentSticker = stickerImage;
        currentSticker.SetParent(selectedStickerParent);
        currentSticker.GetComponent<Image>().raycastTarget = false;
    }

    public void PlaceCurrentStickerDown()
    {
        holdingSticker = false;
        // deactivate hover areas
        StickerBoardController.instance.hoverAreas.SetActive(false);

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
        // update inventory count
        if (!overrodeStickerCount)
        {
            //print ("removing one count");
            overrodeStickerCount = true;
            currentInventorySticker.UpdateStickerCount(StudentInfoSystem.GetStickerCount(currentInventorySticker.myStickerObject) - 1);
        }

        holdingSticker = true;
        // activate hover areas
        StickerBoardController.instance.hoverAreas.SetActive(true);
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
            currentSticker.GetComponent<StickerImage>().UseOneSticker();
            currentSticker.SetParent(placedStickerParent);

            // Save to SIS
            StudentInfoSystem.GlueStickerToBoard(currentInventorySticker.myStickerObject, currentSticker.localPosition, boardType);

            print ("placing sticker down to board: " + boardType);

            // reset
            currentSticker = null;
            currentInventorySticker = null;
            ResetBools();
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
            // dont update inventory if sticker is in use
            if (!holdingSticker && !waitingToGlueSticker)
                UpdateStickerInventory();
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
            // dont update inventory if sticker is in use
            if (!holdingSticker && !waitingToGlueSticker)
                UpdateStickerInventory();
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(1f, 1f), 0.2f, 0.05f);
        }
        // close window
        else
        {
            stickerInventoryWindow.SquishyScaleLerp(new Vector2(1.2f, 1f), new Vector2(0f, 1f), 0.05f, 0.1f);
        }
    }

    public void UpdateStickerInventory()
    {
        // update inventory text
        StickerInventoryButton.instance.UpdateButtonText();
        
        // empty current inventory
        foreach (Transform child in inventoryParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // re-add all stickers to inventory
        List<Sticker> myInventory = new List<Sticker>();
        foreach (var item in StudentInfoSystem.GetCurrentProfile().stickerInventory)
        {
            myInventory.Add(StickerDatabase.instance.GetSticker(item));
        }

        foreach (var sticker in myInventory)
        {
            var newInventorySticker = Instantiate(inventoryStickerPrefab, inventoryParent).GetComponent<InventorySticker>();
            newInventorySticker.SetStickerType(sticker);
            newInventorySticker.SetStickerBoard(this);
        }
    }

    public void ClearBoard()
    {
        // remove all placed stickers
        foreach (Transform child in placedStickerParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    // adds all stickers onto board
    public void AddAllStickersToBoard()
    {
        List<StickerData> stickers = new List<StickerData>();

        switch (boardType)
        {
            case StickerBoardType.Classic:
                stickers.AddRange(StudentInfoSystem.GetCurrentProfile().classicStickerBoard.stickers);
                break;
            case StickerBoardType.Mossy:
                stickers.AddRange(StudentInfoSystem.GetCurrentProfile().mossyStickerBoard.stickers);
                break;
            case StickerBoardType.Emerald:
                stickers.AddRange(StudentInfoSystem.GetCurrentProfile().emeraldStickerBoard.stickers);
                break;
            case StickerBoardType.Beach:
                stickers.AddRange(StudentInfoSystem.GetCurrentProfile().beachStickerBoard.stickers);
                break;
        }

        foreach (var item in stickers)
        {
            AddStickerOntoBoard(item);
        }
    }

    public void RemoveAllStickersFromBoard()
    {
        foreach (Transform sticker in placedStickerParent)
        {
            sticker.GetComponent<StickerImage>().DeleteSticker();
        }
    }

    public void AddStickerOntoBoard(StickerData data)
    {
        var stickerImage = Instantiate(imageStickerPrefab, placedStickerParent).GetComponent<StickerImage>();
        stickerImage.SetStickerType(data);
        // animate sticker
        var lerp = stickerImage.GetComponent<LerpableObject>();
        lerp.transform.localScale = new Vector3(0f, 0f, 1f);
        lerp.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.05f);
    }
}
