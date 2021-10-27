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
        tideAnimator.Play("tideWipe");
        yield return new WaitForSeconds(0.75f);
        // reveal shells
        shell1.ToggleShell(true);
        shell2.ToggleShell(true);
        shell3.ToggleShell(true);
    }
}
