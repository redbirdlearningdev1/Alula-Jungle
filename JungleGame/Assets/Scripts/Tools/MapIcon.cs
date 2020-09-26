using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapIcon : MonoBehaviour, IPointerClickHandler
{
    public string connectedScene;
    private MeshRenderer meshRenderer;
    private static float pressedScaleChange = 0.95f;

    void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetOutineColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(OnPressScale(0.1f));
    }

    private IEnumerator OnPressScale(float duration)
    {
        GameHelper.NewLevelPopup(new Level(connectedScene));

        transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        yield return new WaitForSeconds(duration);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
