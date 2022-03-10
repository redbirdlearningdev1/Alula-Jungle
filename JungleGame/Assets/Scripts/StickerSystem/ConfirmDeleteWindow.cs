using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDeleteWindow : MonoBehaviour
{
    public static ConfirmDeleteWindow instance;

    public LerpableObject BG;
    public LerpableObject window;

    public bool windowActive;

    private StickerData stickerToDelete;
    private GluedSticker gluedSticker;

    void Awake()
    {
        if (instance == null)
            instance = this;

        BG.GetComponent<Image>().raycastTarget = false;

        window.transform.localScale = new Vector3(0f, 0f, 1f);
        windowActive = false;
    }

    public void OpenWindow(StickerData stickerData, GluedSticker physicalSticker)
    {
        if (!windowActive)
        {
            stickerToDelete = stickerData;
            gluedSticker = physicalSticker;

            windowActive = true;
            StartCoroutine(OpenWindowRoutine());
        }
    }

    private IEnumerator OpenWindowRoutine()
    {
        // show BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0.95f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = true;

        // make back button not interatable
        StickerSystem.instance.stickerboardBackButton.GetComponent<BackButton>().interactable = false;

        // show window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        
                
        yield break;
    }

    public void OnNoPressed()
    {
        StartCoroutine(OnNoPressedRoutine());
    }

    private IEnumerator OnNoPressedRoutine()
    {   
        // hide window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = false;

        // make back button interatable
        StickerSystem.instance.stickerboardBackButton.GetComponent<BackButton>().interactable = true;

        // turn off delete sticker mode
        StickerSystem.instance.SetDeleteStickerModeOFF();

        windowActive = false;

        yield break;
    }

    public void OnYesPressed()
    {
        StartCoroutine(OnYesPressedRoutine());
    }

    private IEnumerator OnYesPressedRoutine()
    {   
        // hide window
        window.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.5f);
        windowActive = false;

        // remove BG
        BG.LerpImageAlpha(BG.GetComponent<Image>(), 0f, 0.5f);
        BG.GetComponent<Image>().raycastTarget = false;

        // shrink sticker
        gluedSticker.GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), Vector2.zero, 0.2f, 0.2f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        yield return new WaitForSeconds(0.5f);

        // delete sticker
        Destroy(gluedSticker.gameObject);
        // remove from SIS
        StudentInfoSystem.DeleteStickerFromBoard(stickerToDelete, StickerSystem.instance.GetCurrentBoard());

        // make back button interatable
        StickerSystem.instance.stickerboardBackButton.GetComponent<BackButton>().interactable = true;

        yield break;
    }
}
