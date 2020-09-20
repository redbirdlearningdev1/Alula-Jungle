using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    private Level levelData;

    [SerializeField] Image background;

    void Awake() 
    {
        StartCoroutine(FadeBackground(0.2f));
    }

    private IEnumerator FadeBackground(float time)
    {
        float from = 0f;
        float to = 0.2f;
        background.color = new Color(0f, 0f, 0f, from);

        float timer = 0f;
        while(true)
        {
            if (timer > time)
                break;

            timer += Time.deltaTime;
            background.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, (timer / time)));

            yield return null;
        }
        background.color = new Color(0f, 0f, 0f, to);
    }

    public void InitPopup(Level levelData)
    {
        this.levelData = levelData;
    }

    public void OnClosePressed()
    {
        Destroy(this.gameObject);
    }

    public void OnStartPressed()
    {
        GameHelper.LoadScene(levelData.scene, true);
    }
}
