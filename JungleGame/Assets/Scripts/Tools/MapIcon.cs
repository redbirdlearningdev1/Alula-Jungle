using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Outline outlineComp;

    private static float pressedScaleChange = 0.95f;

    public Color hoverOverColor;
    public Color noHoverColor;

    void Awake() 
    {
        outlineComp = GetComponent<Outline>();
        outlineComp.effectColor = noHoverColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(OnPressScale(0.1f));
    }

    private IEnumerator OnPressScale(float duration)
    {
        GameHelper.NewLevelPopup(new Level("DevMenu"));

        transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        yield return new WaitForSeconds(duration);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineComp.effectColor = hoverOverColor;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        outlineComp.effectColor = noHoverColor;
    }
}
