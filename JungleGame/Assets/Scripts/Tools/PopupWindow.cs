using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupWindow : MonoBehaviour
{
    private GameData data;

    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI sceneNameText;

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

    public void InitPopup(GameData _data)
    {
        this.data = _data;
        sceneNameText.text = data.sceneName;
    }

    public void OnClosePressed()
    {
        Destroy(this.gameObject);
    }

    public void OnStartPressed()
    {
        GameHelper.SetData(data);
        GameHelper.LoadScene(data.sceneName, true);
    }
}
