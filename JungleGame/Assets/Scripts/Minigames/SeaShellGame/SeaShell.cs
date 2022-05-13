using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeaShell : MonoBehaviour
{
    public ActionWordEnum value;
    public int shellNum;
    public Image shadow;
    public Transform shellOrigin;

    //private bool audioPlaying;

    public void SetValue(ActionWordEnum newValue)
    {
        value = newValue;
    }

    public void PlayPhonemeAudio()
    {
        if (true)//!audioPlaying)
        {
            StartCoroutine(PlayPhonemeAudioRoutine());
        }
    }

    public void ToggleShell(bool opt)
    {
        if (opt)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            GetComponent<LerpableObject>().SetImageAlpha(GetComponent<Image>(), 1f);
            shadow.GetComponent<LerpableObject>().SetImageAlpha(shadow, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            GetComponent<LerpableObject>().SetImageAlpha(GetComponent<Image>(), 0f);
            shadow.GetComponent<LerpableObject>().SetImageAlpha(shadow, 0f);
        }
    }

    private IEnumerator PlayPhonemeAudioRoutine()
    {
        //audioPlaying = true;
        AudioManager.instance.PlayPhoneme(ChallengeWordDatabase.ActionWordEnumToElkoninValue(value));
        yield return new WaitForSeconds(1f);
        //audioPlaying = false;
    }

    public void ShowShell()
    {
        StartCoroutine(ShowShellRoutine());
    }

    private IEnumerator ShowShellRoutine()
    {
        transform.SetParent(ShellRayCaster.instance.selectedShellParent);
        shadow.GetComponent<LerpableObject>().LerpImageAlpha(shadow, 0f, 0.25f);
        GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
        GetComponent<LerpableObject>().LerpYPos(shellOrigin.position.y + 1f, 0.2f, false);

        GetComponent<WiggleController>().StartWiggle();
        PlayPhonemeAudio();
        yield return new WaitForSeconds(1f);
        GetComponent<WiggleController>().StopWiggle();

        shadow.GetComponent<LerpableObject>().LerpImageAlpha(shadow, 1f, 0.25f);
        GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
        GetComponent<LerpableObject>().LerpYPos(shellOrigin.position.y, 0.2f, false);

        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SandDrop, 0.5f);
        transform.SetParent(shellOrigin);
    }   

    public void SelectShell()
    {
        shadow.GetComponent<LerpableObject>().LerpImageAlpha(shadow, 0f, 0.25f);
        GetComponent<LerpableObject>().LerpScale(new Vector2(1.2f, 1.2f), 0.2f);
        PlayPhonemeAudio();
        StartCoroutine(SelectShellRoutine());
    }

    private IEnumerator SelectShellRoutine()
    {
        GetComponent<WiggleController>().StartWiggle();
        yield return new WaitForSeconds(1f);
        GetComponent<WiggleController>().StopWiggle();
    }

    public void UnselectShell()
    {
        StartCoroutine(UnselectShellRoutine());
    }

    private IEnumerator UnselectShellRoutine()
    {
        shadow.GetComponent<LerpableObject>().LerpImageAlpha(shadow, 1f, 0.25f);
        GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.2f);
        GetComponent<LerpableObject>().LerpPosition(shellOrigin.position, 0.2f, false);
        yield return new WaitForSeconds(0.2f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.SandDrop, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.SetParent(shellOrigin);
    }

    public void CorrectShell()
    {
        StartCoroutine(CorrectShellRoutine());
    }

    private IEnumerator CorrectShellRoutine()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.3f, 1.3f), new Vector2(0f, 0f), 0.2f, 0.2f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);
        yield return new WaitForSeconds(1f);
        GetComponent<LerpableObject>().LerpPosition(shellOrigin.position, 0.2f, false);
        transform.SetParent(shellOrigin);
    }
}
