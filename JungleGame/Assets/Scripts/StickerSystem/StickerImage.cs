using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum Direction
{
    None, Top, Bottom, Left, Right
}

public class StickerImage : MonoBehaviour
{
    public Image stickerImage;

    public InventorySticker inventorySticker;
    public WiggleController wiggleController;

    public LerpableObject topYesButton;
    public LerpableObject topNoButton;

    public LerpableObject bottomYesButton;
    public LerpableObject bottomNoButton;

    public LerpableObject leftRotateButton;
    public LerpableObject leftMirrorButton;

    public LerpableObject rightRotateButton;
    public LerpableObject rightMirrorButton;

    private Direction yesNoButtonDirection;
    private Direction editButtonDirection;

    [HideInInspector] public bool stickerButtonsShown = false;

    void Awake()
    {
        topYesButton.transform.localScale = Vector3.zero;
        topNoButton.transform.localScale = Vector3.zero;
        bottomYesButton.transform.localScale = Vector3.zero;
        bottomNoButton.transform.localScale = Vector3.zero;

        leftRotateButton.transform.localScale = Vector3.zero;
        leftMirrorButton.transform.localScale = Vector3.zero;
        rightRotateButton.transform.localScale = Vector3.zero;
        rightMirrorButton.transform.localScale = Vector3.zero;
    }

    public void ShowStickerButtons()
    {
        if (stickerButtonsShown)
            return;
        stickerButtonsShown = true;

        Vector2 placedPos = stickerImage.transform.position;
        //print ("placed pos: [" + placedPos.x + ", " + placedPos.y + "]");

        // determine yes / no button direction
        if (placedPos.y >= 0)
        {
            yesNoButtonDirection = Direction.Bottom;
            bottomYesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            bottomNoButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

            bottomYesButton.GetComponent<WiggleController>().StartWiggle();
            bottomNoButton.GetComponent<WiggleController>().StartWiggle();
        }
        else if (placedPos.y < 0)
        {
            yesNoButtonDirection = Direction.Top;
            topYesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            topNoButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);

            topYesButton.GetComponent<WiggleController>().StartWiggle();
            topNoButton.GetComponent<WiggleController>().StartWiggle();
        }

        // determine edit button direction
        if (placedPos.x >= 0)
        {
            editButtonDirection = Direction.Left;
            leftRotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            leftMirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
        else if (placedPos.x < 0)
        {
            editButtonDirection = Direction.Right;
            rightRotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
            rightMirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.one, 0.1f, 0.1f);
        }
    }

    public void HideStickerButtons()
    {
        if (!stickerButtonsShown)
            return;
        stickerButtonsShown = false;

        // hide yes / no buttons
        if (yesNoButtonDirection == Direction.Top)
        {
            topYesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
            topNoButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

            topYesButton.GetComponent<WiggleController>().StopWiggle();
            topNoButton.GetComponent<WiggleController>().StopWiggle();
        }
        else if (yesNoButtonDirection == Direction.Bottom)
        {
            bottomYesButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
            bottomNoButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);

            bottomYesButton.GetComponent<WiggleController>().StopWiggle();
            bottomNoButton.GetComponent<WiggleController>().StopWiggle();
        }

        // hide edit buttons
        if (editButtonDirection == Direction.Left)
        {
            leftRotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
            leftMirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
        else if (editButtonDirection == Direction.Right)
        {
            rightRotateButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
            rightMirrorButton.SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.1f, 0.1f);
        }
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
        currAngle -= 30f; // add 30 degrees
        stickerImage.GetComponent<LerpableObject>().LerpRotation(currAngle, 0.2f);
        yield return new WaitForSeconds(0.2f);

        // if angle is 360, set to 0
        if (currAngle == -360f)
        {
            stickerImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public void OnYesPressed()
    {
        if (StickerSystem.instance.readyToGlueSticker)
        {
            StickerSystem.instance.readyToGlueSticker = false;
            HideStickerButtons();

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
        if (!stickerButtonsShown)
            return;

        HideStickerButtons();
        StickerSystem.instance.ReturnStickerToInventory();
    }
}
