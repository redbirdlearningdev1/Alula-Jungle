using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum InventoryState
{
    Hidden, ShowTab, Open
}

public class StickerInventory : MonoBehaviour
{
    public static StickerInventory instance;

    public Transform inventoryStickerParent;
    public GameObject inventoryStickerPrefab;

    public InventoryState currentState;
    public InventoryState prevState;
    public LerpableObject inventoryWindow;
    public float bumpAmount;

    [Header("Inventory Positions")]
    public Transform hiddenPos;
    public Transform showTabPos;
    public Transform openPos;

    [Header("Rarity Texts")]
    public TextMeshProUGUI commonText;
    public TextMeshProUGUI uncommonText;
    public TextMeshProUGUI rareText;
    public TextMeshProUGUI legendaryText;

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }

        // set current state as hidden
        currentState = InventoryState.ShowTab;
        SetInventoryState(InventoryState.Hidden);
    }

    public void OnInventoryButtonPressed()
    {
        // switch between open and show tab
        if (currentState == InventoryState.ShowTab)
        {
            StickerSystem.instance.SetDeleteStickerModeOFF();
            StickerSystem.instance.deleteStickerButton.GetComponent<Button>().interactable = false;
            SetInventoryState(InventoryState.Open);
        }
        else if (currentState == InventoryState.Open)
        {
            StickerSystem.instance.deleteStickerButton.GetComponent<Button>().interactable = true;
            SetInventoryState(InventoryState.ShowTab);
        }
    }

    public void SetInventoryState(InventoryState state)
    {
        // return if state is already current state
        if (state == currentState)
            return;
        // set prev state
        prevState = currentState;    
        // set current state
        currentState = state;

        // return sticker to inventory if sticker is ready to be glued but not glued
        if (currentState == InventoryState.Open && StickerSystem.instance.readyToGlueSticker)
        {
            StickerSystem.instance.ReturnStickerToInventory();
        }

        // start coroutine
        StartCoroutine(SetInventoryStateRoutine());
    }

    public void UpdateStickerInventory()
    {
        // remove all previous stickers
        foreach (Transform child in inventoryStickerParent.transform)
        {
            Destroy(child.gameObject);
        }

        // add all player stickers
        List<InventoryStickerData> stickerData = new List<InventoryStickerData>();
        stickerData.AddRange(StudentInfoSystem.GetCurrentProfile().stickerInventory);
        foreach (var data in stickerData)
        {
            GameObject newInventorySticker = Instantiate(inventoryStickerPrefab, inventoryStickerParent);
            newInventorySticker.GetComponent<InventorySticker>().SetSticker(data);
        }

        // update sticker unlocks
        commonText.text = CountTrues(StudentInfoSystem.GetCurrentProfile().commonStickerUnlocked) + "/60"; // 60 common stickers
        uncommonText.text = CountTrues(StudentInfoSystem.GetCurrentProfile().uncommonStickerUnlocked) + "/36"; // 36 uncommon stickers
        rareText.text = CountTrues(StudentInfoSystem.GetCurrentProfile().rareStickerUnlocked) + "/12"; // 12 rare stickers
        legendaryText.text = CountTrues(StudentInfoSystem.GetCurrentProfile().legendaryStickerUnlocked) + "/12"; // 12 legendary stickers
    }

    public static int CountTrues(bool[] array)
    {
        int count = 0;
        foreach (bool b in array)
            if (b) count++;
        return count;
    }

    private IEnumerator SetInventoryStateRoutine()
    {
        // bump window
        if (prevState == InventoryState.Hidden && currentState == InventoryState.ShowTab ||
            prevState == InventoryState.ShowTab && currentState == InventoryState.Hidden)
        {
            inventoryWindow.LerpXPos(showTabPos.transform.position.x + bumpAmount, 0.1f, false);
            yield return new WaitForSeconds(0.1f);
        }
        else if (prevState == InventoryState.Open || currentState == InventoryState.Open)
        {
            inventoryWindow.LerpXPos(openPos.transform.position.x + bumpAmount, 0.2f, false);
            yield return new WaitForSeconds(0.2f);
        }
        

        switch (currentState)
        {
            case InventoryState.Hidden:
                inventoryWindow.LerpXPos(hiddenPos.position.x, 0.2f, false);
                break;

            case InventoryState.ShowTab:
                inventoryWindow.LerpXPos(showTabPos.position.x, 0.2f, false);
                break;

            case InventoryState.Open:
                inventoryWindow.LerpXPos(openPos.position.x, 0.2f, false);
                break;
        }
    }
}
