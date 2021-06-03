using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollingBackground : MonoBehaviour
{
    public static ScrollingBackground instance;

    [Header("Dev Stuff")]
    public StoryGameBackground currBackground;
    private StoryGameBackground prevBackground;

    [Header("Scrolling Image Variables")]
    [SerializeField] private List<Image> scrollingImages;
    [Range(0,1)] public float scrollPos;
    private float prevScrollPos;
    public List<float> scrollMultipliers;

    [Header("Gorilla Variables")]
    [SerializeField] private Transform gorilla;
    [SerializeField] private Animator gorillaAnimator;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;

    // Beginning, Emerging, FollowRed, Prologue, Resolution
    [Header("Sprite Groups")]
    [SerializeField] private List<Sprite> beginningSprites;
    [SerializeField] private List<Sprite> emergingSprites;
    [SerializeField] private List<Sprite> followRedSprites;
    [SerializeField] private List<Sprite> prologueSprites;
    [SerializeField] private List<Sprite> resolutionSprites;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Update()
    {
        // only for changing background in edit mode
        if (Application.isEditor)
        {
            if (currBackground != prevBackground)
            {
                SetBackgroundType(currBackground);
                prevBackground = currBackground;
            }
        }

        // return if the prev parallax pos is the same
        if (scrollPos != prevScrollPos)
            prevScrollPos = scrollPos;
        else
            return;

        // parallaxing happens here
        for (int i = 0; i < scrollingImages.Count; i++)
        {
            // set scrolling images position
            Vector3 pos = new Vector3(scrollMultipliers[i] * scrollPos * -1, 0f, 0f);
            scrollingImages[i].transform.localPosition = pos;

            // set gorilla pos
            Vector3 gorillaPos = Vector3.Lerp(startPos.position, endPos.position, scrollPos);
            gorilla.position = gorillaPos;
        }
    }

    public void SetBackgroundType(StoryGameBackground type)
    {
        currBackground = type;
        switch (type)
        {
            case StoryGameBackground.Beginning:
                // set scrolling images to be beginning sprites
                for (int i = 0; i < scrollingImages.Count; i++)
                    scrollingImages[i].sprite = beginningSprites[i];
                break;
            case StoryGameBackground.Emerging:
                // set scrolling images to be emerging sprites
                for (int i = 0; i < scrollingImages.Count; i++)
                    scrollingImages[i].sprite = emergingSprites[i];
                break;
            case StoryGameBackground.FollowRed:
                // set scrolling images to be followRed sprites
                for (int i = 0; i < scrollingImages.Count; i++)
                    scrollingImages[i].sprite = followRedSprites[i];
                break;
            case StoryGameBackground.Prologue:
                // set scrolling images to be prologue sprites
                for (int i = 0; i < scrollingImages.Count; i++)
                    scrollingImages[i].sprite = prologueSprites[i];
                break;
            case StoryGameBackground.Resolution:
                // set scrolling images to be resolution sprites
                for (int i = 0; i < scrollingImages.Count; i++)
                    scrollingImages[i].sprite = resolutionSprites[i];
                break;
        }
    }

    public void LerpScrollPosTo(float newScrollPos, float duration)
    {
        // assert that the new pos is valid (between 0 and 1)
        if (newScrollPos < 0 || newScrollPos > 1)
            return;

        StartCoroutine(LerpScrollPosToRoutine(newScrollPos, duration));
    }

    private IEnumerator LerpScrollPosToRoutine(float newScrollPos, float duration)
    {
        float timer = 0f;
        float startPos = scrollPos;
        float endPos = newScrollPos;
        // gorilla animator -> walk
        gorillaAnimator.Play("walk");

        while (true) 
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                scrollPos = endPos;
                // gorilla animator -> think or yeah
                if (endPos == 1f)
                    gorillaAnimator.Play("sit_yeah");
                else
                    gorillaAnimator.Play("sit_think");
                break;
            }

            scrollPos = Mathf.Lerp(startPos, endPos, timer / duration);
            yield return null;
        }
    }

    public void GorillaCorrectAnim()
    {
        gorillaAnimator.Play("sit_yeah");
    }
}
