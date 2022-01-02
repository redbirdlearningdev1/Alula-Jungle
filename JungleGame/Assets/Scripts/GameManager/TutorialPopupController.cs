using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupController : MonoBehaviour
{
    public static TutorialPopupController instance;

    public GameObject popupObject;

    [Header("Character Popups")]
    public Sprite brutusPopup;
    public Sprite bubblesPopup;
    public Sprite celestePopup;
    public Sprite cloggPopup;
    public Sprite darwinPopup;
    public Sprite juliusPopup;
    public Sprite marcusPopup;
    public Sprite olliePopup;
    public Sprite redPopup;
    public Sprite spindlePopup;
    public Sprite sylviePopup;

    [Header("Popup Positions")]
    public Transform topLeft;
    public Transform topRight;
    public Transform bottomLeft;
    public Transform bottomRight;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void StopAllPopups()
    {
        StopAllCoroutines();
    }

    private Sprite GetCharacterPopupSprite(TalkieCharacter character)
    {
        switch (character)
        {
            default: return brutusPopup;
            case TalkieCharacter.Brutus: return brutusPopup;
            case TalkieCharacter.Bubbles: return bubblesPopup;
            case TalkieCharacter.Celeste: return celestePopup;
            case TalkieCharacter.Clogg: return cloggPopup;
            case TalkieCharacter.Darwin: return darwinPopup;
            case TalkieCharacter.Julius: return juliusPopup;
            case TalkieCharacter.Marcus: return marcusPopup;
            case TalkieCharacter.Ollie: return olliePopup;
            case TalkieCharacter.Red: return redPopup;
            case TalkieCharacter.Spindle: return spindlePopup;
            case TalkieCharacter.Sylvie: return sylviePopup;
        }
    }

    public void NewPopup(Vector3 pos, bool facingLeft, TalkieCharacter character, AudioClip clip)
    {
        GameObject newPopup = Instantiate(popupObject, pos, Quaternion.identity, this.transform);
        newPopup.transform.localScale = new Vector3(0f, 0f, 1f);
        newPopup.GetComponent<Image>().sprite = GetCharacterPopupSprite(character);

        StartCoroutine(NewPopupRoutine(newPopup.GetComponent<LerpableObject>(), clip, facingLeft));
    }

    public void NewPopup(Vector3 pos, bool facingLeft, TalkieCharacter character, List<AudioClip> clips)
    {
        GameObject newPopup = Instantiate(popupObject, pos, Quaternion.identity, this.transform);
        newPopup.transform.localScale = new Vector3(0f, 0f, 1f);
        newPopup.GetComponent<Image>().sprite = GetCharacterPopupSprite(character);

        StartCoroutine(NewPopupRoutine(newPopup.GetComponent<LerpableObject>(), clips, facingLeft));
    }

    private IEnumerator NewPopupRoutine(LerpableObject popup, AudioClip clip, bool facingLeft)
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-1f, 1f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        AudioManager.instance.PlayTalk(clip);

        yield return new WaitForSeconds(clip.length + 0.2f);

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-0f, 0f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        Destroy(popup.gameObject);
    }

    private IEnumerator NewPopupRoutine(LerpableObject popup, List<AudioClip> clips, bool facingLeft)
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-1f, 1f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        foreach (var clip in clips)
        {
            AudioManager.instance.PlayTalk(clip);
            yield return new WaitForSeconds(clip.length + 0.2f);
        }

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-0f, 0f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        Destroy(popup.gameObject);
    }
}
