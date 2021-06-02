using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ScrollingBackground : MonoBehaviour
{
    private static ScrollingBackground instance;

    [Header("Dev Stuff")]
    public StoryGameBackground currBackground;
    private StoryGameBackground prevBackground;
    [Range(0,1)] public float gorillaPos;

    [Header("Scrolling Image Variables")]
    [SerializeField] private List<Image> scrollingImages;

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
}
