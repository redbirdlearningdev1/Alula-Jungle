using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GluedSticker : MonoBehaviour
{
    public Image image;
    private StickerData stickerData;
    public LerpableObject deleteButton;

    private bool deleteMode = false;

    public void SetStickerData(StickerData data)
    {
        stickerData = data;
        Sticker sticker = StickerDatabase.instance.GetSticker(stickerData);
        image.sprite = sticker.sprite;
        transform.position = data.boardPos;
    }

    public void SetStickerData(Sticker data, Vector3 pos)
    {
        image.sprite = data.sprite;
        transform.position = pos;
    }

    public void DeleteSticker()
    {
        StartCoroutine(DeleteStickerRoutine());
    }

    private IEnumerator DeleteStickerRoutine()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
    }

    public void SetDeleteButton(bool opt)
    {   
        if (opt == deleteMode)
            return;

        deleteMode = opt;

        if (opt)
        {
            deleteButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
        else
        {
            deleteButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
    }

    public void OnDeleteButtonPressed()
    {
        // open window
        ConfirmDeleteWindow.instance.OpenWindow(stickerData, this);
    }
}
