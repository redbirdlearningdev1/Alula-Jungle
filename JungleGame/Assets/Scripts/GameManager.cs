using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroy<GameManager>
{
    public bool devModeActivated;
    public const float transitionTime = 0.5f; // time to fade into and out of a scene (total transition time is: transitionTime * 2)
    public List<ActionWord> actionWords;
    
    [SerializeField] private GameObject raycastBlocker; // used to block all raycasts (does not work for UI stuff currently)
    [SerializeField] private Transform popupParent;
    [SerializeField] private GameObject levelPopupPrefab;

    private GameData gameData;

    // DEV STUFF:
    private bool iconsSetBroke = false;

    void Start()
    {
        // disable raycast blocker (allow raycasts)
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
        // clear any popups
        foreach (Transform child in popupParent)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(SceneInitCoroutine());
    }

    private IEnumerator SceneInitCoroutine()
    {
        SetRaycastBlocker(true);
        FadeObject.instance.FadeIn(transitionTime);
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

    public void NewLevelPopup(GameData data)
    {
        PopupWindow window = Instantiate(levelPopupPrefab, transform.position, Quaternion.identity, popupParent).GetComponent<PopupWindow>();
        window.InitPopup(data);
    }

    public void SendError(Object errorContext, string errorMsg)
    {
        Debug.LogError("[ERROR] " + errorMsg + " @ " + errorContext.name, errorContext);
    }

    public void SendLog(Object context, string msg)
    {
        Debug.Log("[LOG] " + msg + " @ " + context.name);
    }

    // returns action word data from enum
    public ActionWord GetActionWord(ActionWordEnum word)
    {
        foreach(ActionWord actionWord in actionWords)
        {
            if (actionWord._enum.Equals(word))
                return actionWord;
        }
        SendError(this, "Could not find action word: \'" + word + "\'");
        return null;
    }


    // returns a list with every action word enum
    public List<ActionWordEnum> GetGlobalActionWordList()
    {
        var globalCoinPool = new List<ActionWordEnum>();
        string[] coins = System.Enum.GetNames(typeof(ActionWordEnum));
        for (int i = 0; i < coins.Length; i++) 
        {
            ActionWordEnum coin = (ActionWordEnum)System.Enum.Parse(typeof(ActionWordEnum), coins[i]);
            globalCoinPool.Add(coin);
        }
        globalCoinPool.Remove(ActionWordEnum.SIZE);
        return globalCoinPool;
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
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);

        SendLog(this, "Loading new scene: " + sceneName);
        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneCoroutine(int sceneNum, bool fadeOut, float time)
    {
        if (fadeOut)
        {
            FadeObject.instance.FadeOut(time);
        }
            
        yield return new WaitForSeconds(time);
        SceneManager.LoadSceneAsync(sceneNum);
    }

    /* 
    ################################################
    #   GAME DATA
    ################################################
    */

    public void SetData(GameData data)
    {
        this.gameData = data;
    }

    public GameData GetData()
    {
        return gameData;
    }
}