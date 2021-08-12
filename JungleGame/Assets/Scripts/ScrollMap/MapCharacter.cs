using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCharacter : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = true;
    [SerializeField] private GameObject exclamationMark;

    [Header("Game Data")]
    public GameData gameData;

    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;

    void Awake()
    {
        // remove exclamation mark by default + make not interactable
        ShowExclamationMark(false);
        interactable = false;
    }

    public void ShowExclamationMark(bool opt)
    {
        exclamationMark.SetActive(opt);
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable)
            return;

        if (!isPressed)
        {
            isPressed = true;
            transform.localScale = new Vector3(pressedScaleChange, pressedScaleChange, 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            transform.localScale = new Vector3(1f, 1f, 1f);

            // set prev map position
            ScrollMapManager.instance.SetPrevMapPos();

            // check for story beat before going to game
            StartCoroutine(CheckForStoryBeatRoutine());
        }
    }

    private IEnumerator CheckForStoryBeatRoutine()
    {
        print ("curr story beat: " + StudentInfoSystem.currentStudentPlayer.currStoryBeat);
        if (StudentInfoSystem.currentStudentPlayer.currStoryBeat == StoryBeat.PrologueStoryGame)
        {
            // remove exclamation mark from gorilla
            ScrollMapManager.instance.gorilla.ShowExclamationMark(false);

            // play gorilla intro 1
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // play gorilla intro 2
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_2);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger and monkies walk in
            MapAnimationController.instance.TigerAndMonkiesWalkOut();
            // wait for animation to be done
            while (!MapAnimationController.instance.animationDone)
                yield return null;
            
            // turn gorilla to face right
            MapAnimationController.instance.gorilla.transform.localScale = new Vector3(-1, 1, 1);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
            ScrollMapManager.instance.gorilla.ShowExclamationMark(true);

            // tiger destroys gorilla village
            MapAnimationController.instance.tigerScreenSwipeAnim.Play("tigerScreenSwipe");
            // shake screen
            ScrollMapManager.instance.ShakeMap();
            // destroy GV objects one by one
            foreach(var icon in ScrollMapManager.instance.mapIconsAtLocation[2].mapIcons)
            {
                icon.SetFixed(false);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(3f);

            // play gorilla intro 3
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_3);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            // tiger runs off screen

            // monkies go hehe and haha
            
            // monkies run off too

            yield return new WaitForSeconds(1f);

            // play gorilla intro 4
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_4);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield return new WaitForSeconds(1f);

            // play gorilla intro 5
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.gorillaIntro_5);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;
        }


        // go to correct game scene
        if (gameData)
        {
            GameManager.instance.SetData(gameData);
            GameManager.instance.LoadScene(gameData.sceneName, true);
        }
        else
        {
            GameManager.instance.LoadScene("MinigameDemoScene", true);
        }
    }
}
