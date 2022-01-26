using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject : MonoBehaviour
{
    public Animator characterAnimator;
    public WiggleController wiggleController;

    void Awake()
    {
        wiggleController.StartWiggle();
    }

    public void SetPopupCharacter(TalkieCharacter character)
    {
        switch (character)
        {
            default: characterAnimator.Play("brutusPopup"); break;
            case TalkieCharacter.Brutus: characterAnimator.Play("brutusPopup"); break;
            case TalkieCharacter.Bubbles: characterAnimator.Play("bubblePopup"); break;
            case TalkieCharacter.Celeste: characterAnimator.Play("celestePopup"); break;
            case TalkieCharacter.Clogg: characterAnimator.Play("cloggPopup"); break;
            case TalkieCharacter.Darwin: characterAnimator.Play("darwinPopup"); break;
            case TalkieCharacter.Julius: characterAnimator.Play("juliusPopup"); break;
            case TalkieCharacter.Marcus: characterAnimator.Play("marcusPopup"); break;
            case TalkieCharacter.Ollie: characterAnimator.Play("olliePopup"); break;
            case TalkieCharacter.Red: characterAnimator.Play("redPopup"); break;
            case TalkieCharacter.Spindle: characterAnimator.Play("spindlePopup"); break;
            case TalkieCharacter.Sylvie: characterAnimator.Play("sylviePopup"); break;
        }
    }
}
