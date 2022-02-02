using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapAnim
{
    BoatIntro,
    RevealGorillaVillage,

}

public class MapAnimationController : MonoBehaviour
{
    public static MapAnimationController instance;
    [HideInInspector] public bool animationDone = false; // used to determine when the current animation is complete

    public Transform offscreenCharacterPos;

    [Header("Characters")]
    public MapCharacter boat;
    public MapCharacter darwin;
    public MapCharacter clogg;
    public MapCharacter julius;
    public MapCharacter brutus;
    public MapCharacter marcus;

    void Awake()
    {
        if (instance == null)
            instance = this;

        // place all characters off screen
        boat.transform.position =   offscreenCharacterPos.position;
        darwin.transform.position = offscreenCharacterPos.position;
        julius.transform.position = offscreenCharacterPos.position;
        brutus.transform.position = offscreenCharacterPos.position;
        marcus.transform.position = offscreenCharacterPos.position;
    }

    public void PlaceCharactersOnMap(StoryBeat storyBeat)
    {

    }

    public void PlayMapAnim(MapAnim animation)
    {
        animationDone = false;

        switch (animation)
        {
            case MapAnim.BoatIntro:
                StartCoroutine(BoatIntro());
                break;
            
            case MapAnim.RevealGorillaVillage:
                StartCoroutine(RevealGorillaVillage());
                break;
        }
    }

    private IEnumerator BoatIntro()
    {
        yield return null;

        // wiggle boat
        boat.GetComponent<MapIcon>().interactable = true;
        boat.GetComponent<WiggleController>().StartWiggle();

        animationDone = false;
    }

    private IEnumerator RevealGorillaVillage()
    {
        yield return null;

        yield return new WaitForSeconds(1f);

        // play dock 1 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_1);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // gorilla excamation point
        darwin.ShowExclamationMark(true);

        // unlock gorilla village
        StartCoroutine(UnlockMapArea(2, true));
        yield return new WaitForSeconds(10f);

        // play dock 2 talkie + wait for talkie to finish
        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.dock_2);
        while (TalkieManager.instance.talkiePlaying)
            yield return null;

        // advance story beat
        StudentInfoSystem.GetCurrentProfile().mapLimit = 2;
        StudentInfoSystem.GetCurrentProfile().currentChapter = Chapter.chapter_1; // new chapter!
        StudentInfoSystem.AdvanceStoryBeat();
        StudentInfoSystem.SaveStudentPlayerData();

        yield return new WaitForSeconds(1f);

        darwin.interactable = true;

        animationDone = false;
    }
}
