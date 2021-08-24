using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerImage : MonoBehaviour
{
    public Transform inventoryStickerParent;

    public void ReturnStickerImageToParent()
    {
        StartCoroutine(ReturnStickerRoutine());
    }
    private IEnumerator ReturnStickerRoutine()
    {
        GetComponent<LerpableObject>().LerpPosition(new Vector2(inventoryStickerParent.transform.position.x, inventoryStickerParent.transform.position.y), 0.25f, false);
        yield return new WaitForSeconds(0.25f);
        transform.SetParent(inventoryStickerParent);
        transform.localPosition = Vector3.zero;
    }

    public void UseOneSticker()
    {
        inventoryStickerParent.GetComponent<InventorySticker>().UseOneSticker();
    }
}
