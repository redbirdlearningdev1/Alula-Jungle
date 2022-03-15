using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerImage : MonoBehaviour
{
    public InventorySticker inventorySticker;
    public WiggleController wiggleController;

    public LerpableObject yesButton;
    public LerpableObject noButton;

    [HideInInspector] public bool yesNoShown = false;

    void Awake()
    {
        yesButton.transform.localScale = Vector3.zero;
        noButton.transform.localScale = Vector3.zero;
    }

    public void ShowYesNoButtons()
    {
        if (yesNoShown)
            return;

        yesNoShown = true;

        yesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        noButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

        yesButton.GetComponent<WiggleController>().StartWiggle();
        noButton.GetComponent<WiggleController>().StartWiggle();
    }

    public void HideYesNoButtons()
    {
        if (!yesNoShown)
            return;

        yesNoShown = false;

        yesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        noButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        yesButton.GetComponent<WiggleController>().StopWiggle();
        noButton.GetComponent<WiggleController>().StopWiggle();
    }

    public void OnYesPressed()
    {
        if (StickerSystem.instance.readyToGlueSticker)
        {
            StickerSystem.instance.readyToGlueSticker = false;
            HideYesNoButtons();

            // save to SIS
            Sticker sticker = StickerDatabase.instance.GetSticker(inventorySticker.currentStickerData);
            StudentInfoSystem.GlueStickerToBoard(sticker, transform.position, StickerSystem.instance.GetCurrentBoard());
            StudentInfoSystem.RemoveStickerFromInventory(sticker);
            StudentInfoSystem.SaveStudentPlayerData();

            StickerSystem.instance.GlueSelectedStickerToBoard(sticker, transform.position);
            StickerInventory.instance.UpdateStickerInventory();

            // delete this sticker image
            Destroy(this.gameObject);
        }
    }

    public void OnNoPressed()
    {
        if (!yesNoShown)
            return;

        yesNoShown = false;

        yesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        noButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        yesButton.GetComponent<WiggleController>().StopWiggle();
        noButton.GetComponent<WiggleController>().StopWiggle();

        StickerSystem.instance.ReturnStickerToInventory();
    }
}
