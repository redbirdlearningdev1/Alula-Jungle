using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordFactoryDeletingManager : MonoBehaviour
{
    public static WordFactoryDeletingManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;

        PregameSetup();
    }

    private void PregameSetup()
    {
        // set emerald head to be closed
        EmeraldHead.instance.animator.Play("PolaroidEatten");

        // set winner cards to be inactive
        WinCardsController.instance.ResetCards();

        // set tiger cards to be inactive


        // start game
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        // init game delay
        yield return new WaitForSeconds(1f);

        // open emerald head
        EmeraldHead.instance.animator.Play("OpenMouth");
        yield return new WaitForSeconds(1f);
    }
}
