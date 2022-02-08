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
            case TalkieCharacter.Marcus: characterAnimator.Play("marcusPopup"); break;
            case TalkieCharacter.Ollie: characterAnimator.Play("olliePopup"); break;
            case TalkieCharacter.Red: characterAnimator.Play("redPopup"); break;
            case TalkieCharacter.Spindle: characterAnimator.Play("spindlePopup"); break;
            case TalkieCharacter.Sylvie: characterAnimator.Play("sylviePopup"); break;

            case TalkieCharacter.Julius: 
            {
                SetJuliusPopup();
                break;
            }
        }
    }

    private void SetJuliusPopup()
    {
        Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

        switch (currChapter)
        {
            case Chapter.chapter_0:
            case Chapter.chapter_1:
            case Chapter.chapter_2:
            case Chapter.chapter_3:
                characterAnimator.Play("juliusPopup"); break;
            case Chapter.chapter_4:
            case Chapter.chapter_5:
            case Chapter.chapter_final:
                characterAnimator.Play("juliusSadPopup"); break;
        }
    }
}
