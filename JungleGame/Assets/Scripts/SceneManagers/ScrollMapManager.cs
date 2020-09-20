using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollMapManager : MonoBehaviour
{
    // Map Navigation
    [SerializeField] private RectTransform Map;
    public float navMoveAmount;
    public float transitionTime;

    void Awake()
    {
        // every scene must call this in Awake()
        GameHelper.SceneInit();

        
    }

    /* 
    ################################################
    #   MAP NAVIGATION 
    ################################################
    */

    public void OnGoLeftPressed()
    {
        StartCoroutine(MapSmoothTransition(Map.position.x, Map.position.x + navMoveAmount, transitionTime));
    }

    public void OnGoRightPressed()
    {
        StartCoroutine(MapSmoothTransition(Map.position.x, Map.position.x - navMoveAmount, transitionTime));
    }

    private IEnumerator MapSmoothTransition(float start, float end, float transitionTime)
    {
        GameHelper.SetRaycastBlocker(true);
        float timer = 0f;
        while (timer <= 1.0)
        {
            timer += Time.deltaTime / transitionTime;
            float pos = Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, timer));
            Map.position = new Vector3(pos, 0f, 0f);
            yield return null;
        }
        GameHelper.SetRaycastBlocker(false);
    }

    /* 
    ################################################
    #   UI BUTTONS 
    ################################################
    */

    public void OnSettingsButtonPressed()
    {
        GameHelper.LoadScene("SettingsScene", true);
    }

    public void OnTrophyRoomButtonPressed()
    {
        GameHelper.LoadScene("TrophyRoomScene", true);
    }
}
