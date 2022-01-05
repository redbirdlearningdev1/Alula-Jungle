using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShellController : MonoBehaviour
{
    public static ShellController instance;

    public SeaShell shell1;
    public SeaShell shell2;
    public SeaShell shell3;

    public Animator tideAnimator;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        // hide shells
        shell1.ToggleShell(false);
        shell2.ToggleShell(false);
        shell3.ToggleShell(false);
    }

    public void RevealShells()
    {
        StartCoroutine(RevealShellsRoutine());
    }

    private IEnumerator RevealShellsRoutine()
    {
        // reset shell positions
        shell1.transform.position = shell1.shellOrigin.position;
        shell2.transform.position = shell2.shellOrigin.position;
        shell3.transform.position = shell3.shellOrigin.position;

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaveCrash, 0.25f);
        yield return new WaitForSeconds(0.7f);
        tideAnimator.Play("tideWipe");
        yield return new WaitForSeconds(0.75f);
        // reveal shells
        shell1.ToggleShell(true);
        shell2.ToggleShell(true);
        shell3.ToggleShell(true);
    }

    public void HideShells()
    {
        StartCoroutine(HideShellsRoutine());
    }

    private IEnumerator HideShellsRoutine()
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.WaveCrash, 0.25f);
        yield return new WaitForSeconds(0.7f);
        tideAnimator.Play("tideWipe");
        yield return new WaitForSeconds(0.75f);
        // reveal shells
        shell1.ToggleShell(false);
        shell2.ToggleShell(false);
        shell3.ToggleShell(false);
    }

    public void ShowCorrectShell()
    {
        // find correct shell 
        if (shell1.value == SeaShellGameManager.instance.currentCoin)
        {
            shell1.ShowShell();
        }
        else if (shell2.value == SeaShellGameManager.instance.currentCoin)
        {
            shell2.ShowShell();
        }
        else if (shell3.value == SeaShellGameManager.instance.currentCoin)
        {
            shell3.ShowShell();
        }
    }
}
