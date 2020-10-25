using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroy<GameManager>
{
    public bool devModeActivated;
    public const float transitionTime = 1f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)

    [SerializeField] GameObject raycastBlocker; // used to block all raycasts (does not work for UI stuff currently)
    [SerializeField] Transform popupParent;
    [SerializeField] GameObject levelPopupPrefab;

    private StoryGameData storyGameData;

    // DEV STUFF:
    private bool iconsSetBroke = false;

    new void Awake() 
    {
        SetRaycastBlocker(false);
    }

    private void Update()
    {
        if (devModeActivated)
        {
            // press 'D' to go to the dev menu
            if (Input.GetKeyDown(KeyCode.D))
                LoadScene("DevMenu", true);
            // press 'F' to toggle between fixed and broken map sprites
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject smm;
                smm = GameObject.Find("ScrollMapManager");

                if (smm == null) 
                    Debug.LogError("GameManager could not find 'ScrollMapManager'");
                else
                {
                    iconsSetBroke = !iconsSetBroke;
                    smm.GetComponent<ScrollMapManager>().SetMapIconsBroke(iconsSetBroke);
                    Debug.Log("Map icons broken set to: " + iconsSetBroke);
                }
            }
        }
    }

    /* 
    ################################################
    #   SCENE INITIALIZATION
    ################################################
    */

    public void SceneInit()
    {
        StartCoroutine(SceneInitCoroutine());
    }

    private IEnumerator SceneInitCoroutine()
    {
        SetRaycastBlocker(true);
        FadeHelper.FadeIn();
        yield return new WaitForSeconds(transitionTime);
        SetRaycastBlocker(false);
    }

    /* 
    ################################################
    #   UTILITY
    ################################################
    */

    public void SetRaycastBlocker(bool opt)
    {
        raycastBlocker.SetActive(opt);
    }

    public void RestartGame()
    {
        LoadScene(0, true);
    }

    public void NewLevelPopup(Level level)
    {
        PopupWindow window = Instantiate(levelPopupPrefab, transform.position, Quaternion.identity, popupParent).GetComponent<PopupWindow>();
        window.InitPopup(level);
    }

    public void SendError(Object errorContext, string errorMsg)
    {
        Debug.LogError("[ERROR] " + errorMsg + " @ " + errorContext.name, errorContext);
    }

    /* 
    ################################################
    #   SCENE MANAGEMENT
    ################################################
    */

    public void LoadScene(string sceneName, bool fadeOut, float time = transitionTime)
    {
        SetRaycastBlocker(true);
        StartCoroutine(LoadSceneCoroutine(sceneName, fadeOut, time));
    }

    public void LoadScene(int sceneNum, bool fadeOut, float time = transitionTime)
    {
        SetRaycastBlocker(true);
        StartCoroutine(LoadSceneCoroutine(sceneNum, fadeOut, time));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeHelper.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneCoroutine(int sceneNum, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeHelper.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(sceneNum);
    }

    /* 
    ################################################
    #   GAME DATA
    ################################################
    */

    public void SetData<TYPE>(DataType dataType, TYPE data)
    {
        switch (dataType)
        {
            case DataType.StoryGame:
                StoryGameData data_cast = data as StoryGameData;
                if (data_cast != null)
                    this.storyGameData = data_cast;
                break;
            default:
                break;
        }
    }

    public object GetData(DataType dataType)
    {
        switch (dataType)
        {
            case DataType.StoryGame:
                return storyGameData;
            default:
                return null;
        }
    }
}

/* 
################################################
#   DATA TYPE ENUM
################################################
*/

public enum DataType
{
    StoryGame
}