using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


public enum AnimatedIcon 
{
    none, fire, shrine, lamp, spider, octo, parrot, mermaids, puppyFire, torch
}

public enum StarLocation
{
    up, down, none
}

[System.Serializable]
public enum MapIconIdentfier
{
    None,
    Boat,

    // gorilla village
    GV_house1,
    GV_house2,
    GV_statue,
    GV_fire,

    GV_challenge_1,
    GV_challenge_2,
    GV_challenge_3,

    // mudslide
    MS_logs,
    MS_pond,
    MS_ramp,
    MS_tower,

    MS_challenge_1,
    MS_challenge_2,
    MS_challenge_3,

    // orc village
    OV_houseL,
    OV_houseS,
    OV_statue,
    OV_fire,

    OV_challenge_1,
    OV_challenge_2,
    OV_challenge_3,

    // spider webs
    SF_lamp,
    SF_web,
    SF_shrine,
    SF_spider,

    SF_challenge_1,
    SF_challenge_2,
    SF_challenge_3,

    // orc camp
    OC_axe,
    OC_bigTent,
    OC_smallTent,
    OC_fire,

    OC_challenge_1,
    OC_challenge_2,
    OC_challenge_3,

    // gorilla poop
    GP_outhouse1,
    GP_outhouse2,
    GP_rocks1,
    GP_rocks2,

    GP_challenge_1,
    GP_challenge_2,
    GP_challenge_3,

    // windy cliff
    WC_statue,
    WC_lighthouse,
    WC_ladder,
    WC_rock,
    WC_sign,
    WC_octo,

    WC_challenge_1,
    WC_challenge_2,
    WC_challenge_3,

    // pirate ship
    PS_wheel,
    PS_sail,
    PS_boat,
    PS_bridge,
    PS_front,
    PS_parrot,

    PS_challenge_1,
    PS_challenge_2,
    PS_challenge_3,

    // mermaid beach
    MB_mermaids,
    MB_rock,
    MB_castle,
    MB_bucket,
    MB_umbrella,
    MB_ladder,

    MB_challenge_1,
    MB_challenge_2,
    MB_challenge_3,

    // ruins
    R_lizard1,
    R_lizard2,
    R_caveRock,
    R_pyramid,
    R_face,
    R_arch,

    R_challenge_1,
    R_challenge_2,
    R_challenge_3,

    // exit jungle
    EJ_puppy,
    EJ_bridge,
    EJ_sign,
    EJ_torch,

    EJ_challenge_1,
    EJ_challenge_2,
    EJ_challenge_3,

    // gorilla study
    GS_tent1,
    GS_tent2,
    GS_statue,
    GS_fire,

    GS_challenge_1,
    GS_challenge_2,
    GS_challenge_3,

    // monkeys
    M_flower,
    M_tree,
    M_bananas,
    M_guards,

    M_challenge_1,
    M_challenge_2,
    M_challenge_3,
}

public class MapIcon : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool interactable = true;
    //public bool popupWindow;

    [Header("Map ID")]
    public MapIconIdentfier identifier;

    [Header("Animation Stuff")]
    public bool canBeFixed;
    public AnimatedIcon animatedIcon;
    public Transform additionalIconObject;

    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Sprite fixedSprite;


    [Header("Quips")]
    public bool playQuips;
    public TalkieObject brokenQuips;
    public TalkieObject fixedQuips;

    private Image image;
    private Animator animator;
    [SerializeField] private Animator repairAnimator;
    [SerializeField] private Animator destroyAnimator;
    private bool isPressed = false;
    private bool isFixed = false;

    [Header("Stars")]
    public StarLocation starLocation;
    [SerializeField] private Star[] upStars;
    [SerializeField] private Star[] downStars;
    public LerpableObject topRRBanner;
    public LerpableObject botRRBanner;
    [SerializeField] Transform upStarsTransform;
    [SerializeField] Transform downStarsTransform;
    private Star[] currentStars;
    private Transform currentStarRevealPos;
    [SerializeField] private Transform starsHiddenPosition;
    public float starMoveSpeed;
    private bool starsHidden = true;

    [Header("Wiggle Controller")]
    public WiggleController wiggleController;
    public float timeBetweenWiggles;
    private bool wiggleIcon = false;
    private bool waitForWiggle = false;
    private float timer = 0f;


    void Awake() 
    {
        image = GetComponent<Image>();
        if (animatedIcon != AnimatedIcon.none) animator = GetComponent<Animator>();
        if (repairAnimator) repairAnimator.Play("clearAnimation");

        // configure current stars
        InitStars(); 
    }

    void Start()
    {
        // determine if icon should periodically wiggle
        if (GetNumStars() == 0)
        {
            // offset wiggles by random amount
            timer -= Random.Range(0f, 2.5f);
            wiggleIcon = true;
        }
    }

    void Update()
    {
        // wiggle icon every so often
        if (wiggleIcon && !waitForWiggle && interactable)
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenWiggles)
            {
                StartCoroutine(WiggleIconRoutine());
            }
        }
    }

    private IEnumerator WiggleIconRoutine()
    {
        waitForWiggle = true;
        wiggleController.StartWiggle();
        yield return new WaitForSeconds(1.5f);
        wiggleController.StopWiggle();
        timer = 0f;
        waitForWiggle = false;
    }

    /* 
    ################################################
    #   STAR ANIMATION METHODS
    ################################################
    */

    public void SetRoyalRumberBanner(bool opt)
    {
        if (starLocation == StarLocation.up)
        {
            if (opt)
                topRRBanner.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            else
                topRRBanner.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        }
        else
        {
            if (opt)
                botRRBanner.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.1f, 0.1f);
            else
                botRRBanner.SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(0f, 0f), 0.1f, 0.1f);
        }
    }

    public void RevealStars()
    {
        if (currentStars == null)
            return;
        
        StartCoroutine(RevealStarsRoutine());
    }

    private IEnumerator RevealStarsRoutine()
    {
        if (starsHidden)
        {
            foreach(var star in currentStars)
            {
                StartCoroutine(MoveStarRoutine(star.transform, currentStarRevealPos));
                star.LerpStarAlphaScale(1f, 1f);
                yield return new WaitForSeconds(0.05f);
            }

            starsHidden = false;

            yield return new WaitForSeconds(0.5f);

            // make stars bob
            foreach(var star in currentStars)
            {
                star.bobController.StartBob();
                yield return new WaitForSeconds(0.15f);
            }            
        }
    }

    public void HideStars()
    {   
        if (currentStars == null)
            return;
            
        StartCoroutine(HideStarsRoutine());
    }

    private IEnumerator HideStarsRoutine()
    {
        if (!starsHidden)
        {
            // make stars not bob
            foreach(var star in currentStars)
            {
                star.bobController.StopBob();
            }

            foreach(var star in currentStars)
            {
                StartCoroutine(MoveStarRoutine(star.transform, starsHiddenPosition));
                star.LerpStarAlphaScale(0f, 0f);
                //star.SetRendererLayer(0);

                yield return new WaitForSeconds(0.05f);
            }

            starsHidden = true;
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
                //star.SetRendererLayer(0);
            }
            starsHidden = true;
        }
    }

    // set stars to empty or filled based on SIS data
    public void SetStars(int num)
    {
        //print ("map icon: " + identfier + " setting stars to: " + num);

        // set all stars to be empty
        for (int i = 0; i < 3; i++)
        {
            currentStars[i].SetStar(false);
        }

        // set correct nuber of stars
        for (int i = 0; i < num; i++)
        {
            currentStars[i].SetStar(true);
        }
    }

    public void SetFixed(bool opt, bool animate, bool saveToSIS)
    {
        if (!canBeFixed) return;

        this.isFixed = opt;

        switch (animatedIcon)
        {
            default:
            case AnimatedIcon.none:
                // special case for monkey guards
                if (this.identifier == MapIconIdentfier.M_guards)
                {
                    MapAnimationController.instance.marcus.characterAnimator.Play("marcusFixed");
                    MapAnimationController.instance.brutus.characterAnimator.Play("brutusFixed");
                }
                else
                {
                    if (!opt) image.sprite = brokenSprite;
                    else image.sprite = fixedSprite;
                }
                
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
            case AnimatedIcon.spider:
                if (!opt) animator.Play("spider_broken");
                else animator.Play("spider_fixed");
                break;
            case AnimatedIcon.octo:
                if (!opt) animator.Play("octo_broken");
                else animator.Play("octo_fixed");
                break;
            case AnimatedIcon.parrot:
                if (!opt) animator.Play("parrot_broken");
                else animator.Play("parrot_fixed");
                break;
            case AnimatedIcon.mermaids:
                if (!opt) animator.Play("mermaids_broken");
                else animator.Play("mermaids_fixed");
                break;
            case AnimatedIcon.puppyFire:
                if (!opt) animator.Play("puppyFire_broken");
                else animator.Play("puppyFire_fixed");
                break;
            case AnimatedIcon.torch:
                if (!opt) animator.Play("torch_broken");
                else animator.Play("torch_fixed");
                break;
        }
        // play animation
        if (animate)
        {
            if (!opt)
            {
                StartCoroutine(DestroyAnimation());
            }
            else
            {
                StartCoroutine(FixAnimation());
            }
        }

        // save to SIS
        if (saveToSIS)
        {
            switch (identifier)
            {
                // gorilla village
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

                // mudslide
                case MapIconIdentfier.MS_logs:
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.isFixed = opt;
                    break;
                case MapIconIdentfier.MS_pond:
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.isFixed = opt;
                    break;
                case MapIconIdentfier.MS_ramp:
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.isFixed = opt;
                    break;
                case MapIconIdentfier.MS_tower:
                    StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.isFixed = opt;
                    break;

                // orc village
                case MapIconIdentfier.OV_houseL:
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.isFixed = opt;
                    break;
                case MapIconIdentfier.OV_houseS:
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.isFixed = opt;
                    break;
                case MapIconIdentfier.OV_statue:
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.isFixed = opt;
                    break;
                case MapIconIdentfier.OV_fire:
                    StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.isFixed = opt;
                    break;

                // spooky forest
                case MapIconIdentfier.SF_lamp:
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.isFixed = opt;
                    break;
                case MapIconIdentfier.SF_shrine:
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.isFixed = opt;
                    break;
                case MapIconIdentfier.SF_spider:
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.isFixed = opt;
                    break;
                case MapIconIdentfier.SF_web:
                    StudentInfoSystem.GetCurrentProfile().mapData.SF_web.isFixed = opt;
                    break;

                // orc camp
                case MapIconIdentfier.OC_axe:
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.isFixed = opt;
                    break;
                case MapIconIdentfier.OC_bigTent:
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.isFixed = opt;
                    break;
                case MapIconIdentfier.OC_smallTent:
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.isFixed = opt;
                    break;
                case MapIconIdentfier.OC_fire:
                    StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.isFixed = opt;
                    break;

                // gorilla poop
                case MapIconIdentfier.GP_outhouse1:
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.isFixed = opt;
                    break;
                case MapIconIdentfier.GP_outhouse2:
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.isFixed = opt;
                    break;
                case MapIconIdentfier.GP_rocks1:
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.isFixed = opt;
                    break;
                case MapIconIdentfier.GP_rocks2:
                    StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.isFixed = opt;
                    break;

                // windy cliff
                case MapIconIdentfier.WC_ladder:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.isFixed = opt;
                    break;
                case MapIconIdentfier.WC_lighthouse:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.isFixed = opt;
                    break;
                case MapIconIdentfier.WC_octo:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.isFixed = opt;
                    break;
                case MapIconIdentfier.WC_rock:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.isFixed = opt;
                    break;
                case MapIconIdentfier.WC_sign:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.isFixed = opt;
                    break;
                case MapIconIdentfier.WC_statue:
                    StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.isFixed = opt;
                    break;

                // pirate ship
                case MapIconIdentfier.PS_boat:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;
                case MapIconIdentfier.PS_bridge:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;
                case MapIconIdentfier.PS_front:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;
                case MapIconIdentfier.PS_parrot:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;
                case MapIconIdentfier.PS_sail:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;
                case MapIconIdentfier.PS_wheel:
                    StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.isFixed = opt;
                    break;

                // mermaid beach
                case MapIconIdentfier.MB_bucket:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.isFixed = opt;
                    break;
                case MapIconIdentfier.MB_castle:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.isFixed = opt;
                    break;
                case MapIconIdentfier.MB_ladder:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.isFixed = opt;
                    break;
                case MapIconIdentfier.MB_mermaids:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.isFixed = opt;
                    break;
                case MapIconIdentfier.MB_rock:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.isFixed = opt;
                    break;
                case MapIconIdentfier.MB_umbrella:
                    StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.isFixed = opt;
                    break;

                // ruins
                case MapIconIdentfier.R_arch:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_arch.isFixed = opt;
                    break;
                case MapIconIdentfier.R_caveRock:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.isFixed = opt;
                    break;
                case MapIconIdentfier.R_face:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_face.isFixed = opt;
                    break;
                case MapIconIdentfier.R_lizard1:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.isFixed = opt;
                    break;
                case MapIconIdentfier.R_lizard2:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.isFixed = opt;
                    break;
                case MapIconIdentfier.R_pyramid:
                    StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.isFixed = opt;
                    break;

                // exit jungle
                case MapIconIdentfier.EJ_bridge:
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.isFixed = opt;
                    break;
                case MapIconIdentfier.EJ_puppy:
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.isFixed = opt;
                    break;
                case MapIconIdentfier.EJ_sign:
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.isFixed = opt;
                    break;
                case MapIconIdentfier.EJ_torch:
                    StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.isFixed = opt;
                    break;

                // gorilla study
                case MapIconIdentfier.GS_fire:
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.isFixed = opt;
                    break;
                case MapIconIdentfier.GS_statue:
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.isFixed = opt;
                    break;
                case MapIconIdentfier.GS_tent1:
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.isFixed = opt;
                    break;
                case MapIconIdentfier.GS_tent2:
                    StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.isFixed = opt;
                    break;

                // monkeys
                case MapIconIdentfier.M_bananas:
                    StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.isFixed = opt;
                    break;
                case MapIconIdentfier.M_flower:
                    StudentInfoSystem.GetCurrentProfile().mapData.M_flower.isFixed = opt;
                    break;
                case MapIconIdentfier.M_guards:
                    StudentInfoSystem.GetCurrentProfile().mapData.M_guards.isFixed = opt;
                    break;
                case MapIconIdentfier.M_tree:
                    StudentInfoSystem.GetCurrentProfile().mapData.M_tree.isFixed = opt;
                    break;
            }
            StudentInfoSystem.SaveStudentPlayerData();
        }
    }

    private IEnumerator FixAnimation()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        wiggleController.StartWiggle();
        yield return new WaitForSeconds(0.2f);
        if (repairAnimator) repairAnimator.Play("repairAnimation");
        yield return new WaitForSeconds(0.2f);
        wiggleController.StopWiggle();
    }

    private IEnumerator DestroyAnimation()
    {
        GetComponent<LerpableObject>().SquishyScaleLerp(new Vector2(1.2f, 1.2f), new Vector2(1f, 1f), 0.2f, 0.2f);
        wiggleController.StartWiggle();
        yield return new WaitForSeconds(0.2f);
        if (destroyAnimator) destroyAnimator.Play("destroyAnimation");
        yield return new WaitForSeconds(0.2f);
        wiggleController.StopWiggle();
    }

    /* 
    ################################################
    #   POINTER METHODS
    ################################################
    */

    private bool isOver = false;

    void OnMouseOver()
    {
        // skip if not interactable
        if (!interactable)
            return;
        
        if (!isOver)
        {
            isOver = true;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);

            if (additionalIconObject != null)
                additionalIconObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1.1f, 1.1f), 0.1f);
        }
    }

    void OnMouseExit()
    {
        if (isOver)
        {
            isOver = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

            if (additionalIconObject != null)
                additionalIconObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // return if not interactable
        if (!interactable || !isOver)
            return;

        if (!isPressed)
        {
            isPressed = true;
            GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.1f);

            if (additionalIconObject != null)
                additionalIconObject.GetComponent<LerpableObject>().LerpScale(new Vector2(0.9f, 0.9f), 0.1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && isOver)
        {
            // play audio blip
            AudioManager.instance.PlayFX_oneShot(AudioDatabase.instance.NeutralBlip, 1f);

            isPressed = false;
            GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

            if (additionalIconObject != null)
                additionalIconObject.GetComponent<LerpableObject>().LerpScale(new Vector2(1f, 1f), 0.1f);

            // check for story beat stuff
            StartCoroutine(CheckForStoryBeatRoutine());
        }
    }

    private IEnumerator CheckForStoryBeatRoutine()
    {   
        // go to boat game if story beat correct
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.InitBoatGame)
        {
            GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(GameType.BoatGame), true);
            yield break;
        }
        if (StudentInfoSystem.GetCurrentProfile().currStoryBeat == StoryBeat.PrologueStoryGame)
        {  
            // pre story game interaction
            TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("PreStory_1_p1"));
            while (TalkieManager.instance.talkiePlaying)
                yield return null;

            yield break;
        }

        // check for royal rumble
        if (StudentInfoSystem.GetCurrentProfile().royalRumbleActive && StudentInfoSystem.GetCurrentProfile().royalRumbleID == this.identifier)
        {
            // get current chapter
            Chapter currChapter = StudentInfoSystem.GetCurrentProfile().currentChapter;

            // play correct RR talkie based on current chapter
            switch (currChapter)
            {
                case Chapter.chapter_0:
                case Chapter.chapter_1:
                case Chapter.chapter_2:
                case Chapter.chapter_3:
                    // play julius RR intro
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRJuliusIntro_1_p1"));
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;
                    break;

                case Chapter.chapter_4:
                case Chapter.chapter_5:
                    // play guards RR intro
                    TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_1_p1"));
                    while (TalkieManager.instance.talkiePlaying)
                        yield return null;
                    // first guards RR?
                    if (StudentInfoSystem.GetCurrentProfile().firstGuradsRoyalRumble)
                    {
                        // play guards RR intro 2 p1
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_2_p1"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;

                        // save to SIS
                        StudentInfoSystem.GetCurrentProfile().firstGuradsRoyalRumble = false;
                        StudentInfoSystem.SaveStudentPlayerData();
                    }
                    else
                    {
                        // play guards RR intro 2 p2
                        TalkieManager.instance.PlayTalkie(TalkieDatabase.instance.GetTalkieObject("RRGuardsIntro_2_p2"));
                        while (TalkieManager.instance.talkiePlaying)
                            yield return null;
                    }
                    break;
            }

            // do not go to game if talkie manager says not to
            if (TalkieManager.instance.doNotContinueToGame)
            {
                TalkieManager.instance.doNotContinueToGame = false;
                yield break;
            }

            GameManager.instance.playingRoyalRumbleGame = true;
            GameManager.instance.mapID = identifier;
            GameManager.instance.LoadScene(GameManager.instance.GameTypeToSceneName(StudentInfoSystem.GetCurrentProfile().royalRumbleGame), true, 0.5f, true);
            yield break;
        }
        else
        {
            // play quip
            if (playQuips)
            {
                // remove settings and wagon buttons
                SettingsManager.instance.ToggleMenuButtonActive(false);
                SettingsManager.instance.ToggleWagonButtonActive(false);

                // add raycast blocker
                RaycastBlockerController.instance.CreateRaycastBlocker("map_icon_quip");

                if (isFixed)
                {
                    // play fixed quip
                    TalkieManager.instance.PlayTalkie(fixedQuips);
                }
                else
                {   
                    // play fixed quip
                    TalkieManager.instance.PlayTalkie(brokenQuips);
                }
                while (TalkieManager.instance.talkiePlaying)
                        yield return null;
            }

            MinigameWheelController.instance.RevealWheel(identifier);
            yield break;
        }
    }

    public int GetNumStars()
    {
        switch (identifier)
        {
            default:
                return -1;

            // gorilla village
            case MapIconIdentfier.GV_house1:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_house1.stars;
            case MapIconIdentfier.GV_house2:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_house2.stars;
            case MapIconIdentfier.GV_statue:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_statue.stars;
            case MapIconIdentfier.GV_fire:
                return StudentInfoSystem.GetCurrentProfile().mapData.GV_fire.stars;
            
            // mudslide
            case MapIconIdentfier.MS_logs:
                return StudentInfoSystem.GetCurrentProfile().mapData.MS_logs.stars;
            case MapIconIdentfier.MS_pond:
                return StudentInfoSystem.GetCurrentProfile().mapData.MS_pond.stars;
            case MapIconIdentfier.MS_ramp:
                return StudentInfoSystem.GetCurrentProfile().mapData.MS_ramp.stars;
            case MapIconIdentfier.MS_tower:
                return StudentInfoSystem.GetCurrentProfile().mapData.MS_tower.stars;

            // orc village
            case MapIconIdentfier.OV_houseL:
                return StudentInfoSystem.GetCurrentProfile().mapData.OV_houseL.stars;
            case MapIconIdentfier.OV_houseS:
                return StudentInfoSystem.GetCurrentProfile().mapData.OV_houseS.stars;
            case MapIconIdentfier.OV_statue:
                return StudentInfoSystem.GetCurrentProfile().mapData.OV_statue.stars;
            case MapIconIdentfier.OV_fire:
                return StudentInfoSystem.GetCurrentProfile().mapData.OV_fire.stars;

            // spooky village
            case MapIconIdentfier.SF_lamp:
                return StudentInfoSystem.GetCurrentProfile().mapData.SF_lamp.stars;
            case MapIconIdentfier.SF_shrine:
                return StudentInfoSystem.GetCurrentProfile().mapData.SF_shrine.stars;
            case MapIconIdentfier.SF_spider:
                return StudentInfoSystem.GetCurrentProfile().mapData.SF_spider.stars;
            case MapIconIdentfier.SF_web:
                return StudentInfoSystem.GetCurrentProfile().mapData.SF_web.stars;

            // orc camp
            case MapIconIdentfier.OC_axe:
                return StudentInfoSystem.GetCurrentProfile().mapData.OC_axe.stars;
            case MapIconIdentfier.OC_bigTent:
                return StudentInfoSystem.GetCurrentProfile().mapData.OC_bigTent.stars;
            case MapIconIdentfier.OC_smallTent:
                return StudentInfoSystem.GetCurrentProfile().mapData.OC_smallTent.stars;
            case MapIconIdentfier.OC_fire:
                return StudentInfoSystem.GetCurrentProfile().mapData.OC_fire.stars;

            // gorilla poop
            case MapIconIdentfier.GP_outhouse1:
                return StudentInfoSystem.GetCurrentProfile().mapData.GP_house1.stars;
            case MapIconIdentfier.GP_outhouse2:
                return StudentInfoSystem.GetCurrentProfile().mapData.GP_house2.stars;
            case MapIconIdentfier.GP_rocks1:
                return StudentInfoSystem.GetCurrentProfile().mapData.GP_rock1.stars;
            case MapIconIdentfier.GP_rocks2:
                return StudentInfoSystem.GetCurrentProfile().mapData.GP_rock2.stars;

            // windy cliff
            case MapIconIdentfier.WC_ladder:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_ladder.stars;
            case MapIconIdentfier.WC_lighthouse:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_lighthouse.stars;
            case MapIconIdentfier.WC_octo:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_octo.stars;
            case MapIconIdentfier.WC_rock:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_rock.stars;
            case MapIconIdentfier.WC_sign:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_sign.stars;
            case MapIconIdentfier.WC_statue:
                return StudentInfoSystem.GetCurrentProfile().mapData.WC_statue.stars;

            // pirate ship
            case MapIconIdentfier.PS_boat:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_boat.stars;
            case MapIconIdentfier.PS_bridge:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_bridge.stars;
            case MapIconIdentfier.PS_front:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_front.stars;
            case MapIconIdentfier.PS_parrot:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_parrot.stars;
            case MapIconIdentfier.PS_sail:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_sail.stars;
            case MapIconIdentfier.PS_wheel:
                return StudentInfoSystem.GetCurrentProfile().mapData.PS_wheel.stars;

            // mermaid beach
            case MapIconIdentfier.MB_bucket:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_bucket.stars;
            case MapIconIdentfier.MB_castle:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_castle.stars;
            case MapIconIdentfier.MB_ladder:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_ladder.stars;
            case MapIconIdentfier.MB_mermaids:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_mermaids.stars;
            case MapIconIdentfier.MB_rock:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_rock.stars;
            case MapIconIdentfier.MB_umbrella:
                return StudentInfoSystem.GetCurrentProfile().mapData.MB_umbrella.stars;

            // ruins
            case MapIconIdentfier.R_arch:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_arch.stars;
            case MapIconIdentfier.R_caveRock:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_caveRock.stars;
            case MapIconIdentfier.R_face:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_face.stars;
            case MapIconIdentfier.R_lizard1:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_lizard1.stars;
            case MapIconIdentfier.R_lizard2:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_lizard2.stars;
            case MapIconIdentfier.R_pyramid:
                return StudentInfoSystem.GetCurrentProfile().mapData.R_pyramid.stars;

            // exit jungle
            case MapIconIdentfier.EJ_bridge:
                return StudentInfoSystem.GetCurrentProfile().mapData.EJ_bridge.stars;
            case MapIconIdentfier.EJ_puppy:
                return StudentInfoSystem.GetCurrentProfile().mapData.EJ_puppy.stars;
            case MapIconIdentfier.EJ_sign:
                return StudentInfoSystem.GetCurrentProfile().mapData.EJ_sign.stars;
            case MapIconIdentfier.EJ_torch:
                return StudentInfoSystem.GetCurrentProfile().mapData.EJ_torch.stars;
            
            // gorilla study
            case MapIconIdentfier.GS_fire:
                return StudentInfoSystem.GetCurrentProfile().mapData.GS_fire.stars;
            case MapIconIdentfier.GS_statue:
                return StudentInfoSystem.GetCurrentProfile().mapData.GS_statue.stars;
            case MapIconIdentfier.GS_tent1:
                return StudentInfoSystem.GetCurrentProfile().mapData.GS_tent1.stars;
            case MapIconIdentfier.GS_tent2:
                return StudentInfoSystem.GetCurrentProfile().mapData.GS_tent2.stars;

            // monkeys
            case MapIconIdentfier.M_bananas:
                return StudentInfoSystem.GetCurrentProfile().mapData.M_bananas.stars;
            case MapIconIdentfier.M_flower:
                return StudentInfoSystem.GetCurrentProfile().mapData.M_flower.stars;
            case MapIconIdentfier.M_guards:
                return StudentInfoSystem.GetCurrentProfile().mapData.M_guards.stars;
            case MapIconIdentfier.M_tree:
                return StudentInfoSystem.GetCurrentProfile().mapData.M_tree.stars;
        }   
    }
}
