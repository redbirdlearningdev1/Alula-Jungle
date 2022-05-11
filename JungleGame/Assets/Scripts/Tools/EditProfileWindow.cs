using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditProfileWindow : MonoBehaviour
{
    public LerpableObject myWindow;

    public LerpableObject newProfileBackground;

    public LerpableObject confirmButton;
    public LerpableObject closeButton;

    public LerpableObject profilePicture;
    public LerpableObject leftArrow;
    public LerpableObject rightArrow;

    public LerpableObject inputField;

    public TextMeshProUGUI windowText;
    public LerpableObject windowTextLerp;

    void Awake()
    {
        // set window to be hidden
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (inputField.GetComponent<TMP_InputField>().text.Length <= 0)
        {
            confirmButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            confirmButton.GetComponent<Button>().interactable = true;
        }
    }

    public void OpenWindow()
    {
        leftArrow.transform.localScale = Vector3.zero;
        rightArrow.transform.localScale = Vector3.zero;

        profilePicture.transform.localScale = Vector3.zero;

        confirmButton.transform.localScale = Vector3.zero;
        closeButton.transform.localScale = Vector3.zero;

        inputField.transform.localScale = Vector3.zero;

        // set text 
        windowTextLerp.transform.localScale = Vector3.zero;
        windowText.text = "welcome, explorer!\nedit your profile here.";

        myWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);

        StartCoroutine(OpenWindowRoutine());
    }

    private IEnumerator OpenWindowRoutine()
    {
        newProfileBackground.LerpImageAlpha(newProfileBackground.GetComponent<Image>(), 0.9f, 0.5f);

        yield return new WaitForSeconds(0.2f);

        leftArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
        rightArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);

        profilePicture.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);

        confirmButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
        closeButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);

        inputField.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);

        windowTextLerp.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.one, 0.1f, 0.1f);
    }

    private IEnumerator CloseWindowRoutine()
    {
        windowTextLerp.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        confirmButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        profilePicture.SquishyScaleLerp(new Vector2(0.6f, 0.6f), Vector2.zero, 0.1f, 0.1f);
        inputField.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        leftArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        rightArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        myWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        newProfileBackground.LerpImageAlpha(newProfileBackground.GetComponent<Image>(), 0f, 0.5f);
        SplashScreenManager.instance.EnableProfileInteraction(false);
    }

    public void OnCloseWindowButtonPressed()
    {
        StartCoroutine(CloseWindowRoutine());
    }

    public void OnConfirmButtonPressed()
    {
        StartCoroutine(UpdateProfileRoutine());
    }

    private IEnumerator UpdateProfileRoutine()
    {
        windowTextLerp.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        confirmButton.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        profilePicture.SquishyScaleLerp(new Vector2(0.6f, 0.6f), Vector2.zero, 0.1f, 0.1f);
        inputField.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        leftArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);
        rightArrow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        myWindow.SquishyScaleLerp(new Vector2(1.1f, 1.1f), Vector2.zero, 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        newProfileBackground.LerpImageAlpha(newProfileBackground.GetComponent<Image>(), 0f, 0.5f);
        SplashScreenManager.instance.UpdateProfilePressed(inputField.GetComponent<TMP_InputField>().text);
    }
}
