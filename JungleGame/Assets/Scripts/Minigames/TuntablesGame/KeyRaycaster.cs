using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyRaycaster : MonoBehaviour
{
    public static KeyRaycaster instance;

    public bool isOn = false;
    public float keyMoveSpeed = 1f;
    public Vector3 oddOffset;
    public Vector3 evenOffset;

    private Key selectedKey = null;
    [SerializeField] private Transform selectedKeyParent;

    private bool playedKeyTutorialPart = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        // return if off, else do thing
        if (!isOn)
            return;

        // drag select coin while mouse 1 down
        if (Input.GetMouseButton(0) && selectedKey)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedKey.transform.position, mousePosWorldSpace, keyMoveSpeed);

            if (selectedKey.keyName == "k2" || selectedKey.keyName == "k4")
            {
                pos += evenOffset;
            }
            else
            {
                pos += oddOffset;
            }
            
            selectedKey.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedKey)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            bool hitRockLock = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    // print (result.gameObject.name);
                    if (result.gameObject.transform.CompareTag("RockLock"))
                    {
                        hitRockLock = true;
                        isCorrect = TurntablesGameManager.instance.EvaluateSelectedKey(selectedKey);
                    }
                }
            }

            if (hitRockLock)
            {
                // make other keys not interactable
                TurntablesGameManager.instance.SetKeysInteractable(false);
            }
            else
            {
                // make other keys interactable
                TurntablesGameManager.instance.SetKeysInteractable(true);
            }

            if (isCorrect)
            {
                
            }
            else
            {
                selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.25f);
                selectedKey.ReturnToRope();

                // re-add key glow if tutorial key
                if (selectedKey.glowingKey)
                    ImageGlowController.instance.SetImageGlow(selectedKey.image, true, GlowValue.glow_1_025);
                    
                selectedKey = null;
            }

            // rock lock glow effect off
            ImageGlowController.instance.SetImageGlow(RockLock.instance.image, false);
            RockLock.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.5f);
            RockLock.instance.GetComponent<WiggleController>().StopWiggle();
        }

        if (Input.GetMouseButtonDown(0))
        {
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("Key"))
                    {
                        var key = result.gameObject.GetComponentInParent<Key>();

                        // continue if key is not interactable
                        if (!key.interactable)
                            break;

                        selectedKey = key;

                        // play tutorial intro 3 if tutorial
                        if (TurntablesGameManager.instance.playTutorial && !playedKeyTutorialPart)
                        {
                            playedKeyTutorialPart = true;

                            StartCoroutine(TutorialPopupRoutine());
                            return;
                        }

                        // make other keys not interactable
                        TurntablesGameManager.instance.SetKeysInteractable(false);

                        selectedKey.PlayAudio();
                        selectedKey.gameObject.transform.SetParent(selectedKeyParent);
                        selectedKey.GetComponent<LerpableObject>().LerpScale(new Vector2(1.25f, 1.25f), 0.25f);

                        // rock lock glow effect on
                        ImageGlowController.instance.SetImageGlow(RockLock.instance.image, true, GlowValue.glow_1_025);
                        RockLock.instance.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.5f);
                        RockLock.instance.GetComponent<WiggleController>().StartWiggle();

                        // remove key glow if tutorial key
                        if (selectedKey.glowingKey)
                            ImageGlowController.instance.SetImageGlow(selectedKey.image, false);
                    }
                }
            }
        }
    }

    private IEnumerator TutorialPopupRoutine()
    {
        isOn = false;
        selectedKey.PlayAudio();

        yield return new WaitForSeconds(1f);

        // play tutorial audio 3
        AudioClip clip = GameIntroDatabase.instance.turntablesIntro3;
        TutorialPopupController.instance.NewPopup(TutorialPopupController.instance.bottomLeft.position, true, TalkieCharacter.Red, clip);
        yield return new WaitForSeconds(clip.length + 1f);

        // reset key
        selectedKey.ReturnToRope();
        selectedKey = null;

        isOn = true;
    }   
}
