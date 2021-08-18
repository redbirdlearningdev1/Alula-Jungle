using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordFactoryRaycaster : MonoBehaviour
{
    public static WordFactoryRaycaster instance;

    public bool isOn = false;
    public float objcetMoveSpeed = 0.1f;

    private GameObject selectedObject = null;
    [SerializeField] private Transform selectedObjectParent;
    [SerializeField] private Transform frontSprite;
    public float lerpedScale;
    // [SerializeField] private GlowLine glowLineTop;
    // [SerializeField] private GlowLine glowLineBottom;

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
        if (Input.GetMouseButton(0) && selectedObject)
        {
            Vector3 mousePosWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorldSpace.z = 0f;

            Vector3 pos = Vector3.Lerp(selectedObject.transform.position, mousePosWorldSpace, objcetMoveSpeed);
            selectedObject.transform.position = pos;
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject)
        {
            // send raycast to check for bag
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            bool isCorrect = false;
            if(raycastResults.Count > 0)
            {
                foreach(var result in raycastResults)
                {
                    if (result.gameObject.transform.CompareTag("PolaroidTarget"))
                    {
                        isCorrect = WordFactoryBlendingManager.instance.EvaluatePolaroid(selectedObject.GetComponent<Polaroid>());
                    }
                }
            }

            // return polaroids to appropriate pos
            WordFactoryBlendingManager.instance.ResetPolaroids();
            selectedObject = null;

            // un-toggle glow lines
            // glowLineTop.ToggleGlow(false);
            // glowLineBottom.ToggleGlow(false);
            frontSprite.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
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
                    if (result.gameObject.transform.CompareTag("Polaroid"))
                    {
                        selectedObject = result.gameObject;
                        selectedObject.gameObject.transform.SetParent(selectedObjectParent);
                        selectedObject.GetComponent<Polaroid>().LerpScale(1.25f, 0.1f);
                        selectedObject.GetComponent<Polaroid>().SetLayer(6);
                        // toggle glow lines
                        // glowLineTop.ToggleGlow(true);
                        // glowLineBottom.ToggleGlow(true);
                        frontSprite.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, lerpedScale), 0.1f);

                        return;
                    }
                    else if (result.gameObject.transform.CompareTag("UniversalCoin"))
                    {
                        WordFactoryBlendingManager.instance.GlowAndPlayAudioCoin(result.gameObject.GetComponent<UniversalCoin>());
                    }
                }
            }
        }
    }
}
