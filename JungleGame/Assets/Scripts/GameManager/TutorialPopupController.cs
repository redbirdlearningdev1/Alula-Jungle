using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TutorialPopupController : MonoBehaviour
{
    public static TutorialPopupController instance;

    public GameObject popupObject;
    public Transform popupParent;

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

        // remove all popups
        foreach (Transform childPopup in popupParent)
        {
            print("destroying popup!");
            Destroy(childPopup.gameObject);
        }
    }

    public void NewPopup(Vector3 pos, bool facingLeft, TalkieCharacter character, AssetReference clipRef)
    {
        GameObject newPopup = Instantiate(popupObject, pos, Quaternion.identity, popupParent);
        newPopup.transform.localScale = new Vector3(0f, 0f, 1f);
        newPopup.GetComponent<PopupObject>().SetPopupCharacter(character);

        StartCoroutine(NewPopupRoutine(newPopup.GetComponent<LerpableObject>(), clipRef, facingLeft));
    }

    public void NewPopup(Vector3 pos, bool facingLeft, TalkieCharacter character, List<AssetReference> clipRefs)
    {
        GameObject newPopup = Instantiate(popupObject, pos, Quaternion.identity, popupParent);
        newPopup.transform.localScale = new Vector3(0f, 0f, 1f);
        newPopup.GetComponent<PopupObject>().SetPopupCharacter(character);

        StartCoroutine(NewPopupRoutine(newPopup.GetComponent<LerpableObject>(), clipRefs, facingLeft));
    }

    private IEnumerator NewPopupRoutine(LerpableObject popup, AssetReference clipRef, bool facingLeft)
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-1f, 1f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);


        CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clipRef));
        yield return cd.coroutine;

        AudioManager.instance.PlayTalk(clipRef);

        yield return new WaitForSeconds(cd.GetResult() + 0.2f);

        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-0f, 0f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        Destroy(popup.gameObject);
    }

    private IEnumerator NewPopupRoutine(LerpableObject popup, List<AssetReference> clipRefs, bool facingLeft)
    {
        AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.Pop, 0.5f);

        if (facingLeft)
            popup.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
        else
            popup.SquishyScaleLerp(new Vector2(-1.2f, 1.2f), new Vector2(-1f, 1f), 0.1f, 0.1f);

        yield return new WaitForSeconds(0.2f);

        foreach (var clipRef in clipRefs)
        {

            CoroutineWithData<float> cd = new CoroutineWithData<float>(AudioManager.instance, AudioManager.instance.GetClipLength(clipRef));
            yield return cd.coroutine;

            AudioManager.instance.PlayTalk(clipRef);
            yield return new WaitForSeconds(cd.GetResult() + 0.2f);
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
