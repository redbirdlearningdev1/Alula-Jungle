using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StickerImage : MonoBehaviour
{
    public Image stickerImage;

    public InventorySticker inventorySticker;
    public WiggleController wiggleController;

    public LerpableObject yesButton;
    public LerpableObject noButton;
    public LerpableObject rotateButton;
    public LerpableObject mirrorButton;

    [HideInInspector] public bool yesNoShown = false;

    void Awake()
    {
        yesButton.transform.localScale = Vector3.zero;
        noButton.transform.localScale = Vector3.zero;
        rotateButton.transform.localScale = Vector3.zero;
        mirrorButton.transform.localScale = Vector3.zero;
    }

    public void ShowYesNoButtons()
    {
        if (yesNoShown)
            return;

        yesNoShown = true;

        yesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        noButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        rotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        mirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

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
        rotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        mirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

        yesButton.GetComponent<WiggleController>().StopWiggle();
        noButton.GetComponent<WiggleController>().StopWiggle();
    }

    public void OnMirrorButtonPressed()
    {
        StartCoroutine(MirrorStickerRoutine());
    }

    private IEnumerator MirrorStickerRoutine()
    {
        float currX = stickerImage.transform.localScale.x;

        stickerImage.GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f * currX, 0.9f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        
        stickerImage.transform.localScale = new Vector3(currX * -0.9f, 0.9f, 1f);

        stickerImage.GetComponent<LerpableObject>().LerpScale(new Vector2(currX * -1f, 1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        
        stickerImage.transform.localScale = new Vector3(currX * -1f, 1f, 1f);
    }

    public void OnRotateButtonPressed()
    {   
        StartCoroutine(RotateStickerRoutine());
    }

    private IEnumerator RotateStickerRoutine()
    {
        float currAngle = stickerImage.transform.localRotation.eulerAngles.z;
        currAngle += 30f; // add 30 degrees
        stickerImage.GetComponent<LerpableObject>().LerpRotation(currAngle, 0.2f);
        yield return new WaitForSeconds(0.2f);

        // if angle is 360, set to 0
        if (currAngle == 360f)
        {
            stickerImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void OnYesPressed()
    {
        if (StickerSystem.instance.readyToGlueSticker)
        {
            StickerSystem.instance.readyToGlueSticker = false;
            HideYesNoButtons();

            // save to SIS
            Sticker sticker = StickerDatabase.instance.GetSticker(inventorySticker.currentStickerData);
            StudentInfoSystem.GlueStickerToBoard(
                sticker, 
                stickerImage.transform.position, 
                stickerImage.transform.localScale, 
                stickerImage.transform.localRotation.eulerAngles.z, 
                StickerSystem.instance.GetCurrentBoard());

            StudentInfoSystem.RemoveStickerFromInventory(sticker);
            StudentInfoSystem.SaveStudentPlayerData();

            StickerSystem.instance.GlueSelectedStickerToBoard(
                sticker, 
                stickerImage.transform.position, 
                stickerImage.transform.localScale, 
                stickerImage.transform.localRotation.eulerAngles.z);

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
