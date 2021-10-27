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
        shell1.GetComponent<LerpableObject>().SetImageAlpha(shell1.GetComponent<Image>(), 0f);
        shell2.GetComponent<LerpableObject>().SetImageAlpha(shell2.GetComponent<Image>(), 0f);
        shell3.GetComponent<LerpableObject>().SetImageAlpha(shell3.GetComponent<Image>(), 0f);
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
        shell1.GetComponent<LerpableObject>().SetImageAlpha(shell1.GetComponent<Image>(), 1f);
        shell2.GetComponent<LerpableObject>().SetImageAlpha(shell2.GetComponent<Image>(), 1f);
        shell3.GetComponent<LerpableObject>().SetImageAlpha(shell3.GetComponent<Image>(), 1f);
    }
}
