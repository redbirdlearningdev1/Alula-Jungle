using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum AnimatedIcon 
{
    none, fire, shrine, lamp
}

public enum StarLocation
{
    up, down, none
}

public enum MapIconIdentfier
{
    GV_house1,
    GV_house2,
    GV_statue,
    GV_fire
}

public class MapIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = true;
    public bool popupWindow;

    [Header("Game Data")]
    public GameData gameData;
    public MapIconIdentfier identfier;

    [Header("Animation Stuff")]
    public bool canBeFixed;
    public AnimatedIcon animatedIcon;

    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite fixedSprite;

    private MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private Animator repairAnimator;
    [SerializeField] private Animator destroyAnimator;
    private static float pressedScaleChange = 0.95f;
    private bool isPressed = false;
    private bool isFixed = false;

    [Header("Stars")]
    public StarLocation starLocation;
    [SerializeField] private Star[] upStars;
    [SerializeField] private Star[] downStars;
    [SerializeField] Transform upStarsTransform;
    [SerializeField] Transform downStarsTransform;
    private Star[] currentStars;
    private Transform currentStarRevealPos;
    [SerializeField] private Transform starsHiddenPosition;
    public float starMoveSpeed;
    private bool starsHidden = true;


    void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animatedIcon != AnimatedIcon.none) animator = GetComponent<Animator>();
        if (repairAnimator) repairAnimator.Play("defaultAnimation");

        // configure current stars
        InitStars();
    }

    /* 
    ################################################
    #   STAR ANIMATION METHODS
    ################################################
    */

    public void RevealStars()
    {
        StartCoroutine(RevealStarsRoutine());
    }

    private IEnumerator RevealStarsRoutine()
    {
        if (starsHidden)
        {
            starsHidden = false;
            foreach(var star in currentStars)
            {
                StartCoroutine(MoveStarRoutine(star.transform, currentStarRevealPos));
                star.LerpStarAlphaScale(1f, 1f);
                star.SetRendererLayer(2);

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void HideStars()
    {   
        StartCoroutine(HideStarsRoutine());
    }

    private IEnumerator HideStarsRoutine()
    {
        if (!starsHidden)
        {
            starsHidden = true;
            foreach(var star in currentStars)
            {
                StartCoroutine(MoveStarRoutine(star.transform, starsHiddenPosition));
                star.LerpStarAlphaScale(0f, 0f);
                star.SetRendererLayer(0);

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private IEnumerator MoveStarRoutine(Transform star, Transform targetTransform)
    {
        Vector3 startPos = star.position;
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > starMoveSpeed)
            {
                star.position = targetTransform.position;
                break;
            }

            var tempPos = Vector3.Lerp(startPos, targetTransform.position, timer / starMoveSpeed);
            star.position = tempPos;
            yield return null;
        }
    }

    private void InitStars()
    {
        switch (starLocation)
        {
            case StarLocation.up:
                currentStars = upStars;
                currentStarRevealPos = upStarsTransform;
                break;
            case StarLocation.down:
                currentStars = downStars;
                currentStarRevealPos = downStarsTransform;
                break;
            default:
            case StarLocation.none:
                currentStars = null;
                break;
        }

        if (currentStars != null)
        {
            // stars should be initially hidden
            foreach (var star in currentStars)
            {
                star.transform.position = starsHiddenPosition.position;
                star.LerpStarAlphaScale(0f, 0f);
                star.SetRendererLayer(0);
            }
            starsHidden = true;
        }
    }

    // set stars to empty or filled based on SIS data
    public void SetStars(int num)
    {
        for (int i = 0; i < num; i++)
        {
            currentStars[i].SetStar(true);
        }
    }

    public void SetOutineColor(Color color)
    {
        meshRenderer.material.color = color;
    }

    public void SetFixed(bool opt, bool animate, bool saveToSIS)
    {
        if (!canBeFixed) return;

        switch (animatedIcon)
        {
            default:
            case AnimatedIcon.none:
                if (!opt) spriteRenderer.sprite = brokenSprite;
                else spriteRenderer.sprite = fixedSprite;
                break;
            case AnimatedIcon.fire:
                if (!opt) animator.Play("fireBroken");
                else animator.Play("fireFixed");
                break;
            case AnimatedIcon.shrine:
                if (!opt) animator.Play("shrineBroken");
                else animator.Play("shrineFixed");
                break;
            case AnimatedIcon.lamp:
                if (!opt) animator.Play("lampBroken");
                else animator.Play("lampFixed");
                break;
        }
        // play animation
        if (animate)
        {
            if (!opt)
            {
                if (destroyAnimator) destroyAnimator.Play("destroyAnimation");
            }
            else
            {
                if (repairAnimator) repairAnimator.Play("repairAnimation");
            }
        }

        // save to SIS
        if (saveToSIS)
        {
            switch (identfier)
            {
                case MapIconIdentfier.GV_house1:
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.isFixed = opt;
                    break;
                case MapIconIdentfier.GV_house2:
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.isFixed = opt;
                    break;
                case MapIconIdentfier.GV_statue:
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.isFixed = opt;
                    break;
                case MapIconIdentfier.GV_fire:
                    StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.isFixed = opt;
                    break;
            }
            StudentInfoSystem.SaveStudentPlayerData();
        }
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

            // check for story beat stuff
            StartCoroutine(CheckForStoryBeatRoutine());
        }
    }

    private IEnumerator CheckForStoryBeatRoutine()
    {
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PrologueStoryGame)
        {  
            // pre story game interaction
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.pre_minigame);
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield break;
        }

        // go to correct game scene
        if (gameData)
        {
            if (popupWindow)
            {
                LevelPreviewWindow.instance.NewWindow(gameData, identfier, GetNumStars());
            }
            else 
            {
                GameManager.instance.SetData(gameData);
                GameManager.instance.LoadScene(gameData.sceneName, true);
            }
        }
        else
        {
            GameManager.instance.LoadScene("MinigameDemoScene", true);
        }
    }

    public int GetNumStars()
    {
        switch (identfier)
        {
            case MapIconIdentfier.GV_house1:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars;
            case MapIconIdentfier.GV_house2:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars;
            case MapIconIdentfier.GV_statue:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars;
            case MapIconIdentfier.GV_fire:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars;
            default:
                return 0;
        }
    }
}
