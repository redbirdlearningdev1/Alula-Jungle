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
        transform.localScale = data.scale;
        print ("z angle: " + data.zAngle);
        transform.localRotation = Quaternion.Euler(0f, 0f, data.zAngle);

        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), transform.localScale, 0.2f, 0.2f);
    }

    public void SetStickerData(Sticker data, Vector3 pos, Vector3 scale, float zAngle)
    {
        image.sprite = data.sprite;
        transform.position = pos;
        transform.localScale = scale;
        print ("z angle: " + zAngle);
        transform.localRotation = Quaternion.Euler(0f, 0f, zAngle);

        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), transform.localScale, 0.2f, 0.2f);
    }

    public void DeleteSticker()
    {
        StartCoroutine(DeleteStickerRoutine());
    }

    private IEnumerator DeleteStickerRoutine()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f), Vector2.zero, 0.1f, 0.1f);
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
