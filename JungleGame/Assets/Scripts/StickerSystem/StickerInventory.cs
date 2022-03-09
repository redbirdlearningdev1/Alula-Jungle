using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            SetInventoryState(InventoryState.Open);
        }
        else if (currentState == InventoryState.Open)
        {
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

        // update sticker inventory iff opening
        if (currentState == InventoryState.Open)
        {
            UpdateStickerInventory();
        }

        // start coroutine
        StartCoroutine(SetInventoryStateRoutine());
    }

    private void UpdateStickerInventory()
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
